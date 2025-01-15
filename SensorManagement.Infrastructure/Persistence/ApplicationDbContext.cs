using Microsoft.EntityFrameworkCore;
using SensorManagement.Domain.Entities;


namespace SensorManagement.Infrastructure.Persistence
{


    namespace SensorManagement.Infrastructure.Persistence
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options) { }

            public DbSet<Sensor> sensors { get; set; }
        }
    }



}
