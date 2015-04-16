using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.CommandSystem
{
    internal class HydroCommandResponses
    {
        public static HydroCommandResponses EmptyResponse = new HydroCommandResponses(null);

        public static class Factory
        {
            public static HydroCommandResponses FromDeviceResponse(byte[] resp)
            {
                if (resp == null || resp.Length == 0)
                    return EmptyResponse;

                var responses = new List<HydroCommandResponse>();
                int idx = 0;
                while (resp[idx] != 0)
                {
                    var commandId = resp[idx++];
                    var opCode = (OpCodes)resp[idx++];
                    var isSuccessful = false;
                    var dataLength = 0;
                    switch (opCode)
                    {
                        case OpCodes.WriteOneByte:
                        case OpCodes.WriteTwoBytes:
                        case OpCodes.WriteMoreBytes:
                            isSuccessful = true;
                            break;

                        case OpCodes.ReadOneByte:
                            dataLength = 1;
                            break;

                        case OpCodes.ReadTwoBytes:
                            dataLength = 2;
                            break;

                        case OpCodes.ReadMoreBytes:
                            dataLength = resp[idx++];
                            break;
                    }
                    byte[] responseData = null;
                    if (dataLength > 0)
                    {
                        responseData = new byte[dataLength];
                        Buffer.BlockCopy(resp, idx, responseData, 0, dataLength);
                        idx += dataLength;
                        isSuccessful = true;
                    }

                    responses.Add(new HydroCommandResponse()
                    {
                        CommandId = commandId,
                        IsSuccessful = isSuccessful,
                        ResponseData = responseData
                    });
                }
                return new HydroCommandResponses(responses);
            }
        }

        private List<HydroCommandResponse> responses;

        public bool AreSuccessful
        {
            get
            {
                return responses != null && responses.All(r => r.IsSuccessful);
            }
        }

        private HydroCommandResponses(List<HydroCommandResponse> responses)
        {
            this.responses = responses;
        }

        public bool TryGetResponseForCommand(HydroCommand command, out HydroCommandResponse response)
        {
            response = responses.FirstOrDefault(r => r.CommandId == command.CommandId);
            return response != null;
        }
    }
}
