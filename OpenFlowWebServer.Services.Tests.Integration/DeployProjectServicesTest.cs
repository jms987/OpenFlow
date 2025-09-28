using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Domain;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using MongoDB.Driver;
using OpenFlowWebServer.Enums;
using OpenFlowWebServer.Repository;

namespace OpenFlowWebServer.Services.Tests.Integration
{
    [NonParallelizable]
    [TestFixture]
    public class DeployProjectServicesTest
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;
        private string _testDbName;
        private string _mongoConnectionString;
        private string _queueConnectionString;
        private Guid projectGuid;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            _testDbName = "TestDb_" + Guid.NewGuid();
            _mongoConnectionString = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING")
                                     ?? "mongodb://localhost:27017";
            _queueConnectionString = Environment.GetEnvironmentVariable("QUEUE_CONNECTIONSTRING")
                                    ?? "UseDevelopmentStorage=true";

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMongoDB(_mongoConnectionString, _testDbName));
            services.AddLogging();
            services.AddSingleton<QueueServiceClient>(new QueueServiceClient(_queueConnectionString));
            services.AddScoped<IModelRepository, ModelRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IHyperparameterRepository, HyperparameterRepository>();

            services.AddScoped<IQueueRepository, QueueRepository>();

            services.AddScoped<IParametersListServices, ParametersListServices>();
            services.AddScoped<IDeployProjectServices, DeployProjectServices>();

            _serviceProvider = services.BuildServiceProvider();

            var _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            projectGuid = Guid.NewGuid();
            _context.Projects.Add(new Project
            {
                Id = projectGuid,
                ProjectName = "Test Project",
                ProjectDescription = "This is a test project",
                ChooseMethod = ProjectDeployModels.DF.ToString(),
                IsDeployed = false
            });
            var firstModelId = Guid.NewGuid();
            _context.Models.AddAsync(new Model()
            {
                Id = firstModelId,
                ProjectId = projectGuid,
                ModelName = "Test Model",
                ModelDescription = "This is a test model",
                SearchMethod = "GS"
            });
            var secondModelId = Guid.NewGuid();
            _context.Models.AddAsync(new Model()
            {
                Id = secondModelId,
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
                    ModelId = firstModelId,
                    Name = "LearningRate",
                    bottomRange = 0.01m,
                    upperRange = 0.1m,
                    step = 0.01m
                },
                new Hyperparameter
                {
                    ModelId = firstModelId,
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
                    ModelId = secondModelId,
                    Name = "LearningRate",
                    bottomRange = 0.01m,
                    upperRange = 0.1m,
                    step = 0.01m
                },
                new Hyperparameter
                {
                    ModelId = secondModelId,
                    Name = "BatchSize",
                    bottomRange = 16m,
                    upperRange = 64m,
                    step = 16m
                }
            });
            
            _context.Datasets.Add(new Dataset
            {
                ProjectId = projectGuid,
                Name = "Test Dataset",
            });
            _context.Datasets.Add(new Dataset
            {
                ProjectId = projectGuid,
                Name = "Validation Dataset",
            });

            _context.SaveChanges();
        }

        [Test]
        [Repeat(10)]
        public async Task DeployProjectWorkProperly()
        {
            var deployProjectServices = _serviceProvider.GetRequiredService<IDeployProjectServices>();
            await deployProjectServices.DeployProjectAsync(projectGuid);
            //Docker image of queue service need to process messages
            //without this delay test fail
            Thread.Sleep(2000);
            var queueRepository = _serviceProvider.GetRequiredService<IQueueRepository>();
            var count = await queueRepository.GetCountAsync($"ProjectId{projectGuid.ToString()}Parameters");
           // Assert.IsTrue(count==200);
            Assert.AreEqual(200,count);
            var projectRepository = _serviceProvider.GetRequiredService<IProjectRepository>();
            var project = await projectRepository.GetByIdAsync(projectGuid);
            Assert.IsTrue(project.IsDeployed);
            Assert.AreEqual(200, project.InitialConfigurations);
        }

        [TearDown]
        public async Task QueueTeardown()
        {
            var queueClient = new QueueServiceClient(_queueConnectionString);
            var queue = queueClient.GetQueueClient($"ProjectId{projectGuid.ToString()}Parameters".ToLower());
            await queue.DeleteIfExistsAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            var client = new MongoClient(_mongoConnectionString);
            await client.DropDatabaseAsync(_testDbName);

            var queueClient = new QueueServiceClient(_queueConnectionString);
            var queue = queueClient.GetQueueClient($"ProjectId{projectGuid.ToString()}Parameters".ToLower());
            await queue.DeleteIfExistsAsync();


            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
