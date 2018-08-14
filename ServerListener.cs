using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerApp
{
    class ServerListener
    {
        TcpListener listener = null;
        public NetworkStream stream = null;
        public enum Command{
            unrecognized,
            download,
            quit
        }

        public string TCPPort { get; set; }
        
        public ServerListener() {

            listener = new TcpListener(IPAddress.Any, 0); //listen on all interface using a random port
            listener.Start();

            var endPoint = (IPEndPoint)listener.LocalEndpoint;
            TCPPort = endPoint.Port.ToString();

            Console.WriteLine("TCP server started and listening on {0}", TCPPort);
        }

        public async Task StartListening()
        {
            try
            {
                string line = string.Empty;
                while (line != null) //line is null when you press CTRL+Z
                {
                    Console.WriteLine("Waiting for client connection ...");

                    var remoteClient = await listener.AcceptTcpClientAsync();
                    var clientEndPoint = (IPEndPoint)remoteClient.Client.LocalEndPoint;

                    Console.WriteLine("Connection received from {0} : {1}", clientEndPoint.Address.ToString(), clientEndPoint.Port.ToString());

                    //get the IO stream
                    using (stream = remoteClient.GetStream())
                    {
                        //inform connected client of the exit command
                        SendMessage("[enter '.quit' to terminate] >");
                        
                        string message;
                        Command command;
                        CommandExecutor CommandExecutor;

                        do{
                            message = GetMessage().Split('\n')[0].Trim();
                            string receivedCommand = message.Split(' ')[0].Trim();
                            
                            if(Enum.TryParse<Command>(receivedCommand, true, out command)){

                                CommandExecutor = new CommandExecutor(this);
                                CommandExecutor.Execute(command, message);

                            }
                            else if(receivedCommand == ".quit")  command = Command.quit;
                            else SendMessage("unrecognized command: " + receivedCommand);

                        }while(command != Command.quit);

                        //'quit' received, send bye to client
                        byte[] data = Encoding.ASCII.GetBytes(" -- Good Bye --");
                        stream.Write(data, 0, data.Length);
                    }


                    Console.WriteLine("Closed " + clientEndPoint.Address.ToString() + " connection. (press CTRL+Z to exit server)");
                    line = Console.ReadLine();
                    remoteClient.Dispose();
                }

                //CTRL+Z was pressed
                listener.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                listener.Stop();
            }

        }

        private string GetMessage()
        {
            byte[] readBuffer = new byte[1024]; //client messages should not be more than 1KB
            stream.Read(readBuffer, 0, readBuffer.Length);
            string message = Encoding.ASCII.GetString(readBuffer);
            return message.Trim();
        }

        public void SendMessage(String message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
