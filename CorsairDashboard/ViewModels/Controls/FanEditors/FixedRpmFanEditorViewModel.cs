using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.ViewModels.Controls.FanEditors
{
    public class FixedRpmFanEditorViewModel : FanEditorViewModelBase
    {
        private UInt16 value;

        public UInt16 Value
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

        public override object ValueForParent
        {
            get { return value; }
        }

        public override void SetInitialValue(object value)
        {
            this.value = (UInt16)value;
            InitialValueSet = true;
        }
    }
}
