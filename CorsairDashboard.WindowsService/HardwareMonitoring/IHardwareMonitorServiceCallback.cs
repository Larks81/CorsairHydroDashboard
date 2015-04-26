using System.ServiceModel;
using CorsairDashboard.HardwareMonitoring;
using CorsairDashboard.HardwareMonitoring.Hw;

namespace CorsairDashboard.WindowsService.HardwareMonitoring
{
    public interface IHardwareMonitorServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        [ServiceKnownType(typeof(Cpu))]
        [ServiceKnownType(typeof(Gpu))]
        [ServiceKnownType(typeof(Hdd))]
        void OnHardwareMonitorUpdate(Hardware hardware);
    }
}