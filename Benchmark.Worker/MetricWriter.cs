using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmark.Worker
{
    class MetricWriter
    {
        private readonly StreamWriter _writer;
        internal MetricWriter(StreamWriter writer)
        {
            _writer = writer;
        }
        public Task Run(CancellationToken token)
        {
            return Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    await ConsumeQueue();
                    await Task.Delay(100);
                }
                
            });
        }
        public void Write(Stats metric)
        {
            _queue.Enqueue(metric);
        }


        private async Task ConsumeQueue()
        {
            Stats s;
            while (_queue.TryDequeue(out s))
            {
                await _writer.WriteLineAsync(JsonConvert.SerializeObject(s));
            }

        }
        private ConcurrentQueue<Stats> _queue = new ConcurrentQueue<Stats>();
    }
}
