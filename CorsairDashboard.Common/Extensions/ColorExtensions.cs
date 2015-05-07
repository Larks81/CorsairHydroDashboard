using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CorsairDashboard.Common.Extensions
{
    public static class ColorExtensions
    {
        public static Color ToColor(this byte[] arr)
        {
            return Color.FromRgb(arr[0], arr[1], arr[2]);
        }
    }
}
