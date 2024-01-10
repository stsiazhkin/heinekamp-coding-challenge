using Microsoft.AspNetCore.Http;

namespace Heinekamp.CodingChallenge.FileApi.Services.FileThumbnails;

/// <summary>
/// A variety of getting an image thumbnail based on file type.
/// This is quite a task on its own. 
/// </summary>
/// <param name="file"></param>
/// <param name="thumbWidth"></param>
/// <param name="thumbHeight"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public interface IThumbnailFactory
{
    Task<byte[]> CreateThumbnailImageAsync(IFormFile file, int thumbWidth, int thumbHeight, CancellationToken cancellationToken);
}