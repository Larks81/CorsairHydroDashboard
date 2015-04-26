using System;
using System.Threading.Tasks;

namespace HydroLib
{
    public interface IHydroDevice : IDisposable
    {
        Task<HydroLedInfo> GetLedInfoAsync();

        Task<HydroFanInfo> GetFanInfoAsync(byte fanNr);

        Task<string> GetModelNameAsync();
        
        Task<int> GetNrOfFansAsync();

        Task<bool> SetFanModeAndValue(byte fanNr, FanMode mode, object value = null);
        
        Task<int> GetTemperatureAsync();
        
        Task<bool> SetLedCycleColorsAsync(byte[] firstColor, byte[] secondColor, byte[] thirdColor, byte[] fourthColor);
        
        Task<bool> SetLedSingleColorAsync(byte red, byte green, byte blue, bool pulse);

        Task<bool> UpdateReferenceTemperatureForFanAsync(byte fanNr, UInt16 temperature);
    }
}
