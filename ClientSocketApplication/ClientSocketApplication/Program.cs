using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientSocketApplication
{
    class Program
    {
        static Socket clientSocket;
        const int MESSAGESIZE = 1024 * 1024;
        static void Main(string[] args)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddr = null;

            try
            {
                Console.WriteLine("Enter the valid IP Address :");
                string strIpAddr = Console.ReadLine();
                if (!IPAddress.TryParse(strIpAddr, out ipAddr))
                {
                    Console.WriteLine("IP Address is not valid");
                    return;
                }
                Console.WriteLine("Enter the valid Port Number (0 - 65535) :");
                string strPortInput = Console.ReadLine().ToString();
                int portNumber = 0;

                if (!int.TryParse(strPortInput.Trim(), out portNumber))
                {
                    Console.WriteLine("Port Number is not valid");
                    return;
                }
                if (portNumber <= 0 && portNumber > 65535)
                {
                    Console.WriteLine("Port Number is not valid, should be between 0 & 65535");
                    return;
                }

                clientSocket.Connect(ipAddr, portNumber);

                Console.WriteLine("Connected to the server...");
                Console.WriteLine("Please enter your name : ");

                String name = Console.ReadLine();

                Byte[] buffName = Encoding.ASCII.GetBytes(name);

                clientSocket.Send(buffName);

                string message = string.Empty;

                Thread receiveThread = new Thread(new ThreadStart(Receive));
                receiveThread.IsBackground = true;
                receiveThread.Start();
                try
                {
                    while (true)
                    {
                        message = Console.ReadLine();
                        Byte[] buffSend = Encoding.ASCII.GetBytes(message);
                        clientSocket.Send(buffSend);
                    }
                }
                catch (Exception e)
                {
                    clientSocket.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
        static void Receive()
        {
            try
            {
                while (true)
                {
                    Byte[] buffReceive = new Byte[MESSAGESIZE];
                    int nRecv = clientSocket.Receive(buffReceive);
                    string data = Encoding.ASCII.GetString(buffReceive, 0, nRecv);
                    string clientName = data.Split('^')[1];
                    Console.WriteLine($"{clientName} : {data.Split('^')[0]}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to close");
            }
        }

    }
}
