using Heinekamp.CodingChallenge.FileApi.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Heinekamp.CodingChallenge.FileApi.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRetryPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        var retrySettings = configuration.GetSection(nameof(RetrySettings)).Get<RetrySettings>();

        if(retrySettings is null)
            throw new ArgumentNullException(nameof(retrySettings));
        
        var retryPolicy = Policy
            .Handle<Exception>()
            .Or<TimeoutException>()
            .RetryAsync(retrySettings.RetryCount);
        
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: retrySettings.ExceptionsAllowedBeforeBreaking, 
                durationOfBreak: TimeSpan.FromSeconds(retrySettings.DurationOfBreakInSeconds)
            );

        var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

        services.AddSingleton(policyWrap);

        return services;
    }
}