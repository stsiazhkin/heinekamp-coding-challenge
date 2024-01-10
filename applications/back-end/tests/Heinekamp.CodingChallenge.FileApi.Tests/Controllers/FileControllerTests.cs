using AutoFixture.Xunit2;
using FluentAssertions;
using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Controllers;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Requests;
using Heinekamp.CodingChallenge.FileApi.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.Tests.Controllers;

public class FileControllerTests
{
    private readonly Mock<IMediator> mediator;
    private readonly FileController sut;

    public FileControllerTests()
    {
        mediator = new ();
        sut = new(mediator.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Given_ValidUploadFileRequest_UploadFileAsync_Should_Return_OkWithFileId()
    {
        var formFile = await PrepareFormFileAsync();
        var uploadFileReuqest = new UploadFileRequest
        {
            File = formFile
        };
        var resultFileId = Guid.NewGuid();
        mediator.Setup(x => x.Send(It.IsAny<UploadFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(resultFileId));

        var result = await sut.UploadFileAsync(uploadFileReuqest, CancellationToken.None);

        var response = result as CreatedResult;
        response.Should().NotBeNull();
        response!.StatusCode.GetValueOrDefault().Should().Be(StatusCodes.Status201Created);
        response.Value.Should().Be(resultFileId);
    }
    
    [Fact]
    public async Task Given_ValidUploadFileRequest_MediatorFailed_UploadFileAsync_Should_Return_500()
    {
        var formFile = await PrepareFormFileAsync();
        var uploadFileReuqest = new UploadFileRequest
        {
            File = formFile
        };
        mediator.Setup(x => x.Send(It.IsAny<UploadFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Fail(ErrorMessage.ErrorUploadingFile));

        var result = await sut.UploadFileAsync(uploadFileReuqest, CancellationToken.None);

        var response = result as ObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
    
    [Fact]
    public async Task Given_GetFilesInformationRequest_GetFilesInformationAsync_Should_Return_Ok()
    {
        var fileInformationItems = new List<FileInformation>
        {
            new ()
            {
                FileId = Guid.NewGuid(),
                FileName = "testName",
                DownloadedCount = 5,
                UploadedOn = DateTimeOffset.UtcNow
            }
        };
        
        mediator.Setup(x => x.Send(It.IsAny<GetFilesInformation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<FileInformation>>.Success(fileInformationItems));
        
        var result = await sut.GetFilesInformationAsync(CancellationToken.None);

        var response = result as OkObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status200OK);
        var resultItems = response.Value as FileInformationResponse;
        resultItems.Should().NotBeNull();
        resultItems!.Items.Count.Should().Be(fileInformationItems.Count);
        var resultItem = resultItems.Items[0];
        var fileInformationItem = fileInformationItems[0];
        resultItem.FileId.Should().Be(fileInformationItem.FileId);
        resultItem.FileName.Should().Be(fileInformationItem.FileName);
        resultItem.DownloadedCount.Should().Be(fileInformationItem.DownloadedCount);
        resultItem.UploadedOn.Should().Be(fileInformationItem.UploadedOn.ToString("dd-MM-yyyy HH:mm:ss"));
    }
    
    [Fact]
    public async Task Given_GetFilesInformationRequest_MediatorFails_GetFilesInformationAsync_Should_Return_500()
    {
        mediator.Setup(x => x.Send(It.IsAny<GetFilesInformation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<FileInformation>>.Fail("somethingWentWrong", new Exception("somethingWentWrong")));
        
        var result = await sut.GetFilesInformationAsync(CancellationToken.None);

        var response = result as ObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
    
    [Fact]
    public async Task Given_ExistentFileId_GetFileDownloadUrlAsync_Should_Return_DownloadUrl()
    {
        var resultUri = new Uri("https://google.com");
        mediator.Setup(x => x.Send(It.IsAny<GetFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Success(resultUri));
        
        var result = await sut.GetFileDownloadUrlAsync(Guid.NewGuid(), CancellationToken.None);

        var response = result as OkObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status200OK);
        (response.Value as Uri).Should().Be(resultUri);
    }
    
    [Fact]
    public async Task Given_NonExistentFileId_GetFileDownloadUrlAsync_Should_Return_404()
    {
        mediator.Setup(x => x.Send(It.IsAny<GetFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Fail(ErrorMessage.FileNotFound));
        
        var result = await sut.GetFileDownloadUrlAsync(Guid.NewGuid(), CancellationToken.None);

        var response = result as ObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Theory, AutoData]
    public async Task Given_SomeNonExistentFileId_GetFilesDownloadUrlAsync_Should_Return_404(Guid fileId)
    {
        mediator.Setup(x => x.Send(It.IsAny<GetFiles>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Fail(ErrorMessage.FileNotFound));
        
        var result = await sut.GetFilesDownloadUrlAsync([ fileId ], CancellationToken.None);

        var response = result as ObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Theory, AutoData]
    public async Task Given_NoFilesDownloadedFromStorage_GetFilesDownloadUrlAsync_Should_Return_404(Guid fileId)
    {
        mediator.Setup(x => x.Send(It.IsAny<GetFiles>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Fail(ErrorMessage.NoFilesToDownload));
        
        var result = await sut.GetFilesDownloadUrlAsync([ fileId ], CancellationToken.None);

        var response = result as ObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task Given_NoFIleIds_GetFilesDownloadUrlAsync_Should_Return_400()
    {
        var result = await sut.GetFilesDownloadUrlAsync([ ], CancellationToken.None);

        var response = result as BadRequestResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    
    [Theory, AutoData]
    public async Task Given_ExistentFileIds_GetFilesDownloadUrlAsync_Should_Return_DownloadUrl(Guid fileId)
    {
        var resultUri = new Uri("https://google.com");
        mediator.Setup(x => x.Send(It.IsAny<GetFiles>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Success(resultUri));
        
        var result = await sut.GetFilesDownloadUrlAsync([ fileId ], CancellationToken.None);

        var response = result as OkObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status200OK);
        (response.Value as Uri).Should().Be(resultUri);
    }
    
    [Fact]
    public async Task Given_SomeNonExistentFileId_PrepareBulkDownloadAsync_Should_Return_404()
    {
        var request = new BulkDownloadRequest
        {
            FilesIds = [ Guid.NewGuid() ]
        };
        
        mediator.Setup(x => x.Send(It.IsAny<PrepareBulkDownload>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Fail(ErrorMessage.FileNotFound));
        
        var result = await sut.PrepareBulkDownloadAsync(request, CancellationToken.None);

        var response = result as ObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task Given_ExistentFileIds_PrepareBulkDownloadAsync_Should_Return_DownloadUrl()
    {
        var request = new BulkDownloadRequest
        {
            FilesIds = [ Guid.NewGuid() ]
        };
        
        var resultUri = new Uri("https://google.com");
        mediator.Setup(x => x.Send(It.IsAny<PrepareBulkDownload>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Uri>.Success(resultUri));
        
        var result = await sut.PrepareBulkDownloadAsync(request, CancellationToken.None);

        var response = result as OkObjectResult;
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(StatusCodes.Status200OK);
        (response.Value as Uri).Should().Be(resultUri);
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