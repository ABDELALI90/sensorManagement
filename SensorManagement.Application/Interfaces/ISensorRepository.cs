
using SensorManagement.Domain.Entities;

namespace SensorManagement.Application.Interfaces
{
   
        public interface ISensorRepository
        {
            Task<Sensor> GetByIdAsync(Guid id);
            Task<IEnumerable<Sensor>> GetAllAsync();
            Task AddAsync(Sensor sensor);
            void Remove(Sensor sensor);
            void Update(Sensor sensor); 
    }
    

}
