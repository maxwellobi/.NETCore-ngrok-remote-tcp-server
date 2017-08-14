## .NET Core TCP server with ngrok
This illustrates a TCP Server listening on all network interfaces on a random port on your local machine and using [ngrok](https://ngrok.com/) to expose it to the internet. 

The concept here can be used to create and communicate with services in an entreprise settings for small tasks like log management etc.

The [Ngrok.cs](https://github.com/maxwellobi/.NETCore-ngrok-remote-tcp-server/blob/master/Ngrok.cs) class downloads and extract the ngrok executable depending on your OS.

## To run
Make sure you have [.NET Core](https://www.microsoft.com/net/download/core) installed. You must have an [ngrok authtoken](https://ngrok.com/docs#authtoken) first.

```
> git clone https://github.com/maxwellobi/.NETCore-ngrok-remote-tcp-server.git TCPRemoteServer
> cd TCPRemoteServer
> dotnet restore
> dotnet run --authtoken "abcdefghijklmnopqrstuvwxyz"
```

If run successfully you will see the ngrok tunnel public url which can then be used by any TCP Client to connect

<img src="https://s3.us-east-2.amazonaws.com/maxwellobi.com-bucket/RunTCPServer.png" />

## Connecting to TCP Server
You can connect any TCP Client to this server or build your own. For instance, you can use [netcat](http://www.thegeekstuff.com/2012/04/nc-command-examples/)

<img src="https://s3.us-east-2.amazonaws.com/maxwellobi.com-bucket/ConnectTCPServer.png" />
