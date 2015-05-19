using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CorsairDashboard.Caliburn;
using CorsairDashboard.Common.Extensions;
using CorsairDashboard.HydroService;
using CorsairDashboard.ViewModels.Controls;

namespace CorsairDashboard.ViewModels
{
    public class TemperatureBasedLedViewModel : ScreenWithShell
    {
        private bool canUpdateDevice;
        private UInt16 minTemp, medTemp, maxTemp;

        public RangeColorChooserViewModel MinTempColorChooser { get; set; }

        public RangeColorChooserViewModel MedTempColorChooser { get; set; }

        public RangeColorChooserViewModel MaxTempColorChooser { get; set; }

        public UInt16 MinTemp
        {
            get { return minTemp; }
            set
            {
                if (minTemp != value)
                {
                    minTemp = value;
                    NotifyOfPropertyChange(() => MinTemp);
                    UpdateDevice();
                }
            }
        }

        public UInt16 MedTemp
        {
            get { return medTemp; }
            set
            {
                if (medTemp != value)
                {
                    medTemp = value;
                    NotifyOfPropertyChange(() => MedTemp);
                    UpdateDevice();
                }
            }
        }

        public UInt16 MaxTemp
        {
            get { return maxTemp; }
            set
            {
                if (maxTemp != value)
                {
                    maxTemp = value;
                    NotifyOfPropertyChange(() => MaxTemp);
                    UpdateDevice();
                }
            }
        }

        public GradientStopCollection GradientStops
        {
            get
            {
                var gradientStops = new List<GradientStop>
                {
                    new GradientStop(MinTempColorChooser.CurrentColor, 0),
                    new GradientStop(MedTempColorChooser.CurrentColor, 0.5),
                    new GradientStop(MaxTempColorChooser.CurrentColor, 1)
                };
                return new GradientStopCollection(gradientStops);
            }
        }

        public TemperatureBasedLedViewModel(IShell shell)
            : base(shell)
        {
            DisplayName = "Temperature based";
            canUpdateDevice = false;

            MinTempColorChooser = new RangeColorChooserViewModel();
            MinTempColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
           
            MedTempColorChooser = new RangeColorChooserViewModel();
            MedTempColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
            
            MaxTempColorChooser = new RangeColorChooserViewModel();
            MaxTempColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
            
            Init();
        }

        void OnRangeColorChooserPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentColor")
            {
                NotifyOfPropertyChange(() => GradientStops);
                UpdateDevice();
            }
        }

        private async void Init()
        {
            var ledInfo = await Shell.HydroDeviceDataProvider.Led;
            MinTempColorChooser.CurrentColor = ledInfo.Color1.ToColor();
            MedTempColorChooser.CurrentColor = ledInfo.Color2.ToColor();
            MaxTempColorChooser.CurrentColor = ledInfo.Color3.ToColor();
            if (ledInfo.Mode == LedMode.TemperatureBased)
            {
                MinTemp = ledInfo.TemperatureMin;
                MedTemp = ledInfo.TemperatureMed;
                MaxTemp = ledInfo.TemperatureMax;
            }
            else
            {
                MinTemp = 20;
                MedTemp = 30;
                MaxTemp = 40;
            }
            canUpdateDevice = true;
        }

        private async void UpdateDevice()
        {
            if (!canUpdateDevice)
                return;

            await Shell.HydroDeviceDataProvider.SetLedTemperatureBaseColorsAsync(MinTemp,
                MedTemp,
                MaxTemp,
                MinTempColorChooser.CurrentColor,
                MedTempColorChooser.CurrentColor,
                MaxTempColorChooser.CurrentColor);
        }
    }
}
