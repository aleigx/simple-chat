using SimpleChat.Server.Exceptions;

namespace SimpleChat.Server
{
    internal class ClientRepository
    {
        private List<Client> _clients;

        public ClientRepository()
        {
            _clients = new List<Client>();
        }

        public bool Exists(string username)
        {
            return _clients.Any(x => x.Username == username);
        }

        public void Add(Client client)
        {
            if (client == null) throw new ArgumentNullException("Client can't be null");
            if (client.Username == null || client.Username.Length == 0) throw new UsernameException("Username can't be empty.");
            if (Exists(client.Username)) throw new UsernameException("The username is already in use.");
            _clients.Add(client);
        }

        public void Remove(Client? client)
        {
            if (client != null) _clients.RemoveAll(x => x.Username == client.Username);
        }

        public List<Client> GetAll()
        {
            return _clients;
        }
    }
}
