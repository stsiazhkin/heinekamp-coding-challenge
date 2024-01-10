using Heinekamp.CodingChallenge.FileApi.Common;
using Heinekamp.CodingChallenge.FileApi.Handlers.Requests;
using MediatR;

namespace Heinekamp.CodingChallenge.FileApi.Handlers;

/// <summary>
/// A Handler to prepare files for download
/// The flow:
/// 1.A request for a bulk download comes from a whatever client
///     An incoming request contains a set of file Ids
/// 2. FileAPI creates a FileInformation record in its database and raises an event/publishes a queue message
/// 4. FileInformation FileId is returned to a client
/// 5. This event triggers a separate service (Perhaps AWS Lambda or Azure function)
///     which performs files download and archives them in a single zip file
/// 6. Zip is uploaded to an S3 bucket/Azure Blob Storage by the separate service 
/// 7. Client now can fetch PreSigned downloadUrl based on previously stored FileInformation FileId.
///     optionaly as soon as zip is uploaded to S3 a download URL can be sent by email through notification service etc
/// </summary>
public class PrepareBulkDownloadHandler : IRequestHandler<PrepareBulkDownload, Result<Uri>>
{
    public Task<Result<Uri>> Handle(PrepareBulkDownload request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
