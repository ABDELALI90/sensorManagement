using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorManagement.Domain.Entities
{/// <summary>
/// Represents a sensor entity.
/// </summary>
    public class Sensor
    {
        /// <summary>
      /// Gets or sets the sensor ID.
      /// </summary>
        public Guid Id { get; set; }
        // <summary>
        /// Gets or sets the name of the sensor.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the type of the sensor.
        /// </summary>
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        
    }

}
