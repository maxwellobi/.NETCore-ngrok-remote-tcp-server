using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace TCPServerApp{

    class CommandExecutor{

        ServerListener serverListener;

        public CommandExecutor(ServerListener serverListener){
            this.serverListener = serverListener;
        }

        public async void Execute(ServerListener.Command command, string entireMessage){

            if(command == ServerListener.Command.download){
                if(entireMessage.Split(' ').Length != 2){
                    serverListener.SendMessage("download command expects 1 parameters");
                    return;
                }

                string path = entireMessage.Split(' ')[1];
                await Task.Run( () => UploadFileToClient(path) );
            }

        }
        
        public void UploadFileToClient(string path){
            if(File.Exists(path)){
                byte[] data = File.ReadAllBytes(path);
                serverListener.stream.Write(data, 0, data.Length);
            }
            else serverListener.SendMessage("could not find file");
        }
        

    }
}