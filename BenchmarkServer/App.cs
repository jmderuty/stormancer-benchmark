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
                scene.AddRoute("echo.in", p =>
                {
                    var msg = p.ReadObject<Msg>();
                    //msg.SenderId = 0;
                    scene.Broadcast("echo.out", s => p.Serializer().Serialize(msg,s), PacketPriority.MEDIUM_PRIORITY, PacketReliability.UNRELIABLE);
                });
            },
            new Dictionary<string, string> { { "description", "Broadcasts data sent to the route 'echo.in' to all connected users on the route 'echo.out'." } });
        }
    }

    public class Msg
    {
        public long SenderId { get; set; }

        public long RequestId { get; set; }

    }
}
