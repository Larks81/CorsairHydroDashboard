using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.ServiceModel;
using System.Threading;
using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using CorsairDashboard.Common.Service;
using CorsairDashboard.HydroService;
using CorsairDashboard.ViewModels.Controls;
using CorsairDashboard.ServiceWrapper;
using CorsairDashboard.HardwareMonitorService;
using CorsairDashboard.Settings;

namespace CorsairDashboard.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ConductorWithFlyouts<object>, IShell
    {
        private int nrOfFans;
        private CorsairHydroServiceState serviceState;
        private bool serviceFaulted;
        private CorsairDashboard.HydroService.ICorsairHydroService hydroService;
        private IHardwareMonitorService hwMonitorService;
        private IWindowManager windowManager;

        public HydroDeviceDataProvider HydroDeviceDataProvider { get; private set; }

        public ReactiveHardwareMonitoring HardwareMonitoringProvider { get; private set; }

        public ISettings Settings { get; private set; }

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

        public CorsairHydroServiceState ServiceState
        {
            get { return serviceState; }
            set
            {
                if (serviceState != value)
                {
                    serviceState = value;
                    NotifyOfPropertyChange(() => ServiceState);
                }
            }
        }

        public bool ServiceFaulted
        {
            get { return serviceFaulted; }
            set
            {
                if (serviceFaulted != value)
                {
                    serviceFaulted = value;
                    NotifyOfPropertyChange(() => ServiceFaulted);
                }
            }
        }

        public bool ServiceBootedOk
        {
            get { return ServiceState == CorsairHydroServiceState.Ready; }
        }
        
        [ImportingConstructor]
        public ShellViewModel(IWindowManager windowManager, ISettings settings)
        {
            Settings = settings;
            this.windowManager = windowManager;
            if (Execute.InDesignMode)
                LoadDesignTimeData();
        }

        public async void Start()
        {
            ServiceState = CorsairHydroServiceState.Bootstrapping;
            DisplayName = "Corsair Hydro Dashboard";

            try
            {
                HardwareMonitoringProvider = new ReactiveHardwareMonitoring();
                hwMonitorService = new HardwareMonitorServiceClient(new InstanceContext(HardwareMonitoringProvider));
                await hwMonitorService.SubscribeAsync();

                HydroDeviceDataProvider = new HydroDeviceDataProvider();
                hydroService = new CorsairHydroServiceClient(new InstanceContext(HydroDeviceDataProvider));
                await HydroDeviceDataProvider.Initialize(hydroService);

                ServiceState = await hydroService.GetServiceStateAsync();
                NotifyOfPropertyChange(() => ServiceBootedOk);

                switch (ServiceState)
                {
                    case CorsairHydroServiceState.Ready:
                        WindowFlyouts.Add(new SettingsViewModel(this));
                        WindowFlyouts.Add(new HardwareMonitorViewModel(this, HardwareMonitoringProvider));
                        MainMenu();
                        ModelName = HydroDeviceDataProvider.ModelName;
                        NotifyOfPropertyChange(() => ModelName);

                        nrOfFans = await HydroDeviceDataProvider.NumberOfFans;
                        FansRpm = new BindableCollection<FanRpmViewModel>();
                        for (int i = 0; i < nrOfFans; i++)
                        {
                            var fanVm = IoC.Get<FanRpmViewModel>();
                            fanVm.FanNr = i;
                            FansRpm.Add(fanVm);
                            HydroDeviceDataProvider.Fans.ElementAt(i)
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
                        break;

                }
            }
            catch (EndpointNotFoundException e)
            {
                ServiceFaulted = true;
            }

        }

        protected override void OnDeactivate(bool close)
        {
            HydroDeviceDataProvider.Dispose();
            hwMonitorService.Unsubscribe();            
            base.OnDeactivate(close);
        }

        public void ChangeCurrentDisplayedViewModelTo(ChildBaseViewModel newViewModel)
        {
            ActivateItem(newViewModel);
        }

        public void MainMenu()
        {
            ActivateItem(IoC.Get<MainMenuViewModel>());
        }

        public void OpenHardwareMonitoring()
        {
            OpenFlyout<HardwareMonitorViewModel>();
        }

        public void OpenSettings()
        {

            OpenFlyout<SettingsViewModel>();
        }

        public bool CanMainMenu()
        {
            return true;
        }

        private void LoadDesignTimeData()
        {
            ServiceFaulted = false;
            //ServiceState = CorsairHydroServiceState.SearchingDevice;
        }
    }
}