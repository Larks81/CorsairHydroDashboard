using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;

namespace CorsairDashboard.Caliburn
{
    public interface IMetroWindowManager : IWindowManager
    {
        Task<MessageDialogResult> ShowMessageAsync(String title, String message, 
            MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null);

        Task<ProgressDialogController> ShowProgressAsync(String title, String message);
    }
}