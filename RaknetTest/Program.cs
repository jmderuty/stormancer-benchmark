using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaknetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var peer = RakNet.RakPeerInterface.GetInstance();

            var peer2 = RakNet.RakPeerInterface.GetInstance();

            peer.Startup(1, new RakNet.SocketDescriptor(60200, null),1);

            peer2.Startup(1, new RakNet.SocketDescriptor(), 1);

            peer2.Connect("localhost", 60200, null, 0);
            var _ = Task.Run(()=>RunServer(peer));
            while(true)
            {
                _tcs = new TaskCompletionSource<bool>();


            }

        }
        private static TaskCompletionSource<bool> _tcs;
        private static Stopwatch _sw;

        private static Task RunClient(RakNet.RakPeerInterface client)
        {

        }
        private static Task RunServer(RakNet.RakPeerInterface server)
        {
            while(true)
            {
                for (var packet = server.Receive(); packet != null; packet = server.Receive())
                {
                    if(packet.data[0] == 200)
                    {
                        _sw.Stop();
                        _tcs.SetResult(true);
                    }
                }
            }
        }
    }
}
