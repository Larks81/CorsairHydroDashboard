using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CorsairDashboard.ViewModels.Controls
{
    public class RangeColorChooserViewModel : PropertyChangedBase
    {
        private byte r, g, b;
        private Color color;

        public byte R
        {
            get { return r; }
            set
            {
                if (r != value)
                {
                    r = value;
                    color = Color.FromRgb(r, g, b);
                    NotifyOfPropertyChange(() => CurrentColor);
                    NotifyOfPropertyChange(() => R);
                }
            }
        }

        public byte G
        {
            get { return g; }
            set
            {
                if (g != value)
                {
                    g = value;
                    color = Color.FromRgb(r, g, b);
                    NotifyOfPropertyChange(() => CurrentColor);
                    NotifyOfPropertyChange(() => G);
                }
            }
        }

        public byte B
        {
            get { return b; }
            set
            {
                if (b != value)
                {
                    b = value;
                    color = Color.FromRgb(r, g, b);
                    NotifyOfPropertyChange(() => CurrentColor);
                    NotifyOfPropertyChange(() => B);
                }
            }
        }

        public Color CurrentColor
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    r = color.R;
                    g = color.G;
                    b = color.B;
                    NotifyOfPropertyChange(() => R);
                    NotifyOfPropertyChange(() => G);
                    NotifyOfPropertyChange(() => B);
                    NotifyOfPropertyChange(() => CurrentColor);
                }
            }
        }

        public RangeColorChooserViewModel()
            : base()
        {

        }
    }
}
