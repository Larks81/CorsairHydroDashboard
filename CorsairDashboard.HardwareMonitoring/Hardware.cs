using CorsairDashboard.HardwareMonitoring.Hw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HardwareMonitoring
{
    [DataContract]
    [KnownType(typeof(Cpu))]
    [KnownType(typeof(Gpu))]
    [KnownType(typeof(Hdd))]
    [KnownType(typeof(Mainboard))]
    public abstract class Hardware
    {
        [DataMember]
        public String Id { get; protected set; }

        [DataMember]
        public String Name { get; protected set; }

        [DataMember]
        public virtual HardwareKind Kind
        {
            get { return HardwareKind.Unkown; }
            private set { }
        }

        [DataMember]
        public IEnumerable<HardwareSensor> Sensors { get; private set; }

        public Hardware()
        {
            Sensors = new List<HardwareSensor>();
        }

        protected void AddSensor(HardwareSensor sensor)
        {
            ((List<HardwareSensor>)Sensors).Add(sensor);
        }
    }
}
