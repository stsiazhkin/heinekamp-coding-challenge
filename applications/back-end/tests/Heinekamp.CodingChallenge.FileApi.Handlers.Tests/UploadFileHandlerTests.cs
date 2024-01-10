using FluentAssertions;
using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MimeTypes;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Tests;

public class UploadFileHandlerTests
{
    private readonly Mock<IFileStorage> fileStorage;
    private readonly Mock<IRepository<FileInformation>> repository;
    private readonly Mock<IMediator> mediator;
    private readonly Mock<ILogger<UploadFileHandler>> logger;
    
    private readonly UploadFileHandler sut;

    public UploadFileHandlerTests()
    {
        fileStorage = new Mock<IFileStorage>();
        repository = new Mock<IRepository<FileInformation>>();
        mediator = new Mock<IMediator>();
        logger = new Mock<ILogger<UploadFileHandler>>();
        
        sut = new UploadFileHandler(fileStorage.Object, repository.Object, mediator.Object, logger.Object);
    }

    [Fact]
    public async Task Given_UploadFileRequest_AndSuccess_Handle_Should_Return_FileId()
    {
        var formFile = await PrepareFormFileAsync();

        var request = new UploadFile(formFile, "CurrentUser");

        fileStorage.Setup(x => x.UploadAsync(
                It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);

        repository.Setup(x => x.CreateAsync(
            It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()));

        var result = await sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(request.FileId);
    }
    
    [Fact]
    public async Task Given_UploadFileRequest_FileStorageFais_Handle_Should_Return_Fail()
    {
        var formFile = await PrepareFormFileAsync();

        var request = new UploadFile(formFile, "CurrentUser");

        fileStorage.Setup(x => x.UploadAsync(
                It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("error", new Exception("error message")));

        var result = await sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(ErrorMessage.ErrorUploadingFile);
        result.Exception.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Given_UploadFileRequest_RepositoryThrows_Handle_Should_Return_Fail()
    {
        var formFile = await PrepareFormFileAsync();

        var request = new UploadFile(formFile, "CurrentUser");

        fileStorage.Setup(x => x.UploadAsync(
                It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);

        var errorMessage = "error message";
        repository.Setup(x => x.CreateAsync(
            It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception(errorMessage));

        var result = await sut.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Exception.Should().NotBeNull();
    }
    
    private async Task<IFormFile> PrepareFormFileAsync()
    {
        var formFile = new Mock<IFormFile>();
        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        
        await writer.WriteAsync("content");
        await writer.FlushAsync();
        stream.Position = 0;
        var fileName = "fileName.txt";
        
        formFile.Setup(p => p.FileName).Returns(fileName);
        formFile.Setup(p => p.ContentType).Returns(MimeTypeMap.GetMimeType(Path.GetExtension(fileName)));
        formFile.Setup(p => p.Length).Returns(stream.Length);
        formFile.Setup(p => p.OpenReadStream()).Returns(stream);

        return formFile.Object;
    }
}