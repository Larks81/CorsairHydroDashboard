using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    [DataContract]
    public class HydroColor
    {
        [DataMember]
        public byte R { get; private set; }

        [DataMember]
        public byte G { get; private set; }

        [DataMember]
        public byte B { get; private set; }

        public HydroColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public HydroColor(byte[] colorBytes)
        {
            if (colorBytes == null)
                throw new ArgumentNullException("colorBytes");
            if (colorBytes.Length != 3)
                throw new ArgumentException("colorBytes must be of length 3");

            R = colorBytes[0];
            G = colorBytes[1];
            B = colorBytes[2];
        }

        public byte[] ToByteArray()
        {
            return new[] { R, G, B };
        }
    }
}