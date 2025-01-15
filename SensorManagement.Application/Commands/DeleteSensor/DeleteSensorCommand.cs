using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SensorManagement.Application.Commands.DeleteSensor
{
    public record DeleteSensorCommand(Guid Id) : IRequest<bool>;

}
