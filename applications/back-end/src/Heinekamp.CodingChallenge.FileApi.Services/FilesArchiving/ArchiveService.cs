using System.IO.Compression;

namespace Heinekamp.CodingChallenge.FileApi.Services.FilesArchiving;

public class ArchiveService : IArchiveService
{
    public async Task<byte[]> CreateAsync(List<Tuple<string, byte[]>> filesToArchive, CancellationToken cancellationToken)
    {
        byte[] archiveFile;
        using (var archiveStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in filesToArchive)
                {
                    var (fileName, fileBytes) = file;
                    
                    var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);

                    await using var zipStream = zipArchiveEntry.Open();
                    await zipStream.WriteAsync(fileBytes, cancellationToken);
                }
            }

            archiveFile = archiveStream.ToArray();
        }

        return archiveFile;
    }
}