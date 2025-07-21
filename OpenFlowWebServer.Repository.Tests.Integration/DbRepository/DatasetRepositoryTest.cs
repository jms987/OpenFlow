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
        

        context.Datasets.AddRange(new[]
        {
            new Dataset { Id = Guid.NewGuid(), Name = "Dataset 1", Description = "abcd",ConfigFile = new File{Id = Guid.NewGuid()},DatasetFile = new File(){Id = Guid.NewGuid()}},
            new Dataset { Id = Guid.NewGuid(), Name = "Dataset 2", Description = "abcd",ConfigFile = new File{Id = Guid.NewGuid()},DatasetFile = new File(){Id = Guid.NewGuid()}}
        });

        context.SaveChanges();

    }

   

    [Test]
    public async Task AddAndGet_ShouldPersistDataset()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var a = repo.GetAllAsync().Result;
        var dataset = new Dataset { Name = "Integration Dataset", Id = Guid.NewGuid(),ConfigFile = new File{Id = Guid.NewGuid()}};
        Assert.That(dataset.Id, Is.Not.EqualTo(Guid.Empty), "Dataset ID should not be empty");
        Assert.That(repo.GetAllAsync().Result.Count, Is.EqualTo(2));        // Act
        
        await repo.AddAsync(dataset);
        await repo.SaveChangesAsync(); // Uwaga: await potrzebny, jeśli SaveChangesAsync zwraca Task
        var all = await repo.GetAllAsync();
        var byId = await repo.GetByIdAsync(dataset.Id);

        // Assert
        Assert.That(all, Has.Exactly(3).Items);
        Assert.That(byId.Name, Is.EqualTo("Integration Dataset"));
        Assert.That(byId.Id, Is.EqualTo(dataset.Id));
        Assert.That(byId.ConfigFile.Id, Is.EqualTo(dataset.ConfigFile.Id), "ConfigFile ID should match");
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
    public async Task GetAll_ShouldReturnAllDatasets()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        // Act
        var allDatasets = await repo.GetAllAsync();
        // Assert
        Assert.That(allDatasets, Has.Exactly(3).Items, "Expected 3 datasets in the repository");
    }

    [Test]
    public async Task Delete_ShouldRemoveDataset()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var dataset = new Dataset { Name = "Dataset to Delete", Id = Guid.NewGuid() };
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

    private class TestDataset
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        [BsonId]
        public Guid ProjectId { get; set; }
        public string? Description { get; set; }
        [BsonId]
        public Guid ConfigFileId { get; set; }
        [BsonId]
        public Guid DatasetFileId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}