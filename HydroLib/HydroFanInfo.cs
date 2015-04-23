using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroFanInfo
    {
        private readonly object settingValue;

        public int Number { get; private set; }

        public int Rpm { get; private set; }

        public FanMode Mode { get; private set; }

        public byte PwmValue
        {
            get { return (byte)settingValue; }
        }

        public UInt16 RpmValue
        {
            get { return Convert.ToUInt16(settingValue); }
        }

        public byte[][] RpmsAndTempsTable
        {
            get { return (byte[][])settingValue; }
        }

        internal HydroFanInfo(int fanNr, int rpm, FanMode mode, object settingValue)
        {
            Number = fanNr;
            Rpm = rpm;
            Mode = mode;
            this.settingValue = settingValue;
        }
    }
}
