using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib.Extensions
{
    public static class NumberExtensions
    {
        public static byte[] ToLittleEndianByteArray(this UInt16 val)
        {
            return new[]
            {
                (byte)(val & 0xff),
                (byte)(val>>8 & 0xff)
            };
        }

        public static byte[] ToLittleEndianByteArray(this UInt16[] values)
        {
            List<byte> bytes = new List<byte>(values.Length * 2);
            foreach (var val in values)
            {
                bytes.AddRange(val.ToLittleEndianByteArray());
            }
            return bytes.ToArray();
        }

        public static UInt16 ToUInt16(this byte[] val)
        {
            return (UInt16)((val[1] << 8) + val[0]);
        }
    }
}
