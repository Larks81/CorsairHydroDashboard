using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroDevice : IDisposable
    {
        private const int WriteBufferLength = 65;
        private const int WriteTimeOut = 3000;
        private const int ReadTimeOut = 3000;

        private HidDevice device;
        private byte[] writeBuffer;
        private byte commandId;
        private SemaphoreSlim hidOperationsSemaphoreSlim;

        public HydroDevice(HidDevice hidDevice)
        {
            hidOperationsSemaphoreSlim = new SemaphoreSlim(1, 1);
            commandId = 1;
            device = hidDevice;
            writeBuffer = new byte[WriteBufferLength];
        }

        #region Infos

        public async Task<String> GetModelNameAsync()
        {
            var resp = await QueryDeviceAsync(0x03, commandId++, (byte)OpCodes.ReadThreeBytes, 0x02);
            if (resp != null)
            {
                var name = Encoding.ASCII.GetString(resp, 3, 32).Trim('\0');
                return name;
            }
            return null;
        }

        #endregion

        #region Temps

        public async Task<int> GetTemperatureAsync()
        {
            var resp = await QueryDeviceAsync(0x03, commandId++, 0x09, 0x0e);
            if (resp != null)
            {
                int temp = resp[3] << 8;
                temp += resp[2];
                temp = temp / 256;
                return temp;
            }
            return 0;
        }

        #endregion

        #region Leds

        public async Task<HydroLedInfo> GetLedInfoAsync()
        {
            var resp = await QueryDeviceAsync(
                0x07,
                commandId++,
                (byte)OpCodes.ReadOneByte,
                (byte)Commands.LED_Mode,
                commandId++,
                (byte)OpCodes.ReadThreeBytes,
                (byte)Commands.LED_CycleColors,
                0x0c);
            if (resp != null)
            {
                var mode = (LedMode)resp[2];
                var color1 = new[] { resp[6], resp[7], resp[8] };
                var color2 = new[] { resp[9], resp[10], resp[11] };
                var color3 = new[] { resp[12], resp[13], resp[14] };
                var color4 = new[] { resp[15], resp[16], resp[17] };
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
            var resp = await QueryDeviceAsync(
                0x12,
                commandId++,
                (byte)OpCodes.WriteOneByte,
                (byte)Commands.LED_SelectCurrent,
                0x00,
                commandId++,
                (byte)OpCodes.WriteOneByte,
                (byte)Commands.LED_Mode,
                pulse ? (byte)0x04b : (byte)0x00,
                commandId++,
                (byte)OpCodes.WriteThreeBytes,
                (byte)Commands.LED_CycleColors,
                0x06,
                red,
                green,
                blue,
                0x00,
                0x00,
                0x00);
            if (resp != null)
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

            var resp = await QueryDeviceAsync(
                0x18,
                commandId++,
                (byte)OpCodes.WriteOneByte,
                (byte)Commands.LED_SelectCurrent,
                0x00,
                commandId++,
                (byte)OpCodes.WriteOneByte,
                (byte)Commands.LED_Mode,
                (byte)ledMode,
                commandId++,
                (byte)OpCodes.WriteThreeBytes,
                (byte)Commands.LED_CycleColors,
                0x0c,
                firstColor[0],
                firstColor[1],
                firstColor[2],
                secondColor[0],
                secondColor[1],
                secondColor[2],
                thirdColor[0],
                thirdColor[1],
                thirdColor[2],
                fourthColor[0],
                fourthColor[1],
                fourthColor[2]);
            if (resp != null)
            {
                return true;
            }
            return false;
        }

        #endregion Leds

        #region Fans

        public async Task<int> GetNrOfFansAsync()
        {
            var resp = await QueryDeviceAsync(
                0x03,
                commandId++,
                (byte)OpCodes.ReadOneByte,
                (byte)Commands.FAN_Count);
            if (resp != null)
            {
                var nrOfFans = resp[2];
                return nrOfFans;
            }
            return 0;
        }

        public async Task<int> GetRpmForFanNrAsync(byte fanNr)
        {
            var resp = await QueryDeviceAsync(
                0x07,
                commandId++,
                (byte)OpCodes.WriteOneByte,
                (byte)Commands.FAN_Select,
                fanNr,
                commandId++,
                (byte)OpCodes.ReadTwoBytes,
                (byte)Commands.FAN_ReadRPM);
            if (resp != null)
            {
                int rpm = resp[5] << 8;
                rpm += resp[4];
                return rpm;
            }
            return 0;

        }

        #endregion

        private async Task<byte[]> QueryDeviceAsync(params byte[] bytes)
        {
            await hidOperationsSemaphoreSlim.WaitAsync();
            try
            {
                OpenDevice();
                FillWriteBuffer(bytes);
                var writeOk = await device.WriteAsync(writeBuffer, WriteTimeOut);
                if (writeOk)
                {
                    var inputReport = await device.ReadReportAsync(ReadTimeOut);
                    return inputReport.ReadStatus == HidDeviceData.ReadStatus.Success ? inputReport.Data : null;
                }
                return null;
            }
            finally
            {
                hidOperationsSemaphoreSlim.Release();
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

        private void ClearBuffer()
        {
            Array.Clear(writeBuffer, 0, WriteBufferLength);
        }

        private void FillWriteBuffer(params byte[] bytes)
        {
            ClearBuffer();
            Buffer.BlockCopy(bytes, 0, writeBuffer, 1, bytes.Count());
        }

        public void Dispose()
        {
            device.CloseDevice();
            device.Dispose();
            device = null;
        }
    }
}
