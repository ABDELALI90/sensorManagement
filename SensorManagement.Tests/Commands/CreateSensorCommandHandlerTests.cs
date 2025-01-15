using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorManagement.Tests.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using global::SensorManagement.Application.Commands.CreateSensor;
    using global::SensorManagement.Application.Interfaces;
    using global::SensorManagement.Domain.Entities;
    using Moq;
    using NUnit.Framework;
 
    namespace SensorManagement.Tests.Commands
    {
        [TestFixture]
        public class CreateSensorCommandHandlerTests
        {
            private Mock<IUnitOfWork> _unitOfWorkMock;
            private CreateSensorCommandHandler _handler;

            [SetUp]
            public void SetUp()
            {
                _unitOfWorkMock = new Mock<IUnitOfWork>();
                _handler = new CreateSensorCommandHandler(_unitOfWorkMock.Object);
            }

            [Test]
            public async Task Handle_ShouldCreateSensorAndReturnId()
            {
                // Arrange
                var command = new CreateSensorCommand("Test Sensor", "Type A");
                var sensorId = Guid.NewGuid();

                _unitOfWorkMock.Setup(u => u.Sensors.AddAsync(It.IsAny<Sensor>()))
                    .Callback<Sensor>(sensor => sensor.Id = sensorId);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Should().Be(sensorId);
                _unitOfWorkMock.Verify(u => u.Sensors.AddAsync(It.Is<Sensor>(s =>
                    s.Name == command.Name &&
                    s.Type == command.Type &&
                    s.CreatedAt != default)), Times.Once);
                _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Test]
            public async Task Handle_ShouldThrowException_WhenAddAsyncFails()
            {
                // Arrange
                var command = new CreateSensorCommand("Test Sensor", "Type A");
                _unitOfWorkMock.Setup(u => u.Sensors.AddAsync(It.IsAny<Sensor>()))
                    .ThrowsAsync(new Exception("Database error"));

                // Act
                Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

                // Assert
                await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
                _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            }
        }
    }

}
