using Caliburn.Micro;
using CorsairDashboard.Caliburn;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.ViewModels
{
    [Export]
    public abstract class ChildBaseViewModel : Screen
    {
        protected IShell Shell { get; private set; }

        [ImportingConstructor]
        public ChildBaseViewModel(IShell shell)
        {
            this.Shell = shell;
        }
    }
}
