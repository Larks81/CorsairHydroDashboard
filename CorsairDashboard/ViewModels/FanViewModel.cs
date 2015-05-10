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
using CorsairDashboard.HydroService;
using CorsairDashboard.ViewModels.Controls.FanEditors;

namespace CorsairDashboard.ViewModels
{
    public class FanViewModel : ChildBaseViewModel
    {
        private FanModeDescription selectedMode;
        private int rpm;
        private bool canUpdateDevice;
        private bool isConnected;
        private bool is4PinFan;

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    NotifyOfPropertyChange(() => IsConnected);
                }
            }
        }

        public bool Is4PinFan
        {
            get { return is4PinFan; }
            set
            {
                if (is4PinFan != value)
                {
                    is4PinFan = value;
                    NotifyOfPropertyChange(() => Is4PinFan);
                }
            }
        }

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
            Shell.HydroDeviceDataProvider.Fans.ElementAt(FanNumber)
                .Where(fanInfo => fanInfo != null)
                .Subscribe(fanInfo =>
                {
                    if (selectedMode == null)
                    {
                        SelectedMode = Modes.First(fmd => fmd.Value == (byte)fanInfo.Mode);
                    }

                    rpm = fanInfo.Rpm;
                    NotifyOfPropertyChange(() => Rpm);
                    IsConnected = fanInfo.IsConnected;
                    Is4PinFan = fanInfo.IsFourPinFan;

                    if (Editor != null && !Editor.InitialValueSet)
                    {
                        switch ((FanMode)SelectedMode.Value)
                        {
                            case FanMode.FixedPWM:
                                Editor.SetInitialValue(fanInfo.PwmValue);
                                break;
                            case FanMode.FixedRPM:
                                Editor.SetInitialValue(fanInfo.RpmValue);
                                ((FixedRpmFanEditorViewModel) Editor).MaxRpm = (UInt16)(fanInfo.MaxRpm + 150);
                                break;
                            case FanMode.Custom:
                                Editor.SetInitialValue(fanInfo.RpmsAndTempsTable);
                                break;
                        }
                    }
                    canUpdateDevice = true;
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
                    Editor = new TemperatureBasedRpmFanEditorViewModel(Shell.HardwareMonitoringProvider);
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
            if (propertyChangedEventArgs.PropertyName == "Value" || propertyChangedEventArgs.PropertyName == "ValueForParent")
            {
                await UpdateDevice();
            }
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
                    await Shell.HydroDeviceDataProvider.SetFanModeToDefaultProfileAsync(FanNumber);
                    break;

                case FanMode.Quiet:
                    await Shell.HydroDeviceDataProvider.SetFanModeToQuietProfileAsync(FanNumber);
                    break;

                case FanMode.Balanced:
                    await Shell.HydroDeviceDataProvider.SetFanModeToBalancedProfileAsync(FanNumber);
                    break;

                case FanMode.Performance:
                    await Shell.HydroDeviceDataProvider.SetFanModeToPerformanceProfileAsync(FanNumber);
                    break;

                case FanMode.Custom:
                    var tempsRpmsSensorId = (Tuple<UInt16[], UInt16[], String>) Editor.ValueForParent;
                    if (tempsRpmsSensorId != null)
                    {
                        await Shell.HydroDeviceDataProvider.SetTemperatureBasedRpmFanAsync(FanNumber, tempsRpmsSensorId.Item1, tempsRpmsSensorId.Item2, tempsRpmsSensorId.Item3);                       
                    }
                    break;
            }

        }
    }
}
