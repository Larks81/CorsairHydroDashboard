using CorsairDashboard.HardwareMonitoring.Hw.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw
{
    [DataContract]
    public class Mainboard : Hardware
    {
        public override HardwareKind Kind
        {
            get
            {
                return HardwareKind.Mainboard;
            }
        }

        public Mainboard(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddTemperature(String sensorId, String sensorName, float temp)
        {
            AddSensor(new TemperatureSensor(sensorId, sensorName, temp));
        }
    }
}
