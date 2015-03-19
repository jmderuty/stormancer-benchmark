using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stormancer;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
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
            Run(json,cts.Token).Wait();
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

        private static async Task Run(string json,CancellationToken token)
        {
            var client = new EchoTestClient(json);
            client.SceneId = "test-scene";
            await client.Run(token);


        }
    }
 

}
