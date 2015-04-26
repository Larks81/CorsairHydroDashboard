using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    [DataContract]
    public class HydroLedInfo
    {
        [DataMember]
        public byte[] Color1 { get; set; }

        [DataMember]
        public byte[] Color2 { get; set; }

        [DataMember]
        public byte[] Color3 { get; set; }

        [DataMember]
        public byte[] Color4 { get; set; }

        [DataMember]
        public LedMode Mode { get; set; }

        public bool IsPulsing()
        {
            return Mode == LedMode.TwoColorsCycle && Color2.SequenceEqual(new byte[] { 0, 0, 0 });
        }
    }
}
