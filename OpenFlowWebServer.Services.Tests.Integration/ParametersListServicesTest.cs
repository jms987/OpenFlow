using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;


namespace OpenFlowWebServer.Services.Tests.Integration
{
    [NonParallelizable]
    [TestFixture]
    public class ParametersListServicesTest
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;
        private string _testDbName;
        private string _mongoConnectionString;
        private Guid GSmodelId;
        private Guid RSmodelId;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            _testDbName = "TestDb_" + Guid.NewGuid();
            _mongoConnectionString = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING")
                                     ?? "mongodb://localhost:27017";

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMongoDB(_mongoConnectionString, _testDbName));
            services.AddLogging();
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Databases:BlobStorage:ConfigContainerName", $"test-config-container{Guid.NewGuid().ToString()}" },
                    { "Databases:BlobStorage:DatasetContainerName", $"test-dataset-container{Guid.NewGuid().ToString()}" }
                })
                .Build();

            services.AddSingleton<IConfiguration>(_config);
            services.AddScoped<IModelRepository, ModelRepository>();
            services.AddScoped<IParametersListServices, ParametersListServices>();
            _serviceProvider = services.BuildServiceProvider();

            var _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            var projectGuid = Guid.NewGuid();
            _context.Projects.Add(new Project
            {
                ProjectName = "Test Project",
                ProjectDescription = "This is a test project",
                ChooseMethod = "Random",
                IsDeployed = false
            });
            GSmodelId = Guid.NewGuid();
            _context.Models.AddAsync(new Model()
            {
                Id = GSmodelId,
                ProjectId = projectGuid,
                ModelName = "Test Model",
                ModelDescription = "This is a test model",
                SearchMethod = "GS"
            });
            RSmodelId = Guid.NewGuid();
            _context.Models.AddAsync(new Model()
            {
                Id = RSmodelId,
                ProjectId = projectGuid,
                ModelName = "Test Model",
                ModelDescription = "This is a test model",
                SearchMethod = "RS",
                Steps = 10,
            });
            _context.Hyperparameter.AddRangeAsync(new List<Hyperparameter>
            {
                new Hyperparameter
                {
                    ModelId = GSmodelId,
                    Name = "LearningRate",
                    bottomRange = 0.01m,
                    upperRange = 0.1m,
                    step = 0.01m
                },
                new Hyperparameter
                {
                    ModelId = GSmodelId,
                    Name = "BatchSize",
                    bottomRange = 16m,
                    upperRange = 64m,
                    step = 16m
                }
            });
            _context.Hyperparameter.AddRangeAsync(new List<Hyperparameter>
            {
                new Hyperparameter
                {
                    ModelId = RSmodelId,
                    Name = "LearningRate",
                    bottomRange = 0.01m,
                    upperRange = 0.1m,
                    step = 0.01m
                },
                new Hyperparameter
                {
                    ModelId = RSmodelId,
                    Name = "BatchSize",
                    bottomRange = 16m,
                    upperRange = 64m,
                    step = 16m
                }
            });
            _context.SaveChanges(); 
        }

        [Test]
        public async Task GetParametersList_ShouldReturnParametersList()
        {
            var parametersListService = _serviceProvider.GetRequiredService<IParametersListServices>();
            var parametersList = await parametersListService.CreateList(GSmodelId);
            Assert.IsNotNull(parametersList);
            Assert.IsTrue(parametersList.ParametersList.Any());
            Assert.That(parametersList.ParametersList, Has.Exactly(40).Items);
            //Assert.IsTrue(parametersList.All(p => p.Name != null && p.bottomRange >= 0 && p.upperRange > p.bottomRange));

        }

        [Test]
        public async Task GetParametersList_ShouldReturnEmptyList_WhenNoHyperparameters()
        {
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            var modelWithoutHyperparameters = new Model
            {
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                ModelName = "Model Without Hyperparameters",
                ModelDescription = "This model has no hyperparameters",
                SearchMethod = "GS"
            };
            context.Models.Add(modelWithoutHyperparameters);
            await context.SaveChangesAsync();
            var parametersListService = _serviceProvider.GetRequiredService<IParametersListServices>();
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await parametersListService.CreateList(modelWithoutHyperparameters.Id));
        }

        [Test]
        public async Task GetParametersList_ShouldThrowException_WhenModelNotFound()
        {
            var parametersListService = _serviceProvider.GetRequiredService<IParametersListServices>();
            var nonExistentModelId = Guid.NewGuid();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await parametersListService.CreateList(nonExistentModelId));
        }


        [Test]
        public async Task GetParametersList_ShouldReturnParameterListWhenRandomSearch()
        {
            var parametersListService = _serviceProvider.GetRequiredService<IParametersListServices>();
            var parametersList = await parametersListService.CreateList(RSmodelId);
            Assert.IsNotNull(parametersList);
            Assert.IsTrue(parametersList.ParametersList.Any());
            Assert.That(parametersList.ParametersList, Has.Exactly(10).Items);
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
}
