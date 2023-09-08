using AhpilyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDZServer
{
    internal class Program
    {
        private static Socket serverSocket;
        static void Main(string[] args)
        {
            ServerPeer peer = new ServerPeer();
            peer.App = new NetMsgCenter();
            peer.Start(9999,100);
            while (true) { }
        }
    }
}
