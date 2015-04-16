﻿using System;
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
                Color1 = new byte[] { 0x33, 0xf1, 0x90 },
                Color2 = new byte[3],
                Color3 = new byte[3],
                Color4 = new byte[3],
                Mode = LedMode.StaticColor
            });
        }

        public Task<string> GetModelNameAsync()
        {
            return Task.FromResult("H100i Fake");
        }

        public Task<int> GetNrOfFansAsync()
        {
            return Task.FromResult(1);
        }

        public Task<int> GetRpmForFanNrAsync(byte fanNr)
        {
            return Task.FromResult(1995);
        }

        public Task<int> GetTemperatureAsync()
        {
            return Task.FromResult(new Random().Next(18, 26));
        }

        public Task<bool> SetLedCycleColorsAsync(byte[] firstColor, byte[] secondColor, byte[] thirdColor, byte[] fourthColor)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetLedSingleColorAsync(byte red, byte green, byte blue, bool pulse)
        {
            return Task.FromResult(true);
        }

        public Task<HydroFanInfo> GetFanInfoAsync(byte fanNr)
        {
            return Task.FromResult(new HydroFanInfo() { Number = fanNr, Rpm = 2000 });
        }

        public void Dispose() { }
    }
}
