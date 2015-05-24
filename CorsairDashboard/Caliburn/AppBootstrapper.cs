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
using CorsairDashboard.Settings;
using MahApps.Metro;
using System.Data.Entity;
using CorsairDashboard.Common.SqliteMigrations;

namespace CorsairDashboard.Caliburn
{
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer container;
        private ISettings settings;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            Database.SetInitializer<SettingsContext>(new SqliteMigrationsInitializer<SettingsContext>());
            settings = new SettingsContext();
            settings.Initialize();

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
            batch.AddExportedValue<IMetroWindowManager>(new MahAppsWindowManager());
            batch.AddExportedValue<ISettings>(settings);
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
            var accentColor = settings.AccentColor;
            var themeColor = settings.ThemeColor;
            var accent = ThemeManager.Accents.First(a => a.Name == accentColor);
            var theme = ThemeManager.AppThemes.First(t => t.Name == themeColor);
            ThemeManager.ChangeAppStyle(Application, accent, theme);
            
            DisplayRootViewFor<IShell>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            settings.SaveSettings();
            settings = null;

            base.OnExit(sender, e);
        }
    }
}
