/*using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OpenFlowWebServer.IntegrationTest;
using OpenFlowWebServer.Repository;

namespace OpenFlowWebServer.IntegrationTests.Integration
{
    public class BrowserFileServiceIntegrationTests
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public BrowserFileServiceIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HandleFileChange_EndToEnd_ShouldCreateFile()
        {
            // Arrange
            var scope = _factory.Services.CreateScope();
            var browserFileService = scope.ServiceProvider.GetRequiredService<IBrowserFileService>();

            // Przygotowanie danych testowych
            var mockFile = new Mock<IBrowserFile>();
            mockFile.Setup(f => f.Name).Returns("integration-test.docx");
            mockFile.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("Integration test content")));

            var mockEventArgs = new Mock<InputFileChangeEventArgs>();
            mockEventArgs.Setup(e => e.File).Returns(mockFile.Object);

            // Act
            var result = await browserFileService.HandleFileChange(mockEventArgs.Object, "integration-tests");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("integration-test.docx", result.Name);
            Assert.Equal("docx", result.Extension);
            Assert.Equal("integration-tests", result.Container);
            Assert.NotEqual(Guid.Empty, result.BlobGuid);
            Assert.NotNull(result.Url);
        }
    }
}*/