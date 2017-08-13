using System;
using System.Net;
using System.Net.Sockets;
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
            while (true)
            {
                Console.WriteLine("Waiting for client...");

                var clientTask = await listener.AcceptTcpClientAsync();
                //Client has connected
                
            }

        }

    }
}
