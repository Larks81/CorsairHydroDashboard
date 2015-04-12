using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public enum LedMode : byte
    {
        StaticColor = 0x00,
        TwoColorsCycle = 0x4b,
        FourColorCycle = 0x8b,
        TemperatureBased = 0xc0
    }
}
