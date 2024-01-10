using Heinekamp.CodingChallenge.FileApi.Authentication;
using Microsoft.OpenApi.Models;

namespace Heinekamp.CodingChallenge.FileApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiExplorerServices(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = AuthenticationConstants.ApiKeyHeaderName,
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });
            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };
            var requirement = new OpenApiSecurityRequirement
            {
                { scheme, new List<string>() }
            };
            o.AddSecurityRequirement(requirement);
        });

        return services;
    }
}