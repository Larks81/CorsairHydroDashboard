using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.ViewModels.Controls;
using HydroLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.ViewModels
{
    public class SingleColorLedViewModel : ChildBaseViewModel
    {
        bool pulse;

        public RangeColorChooserViewModel RangeColorChooser { get; set; }

        public bool Pulse
        {
            get { return pulse; }
            set
            {
                if (pulse != value)
                {
                    pulse = value;
                    NotifyOfPropertyChange(() => Pulse);
                }
            }
        }

        public SingleColorLedViewModel(IShell shell, HydroLedInfo ledInfo)
            : base(shell)
        {
            this.RangeColorChooser = new RangeColorChooserViewModel();
            if (ledInfo != null)
            {
                var ledColor = ledInfo.Color1;
                RangeColorChooser.R = ledColor[0];
                RangeColorChooser.G = ledColor[1];
                RangeColorChooser.B = ledColor[2];
                Pulse = ledInfo.IsPulsing();
            }

            RangeColorChooser.PropertyChanged += OnPropertyChanged; //leak?
            PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentColor" || e.PropertyName == "Pulse")
            {
                await Shell.HydroDevice.SetLedSingleColorAsync(RangeColorChooser.R, RangeColorChooser.G, RangeColorChooser.B, Pulse);
            }
        }
    }
}
