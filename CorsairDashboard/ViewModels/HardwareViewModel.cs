using Caliburn.Micro;
using CorsairDashboard.HardwareMonitorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.ViewModels
{
    public class HardwareViewModel
    {
        public String Name { get; private set; }

        public BindableCollection<SensorViewModel> Sensors { get; private set; }

        public HardwareViewModel(Hardware hw)
        {
            Name = hw.Name;
            Sensors = new BindableCollection<SensorViewModel>();
        }
    }

    public class SensorViewModel : PropertyChangedBase
    {
        private String value;
        private Type sensorType;

        public String SensorId { get; private set; }

        public String SensorName { get; private set; }

        public String Value
        {
            get { return value; }
            private set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        public SensorViewModel(HardwareSensor sensor)
        {
            SensorId = sensor.Id;
            SensorName = sensor.Name;
            sensorType = sensor.GetType();
        }

        public void Update(HardwareSensor sensor)
        {
            if (sensorType == typeof(TemperatureSensor))
            {
                Value = String.Format("{0:N0} °C", sensor.Value);
            }
            else if (sensorType == typeof(UsageSensor))
            {
                Value = String.Format("{0:N2} %", sensor.Value);
            }
            else
            {
                Value = sensor.Value.ToString();
            }
        }
    }

    public class SensorWithHardwareNameViewModel : SensorViewModel
    {
        public String HardwareName { get; set; }

        public SensorWithHardwareNameViewModel(Hardware hw, HardwareSensor sensor)
            : base(sensor)
        {
            HardwareName = hw.Name;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", HardwareName, SensorName);
        }
    }
}
