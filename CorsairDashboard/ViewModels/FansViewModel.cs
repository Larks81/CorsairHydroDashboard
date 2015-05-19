using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairDashboard.Settings;

namespace CorsairDashboard.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class FansViewModel : Conductor<ScreenWithShell>.Collection.OneActive
    {        
        private IShell Shell { get; set; }

        [ImportingConstructor]
        public FansViewModel(IShell shell)
        {
            Shell = shell;
        }

        protected async override void OnInitialize()
        {
            var totalFansNr = await Shell.HydroDeviceDataProvider.NumberOfFans;
            for (int i = 0; i < totalFansNr; i++)
            {
                if (i == totalFansNr - 1)
                {
                    //pump
                }
                else
                {                    
                    Items.Add(new FanViewModel(Shell, i));
                }
            }
        }
    }
}
