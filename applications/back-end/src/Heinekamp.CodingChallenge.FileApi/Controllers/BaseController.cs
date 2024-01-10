using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Heinekamp.CodingChallenge.FileApi.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected string UserName => User.Identity!.Name!;
    
    protected IActionResult HandleErrorResult(Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException($"{nameof(HandleErrorResult)} should only be called for Result.Fail");

        return GetErrorResponse(result);
    }

    protected IActionResult HandleErrorResult<T>(Result<T> result)
    {
        return HandleErrorResult(result as Result);
    }

    private IActionResult GetErrorResponse(Result result)
    {
        if(result.Exception is not null)
            return result.Exception switch
            {
                OperationCanceledException _ => StatusCode(StatusCodes.Status408RequestTimeout, result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Exception?.Message)
            };

        return result.ErrorMessage switch
        {
            ErrorMessage.FileNotFound =>  StatusCode(StatusCodes.Status404NotFound, result.ErrorMessage),
            ErrorMessage.NoFilesToDownload => StatusCode(StatusCodes.Status404NotFound, result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage)
        };
    }
}