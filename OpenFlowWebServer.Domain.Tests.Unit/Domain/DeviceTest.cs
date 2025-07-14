using System;
using OpenFlowWebServer.Domain.Entities;
using Xunit;

namespace OpenFlowWebServer.Domain.UnitTests.Domain
{
    public class DeviceTest
    {
        [Fact]
        public void Constructor_ShouldInitializeIdAndCreatedAt()
        {
            // Act
            var device = new Device();

            // Assert
            Assert.NotEqual(Guid.Empty, device.Id);
            Assert.True((DateTime.UtcNow - device.CreatedAt).TotalSeconds < 5);
        }

    }
}