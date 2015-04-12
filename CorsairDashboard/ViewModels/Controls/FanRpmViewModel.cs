using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CorsairDashboard.ViewModels.Controls
{
    public class FanRpmViewModel : PropertyChangedBase
    {
        int fanNr, rpm;

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
                    NotifyOfPropertyChange(() => RotateAnimationDuration);
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
                    NotifyOfPropertyChange(() => RotateAnimationDuration);
                }
            }
        }

        public String Description
        {
            get
            {
                return String.Format("Fan {0}: {1} rpm", FanNr, Rpm);
            }
        }

        public Duration RotateAnimationDuration
        {
            get
            {
                int sec;
                if (rpm == 0)
                {
                    sec = 0;
                }
                else
                {
                    sec = 1;
                }
                return new Duration(TimeSpan.FromSeconds(sec));
            }
        }
    }
}
