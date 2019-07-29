using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerSocketApplication
{
    class Program
    {
        static Socket serverSocket;
        static Dictionary<Socket, string> clientSocketNameMapper = new Dictionary<Socket, string>(); 
        static string serverName;
        static void Main(string[] args)
        {
            
            string hostName = Dns.GetHostName();

            IPAddress iPAddress = Dns.GetHostEntry(hostName).AddressList[1];
           
            int portNumber = 23000;

            Console.WriteLine("IP Address : " + iPAddress.ToString());
            Console.WriteLine("Port Number : " + portNumber);

           

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipep = new IPEndPoint(iPAddress, portNumber);

            serverSocket.Bind(ipep);

            //Console.WriteLine($"IP Address : {iPAddress.ToString()}, Port No : {portNumber} ");

            serverSocket.Listen(50);

            Console.Write("Enter the name : ");
            serverName = Console.ReadLine();

            Console.WriteLine("Ready to receive connections..");

            for (int i = 0; i < 50; i++)
            {
                Thread newThread = new Thread(new ThreadStart(Listeners));
                newThread.Start();
            }

            Thread sendThread = new Thread(new ThreadStart(Send));
            sendThread.Start();


            //Listeners();



        }
        static void Listeners()
        {
            Socket client = serverSocket.Accept();

            Byte[] buffName = new Byte[128];

            int nBuffSize = client.Receive(buffName);

            String clientName = Encoding.ASCII.GetString(buffName, 0, nBuffSize);

            clientSocketNameMapper[client]= clientName;


            Console.WriteLine($"Client Name:{clientName} Remote End Point : {client.RemoteEndPoint.ToString()} connected");

            //string msgToSend;

            while (true)
            {

                while (true)
                {
                    string data = "";

                    Byte[] buffReceive = new Byte[128];

                    int numByte = client.Receive(buffReceive);

                    data = Encoding.ASCII.GetString(buffReceive, 0, numByte);


                    Console.WriteLine($"{clientName} : {data}");

                    foreach(var clientSocket in clientSocketNameMapper.Keys)
                    {
                        data += $"^{clientName}";
                        if (!clientSocket.Equals(client))
                            clientSocket.Send(Encoding.ASCII.GetBytes(data));
                       
                    }

                    //data += Encoding.ASCII.GetString(buffReceive,
                    //                         0, numByte);

                    //if (data[data.Length - 1].Equals(';'))
                    //    break;
                }
                //Console.WriteLine("Message has received from client, Now you can send message, place ';' to terminate the message");

                ////int nRecv = client.Receive(buffReceive);

                ////Console.WriteLine($"Data Received from Client : {Encoding.ASCII.GetString(buffReceive, 0, nRecv)}");
                ////Console.WriteLine($"Data Received from Client : {data}");
                ////Console.WriteLine($"Enter text to send to client :");
                //while (true)
                //{
                //    msgToSend = Console.ReadLine();

                //    Byte[] buffSend = Encoding.ASCII.GetBytes(msgToSend);
                //    client.Send(buffSend);
                //    if (msgToSend[msgToSend.Length - 1].Equals(';'))
                //        break;
                //}


            }

        }
        static void Send()
        {
            while(true)
            {

                string msgToSend = Console.ReadLine()+"^"+serverName;
                Byte[] buffSend = Encoding.ASCII.GetBytes(msgToSend);

                foreach (var clientSocket in clientSocketNameMapper.Keys)
                {
                    //msgToSend += $"^{clientName}";
                    clientSocket.Send(Encoding.ASCII.GetBytes(msgToSend));

                }
               

            }
        }
        //static string GetPublicIPAddress()
        //{
        //    String address = "";
        //    WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
        //    using (WebResponse response = request.GetResponse())
        //    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
        //    {
        //        address = stream.ReadToEnd();
        //    }

        //    int first = address.IndexOf("Address: ") + 9;
        //    int last = address.LastIndexOf("</body>");
        //    address = address.Substring(first, last - first);

        //    return address;
        //}
    }
}
;