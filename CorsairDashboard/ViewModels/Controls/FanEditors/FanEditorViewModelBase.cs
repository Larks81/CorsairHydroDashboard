using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CorsairDashboard.ViewModels.Controls.FanEditors
{
    public abstract class FanEditorViewModelBase : PropertyChangedBase
    {
        public abstract Object ValueForParent { get; }

        public bool InitialValueSet { get; protected set; }

        public abstract void SetInitialValue(object value);
    }
}
