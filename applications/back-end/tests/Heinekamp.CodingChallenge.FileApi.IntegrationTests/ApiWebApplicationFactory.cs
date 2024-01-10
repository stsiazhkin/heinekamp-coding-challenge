using System.Text.Json;
using Amazon.S3;
using Heinekamp.CodingChallenge.FileApi.Authentication;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.IntegrationTests.TestFileData;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Testcontainers.LocalStack;
using Testcontainers.PostgreSql;
using Xunit;

namespace Heinekamp.CodingChallenge.FileApi.IntegrationTests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public FileInformation[]? DbFilesInformation; 
    
    private readonly LocalStackContainer localStack = new LocalStackBuilder()
        .WithEnvironment("SERVICES", "s3")
        .WithEnvironment("DEBUG","${DEBUG:-0}")
        .Build();

    private readonly PostgreSqlContainer database = new PostgreSqlBuilder()
        .WithUsername("postgres")
        .WithPassword("example")
        .WithBindMount(ToAbsolutePath("/Setup/sql"), "/docker-entrypoint-initdb.d")
        .Build();
    
    public readonly JsonSerializerOptions SerializerOptions = new () { PropertyNameCaseInsensitive = true };
    
    public async Task InitializeAsync()
    {
        await database.StartAsync();
        await localStack.StartAsync();
        await CreateBucketAsync();
        DbFilesInformation = await PrepareTestDataDataAsync();
    }

    public new async Task DisposeAsync()
    {
        await database.StopAsync();
        await localStack.StopAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");
        
        var connectionString = database.GetConnectionString();
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(FileInformationDataBaseContext));
            services.AddDbContext<FileInformationDataBaseContext>(options =>
                options.UseNpgsql(connectionString));
            services.RemoveAll(typeof(IAmazonS3));
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(new AmazonS3Config
            {
                ServiceURL = localStack.GetConnectionString(),
                UseHttp = true,
                ForcePathStyle = true
            }));
        });
        
        base.ConfigureWebHost(builder);
    }

    private async Task CreateBucketAsync()
    {
        await localStack.ExecAsync(new[]
            { "sh", "-c", "awslocal --endpoint-url=http://localhost:4566 s3 mb s3://files-bucket" });
    }
    
    private async Task<FileInformation[]> PrepareTestDataDataAsync()
    {
        using var scope = Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<FileInformation>>();
        var configuration = scope.ServiceProvider.GetService<IConfiguration>();
        var userKeyValuePairs = configuration!.GetSection(nameof(ExternalApiUsers)).Get<ExternalApiUsers>()!;

        var file = TestFormFile.GetMock().Object;
        var fileId = new Guid("4147d5d2-f1ce-4452-b70e-b33e4097f72e");
        
        var item = new FileInformation
        {
            FileId = fileId,
            FileName = file.FileName,
            DownloadedCount = 0,
            UploadedOn = DateTimeOffset.UtcNow,
            UploadedBy = userKeyValuePairs.First().Value
        };
        
        await repository.CreateAsync(item, CancellationToken.None);
        
        var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorage>();

        await fileStorage.UploadAsync(
            file.OpenReadStream(),
            file.ContentType,
            fileId,
            CancellationToken.None);

        return [ item ];
    }
    
    private static string ToAbsolutePath(string postfix)
    {
        var path = Directory.GetCurrentDirectory();
        var pathParts = path.Split(["/bin"], StringSplitOptions.None);
        var directory = pathParts[0];

        return $"{directory}{postfix}";
    }
}