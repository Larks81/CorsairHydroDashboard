﻿using Caliburn.Micro;
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
    public class MainMenuViewModel : ScreenWithShell
    {
        [ImportingConstructor]
        public MainMenuViewModel(IShell shell)
            : base(shell)
        {

        }

        public void Leds()
        {
            Shell.ChangeCurrentDisplayedViewModelTo(IoC.Get<LedViewModel>());
        }

        public void Fans()
        {
            Shell.ChangeCurrentDisplayedViewModelTo(IoC.Get<FansViewModel>());
        }
    }
}
