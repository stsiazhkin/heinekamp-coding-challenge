using FluentAssertions;
using FluentValidation.Results;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Requests;
using Heinekamp.CodingChallenge.FileApi.Validation;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.Tests.Validation;

public class UploadFileRequestValidatorTests
{
    private readonly UploadFileRequestValidator sut = new();

    [Fact]
    public void Should_have_FileRequired_error_when_null()
    {
        sut.Validate(new UploadFileRequest())
            .Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == false &&
            validationResult.Errors.Single(failure => 
                failure.ErrorMessage == ErrorMessage.FileRequired) != null);
    }

    [Fact]
    public void Should_have_FileSizeInvalid_error_when_file_bytes_0()
    {
        var formFile = new Mock<IFormFile>();
        formFile.Setup(file => file.Length)
            .Returns(0);
        formFile.Setup(file => file.FileName)
            .Returns("HelloWorld.zip");
        formFile.Setup(x => x.ContentType)
            .Returns(MimeTypes.MimeTypeMap.GetMimeType(".zip"));
        var request = new UploadFileRequest
        {
            File = formFile.Object
        };
        
        var result = sut.Validate(request);
        
        result.Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == false &&
            validationResult.Errors.Single(failure =>
                failure.ErrorMessage == ErrorMessage.FileSizeInvalid) != null);
    }

    [Fact]
    public void Should_have_FileSizeExceeded_error_when_file_bytes_larger_then_MaxFileSize()
    {
        var formFile = new Mock<IFormFile>();
        formFile.Setup(file => file.Length)
            .Returns(100L * 1024L * 1024L);
        formFile.Setup(file => file.FileName)
            .Returns("SomeFiles.zip");
        formFile.Setup(x => x.ContentType)
            .Returns(MimeTypes.MimeTypeMap.GetMimeType(".zip"));
        var request = new UploadFileRequest
        {
            File = formFile.Object
        };
        
        var result = sut.Validate(request);
        
        result.Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == false &&
            validationResult.Errors.Single(failure =>
                failure.ErrorMessage == ErrorMessage.FileSizeExceeded) != null);
    }

    [Fact]
    public void Should_have_FileNameTooLong_error_when_filename_longer_then_145()
    {
        var formFile = new Mock<IFormFile>();
        formFile.Setup(file => file.Length)
            .Returns(10);
        formFile.Setup(file => file.FileName)
            .Returns($"{new string('*', 150)}.zip");
        formFile.Setup(x => x.ContentType)
            .Returns(MimeTypes.MimeTypeMap.GetMimeType(".zip"));
        var request = new UploadFileRequest
        {
            File = formFile.Object
        };
        
        var result = sut.Validate(request);
        
        result.Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == false &&
            validationResult.Errors.Single(failure =>
                failure.ErrorMessage == ErrorMessage.FileNameTooLong) != null);
    }

    [Fact]
    public void Should_have_FileExtensionNotAllowed_error_when_file_extension_not_in_AllowedFileExtensions()
    {
        var formFile = new Mock<IFormFile>();
        formFile.Setup(file => file.Length)
            .Returns(10);
        formFile.Setup(file => file.FileName)
            .Returns("HelloWorld.xyz");
        formFile.Setup(x => x.ContentType)
            .Returns(MimeTypes.MimeTypeMap.GetMimeType(".xyz"));
        var request = new UploadFileRequest
        {
            File = formFile.Object
        };
        
        var result = sut.Validate(request);
        
        result.Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == false &&
            validationResult.Errors.Single(failure =>
                failure.ErrorMessage == ErrorMessage.FileExtensionNotAllowed) != null);
    }

    [Fact]
    public void Should_have_FileContentTypeInvalid_error_when_file_content_type_does_not_match_extension()
    {
        var formFile = new Mock<IFormFile>();
        formFile.Setup(file => file.Length)
            .Returns(10);
        formFile.Setup(file => file.FileName)
            .Returns("HelloWorld.doc");
        formFile.Setup(x => x.ContentType)
            .Returns(MimeTypes.MimeTypeMap.GetMimeType(".zip"));
        var request = new UploadFileRequest
        {
            File = formFile.Object
        };
        
        var result = sut.Validate(request);
        
        result.Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == false &&
            validationResult.Errors.Single(failure =>
                failure.ErrorMessage == ErrorMessage.FileContentTypeInvalid) != null);
    }

    [Fact]
    public void Given_valid_file_with_upper_case_extension_should_return_validation_result_ok()
    {
        var formFile = new Mock<IFormFile>();
        formFile.Setup(file => file.Length)
            .Returns(10);
        formFile.Setup(file => file.FileName)
            .Returns("HelloWorld.DOC");
        formFile.Setup(x => x.ContentType)
            .Returns(MimeTypes.MimeTypeMap.GetMimeType(".doc"));
        var request = new UploadFileRequest
        {
            File = formFile.Object
        };
        
        var result = sut.Validate(request);
        
        result.Should().Match<ValidationResult>(validationResult =>
            validationResult.IsValid == true);
    }
}