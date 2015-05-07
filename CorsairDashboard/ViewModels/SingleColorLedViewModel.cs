﻿using Caliburn.Micro;
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

        public SingleColorLedViewModel(IShell shell)
            : base(shell)
        {
            this.RangeColorChooser = new RangeColorChooserViewModel();

            shell.HydroDeviceDataProvider.Led
                .ContinueWith(task =>
                {
                    var ledInfo = task.Result;
                    var ledColor = ledInfo.Color1;
                    RangeColorChooser.R = ledColor.R;
                    RangeColorChooser.G = ledColor.G;
                    RangeColorChooser.B = ledColor.B;
                    Pulse = ledInfo.IsPulsing();
                });

            RangeColorChooser.PropertyChanged += OnPropertyChanged; //leak?
            PropertyChanged += OnPropertyChanged;
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
