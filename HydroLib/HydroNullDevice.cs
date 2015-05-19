using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroNullDevice : IHydroDevice
    {
        HydroLedInfo ledInfo;
        List<HydroFanInfo> fansInfo;
        volatile UInt16[] extTempsForFans;
        CancellationTokenSource cancellationTokenSource;
        int deviceNumber;

        public HydroNullDevice(int fakeDeviceNumber)
        {
            deviceNumber = fakeDeviceNumber;
            ledInfo = new HydroLedInfo()
            {
                Color1 = new HydroColor(0x33, 0xf1, 0x90),
                Color2 = new HydroColor(0x10, 0x90, 0xf1),
                Color3 = new HydroColor(0xf1, 0x33, 0x80),
                Color4 = new HydroColor(0x13, 0xe0, 0x11),
                Mode = LedMode.FourColorCycle
            };
            fansInfo = new List<HydroFanInfo>();
            for (int i = 0; i < 4; i++)
            {
                fansInfo.Add(new HydroFanInfo(
                    fanNr: i,
                    isConnected: i % 2 == 0,
                    isFourPin: true,
                    rpm: (UInt16)new Random().Next(400, 2000),
                    maxRpm: 2000,
                    mode: FanMode.FixedRPM,
                    settingValue: (UInt16)0));
            }
            extTempsForFans = new UInt16[fansInfo.Count];
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Task.Run(async () =>
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    foreach (var fanInfo in fansInfo)
                    {
                        if (!fanInfo.IsConnected)
                            continue;

                        UInt16 currentFanRpm = 0;
                        switch (fanInfo.Mode)
                        {
                            case FanMode.FixedPWM:
                                var pwmValue = (byte)fanInfo.RawValue;
                                currentFanRpm = (UInt16)((double)fanInfo.MaxRpm * (double)pwmValue / 255.0f);
                                break;

                            case FanMode.FixedRPM:
                                currentFanRpm = (UInt16)fanInfo.RawValue;
                                break;

                            case FanMode.Default:
                                currentFanRpm = (UInt16)((double)fanInfo.MaxRpm * 0.50f);
                                break;

                            case FanMode.Quiet:
                                currentFanRpm = (UInt16)((double)fanInfo.MaxRpm * 0.30f);
                                break;

                            case FanMode.Balanced:
                                currentFanRpm = (UInt16)((double)fanInfo.MaxRpm * 0.70f);
                                break;

                            case FanMode.Performance:
                                currentFanRpm = (UInt16)fanInfo.MaxRpm;
                                break;

                            case FanMode.Custom:
                                //TODO: Internal or external sensor? For now just external is considered
                                var tempsAndRpms = (Tuple<UInt16[], UInt16[]>)fanInfo.RawValue;
                                var temps = tempsAndRpms.Item1;
                                var rpms = tempsAndRpms.Item2;
                                var reportedTemp = (double)extTempsForFans[fanInfo.Number];
                                int i = -1;
                                for (int j = 0; j < temps.Length; j++)
                                {
                                    if (reportedTemp >= temps[j])
                                    {
                                        i = j;
                                        break;
                                    }
                                }
                                if (i == -1)
                                {
                                    //reported temp is under the first step
                                    currentFanRpm = rpms.First();
                                }
                                else if (i == temps.Length - 1)
                                {
                                    //reported temp is above or equal to the last step
                                    currentFanRpm = rpms.Last();
                                }
                                else
                                {
                                    var minTemp = (double)temps[i];
                                    var maxTemp = (double)temps[i + 1];
                                    var weight = (reportedTemp - minTemp) / (maxTemp - minTemp);
                                    var minRpm = (double)rpms[i];
                                    var maxRpm = (double)rpms[i + 1];
                                    currentFanRpm = (UInt16)(minRpm + (maxRpm - minRpm) * weight);
                                }

                                break;
                        }                        
                        fanInfo.Rpm = currentFanRpm;
                    }

                    await Task.Delay(2000);
                }
            }, cancellationToken);
        }

        public Guid GetDeviceGuid()
        {
            using (var md5 = MD5.Create())
            {
                var devName = String.Format("Fake Hydro {0}", deviceNumber);
                var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(devName));
                var guid = new Guid(hash);
                return guid;
            }
        }

        public Task<HydroLedInfo> GetLedInfoAsync()
        {
            return Task.FromResult(ledInfo);
        }

        public Task<string> GetModelNameAsync()
        {
            return Task.FromResult("H100i Fake");
        }

        public Task<int> GetNrOfFansAsync()
        {
            return Task.FromResult(fansInfo.Count);
        }

        public Task<bool> SetFanModeAndValue(byte fanNr, FanMode mode, bool useExternalTemperatureSensorInCustomMode = false, object value = null)
        {
            if (fanNr >= fansInfo.Count)
                return Task.FromResult(false);

            var fanInfo = fansInfo[fanNr];
            if (fanInfo.IsConnected)
            {
                fanInfo.Mode = mode;
                fanInfo.RawValue = value;
            }
            return Task.FromResult(true);
        }

        public Task<int> GetTemperatureAsync()
        {
            return Task.FromResult(new Random().Next(18, 26));
        }

        public async Task<bool> SetLedModeAndValue(LedMode mode, object value)
        {
            switch (mode)
            {
                case LedMode.StaticColor:
                    if (!(value is HydroColor))
                        return false;

                    ledInfo.Color1 = (HydroColor)value;
                    break;

                case LedMode.TwoColorsCycle:
                    if (!(value is IEnumerable<HydroColor>))
                        return false;

                    var twoColorsArray = ((IEnumerable<HydroColor>)value).ToArray();
                    if (twoColorsArray.Length != 2)
                        return false;

                    ledInfo.Color1 = twoColorsArray.ElementAt(0);
                    ledInfo.Color2 = twoColorsArray.ElementAt(1);
                    break;

                case LedMode.FourColorCycle:
                    if (!(value is IEnumerable<HydroColor>))
                        return false;

                    var fourColorsArray = ((IEnumerable<HydroColor>)value).ToArray();
                    if (fourColorsArray.Length != 4)
                        return false;

                    ledInfo.Color1 = fourColorsArray.ElementAt(0);
                    ledInfo.Color2 = fourColorsArray.ElementAt(1);
                    ledInfo.Color3 = fourColorsArray.ElementAt(2);
                    ledInfo.Color4 = fourColorsArray.ElementAt(3);
                    break;

                case LedMode.TemperatureBased:
                    if (!(value is Tuple<UInt16[], HydroColor[]>))
                        return false;

                    var tuple = (Tuple<UInt16[], HydroColor[]>)value;
                    var temps = tuple.Item1;
                    var colors = tuple.Item2;
                    if (temps.Length != 3 || colors.Length != 3)
                        return false;

                    ledInfo.TemperatureMin = temps[0];
                    ledInfo.TemperatureMed = temps[1];
                    ledInfo.TemperatureMax = temps[2];
                    ledInfo.Color1 = colors[0];
                    ledInfo.Color2 = colors[1];
                    ledInfo.Color3 = colors[2];
                    break;
            }
            ledInfo.Mode = mode;
            return true;
        }

        public Task<HydroFanInfo> GetFanInfoAsync(byte fanNr)
        {
            if (fanNr >= fansInfo.Count)
                return Task.FromResult<HydroFanInfo>(null);

            return Task.FromResult(fansInfo[fanNr]);
        }

        public Task<bool> UpdateReferenceTemperatureForFanAsync(byte fanNr, UInt16 temperature)
        {
            if (fanNr >= extTempsForFans.Length)
                return Task.FromResult(false);

            extTempsForFans[fanNr] = temperature;
            return Task.FromResult(true);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
