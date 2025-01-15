using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SensorManagement.Application.Commands.UpdateSensor
{
    public record UpdateSensorCommand(Guid Id, string Name, string Type) : IRequest<bool>;

}
