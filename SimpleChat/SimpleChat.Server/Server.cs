using SimpleChat.Common.Network;
using SimpleChat.Server.Exceptions;
using System.Net;
using System.Net.Sockets;

namespace SimpleChat.Server
{
    internal class Server
    {
        private TcpListener _tcpListener;
        private bool _run;
        private ClientRepository _clientRepository;

        public Server(string ip, string port)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            _tcpListener = new TcpListener(endpoint);
            _clientRepository = new ClientRepository();
        }

        public async void Start()
        {
            Console.WriteLine("Starting Server");
            _tcpListener.Start(100);
            _run = true;
            Task.Run(async () => await ListenForConnectionsAsync());
            while (_run)
            {
                Console.WriteLine("Enter :q to exit server.");
                var input = Console.ReadLine();
                if (input == ":q")
                {
                    await Shutdown();
                    _run = false;
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
        }

        private async Task ListenForConnectionsAsync()
        {
            while (_run)
            {
                try
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine("New connection");
                    Task.Run(async () => await HandleClientAsync(tcpClient));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _run = false;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            Client client = null;
            try
            {
                client = new Client(tcpClient);
                var connected = true;
                var usernameSet = false;

                while (connected && _run && !usernameSet)
                {
                    var message = await client.ReceiveAsync();

                    if (message.Header.Command == Commands.DISCONNECT)
                    {
                        connected = false;
                        tcpClient.Close();
                        break;
                    }

                    if (message.Header.Command != Commands.SET_USER)
                    {
                        await client.SendAsync(Commands.SET_USER_FAIL, "Server: " + "First, you need to set your username.");
                        continue;
                    }

                    usernameSet = await AddClient(client, message.Text);
                }

                while (connected && _run)
                {
                    var message = await client.ReceiveAsync();

                    if (message.Header.Command == Commands.DISCONNECT)
                    {
                        connected = false;
                        tcpClient.Close();
                        break;
                    }

                    if (message.Header.Command != Commands.MESSAGE)
                    {
                        await client.SendAsync(Commands.ERROR, "Server: " + "Incorrect command sent.");
                        continue;
                    }

                    foreach (var c in _clientRepository.GetAll())
                    {
                        string messageToSend = client.Username + ": " + message.Text;
                        if (c.Username != client.Username) await c.SendAsync(Commands.MESSAGE, messageToSend);
                    }
                }

            }
            catch (Exception)
            {
                tcpClient.Close();
                _clientRepository.Remove(client);
            }
        }

        private async Task<bool> AddClient(Client client, string username)
        {
            client.Username = username;
            try
            {
                _clientRepository.Add(client);
                return true;
            }
            catch (UsernameException ex)
            {
                await client.SendAsync(Commands.SET_USER_FAIL, "Server: " + ex.Message);
                return false;
            }
            catch (Exception)
            {
                await client.SendAsync(Commands.ERROR, "Server: Unknown error, try again later");
                return false;
            }
        }

        private async Task Shutdown()
        {
            _tcpListener.Stop();
            foreach (var client in _clientRepository.GetAll())
            {
                await client.SendAsync(Commands.SERVER_STOPPED, "Server stopped.");
                client.Close();
            }
            Console.WriteLine("Server stopped.");
        }
    }
}
