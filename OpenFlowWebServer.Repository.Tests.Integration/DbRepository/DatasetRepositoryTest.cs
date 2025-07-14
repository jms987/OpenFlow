using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;

namespace OpenFlowWebServer.Repository.Tests.Integration.DbRepository;
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

        var client = new MongoClient(_mongoConnectionString);
        var db = client.GetDatabase(_testDbName);
        var collection = db.GetCollection<Dataset>("Datasets"); // Uwaga na nazwę kolekcji

        collection.InsertMany(new[]
        {
            new Dataset { Id = Guid.NewGuid(), Name = "Dataset 1" , Description="abcd"},
            new Dataset { Id = Guid.NewGuid(), Name = "Dataset 2" , Description="abcd"}
        });

    }

    [Test]
    public async Task AddAndGet_ShouldPersistDataset()
    {
        // Arrange
        var repo = _serviceProvider.GetRequiredService<IDatasetRepository>();
        var dataset = new Dataset { Name = "Integration Dataset", Id = Guid.NewGuid() };
        Assert.That(dataset.Id, Is.Not.EqualTo(Guid.Empty), "Dataset ID should not be empty");
        Assert.AreEqual(Has.Exactly(2).Items, repo.GetAllAsync().Result.Count);
        // Act
        await repo.AddAsync(dataset);
        await repo.SaveChangesAsync(); // Uwaga: await potrzebny, jeśli SaveChangesAsync zwraca Task
        var all = await repo.GetAllAsync();
        var byId = await repo.GetByIdAsync(dataset.Id);

        // Assert
        Assert.That(all, Has.Exactly(3).Items);
        Assert.That(byId.Name, Is.EqualTo("Integration Dataset"));
        Assert.That(byId.Id, Is.EqualTo(dataset.Id));
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