
using SensorManagement.Application.Interfaces; // For IUnitOfWork and ISensorRepository
using SensorManagement.Infrastructure.Repositories; // For SensorRepository
using SensorManagement.Infrastructure.Persistence;
using SensorManagement.Infrastructure.Persistence.SensorManagement.Infrastructure.Persistence; // For ApplicationDbContext

namespace SensorManagement.Infrastructure.UnitOfWork
{
  

    namespace SensorManagement.Infrastructure
    {
        public class UnitOfWork : IUnitOfWork
        {
            private readonly ApplicationDbContext _context;

            public UnitOfWork(ApplicationDbContext context)
            {
                _context = context;
                Sensors = new SensorRepository(_context);
            }

            public ISensorRepository Sensors { get; }

            public async Task<int> CommitAsync()
            {
                return await _context.SaveChangesAsync();
            }

            public void Dispose()
            {
                _context.Dispose();
            }
        }
    }


}
