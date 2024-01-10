using AutoFixture.Xunit2;
using FluentAssertions;
using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using Heinekamp.CodingChallenge.FileApi.Services.FilesArchiving;
using Microsoft.Extensions.Logging;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Tests;

public class GetFilesHandlerTests
{
    private readonly Mock<IFileStorage> fileStorage;
    private readonly Mock<IRepository<FileInformation>> repository;
    private readonly Mock<IArchiveService> archiveService;
    private readonly Mock<ILogger<GetFilesHandler>> logger;
    
    private readonly GetFilesHandler sut;
    
    public GetFilesHandlerTests()
    {
        fileStorage = new Mock<IFileStorage>();
        repository = new Mock<IRepository<FileInformation>>();
        archiveService = new Mock<IArchiveService>();
        logger = new Mock<ILogger<GetFilesHandler>>();
        
        sut = new GetFilesHandler(fileStorage.Object, repository.Object, archiveService.Object, logger.Object);
    }

    [Theory, AutoData]
    public async Task Given_GetFilesRequestAndFilesFound_Handle_Should_Return_DownloadUrl(FileInformation fileInformation)
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileInformation);

        repository.Setup(x => x.UpdateAsync(
            It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()));

        fileStorage.Setup(x => x.DownloadFileAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<byte[]>.Success([]));

        archiveService.Setup(x => x.CreateAsync(
                It.IsAny<List<Tuple<string, byte[]>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        fileStorage.Setup(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()));
        
        var resultUrl = new Uri("https://google.com");
        fileStorage.Setup(x => x.GetPreSignedDownloadUrl(It.IsAny<Guid>()))
            .Returns(Result<Uri>.Success(resultUrl));
        
        var result = await sut.Handle(new GetFiles([Guid.NewGuid()], "CurrentUSer"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(resultUrl);
    }
    
    [Fact]
    public async Task Given_GetFilesRequestAndFilesNotFound_Handle_Should_Return_Fail()
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync((FileInformation)null!);
        
        var result = await sut.Handle(new GetFiles([ Guid.NewGuid() ], "CurrentUser"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
    
    [Theory, AutoData]
    public async Task Given_GetFilesRequestAndFilesFound_FailedToDownload_Handle_Should_Return_Fail(
        FileInformation fileInformation)
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileInformation);

        repository.Setup(x => x.UpdateAsync(
            It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()));

        fileStorage.Setup(x => x.DownloadFileAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<byte[]>.Fail(ErrorMessage.NoFilesToDownload));
        
        var result = await sut.Handle(new GetFiles([Guid.NewGuid()], "CurrentUser"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(ErrorMessage.NoFilesToDownload);
    }
    
    [Theory, AutoData]
    public async Task Given_GetFilesRequestAndFilesFound_FailedToGenerateUrl_Handle_Should_Return_Fail(
        FileInformation fileInformation)
    {
        repository.Setup(x => x.GetAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileInformation);

        repository.Setup(x => x.UpdateAsync(
            It.IsAny<FileInformation>(), It.IsAny<CancellationToken>()));

        fileStorage.Setup(x => x.DownloadFileAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<byte[]>.Success([]));

        archiveService.Setup(x => x.CreateAsync(
                It.IsAny<List<Tuple<string, byte[]>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        fileStorage.Setup(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()));
        
        var errorMessage = "error message";
        fileStorage.Setup(x => x.GetPreSignedDownloadUrl(It.IsAny<Guid>()))
            .Returns(Result<Uri>.Fail(errorMessage));
        
        var result = await sut.Handle(new GetFiles([Guid.NewGuid()], "CurrentUser"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }
}