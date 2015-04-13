using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroLib
{
    public class HydroDeviceEnumerator : IEnumerable<IHydroDevice>
    {
        bool canReturnNullDevice;

        /// <summary>
        /// Builds a new Hydro devices enumerator.
        /// </summary>
        /// <param name="canReturnNullDevice">Returns a mock/null device if a real one isn't found.</param>
        public HydroDeviceEnumerator(bool canReturnNullDevice)
        {
            this.canReturnNullDevice = canReturnNullDevice;
        }

        public IEnumerator<IHydroDevice> GetEnumerator()
        {
            bool atLeastOneRealDeviceIsPresent = false;
            foreach (var device in HidDevices.Enumerate(0x1b1c, 0x0c04))
            {
                if (!atLeastOneRealDeviceIsPresent)
                    atLeastOneRealDeviceIsPresent = true;

                yield return new HydroDevice(device);
            }

            if (canReturnNullDevice && !atLeastOneRealDeviceIsPresent)
            {
                yield return new HydroNullDevice();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
