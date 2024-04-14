using SimpleChat.Common.Network;
using System.Net;
using System.Net.Sockets;

namespace SimpleChat.Client
{
    internal class Client
    {
        private TcpClient _tcpClient;
        private string _username;
        private bool _run;

        private string _serverHostname;
        private int _serverPort;

        private MessageReceiver _receiver;
        private MessageSender _sender;

        public Client(string clientIp, string serverHostname, int serverPort, string username)
        {
            IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(clientIp), 0);
            _username = username;
            _tcpClient = new TcpClient(clientEndpoint);
            _serverHostname = serverHostname;
            _serverPort = serverPort;
        }

        public async Task RunAsync()
        {
            _run = true;
            try
            {
                await ConnectToServerAsync();
                await SetUsernameAsync();
                Console.WriteLine("You can type :q anytime to close the app.");
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
            await _tcpClient.ConnectAsync(_serverHostname, _serverPort);
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
            Header header = new Header(Commands.SET_USER, _username);
            var userMessage = new Message(header, _username);
            await _sender.SendAsync(userMessage);
            var response = await _receiver.GetAsync();
            if (response.Header.Command != Commands.SET_USER_OK)
            {
                throw new Exception(response.Text);
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
