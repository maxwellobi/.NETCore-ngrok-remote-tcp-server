using System;
using System.Text;
using System.Net.Sockets;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace TCPServerApp
{
    class ServerListener
    {
        TcpListener listener;

        public ServerListener() {

            
            
        }

        //start listening on local
        //connect to remote server
        //send ip and port to remote server

        public async Task Start()
        {
            listener = new TcpListener(IPAddress.Any, 0); //listen on all interface using a random port
            listener.Start();

            var endPoint = (IPEndPoint)listener.LocalEndpoint;
            Console.WriteLine("TCP server started and listening on {0}:{1}", endPoint.Address.ToString(), endPoint.Port.ToString());

            
            while (true)
            {
                Console.WriteLine("Waiting for client...");

                var clientTask = await listener.AcceptTcpClientAsync();
                //Client has connected
                
            }

        }

    }
}
