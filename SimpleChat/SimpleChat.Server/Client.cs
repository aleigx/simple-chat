using SimpleChat.Common.Network;
using System.Net.Sockets;

namespace SimpleChat.Server
{
    internal class Client
    {
        public string Username { get; set; }
        private MessageReceiver _receiver;
        private MessageSender _sender;
        private TcpClient _tcpClient;

        public Client(TcpClient tcpClient)
        {
            _receiver = new MessageReceiver(tcpClient);
            _sender = new MessageSender(tcpClient);
        }

        public async Task SendAsync(Commands command, string text)
        {
            var header = new Header(command, text);
            var message = new Message(header, text);
            await _sender.SendAsync(message);
        }

        public async Task<Message> ReceiveAsync()
        {
            return await _receiver.GetAsync();
        }

        public void Close()
        {
            _tcpClient.Close();
        }

    }
}
