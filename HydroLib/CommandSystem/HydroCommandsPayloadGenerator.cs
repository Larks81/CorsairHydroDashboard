using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.CommandSystem
{
    internal class HydroCommandsPayloadGenerator
    {
        private const int WriteBufferLength = 65;

        public byte[] PayloadForCommands(IEnumerable<HydroCommand> commands)
        {
            var payload = new byte[WriteBufferLength];
            byte bytesWrote = 0;
            int startIdx = 2;
            foreach (var command in commands)
            {
                var commandPayload = PayloadForCommand(command);
                Buffer.BlockCopy(commandPayload, 0, payload, startIdx, commandPayload.Length);
                bytesWrote += (byte)commandPayload.Length;
                startIdx += commandPayload.Length;
            }
            //the first byte is the command lenght
            payload[1] = bytesWrote;
            return payload;
        }

        private byte[] PayloadForCommand(HydroCommand command)
        {
            var commandPayload = new List<byte>()
            {
                command.CommandId,
                (byte)command.OpCode,
                (byte)command.Register
            };

            if (command.Data != null && command.Data.Length > 0)
            {
                commandPayload.AddRange(command.Data);
            }

            return commandPayload.ToArray();
        }
    }
}
