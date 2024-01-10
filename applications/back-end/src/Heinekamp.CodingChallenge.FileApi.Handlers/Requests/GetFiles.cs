using Heinekamp.CodingChallenge.FileApi.Common;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Requests;

public record GetFiles(Guid[] FileIds, string CurrentUser) : IAuthorizedRequest<Result<Uri>>;