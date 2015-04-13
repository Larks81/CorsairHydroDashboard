using CorsairDashboard.ViewModels;
using HydroLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Caliburn
{
    public interface IShell
    {
        IHydroDevice HydroDevice { get; }

        void ChangeCurrentDisplayedViewModelTo(ChildBaseViewModel newViewModel);
    }
}
