using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    
    public enum FanMode : byte
    {
        FixedPWM = 0x02,
        FixedRPM = 0x04,
        Default = 0x06,
        Quiet = 0x08,
        Balanced = 0x0a,
        Performance = 0x0c,
        Custom = 0x0e
    }
}
