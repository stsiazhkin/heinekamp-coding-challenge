using Heinekamp.CodingChallenge.FileApi.Common;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Requests;

public record GetFile(Guid FileId, string CurrentUser) : IAuthorizedRequest<Result<Uri>>;