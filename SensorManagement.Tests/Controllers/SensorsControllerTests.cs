using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using SensorManagement.API.Controllers;
using SensorManagement.Application.Commands.CreateSensor;
using SensorManagement.Application.Commands.DeleteSensor;
using SensorManagement.Application.Commands.UpdateSensor;
using SensorManagement.Application.Queries.GetSensor;
using SensorManagement.Caching;
using SensorManagement.Domain.Entities;

namespace SensorManagement.Tests.Controllers
{
    [TestFixture]
    public class SensorsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ICacheService> _cacheServiceMock;
        private SensorsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _cacheServiceMock = new Mock<ICacheService>();
            _controller = new SensorsController(_mediatorMock.Object, _cacheServiceMock.Object);
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSensorIsCreated()
        {
            // Arrange
            var command = new CreateSensorCommand("Test Sensor", "Type A");
            var sensorId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(sensorId);

            // Act
            var result = await _controller.Create(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var actionResult = (CreatedAtActionResult)result;
            actionResult.ActionName.Should().Be(nameof(_controller.GetById));
        }

        [Test]
        public async Task GetById_ShouldReturnOk_WhenSensorIsFoundInCache()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var cacheKey = $"Sensor-{sensorId}";
            var sensor = new Sensor { Id = sensorId, Name = "Cached Sensor", Type = "Type A" };
            _cacheServiceMock.Setup(c => c.GetAsync<Sensor>(cacheKey)).ReturnsAsync(sensor);

            // Act
            var result = await _controller.GetById(sensorId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(sensor);
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenSensorIsNotFound()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var cacheKey = $"Sensor-{sensorId}";
            _cacheServiceMock.Setup(c => c.GetAsync<Sensor>(cacheKey)).ReturnsAsync((Sensor)null);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSensorQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Sensor)null);

            // Act
            var result = await _controller.GetById(sensorId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task Update_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var command = new UpdateSensorCommand(sensorId, "Updated Sensor", "Type B");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(sensorId, command);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }


        [Test]
        public async Task Delete_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSensorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(sensorId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenSensorDoesNotExist()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSensorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(sensorId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
