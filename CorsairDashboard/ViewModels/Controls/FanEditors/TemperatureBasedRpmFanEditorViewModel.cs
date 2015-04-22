using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CorsairDashboard.ViewModels.Controls.FanEditors
{
    public class TemperatureBasedRpmFanEditorViewModel : FanEditorViewModelBase
    {
        public class TemperatureRpmViewModel
        {
            public int Temperature { get; set; }

            public int Rpm { get; set; }
        }

        public BindableCollection<TemperatureRpmViewModel> TemperaturesAndRpms { get; private set; }

        public TemperatureBasedRpmFanEditorViewModel()
        {
            TemperaturesAndRpms = new BindableCollection<TemperatureRpmViewModel>();
            TemperaturesAndRpms.AddRange(
                Enumerable.Range(0, 5)
                .Select(_ => new TemperatureRpmViewModel()
                {
                    Temperature = 25,
                    Rpm = 1000
                }));
        }

        public override object ValueForParent
        {
            get
            {
                return new[]
                {
                    TemperaturesAndRpms.Select(t => t.Temperature).ToArray(),
                    TemperaturesAndRpms.Select(t => t.Rpm).ToArray()
                };
            }
        }

        public override void SetInitialValue(object value)
        {
            var tempsAndRpms = (int[][]) value;
        }
    }
}
