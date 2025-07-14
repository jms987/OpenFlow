/*
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
        /*public Task<Uri> AddBlobAsync(string blobContent, string containerName);
        public Task<Uri> AddBlobAsync(byte[] blobContent, string containerName);#1#
        public Task<BlobRepository.Blob> AddBlobAsync(T blobContent, string containerName);
        public Task DeleteBlobAsync(string blobId, string containerName);
    }

    public class BlobRepository : IBlobRepository<byte[]>, IBlobRepository<string>, IBlobRepository<Stream>
    {
        private string ConnectionString { get; set; }
        private BlobServiceClient BlobServiceClient { get; set; }
        private ILogger<BlobRepository> logger { get; set; }
        
        /*
        public BlobRepository(string connectionString, ILogger logger)
        {
            ConnectionString = connectionString;
            BlobServiceClient = new BlobServiceClient(connectionString);
            this.logger = logger;
        }

        public BlobRepository(string connectionString)
        {
            ConnectionString = connectionString;
            BlobServiceClient = new BlobServiceClient(connectionString);
        }
        #1#

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

        public class Blob
        {
            public Guid BlobId { get; set; }
            public string BlobName { get; set; }
            public string BlobUrl { get; set; }
            /*public string ContainerName { get; set; }#1#
        }
    }
}

/*public class BlobRepository : IBlobRepository
{
    private string ConnectionString { get; set; }
    private BlobServiceClient BlobServiceClient { get; set; }

    public BlobRepository(string connectionString)
    {
        ConnectionString = connectionString;
        BlobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<Uri> AddBlobAsync(object blobContent, string containerName)
    {
        if (blobContent is string stringContent)
        {
            return await AddBlobAsync(stringContent, containerName);
        }
        else if (blobContent is byte[] byteArrayContent)
        {
            return await AddBlobAsync(byteArrayContent, containerName);
        }
        else
        {
            throw new ArgumentException("Unsupported blob content type.");
        }
    }

    private async Task<Uri> AddBlobAsync(string blobContent, string containerName)
    {
        // Implementation for string content
        // ...
        return new Uri("https://example.com");
    }

    private async Task<Uri> AddBlobAsync(byte[] blobContent, string containerName)
    {
        // Implementation for byte[] content
        // ...
        return new Uri("https://example.com");
    }
}#1#
*/
