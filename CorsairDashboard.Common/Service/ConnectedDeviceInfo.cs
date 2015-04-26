using System;
using System.Runtime.Serialization;

namespace CorsairDashboard.Common.Service
{
    [DataContract]
    public class ConnectedDeviceInfo
    {
        [DataMember]
        public Guid DeviceId { get; private set; }

        [DataMember]
        public String ModelName { get; private set; }

        public ConnectedDeviceInfo(Guid deviceId, String modelName)
        {
            DeviceId = deviceId;
            ModelName = modelName;
        }
    }
}