using CorsairDashboard.ServiceWrapper;
using CorsairDashboard.ViewModels;

namespace CorsairDashboard.Caliburn
{
    public interface IShell
    {
        HydroDeviceDataProvider HydroDeviceDataProvider { get; }

        ReactiveHardwareMonitoring HardwareMonitoringProvider { get; }

        void ChangeCurrentDisplayedViewModelTo(ChildBaseViewModel newViewModel);
    }
}
