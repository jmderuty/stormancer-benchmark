﻿using System;
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
        public ConfigurationPage(string test)
        {
            InitializeComponent();

            this.DataContext = new ConfigViewModel(test)
            {
                Endpoint = "https://api.stormancer.com",
                AccountId = "d81fc876-6094-3d92-a3d0-86d42d866b96",
                Application = "benchmark-echo",
                SceneId = "test-scene"
            };
        }


        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            var config = (ConfigViewModel)this.DataContext;
            MainWindow.MainFrame.Navigate(new TestWindow(config.ClientCount, config));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
