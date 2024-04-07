using SimpleChat.Common.Exceptions;
using System.Text;

namespace SimpleChat.Common.Network
{
    public class Header
    {
        private const int CommandLength = 2;
        private const int Length = 4;
        private const int MaxPacketSize = 32768;

        public static int GetLength()
        {
            return Header.Length + Header.CommandLength;
        }

        public static Header GetHeader(byte[] data)
        {
            Header header;
            try
            {
                var command = (Commands)int.Parse(Encoding.UTF8.GetString(data, 0, CommandLength));
                var messageLength = int.Parse(Encoding.UTF8.GetString(data, CommandLength, Length));
                header = new Header(command, messageLength);
            }
            catch
            {
                throw new HeaderException("The byte array contains invalid data.");
            }

            return header;
        }

        private byte[] _command;
        private byte[] _messageLength;

        public Header(Commands command, int messageLength)
        {
            Command = command;
            MessageLength = messageLength;
        }

        public Header(Commands command, string message)
        {
            Command = command;
            byte[] data = Encoding.UTF8.GetBytes(message);
            MessageLength = data.Length;
        }


        public byte[] Bytes => GetBytes();

        public Commands Command
        {
            get => (Commands)int.Parse(Encoding.UTF8.GetString(_command));
            set => SetCommand(value);
        }

        public int MessageLength
        {
            get => int.Parse(Encoding.UTF8.GetString(_messageLength));
            set => SetMessageLength(value);
        }

        private void SetCommand(Commands command)
        {
            var c = ((int)command).ToString("D2");
            if (c.Length > CommandLength)
                throw new HeaderException("Invalid command length. Maximum is " + CommandLength);
            _command = Encoding.UTF8.GetBytes(c);
        }

        private void SetMessageLength(int messageLength)
        {
            string length = messageLength.ToString("D4");
            if (length.Length > Length)
                throw new HeaderException("Message length should be smaller than " + MessageLength);
            _messageLength = Encoding.UTF8.GetBytes(length);
        }

        private byte[] GetBytes()
        {
            var request = new byte[GetLength()];
            Array.Copy(_command, 0, request, 0, CommandLength);
            Array.Copy(_messageLength, 0, request, CommandLength, Length);
            return request;
        }
    }
}