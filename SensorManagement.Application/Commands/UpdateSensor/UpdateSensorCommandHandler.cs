using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SensorManagement.Application.Interfaces;

namespace SensorManagement.Application.Commands.UpdateSensor
{
    public class UpdateSensorCommandHandler : IRequestHandler<UpdateSensorCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSensorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateSensorCommand request, CancellationToken cancellationToken)
        {
            var sensor = await _unitOfWork.Sensors.GetByIdAsync(request.Id);

            if (sensor == null)
            {
                return false;
            }

            sensor.Name = request.Name;
            sensor.Type = request.Type;
            sensor.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Sensors.Update(sensor);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}


