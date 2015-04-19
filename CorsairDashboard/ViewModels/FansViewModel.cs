using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.ViewModels
{
    [Export]
    public class FansViewModel : ChildBaseViewModel
    {
        public BindableCollection<ChildBaseViewModel> Fans { get; set; }

        private ChildBaseViewModel selectedFan;
        public ChildBaseViewModel SelectedFan
        {
            get
            {
                return selectedFan;
            }
            set
            {
                if (selectedFan != value)
                {
                    if (selectedFan != null)
                    {
                        selectedFan.DeactivateManually();
                    }
                    selectedFan = value;
                    if (selectedFan != null)
                    {
                        selectedFan.ActivateManually();
                    }
                }
            }
        }

        [ImportingConstructor]
        public FansViewModel(IShell shell)
            : base(shell)
        {
            Fans = new BindableCollection<ChildBaseViewModel>();
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
                    Fans.Add(new FanViewModel(Shell, i));
                }
            }

            base.OnInitialize();
        }

        protected override void OnDeactivate(bool close)
        {
            if (SelectedFan != null)
            {
                SelectedFan.DeactivateManually();
            }
            Fans = null;
            base.OnDeactivate(close);
        }
    }
}
