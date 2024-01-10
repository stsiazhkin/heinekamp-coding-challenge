using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Models;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Requests;

public record GetFilesInformation(
    string CurrentUser, int PageNumber = 1, int PageSize = 50, string? Search = null) 
    : IAuthorizedRequest<Result<List<FileInformation>>>;