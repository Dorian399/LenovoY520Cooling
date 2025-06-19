using System.Configuration;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Input;
using Wpf.Ui.Tray;

namespace LenovoY520Cooling
{
    /// <summary>  
    /// Interaction logic for App.xaml  
    /// </summary>  
    public partial class App : System.Windows.Application
    {
        private static Mutex? _mutex;
        private static NotifyIconService? _notifyIconService;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _mutex = new Mutex(true, "65834ef0-510d-4193-8447-f1067184e9cf", out createdNew);

            if (!createdNew)
            {
                System.Windows.MessageBox.Show("Application is already running.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();

            _notifyIconService = new LocalNotifyIconService();
            _notifyIconService.SetParentWindow(MainWindow);
            BitmapImage notifyIconImage = new(new Uri("pack://application:,,,/icon.ico"));
            _notifyIconService.Icon = notifyIconImage;
            _notifyIconService.ContextMenu = new ContextMenu
            {
                Items =
                    {
                        new MenuItem
                        {
                            Header = "Open",
                            Command = new RelayCommand<object>(_ =>
                            {
                                if (_notifyIconService.ParentWindow is MainWindow mainWindow)
                                {
                                    mainWindow.ShowInCorner();
                                }
                            }),
                        },
                        new MenuItem
                        {
                            Header = "Exit",
                            Command = new RelayCommand<object>(_ => {
                                if (_notifyIconService.ParentWindow is MainWindow mainWindow)
                                {
                                    _notifyIconService.Unregister();
                                    System.Windows.Application.Current.Shutdown();
                                }
                            }),
                        }
                    }
            };

            _notifyIconService.Register();

            base.OnStartup(e);
        }
    }

}
