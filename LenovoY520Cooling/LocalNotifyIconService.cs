using LenovoY520Cooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Wpf.Ui.Input;
using Wpf.Ui.Tray;

namespace LenovoY520Cooling
{
    public class LocalNotifyIconService : NotifyIconService
    {
        protected override void OnLeftClick()
        {
            if (ParentWindow is MainWindow mainWindow)
            {
                mainWindow.ShowInCorner();
            }
        }
    }
}

