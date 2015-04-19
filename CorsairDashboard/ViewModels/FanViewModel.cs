using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using CorsairDashboard.ViewModels.Controls.FanEditors;
using HydroLib;

namespace CorsairDashboard.ViewModels
{
    public class FanViewModel : ChildBaseViewModel
    {
        private FanModeDescription selectedMode;
        private int rpm;
        private bool canUpdateDevice;

        public int FanNumber { get; set; }

        public override string DisplayName
        {
            get
            {
                return String.Format("Fan {0}", FanNumber);
            }
            set { }
        }

        public BindableCollection<FanModeDescription> Modes { get; private set; }

        public FanModeDescription SelectedMode
        {
            get
            {
                return selectedMode;
            }
            set
            {
                if (selectedMode != value)
                {
                    selectedMode = value;
                    NotifyOfPropertyChange(() => SelectedMode);
                    LoadAndBindToEditor();
                    UpdateDevice();
                }
            }
        }

        public String Rpm
        {
            get { return String.Format("{0} RPM", rpm); }
        }

        public FanEditorViewModelBase Editor { get; private set; }

        public FanViewModel(IShell shell, int fanNr)
            : base(shell)
        {
            FanNumber = fanNr;
            Modes = new BindableCollection<FanModeDescription>(FanModeDescription.GetFanModeDescriptions());
        }

        protected override void OnActivate()
        {
            Shell.HydroDeviceDataProvider.Fans[FanNumber]
                .Where(fanInfo => fanInfo != null)
                .Subscribe(fanInfo =>
                {
                    if (selectedMode == null)
                    {
                        SelectedMode = Modes.First(fmd => fmd.Value == (byte)fanInfo.Mode);
                    }

                    rpm = fanInfo.Rpm;
                    NotifyOfPropertyChange(() => Rpm);

                    if (Editor != null && !Editor.InitialValueSet)
                    {
                        switch ((FanMode)SelectedMode.Value)
                        {
                            case FanMode.FixedPWM:
                                Editor.SetInitialValue(fanInfo.PwmValue);
                                break;
                            case FanMode.FixedRPM:
                                Editor.SetInitialValue(fanInfo.RpmValue);
                                break;
                            case FanMode.Custom:
                                break;
                        }
                        
                        canUpdateDevice = true;
                    }
                });

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if (Editor != null)
                Editor.PropertyChanged -= OnEditorPropertyChanged;

            base.OnDeactivate(close);
        }

        private void LoadAndBindToEditor()
        {
            if (Editor != null)
                Editor.PropertyChanged -= OnEditorPropertyChanged;

            switch ((FanMode)SelectedMode.Value)
            {
                case FanMode.FixedPWM:
                    Editor = new FixedPwmFanEditorViewModel();
                    break;
                case FanMode.FixedRPM:
                    Editor = new FixedRpmFanEditorViewModel();
                    break;
                case FanMode.Custom:
                    break;
            }
            if (Editor != null)
            {
                Editor.PropertyChanged += OnEditorPropertyChanged;
            }
            NotifyOfPropertyChange(() => Editor);
        }

        private async void OnEditorPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Value")
                await UpdateDevice();
        }

        private async Task UpdateDevice()
        {
            if (!canUpdateDevice)
                return;

            switch ((FanMode)SelectedMode.Value)
            {
                case FanMode.FixedPWM:
                    await Shell.HydroDeviceDataProvider.SetPwmFanAsync(FanNumber, (byte)Editor.ValueForParent);
                    break;

                case FanMode.FixedRPM:
                    await Shell.HydroDeviceDataProvider.SetRpmFanAsync(FanNumber, (UInt16)Editor.ValueForParent);
                    break;

                case FanMode.Default:
                    break;

                case FanMode.Quiet:
                    break;

                case FanMode.Balanced:
                    break;

                case FanMode.Performance:
                    break;

                case FanMode.Custom:
                    break;
            }

        }
    }
}
