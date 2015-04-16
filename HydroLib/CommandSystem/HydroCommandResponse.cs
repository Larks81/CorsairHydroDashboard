using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.CommandSystem
{
    internal class HydroCommandResponse
    {
        public int CommandId { get; set; }

        public bool IsSuccessful { get; set; }

        public byte[] ResponseData { get; set; }
    }
}
