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
    [Export(typeof(IMetroWindowManager))]
    public class MahAppsWindowManager : WindowManager, IMetroWindowManager
    {
        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            var metroWindow = await FindMetroWindow();
            if (metroWindow.Dispatcher.CheckAccess())
            {
                return await metroWindow.ShowMessageAsync(title, message, style, settings);
            }
            else
            {
                return await metroWindow.Dispatcher.Invoke(() => metroWindow.ShowMessageAsync(title, message, style, settings));
            }
        }

        public async Task<ProgressDialogController> ShowProgressAsync(String title, String message)
        {
            var metroWindow = await FindMetroWindow();
            if (metroWindow.Dispatcher.CheckAccess())
            {
                return await metroWindow.ShowProgressAsync(title, message);
            }
            else
            {
                return await metroWindow.Dispatcher.Invoke(() => metroWindow.ShowProgressAsync(title, message));
            }
        }
        
        protected async Task<MetroWindow> FindMetroWindow()
        {
            var findMetroWindowFunc = new Func<MetroWindow>(() => Application.Current.MainWindow as MetroWindow);

            if (Application.Current.CheckAccess())
            {
                return findMetroWindowFunc();
            }
            else
            {
                return await Application.Current.Dispatcher.InvokeAsync(findMetroWindowFunc);
            }
        }

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
