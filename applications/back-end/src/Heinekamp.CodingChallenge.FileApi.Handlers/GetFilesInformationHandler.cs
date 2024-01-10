using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Heinekamp.CodingChallenge.FileApi.Handlers;

public class GetFilesInformationHandler(
    IRepository<FileInformation> repository, 
    ILogger<GetFilesInformationHandler> logger) : IRequestHandler<GetFilesInformation, Result<List<FileInformation>>>
{
    public async Task<Result<List<FileInformation>>> Handle(GetFilesInformation request, CancellationToken cancellationToken)
    {
        var searchParams = new SearchParams
        {
            Search = request.Search,
            CurrentUser = request.CurrentUser,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        logger.LogDebug("Retrieving files information");
        var filesInformation = await repository.GetAsync(searchParams, cancellationToken);

        return Result<List<FileInformation>>.Success(filesInformation);
    }
}