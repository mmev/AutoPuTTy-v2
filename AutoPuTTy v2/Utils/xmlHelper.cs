using AutoPuTTY.Properties;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using AutoPuTTY.Utils.Data;

namespace AutoPuTTY.Utils
{
    class XmlHelper
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

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        public void configSet(string id, string val)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);

            XmlElement newPath = xmlDocument.CreateElement("Config");
            XmlAttribute name = xmlDocument.CreateAttribute("ID");
            name.Value = id;
            newPath.SetAttributeNode(name);
            newPath.InnerText = val;

            XmlNodeList xmlnode = xmlDocument.SelectNodes("//*[@ID=" + parseXpathString(id) + "]");
            if (xmlnode != null)
            {
                if (xmlDocument.DocumentElement != null)
                {
                    if (xmlnode.Count > 0)
                    {
                        xmlDocument.DocumentElement.ReplaceChild(newPath, xmlnode[0]);
                    }
                    else
                    {
                        xmlDocument.DocumentElement.InsertBefore(newPath, xmlDocument.DocumentElement.FirstChild);
                    }
                }
            }

            try
            {
                xmlDocument.Save(file);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                OtherHelper.Error("\"" + Settings.Default.cfgpath + "\" file is corrupt, delete it and try again.");
                Environment.Exit(-1);
            }

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@ID=" + parseXpathString(id) + "]");
            if (xmlnode != null)
            {
                if (xmlnode.Count > 0) return xmlnode[0].InnerText;
            }
            return "";
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="node"></param>
        public void dropNode(string node)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);

            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//*[@" + node + "]");
            if (xmlDocument.DocumentElement != null)
            {
                if (xmlNodeList != null) xmlDocument.DocumentElement.RemoveChild(xmlNodeList[0]);
            }

            try
            {
                xmlDocument.Save(file);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get all data from xml config
        /// </summary>
        /// <returns>arraylist with groupelement</returns>
        public ArrayList getAllData()
        {
            if (!File.Exists(Settings.Default.cfgpath))
            {
                return new ArrayList();
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            ArrayList groups = new ArrayList();

            XmlNodeList groupNodes = xmlDocument.SelectNodes("//*[@GroupName]");
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

                        foreach (XmlElement childNode in groupNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "DefaultHost":
                                    groupDefaulHostname = CryptHelper.Decrypt(childNode.InnerText);
                                    break;
                                case "DefaultPort":
                                    groupDefaultPort = CryptHelper.Decrypt(childNode.InnerText);
                                    break;
                                case "DefaultUsername":
                                    groupDefaultUsername = CryptHelper.Decrypt(childNode.InnerText);
                                    break;
                                case "DefaultPassword":
                                    groupDefaultPassword = CryptHelper.Decrypt(childNode.InnerText);
                                    break;
                                case "Server":
                                    string serverName = childNode.GetAttribute("Name").Trim();

                                    string serverHost = "";
                                    string serverPort = "";
                                    string serverUsername = "";
                                    string serverPassword = "";
                                    string serverType = "";
                                    bool serverChecks = false;

                                    foreach (XmlElement serverElement in childNode.ChildNodes)
                                    {
                                        switch (serverElement.Name)
                                        {
                                            case "Host":
                                                serverHost = CryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Port":
                                                serverPort = CryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Username":
                                                serverUsername = CryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Password":
                                                serverPassword = CryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Type":
                                                serverType = CryptHelper.Decrypt(serverElement.InnerText);
                                                break;
                                            case "Checks":
                                                serverChecks = Boolean.Parse(serverElement.InnerText);
                                                break;

                                        }
                                    }

                                    servers.Add(new ServerElement(serverName, serverHost, serverPort, serverUsername, serverPassword, serverType, serverChecks));
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

        /// <summary>
        /// Create new group in xml config
        /// </summary>
        /// <param name="groupName">new group name</param>
        /// <param name="defaultHost">default param</param>
        /// <param name="defaultPort">default param</param>
        /// <param name="defaultUsername">default param</param>
        /// <param name="defaultPassword">default param</param>
        public void createGroup(string groupName, string defaultHost, string defaultPort,
            string defaultUsername, string defaultPassword)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlElement newGroup = xmlDocument.CreateElement("Group");
            XmlAttribute name = xmlDocument.CreateAttribute("GroupName");
            name.Value = groupName;
            newGroup.SetAttributeNode(name);

            if (defaultHost != "")
            {
                XmlElement host = xmlDocument.CreateElement("DefaultHost");
                host.InnerText = CryptHelper.Encrypt(defaultHost);
                newGroup.AppendChild(host);
            }

            if (defaultPort != "")
            {
                XmlElement host = xmlDocument.CreateElement("DefaultPort");
                host.InnerText = CryptHelper.Encrypt(defaultPort);
                newGroup.AppendChild(host);
            }

            if (defaultUsername != "")
            {
                XmlElement host = xmlDocument.CreateElement("DefaultUsername");
                host.InnerText = CryptHelper.Encrypt(defaultUsername);
                newGroup.AppendChild(host);
            }

            if (defaultPassword != "")
            {
                XmlElement host = xmlDocument.CreateElement("DefaultPassword");
                host.InnerText = CryptHelper.Encrypt(defaultPassword);
                newGroup.AppendChild(host);
            }

            xmlDocument.DocumentElement?.InsertAfter(newGroup, xmlDocument.DocumentElement.LastChild);

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        /// <summary>
        /// Get group default data by group name
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <returns>group data without servers info</returns>
        public GroupElement getGroupDefaultInfo(string groupName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            string groupDefaultHostname = "";
            string groupDefaultPort = "";
            string groupDefaultUsername = "";
            string groupDefaultPassword = "";

            XmlNodeList groupNodes = xmlDocument.SelectNodes("//*[@GroupName='" + groupName + "']");
            if (groupNodes != null)
            {
                if (groupNodes.Count > 0)
                {
                    foreach (XmlElement groupNode in groupNodes[0].ChildNodes)
                    {
                        switch (groupNode.Name)
                        {
                            case "DefaultHost":
                                groupDefaultHostname = CryptHelper.Decrypt(groupNode.InnerText);
                                break;
                            case "DefaultPort":
                                groupDefaultPort = CryptHelper.Decrypt(groupNode.InnerText);
                                break;
                            case "DefaultUsername":
                                groupDefaultUsername = CryptHelper.Decrypt(groupNode.InnerText);
                                break;
                            case "DefaultPassword":
                                groupDefaultPassword = CryptHelper.Decrypt(groupNode.InnerText);
                                break;
                        }

                        
                    }
                }
                else return null;
            }
            var groupsElement = new GroupElement(groupName, groupDefaultHostname, groupDefaultPort, groupDefaultUsername, groupDefaultPassword, null);
            return groupsElement;
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
                xmlDocument.DocumentElement?.RemoveChild(groupNodes[0]);

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
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

            XmlNodeList groupNodes = xmlDocument.SelectNodes("//*[@GroupName='" + groupName + "']");
            if (groupNodes != null)
            {
                if (groupNodes.Count > 0)
                {
                    XmlNode currentGroup = groupNodes[0];
                    if (currentGroup.Attributes != null) currentGroup.Attributes["GroupName"].Value = newGroupName;

                    foreach (XmlElement groupNode in currentGroup.ChildNodes)
                    {
                        switch (groupNode.Name)
                        {
                            case "DefaultHost":
                                groupNode.InnerText = CryptHelper.Encrypt(defaultHost);
                                break;
                            case "DefaultPort":
                                groupNode.InnerText = CryptHelper.Encrypt(defaultPort);
                                break;
                            case "DefaultUsername":
                                groupNode.InnerText = CryptHelper.Encrypt(defaultUsername);
                                break;
                            case "DefaultPassword":
                                groupNode.InnerText = CryptHelper.Encrypt(defaultPassword);
                                break;
                        }


                    }
                }
            }

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
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
            string serverUsername, string serverPassword, string serverType, bool autoChecks)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            Console.WriteLine(groupName);
            
            XmlNode xmlGroup = xmlDocument.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            XmlElement newServer = xmlDocument.CreateElement("Server");
            XmlAttribute name = xmlDocument.CreateAttribute("Name");
            name.Value = serverName;
            newServer.SetAttributeNode(name);

            if (serverHost != "")
            {
                XmlElement host = xmlDocument.CreateElement("Host");
                host.InnerText = CryptHelper.Encrypt(serverHost);
                newServer.AppendChild(host);
            }

            if (serverPort != "")
            {
                XmlElement host = xmlDocument.CreateElement("Port");
                host.InnerText = CryptHelper.Encrypt(serverPort);
                newServer.AppendChild(host);
            }

            if (serverUsername != "")
            {
                XmlElement host = xmlDocument.CreateElement("Username");
                host.InnerText = CryptHelper.Encrypt(serverUsername);
                newServer.AppendChild(host);
            }

            if (serverPassword != "")
            {
                XmlElement host = xmlDocument.CreateElement("Password");
                host.InnerText = CryptHelper.Encrypt(serverPassword);
                newServer.AppendChild(host);
            }

            if (serverType != "")
            {
                XmlElement host = xmlDocument.CreateElement("Type");
                host.InnerText = CryptHelper.Encrypt(serverType);
                newServer.AppendChild(host);
            }

            XmlElement checks = xmlDocument.CreateElement("Checks");
            checks.InnerText = autoChecks.ToString();
            newServer.AppendChild(checks);

            xmlGroup?.AppendChild(newServer);

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        /// <summary>
        /// Get server data by group and server name
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="serverName">server name</param>
        /// <returns></returns>
        public static ServerElement getServerByName(string groupName, string serverName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmlDocument.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            if (xmlGroup != null)
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
                            bool serverChecks = false;

                            foreach (XmlElement serverElements in xmlElement.ChildNodes)
                            {
                                switch (serverElements.Name)
                                {
                                    case "Host":
                                        serverHost = CryptHelper.Decrypt(serverElements.InnerText);
                                        break;

                                    case "Port":
                                        serverPort = CryptHelper.Decrypt(serverElements.InnerText);
                                        break;

                                    case "Username":
                                        serverUsername = CryptHelper.Decrypt(serverElements.InnerText);
                                        break;

                                    case "Password":
                                        serverPassword = CryptHelper.Decrypt(serverElements.InnerText);
                                        break;

                                    case "Type":
                                        serverType = CryptHelper.Decrypt(serverElements.InnerText);
                                        break;
                                    case "Checks":
                                        serverChecks = Boolean.Parse(serverElements.InnerText);
                                        break;
                                }
                            }

                            ServerElement currentServer = new ServerElement(foundedServerName, serverHost, serverPort,
                                serverUsername, serverPassword, serverType, serverChecks);

                            return currentServer;
                    }
                }

            //TODO: Fix return null
            return null;
        }

        /// <summary>
        /// Remove server from xml config
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="serverName">server name</param>
        public void deleteServerByName(string groupName, string serverName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmlDocument.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            if (xmlGroup != null)
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
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        /// <summary>
        /// modify all server data by group name and old server name
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="oldServerName">old server name</param>
        /// <param name="serverElement">new server data</param>
        public void modifyServer(string groupName, string oldServerName, ServerElement serverElement)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmldoc.SelectSingleNode("//*[@GroupName='" + groupName + "']");

            if (xmlGroup != null)
                foreach (XmlElement xmlElement in xmlGroup)
                {
                    switch (xmlElement.Name)
                    {
                        case "Server":
                            string foundedServerName = xmlElement.GetAttribute("Name");

                            if (!foundedServerName.Equals(oldServerName)) continue;

                            xmlElement.Attributes["Name"].Value = serverElement.Name;

                            bool existHost = false;
                            bool existPort = false;
                            bool existUsername = false;
                            bool existPassword = false;
                            bool existChecks = false;

                            foreach (XmlElement subElements in xmlElement.ChildNodes)
                            {
                                switch (subElements.Name)
                                {
                                    case "Host":
                                        subElements.InnerText = CryptHelper.Encrypt(serverElement.Host);
                                        existHost = true;
                                        break;

                                    case "Port":
                                        subElements.InnerText = CryptHelper.Encrypt(serverElement.Port);
                                        existPort = true;
                                        break;

                                    case "Username":
                                        subElements.InnerText = CryptHelper.Encrypt(serverElement.Username);
                                        existUsername = true;
                                        break;

                                    case "Password":
                                        subElements.InnerText = CryptHelper.Encrypt(serverElement.Password);
                                        existPassword = true;
                                        break;

                                    case "Type":
                                        subElements.InnerText =
                                            CryptHelper.Encrypt(((int) serverElement.Type).ToString());
                                        break;

                                    case "Checks":
                                        subElements.InnerText = serverElement.AutoChecks.ToString();
                                        existChecks = true;
                                        break;
                                }
                            }

                            if (!existHost)
                            {
                                XmlElement newElement = xmldoc.CreateElement("Host");
                                newElement.InnerText = CryptHelper.Encrypt(serverElement.Host);
                                xmlElement.AppendChild(newElement);
                            }

                            if (!existPort)
                            {
                                XmlElement newElement = xmldoc.CreateElement("Port");
                                newElement.InnerText = CryptHelper.Encrypt(serverElement.Port);
                                xmlElement.AppendChild(newElement);
                            }

                            if (!existUsername)
                            {
                                XmlElement newElement = xmldoc.CreateElement("Username");
                                newElement.InnerText = CryptHelper.Encrypt(serverElement.Username);
                                xmlElement.AppendChild(newElement);
                            }

                            if (!existPassword)
                            {
                                XmlElement newElement = xmldoc.CreateElement("Password");
                                newElement.InnerText = CryptHelper.Encrypt(serverElement.Password);
                                xmlElement.AppendChild(newElement);
                            }

                            if (!existChecks)
                            {
                                XmlElement newElement = xmldoc.CreateElement("Checks");
                                newElement.InnerText = serverElement.AutoChecks.ToString();
                                xmlElement.AppendChild(newElement);
                            }

                            break;
                    }

                    try
                    {
                        xmldoc.Save(Settings.Default.cfgpath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        OtherHelper.Error(
                            "Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
                    }
                }
        }

        /// <summary>
        /// check server exist in xml config
        /// </summary>
        /// <param name="groupName">group name</param>
        /// <param name="serverName">server name</param>
        /// <returns></returns>
        public bool serverExist(string groupName, string serverName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmlDocument.SelectSingleNode("//*[@GroupName='" + groupName + "']/*[@Name='"+ serverName + "']");

            if (xmlGroup != null)
                return true;

            return false;
        }

        /// <summary>
        /// get param in server by server, group, paramname
        /// </summary>
        /// <param name="_groupName"></param>
        /// <param name="_serverName"></param>
        /// <param name="_paramName"></param>
        /// <returns></returns>
        public static string getServerParamByServerId(string _groupName, string _serverName, string _paramName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmlDocument.SelectSingleNode("//*[@GroupName='" + _groupName + "']");

            if (xmlGroup != null)
                foreach (XmlElement xmlElement in xmlGroup)
                {
                    switch (xmlElement.Name)
                    {
                        case "Server":
                            string foundedServerName = xmlElement.GetAttribute("Name");

                            if (!foundedServerName.Equals(_serverName)) continue;

                            string currentData = "";

                            foreach (XmlElement serverElements in xmlElement.ChildNodes)
                            {
                                if (serverElements.Name == _paramName)
                                    currentData = serverElements.InnerText;
                            }

                            return currentData;
                    }
                }

            //TODO: Fix return null
            return "";
        }

        public static void setServerParamByServerId(string _groupName, string _serverName, string _paramName,
            string _newData)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Settings.Default.cfgpath);

            XmlNode xmlGroup = xmlDocument.SelectSingleNode("//*[@GroupName='" + _groupName + "']");

            if (xmlGroup != null)
                foreach (XmlElement xmlElement in xmlGroup)
                {
                    switch (xmlElement.Name)
                    {
                        case "Server":
                            string foundedServerName = xmlElement.GetAttribute("Name");

                            if (!foundedServerName.Equals(_serverName)) continue;

                            string currentData = "";
                            bool isCurrentExist = false;

                            foreach (XmlElement serverElements in xmlElement.ChildNodes)
                            {
                                if (serverElements.Name == _paramName)
                                {
                                    serverElements.InnerText = _newData;
                                    isCurrentExist = true;
                                }
                            }

                            if (!isCurrentExist)
                            {
                                XmlElement newElement = xmlDocument.CreateElement(_paramName);
                                newElement.InnerText = _newData;
                                xmlElement.AppendChild(newElement);
                            }

                            break;
                    }
                }

            try
            {
                xmlDocument.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                OtherHelper.Error(
                    "Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }
    }
}
