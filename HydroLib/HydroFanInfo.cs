using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroFanInfo
    {
        public int Number { get; set; }

        public int Rpm { get; set; }

        public FanMode Mode { get; set; }
    }
}
