using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPuTTY.Utils.Datas
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
            this.Name = name.Trim();

            this.Host = host.Trim();
            this.Port = port.Trim();
            this.Username = username.Trim();
            this.Password = password.Trim();

            this.Type = (ConnectionType) Int32.Parse(type.Trim());

            this.HostWithServer = host.Trim() + ":" + port.Trim();
        }

    }
}
