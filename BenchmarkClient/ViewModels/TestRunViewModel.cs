using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
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
        private int _currentId;
        public async Task StartTest(int nbClients,int maxNumberClientsPerWorker, IWorkerConfig clientConfigTemplate, CancellationToken token)
        {
            _d = App.Current.Dispatcher;


            var tasks = new List<Task>();
            var i = 0;
          
            while (i < nbClients)
            {

                var numberClients = Math.Max(maxNumberClientsPerWorker, nbClients - i);

                i += numberClients;
                var clientIds = new int[numberClients];
                for (int j = 0; j < numberClients; j++)
                {
                    clientIds[j] = _currentId++;
                }
                var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, System.IO.HandleInheritability.Inheritable);
                dynamic clientConfig = JObject.FromObject(clientConfigTemplate);
                clientConfig.PipeServerHandle = pipeServer.GetClientHandleAsString();
                clientConfig.Clients = new JArray(clientIds);
                var args = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(clientConfig)));
                var psInfos = new ProcessStartInfo("worker/benchmark.worker.exe", args);
                psInfos.CreateNoWindow = true;
                psInfos.UseShellExecute = false;
                psInfos.RedirectStandardOutput = true;

                var prc = Process.Start(psInfos);
                tasks.Add(ConnectToWorker(clientIds, prc, pipeServer, token));
                await Task.Delay(2000);
            }

            await Task.WhenAll(tasks);
        }

        private Task ConnectToWorker(int[] clients, Process prc, AnonymousPipeServerStream pipeServer, CancellationToken token)
        {
            return Task.Run(async () =>
            {

                _d.Invoke(() =>
                {
                    foreach (var cId in clients)
                    {
                        var client = new ClientViewModel(cId);
                        Clients.Add(client);
                    }
                });

                pipeServer.DisposeLocalCopyOfClientHandle();
                try
                {
                    using (var reader = new StreamReader(pipeServer))
                    {
                        while (!token.IsCancellationRequested)
                        {
                            var json = await reader.ReadLineAsync();
                            if (json != null)
                            {
                                var metric = JsonConvert.DeserializeObject<Metric>(json);
                                metric.Date = DateTime.Now;
                                _d.Invoke(() => { AddMetric(metric); });
                            }

                        }
                    }
                }
                finally
                {
                    prc.Kill();
                    _d.Invoke(() =>
                    {
                        foreach (var cId in clients)
                        {
                            var client = Clients.FirstOrDefault(c => c.Id == cId);
                            Clients.Remove(client);
                        }
                    });
                }
            });

        }

        private void AddMetric(Metric metric)
        {
            var client = Clients.FirstOrDefault(c => c.Id == metric.Id);
            client.AddMetric(metric);
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
