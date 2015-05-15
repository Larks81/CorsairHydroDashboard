using CorsairDashboard.ServiceWrapper;
using CorsairDashboard.Settings;
using CorsairDashboard.ViewModels;

namespace CorsairDashboard.Caliburn
{
    public interface IShell
    {
        HydroDeviceDataProvider HydroDeviceDataProvider { get; }

        ReactiveHardwareMonitoring HardwareMonitoringProvider { get; }

        ISettings Settings { get; }

        void ChangeCurrentDisplayedViewModelTo(ChildBaseViewModel newViewModel);
    }
}
