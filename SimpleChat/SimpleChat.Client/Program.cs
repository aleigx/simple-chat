using SimpleChat.Client;
using System.Net;

if (args.Length != 2)
{
    Console.WriteLine("Usage: chat <serverHostname> <username>");
    return;
}

string serverHostname = args[0];
string username = args[1];

await Dns.GetHostAddressesAsync(serverHostname); //WITHOUT THIS LINE IT COULDN'T RESOLVE THE IP CORRECTLY ON FIRST CONNECTIONS

Client client = new Client("0.0.0.0", serverHostname, 9898, username);
await client.RunAsync();