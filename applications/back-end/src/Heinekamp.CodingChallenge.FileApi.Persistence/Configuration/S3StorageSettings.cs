namespace Heinekamp.CodingChallenge.FileApi.Persistence.Configuration;

public class S3StorageSettings
{
    public string Bucket { get; set; } = null!;
    public double DownloadLinkExpirationInMinutes { get; set; }
}