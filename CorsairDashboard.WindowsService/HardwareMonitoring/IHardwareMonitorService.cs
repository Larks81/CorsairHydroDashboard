using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.WindowsService.HardwareMonitoring
{
    [ServiceContract(CallbackContract = typeof(IHardwareMonitorServiceCallback))]
    public interface IHardwareMonitorService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe();

        void InternalSubscribe(IHardwareMonitorServiceCallback callback);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();

        void InternalUnsubscribe(IHardwareMonitorServiceCallback callback);
    }
}
