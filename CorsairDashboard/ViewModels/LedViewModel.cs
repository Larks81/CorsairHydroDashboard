using CorsairDashboard.Caliburn;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CorsairDashboard.ViewModels
{
    [Export]
    public class LedViewModel : ChildBaseViewModel
    {
        public SingleColorLedViewModel SingleColorLed { get; set; }

        public CyclingColorLedViewModel CyclingColorLed { get; set; }

        public TemperatureBasedLedViewModel TemperatureBasedLed { get; set; }

        [ImportingConstructor]
        public LedViewModel(IShell shell)
            : base(shell)
        {
            SingleColorLed = new SingleColorLedViewModel(shell);
            CyclingColorLed = new CyclingColorLedViewModel(shell);
            TemperatureBasedLed = new TemperatureBasedLedViewModel(shell);
        }
    }
}
