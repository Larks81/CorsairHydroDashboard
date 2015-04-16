using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.CommandSystem
{
    internal sealed class HydroCommand
    {
        public byte CommandId { get; set; }

        public OpCodes OpCode { get; set; }

        public Registers Register { get; set; }

        public byte[] Data { get; set; }

        //public byte NumberOfBytesToReadOrWrite { get; set; }
    }
}
