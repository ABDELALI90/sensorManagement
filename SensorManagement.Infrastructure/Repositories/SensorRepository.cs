using global::SensorManagement.Domain.Entities;
using global::SensorManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SensorManagement.Application.Interfaces;
using SensorManagement.Domain;
using SensorManagement.Infrastructure.Persistence.SensorManagement.Infrastructure.Persistence;

namespace SensorManagement.Infrastructure.Repositories
{
  
        public class SensorRepository : ISensorRepository
        {
            private readonly ApplicationDbContext _context;

            public SensorRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Sensor> GetByIdAsync(Guid id)
            {
                return await _context.sensors.FindAsync(id);
            }

            public async Task<IEnumerable<Sensor>> GetAllAsync()
            {
                return await _context.sensors.ToListAsync();
            }

            public async Task AddAsync(Sensor sensor)
            {
                await _context.sensors.AddAsync(sensor);
            }

            public void Remove(Sensor sensor)
            {
                _context.sensors.Remove(sensor);
            }

        public void Update(Sensor sensor)
        {
            _context.sensors.Update(sensor);
        }
    }
    

}
