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
        public int Number { get; internal set; }

        [DataMember]
        public bool IsConnected { get; internal set; }

        [DataMember]
        public bool IsFourPinFan { get; internal set; }

        [DataMember]
        public int Rpm { get; internal set; }

        [DataMember]
        public int MaxRpm { get; internal set; }

        [DataMember]
        public FanMode Mode { get; internal set; }

        [DataMember]
        public object RawValue { get; internal set; }

        internal HydroFanInfo(int fanNr, bool isConnected, bool isFourPin, int rpm, int maxRpm, FanMode mode, object settingValue)
        {
            Number = fanNr;
            IsConnected = isConnected;
            IsFourPinFan = isFourPin;
            Rpm = rpm;
            MaxRpm = maxRpm;
            Mode = mode;
            RawValue = settingValue;
        }
    }
}
