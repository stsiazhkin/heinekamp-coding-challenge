using FluentValidation;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using MimeTypes;
using Heinekamp.CodingChallenge.FileApi.Requests;

namespace Heinekamp.CodingChallenge.FileApi.Validation;

public class UploadFileRequestValidator: AbstractValidator<UploadFileRequest>
{
    private const long MaxFileSize = 10L * 1024L * 1024L; //10 mb
    private const int MaxFileNameLength = 145;

    private static IEnumerable<string> AllowedFileExtensions =>
        new List<string>
        {
            ".pdf", 
            ".doc", ".docx", 
            ".xls", ".xlsx",
            ".txt", 
            ".jpg", ".jpeg", ".png", ".gif"
        };
    
    public UploadFileRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage(ErrorMessage.FileRequired)
            .Must(file => file is not null && string.Equals(
                MimeTypeMap.GetMimeType(Path.GetExtension(file.FileName)), file.ContentType, StringComparison.OrdinalIgnoreCase))
            .WithMessage(ErrorMessage.FileContentTypeInvalid);

        RuleFor(upload => upload.File.Length)
            .Must(fileLength => fileLength <= MaxFileSize)
            .WithMessage(ErrorMessage.FileSizeExceeded)
            .Must(fileLength => fileLength > 0)
            .WithMessage(ErrorMessage.FileSizeInvalid);

        RuleFor(upload => upload.File.FileName)
            .Must(fileName => fileName.Length <= MaxFileNameLength)
            .WithMessage(ErrorMessage.FileNameTooLong)
            .Must(fileName => AllowedFileExtensions.Any(fileName.ToLower().EndsWith))
            .WithMessage(ErrorMessage.FileExtensionNotAllowed);
    }
}