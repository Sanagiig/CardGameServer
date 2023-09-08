using AhpilyServer;
using DDZServer.Logic;
using System;
using Protocol.Code;

namespace DDZServer
{
    public class NetMsgCenter : IApplication
    {
        IHandler accountHandler = new AccountHandler();
        IHandler playerHandler = new PlayerHandler();
        IHandler matchHandler = new MatchHandler();
        IHandler fightHandler = new FightHandler();
        public void OnConnected(ClientPeer peer)
        {
        }

        public void OnDisconnected(ClientPeer peer)
        {
            accountHandler.OnDisconnect(peer);
            playerHandler.OnDisconnect(peer);
            matchHandler.OnDisconnect(peer);
            fightHandler.OnDisconnect(peer);
        }

        public void OnRecevedMsg(ClientPeer peer, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    accountHandler.OnReceive(peer, msg);
                    break;
                case OpCode.PLAYER:
                    playerHandler.OnReceive(peer, msg);
                    break;
                case OpCode.MATCH:
                    matchHandler.OnReceive(peer, msg);
                    break;
                case OpCode.FIGHT:
                    fightHandler.OnReceive(peer, msg);
                    break;
                default:
                    throw new Exception("unkonw OpCode");
            }
        }
    }
}
