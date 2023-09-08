using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue;

        public ClientPeerPool(int capacity) { 
            clientPeerQueue = new Queue<ClientPeer>(capacity);
        }

        public void Enqueue(ClientPeer peer)
        {
            clientPeerQueue.Enqueue(peer);
        }

        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }

        public void BroadCast(int[] idArr, int opCode,int subCode,object val)
        {
            BroadCast(idArr, new SocketMsg(opCode, subCode, val));
        }
        public void BroadCast(int[] idArr, SocketMsg msg)
        {
            foreach (ClientPeer client in clientPeerQueue) {
                if(client != null && idArr.Contains(client.accountID)) {
                    client.Send(msg);
                }
            }
        }
    }
}
