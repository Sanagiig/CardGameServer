using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        private static Socket clientSocket;

        static void Main(string[] args)
        {
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(serverEP);
            Thread clientThread = new Thread(reveiveServerMsg);
            clientThread.Start();

            while (true) { }
        }

        static private void reveiveServerMsg()
        {
            byte[] buffer = new byte[1024];
            clientSocket.Receive(buffer);
            Console.WriteLine("server: " + Encoding.Default.GetString(buffer));

            clientSocket.Send(Encoding.Default.GetBytes("hello Im client 01"));
        }
    }
}
