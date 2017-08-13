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
                while (line != null)
                {
                    Console.WriteLine("Waiting for client connection ...");

                    var remoteClient = await listener.AcceptTcpClientAsync();
                    var clientEndPoint = (IPEndPoint)remoteClient.Client.LocalEndPoint;

                    Console.WriteLine("Connection received from {0} : {1}", clientEndPoint.Address.ToString(), clientEndPoint.Port.ToString());

                    //get the IO stream
                    using (NetworkStream stream = remoteClient.GetStream())
                    {
                        string message = string.Empty;
                        while (!message.StartsWith(".quit")) //end connection if .quit is received 
                        {
                            SendPrompt(stream);
                            message = GetMessage(stream);
                        }

                        //'.quite' received
                        byte[] data = Encoding.ASCII.GetBytes(" -- Good Bye --");
                        stream.Write(data, 0, data.Length);
                    }


                    // '.quit' was sent
                    Console.WriteLine("Closed " + clientEndPoint.Address.ToString() + " connection. (press CTRL+Z to exit server)");
                    line = Console.ReadLine();

                    remoteClient.Dispose();
                }

                //CTRL+Z
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

        private string GetMessage(NetworkStream stream)
        {
            byte[] readBuffer = new byte[1024];
            stream.Read(readBuffer, 0, readBuffer.Length);
            string message = Encoding.ASCII.GetString(readBuffer);
            return message.Trim();
        }

        private void SendPrompt(NetworkStream stream)
        {
            byte[] data = Encoding.ASCII.GetBytes("[enter '.quit' to terminate] >");
            stream.Write(data, 0, data.Length);
        }
    }
}
