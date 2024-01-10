using Heinekamp.CodingChallenge.FileApi.Services.Configuration;
using Heinekamp.CodingChallenge.FileApi.Services.FilesArchiving;
using Heinekamp.CodingChallenge.FileApi.Services.FileThumbnails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heinekamp.CodingChallenge.FileApi.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageThumbnailServices(this IServiceCollection services, IConfiguration configuration)
    {
        var thumbnailImageSettings = configuration.GetSection(nameof(ThumbnailImageSettings)).Get<ThumbnailImageSettings>();

        if (thumbnailImageSettings is null)
            throw new ArgumentNullException(nameof(thumbnailImageSettings));

        services.AddSingleton(thumbnailImageSettings);

        services.AddScoped<IThumbnailFactory, ThumbnailFactoryStub>();

        return services;
    }
    
    public static IServiceCollection AddArchiveServices(this IServiceCollection services)
    {
        services.AddScoped<IArchiveService, ArchiveService>();

        return services;
    }
}