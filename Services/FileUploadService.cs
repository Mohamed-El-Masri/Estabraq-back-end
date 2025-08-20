using EstabraqTourismAPI.Configuration;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface IFileUploadService
{
    Task<ApiResponse<FileUploadResult>> UploadImageAsync(IFormFile file, string folder = "images");
    Task<ApiResponse<FileUploadResult>> UploadVideoAsync(IFormFile file, string folder = "videos");
    Task<ApiResponse<string>> DeleteFileAsync(string filePath);
    Task<ApiResponse<List<FileUploadResult>>> UploadMultipleImagesAsync(IList<IFormFile> files, string folder = "images");
}

public class FileUploadService : IFileUploadService
{
    private readonly FileUploadSettings _uploadSettings;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(
        FileUploadSettings uploadSettings,
        IWebHostEnvironment environment,
        ILogger<FileUploadService> logger)
    {
        _uploadSettings = uploadSettings;
        _environment = environment;
        _logger = logger;
    }

    public async Task<ApiResponse<FileUploadResult>> UploadImageAsync(IFormFile file, string folder = "images")
    {
        try
        {
            var validationResult = ValidateImageFile(file);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            return await SaveFileAsync(file, folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image file {FileName}", file.FileName);
            return ApiResponse<FileUploadResult>.FailureResult("An error occurred while uploading the image");
        }
    }

    public async Task<ApiResponse<FileUploadResult>> UploadVideoAsync(IFormFile file, string folder = "videos")
    {
        try
        {
            var validationResult = ValidateVideoFile(file);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            return await SaveFileAsync(file, folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading video file {FileName}", file.FileName);
            return ApiResponse<FileUploadResult>.FailureResult("An error occurred while uploading the video");
        }
    }

    public async Task<ApiResponse<List<FileUploadResult>>> UploadMultipleImagesAsync(IList<IFormFile> files, string folder = "images")
    {
        try
        {
            var results = new List<FileUploadResult>();
            var errors = new List<string>();

            foreach (var file in files)
            {
                var validationResult = ValidateImageFile(file);
                if (!validationResult.Success)
                {
                    errors.Add($"{file.FileName}: {validationResult.Message}");
                    continue;
                }

                var uploadResult = await SaveFileAsync(file, folder);
                if (uploadResult.Success && uploadResult.Data != null)
                {
                    results.Add(uploadResult.Data);
                }
                else
                {
                    errors.Add($"{file.FileName}: {uploadResult.Message}");
                }
            }

            if (errors.Any() && !results.Any())
            {
                return ApiResponse<List<FileUploadResult>>.FailureResult("Failed to upload any files", errors);
            }

            var message = errors.Any() 
                ? $"Uploaded {results.Count} files successfully. {errors.Count} files failed."
                : $"All {results.Count} files uploaded successfully";

            return ApiResponse<List<FileUploadResult>>.SuccessResult(results, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading multiple image files");
            return ApiResponse<List<FileUploadResult>>.FailureResult("An error occurred while uploading images");
        }
    }

    public async Task<ApiResponse<string>> DeleteFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return ApiResponse<string>.FailureResult("File path is required");
            }

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                _logger.LogInformation("File deleted: {FilePath}", filePath);
                return ApiResponse<string>.SuccessResult("", "File deleted successfully");
            }

            return ApiResponse<string>.FailureResult("File not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            return ApiResponse<string>.FailureResult("An error occurred while deleting the file");
        }
    }

    private ApiResponse<FileUploadResult> ValidateImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ApiResponse<FileUploadResult>.FailureResult("No file provided");
        }

        if (file.Length > _uploadSettings.MaxImageSizeBytes)
        {
            return ApiResponse<FileUploadResult>.FailureResult(
                $"File size exceeds maximum allowed size of {_uploadSettings.MaxImageSizeBytes / (1024 * 1024)} MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = _uploadSettings.AllowedImageExtensions;

        if (!allowedExtensions.Contains(extension))
        {
            return ApiResponse<FileUploadResult>.FailureResult(
                $"File type not allowed. Allowed types: {string.Join(", ", _uploadSettings.AllowedImageExtensions)}");
        }

        return ApiResponse<FileUploadResult>.SuccessResult(new FileUploadResult { Success = true });
    }

    private ApiResponse<FileUploadResult> ValidateVideoFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ApiResponse<FileUploadResult>.FailureResult("No file provided");
        }

        if (file.Length > _uploadSettings.MaxVideoSizeBytes)
        {
            return ApiResponse<FileUploadResult>.FailureResult(
                $"File size exceeds maximum allowed size of {_uploadSettings.MaxVideoSizeBytes / (1024 * 1024)} MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = _uploadSettings.AllowedVideoExtensions;

        if (!allowedExtensions.Contains(extension))
        {
            return ApiResponse<FileUploadResult>.FailureResult(
                $"File type not allowed. Allowed types: {string.Join(", ", _uploadSettings.AllowedVideoExtensions)}");
        }

        return ApiResponse<FileUploadResult>.SuccessResult(new FileUploadResult { Success = true });
    }

    private async Task<ApiResponse<FileUploadResult>> SaveFileAsync(IFormFile file, string folder)
    {
        try
        {
            var uploadPath = Path.Combine(_environment.WebRootPath, _uploadSettings.UploadPath, folder);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate URL
            var relativePath = $"/{_uploadSettings.UploadPath}/{folder}/{fileName}".Replace('\\', '/');
            var fileUrl = $"{_uploadSettings.BaseUrl?.TrimEnd('/')}{relativePath}";

            var result = new FileUploadResult
            {
                Success = true,
                FileName = fileName,
                FilePath = relativePath,
                FileUrl = fileUrl,
                FileSize = file.Length
            };

            _logger.LogInformation("File uploaded successfully: {FileName}", fileName);
            return ApiResponse<FileUploadResult>.SuccessResult(result, "File uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName}", file.FileName);
            return ApiResponse<FileUploadResult>.FailureResult("An error occurred while saving the file");
        }
    }
}
