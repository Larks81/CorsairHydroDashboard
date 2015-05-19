using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Caliburn
{
    [Export]
    public abstract class ScreenWithShell : Screen
    {
        protected IShell Shell { get; private set; }

        [ImportingConstructor]
        public ScreenWithShell(IShell shell)
        {
            this.Shell = shell;
        }
    }
}
