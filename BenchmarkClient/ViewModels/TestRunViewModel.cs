using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BenchmarkClient.ViewModels
{
    class TestRunViewModel
    {
        private Dispatcher _d;
        public async Task StartTest(int nbClients, object clientConfig, CancellationToken token)
        {
            _d = App.Current.Dispatcher;
            var json = JsonConvert.SerializeObject(clientConfig);
            var args = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            var tasks = new List<Task>();
            for (int i = 0; i < nbClients; i++)
            {
                var psInfos = new ProcessStartInfo("worker/benchmark.worker.exe", args);
                psInfos.CreateNoWindow = true;
                psInfos.UseShellExecute = false;
                psInfos.RedirectStandardOutput = true;
                var prc = Process.Start(psInfos);
                tasks.Add(ConnectToWorker(prc, token));
                await Task.Delay(2000);
            }

            await Task.WhenAll(tasks);
        }

        private Task ConnectToWorker(Process prc, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var client = new ClientViewModel();
                _d.Invoke(() => { 
                    Clients.Add(client); 
                });
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        var json = await prc.StandardOutput.ReadLineAsync();
                        var metric = JsonConvert.DeserializeObject<Metric>(json);
                        metric.Date = DateTime.Now;
                        _d.Invoke(() => { client.AddMetric(metric); });

                    }
                }
                finally
                {
                    prc.Kill();
                    _d.Invoke(() => { Clients.Remove(client); });
                }
            });

        }
        private readonly ObservableCollection<ClientViewModel> _clients = new ObservableCollection<ClientViewModel>();
        public ObservableCollection<ClientViewModel> Clients
        {
            get
            {
                return _clients;
            }
        }
    }
}
