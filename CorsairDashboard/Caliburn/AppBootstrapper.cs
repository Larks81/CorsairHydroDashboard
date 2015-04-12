using Caliburn.Micro;
using CorsairDashboard.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CorsairDashboard.Caliburn
{
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            ConfigureIoC();
            ConfigureValueConverters();
        }

        private void ConfigureValueConverters()
        {
            var baseApplyValueConverter = ConventionManager.ApplyValueConverter;
            ConventionManager.ApplyValueConverter = (binding, bindableProperty, property) =>
            {
                baseApplyValueConverter(binding, bindableProperty, property);

                if ((bindableProperty == TextBox.TextProperty || bindableProperty == TextBlock.TextProperty) &&
                    typeof(SolidColorBrush).IsAssignableFrom(property.PropertyType))
                {
                    binding.Converter = new SolidColorBrushConverter();
                }
                else if ((bindableProperty == TextBox.TextProperty || bindableProperty == TextBlock.TextProperty) &&
                  typeof(Color).IsAssignableFrom(property.PropertyType))
                {
                    binding.Converter = new CorsairDashboard.Converters.ColorConverter();
                }
            };
        }

        private void ConfigureIoC()
        {
            var aggregateCatalog = new AggregateCatalog(AssemblySource.Instance.Select(assembly => new AssemblyCatalog(assembly)));
            container = new CompositionContainer(aggregateCatalog);
            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);
            container.Compose(batch);
        }

        protected override object GetInstance(Type service, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Any())
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}
