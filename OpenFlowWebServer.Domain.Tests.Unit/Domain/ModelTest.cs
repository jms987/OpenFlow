using System;
using System.Collections.Generic;
using OpenFlowWebServer.Domain.Entities;
using Xunit;

namespace OpenFlowWebServer.Domain.UnitTests.Domain
{
    public class ModelTest
    {
        [Fact]
        public void Constructor_ShouldInitializeIdAndHyperparameters()
        {
            // Act
            var model = new Model();

            // Assert
            Assert.NotEqual(Guid.Empty, model.Id);
            Assert.NotNull(model.Hyperparameters);
            Assert.Empty(model.Hyperparameters);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var hyperparameters = new List<Hyperparameter>
            {
                new Hyperparameter { Name = "learning_rate", bottomRange = 0.001m, upperRange = 0.1m, step = 0.001m }
            };

            var model = new Model
            {
                ModelName = "Test Model",
                ProjectId = projectId,
                ModelDescription = "Opis modelu",
                ModelType = "Regression",
                Hyperparameters = hyperparameters
            };

            // Assert
            Assert.Equal("Test Model", model.ModelName);
            Assert.Equal(projectId, model.ProjectId);
            Assert.Equal("Opis modelu", model.ModelDescription);
            Assert.Equal("Regression", model.ModelType);
            Assert.Equal(hyperparameters, model.Hyperparameters);
            Assert.Single(model.Hyperparameters);
            Assert.Equal("learning_rate", model.Hyperparameters.First().Name);
        }
    }
}