using FluentAssertions;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using Microsoft.Extensions.Logging;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Tests;

public class GetFilesInformationHandlerTests
{
    private readonly Mock<IRepository<FileInformation>> repository;
    private readonly Mock<ILogger<GetFilesInformationHandler>> logger;
    
    private readonly GetFilesInformationHandler sut;

    public GetFilesInformationHandlerTests()
    {
        repository = new Mock<IRepository<FileInformation>>();
        logger = new Mock<ILogger<GetFilesInformationHandler>>();
        
        sut = new GetFilesInformationHandler(repository.Object, logger.Object);
    }

    [Fact]
    public async Task Given_GetFilesInformationRequest_Handle_Should_Return_Ok()
    {
        var user = "CurrentUser";
        var fileInformationItems = new List<FileInformation>
        {
            new ()
            {
                FileId = Guid.NewGuid(),
                FileName = "testName",
                DownloadedCount = 5,
                UploadedOn = DateTimeOffset.UtcNow,
                UploadedBy = user
            }
        };
        

        repository.Setup(x => x.GetAsync(
                It.IsAny<SearchParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fileInformationItems);

        var result = await sut.Handle(new GetFilesInformation(user), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Count.Should().Be(fileInformationItems.Count);
        var resultItem = result.Value[0];
        var fileInformationItem = fileInformationItems[0];
        resultItem.FileId.Should().Be(fileInformationItem.FileId);
        resultItem.FileName.Should().Be(fileInformationItem.FileName);
        resultItem.DownloadedCount.Should().Be(fileInformationItem.DownloadedCount);
        resultItem.UploadedOn.Should().Be(fileInformationItem.UploadedOn);
        resultItem.UploadedBy.Should().Be(fileInformationItem.UploadedBy);
    }
}