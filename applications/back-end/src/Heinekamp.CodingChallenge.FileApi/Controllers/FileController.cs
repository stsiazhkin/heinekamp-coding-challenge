using Heinekamp.CodingChallenge.FileApi.Authentication;
using Heinekamp.CodingChallenge.FileApi.Extensions;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heinekamp.CodingChallenge.FileApi.Controllers;

[Authorize(AuthenticationSchemes = AuthenticationConstants.ApiKeyHeaderName)]
[Route("files")]
public class FileController(IMediator mediator) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> UploadFileAsync(UploadFileRequest upload, CancellationToken cancellationToken)
    {
        var request = new UploadFile(upload.File, UserName);
        
        var result = await mediator.Send(request, cancellationToken);

        if (result.IsSuccess)
            return new CreatedResult(nameof(GetFileDownloadUrlAsync), result.Value);

        return HandleErrorResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetFilesInformationAsync(CancellationToken cancellationToken)
    {
        var request = new GetFilesInformation(UserName);

        var result = await mediator.Send(request, cancellationToken);
        
        if (result.IsSuccess)
            return new OkObjectResult(result.Value.ToApiResponse());

        return HandleErrorResult(result);
    }
    
    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFileDownloadUrlAsync([FromRoute]Guid fileId, CancellationToken cancellationToken)
    {
        var request = new GetFile(fileId, UserName);

        var result = await mediator.Send(request, cancellationToken);
        
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return HandleErrorResult(result);
    }
    
    [HttpGet("bulk-download")]
    public async Task<IActionResult> GetFilesDownloadUrlAsync([FromQuery]Guid[]? fileIds, CancellationToken cancellationToken)
    {
        if (fileIds is null || fileIds.Length == 0)
            return new BadRequestResult();
        
        var request = new GetFiles(fileIds, UserName);

        var result = await mediator.Send(request, cancellationToken);

        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return HandleErrorResult(result);
    }

    [HttpPost("bulk-download")]
    public async Task<IActionResult> PrepareBulkDownloadAsync(BulkDownloadRequest bulkDownload, CancellationToken cancellationToken)
    {
        var request = new PrepareBulkDownload(bulkDownload.FilesIds, UserName);

        var result = await mediator.Send(request, cancellationToken);

        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return HandleErrorResult(result);
    }
}