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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BenchmarkClient
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationPage.xaml
    /// </summary>
    public partial class ConfigurationPage : Page
    {
        public ConfigurationPage()
        {
            InitializeComponent();

            this.DataContext = new ConfigViewModel
            {
                Endpoint = "http://localhost:8081",
                AccountId = "91368576-b314-1fa3-2506-1a9a8811d90d",
                Application = "test",
                SceneId = "test-scene"
            };
        }


        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            var config = (ConfigViewModel)this.DataContext;
            MainWindow.MainFrame.Navigate(new TestWindow(config.ClientCount, config));

        }
    }
}
