using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.CommandSystem
{
    internal sealed class HydroCommandBuilder
    {
        private static byte commandId = 1;
        private static object builderLock = new object();

        private OpCodes? opCode;
        private Registers? register;
        private byte[] data;
        private bool inferOpCode;
        private byte nrOfBytesToRead;

        public HydroCommandBuilder()
        {           
            inferOpCode = false;
            this.nrOfBytesToRead = 0;
        }

        public HydroCommandBuilder WithOpCode(OpCodes opCode, byte nrOfBytesToRead = 0)
        {
            this.opCode = opCode;
            this.nrOfBytesToRead = nrOfBytesToRead;
            inferOpCode = false;
            return this;
        }

        public HydroCommandBuilder WithRegister(Registers register)
        {
            this.register = register;
            inferOpCode = false;
            return this;
        }

        public HydroCommandBuilder WithRegisterInferringOpCode(Registers register, byte nrOfBytesToRead = 0)
        {
            this.register = register;
            this.nrOfBytesToRead = nrOfBytesToRead;
            inferOpCode = true;
            return this;
        }

        public HydroCommandBuilder WithData(byte[] data)
        {
            this.data = data;
            return this;
        }

        public HydroCommand Build()
        {
            if (register == null)
                throw new ArgumentNullException("Register cannot be null. Make sure to set the register before building the command.");
            if (opCode == null && !inferOpCode)
                throw new ArgumentNullException("OpCode cannot be null. Make sure to set the OpCode before building the command, or use the method WithRegisterInferringOpCode");

            lock (builderLock)
            {

                var isReadCommand = (data == null);

                if (opCode == null && inferOpCode)
                {
                    opCode = InferOpCodeForRegister(register.Value, isReadCommand);
                }

                if (!isReadCommand)
                {
                    //prevent writing of readonly register
                    var memberInfo = (MemberInfo)typeof(Registers).GetMember(register.ToString()).First();
                    var readOnlyAttribute = memberInfo.GetCustomAttribute<ReadOnlyAttribute>();
                    var isReadOnly = (readOnlyAttribute != null && readOnlyAttribute.IsReadOnly);
                    if (isReadOnly)
                        throw new ArgumentException(register.ToString() + " register is read only.");
                }

                switch (opCode)
                {
                    case OpCodes.WriteMoreBytes:
                        //the first byte must report the nr of bytes to write
                        var newData = new byte[data.Length + 1];
                        newData[0] = (byte)data.Length;
                        Buffer.BlockCopy(data, 0, newData, 1, data.Length);
                        data = newData;
                        break;

                    case OpCodes.ReadMoreBytes:
                        if (nrOfBytesToRead == 0)
                            throw new ArgumentException("You've not set the number of bytes to read.");

                        data = new byte[] { nrOfBytesToRead };
                        break;
                }

                return new HydroCommand()
                {
                    CommandId = commandId++,
                    OpCode = opCode.Value,
                    Register = register.Value,
                    Data = data,
                    //NumberOfBytesToReadOrWrite = nrOfBytesToRead
                };
            }
        }

        private static OpCodes InferOpCodeForRegister(Registers reg, bool read)
        {
            switch (reg)
            {
                case Registers.DeviceID:
                case Registers.Status:
                case Registers.LED_SelectCurrent:
                case Registers.LED_Count:
                case Registers.LED_Mode:
                case Registers.TEMP_SelectActiveSensor:
                case Registers.TEMP_CountSensors:
                case Registers.FAN_Select:
                case Registers.FAN_Count:
                case Registers.FAN_Mode:
                case Registers.FAN_FixedPWM:
                    return read ? OpCodes.ReadOneByte : OpCodes.WriteOneByte;

                case Registers.FirmwareID:
                case Registers.LED_TemperatureColor:
                case Registers.TEMP_Read:
                case Registers.TEMP_Limit:
                case Registers.FAN_FixedRPM:
                case Registers.FAN_ReportExtTemp:
                case Registers.FAN_ReadRPM:
                case Registers.FAN_MaxRecordedRPM:
                case Registers.FAN_UnderSpeedThreshold:
                    return read ? OpCodes.ReadTwoBytes : OpCodes.WriteTwoBytes;

                case Registers.ProductName:
                case Registers.LED_CurrentColor:
                case Registers.LED_TemperatureMode:
                case Registers.LED_TemperatureModeColors:
                case Registers.LED_CycleColors:
                case Registers.FAN_RPMTable:
                case Registers.FAN_TempTable:
                    return read ? OpCodes.ReadMoreBytes : OpCodes.WriteMoreBytes;
            }
            throw new Exception("Cannot infer the right OpCode.");
        }
    }
}
