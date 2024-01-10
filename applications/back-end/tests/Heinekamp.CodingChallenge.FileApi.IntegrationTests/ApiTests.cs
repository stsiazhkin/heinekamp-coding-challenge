using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using Heinekamp.CodingChallenge.FileApi.IntegrationTests.Extensions;
using Heinekamp.CodingChallenge.FileApi.IntegrationTests.TestFileData;
using Heinekamp.CodingChallenge.FileApi.Responses;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Heinekamp.CodingChallenge.FileApi.IntegrationTests;

public class ApiTests(ApiWebApplicationFactory factory) 
    : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task Given_ExistingFileId_GetFileDownloadUrlAsync_Should_return_DownloadUrl()
    {
        //Given
        var dbFileInformation = factory.DbFilesInformation![0];
        var initialDownloadCount = dbFileInformation.DownloadedCount;
        
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);

        //When
        var response = await client.GetAsync($"/files/{dbFileInformation.FileId}");
        var filesInformationResponse = await client.GetAsync("/files");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        //Then
        responseContent.Should().NotBeNull();
        var parsedSuccessfully = Uri.TryCreate(responseContent, UriKind.RelativeOrAbsolute, out var resultUrl);
        parsedSuccessfully.Should().BeTrue();
        resultUrl.Should().NotBeNull();

        var filesInformationResponseContent = await filesInformationResponse.Content.ReadAsStreamAsync();
        var filesInformation = JsonSerializer
            .Deserialize<FileInformationResponse>(filesInformationResponseContent, factory.SerializerOptions);

        var fileInformation = filesInformation!.Items.First(x => x.FileId == dbFileInformation.FileId);
        fileInformation.DownloadedCount.Should().BeGreaterThan(initialDownloadCount);
    }
    
    [Fact]
    public async Task Given_NonExistingFileId_GetFileDownloadUrlAsync_Should_return_404()
    {
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);

        var response = await client.GetAsync($"/files/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Given_requestWithoutApiKey_GetFileDownloadUrlAsync_Should_return_401()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/files/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Given_request_GetFilesInformationAsync_Should_return_Ok_response()
    {
        //Given
        var dbFilesInformation = factory.DbFilesInformation!;
        
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);

        //When
        var response = await client.GetAsync("/files/");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        //Then
        responseContent.Should().NotBeNull();
        
        var filesInformation = JsonSerializer
            .Deserialize<FileInformationResponse>(responseContent, factory.SerializerOptions);

        filesInformation.Should().NotBeNull();
        filesInformation!.Items.Should().NotBeEmpty();
        filesInformation.Items.Count.Should().Be(dbFilesInformation.Length);
        var resultFileInformation = filesInformation.Items[0];
        var dbFileInformation = dbFilesInformation[0];
        resultFileInformation.FileId.Should().Be(dbFileInformation.FileId);
        resultFileInformation.FileName.Should().Be(dbFileInformation.FileName);
        resultFileInformation.UploadedOn.Should().Be(dbFileInformation.UploadedOn.ToString("dd-MM-yyyy HH:mm:ss"));
    }
    
    [Fact]
    public async Task Given_requestWithoutApiKey_GetFilesInformationAsync_Should_return_401()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/files/");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Given_validRequest_UploadFileAsync_Should_return_201()
    {
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);
        var requestPayload = TestFormFile.GetMock().Object;
        var content = await CreatePostRequestContentAsync(requestPayload);
        
        var response = await client.PostAsync("/files", content);

        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        responseContent.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Given_requestWithoutApiKey_UploadFileAsync_Should_return_401()
    {
        using var client = factory.CreateClient();
        var requestPayload = TestFormFile.GetMock().Object;
        var content = await CreatePostRequestContentAsync(requestPayload);
        
        var response = await client.PostAsync("/files", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Given_requestInvalid_UploadFileAsync_Should_return_400()
    {
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);
        var requestPayload = TestFormFile.GetMock(
            "fileName.invalid",
            "47234238", 
            "application/invalidMimeType").Object;
        var content = await CreatePostRequestContentAsync(requestPayload);
        
        var response = await client.PostAsync("/files", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Given_ExistingFileIds_GetFilesDownloadUrlAsync_Should_return_DownloadUrl()
    {
        var dbFilesInformation = factory.DbFilesInformation!;
        
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);

        var response = await client.GetAsync(
            $"/files/bulk-download/?fileIds={dbFilesInformation[0].FileId}&fileIds={Guid.NewGuid()}");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        
        responseContent.Should().NotBeNull();
        var parsedSuccessfully = Uri.TryCreate(responseContent, UriKind.RelativeOrAbsolute, out var resultUrl);
        parsedSuccessfully.Should().BeTrue();
        resultUrl.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Given_NonExistingFileIds_GetFilesDownloadUrlAsync_Should_return_NotFound()
    {
        using var client = factory.CreateClient();
        client.AddApiKeyHeader(factory.Services);

        var response = await client.GetAsync(
            $"/files/bulk-download/?fileIds={Guid.NewGuid()}&fileIds={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Given_ExistingFileIdsWithoutApiKey_GetFilesDownloadUrlAsync_Should_return_NotFound()
    {
        var dbFilesInformation = factory.DbFilesInformation!;
        
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/files/bulk-download/?fileIds={dbFilesInformation[0].FileId}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    private static async Task<MultipartFormDataContent> CreatePostRequestContentAsync(IFormFile formFile)
    {
        using var streamContent = new StreamContent(formFile.OpenReadStream());
        streamContent.Headers.ContentLength = formFile.Length;
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

        var formContent = new MultipartFormDataContent();
        formContent.Headers.ContentType!.MediaType = "multipart/form-data";
        formContent.Add(streamContent, "File", formFile.FileName);

        await formContent.LoadIntoBufferAsync();

        return formContent;
    }
}