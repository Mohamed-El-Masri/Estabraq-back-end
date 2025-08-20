namespace EstabraqTourismAPI.Configuration;

public class FileUploadSettings
{
    public long MaxFileSize { get; set; }
    public long MaxImageSizeBytes { get; set; }
    public long MaxVideoSizeBytes { get; set; }
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public string[] AllowedImageExtensions { get; set; } = Array.Empty<string>();
    public string[] AllowedVideoExtensions { get; set; } = Array.Empty<string>();
    public string UploadPath { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}
