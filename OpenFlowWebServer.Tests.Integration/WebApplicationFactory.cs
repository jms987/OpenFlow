/*
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Moq;
using OpenFlowWebServer.Data;
using OpenFlowWebServer.Repository;


namespace OpenFlowWebServer.IntegrationTest
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Znajdź rejestrację DbContext i zastąp ją bazą danych w pamięci
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Dodaj bazę danych w pamięci dla testów
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Dodaj usługę IBrowserFileService do testowania
                services.AddScoped<IBrowserFileService, BrowserFileService>();

                // Skonfiguruj mockowane repozytoria
                services.AddScoped<IBlobRepository<Stream>>(sp =>
                {
                    var mockRepo = new Mock<IBlobRepository<Stream>>();
                    mockRepo.Setup(repo => repo.AddBlobAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                        .ReturnsAsync(new BlobRepository.Blob
                        {
                            BlobId = Guid.NewGuid(),
                            BlobUrl = "https://test-integration.com/blob"
                        });
                    return mockRepo.Object;
                });
            });
        }
    }
}
*/
