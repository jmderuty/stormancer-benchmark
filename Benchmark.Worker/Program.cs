using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormancer;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.IO.Pipes;
namespace Benchmark.Worker
{
    class Program
    {
        /// <summary>
        /// The main console entry point.
        /// </summary>
        /// <param name="args">The commandline arguments.</param>
        private static void Main(string[] args)
        {
           
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
            var cts = new CancellationTokenSource();
            Run(json, cts.Token).Wait();

            //// Wait for the termination event
            //while (!TerminationRequestedEvent.WaitOne(0)) { }
            //cts.Cancel();

            //// Sleep until termination
            //TerminationRequestedEvent.WaitOne();

            //// Print a message which represents the operation
            //Console.WriteLine("Cleanup");

            //// Set this to terminate immediately (if not set, the OS will
            //// eventually kill the process)
            //TerminationCompletedEvent.Set();
        }

        private static async Task Run(string json, CancellationToken token)
        {
            var workerConfig = JsonConvert.DeserializeObject<WorkerConfig>(json);
            var pipeHandle = workerConfig.PipeServerHandle;
            var tasks = new List<Task>();
            
            using (var pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, pipeHandle))
            {
                using (var writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;
                    var stats = new MetricWriter(writer);
                    tasks.Add(stats.Run(token));
                    foreach (var cId in workerConfig.Clients)
                    {
                        var client = new EchoTestClient(cId, workerConfig.Endpoint, json, stats);

                        tasks.Add(client.Run(token));
                    }

                    await Task.WhenAll(tasks);
                }

            }

        }

       

        public class WorkerConfig
        {
            public List<int> Clients { get; set; }

            public string PipeServerHandle { get; set; }

            public string Endpoint { get; set; }
        }
    }


}
