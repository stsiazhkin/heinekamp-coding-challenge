namespace Heinekamp.CodingChallenge.FileApi.Common.Models;

public class SearchParams
{
    public string? Search { get; set; }
    public string CurrentUser { get; set; } = null!;
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}