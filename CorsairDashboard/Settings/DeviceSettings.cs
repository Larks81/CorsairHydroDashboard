using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Settings
{
    public class DeviceSettings
    {
        public Guid DeviceId { get; set; }

        public virtual ICollection<FanSettings> FansSettings { get; set; }
    }

    public class FanSettings
    {
        public Guid DeviceId { get; set; }

        public virtual DeviceSettings Device { get; set; }

        public int FanNr { get; set; }

        public String Label { get; set; }
    }
}
