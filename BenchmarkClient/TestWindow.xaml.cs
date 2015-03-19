using BenchmarkClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BenchmarkClient
{
    /// <summary>
    /// Logique d'interaction pour TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Page
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private Task _testTask;
        public TestWindow(int nbClients, IWorkerConfig clientConfig)
        {
            InitializeComponent();
            var vm = new TestRunViewModel();
            DataContext = vm;
            this.Unloaded += TestWindow_Unloaded;
            _testTask = vm.StartTest(nbClients, 8, clientConfig, _cts.Token);
        }

        void TestWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }


    }
}
