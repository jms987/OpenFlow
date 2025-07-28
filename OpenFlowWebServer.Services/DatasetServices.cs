using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository;

namespace OpenFlowWebServer.Services
{
    public interface IDatasetServices
    {
        public Task AddDatasetAsync(Dataset dataset, FileUpload ConfigFile, FileUpload DatasetFile);
    }

    public class DatasetServices : IDatasetServices
    {
        public readonly IDatasetRepository _datasetRepository;
        public readonly IFileRepository _fileRepository;
        public readonly IBlobRepository<Stream> _blobRepository;
        public readonly IConfiguration _config;


        public DatasetServices(IDatasetRepository datasetRepository, IFileRepository fileRepository, IBlobRepository<Stream> blobRepository, IConfiguration config)
        {
            _datasetRepository = datasetRepository;
            _fileRepository = fileRepository;
            _blobRepository = blobRepository;
            _config = config;
        }

        public async Task AddDatasetAsync(Dataset dataset, FileUpload ConfigFile, FileUpload DatasetFile)
        {
            if (dataset == null)
            {
                throw new ArgumentNullException(nameof(dataset), "Dataset cannot be null");
            }
            dataset.ConfigFileId = ConfigFile.File.Id;
            dataset.DatasetFileId = DatasetFile.File.Id;
            var configBlob  = await _blobRepository.AddBlobAsync(ConfigFile.DataStream, _config["Databases:BlobStorage:ConfigContainerName"]);
            var datasetBlob = await _blobRepository.AddBlobAsync(DatasetFile.DataStream, _config["Databases:BlobStorage:DatasetContainerName"]);
            
            ConfigFile.File.BlobGuid = configBlob.BlobId;
            DatasetFile.File.BlobGuid = datasetBlob.BlobId;
            ConfigFile.File.Url = configBlob.BlobUrl;
            DatasetFile.File.Url = datasetBlob.BlobUrl;
            await _datasetRepository.AddAsync(dataset);
            await _fileRepository.AddAsync(ConfigFile.File);
            await _fileRepository.AddAsync(DatasetFile.File);

            await _datasetRepository.SaveChangesAsync();
            // throw new NotImplementedException();
        }
    }
}
