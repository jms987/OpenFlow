using OpenFlowWebServer.Domain.Entities;

namespace OpenFlowWebServer.Domain.UnitTests.Domain
{
    public class HyperparameterTest
    {
        [Fact]
        public void Constructor_ShouldInitializeId()
        {
            // Act
            var hyperparameter = new Hyperparameter();

            // Assert
            Assert.NotEqual(Guid.Empty, hyperparameter.Id);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var modelId = Guid.NewGuid();
            var model = new Model();
            var hyperparameter = new Hyperparameter
            {
                ModelId = modelId,
                Type = "int",
                Description = "Liczba epok",
                Model = model,
                Name = "epochs",
                bottomRange = 1,
                upperRange = 100,
                step = 1
            };

            // Assert
            Assert.Equal(modelId, hyperparameter.ModelId);
            Assert.Equal("int", hyperparameter.Type);
            Assert.Equal("Liczba epok", hyperparameter.Description);
            Assert.Equal(model, hyperparameter.Model);
            Assert.Equal("epochs", hyperparameter.Name);
            Assert.Equal(1, hyperparameter.bottomRange);
            Assert.Equal(100, hyperparameter.upperRange);
            Assert.Equal(1, hyperparameter.step);
        }
    }
}