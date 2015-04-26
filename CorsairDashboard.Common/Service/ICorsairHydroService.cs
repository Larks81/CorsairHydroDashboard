
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Media;
using HydroLib;

namespace CorsairDashboard.Common.Service
{
    [ServiceContract(CallbackContract = typeof(ICorsairHydroServiceCallback))]
    public interface ICorsairHydroService
    {
        [OperationContract]
        CorsairHydroServiceState GetServiceState();

        [OperationContract]
        IEnumerable<ConnectedDeviceInfo> GetConnectedDevicesInfo();

        [OperationContract(IsOneWay = true)]
        void ForceRefreshConnectedDevicesInfo();

        [OperationContract(IsOneWay = true)]
        void SubscribeForUpdateForDevice(Guid deviceId);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeForUpdateForDevice(Guid deviceId);

        [OperationContract]
        int GetNumberOfFansForDevice(Guid deviceId);

        [OperationContract]
        HydroLedInfo GetLedInfoForDevice(Guid deviceId);

        [OperationContract]
        bool SetLedCycleColorsForDevice(Guid deviceId, byte[] color1, byte[] color2, byte[] color3, byte[] color4);

        [OperationContract]
        bool SetLedSingleColorForDevice(Guid deviceId, byte[] color, bool pulse);

        [OperationContract]
        bool SetPwmFanForDevice(Guid deviceId, int fanNr, byte pwmDutyCycle);

        [OperationContract]
        bool SetRpmFanForDevice(Guid deviceId, int fanNr, UInt16 rpm);

        [OperationContract]
        bool SetFanModeToDefaultProfileForDevice(Guid deviceId, int fanNr);

        [OperationContract]
        bool SetFanModeToQuietProfileForDevice(Guid deviceId, int fanNr);

        [OperationContract]
        bool SetFanModeToBalancedProfileForDevice(Guid deviceId, int fanNr);

        [OperationContract]
        bool SetFanModeToPerformanceProfileForDevice(Guid deviceId, int fanNr);

        [OperationContract]
        bool SetTemperatureBasedRpmFanForDevice(Guid deviceId, int fanNr, UInt16[] temperatures, UInt16[] rpms, string sensorId);
    }
}