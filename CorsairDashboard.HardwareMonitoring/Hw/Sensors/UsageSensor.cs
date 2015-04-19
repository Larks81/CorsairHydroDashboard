using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw.Sensors
{
    public class UsageSensor : IHardwareSensor
    {
        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public object Value { get; internal set; }
    }
}
