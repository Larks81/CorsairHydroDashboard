using System;

namespace CorsairDashboard.HardwareMonitoring
{
    public interface IHardwareSensor
    {
        String Id { get; }

        String Name { get; }

        object Value { get; }
    }
}