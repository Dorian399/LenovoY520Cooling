using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32.TaskScheduler;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
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
        private static Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static bool TaskScheduled = false;
        private static bool ExtremeCoolingEnabled = false;

        public static void UpdateConfigs()
        {
            AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }
        private Configs configs
        {
            get
            {
                return AppConfig.GetSection("Configs") as Configs;
            }
        }

        public static void ScheduleStartUpTask()
        {
            if (TaskScheduled)
                return;

            TaskService ts = new TaskService();
            Uri uri = new Uri("pack://application:,,,/task.xml");
            StreamResourceInfo streamInfo = System.Windows.Application.GetResourceStream(uri);
            string xml = "";

            if (streamInfo != null)
            {
                using StreamReader reader = new StreamReader(streamInfo.Stream);
                xml = reader.ReadToEnd();
            }
            else
            {
                return;
            }

            TaskDefinition taskDef = ts.NewTask();
            taskDef.XmlText = xml;

            taskDef.Actions.Clear();
            taskDef.Actions.Add(new ExecAction(System.Environment.ProcessPath));

            ts.RootFolder.RegisterTaskDefinition(
                "LenovoY520Cooling",
                taskDef,
                TaskCreation.CreateOrUpdate,
                null,
                null,
                TaskLogonType.InteractiveToken,
                null
            );
            TaskScheduled = true;
        }

        public static void CancelStartUpTask()
        {
            TaskScheduled = false;

            TaskService ts = new TaskService();
            try
            {
                ts.RootFolder.DeleteTask("LenovoY520Cooling");
            }
            catch (Exception)
            {
                return;
            }
        }

        protected async System.Threading.Tasks.Task StartBackgroundServiceAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await timer.WaitForNextTickAsync())
            {
                int? AVGCPUTemp = CPUInfo.GetAVGTemp(8);
                if(AVGCPUTemp != null)
                {
                    Configs curConf = configs;

                    if (AVGCPUTemp >= curConf.maxTemp && !ExtremeCoolingEnabled)
                    {
                        ExtremeCooling.SetEnabled(true);
                        ExtremeCoolingEnabled = true;
                    }
                    else if (AVGCPUTemp <= curConf.minTemp && ExtremeCoolingEnabled)
                    {
                        ExtremeCooling.SetEnabled(false);
                        ExtremeCoolingEnabled = false;
                    }
                }
            }
        }
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            bool createdNew;
            _mutex = new Mutex(true, "65834ef0-510d-4193-8447-f1067184e9cf", out createdNew);

            if (!createdNew)
            {
                System.Windows.MessageBox.Show("Application is already running.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            if (!ExtremeCooling.Exists())
            {
                System.Windows.MessageBox.Show("Your device is incompatible", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            MainWindow MainWindow = new MainWindow();

            MainWindow.Show();

            if (configs.startMinimized)
            {
                MainWindow.Hide();
            }

            _notifyIconService = new LocalNotifyIconService();
            _notifyIconService.SetParentWindow(MainWindow);
            BitmapImage notifyIconImage = new(new Uri("pack://application:,,,/icon.ico"));
            _notifyIconService.Icon = notifyIconImage;
            _notifyIconService.TooltipText = "LenovoY520Cooling";
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

            CPUInfo.OpenComputer();

            StartBackgroundServiceAsync();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CPUInfo.CloseComputer();
            base.OnExit(e);
        }
    }

}
