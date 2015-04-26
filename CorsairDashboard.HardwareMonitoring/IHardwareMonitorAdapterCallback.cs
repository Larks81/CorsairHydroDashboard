namespace CorsairDashboard.HardwareMonitoring
{
    public interface IHardwareMonitorAdapterCallback
    {
        void UpdateHardware(Hardware newHardware);
    }
}