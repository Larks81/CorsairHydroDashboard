using System;

namespace CorsairDashboard.HydroService
{
    public partial class HydroFanInfo 
    {
        public byte PwmValue
        {
            get { return (byte)RawValue; }
        }

        public UInt16 RpmValue
        {
            get { return Convert.ToUInt16(RawValue); }
        }

        public Tuple<UInt16[], UInt16[], String> RmpsTempsAndSensorId
        {
            get
            {
                if (!(RawValue is Tuple<UInt16[], UInt16[]>))
                    return null;

                var rpmsAndTemps = (Tuple<UInt16[], UInt16[]>)RawValue;
                return new Tuple<UInt16[], UInt16[], String>(rpmsAndTemps.Item1, rpmsAndTemps.Item2, ExternalSensorId as String);
            }
        }
    }
}
