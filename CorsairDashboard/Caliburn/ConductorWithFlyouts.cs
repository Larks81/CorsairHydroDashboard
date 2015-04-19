using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Caliburn
{
    public abstract class ConductorWithFlyouts<T> : Conductor<T> where T : class
    {
        public BindableCollection<FlyoutScreen> WindowFlyouts { get; protected set; }

        public ConductorWithFlyouts()
        {
            WindowFlyouts = new BindableCollection<FlyoutScreen>();
        }

        protected void OpenFlyout<T>() where T : FlyoutScreen
        {
            EnumerateFlyoutOfType<T>(flyout => flyout.IsOpen = true);         
        }

        protected void CloseFlyout<T>() where T : FlyoutScreen
        {
            EnumerateFlyoutOfType<T>(flyout => flyout.IsOpen = false);
        }

        protected void ToggleFlyout<T>() where T : FlyoutScreen
        {
            EnumerateFlyoutOfType<T>(flyout => flyout.IsOpen = !flyout.IsOpen);
        }

        private void EnumerateFlyoutOfType<T>(Action<FlyoutScreen> action) where T : FlyoutScreen
        {
            foreach (var flyout in WindowFlyouts.OfType<T>())
            {
                action(flyout);
            }  
        }
    }
}
