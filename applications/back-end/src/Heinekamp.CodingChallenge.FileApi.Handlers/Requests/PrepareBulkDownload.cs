using Heinekamp.CodingChallenge.FileApi.Common;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Requests;

public record PrepareBulkDownload(Guid[] FilesIds, string CurrentUser) : IAuthorizedRequest<Result<Uri>>;