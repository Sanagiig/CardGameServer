using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public class ClientPeer
    {
        public int accountID = -1;
        public string account;
        public Socket socket;
        public SocketAsyncEventArgs receiveArgs;
        public SocketAsyncEventArgs sendArgs;

        public delegate void CompletedReceiveDele(ClientPeer client, SocketMsg msg);
        public delegate void SendDisconnectDele(ClientPeer client, string reason);

        public CompletedReceiveDele CompletedReceive;
        public SendDisconnectDele SendDisconnect;

        private List<byte> dataCache = new List<byte>();
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private bool isRecvProcess = false;
        private bool isSendProcess = false;
        public ClientPeer()
        {
            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.UserToken = this;
            receiveArgs.SetBuffer(new byte[1024],0, 1024);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += completedSend;
        }

        public void SetSocket(Socket s)
        {
            socket = s;
        }

        #region Receive
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            Console.WriteLine("start receive");
            if (!isRecvProcess)
                processReceive();
        }

        private void processReceive()
        {
            isRecvProcess = true;
            byte[] data = PackageTool.DecodeMessagePacket(ref dataCache);
            if (data == null)
            {
                isRecvProcess = false;
                return;
            }

            SocketMsg msg = PackageTool.DecodeSocketMsg(data);
            CompletedReceive?.Invoke(this, msg);
            processReceive();
        }
        #endregion

        #region Send

        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            Send(msg);
        }

        public void Send(SocketMsg msg)
        {
            byte[] data = PackageTool.EncodeSocketMsg(msg);
            byte[] packet = PackageTool.EncodeMessagePacket(data);

            Send(packet);
        }

        public void Send(byte[] packet)
        {
            sendQueue.Enqueue(packet);
            if (!isSendProcess)
            {
                send() ;
            }
        }

        private void send()
        {
            isSendProcess = true;
            try
            {
                if (sendQueue.Count == 0)
                {
                    isSendProcess = false;
                    return;
                }
                byte[] packet = sendQueue.Dequeue();
                sendArgs.SetBuffer(packet, 0, packet.Length);
                bool result = socket.SendAsync(sendArgs);
                if (!result)
                {
                    processSend();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void processSend()
        {
            //发送的有没有错误
            if (sendArgs.SocketError != SocketError.Success)
            {
                //发送出错了 客户端断开连接了
                SendDisconnect(this, sendArgs.SocketError.ToString());
            }
            else
            {
                send();
            }
        }

        // 异步发送完成
        private void completedSend(object sender, SocketAsyncEventArgs se)
        {
            processSend();
        }

        #endregion

        public void Disconnect()
        {
            try
            {
                Console.WriteLine("socket disconnect");
                dataCache.Clear();
                isRecvProcess = false;
                sendQueue.Clear();

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
                account = null;
                accountID = -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
