using System;
using System.Diagnostics;

namespace TCPServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to TCP Remote Server");

            if (args.Length < 2) Exit();
            else if (!args[0].Equals("--authtoken") || string.IsNullOrEmpty(args[1])) Exit();

            string ngrok_auth_token = args[1].Trim();

            //kill any ngrok process already running in the background
            foreach (var process in Process.GetProcessesByName("ngrok"))
                process.Kill();

            ServerListener server = new ServerListener();

            Ngrok ngrok = new Ngrok();
            ngrok.AuthToken = ngrok_auth_token;
            ngrok.Start(server.TCPPort); //ngrok has created a tcp tunnel to local interface 

            //retrieve the public channel from ngrok API

            //start the local TCP server
            server.StartListening()
                .Wait(); //listen indefinitely until a .quit message is received
            
          
            Console.Read();
        }

        static void Exit()
        {
            Console.WriteLine("Your provided invalid arguments.");
            Console.WriteLine("Usage: specify your ngrok authtoken using --authtoken ");
            Console.WriteLine("Press any key to exit");
            Console.Read();
            Process.GetCurrentProcess().Kill();
        }
    }
}