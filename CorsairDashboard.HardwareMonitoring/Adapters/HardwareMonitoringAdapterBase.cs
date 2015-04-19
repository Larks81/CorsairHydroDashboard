using System;

namespace CorsairDashboard.HardwareMonitoring.Adapters
{
    public abstract class HardwareMonitoringAdapterBase : IDisposable
    {
        public IHardwareMonitorAdapterCallback Callback { get; set; }

        public abstract void BeginAdapt();

        public abstract void Dispose();
    }
}