using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SensorManagement.Application.Interfaces;
using SensorManagement.Domain.Entities;

namespace SensorManagement.Application.Commands.CreateSensor
{
    
        public class CreateSensorCommandHandler : IRequestHandler<CreateSensorCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSensorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
        {
            var sensor = new Sensor
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Sensors.AddAsync(sensor);
            await _unitOfWork.CommitAsync();

            return sensor.Id;
        }
    }


}
