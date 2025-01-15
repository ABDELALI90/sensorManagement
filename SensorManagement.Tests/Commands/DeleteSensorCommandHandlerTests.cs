using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SensorManagement.Application.Commands.DeleteSensor;
using SensorManagement.Application.Interfaces;
using SensorManagement.Domain.Entities;

namespace SensorManagement.Tests.Commands
{
    [TestFixture]
    public class DeleteSensorCommandHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private DeleteSensorCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteSensorCommandHandler(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTrue_WhenSensorExists()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor { Id = sensorId, Name = "Test Sensor", Type = "Type A" };

            _unitOfWorkMock.Setup(u => u.Sensors.GetByIdAsync(sensorId)).ReturnsAsync(sensor);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var command = new DeleteSensorCommand(sensorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.Sensors.Remove(sensor), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.Sensors.GetByIdAsync(sensorId)).ReturnsAsync((Sensor)null);

            var command = new DeleteSensorCommand(sensorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _unitOfWorkMock.Verify(u => u.Sensors.Remove(It.IsAny<Sensor>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
