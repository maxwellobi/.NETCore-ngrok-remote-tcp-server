using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TCPServerApp
{
    class Ngrok
    {
        string ngrok_download_url;
        string file_name = "ngrok.zip";
        string ngrok_base_api = "http://127.0.0.1:4040/api";

        public Process process;

        public string AuthToken { get; set; }

        public Ngrok()
        {
            //if the ngrok zip is there, just unzip
            if (File.Exists(file_name)) UnZipArchive();

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
                Console.WriteLine("Starting ngrok TCP tunnel");

                ExecuteNgrok("authtoken  " + AuthToken).WaitForExit();
                Task.Run(
                    () => ExecuteNgrok("tcp " + TCPPort)
                    .WaitForExit()  //keep tunnel service alive
                );
            }
        }

        public Process ExecuteNgrok(string arguments){
            return process = RunProcess("./ngrok", arguments);
        }

        public void UnZipArchive(){

            Console.WriteLine("Extracting Executable ... ");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                ZipFile.ExtractToDirectory(file_name, "./");
            else RunProcess("unzip", file_name).WaitForExit();

            File.Delete(file_name);
        }

        private Process RunProcess(string command, string arguments){

            Process process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return process;
        }        

        public async Task<string> PublicUrl()
        {
            Console.WriteLine("Fetching Ngrok Tunnels ... ");
            
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string tunnels = await httpClient.GetStringAsync(ngrok_base_api + "/tunnels");

                TunnelsResponse response = JsonConvert.DeserializeObject<TunnelsResponse>(tunnels);
                var tunnel = response.tunnels[0];

                Console.WriteLine("Retrieved Tunnel: " + tunnel.public_url);
                return tunnel.public_url;
            }
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

                Console.WriteLine("Done With Download");
                UnZipArchive(); //unzip the ngrok archive
            }
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


    class TunnelsResponse
    {
        public List<Tunnel> tunnels { get; set; }
        public string uri { get; set; }
    }

    class Tunnel
    {
        public string name { get; set; }
        public string uri { get; set; }
        public string public_url { get; set; }
        public string proto { get; set; }
    }
}
