using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SensorManagement.Domain.Entities;

namespace SensorManagement.Application.Queries.GetSensor
{
    public record GetSensorQuery(Guid Id) : IRequest<Sensor>;

}
