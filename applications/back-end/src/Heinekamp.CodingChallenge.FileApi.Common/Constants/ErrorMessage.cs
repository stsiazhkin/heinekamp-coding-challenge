namespace Heinekamp.CodingChallenge.FileApi.Common.Constants;

public static class ErrorMessage
{
    public const string FileRequired = "file_required";
    public const string FileSizeExceeded = "file_too_large";
    public const string FileSizeInvalid = "file_size_invalid";
    public const string FileExtensionNotAllowed = "file_extension_not_allowed";
    public const string FileNameTooLong = "file_name_too_long";
    public const string FileContentTypeInvalid = "file_content_type_does_not_match_extension";

    public const string ErrorUploadingFile = "error_uploading_file";
    public const string ErrorDownloadingFile = "error_downloading_file";
    public const string NoFilesToDownload = "no_files_to_download";
    public const string FileNotFound = "file_not_found";
}