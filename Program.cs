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