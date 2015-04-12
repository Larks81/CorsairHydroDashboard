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

        [ImportingConstructor]
        public LedViewModel(IShell shell)
            : base(shell)
        {
            Shell.HydroDevice.GetLedInfoAsync()
                .ContinueWith(t =>
                {
                    var ledInfo = t.Result;
                    SingleColorLed = new SingleColorLedViewModel(shell, ledInfo);
                    NotifyOfPropertyChange(() => SingleColorLed);
                    
                    CyclingColorLed = new CyclingColorLedViewModel(shell, ledInfo);
                    NotifyOfPropertyChange(() => CyclingColorLed);

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
