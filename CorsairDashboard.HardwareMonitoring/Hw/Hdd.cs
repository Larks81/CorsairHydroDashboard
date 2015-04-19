using CorsairDashboard.HardwareMonitoring.Hw.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw
{
    public class Hdd : IHardware
    {
        private TemperatureSensor temperatureSensor;

        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public HardwareKind Kind { get; private set; }

        public IEnumerable<IHardwareSensor> Sensors
        {
            get
            {
                yield return temperatureSensor;
            }
        }

        public Hdd()
        {
            Kind = HardwareKind.HardDisk;
        }

        public void SetTemperature(String sensorId, float temp)
        {
            temperatureSensor = new TemperatureSensor()
            {
                Id = sensorId,
                Value = temp
            };
        }
    }
}
