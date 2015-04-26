using System;
using System.ServiceModel;
using HydroLib;

namespace CorsairDashboard.Common.Service
{
    public interface ICorsairHydroServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnWaterTemperatureUpdateForDevice(Guid deviceId, int temperature);

        [OperationContract(IsOneWay = true)]
        void OnFanInfoUpdateForDevice(Guid deviceId, HydroFanInfo fanInfo);
    }
}