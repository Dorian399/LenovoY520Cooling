using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Diagnostics;

namespace LenovoY520Cooling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public MainWindow()
        {
            InitializeComponent();

            if (AppConfig.Sections["Configs"] == null)
            {
                AppConfig.Sections.Add("Configs", new Configs());
            }

            var ConfigsSection = AppConfig.GetSection("Configs");

            this.DataContext = ConfigsSection;
        }

        private void UpdateSettings(object sender, RoutedEventArgs e)
        {
            var configs = AppConfig.GetSection("Configs") as Configs;

            if (configs == null)
            {
                return;
            }

            if (configs.minTemp > configs.maxTemp) {
                System.Windows.MessageBox.Show("Turn off temp cannot be higher than turn on temp.","Invalid Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
                configs.minTemp = Math.Clamp(configs.maxTemp - 1, 30, 100);
                MinTempSlider.Value = configs.minTemp;
            }

            if (configs.startWithWindows)
            {
                App.ScheduleStartUpTask();
            }
            else
            {
                App.CancelStartUpTask();
            }

            AppConfig.Save();
            App.UpdateConfigs();

        }

        public virtual void ShowInCorner()
        {
            var workingArea = SystemParameters.WorkArea;
            this.Left = workingArea.Right - this.ActualWidth*1.1;
            this.Top = workingArea.Bottom - this.ActualHeight*1.65;

            this.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}