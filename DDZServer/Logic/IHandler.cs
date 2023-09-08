using AhpilyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Logic
{
    internal interface IHandler
    {
        void OnReceive(ClientPeer peer, SocketMsg msg);

        void OnDisconnect(ClientPeer peer);
    }
}
