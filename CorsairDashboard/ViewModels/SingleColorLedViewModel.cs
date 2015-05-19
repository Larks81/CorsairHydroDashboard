using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace CorsairDashboard.ViewModels
{
    public class SingleColorLedViewModel : ScreenWithShell
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

        public SingleColorLedViewModel(IShell shell)
            : base(shell)
        {
            DisplayName = "Single color";
            RangeColorChooser = new RangeColorChooserViewModel();
        }

        protected async override void OnInitialize()
        {
            var ledInfo = await Shell.HydroDeviceDataProvider.Led;
            var ledColor = ledInfo.Color1;
            RangeColorChooser.R = ledColor.R;
            RangeColorChooser.G = ledColor.G;
            RangeColorChooser.B = ledColor.B;
            Pulse = ledInfo.IsPulsing();

            RangeColorChooser.PropertyChanged += OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                RangeColorChooser.PropertyChanged -= OnPropertyChanged;
            }
        }

        private async void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentColor" || e.PropertyName == "Pulse")
            {
                await Shell.HydroDeviceDataProvider.SetLedSingleColorAsync(RangeColorChooser.CurrentColor, Pulse);
            }
        }
    }
}
