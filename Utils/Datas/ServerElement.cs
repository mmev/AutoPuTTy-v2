using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPuTTY.Utils.Datas
{
    class ServerElement
    {
        public string serverName;

        public string serverHost;
        public string serverPort;
        public string serverUsername;
        public string serverPassword;

        public string serverType;

        public ServerElement(string serverName, string serverHost, string serverPort, 
            string serverUsername, string serverPassword, string serverType)
        {
            this.serverName = serverName;

            this.serverHost = serverHost;
            this.serverPort = serverPort;
            this.serverUsername = serverUsername;
            this.serverPassword = serverPassword;

            this.serverType = serverType;
        }

    }
}
