using Heinekamp.CodingChallenge.FileApi.Common;

namespace Heinekamp.CodingChallenge.FileApi.Persistence;

public interface IFileStorage
{
    Task<Result> UploadAsync(
        Stream inputStream,
        string contentType,
        Guid key,
        CancellationToken cancellationToken);

    Result<Uri> GetPreSignedDownloadUrl(Guid key);
    
    Task<Result<byte[]>> DownloadFileAsync(Guid key, CancellationToken cancellationToken);
}