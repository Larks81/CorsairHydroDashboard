using HidLibrary;
using HydroLib.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HydroLib.Extensions;

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

            var ledMode = thirdColor == null ? LedMode.TwoColorsCycle : LedMode.FourColorCycle;

            if (thirdColor == null)
                thirdColor = new byte[] { 0, 0, 0 };
            if (fourthColor == null)
                fourthColor = new byte[] { 0, 0, 0 };

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

            var fanRpmSettingCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_FixedRPM)
                    .Build();

            var fanPwmSettingCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_FixedPWM)
                    .Build();

            var fanRpmTableSettingCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_RPMTable, nrOfBytesToRead: 0x0a)
                    .Build();

            var fanTempTableSettingCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_TempTable, nrOfBytesToRead: 0x0a)
                    .Build();

            var responses = await QueryDeviceAsync(fanSelectCommand, fanModeCommand, fanRpmCommand, fanRpmSettingCommand,
                fanPwmSettingCommand, fanRpmTableSettingCommand, fanTempTableSettingCommand);
            HydroCommandResponse fanModeResp, fanRpmResp, fanRpmSettingResp, fanPwmSettingResp, fanRpmTableResp, fanTempTableResp;
            if (responses.AreSuccessful && responses.TryGetResponseForCommand(fanModeCommand, out fanModeResp)
                && responses.TryGetResponseForCommand(fanRpmCommand, out fanRpmResp)
                && responses.TryGetResponseForCommand(fanRpmSettingCommand, out fanRpmSettingResp)
                && responses.TryGetResponseForCommand(fanPwmSettingCommand, out fanPwmSettingResp)
                && responses.TryGetResponseForCommand(fanRpmTableSettingCommand, out fanRpmTableResp)
                && responses.TryGetResponseForCommand(fanTempTableSettingCommand, out fanTempTableResp))
            {

                object settingValue = null;
                var fanMode = (FanMode)(fanModeResp.ResponseData[0] & 0x0e);
                switch (fanMode)
                {
                    case FanMode.FixedPWM:
                        settingValue = fanPwmSettingResp.ResponseData[0];
                        break;
                    case FanMode.FixedRPM:
                        settingValue = fanRpmSettingResp.ResponseData.ToUInt16();
                        break;
                    case FanMode.Custom:

                        break;
                }

                var fanInfo = new HydroFanInfo(
                    fanNr: fanNr,
                    rpm: fanRpmResp.ResponseData.ToUInt16(),
                    mode: fanMode,
                    settingValue: settingValue);
                return fanInfo;
            }
            return null;
        }

        public async Task<bool> SetFanModeAndValue(byte fanNr, FanMode mode, object value = null)
        {
            var fanSelectCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Select)
                    .WithData(new byte[] { fanNr })
                    .Build();

            var fanSetModeCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Mode)
                    .WithData(new[] { (byte)mode })
                    .Build();

            HydroCommand fanSetValueCommand = null;
            switch (mode)
            {
                case FanMode.FixedPWM:
                    if (!(value is byte))
                        throw new ArgumentException("value must be of byte type");

                    fanSetValueCommand =
                        new HydroCommandBuilder()
                            .WithRegisterInferringOpCode(Registers.FAN_FixedPWM)
                            .WithData(new[] { (byte)value })
                            .Build();
                    break;

                case FanMode.FixedRPM:
                    if (!(value is UInt16))
                        throw new ArgumentException("value must be of UInt16 type");

                    fanSetValueCommand =
                        new HydroCommandBuilder()
                            .WithRegisterInferringOpCode(Registers.FAN_FixedRPM)
                            .WithData(((UInt16)value).ToLittleEndianByteArray())
                            .Build();
                    break;

                case FanMode.Custom:
                    if (!(value is int[][]))
                        throw new ArgumentException("value must be of int[][] type");


                    break;
            }

            var responses = await QueryDeviceAsync(fanSelectCommand, fanSetModeCommand, fanSetValueCommand);
            return responses.AreSuccessful;
        }

        public async Task<bool> UpdateReferenceTemperatureForFanAsync(byte fanNr, UInt16 temperature)
        {
            var fanSelectCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_Select)
                    .WithData(new byte[] { fanNr })
                    .Build();

            var reportTemperatureCommand =
                new HydroCommandBuilder()
                    .WithRegisterInferringOpCode(Registers.FAN_ReportExtTemp)
                    .WithData(temperature.ToLittleEndianByteArray())
                    .Build();

            var responses = await QueryDeviceAsync(fanSelectCommand, reportTemperatureCommand);
            return responses.AreSuccessful;
        }

        #endregion

        private async Task<HydroCommandResponses> QueryDeviceAsync(params HydroCommand[] commands)
        {
            await deviceIOLock.WaitAsync();
            try
            {                
                OpenDevice();
                var validCommands = commands.Where(command => command != null);
                var payload = payloadGenerator.PayloadForCommands(validCommands);
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
