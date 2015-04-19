using CorsairDashboard.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CorsairDashboard.Views
{
    /// <summary>
    /// Interaction logic for HardwareMonitorView.xaml
    /// </summary>
    public partial class HardwareMonitorView : UserControl
    {
        public HardwareMonitorView()
        {
            InitializeComponent();
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            //hide the expander            
            var contentPresenter = VisualTreeHelper.GetParent((DependencyObject)sender);
            var contentPresenterParent = VisualTreeHelper.GetParent(contentPresenter) as FrameworkElement;
            var expander = contentPresenterParent.FindName("Expander") as ToggleButton;
            expander.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
