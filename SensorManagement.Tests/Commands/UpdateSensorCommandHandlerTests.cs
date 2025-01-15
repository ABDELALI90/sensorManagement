using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SensorManagement.Application.Commands.UpdateSensor;
using SensorManagement.Application.Interfaces;
using SensorManagement.Domain.Entities;

namespace SensorManagement.Tests.Commands
{
    [TestFixture]
    public class UpdateSensorCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private UpdateSensorCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateSensorCommandHandler(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenSensorExists()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Old Name", Type = "Old Type" };
            var command = new UpdateSensorCommand(sensorId, "Updated Name", "Updated Type");

            _unitOfWorkMock.Setup(u => u.Sensors.GetByIdAsync(sensorId)).ReturnsAsync(sensor);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            sensor.Name.Should().Be(command.Name);
            sensor.Type.Should().Be(command.Type);
            _unitOfWorkMock.Verify(u => u.Sensors.Update(sensor), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var command = new UpdateSensorCommand(sensorId, "Updated Name", "Updated Type");

            _unitOfWorkMock.Setup(u => u.Sensors.GetByIdAsync(sensorId)).ReturnsAsync((Sensor)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _unitOfWorkMock.Verify(u => u.Sensors.Update(It.IsAny<Sensor>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
