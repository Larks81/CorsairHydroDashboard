using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OpenHardwareMonitor.Reactive
{
    public class ReactiveOpenHardwareMonitor : IDisposable
    {
        private Computer computer;
        private CancellationTokenSource updateCancellationToken;
        private Subject<IHardware> hardwareSubject;

        public IObservable<IHardware> HardwareUpdatesObservable
        {
            get { return hardwareSubject; }
        }

        public ReactiveOpenHardwareMonitor()
        {
            hardwareSubject = new Subject<IHardware>();
        }

        public void BeginMonitoring()
        {
            computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true,
                MainboardEnabled = true,
                RAMEnabled = true,
                FanControllerEnabled = true
            };
            computer.Open();
            updateCancellationToken = new CancellationTokenSource();
            var token = updateCancellationToken.Token;
            Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await UpdateValues();
                    await Task.Delay(1000, token);
                }
            }, token);
        }

        private async Task UpdateValues()
        {
            await Task.Run(() =>
            {
                foreach (var hardware in computer.Hardware)
                {
                    if (updateCancellationToken.IsCancellationRequested)
                        return;

                    hardware.Update();
                    foreach (IHardware subHardware in hardware.SubHardware)
                        subHardware.Update();

                    hardwareSubject.OnNext(hardware);
                }
            });
        }

        public void Dispose()
        {
            hardwareSubject.Dispose();
            updateCancellationToken.Cancel();
        }
    }
}
