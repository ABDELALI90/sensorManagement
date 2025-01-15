using MediatR;
using Microsoft.AspNetCore.Mvc;
using SensorManagement.Application.Commands.CreateSensor;
using SensorManagement.Application.Commands.UpdateSensor;
using SensorManagement.Application.Commands.DeleteSensor;
using SensorManagement.Application.Queries.GetSensor;
using SensorManagement.Caching;
using SensorManagement.Domain.Entities;
using SensorManagement.ErrorHandlingLibrary;

namespace SensorManagement.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class SensorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;

        public SensorsController(IMediator mediator, ICacheService cacheService)
        {
            _mediator = mediator;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSensorCommand command)
        {
            if (command == null)
                return BadRequest("Invalid sensor data.");

            var id = await _mediator.Send(command);

            var sensor = new Sensor { Id = id, Name = command.Name, Type = command.Type };
            var cacheKey = GetCacheKey(id);

            await _cacheService.SetAsync(cacheKey, sensor, TimeSpan.FromMinutes(30));

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cacheKey = GetCacheKey(id);
            var sensor = await _cacheService.GetAsync<Sensor>(cacheKey);

            if (sensor == null)
            {
                sensor = await _mediator.Send(new GetSensorQuery(id));
                if (sensor == null)
                    return NotFound();

                await _cacheService.SetAsync(cacheKey, sensor, TimeSpan.FromMinutes(10));
            }

            return Ok(sensor);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSensorCommand command)
        {
            if (command == null || id != command.Id)
                return BadRequest("Invalid sensor data or ID mismatch.");

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound();

            var updatedSensor = new Sensor { Id = id, Name = command.Name, Type = command.Type };
            var cacheKey = GetCacheKey(id);

            await _cacheService.SetAsync(cacheKey, updatedSensor, TimeSpan.FromMinutes(10));

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteSensorCommand(id));
            if (!result)
                return NotFound();

            var cacheKey = GetCacheKey(id);
            await _cacheService.RemoveAsync(cacheKey);

            return NoContent();
        }

        [HttpGet, MapToApiVersion("2.0")]
        public IActionResult GetV2()
        {
            return Ok("This is version 2.0");
        }

        private static string GetCacheKey(Guid id) => $"Sensor-{id}";
    }
}
