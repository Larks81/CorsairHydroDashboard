using CorsairDashboard.HardwareMonitoring.Hw.Sensors;
using System;
using System.Runtime.Serialization;

namespace CorsairDashboard.HardwareMonitoring
{
    [DataContract]
    [KnownType(typeof(TemperatureSensor))]
    [KnownType(typeof(UsageSensor))]
    public abstract class HardwareSensor
    {
        [DataMember]
        public String Id { get; protected set; }

        [DataMember]
        public String Name { get; protected set; }

        [DataMember]
        public object Value { get; protected set; }
    }
}