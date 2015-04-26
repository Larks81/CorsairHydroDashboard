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
    public class Cpu : Hardware
    {
        [DataMember]
        public override HardwareKind Kind
        {
            get
            {
                return HardwareKind.Cpu;
            }        
        }

        public Cpu(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void SetTemperature(String sensorId, float temp)
        {
            AddSensor(new TemperatureSensor(sensorId, temp));
        }

        public void SetUsageForCore(String sensorId, int core, float usage)
        {
            AddSensor(new UsageSensor(sensorId, core, usage));
        }
    }
}
