using System.Net.Sockets;
using System.Text;

namespace SimpleChat.Common.Network
{
    public class MessageSender
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public MessageSender(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public TcpClient Client { get { return _client; } }

        public async Task SendAsync(Message message)
        {
            await SendBytesAsync(message.Header.Bytes);
            await SendAsync(message.Text);
        }

        public async Task SendAsync(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            await SendBytesAsync(data);
        }

        public async Task SendBytesAsync(byte[] data)
        {
            await _stream.WriteAsync(data).ConfigureAwait(false);
        }

    }
}
