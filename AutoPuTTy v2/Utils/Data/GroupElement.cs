using System.Collections;

namespace AutoPuTTY.Utils.Data
{
    class GroupElement
    {
        public string groupName;

        public string defaultHost;
        public string defaultPort;
        public string defaultUsername;
        public string defaultPassword;

        public ArrayList servers;

        public GroupElement(string groupName, string defaultHost, string defaultPort, string defaultUsername, string defaultPassword, ArrayList servers)
        {
            this.groupName = groupName;

            this.defaultHost = defaultHost;
            this.defaultPort = defaultPort;
            this.defaultUsername = defaultUsername;
            this.defaultPassword = defaultPassword;

            this.servers = servers;
        }
    }
}
