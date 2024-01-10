namespace Heinekamp.CodingChallenge.FileApi.Common.Configuration;

public class RetrySettings
{
    public int RetryCount { get; set; } = 3;
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 2;
    public int DurationOfBreakInSeconds  { get; set; } = 5;
}