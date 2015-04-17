using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.HydroDataProvider;
using CorsairDashboard.ViewModels.Controls;
using HydroLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace CorsairDashboard.ViewModels
{
    [Export(typeof(IShell))]
    class ShellViewModel : Conductor<object>, IShell
    {
        private CancellationTokenSource updateStatsCancellationToken;
        private int nrOfFans;

        public HydroDeviceDataProvider HydroDeviceDataProvider { get; private set; }

        public String WaterTemperature { get; private set; }

        public String ModelName { get; private set; }

        public BindableCollection<FanRpmViewModel> FansRpm { get; set; }

        public bool IsOriginalCorsairLinkRunning
        {
            get
            {
                return Process.GetProcessesByName("CorsairLink").Any();
            }
        }

        public async void Start()
        {
            SetupDevice();
            DisplayName = "Corsair Hydro Dashboard";
            MainMenu();
            ModelName = await HydroDeviceDataProvider.ModelName;
            NotifyOfPropertyChange(() => ModelName);

            nrOfFans = await HydroDeviceDataProvider.NumberOfFans;
            FansRpm = new BindableCollection<FanRpmViewModel>();
            for (int i = 0; i < nrOfFans; i++)
            {
                var fanVm = new FanRpmViewModel() { FanNr = i };
                FansRpm.Add(fanVm);
                HydroDeviceDataProvider.Fans[i]
                    .Where(fanInfo => fanInfo != null)
                    .Subscribe(
                        fanInfo =>
                        {
                            fanVm.Rpm = fanInfo.Rpm;
                        });
            }
            NotifyOfPropertyChange(() => FansRpm);

            HydroDeviceDataProvider.Temperature.Subscribe(Observer.Create<int>(temp =>
            {
                if (temp == 0)
                {
                    WaterTemperature = "N/A";
                }
                else
                {
                    WaterTemperature = temp.ToString() + " °C";
                }
                NotifyOfPropertyChange(() => WaterTemperature);
            }));

        }

        protected override void OnDeactivate(bool close)
        {
            if (updateStatsCancellationToken != null)
            {
                updateStatsCancellationToken.Cancel();
            }
            base.OnDeactivate(close);
        }

        public void ChangeCurrentDisplayedViewModelTo(ChildBaseViewModel newViewModel)
        {
            ActivateItem(newViewModel);
            NotifyOfPropertyChange("MainMenu");
        }

        public void MainMenu()
        {
            ActivateItem(IoC.Get<MainMenuViewModel>());
        }

        public bool CanMainMenu()
        {
            return true;
        }

        private void SetupDevice()
        {
            var hydroEnumerator = new HydroDeviceEnumerator(canReturnNullDevice: true);
            var hydroDevice = hydroEnumerator.First();
            HydroDeviceDataProvider = new HydroDeviceDataProvider(hydroDevice);
            HydroDeviceDataProvider.BeginUpdate();
        }
    }
}