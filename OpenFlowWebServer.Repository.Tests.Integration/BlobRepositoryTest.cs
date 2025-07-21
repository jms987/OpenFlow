using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Configuration;
using System.Text;
using System;

namespace OpenFlowWebServer.Repository.Tests.Integration
{
    [NonParallelizable]
    [TestFixture]
    public class BlobRepositoryTest
    {
            private IServiceProvider _serviceProvider;
        private string _blobConnectionString;
        private string _containerName;
        private BlobContainerClient _containerClient;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {

            
            _blobConnectionString = Environment.GetEnvironmentVariable("BLOB_CONNECTIONSTRING")
                                     ?? "UseDevelopmentStorage=true";
            _containerName = "test-container"+Guid.NewGuid();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<BlobServiceClient>(new BlobServiceClient(_blobConnectionString));
            services.AddScoped<IBlobRepository<string>, BlobRepository>();
            services.AddScoped<IBlobRepository<byte[]>, BlobRepository>();
            services.AddScoped<IBlobRepository<Stream>, BlobRepository>();
            _serviceProvider = services.BuildServiceProvider();
            _containerClient = new BlobContainerClient(_blobConnectionString, _containerName);
            await _containerClient.CreateIfNotExistsAsync();
        }

        [Test]
        public async Task AddAndDeleteBlob_WithByteArray_WorksCorrectly()
        {
            // Arrange
            var repo = _serviceProvider.GetRequiredService<IBlobRepository<byte[]>>();
            byte[] content = Encoding.UTF8.GetBytes("byte[] test content");

            // Act
            var blob = await repo.AddBlobAsync(content, _containerName);

            // Assert
            Assert.NotNull(blob);
            Assert.AreNotEqual(Guid.Empty, blob.BlobId);
            Assert.False(string.IsNullOrWhiteSpace(blob.BlobName));
            Assert.AreEqual(_containerName, blob.ContainerName);
            Assert.IsTrue(Uri.IsWellFormedUriString(blob.BlobUrl, UriKind.Absolute));

            // Cleanup & verify
            await repo.DeleteBlobAsync(blob.BlobName, _containerName);
            var exists = await _containerClient.GetBlobClient(blob.BlobName).ExistsAsync();
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task AddAndDeleteBlob_WithBase64String_WorksCorrectly()
        {
            // Arrange
            var repo = _serviceProvider.GetRequiredService<IBlobRepository<string>>();
            string originalContent = "string test content";
            string base64Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(originalContent));

            // Act
            var blob = await repo.AddBlobAsync(base64Content, _containerName);

            // Assert
            Assert.NotNull(blob);
            Assert.AreNotEqual(Guid.Empty, blob.BlobId);
            Assert.False(string.IsNullOrWhiteSpace(blob.BlobName));
            Assert.AreEqual(_containerName, blob.ContainerName);
            Assert.IsTrue(Uri.IsWellFormedUriString(blob.BlobUrl, UriKind.Absolute));

            // Cleanup & verify
            await repo.DeleteBlobAsync(blob.BlobName, _containerName);
            var exists = await _containerClient.GetBlobClient(blob.BlobName).ExistsAsync();
            Assert.IsFalse(exists);
        }


        [Test]
        public async Task AddAndDeleteBlob_WithStream_WorksCorrectly()
        {
            // Arrange
            var repo = _serviceProvider.GetRequiredService<IBlobRepository<Stream>>();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("stream test content"));

            // Act
            var blob = await repo.AddBlobAsync(stream, _containerName);

            // Assert
            Assert.NotNull(blob);
            Assert.AreNotEqual(Guid.Empty, blob.BlobId);
            Assert.False(string.IsNullOrWhiteSpace(blob.BlobName));
            Assert.AreEqual(_containerName, blob.ContainerName);
            Assert.IsTrue(Uri.IsWellFormedUriString(blob.BlobUrl, UriKind.Absolute));

            // Cleanup & verify
            await repo.DeleteBlobAsync(blob.BlobName, _containerName);
            var exists = await _containerClient.GetBlobClient(blob.BlobName).ExistsAsync();
            Assert.IsFalse(exists);
        }
    

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {

            await _containerClient.DeleteIfExistsAsync();
        }

    }
}
