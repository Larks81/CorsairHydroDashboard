using HydroLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Models
{
    public sealed class FanModeDescription
    {
        public static IEnumerable<FanModeDescription> GetFanModeDescriptions()
        {
            return Enum.GetValues(typeof(FanMode))
                .Cast<FanMode>()
                .Select(fm => new FanModeDescription()
                {
                    Description = GetEnumDescription(fm),
                    Value = (byte)fm
                });
        }

        private static String GetEnumDescription(FanMode fm)
        {
            switch (fm)
            {
                case FanMode.FixedPWM:
                    return "Fixed PWM";
                case FanMode.FixedRPM:
                    return "Fixed RPM";
                case FanMode.Default:
                    return "Default";
                case FanMode.Quiet:
                    return "Quiet";
                case FanMode.Balanced:
                    return "Balanced";
                case FanMode.Performance:
                    return "Performance";
                case FanMode.Custom:
                    return "Custom";
            }
            return null;
        }

        public string Description { get; private set; }

        public byte Value { get; private set; }

        private FanModeDescription() { }

        public override string ToString()
        {
            return Description;
        }
    }
}
