using Microsoft.AspNetCore.Http;
using Moq;

namespace Heinekamp.CodingChallenge.FileApi.IntegrationTests.TestFileData;

public class TestFormFile
{
    private const string TestFileName = "test.png";

    public static Mock<IFormFile> GetMock(
        string fileName = TestFileName, 
        string contents = "123", 
        string contentType = "image/png")
    {
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        if (fileName == TestFileName)
        {
            var physicalFile = new FileInfo($"./TestFileData/{fileName}");
            writer.Write(physicalFile.OpenRead());
        }
        else
        {
            writer.Write(contents);
        }
     
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(m => m.FileName).Returns(fileName);
        fileMock.Setup(m => m.Length).Returns(stream.Length);
        fileMock.Setup(m => m.ContentType).Returns(contentType);
        fileMock.Setup(m => m.OpenReadStream()).Returns(stream);
        fileMock.Setup(_ => _.ContentDisposition).Returns($"inline; filename={fileName}");

        return fileMock;
    }
}