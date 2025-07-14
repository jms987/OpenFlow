using System;
using System.Collections.Generic;
using OpenFlowWebServer.Domain.Entities;
using Xunit;

namespace OpenFlowWebServer.Domain.UnitTests.Domain
{
    public class LogTest
    {
        [Fact]
        public void Constructor_ShouldInitializeIdAndLogData()
        {
            // Act
            var log = new Log();

            // Assert
            Assert.NotEqual(Guid.Empty, log.Id);
            Assert.NotNull(log.LogData);
            Assert.Empty(log.LogData);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var deviceId = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;
            var device = new Device();
            var logData = new Dictionary<string, string>
            {
                { "Temperature", "22.5" },
                { "Status", "OK" }
            };

            var log = new Log
            {
                DeviceId = deviceId,
                Timestamp = timestamp,
                Device = device,
                LogData = logData
            };

            // Assert
            Assert.Equal(deviceId, log.DeviceId);
            Assert.Equal(timestamp, log.Timestamp);
            Assert.Equal(device, log.Device);
            Assert.Equal(logData, log.LogData);
            Assert.Equal("22.5", log.LogData["Temperature"]);
            Assert.Equal("OK", log.LogData["Status"]);
        }
    }
}