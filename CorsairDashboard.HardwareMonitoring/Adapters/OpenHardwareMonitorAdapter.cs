using System;
using System.Linq;
using CorsairDashboard.HardwareMonitoring.Hw;
using OpenHardwareMonitor.Reactive;
using OpenHardwareMonitor.Hardware;

namespace CorsairDashboard.HardwareMonitoring.Adapters
{
    public class OpenHardwareMonitorAdapter : HardwareMonitoringAdapterBase, IObserver<OpenHardwareMonitor.Hardware.IHardware>
    {
        private ReactiveOpenHardwareMonitor reactiveOpenHardwareMonitor;
        private IDisposable subscriptionDisposable;

        public OpenHardwareMonitorAdapter()
            : base()
        {
            reactiveOpenHardwareMonitor = new ReactiveOpenHardwareMonitor();
            subscriptionDisposable = reactiveOpenHardwareMonitor.HardwareUpdatesObservable.Subscribe(this);
        }

        public override void Dispose()
        {
            subscriptionDisposable.Dispose();
            reactiveOpenHardwareMonitor.Dispose();
            reactiveOpenHardwareMonitor = null;
        }

        public override void BeginAdapt()
        {
            if (Callback == null)
                throw new NullReferenceException("Callback cannot be null");

            reactiveOpenHardwareMonitor.BeginMonitoring();
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(OpenHardwareMonitor.Hardware.IHardware hw)
        {
            CorsairDashboard.HardwareMonitoring.IHardware cHw = null;
            switch (hw.HardwareType)
            {
                case OpenHardwareMonitor.Hardware.HardwareType.CPU:
                    var cpu = new Cpu
                    {
                        Id = hw.Identifier.ToString(), 
                        Name = hw.Name
                    };
                    var cpuTempSensor = hw.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                    if (cpuTempSensor != null && cpuTempSensor.Value.HasValue)
                    {
                        cpu.SetTemperature(cpuTempSensor.Identifier.ToString(), cpuTempSensor.Value.Value);
                    }

                    foreach (var coreUsageSensor in hw.Sensors.Where(s => s.SensorType == SensorType.Load))
                    {
                        int coreNr;
                        if (!int.TryParse(coreUsageSensor.Name.Split('#').LastOrDefault(), out coreNr))
                            coreNr = 0;

                        cpu.SetUsageForCore(coreUsageSensor.Identifier.ToString(), coreNr, coreUsageSensor.Value.GetValueOrDefault());
                    }

                    cHw = cpu;
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.GpuAti:
                    break;
                case OpenHardwareMonitor.Hardware.HardwareType.GpuNvidia:
                    var gpu = new Gpu()
                    {
                        Id = hw.Identifier.ToString(),
                        Name = hw.Name
                    };
                    var gpuTempSensor = hw.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                    if (gpuTempSensor != null && gpuTempSensor.Value.HasValue)
                    {
                        gpu.SetTemperature(gpuTempSensor.Identifier.ToString(), gpuTempSensor.Value.Value);
                    }
                    cHw = gpu;
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.HDD:
                    var hdd = new Hdd
                    {
                        Id = hw.Identifier.ToString(), 
                        Name = hw.Name
                    };
                    var hddTempSensor = hw.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                    if (hddTempSensor != null && hddTempSensor.Value.HasValue)
                    {
                        hdd.SetTemperature(hddTempSensor.Identifier.ToString(), hddTempSensor.Value.Value);
                    }
                    cHw = hdd;
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.Heatmaster:
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.Mainboard:
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.RAM:
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.SuperIO:
                    break;

                case OpenHardwareMonitor.Hardware.HardwareType.TBalancer:
                    break;
            }

            if (cHw != null)
            {
                Callback.UpdateHardware(cHw);
            }
        }
    }
}