using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using log4net;

namespace CorsairDashboard.WindowsService
{
    public static class ServiceHostFactory
    {
        public static ServiceHost ServiceHostForSingleInstance<TContract>(TContract instance, string namedPipeUri, ILog log) where TContract : class
        {            
            try
            {
                var serviceHost = new ServiceHost(instance);
                serviceHost.AddServiceEndpoint(
                    typeof(TContract),
                    new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
                    namedPipeUri);
                var smb = new ServiceMetadataBehavior { MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 } };
                serviceHost.Description.Behaviors.Add(smb);

                serviceHost.AddServiceEndpoint(
                    ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                    new Uri(namedPipeUri + "/mex"));

                serviceHost.Open();

                log.InfoFormat("WCF service up and running at {0}", namedPipeUri);

                foreach (var baseAddress in serviceHost.BaseAddresses)
                {
                    log.InfoFormat("WCF service available at {0}", baseAddress.AbsoluteUri);
                }

                return serviceHost;
            }
            catch (Exception e)
            {
                log.Error("Error initializing the WCF service", e);
            }
            return null;
        }
    }
}
