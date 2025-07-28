using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using File = OpenFlowWebServer.Domain.Entities.File;
namespace OpenFlowWebServer.Repository.Tests.Integration.DbRepository;
[NonParallelizable]
[TestFixture]
public class DatasetRepositoryMongoIntegrationTest
{
    private IServiceProvider _serviceProvider;
    private string _testDbName;
    private string _mongoConnectionString;
    private Guid ExistentId = Guid.NewGuid();
    private Guid NoConfigId = Guid.NewGuid();
    private Guid NoDatasetId = Guid.NewGuid();
    private File _tempFile;
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        _testDbName = "TestDb_" + Guid.NewGuid();
        _mongoConnectionString = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING")
                                 ?? "mongodb://localhost:27017";

        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMongoDB(_mongoConnectionString, _testDbName));
        services.AddScoped<IDatasetRepository, DatasetRepository>();

        _serviceProvider = services.BuildServiceProvider();
        
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        _tempFile = new File
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

        context.SaveChanges();

    }

    [Test]
    public async Task AddAndGet_ShouldPersistDataset()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var dataset = new Dataset { Name = "Integration Dataset", Id = Guid.NewGuid(),ConfigFileId = _tempFile.Id,DatasetFileId = _tempFile.Id };
        Assert.That(dataset.Id, Is.Not.EqualTo(Guid.Empty), "Dataset ID should not be empty");
        Assert.That(repo.GetAllAsync().Result.Count, Is.EqualTo(4));        // Act
        
        await repo.AddAsync(dataset);
        await repo.SaveChangesAsync(); // Uwaga: await potrzebny, jeśli SaveChangesAsync zwraca Task
        var all = await repo.GetAllAsync();
        var byId = await repo.GetByIdAsync(dataset.Id);

        // Assert
        Assert.That(all, Has.Exactly(5).Items);
        Assert.That(byId.Name, Is.EqualTo("Integration Dataset"));
        Assert.That(byId.Id, Is.EqualTo(dataset.Id));
        Assert.That(byId.ConfigFile.Id, Is.EqualTo(dataset.ConfigFile.Id), "ConfigFile ID should match");
    }

    [Test]
    public async Task Add_NoConfigFile_ShouldThrowError()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var dataset = new Dataset { Name = "Integration Dataset", Id = Guid.NewGuid(), DatasetFileId = _tempFile.Id };

       Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await repo.AddAsync(dataset);
            await repo.SaveChangesAsync();
        });

    }

    [Test]
    public async Task Add_NoDatasetFile_ShouldThrowError()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var dataset = new Dataset { Name = "Integration Dataset", Id = Guid.NewGuid(), ConfigFileId = _tempFile.Id };
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await repo.AddAsync(dataset);
            await repo.SaveChangesAsync();
        });
    }

    [Test]
    public async Task GetById_ShouldReturnNull_WhenDatasetNotFound()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var nonExistentId = Guid.NewGuid();
        // Act
        var result = await repo.GetByIdAsync(nonExistentId);
        // Assert
        Assert.IsNull(result, "Expected null for non-existent dataset");
    }

    [Test]
    public async Task GetById_ShouldReturnNull_WhenConfigFileNotFound()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await repo.GetByIdAsync(NoConfigId));
    }

    [Test]
    public async Task GetById_ShouldReturnNull_WhenDatasetFileNotFound()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await repo.GetByIdAsync(NoDatasetId));
    }


    [Test]
    public async Task GetById_ShouldReturnDataset()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();

        // Act
        var result = await repo.GetByIdAsync(ExistentId);
        // Assert
        Assert.IsNotNull(result, "Expected dataset to be found");

    }


    [Test]
    public async Task GetAll_ShouldReturnAllDatasets()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        // Act
        var allDatasets = await repo.GetAllAsync();
        // Assert
        Assert.That(allDatasets, Has.Exactly(5).Items, "Expected 4 datasets in the repository");
    }

    [Test]
    public async Task Delete_ShouldRemoveDataset()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var dataset = new Dataset { Name = "Dataset to Delete", Id = Guid.NewGuid(),ConfigFileId = _tempFile.Id,DatasetFileId = _tempFile.Id};
        await repo.AddAsync(dataset);
        await repo.SaveChangesAsync();
        var oldValue = repo.GetAllAsync().Result.Count;
        // Act
        await repo.DeleteAsync(dataset);
        await repo.SaveChangesAsync();
        // Assert
        Assert.That(await repo.GetAllAsync(), Has.Exactly(oldValue-1).Items);
        Assert.IsNull(await repo.GetByIdAsync(dataset.Id), "Deleted dataset should not be found");
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        var client = new MongoClient(_mongoConnectionString);
        await client.DropDatabaseAsync(_testDbName);
        if (_serviceProvider is IDisposable disposable)
            disposable.Dispose();
    }
    
}