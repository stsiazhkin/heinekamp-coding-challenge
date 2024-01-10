using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Persistence.Configuration;
using Microsoft.Extensions.Logging;

namespace Heinekamp.CodingChallenge.FileApi.Persistence;

public class S3FileStorage(
    IAmazonS3 s3Client,
    ITransferUtility transferUtility,
    S3StorageSettings settings,
    ILogger<S3FileStorage> logger) : IFileStorage
{
    public async Task<Result> UploadAsync(Stream inputStream, string contentType, Guid key, CancellationToken cancellationToken)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = inputStream,
            ContentType = contentType,
            BucketName = settings.Bucket,
            Key = key.ToString(),
            AutoCloseStream = true
        };

        try
        {
            logger.LogDebug("Uploading {key} to S3", key);
            await transferUtility.UploadAsync(uploadRequest, cancellationToken);
            logger.LogDebug("File with key='{key}' uploaded to S3", key);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);

            return Result.Fail(ErrorMessage.ErrorUploadingFile, e);
        }
    }

    public Result<Uri> GetPreSignedDownloadUrl(Guid key)
    {
        var urlRequest = new GetPreSignedUrlRequest
        {
            BucketName = settings.Bucket,
            Key = key.ToString(),
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(settings.DownloadLinkExpirationInMinutes)),
            Verb = HttpVerb.GET,
            //Local stack for local environment needs HTTP
            Protocol = s3Client.Config.UseHttp ? Protocol.HTTP : Protocol.HTTPS
        };
        
        try
        {
            logger.LogDebug("Getting S3 presigned URL for {key}", key);
            var response = s3Client.GetPreSignedURL(urlRequest)!;
            logger.LogDebug("Pre-signed URL generated");

            return Result<Uri>.Success(new Uri(response));
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);

            return Result<Uri>.Fail(ErrorMessage.ErrorUploadingFile, e);
        }
    }

    public async Task<Result<byte[]>> DownloadFileAsync(Guid key, CancellationToken cancellationToken)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.Bucket,
            Key = key.ToString()
        };

        try
        {
            logger.LogDebug("Downloading {key} from S3", key);
            using var response = await s3Client.GetObjectAsync(request, cancellationToken);
            await using var responseStream = response.ResponseStream;
            using var memoryStream = new MemoryStream();
            await responseStream.CopyToAsync(memoryStream, cancellationToken);
            logger.LogDebug("Downloaded {key} from S3", key);
            
            return Result<byte[]>.Success(memoryStream.ToArray());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);

            return Result<byte[]>.Fail(ErrorMessage.ErrorDownloadingFile, e);
        }
    }
}