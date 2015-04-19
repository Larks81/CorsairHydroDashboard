namespace CorsairDashboard.HardwareMonitoring
{
    public interface IHardwareMonitorAdapterCallback
    {
        void UpdateHardware(IHardware newHardware);
    }
}