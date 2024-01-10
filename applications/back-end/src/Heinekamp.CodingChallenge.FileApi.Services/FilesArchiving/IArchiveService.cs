namespace Heinekamp.CodingChallenge.FileApi.Services.FilesArchiving;

public interface IArchiveService
{
    Task<byte[]> CreateAsync(List<Tuple<string, byte[]>> filesToArchive, CancellationToken cancellationToken);
}