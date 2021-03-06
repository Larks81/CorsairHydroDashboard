﻿using CorsairDashboard.HardwareMonitoring.Hw.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring.Hw
{
    [DataContract]
    public class Hdd : Hardware
    {
        public override HardwareKind Kind
        {
            get
            {
                return HardwareKind.HardDisk;
            }
        }

        public Hdd(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void SetTemperature(String sensorId, float temp)
        {
            AddSensor(new TemperatureSensor(sensorId, temp));
        }
    }
}
