using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace HydroLib
{
    [DataContract]
    [KnownType(typeof(Tuple<UInt16[], UInt16[]>))]
    [ServiceKnownType(typeof(Tuple<UInt16[], UInt16[]>))]
    public class HydroFanInfo
    {
        [DataMember]
        public int Number { get; private set; }

        [DataMember]
        public int Rpm { get; private set; }

        [DataMember]
        public int MaxRpm { get; private set; }

        [DataMember]
        public FanMode Mode { get; private set; }

        [DataMember]        
        public object RawValue { get; private set; }

        internal HydroFanInfo(int fanNr, int rpm, int maxRpm, FanMode mode, object settingValue)
        {
            Number = fanNr;
            Rpm = rpm;
            MaxRpm = maxRpm;
            Mode = mode;
            RawValue = settingValue;
        }
    }
}
