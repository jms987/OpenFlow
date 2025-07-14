/*using Microsoft.AspNetCore.Components.Forms;
using Moq;
using OpenFlowWebServer.Repository;
using System.Text;
using File = OpenFlowWebServer.Data.Domain.File;

namespace OpenFlowWebServer.IntegrationTests.Services
{
    public class BrowserFileServiceTests
    {
        [Fact]
        public async Task HandleFileChange_ShouldReturnFileWithCorrectMetadata()
        {
            // Arrange
            var mockBlobRepo = new Mock<IBlobRepository<Stream>>();
            var blobId = Guid.NewGuid();
            var blobUrl = "https://testblob.com/testcontainer/testblob";

            // Konfiguracja mockowanego repozytorium
            mockBlobRepo
                .Setup(repo => repo.AddBlobAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new BlobRepository.Blob { BlobId = blobId, BlobUrl = blobUrl });

            // Utwórz testowy serwis
            var browserFileService = new BrowserFileService(mockBlobRepo.Object);

            // Utwórz mock dla InputFileChangeEventArgs
            var fileName = "test.txt";
            var mockFile = new Mock<IBrowserFile>();
            mockFile.Setup(f => f.Name).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("Test content")));

            var mockEventArgs = new Mock<InputFileChangeEventArgs>();
            mockEventArgs.Setup(e => e.File).Returns(mockFile.Object);

            // Act
            var result = await browserFileService.HandleFileChange(mockEventArgs.Object, "testcontainer");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testcontainer", result.Container);
            Assert.Equal(fileName, result.Name);
            Assert.Equal("txt", result.Extension);
            Assert.Equal(blobId, result.BlobGuid);
            Assert.Equal(blobUrl, result.Url);
        }

        [Fact]
        public async Task HandleFileChange_WithDifferentExtension_ShouldExtractExtensionCorrectly()
        {
            // Arrange
            var mockBlobRepo = new Mock<IBlobRepository<Stream>>();
            mockBlobRepo
                .Setup(repo => repo.AddBlobAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new BlobRepository.Blob { BlobId = Guid.NewGuid(), BlobUrl = "https://test.com/file" });

            var browserFileService = new BrowserFileService(mockBlobRepo.Object);

            var fileName = "document.pdf";
            var mockFile = new Mock<IBrowserFile>();
            mockFile.Setup(f => f.Name).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("Test PDF content")));

            var mockEventArgs = new Mock<InputFileChangeEventArgs>();
            mockEventArgs.Setup(e => e.File).Returns(mockFile.Object);

            // Act
            var result = await browserFileService.HandleFileChange(mockEventArgs.Object, "docs");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("pdf", result.Extension);
        }

        [Fact]
        public async Task HandleFileChange_ShouldUseProvidedContainerName()
        {
            // Arrange
            var mockBlobRepo = new Mock<IBlobRepository<Stream>>();
            var containerName = "custom-container";

            mockBlobRepo
                .Setup(repo => repo.AddBlobAsync(It.IsAny<Stream>(), containerName))
                .ReturnsAsync(new BlobRepository.Blob
                    { BlobId = Guid.NewGuid(), BlobUrl = "https://test.com/custom-file" });

            var browserFileService = new BrowserFileService(mockBlobRepo.Object);

            var mockFile = new Mock<IBrowserFile>();
            mockFile.Setup(f => f.Name).Returns("test-file.txt");
            mockFile.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("Content")));

            var mockEventArgs = new Mock<InputFileChangeEventArgs>();
            mockEventArgs.Setup(e => e.File).Returns(mockFile.Object);

            // Act
            var result = await browserFileService.HandleFileChange(mockEventArgs.Object, containerName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(containerName, result.Container);

            // Weryfikacja, ¿e AddBlobAsync zosta³o wywo³ane z w³aœciwym parametrem container
            mockBlobRepo.Verify(repo => repo.AddBlobAsync(It.IsAny<Stream>(), containerName), Times.Once);
        }
    }

    // Klasa pomocnicza, która pozwoli nam testowaæ kod bez implementowania ca³ego IBrowserFile
    //public static class BrowserFileExtensions
    //{
    //    public class TestInputFileChangeEventArgs : InputFileChangeEventArgs
    //    {
    //        private readonly IBrowserFile _file;

    //        public TestInputFileChangeEventArgs(IBrowserFile file)
    //        {
    //            _file = file;
    //        }

    //        public override IBrowserFile File => _file;
    //    }
    //}
}*/