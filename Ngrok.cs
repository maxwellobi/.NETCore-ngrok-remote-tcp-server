using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TCPServerApp
{
    class Ngrok
    {
        string ngrok_download_url;

        public Ngrok()
        {
            //If ngrok is not in root folder, download and unzip
            if (!File.Exists("ngrok") || !File.Exists("ngrok.exe"))
            {
                if (string.IsNullOrEmpty(NgrokURL()))
                {
                    Console.WriteLine("@@ Invalid System Architecture Detected - Cannot Run Program");
                    Console.WriteLine("@@ Press any key to exit: ");
                    Console.Read();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }

                else DownloadNgrok().Wait();
            }
        }

        private async Task DownloadNgrok()
        {
            Console.WriteLine("Downloading Ngrok now ... ");

           
        }

        private string NgrokURL()
        {
            return ngrok_download_url;
        }
    }
}
