/*
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using OpenFlowWebServer.Repository;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OpenFlowWebServer.IntegrationTest;

namespace OpenFlowWebServer.IntegrationTests.Integration
{
    public class BlobStorageIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public BlobStorageIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UploadFileToBlobStorage_ShouldReturnValidBlobInfo()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var browserFileService = scope.ServiceProvider.GetRequiredService<IBrowserFileService>();

            var fileName = "integration_blob_test.pdf";
            var fileContent = "Test content for blob storage";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var mockFile = new Mock<IBrowserFile>();
            mockFile.Setup(f => f.Name).Returns(fileName);
            mockFile
              .Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
              .Returns(fileStream);

            var mockEventArgs = new Mock<InputFileChangeEventArgs>();
            mockEventArgs.Setup(e => e.File).Returns(mockFile.Object);

            // Act
            var result = await browserFileService.HandleFileChange(mockEventArgs.Object, "blob-container");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileName, result.Name);
            Assert.Equal("pdf", result.Extension);
            Assert.Equal("blob-container", result.Container);
            Assert.NotEqual(Guid.Empty, result.BlobGuid);
            Assert.False(string.IsNullOrEmpty(result.Url));

            // Jeśli istnieje endpoint do weryfikacji zawartości blob storage, można dodać dodatkowe asercje
        }
    }
}
*/
