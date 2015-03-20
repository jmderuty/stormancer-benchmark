using Stormancer;
using Stormancer.Core;
using Stormancer.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Benchmark
{
    public class App : IStartup
    {
        public void Run(IAppBuilder builder)
        {

            builder.SceneTemplate("test-template", scene =>
                {
                    var msgQueue = new ConcurrentQueue<Stream>();
                    var sceneRunning = true;
                    scene.Starting.Add(data =>
                    {

                        var t = Task.Run(async () =>
                        {

                            while (sceneRunning)
                            {
                               
                                scene.Broadcast("broadcast.out", output =>
                                {
                                    Stream s;
                                    while (msgQueue.TryDequeue(out s))
                                    {
                                        s.CopyTo(output);
                                    }
                                   
                                }, PacketPriority.MEDIUM_PRIORITY, PacketReliability.UNRELIABLE);

                                await Task.Delay(30);
                              
                            }

                        });

                        return Task.FromResult(true);
                    });
                    scene.Shuttingdown.Add(ctx =>
                    {
                        sceneRunning = false;
                        return Task.FromResult(true);
                    });
                    scene.AddRoute("broadcast.in", p =>
                    {
                        msgQueue.Enqueue(p.Stream);
                        //scene.Broadcast("broadcast.out", s => p.Stream.CopyTo(s), PacketPriority.MEDIUM_PRIORITY, PacketReliability.UNRELIABLE);
                    });

                    scene.AddRoute("echo.in", p =>
                    {

                        p.Connection.Send("echo.out", s => p.Stream.CopyTo(s), PacketPriority.MEDIUM_PRIORITY, PacketReliability.UNRELIABLE);
                    });
                },
                new Dictionary<string, string> { { "description", "Broadcasts data sent to the route 'broadcast.in' to 'broadcast.out' and echoes data sent to 'echo.in' to 'echo.out' " } }
            );


        }
    }

    public class Msg
    {
        public long SenderId { get; set; }

        public long RequestId { get; set; }

    }
}
