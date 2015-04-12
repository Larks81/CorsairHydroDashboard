using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroDeviceEnumerator : IEnumerable<HydroDevice>
    {
        public static readonly HydroDeviceEnumerator Instance = new HydroDeviceEnumerator();

        private HydroDeviceEnumerator() { }

        public IEnumerator<HydroDevice> GetEnumerator()
        {
            foreach (var device in HidDevices.Enumerate(0x1b1c, 0x0c04))
                yield return new HydroDevice(device);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
