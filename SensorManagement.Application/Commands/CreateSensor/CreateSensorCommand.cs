
using MediatR;

namespace SensorManagement.Application.Commands.CreateSensor
{
    public record CreateSensorCommand(string Name, string Type) : IRequest<Guid>;

}
