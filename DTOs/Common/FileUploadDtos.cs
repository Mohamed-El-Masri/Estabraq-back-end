namespace EstabraqTourismAPI.DTOs.Common;

public class FileUploadResultDto
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class FileInfoDto
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool Exists { get; set; }
}

public class UploadStatisticsDto
{
    public int TotalFiles { get; set; }
    public long TotalSize { get; set; }
    public int ImagesCount { get; set; }
    public int TripImagesCount { get; set; }
    public int CategoryImagesCount { get; set; }
    public int ProfileImagesCount { get; set; }
    public DateTime LastUpload { get; set; }
    public string FormattedTotalSize { get; set; } = string.Empty;
}

public class FileCleanupResultDto
{
    public int FilesDeleted { get; set; }
    public long SpaceFreed { get; set; }
    public List<string> DeletedFiles { get; set; } = new();
    public string FormattedSpaceFreed { get; set; } = string.Empty;
    public DateTime CleanupDate { get; set; }
}
