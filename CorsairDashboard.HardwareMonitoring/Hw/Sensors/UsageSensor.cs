using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw.Sensors
{
    [DataContract]
    public class UsageSensor : HardwareSensor
    {
        public UsageSensor(String sensorId, int coreNr, float usage)
        {
            Id = sensorId;
            Name = String.Format("Core #{0} usage", coreNr);
            Value = usage;
        }
    }
}
