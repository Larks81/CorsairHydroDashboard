using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace CorsairDashboard.WindowsService
{
    public static class ServiceSettings
    {
        [DataContract]
        class InternalSettings
        {
            [DataMember]
            public Dictionary<FanKey, string> FansSensors { get; set; }

            public InternalSettings()
            {
                FansSensors = new Dictionary<FanKey, string>();
            }
        }

        [DataContract]
        public class FanKey
        {
            [DataMember]
            public Guid DeviceId { get; set; }

            [DataMember]
            public int FanNr { get; set; }

            public FanKey()
            {

            }

            public FanKey(Guid deviceId, int fanNr)
            {
                DeviceId = deviceId;
                FanNr = fanNr;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is FanKey))
                    return false;

                var otherFanKey = (FanKey)obj;
                return DeviceId == otherFanKey.DeviceId && FanNr == otherFanKey.FanNr;
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + DeviceId.GetHashCode();
                hash = (hash * 7) + FanNr.GetHashCode();
                return hash;
            }
        }

        private static InternalSettings internalSettings;
        private const String FileName = @"C:\ServiceSettings.xml";

        public static void LoadSettings()
        {
            if (File.Exists(FileName))
            {
                var ser = new DataContractSerializer(typeof(InternalSettings));
                using (var reader = new StreamReader(FileName))
                using (var xr = XmlReader.Create(reader))
                {
                    internalSettings = (InternalSettings)ser.ReadObject(xr);
                }
            }
            else
            {
                internalSettings = new InternalSettings();
            }
        }

        public static void SaveSettings()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);


            var ser = new DataContractSerializer(typeof(InternalSettings));
            using (var writer = new StreamWriter(FileName))
            using (var xw = XmlWriter.Create(writer))
            {
                ser.WriteObject(xw, internalSettings);
            }
        }

        public static void SetFanSensorId(Guid deviceId, int fanNr, string sensorId)
        {
            var fanKey = new FanKey(deviceId, fanNr);
            internalSettings.FansSensors[fanKey] = sensorId;
        }

        public static String GetFanSensorId(Guid deviceId, int fanNr)
        {
            String sensorId = null;
            var fanKey = new FanKey(deviceId, fanNr);
            internalSettings.FansSensors.TryGetValue(fanKey, out sensorId);
            return sensorId;
        }

        public static IEnumerable<String> GetAllFansSensorsId()
        {
            return internalSettings.FansSensors.Values.Distinct();
        }
    }
}
