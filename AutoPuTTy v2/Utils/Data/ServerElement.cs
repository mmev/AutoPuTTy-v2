using System;

namespace AutoPuTTY.Utils.Data
{
    class ServerElement
    {
        public string Name;

        public string Host;
        public string Port;
        public string Username;
        public string Password;

        public ConnectionType Type;

        public string HostWithServer;

        public ServerElement(string name, string host, string port, 
            string username, string password, string type)
        {
            Name = name.Trim();

            Host = host.Trim();
            Port = port.Trim();
            Username = username.Trim();
            Password = password.Trim();

            Type = (ConnectionType) Int32.Parse(type.Trim());

            HostWithServer = host.Trim() + ":" + port.Trim();
        }

    }
}
