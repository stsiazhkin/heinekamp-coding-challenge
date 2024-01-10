using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Notifications;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using Heinekamp.CodingChallenge.FileApi.Services.Configuration;
using Heinekamp.CodingChallenge.FileApi.Services.FileThumbnails;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Heinekamp.CodingChallenge.FileApi.Handlers;

/// <summary>
/// We pretend this is a separate service. Likely AWS Lambda or Azure function
/// triggered by AWS SQS/Azure Storage Queues
/// It will genereate an Image Thumbnail for a given uploaded file and store it in
/// a Storage of a choice.
/// </summary>
public class GenerateThumbnailHandler(
    IThumbnailFactory thumbnailFactory,
    IRepository<FileInformation> repository,
    ThumbnailImageSettings thumbnailImageSettings,
    ILogger<GenerateThumbnailHandler> logger) : INotificationHandler<FileUploaded>
{
    public async Task Handle(FileUploaded notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Retrieving file information with id:'{fileId}'", notification.FileId);
            var fileInformation = await repository.GetAsync(notification.FileId, notification.CurrentUser, cancellationToken);
            if (fileInformation is null)
            {
                logger.LogError("File information for the file with id: '{fileId}' not found in Database",
                    notification.FileId);
                return;
            }
        
            logger.LogDebug("Generate a thumbnail for file with id: '{fileId}'", notification.FileId);
            var thumbnail = await thumbnailFactory.CreateThumbnailImageAsync(
                notification.File,
                thumbnailImageSettings.ThumbnailImageWidth, 
                thumbnailImageSettings.ThumbnailImageHeight,
                cancellationToken);
        
            logger.LogDebug("Updating file information for the file with id: '{fileId}' with a thumbnail", 
                notification.FileId);
            fileInformation.ThumbnailImage = thumbnail;
            await repository.UpdateAsync(fileInformation, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Failed to generate a thumbnail for file with id: '{fileId}'. Reason: {eMessage}", 
                notification.FileId, e.Message);
        }
    }
}