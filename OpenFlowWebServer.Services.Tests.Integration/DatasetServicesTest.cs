using System.Text;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository;
using static System.Formats.Asn1.AsnWriter;
using File = OpenFlowWebServer.Domain.Entities.File;


namespace OpenFlowWebServer.Services.Tests.Integration
{
    [NonParallelizable]
    [TestFixture]
    public class DatasetServicesTest
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;
        private BlobServiceClient _blobClient;
        private string _testDbName;
        private string _mongoConnectionString;
        private string _blobConnectionString;
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            _testDbName = "TestDb_" + Guid.NewGuid();
            _mongoConnectionString = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING")
                                     ?? "mongodb://localhost:27017";

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMongoDB(_mongoConnectionString, _testDbName));

            _blobConnectionString = Environment.GetEnvironmentVariable("BLOB_CONNECTIONSTRING")
                                    ?? "UseDevelopmentStorage=true";
            services.AddLogging();
            
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Databases:BlobStorage:ConfigContainerName", $"test-config-container{Guid.NewGuid().ToString()}" },
                    { "Databases:BlobStorage:DatasetContainerName", $"test-dataset-container{Guid.NewGuid().ToString()}" }
                })
                .Build();

            services.AddSingleton<BlobServiceClient>(new BlobServiceClient(_blobConnectionString));
            services.AddSingleton<IConfiguration>(_config);
            services.AddScoped<IDatasetRepository, DatasetRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IBlobRepository<Stream>, BlobRepository>();
            services.AddScoped<IDatasetServices, DatasetServices>();

            _serviceProvider = services.BuildServiceProvider();

            _blobClient = _serviceProvider.GetRequiredService<BlobServiceClient>();
            _blobClient.CreateBlobContainer(_config["Databases:BlobStorage:ConfigContainerName"]);
            _blobClient.CreateBlobContainer(_config["Databases:BlobStorage:DatasetContainerName"]);



            //using var scope = _serviceProvider.CreateScope();
            //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            /*_tempFile = new File
            {
                Id = Guid.NewGuid(),
                Name = "ConfigFile",
                Container = "Container",
                Extension = "ext",
                Url = "http://example.com/config.ext"
            };
            context.Files.Add(_tempFile);
            context.Datasets.AddRange(new[]
            {
                new Dataset { Id =  ExistentId, Name = "Dataset 1", Description = "abcd",ConfigFile =    _tempFile,DatasetFile = _tempFile},
                new Dataset { Id = Guid.NewGuid(), Name = "Dataset 2", Description = "abcd",ConfigFile = _tempFile,DatasetFile = _tempFile},
                new Dataset { Id = NoConfigId, Name = "Dataset 3", Description = "abcd",DatasetFile = _tempFile},
                new Dataset { Id = NoDatasetId, Name = "Dataset 4", Description = "abcd",ConfigFile = _tempFile}
            });

            context.SaveChanges();*/

        }


        [Test]
        public async Task Test_AddDatasetAsync_WorkProperly()
        {
            var serv = _serviceProvider.GetRequiredService<IDatasetServices>();
            var dataset = new Dataset(){Id=Guid.NewGuid(),Name="new dataset"};
            var configFile = new FileUpload
            {
                File = new File
                {
                    Id = Guid.NewGuid(),
                    Name = "ConfigFile",
                    Container = "Container",
                    Extension = "ext",
                    Url = "http://example.com/config.ext"
                },
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes("Config file content"))
            };
            var datasetFile = new FileUpload
            {
                File = new File
                {
                    Id = Guid.NewGuid(),
                    Name = "DatasetFile",
                    Container = "Container",
                    Extension = "ext",
                    Url = "http://example.com/dataset.ext"
                },
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes("Dataset file content"))
            };

            await serv.AddDatasetAsync(dataset,configFile,datasetFile);
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //check dataset
            var datasetList = context.Datasets.ToList();
            Assert.IsNotEmpty(datasetList, "Dataset list should not be empty");
            Assert.IsTrue(datasetList.Any(d => d.Name == dataset.Name), "New dataset should be added to the database");
            Assert.That(datasetList, Has.Exactly(1).Items);
            //check files db
            var fileList = context.Files.ToList();
            Assert.IsNotEmpty(fileList, "File list should not be empty");
            Assert.IsTrue(fileList.Any(f => f.Name == configFile.File.Name), "Config file should be added to the database");
            Assert.IsTrue(fileList.Any(f => f.Name == datasetFile.File.Name), "Dataset file should be added to the database");
            Assert.That(fileList, Has.Exactly(2).Items);
            //check blobs
            var blobConfig = _blobClient.GetBlobContainerClient(_config["Databases:BlobStorage:ConfigContainerName"])
                .GetBlobClient(configFile.File.BlobGuid.ToString())
                ;
            Assert.IsTrue(await blobConfig.ExistsAsync(), "Config file blob should exist in the blob storage");
            Assert.That(blobConfig.Name, Is.EqualTo(configFile.File.BlobGuid.ToString()), "Blob name should match the file BlobGuid");
            var blobDataset = _blobClient.GetBlobContainerClient(_config["Databases:BlobStorage:DatasetContainerName"])
                .GetBlobClient(datasetFile.File.BlobGuid.ToString());
            Assert.IsTrue(await blobDataset.ExistsAsync(), "Dataset file blob should exist in the blob storage");
            Assert.That(blobDataset.Name, Is.EqualTo(datasetFile.File.BlobGuid.ToString()), "Blob name should match the file BlobGuid");

        }

        [Test]
        public async Task Test_AddDatasetAsync_ThrowNullException()
        {
            var serv = _serviceProvider.GetRequiredService<IDatasetServices>();
            var dataset = new Dataset() { Id = Guid.NewGuid(), Name = "new dataset" };
            var configFile = new FileUpload
            {
                File = new File
                {
                    Id = Guid.NewGuid(),
                    Name = "ConfigFile",
                    Container = "Container",
                    Extension = "ext",
                    Url = "http://example.com/config.ext"
                },
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes("Config file content"))
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await serv.AddDatasetAsync(dataset, configFile, null);
            });
        }

        [Test]
        public async Task Test_AddDatasetAsync_ThrowInnerNullException()
        {
            var serv = _serviceProvider.GetRequiredService<IDatasetServices>();
            var dataset = new Dataset() { Id = Guid.NewGuid(), Name = "new dataset" };
            var configFile = new FileUpload
            {
                File = new File
                {
                    Id = Guid.NewGuid(),
                    Name = "ConfigFile",
                    Container = "Container",
                    Extension = "ext",
                    Url = "http://example.com/config.ext"
                },
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes("Config file content"))
            };
            var datasetFile = new FileUpload
            {
                File = new File
                {
                    Id = Guid.NewGuid(),
                    Name = "DatasetFile",
                    Container = "Container",
                    Extension = "ext",
                    Url = "http://example.com/dataset.ext"
                },
                DataStream = null
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await serv.AddDatasetAsync(dataset, configFile, datasetFile);
            });
        }



        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            var blobclient = _serviceProvider.GetRequiredService<BlobServiceClient>();
            blobclient.DeleteBlobContainer(_config["Databases:BlobStorage:ConfigContainerName"]);
            blobclient.DeleteBlobContainer(_config["Databases:BlobStorage:DatasetContainerName"]);
            
            var client = new MongoClient(_mongoConnectionString);
            await client.DropDatabaseAsync(_testDbName);
            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }

}
