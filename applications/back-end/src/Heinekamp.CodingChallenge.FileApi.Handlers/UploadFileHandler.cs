using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Notifications;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Heinekamp.CodingChallenge.FileApi.Handlers;

public class UploadFileHandler(
    IFileStorage fileStorage, 
    IRepository<FileInformation> repository,
    IMediator mediator,
    ILogger<UploadFileHandler> logger) : IRequestHandler<UploadFile, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UploadFile request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Uploading file with id: '{fileId}', name: '{fileName}'", 
                request.FileId, request.File.Name);
            var uploadResult = await fileStorage.UploadAsync(
                request.File.OpenReadStream(), 
                request.File.ContentType, 
                request.FileId,
                cancellationToken);

            if (!uploadResult.IsSuccess)
                return Result<Guid>.Fail(ErrorMessage.ErrorUploadingFile, uploadResult.Exception);
        
            logger.LogDebug("Uploading file information with id: '{fileId}', name: '{fileName}'", 
                request.FileId, request.File.Name);
            await repository.CreateAsync(new FileInformation
                {
                    FileId = request.FileId,
                    FileName = request.File.FileName,
                    FileContentType = request.File.ContentType,
                    UploadedOn = DateTimeOffset.UtcNow,
                    UploadedBy = request.CurrentUser
                },
                cancellationToken);

            //Generating images thumbnails can be a separate task performed by a separate service
            //like AWS Lambda or Azure function
            //Image thumbnail can be a separate entity with relation to FileInformation
            //Therefore we emit an event here so that a dedicated service can be triggered by it
            await PublishFileUploadedEventAsync(request, cancellationToken);

            return Result<Guid>.Success(request.FileId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to upload the file  with id: '{fileId}', name: '{fileName}'", 
                request.FileId, request.File.Name);
            return Result<Guid>.Fail(e.Message, e);
        }
    }

    //Mediatr only for demo purpose to give the idea
    
    //This will be a message published to AWS SQS/Azure Storage Queues or any other
    //we use tell dont ask principle and making our architecture more reactive, event driven,
    //decoupling components helping with scalability
    private async Task PublishFileUploadedEventAsync(UploadFile request, CancellationToken cancellationToken)
    {
        var fileUploadedNotification = new FileUploaded(request.File, request.FileId, request.CurrentUser);
        await mediator.Publish(fileUploadedNotification, cancellationToken);
    }
}