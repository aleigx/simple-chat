using SimpleChat.Server;

Server server = new Server("127.0.0.1", "9898");
await server.StartAsync();