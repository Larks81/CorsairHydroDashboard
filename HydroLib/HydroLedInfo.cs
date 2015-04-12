using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroLedInfo
    {
        public byte[] Color1 { get; set; }

        public byte[] Color2 { get; set; }
        
        public byte[] Color3 { get; set; }
        
        public byte[] Color4 { get; set; }

        public LedMode Mode { get; set; }

        public bool IsPulsing()
        {
            return Mode == LedMode.TwoColorsCycle && Color2.SequenceEqual(new byte[] { 0, 0, 0 });
        }
    }
}
