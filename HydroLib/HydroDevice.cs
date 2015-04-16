using HidLibrary;
using HydroLib.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroDevice : IHydroDevice
    {
        private const int WriteTimeOut = 3000;
        private const int ReadTimeOut = 3000;

        private HidDevice device;
        private HydroCommandsPayloadGenerator payloadGenerator;
        private SemaphoreSlim deviceIOLock;

        public HydroDevice(HidDevice hidDevice)
        {
            device = hidDevice;
            deviceIOLock = new SemaphoreSlim(1);
            payloadGenerator = new HydroCommandsPayloadGenerator();
        }

        #region Infos

        public async Task<String> GetModelNameAsync()
        {
            var modelNameCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.ProductName, nrOfBytesToRead: 8)
                    .Build();
            var responses = await QueryDeviceAsync(modelNameCommand);
            HydroCommandResponse resp;
            if (responses.TryGetResponseForCommand(modelNameCommand, out resp) && resp.IsSuccessful)
            {
                var name = Encoding.ASCII.GetString(resp.ResponseData).Trim('\0');
                return name;
            }
            return null;
        }

        #endregion

        #region Temps

        public async Task<int> GetTemperatureAsync()
        {
            var temperatureCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.TEMP_Read)
                    .Build();
            var responses = await QueryDeviceAsync(temperatureCommand);
            HydroCommandResponse resp;
            if (responses.TryGetResponseForCommand(temperatureCommand, out resp) && resp.IsSuccessful)
            {
                int temp = resp.ResponseData[1] << 8;
                temp += resp.ResponseData[0];
                temp = temp / 256;
                return temp;
            }
            return 0;
        }

        #endregion

        #region Leds

        public async Task<HydroLedInfo> GetLedInfoAsync()
        {
            var ledModeCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_Mode)
                    .Build();
            var ledColorsCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_CycleColors, nrOfBytesToRead: 0x0c)
                    .Build();
            var responses = await QueryDeviceAsync(ledModeCommand, ledColorsCommand);
            HydroCommandResponse modeResp, colorResp;
            if (responses.AreSuccessful && responses.TryGetResponseForCommand(ledModeCommand, out modeResp)
                && responses.TryGetResponseForCommand(ledColorsCommand, out colorResp))
            {
                var mode = (LedMode)modeResp.ResponseData[0];
                var colorArray = colorResp.ResponseData;
                var color1 = new[] { colorArray[0], colorArray[1], colorArray[2] };
                var color2 = new[] { colorArray[3], colorArray[4], colorArray[5] };
                var color3 = new[] { colorArray[6], colorArray[7], colorArray[8] };
                var color4 = new[] { colorArray[9], colorArray[10], colorArray[11] };
                return new HydroLedInfo()
                {
                    Mode = mode,
                    Color1 = color1,
                    Color2 = color2,
                    Color3 = color3,
                    Color4 = color4
                };
            }
            return null;
        }

        public async Task<bool> SetLedSingleColorAsync(byte red, byte green, byte blue, bool pulse)
        {
            var ledSelectCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_SelectCurrent)
                    .WithData(new byte[] { 0x00 })
                    .Build();
            var ledModeCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_Mode)
                    .WithData(new byte[] { pulse ? (byte)LedMode.TwoColorsCycle : (byte)LedMode.StaticColor })
                    .Build();
            var ledColorCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_CycleColors)
                    .WithData(new byte[] { red, green, blue, 0x00, 0x00, 0x00 })
                    .Build();

            var responses = await QueryDeviceAsync(ledSelectCommand, ledModeCommand, ledColorCommand);
            if (responses.AreSuccessful)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SetLedCycleColorsAsync(byte[] firstColor, byte[] secondColor, byte[] thirdColor, byte[] fourthColor)
        {
            if (firstColor == null || secondColor == null)
                throw new ArgumentNullException("first and second colors cannot be null");

            if (thirdColor == null)
                thirdColor = new byte[] { 0, 0, 0 };
            if (fourthColor == null)
                fourthColor = new byte[] { 0, 0, 0 };

            var ledMode = thirdColor == null ? LedMode.TwoColorsCycle : LedMode.FourColorCycle;
            var ledColorArray = firstColor.Concat(secondColor).Concat(thirdColor).Concat(fourthColor).ToArray();

            var ledSelectCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_SelectCurrent)
                    .WithData(new byte[] { 0x00 })
                    .Build();
            var ledModeCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_Mode)
                    .WithData(new byte[] { (byte)ledMode })
                    .Build();
            var ledColorCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.LED_CycleColors)
                    .WithData(ledColorArray)
                    .Build();

            var responses = await QueryDeviceAsync(ledSelectCommand, ledModeCommand, ledColorCommand);
            if (responses.AreSuccessful)
            {
                return true;
            }
            return false;
        }

        #endregion Leds

        #region Fans

        public async Task<int> GetNrOfFansAsync()
        {
            var nrOfFansCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Count)
                    .Build();
            var responses = await QueryDeviceAsync(nrOfFansCommand);
            HydroCommandResponse resp;
            if (responses.TryGetResponseForCommand(nrOfFansCommand, out resp) && resp.IsSuccessful)
            {
                return resp.ResponseData[0];
            }
            return 0;
        }

        public async Task<int> GetRpmForFanNrAsync(byte fanNr)
        {
            var fanSelectCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Select)
                    .WithData(new byte[] { fanNr })
                    .Build();
            var fanRpmCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_ReadRPM)
                    .Build();

            var responses = await QueryDeviceAsync(fanSelectCommand, fanRpmCommand);
            HydroCommandResponse rpmResp;
            if (responses.AreSuccessful && responses.TryGetResponseForCommand(fanRpmCommand, out rpmResp))
            {
                int rpm = rpmResp.ResponseData[1] << 8;
                rpm += rpmResp.ResponseData[0];
                return rpm;
            }
            return 0;

        }

        public async Task<HydroFanInfo> GetFanInfoAsync(byte fanNr)
        {
            var fanSelectCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Select)
                    .WithData(new byte[] { fanNr })
                    .Build();

            var fanModeCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Mode)
                    .Build();

            var fanRpmCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_ReadRPM)
                    .Build();

            var responses = await QueryDeviceAsync(fanSelectCommand, fanModeCommand, fanRpmCommand);
            HydroCommandResponse fanModeResp, fanRpmResp;
            if (responses.AreSuccessful && responses.TryGetResponseForCommand(fanModeCommand, out fanModeResp)
                && responses.TryGetResponseForCommand(fanRpmCommand, out fanRpmResp))
            {
                var fanInfo = new HydroFanInfo()
                {
                    Number = fanNr,
                    Rpm = (fanRpmResp.ResponseData[1] << 8) + fanRpmResp.ResponseData[0],
                    Mode = (FanMode)fanModeResp.ResponseData[0]
                };
                return fanInfo;
            }
            return null;
        }

        #endregion

        private async Task<HydroCommandResponses> QueryDeviceAsync(params HydroCommand[] commands)
        {
            await deviceIOLock.WaitAsync();
            try
            {
                OpenDevice();
                var payload = payloadGenerator.PayloadForCommands(commands);
                var writeOk = await device.WriteAsync(payload, WriteTimeOut);
                if (writeOk)
                {
                    var inputReport = await device.ReadReportAsync(ReadTimeOut);
                    if (inputReport.ReadStatus == HidDeviceData.ReadStatus.Success)
                    {
                        return HydroCommandResponses.Factory.FromDeviceResponse(inputReport.Data);
                    }
                }
                return HydroCommandResponses.EmptyResponse;
            }
            finally
            {
                deviceIOLock.Release();
            }
        }

        private bool OpenDevice()
        {
            if (device.IsOpen)
                return true;

            device.OpenDevice(DeviceMode.Overlapped, DeviceMode.Overlapped, ShareMode.Exclusive);
            device.MonitorDeviceEvents = true;
            return device.IsOpen && device.IsConnected;
        }

        public void Dispose()
        {
            device.CloseDevice();
            device.Dispose();
            device = null;
        }
    }
}
