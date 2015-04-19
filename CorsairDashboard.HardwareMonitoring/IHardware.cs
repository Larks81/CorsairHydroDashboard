using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring
{
    public interface IHardware
    {
        String Id { get; }

        String Name { get; }

        HardwareKind Kind { get; }

        IEnumerable<IHardwareSensor> Sensors { get; }
    }
}
