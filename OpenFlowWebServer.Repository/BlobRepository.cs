using Azure.Storage.Blobs;
using OpenFlowWebServer.Repository;
using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OpenFlowWebServer.Repository
{
    public interface IBlobRepository<T>
    {
        public Task<Blob> AddBlobAsync(T blobContent, string containerName);
        public Task DeleteBlobAsync(string blobId, string containerName);
    }

    public class BlobRepository : IBlobRepository<byte[]>, IBlobRepository<string>, IBlobRepository<Stream>
    {
        private BlobServiceClient BlobServiceClient { get; set; }
        private ILogger<BlobRepository> logger { get; set; }

        public BlobRepository(BlobServiceClient blobServiceClient, ILogger<BlobRepository> logger)
        {
            BlobServiceClient = blobServiceClient;
            this.logger = logger;
        }

        private class PreparedBlob
        {
            public Blob Blob { get; set; }
            public BlobClient BlobClient { get; set; }
        }

        private PreparedBlob BlobPrepare(string containerName)
        {
            var blobContainerClient = BlobServiceClient.GetBlobContainerClient(containerName.ToLower());
            blobContainerClient.CreateIfNotExists();
            var guid = Guid.NewGuid();
            var blob = new Blob
            {
                BlobId = guid,
                BlobName = guid.ToString(),
            };
            var blobClient = blobContainerClient.GetBlobClient(blob.BlobName);
            var response = new PreparedBlob
            {
                Blob = blob,
                BlobClient = blobClient
            };
            blob.BlobUrl = blobClient.Uri.ToString();
            blob.ContainerName = containerName.ToLower();
            return response;
        }

        public async Task<Blob> AddBlobAsync(byte[] blobContent, string containerName)
        {
            var blobPrepared= BlobPrepare(containerName);

            using (var stream = new MemoryStream(blobContent))
            {
                var response = await blobPrepared.BlobClient.UploadAsync(stream, overwrite: true);
                logger.Log(LogLevel.Information, $"Blob {blobPrepared.Blob.BlobName} uploaded to container {containerName}. Response {response.Value.ToString()}");
            }

            return blobPrepared.Blob;
        }

        public async Task<Blob> AddBlobAsync(string blobContent, string containerName)
        {
            var blobPrepared = BlobPrepare(containerName);
            using (var stream = new MemoryStream(Convert.FromBase64String(blobContent)))
            {
                var response = await blobPrepared.BlobClient.UploadAsync(stream, overwrite: true);
                logger.Log(LogLevel.Information, $"Blob {blobPrepared.Blob.BlobName} uploaded to container {containerName}. Response {response.Value.ToString()}");
            }

            return blobPrepared.Blob;
        }


        public async Task<Blob> AddBlobAsync(Stream blobContent, string containerName)
        {
            var blobPrepared = BlobPrepare(containerName);
            var response = await blobPrepared.BlobClient.UploadAsync(blobContent, overwrite: true);
            logger.Log(LogLevel.Information, $"Blob {blobPrepared.Blob.BlobName} uploaded to container {containerName}. Response {response.Value.ToString()}");
            return blobPrepared.Blob;
        }

        public async Task DeleteBlobAsync(string blobId, string containerName)
        {
            var blobContainerClient = BlobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobId);
            await blobClient.DeleteIfExistsAsync();
        }
    }

    public class Blob
    {
        public Guid BlobId { get; set; }
        public string BlobName { get; set; }
        public string BlobUrl { get; set; }
        public string ContainerName { get; set; }
    }
}
