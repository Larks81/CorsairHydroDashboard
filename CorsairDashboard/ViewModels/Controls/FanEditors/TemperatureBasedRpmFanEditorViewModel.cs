using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        public class SensorViewModel
        {
            public String Id { get; set; }

            public String Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private readonly SensorViewModel internalWaterTemperatureSensor = new SensorViewModel() { Id = null, Name = "Internal water sensor" };
        private SensorViewModel selectedSensor;

        public BindableCollection<TemperatureRpmViewModel> TemperaturesAndRpms { get; private set; }

        public BindableCollection<SensorViewModel> Sensors { get; set; }

        public SensorViewModel SelectedSensor
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

            Sensors = new BindableCollection<SensorViewModel>()
            {
                internalWaterTemperatureSensor
            };

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.Cpu)
                .Where(hwList => hwList != null && hwList.Any())
                .TakeWhile(CanTakeHardwareSensorUpdates)
                .Subscribe(AddHardwareSensorsData);

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.GraphicCard)
                .Where(hwList => hwList != null && hwList.Any())
                .TakeWhile(CanTakeHardwareSensorUpdates)
                .Subscribe(AddHardwareSensorsData);

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.HardDisk)
                .Where(hwList => hwList != null && hwList.Any())
                .TakeWhile(CanTakeHardwareSensorUpdates)
                .Subscribe(AddHardwareSensorsData);

            hwMonitor.GetSignalForHardwareOfKind(HardwareKind.Mainboard)
                .Where(hwList => hwList != null && hwList.Any())
                .TakeWhile(CanTakeHardwareSensorUpdates)
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
                    SelectedSensor.Id);
            }
        }

        public override void SetInitialValue(object value)
        {
            if (value != null)
            {
                var tempsRpmsAndSensorId = (Tuple<UInt16[], UInt16[], String>)value;
                var temps = tempsRpmsAndSensorId.Item1;
                var rpms = tempsRpmsAndSensorId.Item2;
                var sensorId = tempsRpmsAndSensorId.Item3;
                if (sensorId != null)
                {
                    if (sensorId == "")
                    {
                        selectedSensor = internalWaterTemperatureSensor;
                    }
                    else
                    {
                        selectedSensor = Sensors.FirstOrDefault(s => s.Id == sensorId);
                    }
                }
                for (int i = 0; i < temps.Length; i++)
                {
                    var vm = new TemperatureRpmViewModel(temps[i], rpms[i]);
                    vm.PropertyChanged += TemperatureRpmViewModelPropertyChanged;
                    TemperaturesAndRpms.Add(vm);
                }
                
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    var vm = new TemperatureRpmViewModel((UInt16)(i * 5 + 20), 1000);
                    vm.PropertyChanged += TemperatureRpmViewModelPropertyChanged;
                    TemperaturesAndRpms.Add(vm);
                }
            }
            InitialValueSet = true;
        }

        private bool CanTakeHardwareSensorUpdates(IEnumerable<Hardware> hardwareList)
        {
            return
                hardwareList.SelectMany(h => h.Sensors)
                    .OfType<TemperatureSensor>()
                    .Any(sens => Sensors.All(s => s.Id != sens.Id));
        }

        private void AddHardwareSensorsData(IEnumerable<Hardware> hardwareList)
        {
            foreach (var hardware in hardwareList)
            {
                foreach (var sensor in hardware.Sensors.OfType<TemperatureSensor>())
                {
                    if (!Sensors.Any(s => s.Id == sensor.Id))
                    {
                        Sensors.Add(new SensorViewModel() { Id = sensor.Id, Name = hardware.Name });
                    }
                }
            }
        }
    }
}
