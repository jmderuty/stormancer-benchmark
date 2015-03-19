using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Stormancer;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.IO;

namespace Benchmark.Worker
{
    internal class EchoTestClient
    {
        public class Config
        {
            public int PacketSize { get; set; }

            public int SendPeriod { get; set; }

            public string AccountId { get; set; }
            public string Application { get; set; }

            public string SceneId { get; set; }
        }
        public EchoTestClient(int id, string endpoint, string json, StreamWriter writer)
        {
            var config = JsonConvert.DeserializeObject<Config>(json);
            this.Configuration = config;
            Interval = config.SendPeriod;
            PacketSize = config.PacketSize;
            _writer = writer;
            _endpoint = endpoint;
            Id = id;
        }
        public int Id { get; set; }
        public Config Configuration { get; set; }
        private readonly string _endpoint;
        private StreamWriter _writer;
        public int Interval { get; set; }
        public int PacketSize { get; set; }


        private ConcurrentDictionary<long, Request> _requests = new ConcurrentDictionary<long, Request>();

        private long _curId = 0;

        private List<long> _currAggrLatencies = new List<long>();
        private Request CreateRequest()
        {
            var rq = new Request();
            do
            {

                rq.Id = Interlocked.Increment(ref _curId);
            }
            while (!_requests.TryAdd(rq.Id, rq));
            return rq;
        }
        public async Task Run(CancellationToken token)
        {
            var buffer = new byte[Math.Max(PacketSize - 8, 0)];
            //var cfg = Stormancer.ClientConfiguration.ForAccount("d81fc876-6094-3d92-a3d0-86d42d866b96", "benchmark-echo");
            var cfg = Stormancer.ClientConfiguration.ForAccount(Configuration.AccountId, Configuration.Application);
            cfg.ServerEndpoint = _endpoint;

            var client = new Stormancer.Client(cfg);
            var guid = Guid.NewGuid().ToString();
            var scene = await client.GetPublicScene(Configuration.SceneId, guid);

            scene.AddRoute("echo.out", p =>
            {

                var msg = p.ReadObject<Msg>();
                if (msg.SenderId == client.Id)
                {
                    Request request;
                    if (_requests.TryRemove(msg.RequestId, out request))
                    {
                        request.Sw.Stop();
                        _currAggrLatencies.Add(request.Sw.ElapsedMilliseconds);
                    }
                }
            });
            await scene.Connect();
            var lastStatsUpdate = DateTime.UtcNow;
            while (!token.IsCancellationRequested)
            {
                var request = CreateRequest();
                request.SentOn = DateTime.UtcNow;
                var serializer = scene.Host.Serializer();
                var msg = new Msg { SenderId = client.Id.Value, RequestId = request.Id };
                request.Sw.Start();
                scene.SendPacket("echo.in", s =>
                {
                    serializer.Serialize(msg, s);
                    s.Write(buffer, 0, buffer.Length);
                }, Stormancer.Core.PacketPriority.MEDIUM_PRIORITY, Stormancer.Core.PacketReliability.UNRELIABLE);
                await Task.Delay(Interval);

                if (DateTime.UtcNow - lastStatsUpdate > TimeSpan.FromSeconds(1))
                {
                    lastStatsUpdate = DateTime.UtcNow;
                    var stats = new Stats { Min = long.MaxValue, Max = long.MinValue, Id = Id };
                    foreach (var v in _currAggrLatencies)
                    {
                        if (stats.Min > v)
                        {
                            stats.Min = v;
                        }
                        if (stats.Max < v)
                        {
                            stats.Max = v;
                        }
                        stats.Avg += ((double)v);
                    }
                    stats.NbSamples = _currAggrLatencies.Count;
                    stats.Avg /= stats.NbSamples;
                    stats.Pending = _requests.Count;
                    _currAggrLatencies.Clear();
                    _writer.WriteLine(JsonConvert.SerializeObject(stats));
                }

            }
        }
    }

    public class Stats
    {
        public int Id { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }
        public double Avg { get; set; }

        public int NbSamples { get; set; }

        public int Pending { get; set; }
    }
    public class Msg
    {
        public long SenderId { get; set; }

        public long RequestId { get; set; }

    }
}
