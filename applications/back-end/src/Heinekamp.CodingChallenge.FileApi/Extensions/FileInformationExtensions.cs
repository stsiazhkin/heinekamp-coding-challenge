using Heinekamp.CodingChallenge.FileApi.Common.Models;
using Heinekamp.CodingChallenge.FileApi.Responses;

namespace Heinekamp.CodingChallenge.FileApi.Extensions;

public static class FileInformationExtensions
{
    public static FileInformationResponse ToApiResponse(this List<FileInformation> filesInformation)
    {
        return new FileInformationResponse
        {
            Items = filesInformation.Select(item => new FileInformationItem
            {
                FileId = item.FileId,
                FileName = item.FileName,
                DownloadedCount = item.DownloadedCount,
                UploadedOn = item.UploadedOn.ToString("dd-MM-yyyy HH:mm:ss"),
                ThumbnailImage = item.ThumbnailImage,
                FileContentType = item.FileContentType
            }).ToList()
        };
    }
}