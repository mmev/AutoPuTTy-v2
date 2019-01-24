using AutoPuTTY.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AutoPuTTY.Utils
{
    class xmlHelper
    {
        public static void create()
        {
            const string xmlcontent = "<?xml version=\"1.0\"?>\r\n<List>\r\n</List>";
            TextWriter newfile = new StreamWriter(Settings.Default.cfgpath);
            newfile.Write(xmlcontent);
            newfile.Close();
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

        public void dropNode(ArrayList node)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            foreach (string item in node)
            {
                XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@" + item + "]");
                if (xmldoc.DocumentElement != null)
                {
                    if (xmlnode != null) xmldoc.DocumentElement.RemoveChild(xmlnode[0]);
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

        internal void toList(ListBox lbList)
        {
            lbList.Items.Clear();

            if (File.Exists(Settings.Default.cfgpath))
            {
                string file = Settings.Default.cfgpath;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(file);

                XmlNodeList xmlnode = xmldoc.GetElementsByTagName("Server");
                for (int i = 0; i < xmlnode.Count; i++) if (!lbList.Items.Contains(xmlnode[i].Attributes[0].Value)) lbList.Items.Add(xmlnode[i].Attributes[0].Value);
            }
            else
            {
                otherHelper.Error("\"" + Settings.Default.cfgpath + "\" file not found.");
            }
        }

        public ArrayList getServer(string name)
        {
            if (!File.Exists(Settings.Default.cfgpath))
            {
                return new ArrayList();
            }

            ArrayList server = new ArrayList();
            string host = "";
            string user = "";
            string pass = "";
            int type = 0;

            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@Name=" + parseXpathString(name) + "]");
            if (xmlnode != null)
            {
                if (xmlnode.Count > 0)
                {
                    foreach (XmlElement childnode in xmlnode[0].ChildNodes)
                    {
                        switch (childnode.Name)
                        {
                            case "Host":
                                host = childnode.InnerText;
                                break;
                            case "User":
                                user = childnode.InnerText;
                                break;
                            case "Password":
                                pass = childnode.InnerText;
                                break;
                            case "Type":
                                Int32.TryParse(childnode.InnerText, out type);
                                break;
                        }
                    }
                }
                else return new ArrayList();
            }

            server.AddRange(new string[] { name, host, user, pass, type.ToString() });
            return server;
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
        public ArrayList getGroupDefaultInfo(string groupName)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            ArrayList groups = new ArrayList();

            string groupDefaulHostname = "";
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
                                groupDefaulHostname = cryptHelper.Decrypt(groupNode.InnerText);
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
                else return new ArrayList();
            }
            groups.Add(new string[] { groupDefaulHostname, groupDefaultPort, groupDefaultUsername, groupDefaultPassword });
            return groups;
        }

        // Delete group by GroupName
        public void deleteGroup(string groupName)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlNodeList groupNodes = xmldoc.SelectNodes("//*[@GroupName='" + groupName + "']");

            if (groupNodes.Count > 0)
                if (xmldoc != null) xmldoc.DocumentElement.RemoveChild(groupNodes[0]);

            try
            {
                xmldoc.Save(Settings.Default.cfgpath);
            }
            catch (UnauthorizedAccessException)
            {
                otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
            }
        }

        // Modify group data, search by GroupName
        public void modifyGroup(string groupName, string newGroupName, string defaultHost, string defaultPort,
            string defaultUsername, string defaultPassword)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlElement newgroup = xmldoc.CreateElement("Group");
            XmlAttribute name = xmldoc.CreateAttribute("GroupName");
            name.Value = newGroupName;
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

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@GroupName='" + groupName + "']");
            if (xmldoc.DocumentElement != null)
            {
                if (xmlnode != null) xmldoc.DocumentElement.ReplaceChild(newgroup, xmlnode[0]);
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
