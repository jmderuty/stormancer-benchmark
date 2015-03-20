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
    /// Logique d'interaction pour TestSelectionPage.xaml
    /// </summary>
    public partial class TestSelectionPage : Page
    {
        public TestSelectionPage()
        {
            InitializeComponent();
        }

        private void broadcastTestBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ConfigurationPage("broadcast"));
        }

        private void requestTestBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ConfigurationPage("echo"));
        }
    }
}
