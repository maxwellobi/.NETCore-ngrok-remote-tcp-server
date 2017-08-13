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
            var arch = RuntimeInformation.OSArchitecture == Architecture.X64 ? "amd64" : (RuntimeInformation.OSArchitecture == Architecture.X86 ? "386" : "");
            if (string.IsNullOrEmpty(ngrok_download_url) && !string.IsNullOrEmpty(arch))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    ngrok_download_url = string.Format("https://bin.equinox.io/c/4VmDzA7iaHb/ngrok-stable-darwin-{0}.zip", arch);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    ngrok_download_url = string.Format("https://bin.equinox.io/c/4VmDzA7iaHb/ngrok-stable-linux-{0}.zip", arch);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ngrok_download_url = string.Format("https://bin.equinox.io/c/4VmDzA7iaHb/ngrok-stable-windows-{0}.zip", arch);
                }
            }

            return ngrok_download_url;
        }
    }
}
