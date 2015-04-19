using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw.Sensors
{
    public class TemperatureSensor : IHardwareSensor
    {
        public string Id { get; internal set; }

        public string Name
        {
            get
            {
                return "Temperature";
            }
        }

        public object Value { get; internal set; }
    }
}
