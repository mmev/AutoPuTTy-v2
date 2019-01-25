using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows.Forms;
using AutoPuTTY.Properties;
using AutoPuTTY.Utils.Datas;

namespace AutoPuTTY.Utils
{
    class connectionHelper
    {
        private static string[] f = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
        private static string[] ps = { "/", "\\\\" };
        private static string[] pr = { "\\", "\\" };

        private static string currentGroup;
        private static string currentServer;

        public static void startConnect(string type, TreeNode selectedNode)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (selectedNode?.Parent == null) return;

            currentGroup = selectedNode.Parent.Text;
            currentServer = selectedNode.Text;

            ServerElement serverElement = xmlHelper.getServerByName(currentGroup, currentServer);
            if (serverElement == null) return;

            string winscpprot = "sftp://";

            string _host = serverElement.Host + ":" + serverElement.Port;
            string _user = serverElement.Username;
            string _pass = serverElement.Password;
            string _type = type == "-1" ? serverElement.Type : type;

            switch (_type)
            {
                case "1": //RDP
                    launchRDP(serverElement);
                    break;
                case "2": //VNC
                    launchVNC(serverElement);
                    break;
                case "3": //WinSCP (SCP)
                    launchWinSCP("scp://", serverElement);
                    break;
                case "4": //WinSCP (SFTP)
                    launchWinSCP("sftp://", serverElement);
                    break;
                case "5": //WinSCP (FTP)
                    launchWinSCP("ftp://", serverElement);
                    break;
                default: //PuTTY
                    launchPuTTy(serverElement);
                    break;
            }
        }

        private static void launchRDP(ServerElement serverElement)
        {
            string[] rdpextractpath = ExtractFilePath(Settings.Default.rdpath);
            string rdpath = Environment.ExpandEnvironmentVariables(rdpextractpath[0]);
            string rdpargs = rdpextractpath[1];

            if (File.Exists(rdpath))
            {
                Mstscpw mstscpw = new Mstscpw();
                string rdppass = mstscpw.encryptpw(serverElement.Password);

                ArrayList arraylist = new ArrayList();
                string[] size = Settings.Default.rdsize.Split('x');

                string rdpout = "";
                if (Settings.Default.rdfilespath != "" && otherHelper.ReplaceA(ps, pr, Settings.Default.rdfilespath) != "\\")
                {
                    rdpout = otherHelper.ReplaceA(ps, pr, Settings.Default.rdfilespath + "\\");

                    try
                    {
                        Directory.CreateDirectory(rdpout);
                    }
                    catch
                    {
                        MessageBox.Show("Output path for generated \".rdp\" connection files doesn't exist.\nFiles will be generated in the current path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rdpout = "";
                    }
                }

                foreach (string width in size)
                {
                    int num;
                    if (Int32.TryParse(width.Trim(), out num)) arraylist.Add(width.Trim());
                }

                TextWriter rdpfile = new StreamWriter(path: rdpout + otherHelper.ReplaceU(f, serverElement.Name) + ".rdp");
                if (Settings.Default.rdsize == "Full screen") rdpfile.WriteLine("screen mode id:i:2");
                else rdpfile.WriteLine("screen mode id:i:1");
                if (arraylist.Count == 2)
                {
                    rdpfile.WriteLine("desktopwidth:i:" + arraylist[0]);
                    rdpfile.WriteLine("desktopheight:i:" + arraylist[1]);
                }
                if (serverElement.HostWithServer != "") rdpfile.WriteLine("full address:s:" + serverElement.HostWithServer);
                if (serverElement.Username != "")
                {
                    rdpfile.WriteLine("username:s:" + serverElement.Username);
                    if (serverElement.Password != "") rdpfile.WriteLine("password 51:b:" + rdppass);
                }
                if (Settings.Default.rddrives) rdpfile.WriteLine("redirectdrives:i:1");
                if (Settings.Default.rdadmin) rdpfile.WriteLine("administrative session:i:1");
                if (Settings.Default.rdspan) rdpfile.WriteLine("use multimon:i:1");
                rdpfile.Close();

                Process myProc = new Process();
                myProc.StartInfo.FileName = rdpath;
                myProc.StartInfo.Arguments = "\"" + rdpout + otherHelper.ReplaceU(f, serverElement.Name) + ".rdp\"";
                if (rdpargs != "") myProc.StartInfo.Arguments += " " + rdpargs;
                //MessageBox.Show(myProc.StartInfo.FileName + myProc.StartInfo.FileName.IndexOf('"').ToString() + File.Exists(myProc.StartInfo.FileName).ToString());
                try
                {
                    myProc.Start();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //user canceled
                }
            }
            else
            {
                if (MessageBox.Show("Could not find file \"" + rdpath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) formMain.optionsform.bRDPath_Click(serverElement.Type);
            }
        }

        private static void launchVNC(ServerElement serverElement)
        {
            string[] vncextractpath = ExtractFilePath(Settings.Default.vncpath);
            string vncpath = vncextractpath[0];
            string vncargs = vncextractpath[1];

            if (File.Exists(vncpath))
            {
                string host;
                string port;
                string[] hostport = serverElement.HostWithServer.Split(':');
                int split = hostport.Length;

                if (split == 2)
                {
                    host = hostport[0];
                    port = hostport[1];
                }
                else
                {
                    host = serverElement.Host;
                    port = "5900";
                }

                string vncout = "";

                if (Settings.Default.vncfilespath != "" && otherHelper.ReplaceA(ps, pr, Settings.Default.vncfilespath) != "\\")
                {
                    vncout = otherHelper.ReplaceA(ps, pr, Settings.Default.vncfilespath + "\\");

                    try
                    {
                        Directory.CreateDirectory(vncout);
                    }
                    catch
                    {
                        MessageBox.Show("Output path for generated \".vnc\" connection files doesn't exist.\nFiles will be generated in the current path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        vncout = "";
                    }
                }

                TextWriter vncfile = new StreamWriter(vncout + otherHelper.ReplaceU(f, serverElement.Name.ToString()) + ".vnc");
                vncfile.WriteLine("[Connection]");
                if (host != "") vncfile.WriteLine("host=" + host.Trim());
                if (port != "") vncfile.WriteLine("port=" + port.Trim());
                if (serverElement.Username != "") vncfile.WriteLine("username=" + serverElement.Username);
                if (serverElement.Password != "") vncfile.WriteLine("password=" + cryptVNC.EncryptPassword(serverElement.Password));
                vncfile.WriteLine("[Options]");
                if (Settings.Default.vncfullscreen) vncfile.WriteLine("fullscreen=1");
                if (Settings.Default.vncviewonly)
                {
                    vncfile.WriteLine("viewonly=1"); //ultravnc
                    vncfile.WriteLine("sendptrevents=0"); //realvnc
                    vncfile.WriteLine("sendkeyevents=0"); //realvnc
                    vncfile.WriteLine("sendcuttext=0"); //realvnc
                    vncfile.WriteLine("acceptcuttext=0"); //realvnc
                    vncfile.WriteLine("sharefiles=0"); //realvnc
                }

                if (serverElement.Password != "" && serverElement.Password.Length > 8) vncfile.WriteLine("protocol3.3=1"); // fuckin vnc 4.0 auth
                vncfile.Close();

                Process myProc = new Process();
                myProc.StartInfo.FileName = Settings.Default.vncpath;
                myProc.StartInfo.Arguments = "-config \"" + vncout + otherHelper.ReplaceU(f, serverElement.Name.ToString()) + ".vnc\"";
                if (vncargs != "") myProc.StartInfo.Arguments += " " + vncargs;
                try
                {
                    myProc.Start();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //user canceled
                }
            }
            else
            {
                if (MessageBox.Show("Could not find file \"" + vncpath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) formMain.optionsform.bVNCPath_Click(serverElement.Type);
            }
        }

        private static void launchPuTTy(ServerElement serverElement)
        {
            string[] puttyextractpath = ExtractFilePath(Settings.Default.puttypath);
            string puttypath = puttyextractpath[0];
            string puttyargs = puttyextractpath[1];
            // for some reason you only have escape \ if it's followed by "
            // will "fix" up to 3 \ in a password like \\\", then screw you with your maniac passwords
            string[] passs = { "\"", "\\\\\"", "\\\\\\\\\"", "\\\\\\\\\\\\\"", };
            string[] passr = { "\\\"", "\\\\\\\"", "\\\\\\\\\\\"", "\\\\\\\\\\\\\\\"", };

            if (File.Exists(puttypath))
            {
                string host;
                string port;
                string[] hostport = serverElement.HostWithServer.Split(':');
                int split = hostport.Length;

                if (split == 2)
                {
                    host = hostport[0];
                    port = hostport[1];
                }
                else
                {
                    host = serverElement.Host;
                    port = "";
                }

                Process myProc = new Process();
                myProc.StartInfo.FileName = Settings.Default.puttypath;
                myProc.StartInfo.Arguments = "-ssh ";
                if (serverElement.Username != "") myProc.StartInfo.Arguments += serverElement.Username + "@";
                if (host != "") myProc.StartInfo.Arguments += host;
                if (port != "") myProc.StartInfo.Arguments += " " + port;
                if (serverElement.Username != "" && serverElement.Password != "") myProc.StartInfo.Arguments += " -pw \"" + otherHelper.ReplaceA(passs, passr, serverElement.Password) + "\"";
                if (Settings.Default.puttyexecute && Settings.Default.puttycommand != "") myProc.StartInfo.Arguments += " -m \"" + Settings.Default.puttycommand + "\"";
                if (Settings.Default.puttykey && Settings.Default.puttykeyfile != "") myProc.StartInfo.Arguments += " -i \"" + Settings.Default.puttykeyfile + "\"";
                if (Settings.Default.puttyforward) myProc.StartInfo.Arguments += " -X";
                //MessageBox.Show(this, myProc.StartInfo.Arguments);
                if (puttyargs != "") myProc.StartInfo.Arguments += " " + puttyargs;
                try
                {
                    myProc.Start();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //user canceled
                }
            }
            else
            {
                if (MessageBox.Show("Could not find file \"" + puttypath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) formMain.optionsform.bPuTTYPath_Click(serverElement.Type);
            }
        }

        private static void launchWinSCP(string protocol, ServerElement serverElement)
        {
            string[] winscpextractpath = ExtractFilePath(Settings.Default.winscppath);
            string winscppath = winscpextractpath[0];
            string winscpargs = winscpextractpath[1];

            if (File.Exists(winscppath))
            {
                string host;
                string port;
                string[] hostport = serverElement.HostWithServer.Split(':');
                int split = hostport.Length;

                if (split == 2)
                {
                    host = hostport[0];
                    port = hostport[1];
                }
                else
                {
                    host = serverElement.Host;
                    port = "";
                }

                Process myProc = new Process();
                myProc.StartInfo.FileName = Settings.Default.winscppath;
                myProc.StartInfo.Arguments = protocol;
                if (serverElement.Username != "")
                {
                    string[] s = { "%", " ", "+", "/", "@", "\"", ":", ";" };
                    serverElement.Username = otherHelper.ReplaceU(s, serverElement.Username);
                    serverElement.Password = otherHelper.ReplaceU(s, serverElement.Password);
                    myProc.StartInfo.Arguments += serverElement.Username;
                    if (serverElement.Password != "") myProc.StartInfo.Arguments += ":" + serverElement.Password;
                    myProc.StartInfo.Arguments += "@";
                }
                if (host != "") myProc.StartInfo.Arguments += HttpUtility.UrlEncode(host);
                if (port != "") myProc.StartInfo.Arguments += ":" + port;
                if (protocol == "ftp://") myProc.StartInfo.Arguments += " /passive=" + (Settings.Default.winscppassive ? "on" : "off");
                if (Settings.Default.winscpkey && Settings.Default.winscpkeyfile != "") myProc.StartInfo.Arguments += " /privatekey=\"" + Settings.Default.winscpkeyfile + "\"";
                if (winscpargs != "") myProc.StartInfo.Arguments += " " + winscpargs;
                try
                {
                    myProc.Start();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //user canceled
                }
            }
            else
            {
                if (MessageBox.Show("Could not find file \"" + winscppath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) formMain.optionsform.bWSCPPath_Click(serverElement.Type);
            }
        }

        private static string[] ExtractFilePath(string path)
        {
            //extract file path and arguments
            if (path.IndexOf("\"", StringComparison.Ordinal) == 0)
            {
                int s = path.Substring(1).IndexOf("\"", StringComparison.Ordinal);
                if (s > 0) return new string[] { path.Substring(1, s), path.Substring(s + 2).Trim() };
                return new string[] { path.Substring(1), "" };
            }
            else
            {
                int s = path.Substring(1).IndexOf(" ", StringComparison.Ordinal);
                if (s > 0) return new string[] { path.Substring(0, s + 1), path.Substring(s + 2).Trim() };
                return new string[] { path.Substring(0), "" };
            }
        }
    }
}
