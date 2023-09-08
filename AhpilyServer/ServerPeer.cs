using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public class ServerPeer
    {
        private Socket serverSocket;
        private Semaphore acceptSemaphore;
        private ClientPeerPool clientPool;
        public IApplication App;
        public ServerPeer()
        {

        }

        #region 开启连接服务
        public void Start(int port, int maxCount)
        {
            try
            {
                ClientPeer tmpClient = null;
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSemaphore = new Semaphore(maxCount, maxCount);
                clientPool = new ClientPeerPool(maxCount);

                for (int i = 0; i < maxCount; i++)
                {
                    tmpClient = new ClientPeer();
                    tmpClient.receiveArgs.Completed += completedReceive;
                    tmpClient.CompletedReceive = completedReciveEvent;
                    tmpClient.SendDisconnect = Disconnect;
                    clientPool.Enqueue(tmpClient);
                }

                serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                serverSocket.Listen(10);
                Console.WriteLine("Server starting...");

                startAccept(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void startAccept(SocketAsyncEventArgs se)
        {
            if (se == null)
            {
                se = new SocketAsyncEventArgs();
                se.Completed += completedAccept;
            }

            bool result = serverSocket.AcceptAsync(se);
            if (!result)
            {
                processAccept(se);
            }
        }

        private void processAccept(SocketAsyncEventArgs se)
        {
            acceptSemaphore.WaitOne();

            // 从队列中取出 socketPeer
            ClientPeer client = clientPool.Dequeue();
            client.SetSocket(se.AcceptSocket);
            Console.WriteLine("client [{0}] connected.", client.socket.RemoteEndPoint.ToString());

            startReceive(client);
            se.AcceptSocket = null;
            startAccept(se);
        }

        private void completedAccept(object sender, SocketAsyncEventArgs se)
        {
            // 得到的客户端
            processAccept(se);
        }
        #endregion

        #region 接收数据
        private void startReceive(ClientPeer client)
        {
            try
            {
                bool result = client.socket.ReceiveAsync(client.receiveArgs);
                if (!result)
                {
                    processReceive(client.receiveArgs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void processReceive(SocketAsyncEventArgs se)
        {
            ClientPeer client = se.UserToken as ClientPeer;
            if (client.receiveArgs.SocketError == SocketError.Success && client.receiveArgs.BytesTransferred > 0)
            {
                byte[] packet = new byte[client.receiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.receiveArgs.Buffer, 0, packet, 0, client.receiveArgs.BytesTransferred);
                client.StartReceive(packet);
                startReceive(client);
            }
            else if (client.receiveArgs.BytesTransferred == 0) // 没有传输字节,表示断开连接
            {


                Console.WriteLine("disconnect");
                if (client.receiveArgs.SocketError == SocketError.Success)
                {
                    //主动断开连接
                    Disconnect(client, "客户端主动断开连接");
                }
                else
                {
                    // 客户端异常断开连接
                    Disconnect(client, client.receiveArgs.SocketError.ToString());
                }

            }
        }

        private void completedReceive(object sender, SocketAsyncEventArgs se)
        {
            processReceive(se);
        }

        //一条数据解析完成的处理
        //TODO
        private void completedReciveEvent(ClientPeer client, SocketMsg msg)
        {
            App?.OnRecevedMsg(client, msg);
        }
        #endregion

        #region 断开连接
        public void Disconnect(ClientPeer clientPeer, string reason)
        {
            try
            {
                if (clientPeer == null)
                {
                    throw new Exception("指定的客户端为 null");
                }
                //TODO 通知应用层
                Console.WriteLine("client [{0}] disconnected. because {1}", clientPeer.socket.RemoteEndPoint.ToString(), reason);
                App?.OnDisconnected(clientPeer);

                clientPeer.Disconnect();
                clientPool.Enqueue(clientPeer);
                acceptSemaphore.Release();
                App?.OnDisconnected(clientPeer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 发送数据
        #endregion
    }
}


