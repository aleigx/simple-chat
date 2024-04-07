using System.Net.Sockets;

namespace SimpleChat.Common.Network
{
    public class MessageReceiver
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public TcpClient TcpClient { get { return _client; } }

        public MessageReceiver(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public async Task<Message> GetAsync()
        {
            var headerLength = Header.GetLength();
            var headerBytes = await ReceiveAsync(headerLength);
            var header = Header.GetHeader(headerBytes);
            var messageBytes = await ReceiveAsync(header.MessageLength);
            return new Message(header, messageBytes);
        }

        private async Task<byte[]> ReceiveAsync(int length)
        {
            byte[] data = new byte[length];
            int totalReceived = 0;
            while (totalReceived < length)
            {
                int received = await _stream.ReadAsync(data, totalReceived, length - totalReceived);
                if (received == 0)
                {
                    throw new SocketException();
                }
                totalReceived += received;
            }
            return data;
        }

    }
}
