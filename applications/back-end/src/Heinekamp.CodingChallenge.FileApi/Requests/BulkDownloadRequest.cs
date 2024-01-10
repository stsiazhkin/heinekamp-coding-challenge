namespace Heinekamp.CodingChallenge.FileApi.Requests;

public class BulkDownloadRequest
{
    public Guid[] FilesIds { get; set; } = [];
}