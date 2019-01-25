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

        public string Type;

        public string HostWithServer;

        public ServerElement(string name, string host, string port, 
            string username, string password, string type)
        {
            this.Name = name;

            this.Host = host;
            this.Port = port;
            this.Username = username;
            this.Password = password;

            this.Type = type;

            this.HostWithServer = host + ":" + port;
        }

    }
}
