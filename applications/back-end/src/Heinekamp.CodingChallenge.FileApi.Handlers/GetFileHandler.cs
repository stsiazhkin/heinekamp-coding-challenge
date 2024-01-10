using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Heinekamp.CodingChallenge.FileApi.Handlers;

public class GetFileHandler(
    IFileStorage fileStorage, 
    IRepository<FileInformation> repository, 
    ILogger<GetFileHandler> logger) : IRequestHandler<GetFile, Result<Uri>>
{
    public async Task<Result<Uri>> Handle(GetFile request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Retrieving file with id: '{fileId}'", request.FileId);
            var fileInformation = await repository.GetAsync(request.FileId, request.CurrentUser, cancellationToken);

            if (fileInformation is null)
                return Result<Uri>.Fail(ErrorMessage.FileNotFound);
        
            logger.LogDebug("Increment download count for the file with id: '{fileId}'", request.FileId);
            fileInformation.DownloadedCount = ++fileInformation.DownloadedCount;
            await repository.UpdateAsync(
                fileInformation,
                cancellationToken);
        
            logger.LogDebug("Preparing download URL for the file with id: '{fileId}'", request.FileId);
            var fileDownloadUrl = fileStorage.GetPreSignedDownloadUrl(request.FileId);

            return fileDownloadUrl;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to fetch the file with id: '{fileId}'", request.FileId);
            return Result<Uri>.Fail(e.Message, e);
        }
    }
}