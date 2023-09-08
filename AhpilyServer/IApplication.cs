using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public interface IApplication
    {
        void OnDisconnected(ClientPeer peer);
        void OnRecevedMsg(ClientPeer peer,SocketMsg msg);
        void OnConnected(ClientPeer peer);
    }
}
