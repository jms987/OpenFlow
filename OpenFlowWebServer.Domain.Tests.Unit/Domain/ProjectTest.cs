using System;
using System.Collections.Generic;
using OpenFlowWebServer.Domain.Entities;
using Xunit;

namespace OpenFlowWebServer.Domain.UnitTests.Domain
{
    public class ProjectTest
    {
        [Fact]
        public void Constructor_ShouldInitializeIdAndCollections()
        {
            // Act
            var project = new Project();

            // Assert
            Assert.NotEqual(Guid.Empty, project.Id);
            Assert.NotNull(project.Datasets);
            Assert.Empty(project.Datasets);
            Assert.NotNull(project.Models);
            Assert.Empty(project.Models);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var datasets = new List<Dataset>
            {
                new Dataset { Name = "Dataset 1" },
                new Dataset { Name = "Dataset 2" }
            };
            var models = new List<Model>
            {
                new Model { ModelName = "Model 1" },
                new Model { ModelName = "Model 2" }
            };

            var project = new Project
            {
                ProjectName = "Test Project",
                ProjectDescription = "Opis projektu",
                ChooseMethod = "Metoda wyboru",
                Datasets = datasets,
                Models = models
            };

            // Assert
            Assert.Equal("Test Project", project.ProjectName);
            Assert.Equal("Opis projektu", project.ProjectDescription);
            Assert.Equal("Metoda wyboru", project.ChooseMethod);
            Assert.Equal(datasets, project.Datasets);
            Assert.Equal(models, project.Models);
            Assert.Equal(2, project.Datasets.Count);
            Assert.Equal(2, project.Models.Count);
            Assert.Equal("Dataset 1", project.Datasets.First().Name);
            Assert.Equal("Model 1", project.Models.First().ModelName);
        }
    }
}