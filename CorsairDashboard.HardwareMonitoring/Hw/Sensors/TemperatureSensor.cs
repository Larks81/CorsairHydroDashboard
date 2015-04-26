using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw.Sensors
{
    [DataContract]
    public class TemperatureSensor : HardwareSensor
    {
        public TemperatureSensor(String sensorId, float value)
        {
            Id = sensorId;
            Name = "Temperature";
            Value = value;
        }
    }
}
