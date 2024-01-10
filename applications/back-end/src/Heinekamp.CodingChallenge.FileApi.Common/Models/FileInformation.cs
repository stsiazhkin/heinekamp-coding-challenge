namespace Heinekamp.CodingChallenge.FileApi.Common.Models;

    public class FileInformation
    {
        public Guid FileId { get; set; }
        public string? FileName { get; set; }
        public long DownloadedCount { get; set; }
        public DateTimeOffset UploadedOn { get; set; }
        public byte[]? ThumbnailImage { get; set; }
        public string? FileContentType { get; set; }
        public string? UploadedBy { get; set; }
    }