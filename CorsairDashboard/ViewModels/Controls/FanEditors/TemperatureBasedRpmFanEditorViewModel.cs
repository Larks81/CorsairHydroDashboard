using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CorsairDashboard.HardwareMonitorService;
using CorsairDashboard.ServiceWrapper;

namespace CorsairDashboard.ViewModels.Controls.FanEditors
{
    public class TemperatureBasedRpmFanEditorViewModel : FanEditorViewModelBase
    {
        public class TemperatureRpmViewModel : PropertyChangedBase
        {
            private UInt16 temperature;
            private UInt16 rpm;

            public UInt16 Temperature
            {
                get { return temperature; }
                set
                {
                    if (temperature != value)
                    {
                        temperature = value;
                        NotifyOfPropertyChange(() => Temperature);
                    }
                }
            }

            public UInt16 Rpm
            {
                get { return rpm; }
                set
                {
                    if (rpm != value)
                    {
                        rpm = value;
                        NotifyOfPropertyChange(() => Rpm);
                    }
                }
            }

            public TemperatureRpmViewModel(UInt16 temperature, UInt16 rpm)
            {
                this.temperature = temperature;
                this.rpm = rpm;
            }
        }

        private SensorWithHardwareNameViewModel selectedSensor;

        public BindableCollection<TemperatureRpmViewModel> TemperaturesAndRpms { get; private set; }

        public BindableCollection<SensorWithHardwareNameViewModel> Sensors { get; set; }

        public SensorWithHardwareNameViewModel SelectedSensor
        {
            get { return selectedSensor; }
            set
            {
                if (selectedSensor != value)
                {
                    selectedSensor = value;
                    NotifyOfPropertyChange(() => SelectedSensor);
                    NotifyOfPropertyChange(() => ValueForParent);
                }
            }
        }

        public TemperatureBasedRpmFanEditorViewModel(ReactiveHardwareMonitoring hwMonitor)
        {
            TemperaturesAndRpms = new BindableCollection<TemperatureRpmViewModel>();

            Sensors = new BindableCollection<SensorWithHardwareNameViewModel>();

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.Cpu)
                .Subscribe(AddHardwareSensorsData);

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.GraphicCard)
                .Subscribe(AddHardwareSensorsData);

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.HardDisk)
                .Subscribe(AddHardwareSensorsData);
        }

        void TemperatureRpmViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => ValueForParent);
        }

        public override object ValueForParent
        {
            get
            {
                if (SelectedSensor == null)
                    return null;

                return new Tuple<UInt16[], UInt16[], String>(
                    TemperaturesAndRpms.Select(t => t.Temperature).ToArray(),
                    TemperaturesAndRpms.Select(t => t.Rpm).ToArray(),
                    SelectedSensor.SensorId);
            }
        }

        public override void SetInitialValue(object value)
        {
            var tempsAndRpms = (Tuple<UInt16[], UInt16[]>)value;
            var temps = tempsAndRpms.Item1;
            var rpms = tempsAndRpms.Item2;
            for (int i = 0; i < temps.Length; i++)
            {
                var vm = new TemperatureRpmViewModel(temps[i], rpms[i]);
                vm.PropertyChanged += TemperatureRpmViewModelPropertyChanged;
                TemperaturesAndRpms.Add(vm);
            }
            InitialValueSet = true;
        }

        private void AddHardwareSensorsData(IEnumerable<Hardware> hardwareList)
        {
            foreach (var hardware in hardwareList)
            {
                foreach (var sensor in hardware.Sensors.OfType<TemperatureSensor>())
                {
                    var hardwareSensorViewModel = Sensors.FirstOrDefault(s => s.SensorId == sensor.Id);
                    if (hardwareSensorViewModel == null)
                    {
                        hardwareSensorViewModel = new SensorWithHardwareNameViewModel(hardware, sensor);
                        Sensors.Add(hardwareSensorViewModel);
                    }
                    hardwareSensorViewModel.Update(sensor);
                }
            }
        }
    }
}
