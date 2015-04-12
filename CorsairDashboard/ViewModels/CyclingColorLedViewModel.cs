using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.ViewModels.Controls;
using HydroLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CorsairDashboard.Extensions;

namespace CorsairDashboard.ViewModels
{
    public class CyclingColorLedViewModel : ChildBaseViewModel
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
                    UpdateDevice();
                }
            }
        }

        public CyclingColorLedViewModel(IShell shell, HydroLedInfo ledInfo) :
            base(shell)
        {
            hydroLedInfo = ledInfo;
            canUpdateDevice = false;
            ranges = new RangeColorChooserViewModel[4];
            SelectedNumberOfColor = ledInfo.Mode == LedMode.FourColorCycle ? NrOfColors.Four : NrOfColors.Two;
            canUpdateDevice = true;
        }

        void UpdateNrOfVisibleColorChoosers()
        {
            if (FirstColorChooser == null)
            {
                FirstColorChooser = new RangeColorChooserViewModel();
                FirstColorChooser.CurrentColor = hydroLedInfo.Color1.ToColor();
                FirstColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
            }
            if (SecondColorChooser == null)
            {
                SecondColorChooser = new RangeColorChooserViewModel();
                SecondColorChooser.CurrentColor = hydroLedInfo.Color2.ToColor();
                SecondColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
            }

            if (SelectedNumberOfColor == NrOfColors.Four)
            {
                if (ThirdColorChooser == null)
                {
                    ThirdColorChooser = new RangeColorChooserViewModel();
                    ThirdColorChooser.CurrentColor = hydroLedInfo.Color3.ToColor();
                    ThirdColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
                }
                if (FourthColorChooser == null)
                {
                    FourthColorChooser = new RangeColorChooserViewModel();
                    FourthColorChooser.CurrentColor = hydroLedInfo.Color4.ToColor();
                    FourthColorChooser.PropertyChanged += OnRangeColorChooserPropertyChanged;
                }
            }
            else
            {
                ThirdColorChooser = null;
                FourthColorChooser = null;
            }
        }

        async void OnRangeColorChooserPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentColor" || e.PropertyName == "SelectedNumberOfColor")
            {
                await UpdateDevice();
            }
        }

        async Task UpdateDevice()
        {
            if (!canUpdateDevice)
                return;

            await Shell.HydroDevice.SetLedCycleColorsAsync(FirstColorChooser.CurrentColor.ToByteArray(),
                SecondColorChooser.CurrentColor.ToByteArray(),
                ThirdColorChooser != null ? ThirdColorChooser.CurrentColor.ToByteArray() : null,
                FourthColorChooser != null ? FourthColorChooser.CurrentColor.ToByteArray() : null);
        }
    }
}
