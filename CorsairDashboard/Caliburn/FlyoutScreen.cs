using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Caliburn
{
    public abstract class FlyoutScreen : Screen
    {
        private String header;
        private bool isOpen, isPinned;
        private Position position;
        
        public String Header 
        {
            get { return header; }
            set
            {
                if (header != value)
                {
                    header = value;
                    NotifyOfPropertyChange(() => Header);
                }
            }
        }

        public bool IsOpen
        {
            get { return isOpen; }
            set 
            {
                if (isOpen != value)
                {
                    isOpen = value;
                    NotifyOfPropertyChange(() => IsOpen);
                }
            }
        }

        public Position Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    NotifyOfPropertyChange(() => Position);
                }
            }
        }

        public bool IsPinned
        {
            get { return isPinned; }
            set
            {
                if (isPinned != value)
                {
                    isPinned = value;
                    NotifyOfPropertyChange(() => IsPinned);
                }
            }
        }

        protected FlyoutScreen(String header, Position position)
        {
            this.header = header;
            this.position = position;
        }

        public void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }
    }
}
