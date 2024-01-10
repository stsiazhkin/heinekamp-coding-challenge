using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Heinekamp.CodingChallenge.FileApi.Common.Configuration;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heinekamp.CodingChallenge.FileApi.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS3Services(this IServiceCollection services, IConfiguration configuration)
    {
        var awsOptions = configuration.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);

        var localStackSettings = configuration.GetSection(nameof(LocalStackSettings)).Get<LocalStackSettings>();

        if (localStackSettings is null)
            throw new ArgumentNullException(nameof(localStackSettings));
        
        if (localStackSettings.Enabled)
        {
            //To prevent "Unable to get IAM security credentials from EC2 Instance Metadata Service"
            //in some docker-compose setups
            var awsCredentials = new BasicAWSCredentials("xxx", "xxx");
            
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsCredentials, new AmazonS3Config
            {
                ServiceURL = awsOptions.DefaultClientConfig.ServiceURL,
                UseHttp = true,
                ForcePathStyle = true
            }));
        }
        else
        {
            services.AddAWSService<IAmazonS3>();
        }
        
        services.AddSingleton<ITransferUtility, TransferUtility>();
        
        var s3StorageSettings = configuration.GetSection(nameof(S3StorageSettings)).Get<S3StorageSettings>();

        if(s3StorageSettings is null)
            throw new ArgumentNullException(nameof(S3StorageSettings));
        
        services.AddSingleton(s3StorageSettings);

        services.AddSingleton<IFileStorage, S3FileStorage>();

        return services;
    }
    
    public static IServiceCollection AddFileInformationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var isNotTestEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "test";
        
        if (isNotTestEnv)
        {
            services.AddDbContext<FileInformationDataBaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));    
        }
        
        services.AddScoped<IRepository<FileInformation>, FileInformationSqlRepository>();
        
        return services;
    }
}