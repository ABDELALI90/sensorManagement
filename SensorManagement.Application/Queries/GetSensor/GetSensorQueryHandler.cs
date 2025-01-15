using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SensorManagement.Application.Interfaces;
using SensorManagement.Domain.Entities;

namespace SensorManagement.Application.Queries.GetSensor
{
    public class GetSensorQueryHandler : IRequestHandler<GetSensorQuery, Sensor>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSensorQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Sensor> Handle(GetSensorQuery request, CancellationToken cancellationToken)
        {
            var sensor = await _unitOfWork.Sensors.GetByIdAsync(request.Id);

            if (sensor == null)
            {
                // Return null if sensor is not found
                return null;
            }

            return sensor;
        }
    }
}
