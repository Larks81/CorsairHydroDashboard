using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.ViewModels.Controls;
using HydroLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorsairDashboard.ViewModels
{
    [Export(typeof(IShell))]
    class ShellViewModel : Conductor<object>, IShell
    {
        private CancellationTokenSource updateStatsCancellationToken;
        private int nrOfFans;

        public IHydroDevice HydroDevice { get; private set; }

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
            ModelName = await HydroDevice.GetModelNameAsync();
            NotifyOfPropertyChange(() => ModelName);

            nrOfFans = await HydroDevice.GetNrOfFansAsync();
            FansRpm = new BindableCollection<FanRpmViewModel>();
            for (int i = 0; i < nrOfFans; i++)
            {
                FansRpm.Add(new FanRpmViewModel() { FanNr = i, Rpm = 1000 });
            }
            NotifyOfPropertyChange(() => FansRpm);

            BeginUpdateStats();
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
            HydroDevice = hydroEnumerator.First();
        }

        private void BeginUpdateStats()
        {
            updateStatsCancellationToken = new CancellationTokenSource();
            var token = updateStatsCancellationToken.Token;
            Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await UpdateWaterTemperature();
                    await UpdateFans();
                    await Task.Delay(500);
                }
            }, updateStatsCancellationToken.Token);
        }

        private async Task UpdateWaterTemperature()
        {
            var temp = await HydroDevice.GetTemperatureAsync();
            if (temp == 0)
            {
                WaterTemperature = "N/A";
            }
            else
            {
                WaterTemperature = temp.ToString() + " °C";
            }
            NotifyOfPropertyChange(() => WaterTemperature);
        }

        private async Task UpdateFans()
        {
            for (int i = 0; i < nrOfFans; i++)
            {
                var rpm = await HydroDevice.GetRpmForFanNrAsync((byte)i);
                var fanViewModel = FansRpm.ElementAt(i);
                fanViewModel.Rpm = rpm;
            }
        }
    }
}