using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HydroService
{
    public partial class HydroLedInfo
    {
        public bool IsPulsing()
        {
            return Mode == LedMode.TwoColorsCycle && Color2.SequenceEqual(new byte[] { 0, 0, 0 });
        }
    }
}
