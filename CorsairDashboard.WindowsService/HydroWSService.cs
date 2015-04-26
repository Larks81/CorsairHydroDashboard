using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CorsairDashboard.Common.Service;
using HydroLib;
using log4net;
using CorsairDashboard.WindowsService.HardwareMonitoring;
using log4net.Util;

namespace CorsairDashboard.WindowsService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class HydroWSService : ICorsairHydroService, IHardwareMonitorServiceCallback, IDisposable
    {
        private CorsairHydroServiceState serviceState;
        private Dictionary<Guid, IHydroDevice> hydroDevices;
        private Dictionary<Guid, List<ICorsairHydroServiceCallback>> callbacks;
        private CancellationTokenSource updateStatsCancellationToken;
        private HardwareMonitorService hwMonitor;
        private ILog log;
        private Dictionary<Guid, int> cachedNrOfFans;
        private Dictionary<String, UInt16> sensorsValue;

        public HydroWSService(ILog log, HardwareMonitorService hwMonitor)
        {
            this.log = log;
            this.hwMonitor = hwMonitor;
            serviceState = CorsairHydroServiceState.Ready;
            callbacks = new Dictionary<Guid, List<ICorsairHydroServiceCallback>>();
            hydroDevices = new Dictionary<Guid, IHydroDevice>();
            updateStatsCancellationToken = new CancellationTokenSource();
            cachedNrOfFans = new Dictionary<Guid, int>();
            sensorsValue = new Dictionary<string, ushort>();

            log.Info("Loading settings...");
            ServiceSettings.LoadSettings();
            log.Info("Settings loaded");

            BeginUpdateStats();
        }

        private void BeginUpdateStats()
        {
            var token = updateStatsCancellationToken.Token;
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();

                        foreach (var kvp in hydroDevices)
                        {
                            var deviceId = kvp.Key;
                            var hydroDevice = kvp.Value;

                            var temp = await hydroDevice.GetTemperatureAsync();
                            NotifyForTemperatureChange(deviceId, temp);

                            var totalNrOfFans = cachedNrOfFans[deviceId];
                            for (byte i = 0; i < totalNrOfFans; i++)
                            {
                                var sensorId = ServiceSettings.GetFanSensorId(deviceId, i);
                                if (!String.IsNullOrEmpty(sensorId) && sensorsValue.ContainsKey(sensorId))
                                {
                                    var sensorTemperature = sensorsValue[sensorId];
                                    var updated = await hydroDevice.UpdateReferenceTemperatureForFanAsync(0, sensorTemperature);
                                    log.InfoFormat("Temperature of {0} degress reported {1}", sensorTemperature, updated ? "ok" : "with error");
                                }

                                var fanInfo = await hydroDevice.GetFanInfoAsync(i);
                                NotifyForFanInfoChange(deviceId, fanInfo);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }

                    await Task.Delay(2000, token);
                }
            }, updateStatsCancellationToken.Token);
        }

        public CorsairHydroServiceState GetServiceState()
        {
            return serviceState; //USELESS??
        }

        #region Subscription Handling Methods

        public void SubscribeForUpdateForDevice(Guid deviceId)
        {
            log.Info("Registering a client's subscription..");
            List<ICorsairHydroServiceCallback> callbacksForDeviceId;
            if (!callbacks.TryGetValue(deviceId, out callbacksForDeviceId))
            {
                callbacksForDeviceId = new List<ICorsairHydroServiceCallback>();
                callbacks.Add(deviceId, callbacksForDeviceId);
            }

            var callback = OperationContext.Current.GetCallbackChannel<ICorsairHydroServiceCallback>();
            if (!callbacksForDeviceId.Contains(callback))
            {
                OperationContext.Current.Channel.Closed += OnChannelClosedOrFaulted;
                OperationContext.Current.Channel.Faulted += OnChannelClosedOrFaulted;
                callbacksForDeviceId.Add(callback);
                log.InfoFormat("Client subscription for device {0} confirmed", deviceId);
            }
            else
            {
                log.WarnFormat("Client subscription ignored beacuse already added");
            }
        }

        void OnChannelClosedOrFaulted(object sender, EventArgs e)
        {
            var callback = sender as ICorsairHydroServiceCallback;
            if (callback != null)
            {
                var lists = callbacks.Where(kvp => kvp.Value.Contains(callback)).Select(kvp => kvp.Value);
                foreach (var callbacksList in lists)
                {
                    callbacksList.Remove(callback);
                }
                log.Warn("Closed a faulted connection");
            }
            else
            {
                log.WarnFormat("Cannot close a faulted channel");
            }
        }

        public void UnsubscribeForUpdateForDevice(Guid deviceId)
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<ICorsairHydroServiceCallback>();
                callbacks[deviceId].Remove(callback);
                log.InfoFormat("Client unsubscription for device {0} confirmed", deviceId);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Cannot unsubscribe client for device {0}:\n{1}", deviceId, e);
            }
        }

        private void NotifyForTemperatureChange(Guid deviceId, int temperature)
        {
            List<ICorsairHydroServiceCallback> callbacksForDeviceId;
            if (callbacks.TryGetValue(deviceId, out callbacksForDeviceId))
            {
                callbacksForDeviceId.ForEach(c => c.OnWaterTemperatureUpdateForDevice(deviceId, temperature));
            }
        }

        private void NotifyForFanInfoChange(Guid deviceId, HydroFanInfo fanInfo)
        {
            List<ICorsairHydroServiceCallback> callbacksForDeviceId;
            if (callbacks.TryGetValue(deviceId, out callbacksForDeviceId))
            {
                callbacksForDeviceId.ForEach(c =>
                {
                    try
                    {
                        c.OnFanInfoUpdateForDevice(deviceId, fanInfo);
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                });
            }
        }

        #endregion

        #region Device IO Methods

        public IEnumerable<ConnectedDeviceInfo> GetConnectedDevicesInfo()
        {
            if (!hydroDevices.Any())
                ForceRefreshConnectedDevicesInfo();

            return hydroDevices.Select(kvp => new ConnectedDeviceInfo(kvp.Key, kvp.Value.GetModelNameAsync().Result));
        }

        public void ForceRefreshConnectedDevicesInfo()
        {
            //can lead to problems if more clients are connected at the same time, but it's not an option at the moment..
            foreach (var hydroDevice in hydroDevices.Values)
            {
                hydroDevice.Dispose();
            }
            hydroDevices.Clear();

            var hydroEnumerator = new HydroDeviceEnumerator(canReturnNullDevice: true);
            var devices = new List<ConnectedDeviceInfo>(hydroEnumerator.Count());
            foreach (var dev in hydroEnumerator)
            {
                var guid = Guid.NewGuid();
                devices.Add(new ConnectedDeviceInfo(guid, dev.GetModelNameAsync().Result));
                hydroDevices.Add(guid, dev);
            }
        }

        public int GetNumberOfFansForDevice(Guid deviceId)
        {
            if (!cachedNrOfFans.ContainsKey(deviceId))
            {
                cachedNrOfFans.Add(deviceId, hydroDevices[deviceId].GetNrOfFansAsync().Result);
            }
            return cachedNrOfFans[deviceId];
        }

        public bool SetPwmFanForDevice(Guid deviceId, int fanNr, byte pwmDutyCycle)
        {
            return hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.FixedPWM, pwmDutyCycle).Result;
        }

        public bool SetRpmFanForDevice(Guid deviceId, int fanNr, ushort rpm)
        {
            return hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.FixedRPM, rpm).Result;
        }

        public bool SetFanModeToDefaultProfileForDevice(Guid deviceId, int fanNr)
        {
            return hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.Default).Result;
        }

        public bool SetFanModeToQuietProfileForDevice(Guid deviceId, int fanNr)
        {
            return hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.Quiet).Result;
        }

        public bool SetFanModeToBalancedProfileForDevice(Guid deviceId, int fanNr)
        {
            return hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.Balanced).Result;
        }

        public bool SetFanModeToPerformanceProfileForDevice(Guid deviceId, int fanNr)
        {
            return hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.Performance).Result;
        }

        public bool SetTemperatureBasedRpmFanForDevice(Guid deviceId, int fanNr, UInt16[] temperatures, UInt16[] rpms, string sensorId)
        {
            var tempsAndRpms = new Tuple<UInt16[], UInt16[]>(temperatures, rpms);
            var result = hydroDevices[deviceId].SetFanModeAndValue((byte)fanNr, FanMode.Custom, tempsAndRpms).Result;
            if (result)
            {
                ServiceSettings.SetFanSensorId(deviceId, fanNr, sensorId);
                hwMonitor.InternalSubscribe(this);
            }
            return result;
        }

        public bool SetLedSingleColorForDevice(Guid deviceId, byte[] color, bool pulse)
        {
            return hydroDevices[deviceId].SetLedSingleColorAsync(color[0], color[1], color[2], pulse).Result;
        }

        public bool SetLedCycleColorsForDevice(Guid deviceId, byte[] color1, byte[] color2, byte[] color3, byte[] color4)
        {
            return hydroDevices[deviceId].SetLedCycleColorsAsync(color1, color2, color3, color4).Result;
        }

        public HydroLedInfo GetLedInfoForDevice(Guid deviceId)
        {
            return hydroDevices[deviceId].GetLedInfoAsync().Result;
        }

        #endregion

        public void OnHardwareMonitorUpdate(CorsairDashboard.HardwareMonitoring.Hardware hardware)
        {
            foreach (var sensorId in ServiceSettings.GetAllFansSensorsId())
            {
                var sensor = hardware.Sensors.FirstOrDefault(s => s.Id == sensorId);
                if (sensor != null)
                {
                    var sensorTemperature = Convert.ToUInt16(sensor.Value);
                    sensorsValue[sensorId] = sensorTemperature;
                }
            }
        }

        public void Dispose()
        {
            log.Info("Saving settings...");
            ServiceSettings.SaveSettings();
            log.Info("Settings saved");

            hwMonitor.InternalUnsubscribe(this);
            updateStatsCancellationToken.Cancel();
            foreach (var device in hydroDevices.Values)
            {
                device.Dispose();
            }
            hydroDevices.Clear();
        }
    }
}
