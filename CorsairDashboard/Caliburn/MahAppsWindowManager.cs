using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace CorsairDashboard.Caliburn
{
    [Export(typeof(IWindowManager))]
    public class MahAppsWindowManager : WindowManager
    {
       /* public bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            if (metroWindow == null)
                return false;

            var view = ViewLocator.LocateForModel(rootModel, null, context);
            var dialog = new CustomDialog()
            {
                Content = view               
            };
            
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok"
            };
            metroWindow.ShowMetroDialogAsync(dialog, dialogSettings);
            return true;
        }

        public void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            throw new NotImplementedException();
        }

        public void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            var view = ViewLocator.LocateForModel(rootModel, null, context) as MetroWindow;
            if (view == null)
                return;
            
            view.SizeToContent = SizeToContent.WidthAndHeight;
            view.Owner = Application.Current.MainWindow as MetroWindow;
            view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            view.Show();
        }*/

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = view as Window;

            if (window == null)
            {
                window = new MetroWindow()
                {
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight
                };

                window.SetValue(View.IsGeneratedProperty, true);

                var owner = InferOwnerOf(window);
                if (owner != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = owner;
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            else
            {
                var owner = InferOwnerOf(window);
                if (owner != null && isDialog)
                {
                    window.Owner = owner;
                }
            }

            return window;
        }
    }
}
