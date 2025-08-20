using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;
using AutoMapper;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Configuration;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.Mappings;
using EstabraqTourismAPI.DTOs.Category;

// Configure Serilog with detailed logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, 
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Bind configuration sections
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("SecuritySettings"));
    builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUploadSettings"));
    builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

    // Register settings as singletons for direct injection
    var jwtSettings = new JwtSettings();
    builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
    builder.Services.AddSingleton(jwtSettings);

    var emailSettings = new EmailSettings();
    builder.Configuration.GetSection("EmailSettings").Bind(emailSettings);
    builder.Services.AddSingleton(emailSettings);

    var securitySettings = new SecuritySettings();
    builder.Configuration.GetSection("SecuritySettings").Bind(securitySettings);
    builder.Services.AddSingleton(securitySettings);

    var fileUploadSettings = new FileUploadSettings();
    builder.Configuration.GetSection("FileUploadSettings").Bind(fileUploadSettings);
    builder.Services.AddSingleton(fileUploadSettings);

    var applicationSettings = new ApplicationSettings();
    builder.Configuration.GetSection("ApplicationSettings").Bind(applicationSettings);
    builder.Services.AddSingleton(applicationSettings);

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Configure Swagger/OpenAPI
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Estabraq Tourism API",
            Description = "API for Estabraq Tourism Management System",
            Contact = new OpenApiContact
            {
                Name = "Estabraq Tourism",
                Email = "info@estabraqtourism.com"
            }
        });

        // Add JWT Authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // Entity Framework
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

    // Add Authorization
    builder.Services.AddAuthorization();

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // CORS Configuration - Open for all origins
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("*");
        });

        // Additional policy for development/specific domains if needed
        options.AddPolicy("Development", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:4200", "https://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Register Services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ITripService, TripService>();
    builder.Services.AddScoped<IBookingService, BookingService>();
    builder.Services.AddScoped<IContactService, ContactService>();
    builder.Services.AddScoped<IContentService, ContentService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IFileUploadService, FileUploadService>();

    var app = builder.Build();

    // Custom exception handling middleware for detailed logging
    app.Use(async (context, next) =>
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception occurred. Request: {Method} {Path}", 
                context.Request.Method, context.Request.Path);
            
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "An internal server error occurred", details = ex.Message };
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    });

    // Additional CORS headers middleware (backup)
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With";
        
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }
        
        await next(context);
    });

    // Log application startup information
    Log.Information("Starting Estabraq Tourism API in {Environment} environment", app.Environment.EnvironmentName);
    
    // Test database connection at startup with detailed logging
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        Log.Information("Testing database connection to: {ConnectionString}", 
            context.Database.GetConnectionString()?.Replace("Password=1HG?@t6kF2", "Password=***").Replace("Password=W?r4xH8#3g=K", "Password=***"));
        
        var canConnect = await context.Database.CanConnectAsync();
        
        if (canConnect)
        {
            Log.Information("Database connection successful");
            
            // Check if database needs migration
            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Log.Information("Found {Count} pending migrations. Running migrations...", pendingMigrations.Count());
                    await context.Database.MigrateAsync();
                    Log.Information("Database migrations completed successfully");
                }
                else
                {
                    Log.Information("Database is up to date - no pending migrations");
                }
                
                // Test a simple query
                var categoryCount = await context.Categories.CountAsync();
                Log.Information("Database query test successful. Categories count: {Count}", categoryCount);
            }
            catch (Exception queryEx)
            {
                Log.Error(queryEx, "Database connected but query/migration failed: {Error}", queryEx.Message);
            }
        }
        else
        {
            Log.Error("Database connection test returned false");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database connection failed at startup. Error: {ErrorType} - {Message}. Inner Exception: {InnerException}", 
            ex.GetType().Name, ex.Message, ex.InnerException?.Message);
    }

    // Configure the HTTP request pipeline
    // Enable Swagger in all environments for API documentation
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Estabraq Tourism API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Estabraq Tourism API Documentation";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // Security Headers
    if (securitySettings.EnableHSTS)
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    // CORS - Must be before Authentication and Authorization
    app.UseCors("AllowAll");
    
    // Log CORS setup
    Log.Information("CORS configured to allow all origins, methods, and headers");

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Static Files (for uploaded files)
    app.UseStaticFiles();

    // Controllers
    app.MapControllers();

    // Root endpoint - redirect to Swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));

    // Simple ping endpoint (no database)
    app.MapGet("/api/ping", () => Results.Ok(new {
        Status = "API is running",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow,
        Message = "Server is responding normally"
    }));

    // Quick categories test (for debugging)
    app.MapGet("/api/test/quick", async (IServiceProvider services) =>
    {
        try
        {
            using var scope = services.CreateScope();
            var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
            
            var request = new GetCategoriesRequestDto { Page = 1, PageSize = 1 };
            var result = await categoryService.GetCategoriesAsync(request);
            
            return Results.Ok(new {
                Success = result.Success,
                Message = result.Message,
                HasData = result.Data != null,
                ItemCount = result.Data?.Items?.Count ?? 0,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Ok(new {
                Success = false,
                Error = ex.Message,
                ErrorType = ex.GetType().Name,
                Timestamp = DateTime.UtcNow
            });
        }
    });

    // Health Check Endpoint
    app.MapGet("/health", () => Results.Ok(new { 
        Status = "Healthy", 
        Timestamp = DateTime.UtcNow,
        Environment = app.Environment.EnvironmentName,
        Version = "1.0.0"
    }));

    // API Info Endpoint
    app.MapGet("/api", () => Results.Ok(new {
        Name = "Estabraq Tourism API",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        Documentation = "/swagger",
        Endpoints = new {
            Categories = "/api/categories",
            Trips = "/api/trips",
            Auth = "/api/auth",
            Bookings = "/api/bookings"
        }
    }));

    // Database Migration Endpoint (for manual migration trigger)
    app.MapPost("/api/migrate", async (IServiceProvider services) =>
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            Log.Information("Starting manual database migration...");
            
            // Get pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                Log.Information("Found {Count} pending migrations: {Migrations}", 
                    pendingMigrations.Count(), string.Join(", ", pendingMigrations));
                
                await context.Database.MigrateAsync();
                Log.Information("Database migrations completed successfully");
                
                return Results.Ok(new {
                    Status = "Success",
                    Message = "Database migrations completed successfully",
                    PendingMigrations = pendingMigrations.ToList(),
                    AppliedMigrations = appliedMigrations.ToList(),
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                Log.Information("No pending migrations found");
                return Results.Ok(new {
                    Status = "Already Up to Date",
                    Message = "Database is already up to date",
                    AppliedMigrations = appliedMigrations.ToList(),
                    Timestamp = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Migration failed: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            return Results.Json(new {
                Status = "Failed",
                Error = ex.Message,
                ErrorType = ex.GetType().Name,
                Timestamp = DateTime.UtcNow
            }, statusCode: 500);
        }
    });

    // Database Schema Info Endpoint
    app.MapGet("/api/database/info", async (IServiceProvider services) =>
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            
            // Try to check if tables exist
            var tablesExist = false;
            try
            {
                await context.Categories.Take(1).ToListAsync();
                tablesExist = true;
            }
            catch
            {
                tablesExist = false;
            }
            
            return Results.Ok(new {
                DatabaseExists = await context.Database.CanConnectAsync(),
                TablesExist = tablesExist,
                PendingMigrations = pendingMigrations.ToList(),
                AppliedMigrations = appliedMigrations.ToList(),
                PendingCount = pendingMigrations.Count(),
                AppliedCount = appliedMigrations.Count(),
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Json(new {
                Error = ex.Message,
                ErrorType = ex.GetType().Name,
                Timestamp = DateTime.UtcNow
            }, statusCode: 500);
        }
    });

    // Database Health Check Endpoint
    app.MapGet("/api/health/database", async (IServiceProvider services) =>
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var connectionString = context.Database.GetConnectionString();
            var maskedConnectionString = connectionString?.Replace("Password=1HG?@t6kF2", "Password=***");
            
            Log.Information("Testing database connection to: {ConnectionString}", maskedConnectionString);
            
            // Test 1: DNS Resolution
            string dnsResult = "Not tested";
            try
            {
                var addresses = await System.Net.Dns.GetHostAddressesAsync("db25711.public.databaseasp.net");
                dnsResult = $"Resolved to {addresses.Length} addresses: {string.Join(", ", addresses.Select(a => a.ToString()))}";
                Log.Information("DNS Resolution successful: {Result}", dnsResult);
            }
            catch (Exception dnsEx)
            {
                dnsResult = $"DNS Failed: {dnsEx.Message}";
                Log.Error("DNS Resolution failed: {Error}", dnsEx.Message);
            }
            
            // Test 2: Database Connection
            var canConnect = await context.Database.CanConnectAsync();
            Log.Information("Database connection test result: {CanConnect}", canConnect);
            
            if (canConnect)
            {
                // Test 3: Simple query
                var categoryCount = await context.Categories.CountAsync();
                Log.Information("Successfully retrieved category count: {Count}", categoryCount);
                
                return Results.Ok(new {
                    Status = "Healthy",
                    Database = "Connected",
                    DNS = dnsResult,
                    CategoryCount = categoryCount,
                    ConnectionString = maskedConnectionString,
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                Log.Warning("Database connection failed - CanConnect returned false");
                return Results.Json(new {
                    Status = "Unhealthy",
                    Database = "Connection Failed",
                    DNS = dnsResult,
                    ConnectionString = maskedConnectionString,
                    Error = "CanConnectAsync returned false",
                    Timestamp = DateTime.UtcNow
                }, statusCode: 503);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database health check failed with exception: {ExceptionType} - {Message}. Inner: {InnerException}", 
                ex.GetType().Name, ex.Message, ex.InnerException?.Message);
            return Results.Json(new {
                Status = "Unhealthy",
                Database = "Exception occurred",
                Error = ex.Message,
                ErrorType = ex.GetType().Name,
                InnerException = ex.InnerException?.Message,
                Timestamp = DateTime.UtcNow
            }, statusCode: 503);
        }
    });

    // Connection String Test Endpoint
    app.MapGet("/api/test/connections", async (IServiceProvider services, IConfiguration config) =>
    {
        var results = new List<object>();
        
        // Test default connection
        var defaultConn = config.GetConnectionString("DefaultConnection");
        var sslConn = config.GetConnectionString("DefaultConnectionSSL");
        
        // Test without SSL
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(defaultConn);
            
            using var context = new ApplicationDbContext(optionsBuilder.Options);
            var canConnect = await context.Database.CanConnectAsync();
            
            results.Add(new {
                Type = "No SSL",
                Connected = canConnect,
                ConnectionString = defaultConn?.Replace("Password=1HG?@t6kF2", "Password=***")
            });
        }
        catch (Exception ex)
        {
            results.Add(new {
                Type = "No SSL",
                Connected = false,
                Error = ex.Message,
                ConnectionString = defaultConn?.Replace("Password=1HG?@t6kF2", "Password=***")
            });
        }
        
        // Test with SSL if available
        if (!string.IsNullOrEmpty(sslConn))
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(sslConn);
                
                using var context = new ApplicationDbContext(optionsBuilder.Options);
                var canConnect = await context.Database.CanConnectAsync();
                
                results.Add(new {
                    Type = "SSL",
                    Connected = canConnect,
                    ConnectionString = sslConn?.Replace("Password=1HG?@t6kF2", "Password=***")
                });
            }
            catch (Exception ex)
            {
                results.Add(new {
                    Type = "SSL",
                    Connected = false,
                    Error = ex.Message,
                    ConnectionString = sslConn?.Replace("Password=1HG?@t6kF2", "Password=***")
                });
            }
        }
        
        return Results.Ok(new {
            Environment = app.Environment.EnvironmentName,
            Tests = results,
            Timestamp = DateTime.UtcNow
        });
    });

    // Simple database test endpoint
    app.MapGet("/api/test/db", async (IServiceProvider services) =>
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            var connectionString = context.Database.GetConnectionString();
            Log.Information("Using connection string: {ConnectionString}", connectionString);
            
            await context.Database.OpenConnectionAsync();
            Log.Information("Database connection opened successfully");
            
            var result = await context.Database.ExecuteSqlRawAsync("SELECT 1");
            Log.Information("Test query executed successfully: {Result}", result);
            
            return Results.Ok(new { 
                Status = "Success",
                Message = "Database connection and query successful",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database test failed: {ExceptionType} - {Message}. StackTrace: {StackTrace}", 
                ex.GetType().Name, ex.Message, ex.StackTrace);
            return Results.Problem($"Database test failed: {ex.Message}", statusCode: 500);
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    });

    Log.Information("Starting Estabraq Tourism API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
