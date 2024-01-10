using Microsoft.AspNetCore.Authentication;

namespace Heinekamp.CodingChallenge.FileApi.Authentication;

public static class ApiKeyAuthenticationExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddAuthentication(AuthenticationConstants.ApiKeyHeaderName)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                AuthenticationConstants.ApiKeyHeaderName, null);
        
        var externalApiUsers = configuration.GetSection(nameof(ExternalApiUsers)).Get<ExternalApiUsers>();
        if (externalApiUsers is null)
            throw new ArgumentNullException(nameof(externalApiUsers));

        serviceCollection.AddSingleton(externalApiUsers);

        return serviceCollection;
    }
}