using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSocketApplication
{
    class Program
    {
        static Socket serverSocket;
        static Dictionary<Socket, string> clientSocketNameMapper = new Dictionary<Socket, string>();
        static string serverName;
        const int MESSAGESIZE = 1024*1024;
        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName();

            IPAddress iPAddress = Dns.GetHostEntry(hostName).AddressList[1];

            int portNumber = 3000;

            Console.WriteLine("IP Address : " + iPAddress.ToString());
            Console.WriteLine("Port Number : " + portNumber);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipEndPoint = new IPEndPoint(iPAddress, portNumber);

            serverSocket.Bind(ipEndPoint);

            serverSocket.Listen(50);

            Console.Write("Enter the name : ");
            serverName = Console.ReadLine();

            Console.WriteLine("Ready to receive connections..");

            Console.WriteLine("Type message to send, enter <exit> to close :");


            //Thread listenerThread = new Thread(new ThreadStart(Listener));
            //listenerThread.Start();

            //Thread sendThread = new Thread(new ThreadStart(Send));
            //sendThread.Start();
            Task.Factory.StartNew(Listener);

            while (true)
            {
                string message = Console.ReadLine();
                if (message == "<exit>")
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.Read();
                    break;
                }             
                message = message + "^" + serverName;
                Byte[] buffSend = Encoding.ASCII.GetBytes(message);

                foreach (var clientSocket in clientSocketNameMapper.Keys)
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes(message));
                }
            }
        }

        static void Listener()
        {
            Socket client = serverSocket.Accept();
            //Thread receiveThread = new Thread(() => Receiver(client));
            //receiveThread.Start();

            //Thread listenerThread = new Thread(new ThreadStart(Listener));
            //listenerThread.Start();
            Task.Factory.StartNew(() => Receiver(client));
            Task.Factory.StartNew(Listener);
        }
        static void Receiver(Socket client)
        {
            Byte[] buffName = new Byte[MESSAGESIZE];

            try
            {
                int nBuffSize = client.Receive(buffName);

                String clientName = Encoding.ASCII.GetString(buffName, 0, nBuffSize);

                foreach (var clientSocket in clientSocketNameMapper.Keys)
                    clientSocket.Send(Encoding.ASCII.GetBytes($"Connected^{clientName}"));

                clientSocketNameMapper[client] = clientName;

                Console.WriteLine($"Client Name:{clientName} Remote End Point : {client.RemoteEndPoint.ToString()} connected");

                while (true)
                {
                    string message = "";

                    Byte[] buffReceive = new Byte[MESSAGESIZE];

                    int numByte = client.Receive(buffReceive);

                    message = Encoding.ASCII.GetString(buffReceive, 0, numByte);

                    Console.WriteLine($"{clientName} : {message}");

                    foreach (var clientSocket in clientSocketNameMapper.Keys)
                    {
                        message = message + $"^{clientName}";
                        if (!clientSocket.Equals(client))
                            clientSocket.Send(Encoding.ASCII.GetBytes(message));
                    }
                }
            }
            catch
            {
                string clientName = clientSocketNameMapper[client];
                Console.WriteLine($"Client Name:{clientName} Remote End Point : {client.RemoteEndPoint.ToString()} disconnected");
                clientSocketNameMapper.Remove(client);
                client.Close();

                foreach (var clientSocket in clientSocketNameMapper.Keys)
                    clientSocket.Send(Encoding.ASCII.GetBytes($"Disconnected^{clientName}"));
            }
        }
        static void Send()
        {
           
        }
    }
}
