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
    public class CyclingColorLedViewModel : ChildBaseViewModel
    {
        public enum NrOfColors
        {
            Two = 2,
            Four = 4
        }

        bool canUpdateDevice;
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
                    NotifyOfPropertyChange(() => SelectedNumberOfColor);
                    UpdateNrOfVisibleColorChoosers();
                    UpdateDevice();
                }
            }
        }

        public CyclingColorLedViewModel(IShell shell, HydroLedInfo ledInfo) :
            base(shell)
        {
            canUpdateDevice = false;
            ranges = new RangeColorChooserViewModel[4];
            SelectedNumberOfColor = ledInfo.Mode == LedMode.FourColorCycle ? NrOfColors.Four : NrOfColors.Two;
        }

        void UpdateNrOfVisibleColorChoosers()
        {
            if (FirstColorChooser == null)
                FirstColorChooser = new RangeColorChooserViewModel();
            if (SecondColorChooser == null)
                SecondColorChooser = new RangeColorChooserViewModel();

            if (SelectedNumberOfColor == NrOfColors.Four)
            {
                if (ThirdColorChooser == null)
                    ThirdColorChooser = new RangeColorChooserViewModel();
                if (FourthColorChooser == null)
                    FourthColorChooser = new RangeColorChooserViewModel();
            }
            else
            {
                ThirdColorChooser = null;
                FourthColorChooser = null;
            }
        }

        void UpdateDevice()
        {
            if (!canUpdateDevice)
                return;


        }
    }
}
