using Caliburn.Micro;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CorsairDashboard.Caliburn;
using CorsairDashboard.Settings;
using MahApps.Metro.Controls;

namespace CorsairDashboard.ViewModels
{
    public class SettingsViewModel : FlyoutScreen
    {
        private List<String> accents, themes;
        private String selectedAccent, selectedTheme;
        private UInt16 maxRpmDelta;

        public List<String> Accents
        {
            get { return accents; }
            set
            {
                accents = value;
                NotifyOfPropertyChange(() => Accents);
            }
        }

        public List<String> Themes
        {
            get { return themes; }
            set
            {
                themes = value;
                NotifyOfPropertyChange(() => Themes);
            }
        }

        public String SelectedAccent
        {
            get { return selectedAccent; }
            set
            {
                if (selectedAccent != value)
                {
                    selectedAccent = value;
                    NotifyOfPropertyChange(() => SelectedAccent);
                    UpdateAppAccentAndTheme();
                }
            }
        }

        public String SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    NotifyOfPropertyChange(() => SelectedTheme);
                    UpdateAppAccentAndTheme();
                }
            }
        }

        public UInt16 MaxRpmDelta
        {
            get { return maxRpmDelta; }
            set
            {
                if (maxRpmDelta != value)
                {
                    maxRpmDelta = value;
                    Shell.Settings.MaxRpmDelta = value;
                    NotifyOfPropertyChange(() => MaxRpmDelta);
                }
            }
        }

        public SettingsViewModel(IShell shell)
            : base(shell, "Settings", Position.Right)
        {

            Accents = ThemeManager.Accents.Select(a => a.Name).ToList();
            Themes = ThemeManager.AppThemes.Select(t => t.Name).ToList();
            var settings = Shell.Settings;
            SelectedTheme = settings.ThemeColor;
            SelectedAccent = settings.AccentColor;
            MaxRpmDelta = settings.MaxRpmDelta;
        }

        private void UpdateAppAccentAndTheme()
        {
            if (String.IsNullOrEmpty(SelectedAccent) || String.IsNullOrEmpty(SelectedTheme))
                return;

            Shell.Settings.AccentColor = SelectedAccent;
            Shell.Settings.ThemeColor = SelectedTheme;
            var accent = ThemeManager.GetAccent(SelectedAccent);
            var theme = ThemeManager.GetAppTheme(SelectedTheme);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
        }
    }
}
