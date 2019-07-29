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
        static void Main(string[] args)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddr = null;

            try
            {
                Console.WriteLine("Enter the valid IP Address :");
                string strIpAddr = Console.ReadLine();
                if(!IPAddress.TryParse(strIpAddr, out ipAddr))
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
                if(portNumber <= 0 && portNumber >65535)
                {
                    Console.WriteLine("Port Number is not valid, should be between 0 & 65535");
                    return;
                }

                //Console.WriteLine($"IP Address : {ipAddr.ToString()}, Port No : {portNumber} ");

                clientSocket.Connect(ipAddr, portNumber);

                Console.WriteLine("Connected to the server...");

                Console.WriteLine("Please enter your name : ");

                String name = Console.ReadLine();

                Byte[] buffName = Encoding.ASCII.GetBytes(name);

                clientSocket.Send(buffName);

               
                //Console.WriteLine("Enter the text to send and press enter, type <exit> to close application ");

                string inputCommand=string.Empty;


                Thread receiveThread = new Thread(new ThreadStart(Receive));
                receiveThread.Start();

                while (true)
                {
                   
                    if (inputCommand.Equals("<exit>"))
                        break;
                    //Console.WriteLine("Type message to send to the client, press ';' to terminate the message ");
                    while (true)
                    {
                        
                        inputCommand = Console.ReadLine();
                        Byte[] buffSend = Encoding.ASCII.GetBytes(inputCommand);
                        clientSocket.Send(buffSend);

                        //if (inputCommand[inputCommand.Length - 1].Equals(';'))
                        //{
                        //    break;
                        //}
                    }

   
                    
                   
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //finally
            //{
            //    clientSocket.Shutdown(SocketShutdown.Both);
            //    clientSocket.Close();
            //    clientSocket.Dispose();
            //}

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
        static void Receive()
        {
            while (true)
            {
                Byte[] buffReceive = new Byte[128];
                int nRecv = clientSocket.Receive(buffReceive);
                string data = Encoding.ASCII.GetString(buffReceive, 0, nRecv);
                string clientName = data.Split('^')[1];
                Console.WriteLine($"{clientName} : {data.Split('^')[0]}");
                //if (data[data.Length - 1].Equals(';'))
                //{
                //    break;
                //}
            }
        }

    }
}
