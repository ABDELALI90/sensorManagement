using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace SensorManagement.Caching
{

    public static class RedisCacheServiceExtensions
    {
        public static IServiceCollection AddRedisCacheService(
            this IServiceCollection services,
            Action<RedisCacheOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<ICacheService, RedisCacheService>();
            return services;
        }
    }
}
