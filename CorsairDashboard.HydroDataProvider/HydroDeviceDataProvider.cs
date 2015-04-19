using HydroLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CorsairDashboard.Common.Extensions;

namespace CorsairDashboard.HydroDataProvider
{
    public class HydroDeviceDataProvider : IDisposable
    {
        public class FanInfos
        {
            internal ISubject<HydroFanInfo>[] fanInfos;

            public IObservable<HydroFanInfo> this[int fanNr]
            {
                get
                {
                    return fanInfos[fanNr];
                }
            }

            public FanInfos(int fanNr)
            {
                fanInfos = new ISubject<HydroFanInfo>[fanNr];
                for (int i = 0; i < fanNr; i++)
                {
                    fanInfos[i] = new BehaviorSubject<HydroFanInfo>(null);
                }
            }
        }

        private IHydroDevice hydroDevice;
        private CancellationTokenSource updateStatsCancellationToken;
        private bool isUpdating;

        private ISubject<int> temperatureSubject;
        private TaskCachedResult<String> modelName;
        private TaskCachedResult<int> nrOfFans;
        private ISubject<HydroLedInfo> ledInfoSubject;
        private bool ledInfoSubjectStateIsInvalid;
        private FanInfos fans;

        public IObservable<int> Temperature
        {
            get
            {
                return temperatureSubject;
            }
        }

        public Task<String> ModelName
        {
            get
            {
                return modelName.Value;
            }
        }

        public Task<int> NumberOfFans
        {
            get
            {
                return nrOfFans.Value;
            }
        }

        public FanInfos Fans
        {
            get
            {
                if (fans == null)
                {
                    var nrOfFansTask = nrOfFans.Value;
                    nrOfFansTask.ConfigureAwait(false);
                    var totalNrOfFans = nrOfFansTask.Result;
                    fans = new FanInfos(totalNrOfFans);
                }
                return fans;
            }
        }

        public IObservable<HydroLedInfo> Led
        {
            get { return ledInfoSubject; }
        }

        public HydroDeviceDataProvider(IHydroDevice device)
        {
            if (device == null)
                throw new ArgumentNullException("device cannot be null");

            hydroDevice = device;
            isUpdating = false;
            updateStatsCancellationToken = new CancellationTokenSource();

            temperatureSubject = new BehaviorSubject<int>(0);
            modelName = new TaskCachedResult<string>(hydroDevice.GetModelNameAsync());
            nrOfFans = new TaskCachedResult<int>(hydroDevice.GetNrOfFansAsync());
            ledInfoSubject = new BehaviorSubject<HydroLedInfo>(null);
            ledInfoSubjectStateIsInvalid = true;
        }

        public void BeginUpdate()
        {
            if (isUpdating)
                return;

            isUpdating = true;
            var token = updateStatsCancellationToken.Token;
            Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    var temp = await hydroDevice.GetTemperatureAsync();
                    temperatureSubject.OnNext(temp);

                    var totalNrOfFans = await nrOfFans.Value;
                    for (byte i = 0; i < totalNrOfFans; i++)
                    {
                        Fans.fanInfos[i].OnNext(await hydroDevice.GetFanInfoAsync(i));
                    }

                    if (ledInfoSubjectStateIsInvalid)
                    {
                        ledInfoSubjectStateIsInvalid = false;
                        ledInfoSubject.OnNext(await hydroDevice.GetLedInfoAsync());
                    }

                    await Task.Delay(500);
                }
            }, updateStatsCancellationToken.Token);
        }

        public Task<bool> SetLedCycleColorsAsync(Color color1, Color color2, Color? color3, Color? color4)
        {
            var ledTask = hydroDevice.SetLedCycleColorsAsync(
                color1.ToByteArray(),
                color2.ToByteArray(),
                color3.HasValue ? color3.Value.ToByteArray() : (byte[])null,
                color4.HasValue ? color4.Value.ToByteArray() : (byte[])null);
            ledTask.ContinueWith(ok => ledInfoSubjectStateIsInvalid = true);
            return ledTask;
        }

        public Task<bool> SetLedSingleColorAsync(Color color, bool pulse)
        {
            var ledTask = hydroDevice.SetLedSingleColorAsync(color.R, color.G, color.B, pulse);
            ledTask.ContinueWith(ok => ledInfoSubjectStateIsInvalid = true);
            return ledTask;
        }

        public Task<bool> SetPwmFanAsync(int fanNr, byte pwmDutyCycle)
        {
            return hydroDevice.SetFanModeAndValue((byte)fanNr, FanMode.FixedPWM, pwmDutyCycle);
        }

        public Task<bool> SetRpmFanAsync(int fanNr, UInt16 rpm)
        {
            return hydroDevice.SetFanModeAndValue((byte) fanNr, FanMode.FixedRPM, rpm);
        }

        public void Dispose()
        {
            updateStatsCancellationToken.Cancel();
            isUpdating = false;
        }
    }
}
