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

        public Tuple<UInt16[], UInt16[]> RpmsAndTempsTable
        {
            get { return (Tuple<UInt16[], UInt16[]>)RawValue; }
        }
    }
}
