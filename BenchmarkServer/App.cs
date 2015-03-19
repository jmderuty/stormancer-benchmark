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

                        scene.Broadcast("echo.out", s => p.Stream.CopyTo(s), PacketPriority.MEDIUM_PRIORITY, PacketReliability.UNRELIABLE);
                    });
                },
                new Dictionary<string, string> { { "description", "Broadcasts data sent to the route 'echo.in' to all connected users on the route 'echo.out'." } }
            );

            //builder.SceneTemplate("requests-test", scene =>
            //{
            //    scene.AddProcedure("test", async ctx => { 
                
                    
            //    });
            //},
            //    new Dictionary<string, string> { { "description", "Test for the RPC plugin." } }
            //);
        }
    }

    public class Msg
    {
        public long SenderId { get; set; }

        public long RequestId { get; set; }

    }
}
