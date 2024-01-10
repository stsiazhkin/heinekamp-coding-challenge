namespace Heinekamp.CodingChallenge.FileApi.Responses;

public class FileInformationResponse
{
    public required List<FileInformationItem> Items { get; set; }
}

public class FileInformationItem
{
    public Guid FileId { get; set; }
    public string? FileName { get; set; }
    public long DownloadedCount { get; set; }
    public string?  UploadedOn { get; set; }
    public byte[]? ThumbnailImage { get; set; }
    public string? FileContentType { get; set; }
}