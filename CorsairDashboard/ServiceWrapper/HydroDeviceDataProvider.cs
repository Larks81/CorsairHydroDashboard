using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CorsairDashboard.Common.Extensions;
using CorsairDashboard.Common.Service;
using CorsairDashboard.HydroService;
using ICorsairHydroService = CorsairDashboard.HydroService.ICorsairHydroService;
using ICorsairHydroServiceCallback = CorsairDashboard.HydroService.ICorsairHydroServiceCallback;

namespace CorsairDashboard.ServiceWrapper
{
    public class HydroDeviceDataProvider : ICorsairHydroServiceCallback
    {
        //private CancellationTokenSource updateStatsCancellationToken;
        private ICorsairHydroService service;
        private ISubject<int> temperatureSubject;
        private TaskCachedResult<int> nrOfFans;
        private TaskCachedResult<HydroLedInfo> ledInfo;
        private List<IObservable<HydroFanInfo>> fansInfo;
        private ConnectedDeviceInfo currentDeviceInfo;

        public Guid SelectedDeviceId
        {
            get
            {
                return currentDeviceInfo != null ? currentDeviceInfo.DeviceId : Guid.Empty;
            }
        }

        public IObservable<int> Temperature
        {
            get { return temperatureSubject; }
        }

        public String ModelName
        {
            get
            {
                return currentDeviceInfo.ModelName;
            }
        }

        public Task<int> NumberOfFans
        {
            get
            {
                return nrOfFans.Value;
            }
        }

        public IEnumerable<IObservable<HydroFanInfo>> Fans
        {
            get
            {
                if (fansInfo == null)
                {
                    var nrOfFansTask = nrOfFans.Value;
                    nrOfFansTask.ConfigureAwait(false);
                    var totalNrOfFans = nrOfFansTask.Result;
                    fansInfo = new List<IObservable<HydroFanInfo>>(totalNrOfFans);
                    for (int i = 0; i < totalNrOfFans; i++)
                    {
                        fansInfo.Add(new BehaviorSubject<HydroFanInfo>(null));
                    }
                }
                return fansInfo.AsReadOnly();
            }
        }

        public Task<HydroLedInfo> Led
        {
            get
            {
                return ledInfo.Value;
            }
        }

        public HydroDeviceDataProvider() { }

        public async Task Initialize(ICorsairHydroService service)
        {
            if (service == null)
                throw new ArgumentNullException("service cannot be null");

            this.service = service;
            currentDeviceInfo = (await service.GetConnectedDevicesInfoAsync()).First();
            await service.SubscribeForUpdateForDeviceAsync(currentDeviceInfo.DeviceId);

            temperatureSubject = new BehaviorSubject<int>(0);
            nrOfFans = new TaskCachedResult<int>(service.GetNumberOfFansForDeviceAsync(currentDeviceInfo.DeviceId));
            ledInfo = new TaskCachedResult<HydroLedInfo>(service.GetLedInfoForDeviceAsync(currentDeviceInfo.DeviceId));
        }

        public Task<bool> SetLedTemperatureBaseColorsAsync(UInt16 minTemp, UInt16 medTemp, UInt16 maxTemp,
            Color minTempColor, Color medTempColor, Color maxTempColor)
        {
            var task = service.SetLedTemperatureBasedColorForDeviceAsync(currentDeviceInfo.DeviceId,
                new[] { minTemp, medTemp, maxTemp },
                new[] { (HydroColor)minTempColor, (HydroColor)medTempColor, (HydroColor)maxTempColor });
            task.ContinueWith(res => ledInfo.Invalidate());
            return task;
        }

        public Task<bool> SetLedCycleColorsAsync(Color color1, Color color2, Color? color3, Color? color4)
        {
            var task = service.SetLedCycleColorsForDeviceAsync(
                currentDeviceInfo.DeviceId,
                (HydroColor)color1,
                (HydroColor)color2,
                color3 != null ? (HydroColor)color3.Value : null,
                color4 != null ? (HydroColor)color4.Value : null);
            task.ContinueWith(res => ledInfo.Invalidate());
            return task;
        }

        public Task<bool> SetLedSingleColorAsync(Color color, bool pulse)
        {
            var task = service.SetLedSingleColorForDeviceAsync(currentDeviceInfo.DeviceId, (HydroColor)color, pulse);
            task.ContinueWith(res => ledInfo.Invalidate());
            return task;
        }

        public Task<bool> SetPwmFanAsync(int fanNr, byte pwmDutyCycle)
        {
            return service.SetPwmFanForDeviceAsync(currentDeviceInfo.DeviceId, fanNr, pwmDutyCycle);
        }

        public Task<bool> SetRpmFanAsync(int fanNr, UInt16 rpm)
        {
            return service.SetRpmFanForDeviceAsync(currentDeviceInfo.DeviceId, fanNr, rpm);
        }

        public Task<bool> SetFanModeToDefaultProfileAsync(int fanNr)
        {
            return service.SetFanModeToDefaultProfileForDeviceAsync(currentDeviceInfo.DeviceId, fanNr);
        }

        public Task<bool> SetFanModeToQuietProfileAsync(int fanNr)
        {
            return service.SetFanModeToQuietProfileForDeviceAsync(currentDeviceInfo.DeviceId, fanNr);
        }

        public Task<bool> SetFanModeToBalancedProfileAsync(int fanNr)
        {
            return service.SetFanModeToBalancedProfileForDeviceAsync(currentDeviceInfo.DeviceId, fanNr);
        }

        public Task<bool> SetFanModeToPerformanceProfileAsync(int fanNr)
        {
            return service.SetFanModeToPerformanceProfileForDeviceAsync(currentDeviceInfo.DeviceId, fanNr);
        }

        public Task<bool> SetTemperatureBasedRpmFanAsync(int fanNr, UInt16[] temperatures, UInt16[] rpms, string sensorId)
        {
            return service.SetTemperatureBasedRpmFanForDeviceAsync(currentDeviceInfo.DeviceId, fanNr, temperatures, rpms, sensorId);
        }

        public void Dispose()
        {
            service.UnsubscribeForUpdateForDevice(currentDeviceInfo.DeviceId);
            temperatureSubject.OnCompleted();
            fansInfo.ForEach(fanInfoSignal => ((ISubject<HydroFanInfo>)fanInfoSignal).OnCompleted());
        }

        public void OnWaterTemperatureUpdateForDevice(Guid deviceId, int temperature)
        {
            temperatureSubject.OnNext(temperature);
        }

        public void OnFanInfoUpdateForDevice(Guid deviceId, HydroFanInfo fanInfo)
        {
            if (fanInfo != null)
            {
                var fanSubject = (ISubject<HydroFanInfo>)Fans.ElementAt(fanInfo.Number);
                fanSubject.OnNext(fanInfo);
            }
        }
    }
}
