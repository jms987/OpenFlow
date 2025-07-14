using System;
using File = OpenFlowWebServer.Domain.Entities.File;
using Xunit;

namespace OpenFlowWebServer.Domain.UnitTests.Domain
{
    public class FileTest
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var blobGuid = Guid.NewGuid();
            var file = new File
            {
                Id = id,
                Url = "http://test/file.txt",
                Container = "test-container",
                BlobGuid = blobGuid,
                Extension = "txt",
                Name = "file.txt"
            };

            // Assert
            Assert.Equal(id, file.Id);
            Assert.Equal("http://test/file.txt", file.Url);
            Assert.Equal("test-container", file.Container);
            Assert.Equal(blobGuid, file.BlobGuid);
            Assert.Equal("txt", file.Extension);
            Assert.Equal("file.txt", file.Name);
        }

        [Fact]
        public void DefaultConstructor_ShouldAllowPropertyAssignment()
        {
            // Act
            var file = new File();

            // Assert
            Assert.Equal(Guid.Empty, file.Id);
            Assert.Null(file.Url);
            Assert.Null(file.Container);
            Assert.Equal(Guid.Empty, file.BlobGuid);
            Assert.Null(file.Extension);
            Assert.Null(file.Name);
        }
    }
}