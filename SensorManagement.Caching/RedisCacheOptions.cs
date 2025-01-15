using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorManagement.Caching
{
    public class RedisCacheOptions
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public int ConnectTimeout { get; set; } = 5000;
        public int ConnectRetry { get; set; } = 5;
        public int KeepAlive { get; set; } = 30;
        public int DefaultDatabase { get; set; } = 0;
    }

}
