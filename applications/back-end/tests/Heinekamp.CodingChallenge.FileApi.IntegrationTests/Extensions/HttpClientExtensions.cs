using Heinekamp.CodingChallenge.FileApi.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heinekamp.CodingChallenge.FileApi.IntegrationTests.Extensions;

internal static class HttpClientExtensions
{
    internal static HttpClient AddApiKeyHeader(this HttpClient client, IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetService<IConfiguration>();
        var userKeyValuePairs = configuration!.GetSection(nameof(ExternalApiUsers)).Get<ExternalApiUsers>()!;
        client.DefaultRequestHeaders.Add(AuthenticationConstants.ApiKeyHeaderName, userKeyValuePairs.First().Key);

        return client;
    }
}