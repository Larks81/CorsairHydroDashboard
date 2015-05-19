using Caliburn.Micro;
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
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class LedViewModel : Conductor<ScreenWithShell>.Collection.OneActive
    {
        [ImportingConstructor]
        public LedViewModel(IShell shell)
        {
            Items.Add(new SingleColorLedViewModel(shell));
            Items.Add(new CyclingColorLedViewModel(shell));
            Items.Add(new TemperatureBasedLedViewModel(shell));
        }
    }
}
