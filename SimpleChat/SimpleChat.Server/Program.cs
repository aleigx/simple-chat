using SimpleChat.Server;
Server server = new Server("0.0.0.0", 9898);
await server.StartAsync();