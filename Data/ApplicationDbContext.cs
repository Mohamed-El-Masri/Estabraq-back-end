using Microsoft.EntityFrameworkCore;
using EstabraqTourismAPI.Models;

namespace EstabraqTourismAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // Users and Authentication
    public DbSet<User> Users { get; set; }
    
    // Trip Management
    public DbSet<Category> Categories { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<TripImage> TripImages { get; set; }
    public DbSet<TripSchedule> TripSchedules { get; set; }
    public DbSet<TripIncluded> TripIncluded { get; set; }
    
    // Booking Management
    public DbSet<Booking> Bookings { get; set; }
    
    // Contact and Communication
    public DbSet<ContactMessage> ContactMessages { get; set; }
    
    // Content Management
    public DbSet<HeroSection> HeroSections { get; set; }
    public DbSet<SiteStats> SiteStats { get; set; }
    public DbSet<ContactInfo> ContactInfo { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("Customer");
            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NameAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DescriptionAr).HasMaxLength(1000);
            entity.Property(e => e.Icon).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Configure Trip entity
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TitleAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DescriptionAr).HasMaxLength(1000);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Duration).IsRequired();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(255);
            entity.Property(e => e.LocationAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MainImage).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            // Foreign key relationships
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Trips)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configure TripImage entity
        modelBuilder.Entity<TripImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.CaptionAr).HasMaxLength(255);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Trip)
                  .WithMany(t => t.Images)
                  .HasForeignKey(e => e.TripId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure TripSchedule entity
        modelBuilder.Entity<TripSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DayNumber).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TitleAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DescriptionAr).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Trip)
                  .WithMany(t => t.Schedule)
                  .HasForeignKey(e => e.TripId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure TripIncluded entity
        modelBuilder.Entity<TripIncluded>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Item).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ItemAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Icon).HasMaxLength(500);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Trip)
                  .WithMany(t => t.IncludedItems)
                  .HasForeignKey(e => e.TripId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure Booking entity
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NumberOfPeople).IsRequired();
            entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
            entity.Property(e => e.AdminNotes).HasMaxLength(1000);
            entity.Property(e => e.BookingDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.Trip)
                  .WithMany(t => t.Bookings)
                  .HasForeignKey(e => e.TripId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configure ContactMessage entity
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("New");
            entity.Property(e => e.AdminReply).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(e => e.RepliedByUser)
                  .WithMany(u => u.RepliedMessages)
                  .HasForeignKey(e => e.RepliedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configure HeroSection entity
        modelBuilder.Entity<HeroSection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TitleAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Subtitle).HasMaxLength(1000);
            entity.Property(e => e.SubtitleAr).HasMaxLength(1000);
            entity.Property(e => e.BackgroundImage).HasMaxLength(500);
            entity.Property(e => e.BackgroundVideo).HasMaxLength(500);
            entity.Property(e => e.ButtonText).HasMaxLength(255);
            entity.Property(e => e.ButtonTextAr).HasMaxLength(255);
            entity.Property(e => e.ButtonLink).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Configure SiteStats entity
        modelBuilder.Entity<SiteStats>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TitleAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Icon).HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Configure ContactInfo entity
        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CompanyNameAr).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.AddressAr).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.WhatsApp).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Facebook).HasMaxLength(500);
            entity.Property(e => e.Instagram).HasMaxLength(500);
            entity.Property(e => e.Twitter).HasMaxLength(500);
            entity.Property(e => e.YouTube).HasMaxLength(500);
            entity.Property(e => e.TikTok).HasMaxLength(500);
            entity.Property(e => e.WorkingHours).HasMaxLength(1000);
            entity.Property(e => e.WorkingHoursAr).HasMaxLength(1000);
            entity.Property(e => e.Logo).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
        
        // Seed initial data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default admin user and test users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "System Administrator",
                Email = "admin@estabraqtourism.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Name = "أحمد محمد",
                Email = "ahmed.mohamed@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Phone = "+966501234567",
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 3,
                Name = "فاطمة أحمد",
                Email = "fatima.ahmed@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Phone = "+966501234568",
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 4,
                Name = "محمد علي",
                Email = "mohammed.ali@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Phone = "+966501234569",
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        
        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Cultural Tours",
                NameAr = "الجولات الثقافية",
                Description = "Explore the rich cultural heritage and historical sites",
                DescriptionAr = "استكشف التراث الثقافي الغني والمواقع التاريخية",
                Icon = "fa-landmark",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 2,
                Name = "Adventure Tours",
                NameAr = "جولات المغامرة",
                Description = "Exciting adventure activities and outdoor experiences",
                DescriptionAr = "أنشطة مغامرة مثيرة وتجارب خارجية",
                Icon = "fa-mountain",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 3,
                Name = "Religious Tours",
                NameAr = "الجولات الدينية",
                Description = "Visit sacred places and religious landmarks",
                DescriptionAr = "زيارة الأماكن المقدسة والمعالم الدينية",
                Icon = "fa-mosque",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 4,
                Name = "Beach Tours",
                NameAr = "جولات الشواطئ",
                Description = "Relaxing beach and coastal experiences",
                DescriptionAr = "تجارب شاطئية وساحلية مريحة",
                Icon = "fa-umbrella-beach",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = 5,
                Name = "Desert Safari",
                NameAr = "سفاري الصحراء",
                Description = "Desert adventures and Bedouin experiences",
                DescriptionAr = "مغامرات صحراوية وتجارب بدوية",
                Icon = "fa-sun",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        
        // Seed sample trips
        modelBuilder.Entity<Trip>().HasData(
            new Trip
            {
                Id = 1,
                Title = "Riyadh Historical Tour",
                TitleAr = "جولة الرياض التاريخية",
                Description = "Explore the historical landmarks of Riyadh including Masmak Fortress and National Museum",
                DescriptionAr = "استكشف المعالم التاريخية في الرياض بما في ذلك قلعة المصمك والمتحف الوطني",
                Price = 299.99m,
                DiscountPrice = 249.99m,
                Duration = 6,
                Location = "Riyadh, Saudi Arabia",
                LocationAr = "الرياض، المملكة العربية السعودية",
                MaxParticipants = 25,
                MainImage = "riyadh-tour.jpg",
                CategoryId = 1,
                IsFeatured = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Trip
            {
                Id = 2,
                Title = "Jeddah Corniche Walk",
                TitleAr = "جولة كورنيش جدة",
                Description = "Beautiful coastal walk along Jeddah Corniche with stunning Red Sea views",
                DescriptionAr = "نزهة ساحلية جميلة على طول كورنيش جدة مع إطلالات خلابة على البحر الأحمر",
                Price = 199.99m,
                Duration = 4,
                Location = "Jeddah, Saudi Arabia",
                LocationAr = "جدة، المملكة العربية السعودية",
                MaxParticipants = 30,
                MainImage = "jeddah-corniche.jpg",
                CategoryId = 4,
                IsFeatured = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Trip
            {
                Id = 3,
                Title = "Empty Quarter Desert Safari",
                TitleAr = "سفاري الربع الخالي",
                Description = "Adventure through the largest sand desert in the world with camel riding and camping",
                DescriptionAr = "مغامرة عبر أكبر صحراء رملية في العالم مع ركوب الجمال والتخييم",
                Price = 599.99m,
                DiscountPrice = 499.99m,
                Duration = 24,
                Location = "Empty Quarter, Saudi Arabia",
                LocationAr = "الربع الخالي، المملكة العربية السعودية",
                MaxParticipants = 15,
                MainImage = "empty-quarter.jpg",
                CategoryId = 5,
                IsFeatured = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Trip
            {
                Id = 4,
                Title = "Al-Ula Archaeological Tour",
                TitleAr = "جولة العلا الأثرية",
                Description = "Discover the ancient Nabatean city of Hegra and stunning rock formations",
                DescriptionAr = "اكتشف مدينة الحجر النبطية القديمة والتشكيلات الصخرية المذهلة",
                Price = 799.99m,
                DiscountPrice = 699.99m,
                Duration = 8,
                Location = "Al-Ula, Saudi Arabia",
                LocationAr = "العلا، المملكة العربية السعودية",
                MaxParticipants = 20,
                MainImage = "alula-hegra.jpg",
                CategoryId = 1,
                IsFeatured = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Trip
            {
                Id = 5,
                Title = "Mecca Ziyarat Tour",
                TitleAr = "جولة زيارة مكة",
                Description = "Religious and historical tour around the holy city of Mecca",
                DescriptionAr = "جولة دينية وتاريخية حول المدينة المقدسة مكة",
                Price = 199.99m,
                DiscountPrice = 179.99m,
                Duration = 3,
                Location = "Mecca, Saudi Arabia",
                LocationAr = "مكة المكرمة، المملكة العربية السعودية",
                MaxParticipants = 35,
                MainImage = "mecca-ziyarat.jpg",
                CategoryId = 3,
                IsFeatured = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        
        // Seed sample bookings
        modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                Id = 1,
                UserId = 2,
                TripId = 1,
                BookingReference = "BK202501001",
                CustomerName = "أحمد محمد",
                CustomerEmail = "ahmed.mohamed@example.com",
                CustomerPhone = "+966501234567",
                NumberOfPeople = 2,
                TotalPrice = 499.98m,
                Status = "Confirmed",
                SpecialRequests = "سيارة إضافية للعائلة",
                BookingDate = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Booking
            {
                Id = 2,
                UserId = 3,
                TripId = 3,
                BookingReference = "BK202501002",
                CustomerName = "فاطمة أحمد",
                CustomerEmail = "fatima.ahmed@example.com",
                CustomerPhone = "+966501234568",
                NumberOfPeople = 1,
                TotalPrice = 499.99m,
                Status = "Pending",
                BookingDate = DateTime.UtcNow.AddDays(45),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Booking
            {
                Id = 3,
                UserId = 4,
                TripId = 2,
                BookingReference = "BK202501003",
                CustomerName = "محمد علي",
                CustomerEmail = "mohammed.ali@example.com",
                CustomerPhone = "+966501234569",
                NumberOfPeople = 3,
                TotalPrice = 599.97m,
                Status = "Confirmed",
                SpecialRequests = "وجبة نباتية",
                BookingDate = DateTime.UtcNow.AddDays(15),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        
        // Seed sample contact messages
        modelBuilder.Entity<ContactMessage>().HasData(
            new ContactMessage
            {
                Id = 1,
                Name = "عبدالله السعد",
                Email = "abdullah.alsaad@email.com",
                Phone = "+966501111111",
                Subject = "استفسار عن رحلة العلا",
                Message = "أرغب في الحصول على مزيد من التفاصيل حول رحلة العلا الأثرية، وما إذا كانت مناسبة للأطفال.",
                Status = "New",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ContactMessage
            {
                Id = 2,
                Name = "نورا أحمد",
                Email = "nora.ahmed@email.com",
                Phone = "+966502222222",
                Subject = "طلب تخصيص رحلة",
                Message = "هل يمكنكم تنظيم رحلة خاصة لمجموعة من 10 أشخاص إلى الطائف؟",
                Status = "Replied",
                AdminReply = "بالطبع يمكننا تنظيم رحلة خاصة لمجموعتكم. سيتواصل معكم فريق المبيعات خلال 24 ساعة.",
                RepliedByUserId = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new ContactMessage
            {
                Id = 3,
                Name = "ريم الزهراني",
                Email = "reem.alzahrani@email.com",
                Phone = "+966504444444",
                Subject = "تعديل الحجز",
                Message = "أحتاج إلى تغيير تاريخ حجزي من 15 سبتمبر إلى 20 سبتمبر.",
                Status = "New",
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                UpdatedAt = DateTime.UtcNow.AddHours(-6)
            }
        );
        
        // Seed default contact info
        modelBuilder.Entity<ContactInfo>().HasData(
            new ContactInfo
            {
                Id = 1,
                CompanyName = "Estabraq Tourism",
                CompanyNameAr = "استبرق للسياحة",
                Address = "Baghdad, Iraq",
                AddressAr = "بغداد، العراق",
                Phone = "+964 770 123 4567",
                WhatsApp = "+964 770 123 4567",
                Email = "info@estabraqtourism.com",
                Website = "https://estabraqtourism.com",
                WorkingHours = "Sunday - Thursday: 9:00 AM - 6:00 PM",
                WorkingHoursAr = "الأحد - الخميس: 9:00 صباحاً - 6:00 مساءً",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        
        // Seed default site stats
        modelBuilder.Entity<SiteStats>().HasData(
            new SiteStats
            {
                Id = 1,
                Title = "Happy Customers",
                TitleAr = "عملاء سعداء",
                Value = 1000,
                Color = "#4CAF50",
                IsActive = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SiteStats
            {
                Id = 2,
                Title = "Tours Completed",
                TitleAr = "جولات مكتملة",
                Value = 500,
                Color = "#2196F3",
                IsActive = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SiteStats
            {
                Id = 3,
                Title = "Years Experience",
                TitleAr = "سنوات خبرة",
                Value = 10,
                Color = "#FF9800",
                IsActive = true,
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new SiteStats
            {
                Id = 4,
                Title = "Destinations",
                TitleAr = "وجهات سياحية",
                Value = 50,
                Color = "#9C27B0",
                IsActive = true,
                SortOrder = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
