﻿using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CorsairDashboard.Common.Extensions;
using System.Reactive.Linq;
using CorsairDashboard.HydroService;

namespace CorsairDashboard.ViewModels
{
    public class CyclingColorLedViewModel : ScreenWithShell
    {
        public enum NrOfColors
        {
            Two = 2,
            Four = 4
        }

        bool canUpdateDevice;
        HydroLedInfo hydroLedInfo;
        NrOfColors selectedNrOfColor;
        RangeColorChooserViewModel[] ranges;

        public RangeColorChooserViewModel FirstColorChooser
        {
            get { return ranges[0]; }
            set
            {
                ranges[0] = value;
                NotifyOfPropertyChange(() => FirstColorChooser);
            }
        }

        public RangeColorChooserViewModel SecondColorChooser
        {
            get { return ranges[1]; }
            set
            {
                ranges[1] = value;
                NotifyOfPropertyChange(() => SecondColorChooser);
            }
        }

        public RangeColorChooserViewModel ThirdColorChooser
        {
            get { return ranges[2]; }
            set
            {
                ranges[2] = value;
                NotifyOfPropertyChange(() => ThirdColorChooser);
            }
        }

        public RangeColorChooserViewModel FourthColorChooser
        {
            get { return ranges[3]; }
            set
            {
                ranges[3] = value;
                NotifyOfPropertyChange(() => FourthColorChooser);
            }
        }

        public BindableCollection<NrOfColors> NumberOfColors
        {
            get
            {
                return new BindableCollection<NrOfColors>(new[] { NrOfColors.Two, NrOfColors.Four });
            }
        }

        public GradientStopCollection GradientStops
        {
            get
            {
                if (!canUpdateDevice)
                    return null;

                var gradientStops = new List<GradientStop>();
                gradientStops.Add(new GradientStop(FirstColorChooser.CurrentColor, 0));
                if (selectedNrOfColor == NrOfColors.Two)
                {
                    gradientStops.Add(new GradientStop(SecondColorChooser.CurrentColor, 1));
                }
                else
                {
                    gradientStops.Add(new GradientStop(SecondColorChooser.CurrentColor, 1.0 / 3.0));
                    gradientStops.Add(new GradientStop(ThirdColorChooser.CurrentColor, 2.0 / 3.0));
                    gradientStops.Add(new GradientStop(FourthColorChooser.CurrentColor, 1));
                }
                return new GradientStopCollection(gradientStops);
            }
        }

        public NrOfColors SelectedNumberOfColor
        {
            get { return selectedNrOfColor; }
            set
            {
                if (selectedNrOfColor != value)
                {
                    selectedNrOfColor = value;
                    UpdateNrOfVisibleColorChoosers();
                    NotifyOfPropertyChange(() => SelectedNumberOfColor);
                    NotifyOfPropertyChange(() => GradientStops);
                    UpdateDevice();
                }
            }
        }

        public CyclingColorLedViewModel(IShell shell) :
            base(shell)
        {
            DisplayName = "Cycling colors";
            canUpdateDevice = false;
            ranges = new RangeColorChooserViewModel[4];

            shell.HydroDeviceDataProvider.Led
               .ContinueWith(task =>
               {
                   hydroLedInfo = task.Result;
                   SelectedNumberOfColor = hydroLedInfo.Mode == LedMode.FourColorCycle ? NrOfColors.Four : NrOfColors.Two;                   
                   canUpdateDevice = true;
                   NotifyOfPropertyChange(() => GradientStops);
               });
        }

        void UpdateNrOfVisibleColorChoosers()
        {
            if (FirstColorChooser == null)
            {
                FirstColorChooser = new RangeColorChooserViewModel();                
                FirstColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
            }
            if (SecondColorChooser == null)
            {
                SecondColorChooser = new RangeColorChooserViewModel();
                SecondColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
            }
            FirstColorChooser.CurrentColor = hydroLedInfo.Color1.ToColor();
            SecondColorChooser.CurrentColor = hydroLedInfo.Color2.ToColor();

            if (SelectedNumberOfColor == NrOfColors.Four)
            {
                if (ThirdColorChooser == null)
                {
                    ThirdColorChooser = new RangeColorChooserViewModel();
                    ThirdColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
                }
                if (FourthColorChooser == null)
                {
                    FourthColorChooser = new RangeColorChooserViewModel();
                    FourthColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
                }
                ThirdColorChooser.CurrentColor = hydroLedInfo.Color3.ToColor();
                FourthColorChooser.CurrentColor = hydroLedInfo.Color4.ToColor();
            }
            else
            {
                if (ThirdColorChooser != null)
                {
                    ThirdColorChooser.PropertyChanged -= OnRangeColorChooserPropertyChanged;
                    ThirdColorChooser = null;
                }
                if (FourthColorChooser != null)
                {
                    FourthColorChooser.PropertyChanged -= OnRangeColorChooserPropertyChanged;
                    FourthColorChooser = null;
                }
            }
        }

        async void OnRangeColorChooserPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentColor" || e.PropertyName == "SelectedNumberOfColor")
            {
                NotifyOfPropertyChange(() => GradientStops);
                await UpdateDevice();
            }
        }

        async Task UpdateDevice()
        {
            if (!canUpdateDevice)
                return;

            await Shell.HydroDeviceDataProvider.SetLedCycleColorsAsync(
                FirstColorChooser.CurrentColor,
                SecondColorChooser.CurrentColor,
                ThirdColorChooser != null ? ThirdColorChooser.CurrentColor : (Color?)null,
                FourthColorChooser != null ? FourthColorChooser.CurrentColor : (Color?)null);
        }
    }
}
