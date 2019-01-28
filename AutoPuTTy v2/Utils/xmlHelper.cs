using AutoPuTTY.Properties;
using AutoPuTTY.Utils.Datas;
using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace AutoPuTTY.Utils
{
    class xmlHelper
    {
        /// <summary>
        /// Creating new xml file with default content
        /// </summary>
        public static void CreateDefaultConfig()
        {
            const string xmlContent = "<?xml version=\"1.0\"?>\r\n<List>\r\n</List>";
            TextWriter newFileWriter = new StreamWriter(Settings.Default.cfgpath);
            newFileWriter.Write(xmlContent);
            newFileWriter.Close();
        }

        public void configSet(string id, string val)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            XmlElement newpath = xmldoc.CreateElement("Config");
            XmlAttribute name = xmldoc.CreateAttribute("ID");
            name.Value = id;
            newpath.SetAttributeNode(name);
            newpath.InnerText = val;

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@ID=" + parseXpathString(id) + "]");
            if (xmlnode != null)
            {
                if (xmldoc.DocumentElement != null)
                {
                    if (xmlnode.Count > 0)
                    {
                        xmldoc.DocumentElement.ReplaceChild(newpath, xmlnode[0]);
                    }
                    else
                    {
                        xmldoc.DocumentElement.InsertBefore(newpath, xmldoc.DocumentElement.FirstChild);
                    }
                }
            }

            try
            {
                xmldoc.Save(file);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        public string configGet(string id)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.Load(file);
            }
            catch
            {
                otherHelper.Error("\"" + Settings.Default.cfgpath + "\" file is corrupt, delete it and try again.");
                Environment.Exit(-1);
            }

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@ID=" + parseXpathString(id) + "]");
            if (xmlnode != null)
            {
                if (xmlnode.Count > 0) return xmlnode[0].InnerText;
            }
            return "";
        }

        public void dropNode(string node)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@" + node + "]");
            if (xmldoc.DocumentElement != null)
            {
                if (xmlnode != null) xmldoc.DocumentElement.RemoveChild(xmlnode[0]);
            }

            try
            {
                xmldoc.Save(file);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        public string parseXpathString(string input)
        {
            string ret = "";
            if (input.Contains("'"))
            {
                string[] inputstrs = input.Split('\'');
                foreach (string inputstr in inputstrs)
                {
                    if (ret != "") ret += ",\"'\",";
                    ret += "'" + inputstr + "'";
                }
                ret = "concat(" + ret + ")";
            }
            else
            {
                ret = "'" + input + "'";
            }
            return ret;
        }

        /*
         * NEW METHODS
         */

        public ArrayList getAllData()
        {
            if (!File.Exists(Settings.Default.cfgpath))
            {
                return new ArrayList();
            }

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            ArrayList groups = new ArrayList();

            XmlNodeList groupNodes = xmldoc.SelectNodes("//*[@GroupName]");
            if (groupNodes != null)
            {
                if (groupNodes.Count > 0)
                {
                    foreach (XmlElement groupNode in groupNodes)
                    {
                        string groupName = groupNode.GetAttribute("GroupName");

                        string groupDefaulHostname = "";
                        string groupDefaultPort = "";
                        string groupDefaultUsername = "";
                        string groupDefaultPassword = "";

                        ArrayList servers = new ArrayList();

                        foreach (XmlElement childnode in groupNode.ChildNodes)
                        {
                            switch (childnode.Name)
                            {
                                case "DefaultHost":
                                    groupDefaulHostname = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "DefaultPort":
                                    groupDefaultPort = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "DefaultUsername":
                                    groupDefaultUsername = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "DefaultPassword":
                                    groupDefaultPassword = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "Server":
                                    string serverName = childnode.GetAttribute("Name").Trim();

                                    string serverHost = "";
                                    string serverPort = "";
                                    string serverUsername = "";
                                    string serverPassword = "";
                                    string serverType = "";

                                    foreach (XmlElement serverElement in childnode.ChildNodes)
                                    {
                                        switch (serverElement.Name)
                                        {
                                            case "Host":
                                                serverHost = cryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Port":
                                                serverPort = cryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Username":
                                                serverUsername = cryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Password":
                                                serverPassword = cryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Type":
                                                serverType = cryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                        }
                                    }

                                    servers.Add(new ServerElement(serverName, serverHost, serverPort, serverUsername, serverPassword, serverType));
                                    break;
                            }
                        }

                        groups.Add(new GroupElement(groupName, groupDefaulHostname,
                            groupDefaultPort, groupDefaultUsername, groupDefaultPassword, servers));
                    }
                }
                else return new ArrayList();
            }

            return groups;
        }

        // Creating group in XML Config
        public void createGroup(string groupName, string defaultHost, string defaultPort,
            string defaultUsername, string defaultPassword)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlElement newgroup = xmldoc.CreateElement("Group");
            XmlAttribute name = xmldoc.CreateAttribute("GroupName");
            name.Value = groupName;
            newgroup.SetAttributeNode(name);

            if (defaultHost != "")
            {
                XmlElement host = xmldoc.CreateElement("DefaultHost");
                host.InnerText = cryptHelper.Encrypt(defaultHost);
                newgroup.AppendChild(host);
            }

            if (defaultPort != "")
            {
                XmlElement host = xmldoc.CreateElement("DefaultPort");
                host.InnerText = cryptHelper.Encrypt(defaultPort);
                newgroup.AppendChild(host);
            }

            if (defaultUsername != "")
            {
                XmlElement host = xmldoc.CreateElement("DefaultUsername");
                host.InnerText = cryptHelper.Encrypt(defaultUsername);
                newgroup.AppendChild(host);
            }

            if (defaultPassword != "")
            {
                XmlElement host = xmldoc.CreateElement("DefaultPassword");
                host.InnerText = cryptHelper.Encrypt(defaultPassword);
                newgroup.AppendChild(host);
            }

            if (xmldoc.DocumentElement != null) xmldoc.DocumentElement.InsertAfter(newgroup, xmldoc.DocumentElement.LastChild);

            try
            {
                xmldoc.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        // Get ALL groups from configuration
        public ArrayList getGroups()
        {
            if (!File.Exists(Settings.Default.cfgpath))
            {
                return new ArrayList();
            }
            
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            ArrayList groups = new ArrayList();

            XmlNodeList groupNodes = xmldoc.SelectNodes("//*[@GroupName]");
            if (groupNodes != null)
            {
                if (groupNodes.Count > 0)
                {
                    foreach (XmlElement groupNode in groupNodes)
                    {
                        string groupName = groupNode.GetAttribute("GroupName");

                        string groupDefaulHostname = "";
                        string groupDefaultPort = "";
                        string groupDefaultUsername = "";
                        string groupDefaultPassword = "";

                        foreach (XmlElement childnode in groupNode.ChildNodes)
                        {
                            switch (childnode.Name)
                            {
                                case "DefaultHost":
                                    groupDefaulHostname = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "DefaultPort":
                                    groupDefaultPort = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "DefaultUsername":
                                    groupDefaultUsername = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                                case "DefaultPassword":
                                    groupDefaultPassword = cryptHelper.Decrypt(childnode.InnerText);
                                    break;
                            }
                        }

                        groups.Add(new string[] {groupName, groupDefaulHostname, groupDefaultPort, groupDefaultUsername, groupDefaultPassword});
                    }
                }
                else return new ArrayList();
            }

            return groups;
        }

        // Get default group info
        public GroupElement getGroupDefaultInfo(string groupName)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            GroupElement groups;

            string groupDefaultHostname = "";
            string groupDefaultPort = "";
            string groupDefaultUsername = "";
            string groupDefaultPassword = "";

            XmlNodeList groupNodes = xmldoc.SelectNodes("//*[@GroupName='" + groupName + "']");
            if (groupNodes != null)
            {
                if (groupNodes.Count > 0)
                {
                    foreach (XmlElement groupNode in groupNodes[0].ChildNodes)
                    {
                        

                        switch (groupNode.Name)
                        {
                            case "DefaultHost":
                                groupDefaultHostname = cryptHelper.Decrypt(groupNode.InnerText);
                                break;
                            case "DefaultPort":
                                groupDefaultPort = cryptHelper.Decrypt(groupNode.InnerText);
                                break;
                            case "DefaultUsername":
                                groupDefaultUsername = cryptHelper.Decrypt(groupNode.InnerText);
                                break;
                            case "DefaultPassword":
                                groupDefaultPassword = cryptHelper.Decrypt(groupNode.InnerText);
                                break;
                        }

                        
                    }
                }
                else return null;
            }
            groups = new GroupElement(groupName, groupDefaultHostname, groupDefaultPort, groupDefaultUsername, groupDefaultPassword, null);
            return groups;
        }

        /// <summary>
        /// Remove group from xml config
        /// </summary>
        /// <param name="groupName">group name</param>
        public void deleteGroup(string groupName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlNodeList groupNodes = xmlDocument.SelectNodes("//*[@GroupName='" + groupName + "']");

            if (groupNodes != null && groupNodes.Count > 0)
                if (xmlDocument.DocumentElement != null)
                    xmlDocument.DocumentElement.RemoveChild(groupNodes[0]);

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        /// <summary>
        /// Modify group information
        /// </summary>
        /// <param name="groupName">old group name for search</param>
        /// <param name="newGroupName">new group name</param>
        /// <param name="defaultHost">new default host</param>
        /// <param name="defaultPort">new default port</param>
        /// <param name="defaultUsername">new default username</param>
        /// <param name="defaultPassword">new default password</param>
        public void modifyGroup(string groupName, string newGroupName, string defaultHost, string defaultPort,
            string defaultUsername, string defaultPassword)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlElement newGroup = xmlDocument.CreateElement("Group");
            XmlAttribute name = xmlDocument.CreateAttribute("GroupName");
            name.Value = newGroupName;
            newGroup.SetAttributeNode(name);

            if (defaultHost != "")
            {
                XmlElement host = xmlDocument.CreateElement("DefaultHost");
                host.InnerText = cryptHelper.Encrypt(defaultHost);
                newGroup.AppendChild(host);
            }

            if (defaultPort != "")
            {
                XmlElement port = xmlDocument.CreateElement("DefaultPort");
                port.InnerText = cryptHelper.Encrypt(defaultPort);
                newGroup.AppendChild(port);
            }

            if (defaultUsername != "")
            {
                XmlElement username = xmlDocument.CreateElement("DefaultUsername");
                username.InnerText = cryptHelper.Encrypt(defaultUsername);
                newGroup.AppendChild(username);
            }

            if (defaultPassword != "")
            {
                XmlElement password = xmlDocument.CreateElement("DefaultPassword");
                password.InnerText = cryptHelper.Encrypt(defaultPassword);
                newGroup.AppendChild(password);
            }

            XmlNodeList groupsNode = xmlDocument.SelectNodes("//*[@GroupName='" + groupName + "']");
            if (xmlDocument.DocumentElement != null)
            {
                if (groupsNode != null) xmlDocument.DocumentElement.ReplaceChild(newGroup, groupsNode[0]);
            }

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }

        }

        /// <summary>
        /// Add new server to group in xml config
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="serverName">server name</param>
        /// <param name="serverHost">server host</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverUsername">server username</param>
        /// <param name="serverPassword">server password</param>
        /// <param name="serverType">server type</param>
        public void addServer(string groupName, string serverName, string serverHost, string serverPort,
            string serverUsername, string serverPassword, string serverType)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            Console.WriteLine(groupName);
            
            XmlNode xmlGroup = xmldoc.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            XmlElement newServer = xmldoc.CreateElement("Server");
            XmlAttribute name = xmldoc.CreateAttribute("Name");
            name.Value = serverName;
            newServer.SetAttributeNode(name);

            if (serverHost != "")
            {
                XmlElement host = xmldoc.CreateElement("Host");
                host.InnerText = cryptHelper.Encrypt(serverHost);
                newServer.AppendChild(host);
            }

            if (serverPort != "")
            {
                XmlElement host = xmldoc.CreateElement("Port");
                host.InnerText = cryptHelper.Encrypt(serverPort);
                newServer.AppendChild(host);
            }

            if (serverUsername != "")
            {
                XmlElement host = xmldoc.CreateElement("Username");
                host.InnerText = cryptHelper.Encrypt(serverUsername);
                newServer.AppendChild(host);
            }

            if (serverPassword != "")
            {
                XmlElement host = xmldoc.CreateElement("Password");
                host.InnerText = cryptHelper.Encrypt(serverPassword);
                newServer.AppendChild(host);
            }

            if (serverType != "")
            {
                XmlElement host = xmldoc.CreateElement("Type");
                host.InnerText = cryptHelper.Encrypt(serverType);
                newServer.AppendChild(host);
            }

            xmlGroup.AppendChild(newServer);

            try
            {
                xmldoc.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        public static ServerElement getServerByName(string groupName, string serverName)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmldoc.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            foreach (XmlElement xmlElement in xmlGroup)
            {
                switch (xmlElement.Name)
                {
                    case "Server":
                        string foundedServerName = xmlElement.GetAttribute("Name");

                        if (!foundedServerName.Equals(serverName)) continue;

                        string serverHost = "";
                        string serverPort = "";
                        string serverUsername = "";
                        string serverPassword = "";
                        string serverType = "";

                        foreach (XmlElement serverElements in xmlElement.ChildNodes)
                        {
                            switch (serverElements.Name)
                            {
                                case "Host":
                                    serverHost = cryptHelper.Decrypt(serverElements.InnerText);
                                    break;

                                case "Port":
                                    serverPort = cryptHelper.Decrypt(serverElements.InnerText);
                                    break;

                                case "Username":
                                    serverUsername = cryptHelper.Decrypt(serverElements.InnerText);
                                    break;

                                case "Password":
                                    serverPassword = cryptHelper.Decrypt(serverElements.InnerText);
                                    break;

                                case "Type":
                                    serverType = cryptHelper.Decrypt(serverElements.InnerText);
                                    break;
                            }
                        }

                        ServerElement currentServer = new ServerElement(foundedServerName, serverHost, serverPort, serverUsername, serverPassword, serverType);

                        return currentServer;
                }
            }

            //TODO: Fix return null
            return null;
        }

        public void deleteServerByName(string groupName, string serverName)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmldoc.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            foreach (XmlElement xmlElement in xmlGroup)
            {
                switch (xmlElement.Name)
                {
                    case "Server":
                        string foundedServerName = xmlElement.GetAttribute("Name");

                        if (!foundedServerName.Equals(serverName)) continue;

                        xmlGroup.RemoveChild(xmlElement);
                        break;
                }
            }

            try
            {
                xmldoc.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        public void modifyServer(string groupName, string oldServerName, ServerElement serverElement)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmldoc.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            foreach (XmlElement xmlElement in xmlGroup)
            {
                switch (xmlElement.Name)
                {
                    case "Server":
                        string foundedServerName = xmlElement.GetAttribute("Name");

                        if (!foundedServerName.Equals(oldServerName)) continue;

                        xmlElement.Attributes["Name"].Value = serverElement.Name;

                        foreach (XmlElement subElements in xmlElement.ChildNodes)
                        {
                            switch (subElements.Name)
                            {
                                case "Host":
                                    subElements.InnerText = cryptHelper.Encrypt(serverElement.Host);
                                    break;

                                case "Port":
                                    subElements.InnerText = cryptHelper.Encrypt(serverElement.Port);
                                    break;

                                case "Username":
                                    subElements.InnerText = cryptHelper.Encrypt(serverElement.Username);
                                    break;

                                case "Password":
                                    subElements.InnerText = cryptHelper.Encrypt(serverElement.Password);
                                    break;

                                case "Type":
                                    subElements.InnerText = cryptHelper.Encrypt(((int)serverElement.Type).ToString());
                                    break;
                            }
                        }

                        break;
                }

                try
                {
                    xmldoc.Save(Settings.Default.cfgpath);
                }
                catch (UnauthorizedAccessException)
                {
                    otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
                }
            }
        }
    }
}
