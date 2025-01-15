using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SensorManagement.Application.Interfaces;

namespace SensorManagement.Application.Commands.DeleteSensor
{
    public class DeleteSensorCommandHandler : IRequestHandler<DeleteSensorCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSensorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
        {
            var sensor = await _unitOfWork.Sensors.GetByIdAsync(request.Id);

            if (sensor == null)
            {
                return false;
            }

            _unitOfWork.Sensors.Remove(sensor);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
