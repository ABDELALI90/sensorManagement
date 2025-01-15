using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorManagement.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISensorRepository Sensors { get; }
        Task<int> CommitAsync();
    }
}
