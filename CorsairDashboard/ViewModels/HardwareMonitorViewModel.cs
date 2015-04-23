﻿using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.HardwareMonitoring;
using CorsairDashboard.HardwareMonitoring.Adapters;
using CorsairDashboard.HardwareMonitoring.Hw.Sensors;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CorsairDashboard.ViewModels
{
    public class HardwareMonitorViewModel : FlyoutScreen
    {
        private HardwareList hardwareList;

        public BindableCollection<HardwareViewModel> Hardware { get; set; }

        public HardwareMonitorViewModel()
            : base("Hardware Monitoring", Position.Right)
        {
            Hardware = new BindableCollection<HardwareViewModel>();
            hardwareList = new HardwareList(new OpenHardwareMonitorAdapter());

            hardwareList.GetSignalForHardwareOfKind(HardwareKind.Cpu)
                .Subscribe(AddHardwareSensorsData);

            hardwareList.GetSignalForHardwareOfKind(HardwareKind.GraphicCard)
                .Subscribe(AddHardwareSensorsData);

            hardwareList.GetSignalForHardwareOfKind(HardwareKind.HardDisk)
                .Subscribe(AddHardwareSensorsData);
        }

        private void AddHardwareSensorsData(IEnumerable<IHardware> hardwareList)
        {
            foreach (var hardware in hardwareList)
            {
                var hw = Hardware.FirstOrDefault(h => h.Name == hardware.Name);
                if (hw == null)
                {
                    hw = new HardwareViewModel(hardware);
                    Hardware.Add(hw);
                }

                foreach (var sensor in hardware.Sensors)
                {
                    var hardwareSensorViewModel = hw.Sensors.FirstOrDefault(hsvm => hsvm.SensorId == sensor.Id);
                    if (hardwareSensorViewModel == null)
                    {
                        hardwareSensorViewModel = new SensorViewModel(sensor);
                        hw.Sensors.Add(hardwareSensorViewModel);
                    }
                    hardwareSensorViewModel.Update(sensor);
                }
            }
        }

        protected override void OnDeactivate(bool close)
        {
            hardwareList.Dispose();
            base.OnDeactivate(close);
        }
    }
}
