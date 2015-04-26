using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CorsairDashboard.WindowsService.HardwareMonitoring;
using log4net.Config;

namespace CorsairDashboard.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            XmlConfigurator.Configure();

            var hardwareMonitorService = new HardwareMonitorService();
            var hydroService = new CorsairHydroService(hardwareMonitorService);

            ServiceBase.Run(new ServiceBase[] { hardwareMonitorService, hydroService });
        }
    }
}
