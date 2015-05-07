using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CorsairDashboard.HydroService
{
    public partial class HydroColor
    {
        public HydroColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte[] ToByteArray()
        {
            return new[] { R, G, B };
        }

        public Color ToColor()
        {
            return Color.FromRgb(R, G, B);
        }

        public static explicit operator HydroColor(Color c)
        {
            return new HydroColor(c.R, c.G, c.B);
        }
    }
}
