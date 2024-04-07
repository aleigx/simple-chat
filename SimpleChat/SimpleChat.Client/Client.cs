using SimpleChat.Common.Network;
using System.Net;
using System.Net.Sockets;

namespace SimpleChat.Client
{
    internal class Client
    {
        private TcpClient _tcpClient;
        private IPEndPoint _serverEndpoint;
        private bool _run;

        private MessageReceiver _receiver;
        private MessageSender _sender;

        public Client(string clientIp, string serverIp, string serverPort)
        {
            IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(clientIp), 0);
            _tcpClient = new TcpClient(clientEndpoint);
            _serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), int.Parse(serverPort));
        }

        public async Task RunAsync()
        {
            _run = true;
            try
            {
                await ConnectToServerAsync();
                await SetUsernameAsync();
                HandleMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Shutdown();
            }

        }

        private async Task ConnectToServerAsync()
        {
            await _tcpClient.ConnectAsync(_serverEndpoint);
            _receiver = new MessageReceiver(_tcpClient);
            _sender = new MessageSender(_tcpClient);
        }

        private void Shutdown()
        {
            if (_tcpClient != null) _tcpClient.Close();
            _run = false;
        }

        private async Task SetUsernameAsync()
        {
            Commands usernameResponse = Commands.SET_USER_FAIL;
            string username = "";
            Console.WriteLine("Enter an username. You can stop the program anytime entering :q");
            while (usernameResponse != Commands.SET_USER_OK && _run)
            {
                Console.Write("Username: ");
                username = Console.ReadLine();
                if (username == ":q")
                {
                    _run = false;
                    break;
                }
                Header header = new Header(Commands.SET_USER, username);
                var userMessage = new Message(header, username);
                await _sender.SendAsync(userMessage);
                var response = await _receiver.GetAsync();
                usernameResponse = response.Header.Command;
                Console.WriteLine(response);
            }
        }

        private async void HandleMessages()
        {
            Task.Run(async () => await ReceiveMessagesAsync());
            await SendMessagesAsync();
        }

        private async Task ReceiveMessagesAsync()
        {
            while (_run)
            {
                Message message = await _receiver.GetAsync();
                if (message.Header.Command == Commands.SERVER_STOPPED)
                {
                    Console.WriteLine(message);
                    _run = false;
                    break;
                }
                if (message.Header.Command == Commands.MESSAGE || message.Header.Command == Commands.ERROR)
                {
                    Console.WriteLine(message);
                }
            }
        }

        private async Task SendMessagesAsync()
        {
            while (_run)
            {
                var text = Console.ReadLine();
                if (text == ":q")
                {
                    _run = false;
                    break;
                }
                Header header = new Header(Commands.MESSAGE, text);
                var userMessage = new Message(header, text);
                await _sender.SendAsync(userMessage);
            }
        }
    }
}
