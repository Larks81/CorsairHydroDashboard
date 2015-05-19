using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using CorsairDashboard.Common.PauseToken;
using CorsairDashboard.HardwareMonitoring.Hw;
using log4net;
using OpenHardwareMonitor.Hardware;

namespace CorsairDashboard.WindowsService.HardwareMonitoring
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class HardwareMonitorService : ServiceBase, IHardwareMonitorService
    {
        const String NetNamedPipeUri = "net.pipe://localhost/CorsairHydroHardwareMonitorService";
        const int CallbackDefaultGroupId = 1; //no grouping in callback manager needed

        private readonly ILog log = LogManager.GetLogger("HwMonitorLogger");
        private ServiceHost serviceHost;
        private Computer computer;
        private CancellationTokenSource updateCancellationTokenSource;
        private PauseTokenSource pauseTokenSource;
        private WCFCallbackManager<int, IHardwareMonitorServiceCallback> callbackManager;
        private Thread hwMonitorThread;

        public HardwareMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service started");

            callbackManager = new WCFCallbackManager<int, IHardwareMonitorServiceCallback>();
            updateCancellationTokenSource = new CancellationTokenSource();
            pauseTokenSource = new PauseTokenSource() { IsPaused = true };
            computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true,
                MainboardEnabled = true,
                RAMEnabled = true,
                FanControllerEnabled = true
            };
            computer.Open();
            BeginMonitoring();

            serviceHost = ServiceHostFactory.ServiceHostForSingleInstance<IHardwareMonitorService>(this, NetNamedPipeUri, log);
        }

        protected override void OnStop()
        {
            callbackManager.Dispose();
            callbackManager = null;
            computer.Close();
            pauseTokenSource.IsPaused = false;
            updateCancellationTokenSource.Cancel();
            log.Info("Hardware monitor stopped");
        }

        private void BeginMonitoring()
        {
            cancellationToken = updateCancellationTokenSource.Token;
            pauseToken = pauseTokenSource.PauseToken;
            hwMonitorThread = new Thread(new ThreadStart(Monitor));
            hwMonitorThread.Name = "HwMonitor Thread";
            hwMonitorThread.Start();
        }

        CancellationToken cancellationToken;
        PauseToken pauseToken;

        private async void Monitor()
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await pauseToken.WaitWhilePausedAsync();

                foreach (var hardware in computer.Hardware)
                {
                    if (updateCancellationTokenSource.IsCancellationRequested)
                        return;

                    hardware.Update();
                    foreach (IHardware subHardware in hardware.SubHardware)
                        subHardware.Update();

                    CorsairDashboard.HardwareMonitoring.Hardware cHw = null;
                    switch (hardware.HardwareType)
                    {
                        case OpenHardwareMonitor.Hardware.HardwareType.CPU:
                            var cpu = new Cpu(id: hardware.Identifier.ToString(), name: hardware.Name);
                            var cpuTempSensor = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                            if (cpuTempSensor != null && cpuTempSensor.Value.HasValue)
                            {
                                cpu.SetTemperature(cpuTempSensor.Identifier.ToString(), cpuTempSensor.Value.Value);
                            }

                            foreach (var coreUsageSensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Load))
                            {
                                int coreNr;
                                if (!int.TryParse(coreUsageSensor.Name.Split('#').LastOrDefault(), out coreNr))
                                    coreNr = 0;

                                cpu.SetUsageForCore(coreUsageSensor.Identifier.ToString(), coreNr, coreUsageSensor.Value.GetValueOrDefault());
                            }

                            cHw = cpu;
                            break;

                        case OpenHardwareMonitor.Hardware.HardwareType.GpuAti:
                        case OpenHardwareMonitor.Hardware.HardwareType.GpuNvidia:
                            var gpu = new Gpu(id: hardware.Identifier.ToString(), name: hardware.Name);
                            var gpuTempSensor = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                            if (gpuTempSensor != null && gpuTempSensor.Value.HasValue)
                            {
                                gpu.SetTemperature(gpuTempSensor.Identifier.ToString(), gpuTempSensor.Value.Value);
                            }
                            cHw = gpu;
                            break;

                        case OpenHardwareMonitor.Hardware.HardwareType.HDD:
                            var hdd = new Hdd(id: hardware.Identifier.ToString(), name: hardware.Name);
                            var hddTempSensor = hardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                            if (hddTempSensor != null && hddTempSensor.Value.HasValue)
                            {
                                hdd.SetTemperature(hddTempSensor.Identifier.ToString(), hddTempSensor.Value.Value);
                            }
                            cHw = hdd;
                            break;

                        case OpenHardwareMonitor.Hardware.HardwareType.Heatmaster:
                            break;

                        case OpenHardwareMonitor.Hardware.HardwareType.Mainboard:
                            var mb = new Mainboard(id: hardware.Identifier.ToString(), name: hardware.Name);
                            foreach (var mbTempSensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Temperature))
                            {
                                mb.AddTemperature(mbTempSensor.Identifier.ToString(),
                                    mbTempSensor.Name, 
                                    mbTempSensor.Value.GetValueOrDefault());
                            }
                            foreach (var mbSubHw in hardware.SubHardware)
                            {
                                foreach (var mbTempSensor in mbSubHw.Sensors.Where(s => s.SensorType == SensorType.Temperature))
                                {
                                    mb.AddTemperature(mbTempSensor.Identifier.ToString(), 
                                        mbTempSensor.Name,
                                        mbTempSensor.Value.GetValueOrDefault());
                                }
                            }
                            cHw = mb;
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
                        callbackManager.NotifyAllClientsOfGroup(CallbackDefaultGroupId,
                            callback => callback.OnHardwareMonitorUpdate(cHw));
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private void PauseMonitoring()
        {
            pauseTokenSource.IsPaused = true;
        }

        private void ResumeMonitoring()
        {
            pauseTokenSource.IsPaused = false;
        }

        public void Subscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IHardwareMonitorServiceCallback>();
            InternalSubscribe(callback);
        }

        public void InternalSubscribe(IHardwareMonitorServiceCallback callback)
        {
            callbackManager.SubscribeClientForGroup(CallbackDefaultGroupId, callback);
            if (pauseTokenSource.IsPaused)
                pauseTokenSource.IsPaused = false;
        }

        public void Unsubscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IHardwareMonitorServiceCallback>();
            InternalUnsubscribe(callback);
        }

        public void InternalUnsubscribe(IHardwareMonitorServiceCallback callback)
        {
            callbackManager.UnsubscribeClientFromGroup(CallbackDefaultGroupId, callback);
            if (!callbackManager.HasAnyClientForGroup(CallbackDefaultGroupId) && !pauseTokenSource.IsPaused)
                pauseTokenSource.IsPaused = true;
        }
    }
}
