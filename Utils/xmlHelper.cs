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
        public static void XmlCreate()
        {
            const string xmlcontent = "<?xml version=\"1.0\"?>\r\n<List>\r\n</List>";
            TextWriter newfile = new StreamWriter(Settings.Default.cfgpath);
            newfile.Write(xmlcontent);
            newfile.Close();
        }

        public void XmlConfigSet(string id, string val)
        {
            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            XmlElement newpath = xmldoc.CreateElement("Config");
            XmlAttribute name = xmldoc.CreateAttribute("ID");
            name.Value = id;
            newpath.SetAttributeNode(name);
            newpath.InnerText = val;

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@ID=" + ParseXpathString(id) + "]");
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

        public string XmlConfigGet(string id)
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

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@ID=" + ParseXpathString(id) + "]");
            if (xmlnode != null)
            {
                if (xmlnode.Count > 0) return xmlnode[0].InnerText;
            }
            return "";
        }

        public void XmlDropNode(string node)
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

        public void XmlDropNode(ArrayList node)
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

        internal void XmlToList(ListBox lbList)
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

        public ArrayList XmlGetServer(string name)
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

            XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@Name=" + ParseXpathString(name) + "]");
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

        public string ParseXpathString(string input)
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
    }
}
