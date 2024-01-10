namespace Heinekamp.CodingChallenge.FileApi.Requests;

public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
}