using System;
using System.Threading.Tasks;

namespace HydroLib
{
    public interface IHydroDevice : IDisposable
    {
        Task<HydroLedInfo> GetLedInfoAsync();

        Task<string> GetModelNameAsync();
        
        Task<int> GetNrOfFansAsync();
        
        Task<int> GetRpmForFanNrAsync(byte fanNr);
        
        Task<int> GetTemperatureAsync();
        
        Task<bool> SetLedCycleColorsAsync(byte[] firstColor, byte[] secondColor, byte[] thirdColor, byte[] fourthColor);
        
        Task<bool> SetLedSingleColorAsync(byte red, byte green, byte blue, bool pulse);
    }
}
