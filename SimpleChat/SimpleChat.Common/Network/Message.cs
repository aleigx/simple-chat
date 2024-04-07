using System.Text;

namespace SimpleChat.Common.Network
{
    public class Message
    {
        public Header Header { get; private set; }
        
        public string Text
        {
            get => GetText();
            private set => SetText(value);
        }

        public Message(Header header, byte[] text)
        {
            if (header == null)
                throw new ArgumentNullException("Header shouldn't be null.");
            if (text == null)
                throw new ArgumentNullException("Message shouldn't be null");
            if (text.Length != header.MessageLength)
                throw new ArgumentException(
                    "Message length should be equal to the data length specified in the header");
            Header = header;
            _text = text;
        }

        public Message(Header header, string text)
        {
            if (header == null)
                throw new ArgumentNullException("Header shouldn't be null.");
            if (text == null)
                throw new ArgumentNullException("Data shouldn't be null");
            Header = header;
            Text = text;

            if (_text.Length != header.MessageLength)
                throw new ArgumentException(
                    "Message length should be equal to the data length specified in the header");
        }



        private byte[] _text;

        private string GetText()
        {
            return Encoding.UTF8.GetString(_text);
        }

        private void SetText(string text)
        {
            _text = Encoding.UTF8.GetBytes(text);
        }

    }
}
