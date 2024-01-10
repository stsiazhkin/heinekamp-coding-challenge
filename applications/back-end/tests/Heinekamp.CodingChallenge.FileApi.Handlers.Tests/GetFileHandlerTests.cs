using FluentAssertions;
using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using Microsoft.Extensions.Logging;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Tests;

public class GetFileHandlerTests
{
    private readonly Mock<IFileStorage> fileStorage;
    private readonly Mock<IRepository<FileInformation>> repository;
    private readonly Mock<ILogger<GetFileHandler>> logger;
    
    private readonly GetFileHandler sut;

    public GetFileHandlerTests()
    {
        fileStorage = new Mock<IFileStorage>();
        repository = new Mock<IRepository<FileInformation>>();
        logger = new Mock<ILogger<GetFileHandler>>();
        
        sut = new GetFileHandler(fileStorage.Object, repository.Object, logger.Object);
    }

    [Fact]
    public async Task Given_GetFileRequestAndFileFound_Handle_Should_Return_DownloadUrl()
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileInformation());

        repository.Setup(x => x.UpdateAsync(
            It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()));

        var resultUrl = new Uri("https://google.com");
        fileStorage.Setup(x => x.GetPreSignedDownloadUrl(It.IsAny<Guid>()))
            .Returns(Result<Uri>.Success(resultUrl));

        var result = await sut.Handle(new GetFile(Guid.NewGuid(), "CurrentUser"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(resultUrl);
    }
    
    [Fact]
    public async Task Given_GetFileRequestAndFileNotFound_Handle_Should_Return_Fail()
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FileInformation)null!);

        var result = await sut.Handle(new GetFile(Guid.NewGuid(), "CurrentUser"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(ErrorMessage.FileNotFound);
    }
    
    [Fact]
    public async Task Given_GetFileRequestAndUpdateThrowsAnError_Handle_Should_Return_Fail()
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileInformation());

        var errorMessage = "error message";
        repository.Setup(x => x.UpdateAsync(
                It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception(errorMessage));

        var result = await sut.Handle(new GetFile(Guid.NewGuid(), "CurrentUser"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Exception.Should().NotBeNull();
    }
}