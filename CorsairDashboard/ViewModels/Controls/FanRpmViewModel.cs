using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using CorsairDashboard.Caliburn;

namespace CorsairDashboard.ViewModels.Controls
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class FanRpmViewModel : PropertyChangedBase
    {
        private int fanNr, rpm;
        private readonly IShell shell;

        public int FanNr
        {
            get { return fanNr; }
            set
            {
                if (fanNr != value)
                {
                    fanNr = value;
                    NotifyOfPropertyChange(() => FanNr);
                    NotifyOfPropertyChange(() => Description);
                    NotifyOfPropertyChange(() => AnimationSpeed);
                }
            }
        }

        public int Rpm
        {
            get { return rpm; }
            set
            {
                if (rpm != value)
                {
                    rpm = value;
                    NotifyOfPropertyChange(() => Rpm);
                    NotifyOfPropertyChange(() => Description);
                    NotifyOfPropertyChange(() => AnimationSpeed);
                }
            }
        }

        public String Description
        {
            get
            {
                var label = shell.Settings.GetLabelForFan(shell.HydroDeviceDataProvider.SelectedDeviceId, fanNr);
                if (String.IsNullOrEmpty(label))
                {
                    label = String.Format("Fan {0}", fanNr);
                }
                return String.Format("{0}: {1} rpm", label, Rpm);
            }
        }

        public double AnimationSpeed
        {
            get
            {
                if (rpm == 0)
                    return 0.0f;

                return 1.0f * ((double)rpm / 1500.0);
            }
        }

        [ImportingConstructor]
        public FanRpmViewModel(IShell shell)
        {
            this.shell = shell;
        }
    }
}
