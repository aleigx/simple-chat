using SimpleChat.Client;

Client client = new Client("127.0.0.1", "127.0.0.1", "9898");
await client.RunAsync();