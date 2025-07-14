using OpenFlowWebServer.Domain.Entities;
using File = OpenFlowWebServer.Domain.Entities.File;
namespace OpenFlowWebServer.Domain.UnitTests.Domain;
public class DatasetTest
{
    [Fact]
    public void TestDatasetCreation()
    {
        var dataset = new Dataset();
        Assert.NotNull(dataset);
        Assert.IsType<Dataset>(dataset);
        Assert.NotNull(dataset.Id);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        // Arrange
        var configFile = new File
        {
            Id = Guid.NewGuid(),
            Name = "config.json",
            Extension = "json",
            Url = "http://test/config.json",
            Container = "test-container",
            BlobGuid = Guid.NewGuid()
        };

        var datasetFile = new File
        {
            Id = Guid.NewGuid(),
            Name = "data.csv",
            Extension = "csv",
            Url = "http://test/data.csv",
            Container = "test-container",
            BlobGuid = Guid.NewGuid()
        };

        var dataset = new Dataset
        {
            Name = "Test Dataset",
            ProjectId = Guid.NewGuid(),
            Description = "Opis testowy",
            ConfigFileId = configFile.Id,
            DatasetFileId = datasetFile.Id,
            ConfigFile = configFile,
            DatasetFile = datasetFile
        };

        // Assert
        Assert.Equal("Test Dataset", dataset.Name);
        Assert.Equal("Opis testowy", dataset.Description);
        Assert.Equal(configFile.Id, dataset.ConfigFileId);
        Assert.Equal(datasetFile.Id, dataset.DatasetFileId);
        Assert.Equal(configFile, dataset.ConfigFile);
        Assert.Equal(datasetFile, dataset.DatasetFile);
    }
}