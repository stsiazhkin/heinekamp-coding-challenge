using Heinekamp.CodingChallenge.FileApi.Common;
using Microsoft.AspNetCore.Http;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Requests;

public record UploadFile(IFormFile File, string CurrentUser) : IAuthorizedRequest<Result<Guid>> 
{
    public Guid FileId { get; } = Guid.NewGuid();
}