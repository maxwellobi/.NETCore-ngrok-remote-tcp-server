using System;

namespace TCPServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ServerListener server = new ServerListener();
            server.Start().Wait();


            Console.Read();
        }
    }
}