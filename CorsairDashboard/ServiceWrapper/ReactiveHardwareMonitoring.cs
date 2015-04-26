using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorsairDashboard.HardwareMonitorService;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace CorsairDashboard.ServiceWrapper
{
    public class ReactiveHardwareMonitoring : IHardwareMonitorServiceCallback, IDisposable
    {
        private readonly List<Hardware> hardwareList;
        private readonly Dictionary<HardwareKind, ISubject<IEnumerable<Hardware>>> signals;

        public IEnumerable<Hardware> AllHardware
        {
            get { return hardwareList.AsReadOnly(); }
        }

        public ReactiveHardwareMonitoring()
        {
            hardwareList = new List<Hardware>();
            signals = new Dictionary<HardwareKind, ISubject<IEnumerable<Hardware>>>();
        }

        public IObservable<IEnumerable<Hardware>> GetSignalForHardwareOfKind(HardwareKind kind)
        {
            ISubject<IEnumerable<Hardware>> signal;
            if (!signals.TryGetValue(kind, out signal))
            {
                signal = new ReplaySubject<IEnumerable<Hardware>>();
                signal.OnNext(GetHardwareOfKind(kind));
                signals[kind] = signal;
            }
            return signal.AsObservable();
        }

        public IEnumerable<Hardware> GetHardwareOfKind(HardwareKind kind)
        {
            return hardwareList.Where(h => h.Kind == kind).ToList().AsReadOnly();
        }

        public void Dispose()
        {
            foreach (var signal in signals.Values)
            {
                signal.OnCompleted();
            }
            signals.Clear();
        }

        #region HardwareServiceCallback

        public void OnHardwareMonitorUpdate(Hardware hardware)
        {
            hardwareList.RemoveAll(h => h.Id == hardware.Id);
            hardwareList.Add(hardware);
            ISubject<IEnumerable<Hardware>> subject;
            if (signals.TryGetValue(hardware.Kind, out subject))
            {
                subject.OnNext(GetHardwareOfKind(hardware.Kind));
            }
        }

        #endregion
    }
}
