using CorsairDashboard.HardwareMonitoring.Hw.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw
{
    public class Cpu : IHardware
    {
        private TemperatureSensor temperatureSensor;
        private readonly List<UsageSensor> usageSensors;

        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public HardwareKind Kind { get; private set; }

        public IEnumerable<IHardwareSensor> Sensors
        {
            get
            {
                yield return temperatureSensor;
                foreach (var usageSensor in usageSensors)
                {
                    yield return usageSensor;
                }
            }
        }

        public Cpu()
        {
            Kind = HardwareKind.Cpu;
            usageSensors = new List<UsageSensor>();
        }

        public void SetTemperature(String sensorId, float temp)
        {
            temperatureSensor = new TemperatureSensor()
            {
                Id = sensorId,
                Value = temp
            };
        }

        public void SetUsageForCore(String sensorId, int core, float usage)
        {
            usageSensors.Add(new UsageSensor()
            {
                Id = sensorId,
                Name = String.Format("Core #{0} usage", core),
                Value = usage
            });
        }
    }
}
