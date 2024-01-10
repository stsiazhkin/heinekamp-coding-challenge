using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Common.Constants;
using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using Heinekamp.CodingChallenge.FileApi.Persistence;
using Heinekamp.CodingChallenge.FileApi.Services.FilesArchiving;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Heinekamp.CodingChallenge.FileApi.Handlers;

/// <summary>
/// !!!
/// This can be a heavy load operation which should be extracted to a Separate Service
/// such as AWS Lambda or Azure Function
/// FileAPI only should create a FileInformation record for a zip file and emit an event with FileInformation.FileId
/// which represents zip archive of requested for download files. These FilesIDs also have to be included in event payload
/// so that the separate service responsible for files archivation knows what files to process
/// !!!
/// </summary>
public class GetFilesHandler(
    IFileStorage fileStorage, 
    IRepository<FileInformation> repository, 
    IArchiveService archiveService,
    ILogger<GetFilesHandler> logger) : IRequestHandler<GetFiles, Result<Uri>>
{
    public async Task<Result<Uri>> Handle(GetFiles request, CancellationToken cancellationToken)
    {
        var filesInformationResult = await GetFilesInformationAsync(request, cancellationToken);
        if (!filesInformationResult.IsSuccess)
            return Result<Uri>.Fail(filesInformationResult.ErrorMessage!, filesInformationResult.Exception);

        var filesToArchive = await DownloadFilesAndUpdateFilesInformationDownloadCountAsync(
            filesInformationResult.Value,
            cancellationToken);

        if (!ShouldPrepareFilesArchive(filesToArchive))
            return Result<Uri>.Fail(ErrorMessage.NoFilesToDownload);

        var archiveFile = await PrepareFilesArchiveAsync(filesToArchive, cancellationToken);

        await UploadArchiveFileAsync(archiveFile, cancellationToken);

        var (archiveFileId, _) = archiveFile;

        var fileDownloadUrl = GetArchiveDownloadUrl(archiveFileId);
            
        return fileDownloadUrl;
    }

    private async Task<Result<List<FileInformation>>> GetFilesInformationAsync(GetFiles request, CancellationToken cancellationToken)
    {
        var result = new List<FileInformation>();
        
        foreach (var fileId in request.FileIds)
        {
            logger.LogDebug("Retrieving file with id: '{fileId}'", fileId);
            var fileInformation = await repository.GetAsync(fileId, request.CurrentUser, cancellationToken);
            if (fileInformation is null)
            {
                logger.LogError("Failed to fetch the file with id: '{fileId}'. File not found", fileId);
                return Result<List<FileInformation>>.Fail(ErrorMessage.FileNotFound);   
            }
            result.Add(fileInformation);
        }

        return Result<List<FileInformation>>.Success(result);
    }

    private async Task<List<Tuple<string, byte[]>>> DownloadFilesAndUpdateFilesInformationDownloadCountAsync(
        List<FileInformation> filesInformation,
        CancellationToken cancellationToken)
    {
        var result = new List<Tuple<string, byte[]>>();
        
        foreach (var fileInformation in filesInformation)
        {
            logger.LogDebug("Downloading file with id: '{fileInformation.FileId}' for archive preparation",
                fileInformation.FileId);
            var fileBytes = await fileStorage.DownloadFileAsync(fileInformation.FileId, cancellationToken);
            if (!fileBytes.IsSuccess)
            {
                logger.LogError("Failed to download file with id:'{fileId}'", fileInformation.FileId);
                continue;
            }
            
            logger.LogDebug("Updating download count of the file  with id: '{fileInformation.FileId}'", 
                fileInformation.FileId);
            fileInformation.DownloadedCount = ++fileInformation.DownloadedCount;
            await repository.UpdateAsync(
                fileInformation,
                cancellationToken);
            
            result.Add(new Tuple<string, byte[]>(fileInformation.FileName!, fileBytes.Value));
        }

        return result;
    }

    private static bool ShouldPrepareFilesArchive(List<Tuple<string, byte[]>> files)
    {
        return files.Count != 0;
    }

    private async Task<(Guid, byte[])> PrepareFilesArchiveAsync(List<Tuple<string, byte[]>> files, CancellationToken cancellationToken)
    {
        var archiveFileId = Guid.NewGuid();
        logger.LogDebug("Archiving files into archive file with id: '{archiveFileId}'", archiveFileId);
        var archiveStreamBytes = await archiveService.CreateAsync(files, cancellationToken);

        return (archiveFileId, archiveStreamBytes);
    }

    private async Task UploadArchiveFileAsync((Guid, byte[]) archiveFile, CancellationToken cancellationToken)
    {
        var (archiveFileId, archiveFileBytes) = archiveFile; 
        
        logger.LogDebug("Uploading archive file with id: '{archiveFileId}'", archiveFileId);
        using var zipArchiveStream = new MemoryStream(archiveFileBytes);
        await fileStorage.UploadAsync(
            zipArchiveStream,
            "application/zip", 
            archiveFileId,
            cancellationToken);
    }

    private Result<Uri> GetArchiveDownloadUrl(Guid archiveFileId)
    {
        logger.LogDebug("Preparing download URL for the file with id: '{archiveFileId}'", archiveFileId);    
        var fileDownloadUrl = fileStorage.GetPreSignedDownloadUrl(archiveFileId);

        return fileDownloadUrl;
    }
}