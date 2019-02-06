using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using AutoPuTTY.Forms;
using AutoPuTTY.Properties;
using AutoPuTTY.Utils.Data;

namespace AutoPuTTY.Utils
{
    class ConnectionHelper
    {
        private static readonly string[] f = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
        private static readonly string[] ps = { "/", "\\\\" };
        private static readonly string[] pr = { "\\", "\\" };

        // for some reason you only have escape \ if it's followed by "
        // will "fix" up to 3 \ in a password like \\\", then screw you with your maniac passwords
        private static readonly string[] passs = { "\"", "\\\\\"", "\\\\\\\\\"", "\\\\\\\\\\\\\"", };
        private static readonly string[] passr = { "\\\"", "\\\\\\\"", "\\\\\\\\\\\"", "\\\\\\\\\\\\\\\"", };

        private static string _currentGroup;
        private static string _currentServer;

        private static string _rdpOutPath;
        private static string _vncOutPath;

        /// <summary>
        /// Start connection method
        /// В зависимости от переданного типа запускает нужный софт
        /// </summary>
        /// <param name="selectedNode">Current selected node from Tree View</param>
        public static void StartConnect(TreeNode selectedNode)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException();

            if (selectedNode?.Parent == null) return;

            _currentGroup = selectedNode.Parent.Text;
            _currentServer = selectedNode.Text;

            ServerElement serverElement = XmlHelper.getServerByName(_currentGroup, _currentServer);
            if (serverElement == null) return;

            switch (serverElement.Type)
            {
                case ConnectionType.Rdp: //RDP
                    LaunchRdp(serverElement);
                    break;
                case ConnectionType.Vnc: //VNC
                    LaunchVnc(serverElement);
                    break;
                case ConnectionType.Scp: //WinSCP (SCP)
                    LaunchWinScp("scp://", serverElement);
                    break;
                case ConnectionType.Sftp: //WinSCP (SFTP)
                    LaunchWinScp("sftp://", serverElement);
                    break;
                case ConnectionType.Ftp: //WinSCP (FTP)
                    LaunchWinScp("ftp://", serverElement);
                    break;
                default: //PuTTY
                    LaunchPuTTy(serverElement);
                    break;
            }
        }

        public static void LaunchTracert(String serverIP)
        {
            string strCmdText = "/C tracert " + serverIP + " &pause";
            Process.Start("CMD.exe", strCmdText);
        }

        public static void LaunchICMPPing(String serverIP)
        {
            string strCmdText = "/C ping " + serverIP + " &pause";
            Process.Start("CMD.exe", strCmdText);
        }

        public static void LaunchNetCat(String serverHost, String serverPort)
        {
            string ncPath = Settings.Default.ncpath;

            if (File.Exists(ncPath))
            {
                string strCmdText = "/C " + ncPath + " -zv " + serverHost + " " + serverPort + " &pause";
                Process.Start("CMD.exe", strCmdText);
            }
            else
            {
                if (MessageBox.Show("Could not find file \"" + ncPath + "\"\nDo you want to change the configuration ?",
                        Resources.connectionHelper_LaunchVnc_Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    formMain.optionsForm.bNCPath_Click();
                }
            }
        }

        /// <summary>
        /// Method for launch default RDP client (mstcs.exe)
        /// </summary>
        /// <param name="serverElement">Server data for launching</param>
        private static void LaunchRdp(ServerElement serverElement)
        {
            string[] rdpExtractFilePath = ExtractFilePath(Settings.Default.rdpath);
            string rdpPath = Environment.ExpandEnvironmentVariables(rdpExtractFilePath[0]);
            string rdpLaunchArgs = rdpExtractFilePath[1];

            if (File.Exists(rdpPath))
            {
                string[] sizes = Settings.Default.rdsize.Split('x');

                _rdpOutPath = "";

                if (Settings.Default.rdfilespath != "" && OtherHelper.ReplaceA(ps, pr, Settings.Default.rdfilespath) != "\\")
                {
                    _rdpOutPath = OtherHelper.ReplaceA(ps, pr, Settings.Default.rdfilespath + "\\");

                    //TODO: add try for exception
                    if (!Directory.Exists(_rdpOutPath))
                        Directory.CreateDirectory(_rdpOutPath);
                }

                
                TextWriter rdpFileWriter = new StreamWriter(path: _rdpOutPath + OtherHelper.ReplaceU(f, serverElement.Name) + ".rdp");

                //TODO: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated

                rdpFileWriter.WriteLine(Settings.Default.rdsize == "Full screen" ? "screen mode id:i:2" : "screen mode id:i:1");
                rdpFileWriter.WriteLine(sizes.Length == 2 ? "desktopwidth:i:" + sizes[0] : "");
                rdpFileWriter.WriteLine(sizes.Length == 2 ? "desktopheight:i:" + sizes[1] : "");
                rdpFileWriter.WriteLine(serverElement.HostWithServer != "" ? "full address:s:" + serverElement.HostWithServer : "");
                rdpFileWriter.WriteLine(serverElement.Username != "" ? "username:s:" + serverElement.Username : "");
                rdpFileWriter.WriteLine(serverElement.Username != "" && serverElement.Password != "" ? "password 51:b:" + CryptHelper.encryptpw(serverElement.Password) : "");
                rdpFileWriter.WriteLine(Settings.Default.rddrives ? "redirectdrives:i:1" : "");
                rdpFileWriter.WriteLine(Settings.Default.rdadmin ? "administrative session:i:1" : "");
                rdpFileWriter.WriteLine(Settings.Default.rdspan ? "use multimon:i:1" : "");

                rdpFileWriter.Close();

                Process myProc = new Process
                {
                    StartInfo =
                    {
                        FileName = rdpPath,
                        Arguments = "\"" + _rdpOutPath + OtherHelper.ReplaceU(f, serverElement.Name) + ".rdp\"" + (rdpLaunchArgs != null ? " " + rdpLaunchArgs : ""),
                    }
                };

                myProc.Start();
            }
            else
            {
                if (MessageBox.Show(Resources.connectionHelper_LaunchVnc_M1 + rdpPath + Resources.connectionHelper_LaunchVnc_M2, Resources.connectionHelper_LaunchVnc_Error,
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)

                {
                    formMain.optionsForm.bRDPath_Click();
                }
            }
        }

        /// <summary>
        /// Method for launch user VNC client like a TinyVNC, UltraVNC or ReadlVNC
        /// </summary>
        /// <param name="serverElement">Current selected server from Tree View</param>
        private static void LaunchVnc(ServerElement serverElement)
        {
            string[] vncExtractPath = ExtractFilePath(Settings.Default.vncpath);
            string vncPath = Environment.ExpandEnvironmentVariables(vncExtractPath[0]);
            string vncArgs = vncExtractPath[1];

            if (File.Exists(vncPath))
            {
                string host = serverElement.Host;
                string port = serverElement.Port != "" ? serverElement.Port : "5900";

                _vncOutPath = "";

                if (Settings.Default.vncfilespath != "" && OtherHelper.ReplaceA(ps, pr, Settings.Default.vncfilespath) != "\\")
                {
                    _vncOutPath = OtherHelper.ReplaceA(ps, pr, Settings.Default.vncfilespath + "\\");

                    if (!Directory.Exists(_vncOutPath))
                        Directory.CreateDirectory(_vncOutPath);
                }

                TextWriter vncFile = new StreamWriter(_vncOutPath + OtherHelper.ReplaceU(f, serverElement.Name) + ".vnc");

                vncFile.WriteLine("[Connection]");
                vncFile.WriteLine(host != "" ? "host=" + host : "");
                vncFile.WriteLine(port != "" ? "port=" + port : "");
                vncFile.WriteLine(serverElement.Username != "" ? "username=" + serverElement.Username : "");
                vncFile.WriteLine(serverElement.Password != "" ? "password=" + CryptHelper.EncryptPassword(serverElement.Password) : "");

                vncFile.WriteLine("[Options]");
                vncFile.WriteLine(Settings.Default.vncfullscreen ? "fullscreen=1" : "");
                vncFile.WriteLine(Settings.Default.vncviewonly ? "viewonly=1" : "");
                vncFile.WriteLine(Settings.Default.vncviewonly ? "sendptrevents=0" : "");
                vncFile.WriteLine(Settings.Default.vncviewonly ? "sendkeyevents=0" : "");
                vncFile.WriteLine(Settings.Default.vncviewonly ? "sendcuttext=0" : "");
                vncFile.WriteLine(Settings.Default.vncviewonly ? "acceptcuttext=0" : "");
                vncFile.WriteLine(Settings.Default.vncviewonly ? "sharefiles=0" : "");

                vncFile.WriteLine(serverElement.Password != "" && serverElement.Password.Length > 8 ? "protocol3.3=1" : ""); // fuckin vnc 4.0 auth

                vncFile.Close();

                Process myProc = new Process
                {
                    StartInfo =
                    {
                        FileName = Settings.Default.vncpath,
                        Arguments = "-config \"" + _vncOutPath + OtherHelper.ReplaceU(f, serverElement.Name) +
                                    ".vnc\"" + (vncArgs != "" ? " " + vncArgs : "")
                    }
                };

                myProc.Start();
            }
            else
            {
                if (MessageBox.Show(Resources.connectionHelper_LaunchVnc_M1 + vncPath + Resources.connectionHelper_LaunchVnc_M2,
                        Resources.connectionHelper_LaunchVnc_Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    formMain.optionsForm.bVNCPath_Click();
                }
            }
        }

        /// <summary>
        /// Method for launch PuTTy
        /// </summary>
        /// <param name="serverElement">Current selected server from Tree View<</param>
        private static void LaunchPuTTy(ServerElement serverElement)
        {
            string[] puttyExtractPath = ExtractFilePath(Settings.Default.puttypath);
            string puttyPath = Environment.ExpandEnvironmentVariables(puttyExtractPath[0]);
            string puttyArgs = puttyExtractPath[1];

            if (File.Exists(puttyPath))
            {
                string host = serverElement.Host;
                string port = serverElement.Port != "" ? serverElement.Port : "22";

                using (Process puttyProcess = new Process())
                {
                    puttyProcess.StartInfo.FileName = Settings.Default.puttypath;
                    puttyProcess.StartInfo.Arguments = "-ssh ";
                    puttyProcess.StartInfo.Arguments += serverElement.Username != "" ? serverElement.Username + "@" : "";

                    puttyProcess.StartInfo.Arguments += host != "" ? host : "";
                    puttyProcess.StartInfo.Arguments += port != "" ? " " + port : "";
                    puttyProcess.StartInfo.Arguments += serverElement.Username != "" && serverElement.Password != "" ? " -pw \"" + OtherHelper.ReplaceA(passs, passr, serverElement.Password) + "\"" : "";
                    puttyProcess.StartInfo.Arguments += Settings.Default.puttyexecute && Settings.Default.puttycommand != "" ? " -m \"" + Settings.Default.puttycommand + "\"" : "";
                    puttyProcess.StartInfo.Arguments += Settings.Default.puttykey && Settings.Default.puttykeyfile != "" ? " -i \"" + Settings.Default.puttykeyfile + "\"" : "";
                    puttyProcess.StartInfo.Arguments += Settings.Default.puttyforward ? " -X" : "";

                    puttyProcess.StartInfo.Arguments += puttyArgs != "" ? " " + puttyArgs + "@" : "";

                    puttyProcess.Start();
                }
            }
            else
            {
                if (MessageBox.Show(Resources.connectionHelper_LaunchVnc_M1 + puttyPath + Resources.connectionHelper_LaunchVnc_M2,
                        Resources.connectionHelper_LaunchVnc_Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    formMain.optionsForm.bPuTTYPath_Click();
                }
            }
        }


        /// <summary>
        /// Method for launch WinSCP: SCP, FTP, SFTP Protocols
        /// </summary>
        /// <param name="protocol"> "scp://" or "ftp://" or "sftp://"</param>
        /// <param name="serverElement">Current selected server from Tree View</param>
        private static void LaunchWinScp(string protocol, ServerElement serverElement)
        {
            string[] winScpExtractPath = ExtractFilePath(Settings.Default.winscppath);
            string winScpPath = Environment.ExpandEnvironmentVariables(winScpExtractPath[0]);
            string winScpArgs = winScpExtractPath[1];

            if (File.Exists(winScpPath))
            {
                string host = serverElement.Host;
                string port = serverElement.Port != "" ? serverElement.Port : "";

                using (Process winScpProcess = new Process())
                {
                    winScpProcess.StartInfo.FileName = Settings.Default.winscppath;
                    winScpProcess.StartInfo.Arguments = protocol;

                    if (serverElement.Username != "")
                    {
                        string[] s = { "%", " ", "+", "/", "@", "\"", ":", ";" };
                        serverElement.Username = OtherHelper.ReplaceU(s, serverElement.Username);
                        serverElement.Password = OtherHelper.ReplaceU(s, serverElement.Password);

                        winScpProcess.StartInfo.Arguments += serverElement.Username;
                        winScpProcess.StartInfo.Arguments += serverElement.Password != "" ? ":" + serverElement.Password : "";
                        winScpProcess.StartInfo.Arguments += "@";
                    }

                    winScpProcess.StartInfo.Arguments += (host != "" ? HttpUtility.UrlEncode(host) : "") ?? throw new InvalidOperationException();
                    winScpProcess.StartInfo.Arguments += port != "" ? ":" + port : "";
                    winScpProcess.StartInfo.Arguments += protocol == "ftp://" ? " /passive=" + (Settings.Default.winscppassive ? "on" : "off") : "";
                    winScpProcess.StartInfo.Arguments += Settings.Default.winscpkey && Settings.Default.winscpkeyfile != "" ? " /privatekey=\"" + Settings.Default.winscpkeyfile + "\"" : "";
                    winScpProcess.StartInfo.Arguments += winScpArgs != "" ? " " + winScpArgs : "";

                    winScpProcess.Start();
                }
            }
            else
            {
                if (MessageBox.Show(Resources.connectionHelper_LaunchVnc_M1 + winScpPath + Resources.connectionHelper_LaunchVnc_M2,
                        Resources.connectionHelper_LaunchVnc_Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    formMain.optionsForm.bWSCPPath_Click();
                }
            }
        }

        //TODO: Refactor func
        /// <summary>
        /// Extract file path and arguments
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns> ??? </returns>
        private static string[] ExtractFilePath(string path)
        {
            //
            if (path.IndexOf("\"", StringComparison.Ordinal) == 0)
            {
                var s = path.Substring(1).IndexOf("\"", StringComparison.Ordinal);
                return s > 0 ? new[] { path.Substring(1, s), path.Substring(s + 2).Trim() } : new[] { path.Substring(1), "" };
            }
            else
            {
                int s = path.Substring(1).IndexOf(" ", StringComparison.Ordinal);
                return s > 0 ? new[] { path.Substring(0, s + 1), path.Substring(s + 2).Trim() } : new[] { path.Substring(0), "" };
            }
        }
    }
}
