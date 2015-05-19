using System;
using System.Threading.Tasks;

namespace HydroLib
{
    public interface IHydroDevice : IDisposable
    {
        Guid GetDeviceGuid();

        Task<HydroLedInfo> GetLedInfoAsync();

        Task<HydroFanInfo> GetFanInfoAsync(byte fanNr);

        Task<string> GetModelNameAsync();
        
        Task<int> GetNrOfFansAsync();

        Task<bool> SetFanModeAndValue(byte fanNr, FanMode mode, bool useExternalTemperatureSensorInCustomMode = false, object value = null);
        
        Task<int> GetTemperatureAsync();

        Task<bool> SetLedModeAndValue(LedMode mode, object value);

        Task<bool> UpdateReferenceTemperatureForFanAsync(byte fanNr, UInt16 temperature);
    }
}
