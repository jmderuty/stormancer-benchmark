using Stormancer;
using Stormancer.Core;
using Stormancer.Server;
using System;
using System.Collections.Generic;

namespace Benchmark
{
    public class App : IStartup
    {
        public void Run(IAppBuilder builder)
        {
            builder.SceneTemplate("test-template", scene =>
                {
                    scene.AddRoute("broadcast.in", p =>
                    {

                        scene.Broadcast("broadcast.out", s => p.Stream.CopyTo(s), PacketPriority.MEDIUM_PRIORITY, PacketReliability.UNRELIABLE);
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
