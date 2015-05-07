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
        public HydroColor Color1 { get; set; }

        [DataMember]
        public HydroColor Color2 { get; set; }

        [DataMember]
        public HydroColor Color3 { get; set; }

        [DataMember]
        public HydroColor Color4 { get; set; }

        [DataMember]
        public UInt16 TemperatureMin { get; set; }

        [DataMember]
        public UInt16 TemperatureMed { get; set; }

        [DataMember]
        public UInt16 TemperatureMax { get; set; }

        [DataMember]
        public LedMode Mode { get; set; }
    }
}
