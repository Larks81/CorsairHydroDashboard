using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CorsairDashboard.ViewModels.Controls;

namespace CorsairDashboard.Views.Controls
{
    /// <summary>
    /// Interaction logic for FanRpmView.xaml
    /// </summary>
    public partial class FanRpmView : UserControl
    {
        private FanRpmViewModel fanRpmViewModel;
        private Storyboard fanRotationStoryboard;

        public FanRpmView()
        {
            InitializeComponent();
        }

        private void FanRpmView_OnLoaded(object sender, RoutedEventArgs e)
        {
            fanRpmViewModel = DataContext as FanRpmViewModel;
            if (fanRpmViewModel != null)
            {
                var animation = new DoubleAnimation(0, -360, new Duration(TimeSpan.FromSeconds(1.0f)));
                Storyboard.SetTarget(animation, FanRectangle);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Rectangle.RenderTransform).(RotateTransform.Angle)"));

                fanRotationStoryboard = new Storyboard();
                fanRotationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                fanRotationStoryboard.Children.Add(animation);
                fanRotationStoryboard.Begin(this, true);

                AdjustAnimationSpeed();
                fanRpmViewModel.PropertyChanged += fanRpmViewModel_PropertyChanged;
            }
        }

        private void FanRpmView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (fanRpmViewModel != null)
            {
                fanRpmViewModel.PropertyChanged -= fanRpmViewModel_PropertyChanged;
                fanRpmViewModel = null;                
            }
        }

        void fanRpmViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AnimationSpeed")
            {
                AdjustAnimationSpeed();
            }
        }

        void AdjustAnimationSpeed()
        {
            if (fanRpmViewModel.AnimationSpeed > 0)
            {
                if (fanRotationStoryboard.GetIsPaused(this))
                    fanRotationStoryboard.Resume(this);

                fanRotationStoryboard.SetSpeedRatio(this, fanRpmViewModel.AnimationSpeed);
            }
            else
            {
                fanRotationStoryboard.Pause(this);                
            }  
        }
    }
}
