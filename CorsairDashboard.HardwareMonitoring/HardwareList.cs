using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CorsairDashboard.HardwareMonitoring.Adapters;

namespace CorsairDashboard.HardwareMonitoring
{
    public class HardwareList : IHardwareMonitorAdapterCallback, IDisposable
    {
        private readonly HardwareMonitoringAdapterBase adapter;
        private readonly List<IHardware> hardwareList;
        private readonly Dictionary<HardwareKind, ISubject<IEnumerable<IHardware>>> signals;

        public IEnumerable<IHardware> AllHardware
        {
            get { return hardwareList.AsReadOnly(); }
        }

        public HardwareList(HardwareMonitoringAdapterBase adapter)
        {            
            hardwareList = new List<IHardware>();
            signals = new Dictionary<HardwareKind, ISubject<IEnumerable<IHardware>>>();
            this.adapter = adapter;
            adapter.Callback = this;
            adapter.BeginAdapt();
        }

        public IObservable<IEnumerable<IHardware>> GetSignalForHardwareOfKind(HardwareKind kind)
        {
            ISubject<IEnumerable<IHardware>> signal;
            if (!signals.TryGetValue(kind, out signal))
            {
                signal = new ReplaySubject<IEnumerable<IHardware>>();
                signal.OnNext(GetHardwareOfKind(kind));
                signals[kind] = signal;
            }
            return signal;
        }

        public IEnumerable<IHardware> GetHardwareOfKind(HardwareKind kind)
        {
            return hardwareList.Where(h => h.Kind == kind).ToList().AsReadOnly();
        }

        void IHardwareMonitorAdapterCallback.UpdateHardware(IHardware newHardware)
        {
            hardwareList.RemoveAll(h => h.Id == newHardware.Id);
            hardwareList.Add(newHardware);                      
            ISubject<IEnumerable<IHardware>> subject;
            if (signals.TryGetValue(newHardware.Kind, out subject))
            {
                subject.OnNext(GetHardwareOfKind(newHardware.Kind));
            }
        }

        public void Dispose()
        {
            signals.Clear();
            adapter.Dispose();
        }
    }
}