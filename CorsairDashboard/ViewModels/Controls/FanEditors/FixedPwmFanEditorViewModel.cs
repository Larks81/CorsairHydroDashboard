using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CorsairDashboard.ViewModels.Controls.FanEditors
{
    public class FixedPwmFanEditorViewModel : FanEditorViewModelBase
    {
        private byte value;

        public byte Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        public override Object ValueForParent
        {
            get { return Value; }
        }

        public override void SetInitialValue(object value)
        {
            this.value = (byte) value;
            InitialValueSet = true;
        }
    }
}
