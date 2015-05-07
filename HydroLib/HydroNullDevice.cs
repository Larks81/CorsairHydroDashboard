using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroNullDevice : IHydroDevice
    {
        public Task<HydroLedInfo> GetLedInfoAsync()
        {
            return Task.FromResult(new HydroLedInfo()
            {
                Color1 = new HydroColor(0x33, 0xf1, 0x90),
                Color2 = new HydroColor(0, 0, 0),
                Color3 = new HydroColor(0, 0, 0),
                Color4 = new HydroColor(0, 0, 0),
                Mode = LedMode.StaticColor
            });
        }

        public Task<string> GetModelNameAsync()
        {
            return Task.FromResult("H100i Fake");
        }

        public Task<int> GetNrOfFansAsync()
        {
            return Task.FromResult(2);
        }

        public Task<int> GetRpmForFanNrAsync(byte fanNr)
        {
            return Task.FromResult(1995);
        }

        public Task<int> GetTemperatureAsync()
        {
            return Task.FromResult(new Random().Next(18, 26));
        }

        public Task<bool> SetLedModeAndValue(LedMode mode, object value)
        {
            return Task.FromResult(true);
        }

        public Task<HydroFanInfo> GetFanInfoAsync(byte fanNr)
        {
            return Task.FromResult(new HydroFanInfo(fanNr, 2000, 2500, FanMode.FixedRPM, (byte)230));
        }

        public void Dispose() { }

        public Task<bool> SetFanModeAndValue(byte fanNr, FanMode mode, object value = null)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateReferenceTemperatureForFanAsync(byte fanNr, UInt16 temperature)
        {
            return Task.FromResult(true);
        }
    }
}
