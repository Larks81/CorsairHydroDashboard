using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CorsairDashboard.HardwareMonitoring.Adapters;
using CorsairDashboard.HardwareMonitoring;
using CorsairDashboard.HardwareMonitoring.Hw.Sensors;

namespace CorsairDashboard.ViewModels.Controls.FanEditors
{
    public class TemperatureBasedRpmFanEditorViewModel : FanEditorViewModelBase
    {
        private HardwareList hardwareList;

        public class TemperatureRpmViewModel
        {
            public int Temperature { get; set; }

            public int Rpm { get; set; }
        }

        public BindableCollection<TemperatureRpmViewModel> TemperaturesAndRpms { get; private set; }

        public BindableCollection<SensorWithHardwareNameViewModel> Sensors { get; set; }

        public SensorWithHardwareNameViewModel SelectedSensor { get; set; }

        public TemperatureBasedRpmFanEditorViewModel()
        {
            TemperaturesAndRpms = new BindableCollection<TemperatureRpmViewModel>();
            TemperaturesAndRpms.AddRange(
                Enumerable.Range(0, 5)
                .Select(_ => new TemperatureRpmViewModel()
                {
                    Temperature = 25,
                    Rpm = 1000
                }));

            Sensors = new BindableCollection<SensorWithHardwareNameViewModel>();
            hardwareList = new HardwareList(new OpenHardwareMonitorAdapter());

            hardwareList.GetSignalForHardwareOfKind(HardwareKind.Cpu)
                .Subscribe(AddHardwareSensorsData);

            hardwareList.GetSignalForHardwareOfKind(HardwareKind.GraphicCard)
                .Subscribe(AddHardwareSensorsData);

            hardwareList.GetSignalForHardwareOfKind(HardwareKind.HardDisk)
                .Subscribe(AddHardwareSensorsData);
        }

        public override object ValueForParent
        {
            get
            {
                return new[]
                {
                    TemperaturesAndRpms.Select(t => t.Temperature).ToArray(),
                    TemperaturesAndRpms.Select(t => t.Rpm).ToArray()
                };
            }
        }

        public override void SetInitialValue(object value)
        {
            var tempsAndRpms = (int[][]) value;
        }

        private void AddHardwareSensorsData(IEnumerable<IHardware> hardwareList)
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
