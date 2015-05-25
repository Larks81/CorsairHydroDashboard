using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using CorsairDashboard.Common.Service;
using CorsairDashboard.WindowsService.HardwareMonitoring;
using log4net;
using log4net.Config;

namespace CorsairDashboard.WindowsService
{
    public partial class CorsairHydroService : ServiceBase
    {
	//Comment to test pull request
        const String NetNamedPipeUri = "net.pipe://localhost/CorsairHydroService";

        private readonly ILog log = LogManager.GetLogger("HydroServiceLogger");
        private ServiceHost serviceHost;
        private HydroWSService wsService;
        private HardwareMonitorService hardwareMonitorService;

        public CorsairHydroService(HardwareMonitorService hardwareMonitorService)
        {
            this.hardwareMonitorService = hardwareMonitorService;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service started");            
            wsService = new HydroWSService(log, hardwareMonitorService);
            serviceHost = ServiceHostFactory.ServiceHostForSingleInstance<ICorsairHydroService>(wsService,
                NetNamedPipeUri, log);
        }

        protected override void OnStop()
        {
            //workerThread.Join(new TimeSpan(0, 1, 0));

            try
            {
                if (serviceHost != null)
                    serviceHost.Close();                
            }
            catch (Exception e)
            {
                log.Error("Error closing WCF service", e);
            }

            if (wsService != null)
                wsService.Dispose();

            log.Info("Service stopped");
        }
    }
}
