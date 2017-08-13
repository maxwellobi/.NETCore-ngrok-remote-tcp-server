using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TCPServerApp
{
    class Ngrok
    {
        string ngrok_download_url;
        string file_name = "ngrok.zip";
        

        public string AuthToken { get; set; }

        public Process Compiler { get; set; }

        public Ngrok()
        {
            //if the zip is there, just unzip
            if (File.Exists(file_name)) UnzipFile(file_name);

            //If ngrok executable is not in root folder, download and unzip
            if (!File.Exists("ngrok") && !File.Exists("ngrok.exe"))
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

        public void Start(string TCPPort)
        {
            if (string.IsNullOrEmpty(AuthToken)) throw new System.ArgumentNullException("Invalid ngrok authentication token");
            else
            {
                ExecuteNgrok("authtoken  " + AuthToken)
                    .Kill(); //kill the process that creates the auth token - process might internally exit before kill

                ExecuteNgrok("tcp " + TCPPort)
                    .WaitForExit();  //keep tunnel service alive
            }
        }

        public Process ExecuteNgrok(string arguments)
        {
            Compiler = new Process();
            Compiler.StartInfo.FileName = "ngrok";
            Compiler.StartInfo.Arguments = arguments;
            Compiler.StartInfo.UseShellExecute = false;
            Compiler.StartInfo.RedirectStandardOutput = true;
            Compiler.StartInfo.CreateNoWindow = true;
            Compiler.Start();

            var result = Compiler.StandardOutput.ReadToEnd();
            return Compiler;
        }

        private async Task DownloadNgrok()
        {
            Console.WriteLine("Downloading Ngrok ... ");
            
            using (HttpClient httpClient = new HttpClient())
            {
                var stream = await httpClient.GetStreamAsync(ngrok_download_url);
                using (var fileStream = File.Create(file_name))
                {
                    await stream.CopyToAsync(fileStream);
                }

                UnzipFile(file_name);
            }
        }

        private void UnzipFile(string path)
        {
            Console.WriteLine("Extracting Executable ... ");

            ZipFile.ExtractToDirectory(path, "./");
            File.Delete(path);

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
