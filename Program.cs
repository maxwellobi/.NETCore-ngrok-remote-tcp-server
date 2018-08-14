using System;
using System.Diagnostics;
using System.Threading;

namespace TCPServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to TCP Remote Server");

            if (args.Length < 2) Exit();
            else if (!args[0].ToLower().Equals("--authtoken") || string.IsNullOrEmpty(args[1])) Exit();

            string ngrok_auth_token = args[1].Trim();

            //kill any ngrok process already running in the background
            foreach (var process in Process.GetProcessesByName("ngrok"))
                process.Kill();

            ServerListener server = new ServerListener();

            Ngrok ngrok = new Ngrok();
            ngrok.AuthToken = ngrok_auth_token;
            ngrok.Start(server.TCPPort); //ngrok has created a tcp tunnel to local interface 

            //ngrok process is a blocking thread so its async. Delay for 2 secs to complete propagation
            Thread.Sleep(2000);

            //retrieve the public channel from ngrok API
            ngrok.PublicUrl().Wait();

            //start the local TCP server
            server.StartListening()
                .Wait(); //listen indefinitely until a .quit message is received

            //kill internal ngrok thread
            ngrok.process.Kill();

            //Console.Read();
        }

        static void Exit()
        {
            Console.WriteLine("You provided invalid arguments.");
            Console.WriteLine("Usage: specify your ngrok authtoken using --authtoken ");
            Console.WriteLine("Press any key to exit");
            Console.Read();
            Process.GetCurrentProcess().Kill();
        }
    }
}