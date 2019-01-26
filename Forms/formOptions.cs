using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AutoPuTTY.Properties;
using AutoPuTTY.Utils;

namespace AutoPuTTY
{
    public partial class formOptions : Form
    {
        #region Conts Init

        public formMain mainform;
        public popupImport importpopup;
        public popupRecrypt recryptpopup;
        public bool firstread = true;
        public bool importcancel;
        public bool importempty;
        public object locker = new object();
        public string importreplace = "";

        #endregion

        #region Element Loading

        public formOptions(formMain form)
        {
            mainform = form;
            InitializeComponent();
            importpopup = new popupImport(this);
            recryptpopup = new popupRecrypt(this);

            Settings.Default.ocryptkey = Settings.Default.cryptkey;

            if (File.Exists(Settings.Default.cfgpath))
            {
                tbPuTTYPath.Text = Settings.Default.puttypath;
                cbPuTTYExecute.Checked = Settings.Default.puttyexecute;
                tbPuTTYExecute.Text = Settings.Default.puttycommand;
                cbPuTTYKey.Checked = Settings.Default.puttykey;
                tbPuTTYKey.Text = Settings.Default.puttykeyfile;
                cbPuTTYForward.Checked = Settings.Default.puttyforward;

                tbRDPath.Text = Settings.Default.rdpath;
                tbRDKeep.Text = Settings.Default.rdfilespath;
                cbRDAdmin.Checked = Settings.Default.rdadmin;
                cbRDDrives.Checked = Settings.Default.rddrives;
                cbRDSpan.Checked = Settings.Default.rdspan;
                cbRDSize.Text = Settings.Default.rdsize;

                tbVNCPath.Text = Settings.Default.vncpath;
                tbVNCKeep.Text = Settings.Default.vncfilespath;
                cbVNCFullscreen.Checked = Settings.Default.vncfullscreen;
                cbVNCViewonly.Checked = Settings.Default.vncviewonly;

                tbWSCPPath.Text = Settings.Default.winscppath;
                cbWSCPKey.Checked = Settings.Default.winscpkey;
                tbWSCPKey.Text = Settings.Default.winscpkeyfile;
                cbWSCPPassive.Checked = Settings.Default.winscppassive;

                cbGMinimize.Checked = Settings.Default.minimize;
                if (Settings.Default.password.Trim() != "")
                {
                    Settings.Default.password = cryptHelper.Decrypt(Settings.Default.password, Settings.Default.pcryptkey);
                    tbGPassword.Text = Settings.Default.password;
                    tbGConfirm.Text = Settings.Default.password;
                    Settings.Default.cryptkey = Settings.Default.password;
                    cbGPassword.Checked = true;
                }
            }

            bGPassword.Enabled = false;
            firstread = false;
        }

        #endregion

        #region Button Events

        public void bPuTTYPath_Click(string type)
        {
            bPuTTYPath_Click(new object(), new EventArgs());
            mainform.connect(type);
        }

        public void bPuTTYPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select PuTTY executable";
            browseFile.Filter = "EXE File (*.exe)|*.exe";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbPuTTYPath.Text = browseFile.FileName;
            }
            else return;
        }

        public void bRDPath_Click(string type)
        {
            bRDPath_Click(new object(), new EventArgs());
            mainform.connect(type);
        }

        public void bRDPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select Remote Desktop executable";
            browseFile.Filter = "EXE File (*.exe)|*.exe";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbRDPath.Text = browseFile.FileName;
            }
            else return;
        }

        public void bVNCPath_Click(string type)
        {
            bVNCPath_Click(new object(), new EventArgs());
            mainform.connect(type);
        }

        public void bVNCPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select VNC Viewer executable";
            browseFile.Filter = "EXE File (*.exe)|*.exe";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbVNCPath.Text = browseFile.FileName;
            }
            else return;
        }

        private void bVNCKeep_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select .vnc files path";
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tbVNCKeep.Text = folderBrowserDialog.SelectedPath;
            }
            else return;
        }

        public void bWSCPPath_Click(string type)
        {
            bWSCPPath_Click(new object(), new EventArgs());
            mainform.connect(type);
        }

        public void bWSCPPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select WinSCP executable";
            browseFile.Filter = "EXE File (*.exe)|*.exe";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbWSCPPath.Text = browseFile.FileName;
            }
            else return;
        }

        private void bWSCPKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select private key file";
            browseFile.Filter = "PuTTY private key files (*.ppk)|*.ppk|All files (*.*)|*.*";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                tbWSCPKey.Text = browseFile.FileName;
            }
            else return;
        }

        private void bPassword_Click(object sender, EventArgs e)
        {
            if (tbGPassword.Text.Trim() == "")
            {
                MessageBox.Show(this, "Password can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGPassword.Text = "";
                tbGConfirm.Text = "";
            }
            else if (tbGConfirm.Text != tbGPassword.Text)
            {
                MessageBox.Show(this, "Password confirmation doesn't match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGConfirm.Text = "";
            }
            else
            {
                if (Settings.Default.password != tbGPassword.Text)
                {
                    Settings.Default.password = tbGPassword.Text;
                    mainform.XmlHelper.configSet("password", cryptHelper.Encrypt(Settings.Default.password, Settings.Default.pcryptkey));

                    string[] bwArgs = { "recrypt", Settings.Default.password };
                    bwProgress.RunWorkerAsync(bwArgs);
                    recryptpopup = new popupRecrypt(this);
                    recryptpopup.Text = "Applying" + recryptpopup.Text;
                    recryptpopup.ShowDialog(this);

                    Settings.Default.cryptkey = Settings.Default.password;
                }
                bGPassword.Enabled = false;
            }
        }

        private void bGImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select server list";
            browseFile.Filter = "TXT File (*.txt)|*.txt";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                string file = browseFile.FileName;
                if (File.Exists(file))
                {
                    importpopup = new popupImport(this);
                    object[] bwArgs = { "import", file };
                    bwProgress.RunWorkerAsync(bwArgs);
                    importpopup.ShowDialog(this);
                    return;
                }
            }
        }

        private void bPuTTYExecute_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select commands file";
            browseFile.Filter = "TXT File (*.txt)|*.txt";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                tbPuTTYExecute.Text = browseFile.FileName;
            }
            else return;
        }

        private void bPuTTYKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Title = "Select private key file";
            browseFile.Filter = "PuTTY private key files (*.ppk)|*.ppk|All files (*.*)|*.*";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                tbPuTTYKey.Text = browseFile.FileName;
            }
            else return;
        }

        private void bRDKeep_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select .rdp files path";
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tbRDKeep.Text = folderBrowserDialog.SelectedPath;
            }
            else return;
        }


        #endregion

        #region TextBox Events

        private void tbPuTTY_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.puttypath = tbPuTTYPath.Text;
            if (!firstread) mainform.XmlHelper.configSet("putty", Settings.Default.puttypath);
        }

        private void tbPuTTYExecute_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.puttycommand = tbPuTTYExecute.Text;
            if (!firstread) mainform.XmlHelper.configSet("puttycommand", Settings.Default.puttycommand);
        }

        private void tbPuTTYKey_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.puttykeyfile = tbPuTTYKey.Text;
            if (!firstread) mainform.XmlHelper.configSet("puttykeyfile", Settings.Default.puttykeyfile);
        }

        private void tbRD_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.rdpath = tbRDPath.Text;
            if (!firstread) mainform.XmlHelper.configSet("remotedesktop", Settings.Default.rdpath);
        }

        private void tbRDKeep_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.rdfilespath = tbRDKeep.Text;
            if (!firstread) mainform.XmlHelper.configSet("rdfilespath", Settings.Default.rdfilespath);
        }

        private void tbVNCPath_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.vncpath = tbVNCPath.Text;
            if (!firstread) mainform.XmlHelper.configSet("vnc", Settings.Default.vncpath);
        }

        private void tbVNCKeep_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.vncfilespath = tbVNCKeep.Text;
            if (!firstread) mainform.XmlHelper.configSet("vncfilespath", Settings.Default.vncfilespath);
        }

        private void tbWSCPPath_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.winscppath = tbWSCPPath.Text;
            if (!firstread) mainform.XmlHelper.configSet("winscp", Settings.Default.winscppath);
        }

        private void tbWSCPKey_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.winscpkeyfile = tbWSCPKey.Text;
            if (!firstread) mainform.XmlHelper.configSet("winscpkeyfile", Settings.Default.winscpkeyfile);
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            if (tbGPassword.Text == "" || tbGConfirm.Text == "") bGPassword.Enabled = false;
            else bGPassword.Enabled = true;
        }

        private void tbPassword_GotFocus(object sender, EventArgs e)
        {
            AcceptButton = bGPassword;
        }

        private void tbPassword_LostFocus(object sender, EventArgs e)
        {
            AcceptButton = null;
        }

        private void tbConfirm_TextChanged(object sender, EventArgs e)
        {
            tbPassword_TextChanged(this, e);
        }

        private void tbConfirm_GotFocus(object sender, EventArgs e)
        {
            tbPassword_GotFocus(this, e);
        }

        private void tbConfirm_LostFocus(object sender, EventArgs e)
        {
            tbPassword_LostFocus(this, e);
        }

        #endregion

        #region CheckBox Events

        private void cbPuTTYExecute_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.puttyexecute = cbPuTTYExecute.Checked;
            if (!firstread) mainform.XmlHelper.configSet("puttyexecute", Settings.Default.puttyexecute.ToString());

            if (Settings.Default.puttyexecute)
            {
                tbPuTTYExecute.Enabled = true;
                bPuTTYExecute.Enabled = true;
            }
            else
            {
                tbPuTTYExecute.Enabled = false;
                bPuTTYExecute.Enabled = false;
            }
        }

        private void cbPuTTYKey_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.puttykey = cbPuTTYKey.Checked;
            if (!firstread) mainform.XmlHelper.configSet("puttykey", Settings.Default.puttykey.ToString());

            if (Settings.Default.puttykey)
            {
                tbPuTTYKey.Enabled = true;
                bPuTTYKey.Enabled = true;
            }
            else
            {
                tbPuTTYKey.Enabled = false;
                bPuTTYKey.Enabled = false;
            }
        }

        private void cbPuTTYXforward_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.puttyforward = cbPuTTYForward.Checked;
            if (!firstread) mainform.XmlHelper.configSet("puttyforward", Settings.Default.puttyforward.ToString());
        }

        private void cbRDAdmin_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.rdadmin = cbRDAdmin.Checked;
            if (!firstread) mainform.XmlHelper.configSet("rdadmin", Settings.Default.rdadmin.ToString());
        }

        private void cbDrives_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.rddrives = cbRDDrives.Checked;
            if (!firstread) mainform.XmlHelper.configSet("rddrives", Settings.Default.rddrives.ToString());
        }

        private void cbRDSpan_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.rdspan = cbRDSpan.Checked;
            if (!firstread) mainform.XmlHelper.configSet("rdspan", Settings.Default.rdspan.ToString());
            cbRDSize.Enabled = !cbRDSpan.Checked;
        }

        private void cbRDSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList arraylist = new ArrayList();
            string[] size = cbRDSize.Text.Split('x');

            foreach (string width in size)
            {
                int num;
                if (Int32.TryParse(width.Trim(), out num)) arraylist.Add(width.Trim());
            }

            if (arraylist.Count == 2 || cbRDSize.Text.Trim() == cbRDSize.Items[cbRDSize.Items.Count - 1].ToString()) Settings.Default.rdsize = cbRDSize.Text.Trim();
            else Settings.Default.rdsize = "";
            if (!firstread) mainform.XmlHelper.configSet("rdsize", Settings.Default.rdsize);
        }

        private void cbRDSize_TextChanged(object sender, EventArgs e)
        {
            cbRDSize_SelectedIndexChanged(sender, e);
        }

        private void cbVNCFullscreen_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.vncfullscreen = cbVNCFullscreen.Checked;
            if (!firstread) mainform.XmlHelper.configSet("vncfullscreen", Settings.Default.vncfullscreen.ToString());
        }

        private void cbVNCView_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.vncviewonly = cbVNCViewonly.Checked;
            if (!firstread) mainform.XmlHelper.configSet("vncviewonly", Settings.Default.vncviewonly.ToString());
        }

        private void cbWSCPKey_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.winscpkey = cbWSCPKey.Checked;
            if (!firstread) mainform.XmlHelper.configSet("winscpkey", Settings.Default.winscpkey.ToString());

            if (Settings.Default.winscpkey)
            {
                tbWSCPKey.Enabled = true;
                bWSCPKey.Enabled = true;
            }
            else
            {
                tbWSCPKey.Enabled = false;
                bWSCPKey.Enabled = false;
            }
        }

        private void cbWSCPPassive_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.winscppassive = cbWSCPPassive.Checked;
            if (!firstread) mainform.XmlHelper.configSet("winscppassive", Settings.Default.winscppassive.ToString());
        }

        private void cbMulti_CheckedChanged(object sender, EventArgs e)
        {
            if (!firstread) mainform.XmlHelper.configSet("multicolumn", Settings.Default.multicolumn.ToString());

            if (Settings.Default.multicolumn)
            {
                //mainform.lbList.MultiColumn = true;
                slMulti_Scroll(this, e);
            }
            else
            {
                //mainform.lbList.MultiColumn = false;
            }

        }

        private void cbGPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGPassword.Checked)
            {
                tbGPassword.Enabled = true;
                tbGConfirm.Enabled = true;
            }
            else
            {
                if (Settings.Default.password != "")
                {
                    string[] bwArgs = { "recrypt", Settings.Default.ocryptkey };
                    bwProgress.RunWorkerAsync(bwArgs);
                    recryptpopup = new popupRecrypt(this);
                    recryptpopup.Text = "Removing" + recryptpopup.Text;
                    recryptpopup.ShowDialog(this);

                    mainform.XmlHelper.dropNode("ID='password'");
                    Settings.Default.password = "";
                    Settings.Default.cryptkey = Settings.Default.ocryptkey;
                }

                tbGPassword.Enabled = false;
                tbGPassword.Text = "";
                tbGConfirm.Enabled = false;
                tbGConfirm.Text = "";
                bGPassword.Enabled = false;
            }
        }

        private void cbGReplace_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGReplace.Checked) cbGSkip.Checked = false;
        }

        private void cbGSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGSkip.Checked) cbGReplace.Checked = false;
        }

        private void cbGMinimize_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.minimize = cbGMinimize.Checked;
            if (!firstread) mainform.XmlHelper.configSet("minimize", Settings.Default.minimize.ToString());
            mainform.notifyIcon.Visible = Settings.Default.minimize;
        }

        #endregion

        #region Other Events

        private void liGImport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("List format:\r\n\r\nName     Hostname[:port]     [[Domain\\]username]     [Password]     [Type]\r\n\r\n- One server per line.\r\n- Use a tab as separator.\r\n- Only \"Name\" and \"Hostname\" are required.\r\n- \"Type\" is a numerical value, use the following correspondence:\r\n    0 = PuTTY\r\n    1 = Remote Desktop\r\n    2 = VNC\r\n    3 = WinSCP (SCP)\r\n    4 = WinSCP (SFTP)\r\n    5 = WinSCP (FTP)\r\n- If no \"Type\" is given it'll be set as \"PuTTY\" by default.", "Import list");
        }

        private void bwProgress_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            object[] args = (object[])e.Argument;
            switch ((string)args[0])
            {
                case "import":
                    ImportList((string)args[1]);
                    break;
                case "recrypt":
                    ReCryptServeList((string)args[1]);
                    break;
            }
            e.Result = args[0];
        }

        private void bwProgress_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            string[] args = (string[])e.UserState;
            switch (args[0])
            {
                case "import":
                    args[0] = e.ProgressPercentage.ToString();
                    importpopup.ImportProgress(args);
                    break;
                case "recrypt":
                    args[0] = e.ProgressPercentage.ToString();
                    recryptpopup.RecryptProgress(args);
                    break;
            }
        }

        private void bwProgress_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            switch ((string)e.Result)
            {
                case "import":
                    importpopup.ImportComplete();
                    //mainform.lbList.SelectedItems.Clear();
                    //if (mainform.lbList.Items.Count > 0) mainform.lbList.SelectedIndex = 0;
                    break;
                case "recrypt":
                    recryptpopup.RecryptComplete();
                    break;
            }
        }

        private void slMulti_Scroll(object sender, EventArgs e)
        {
            if (!firstread) mainform.XmlHelper.configSet("multicolumnwidth", Settings.Default.multicolumnwidth.ToString());
            //mainform.lbList.ColumnWidth = Settings.Default.multicolumnwidth * 10;
        }

        #endregion

        #region Methods

        private void ReCryptServeList(string newPassword)
        {
            importcancel = false;
            int count = 0;

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Settings.Default.cfgpath);

            XmlNodeList xmlGroup = xmldoc.SelectNodes("//*[@GroupName]");

            foreach (XmlElement currentGroup in xmlGroup)
            {
                count++;

                foreach (XmlElement xmlElement in currentGroup.ChildNodes)
                {
                    string serverType = "";
                    switch (xmlElement.Name)
                    {
                        case "Server":

                            count++;

                            string serverHost = "";
                            string serverPort = "";
                            string serverUsername = "";
                            string serverPassword = "";

                            foreach (XmlElement serverElements in xmlElement.ChildNodes)
                            {

                                switch (serverElements.Name)
                                {

                                    case "Host":
                                        serverHost = cryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = cryptHelper.Encrypt(serverHost, newPassword);
                                        break;

                                    case "Port":
                                        serverPort = cryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = cryptHelper.Encrypt(serverPort, newPassword);
                                        break;

                                    case "Username":
                                        serverUsername = cryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = cryptHelper.Encrypt(serverUsername, newPassword);
                                        break;

                                    case "Password":
                                        serverPassword = cryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = cryptHelper.Encrypt(serverPassword, newPassword);
                                        break;

                                    case "Type":
                                        serverType = cryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = cryptHelper.Encrypt(serverType, newPassword);
                                        break;
                                }

                                string[] progressArgs = new string[] { "recrypt", count + " / " + mainform.tView.GetNodeCount(true) };
                                bwProgress.ReportProgress(((int)((double)count / (double)mainform.tView.GetNodeCount(true) * 100)), progressArgs);
                            }

                            break;
                        case "DefaultHost":
                            serverHost = cryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = cryptHelper.Encrypt(serverHost, newPassword);
                            break;

                        case "DefaultPort":
                            serverPort = cryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = cryptHelper.Encrypt(serverPort, newPassword);
                            break;

                        case "DefaultUsername":
                            serverUsername = cryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = cryptHelper.Encrypt(serverUsername, newPassword);
                            break;

                        case "DefaultPassword":
                            serverPassword = cryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = cryptHelper.Encrypt(serverPassword, newPassword);
                            break;
                    }

                    
                }
            }

            string[] args = new string[] { "recrypt", count + " / " + mainform.tView.GetNodeCount(true) };
            bwProgress.ReportProgress(((int)((double)count / (double)mainform.tView.GetNodeCount(true) * 100)), args);

            xmldoc.Save(Settings.Default.cfgpath);
            Console.WriteLine(mainform.tView.GetNodeCount(true));
        }

        private void ImportList(string f)
        {
            #if DEBUG
            DateTime time = DateTime.Now;
            #endif

            importcancel = false;
            importempty = false;
            string line;
            int c_add = 0;
            int c_replace = 0;
            int c_skip = 0;
            int c_total = 0;

            // Read the import file line by line.

            ArrayList lines = new ArrayList();
            StreamReader stream = new StreamReader(f);
            while ((line = stream.ReadLine()) != null) lines.Add(line.Trim());
            stream.Close();

            string file = Settings.Default.cfgpath;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(file);

            string[] args = new string[] { "import", c_total + " / " + lines.Count, c_add.ToString(), c_replace.ToString(), c_skip.ToString() };
            bwProgress.ReportProgress(((int)((double)c_total / (double)lines.Count * 100)), args);

            for (int i = 0; i < lines.Count && !importcancel; i++)
            {
                //cancel = bwProgress.CancellationPending;
                //if (cancel) break;
                c_total++;
                line = lines[i].ToString();

                ArrayList listarray = new ArrayList();
                string[] split = line.Split('	');

                foreach (string arg in split) listarray.Add(arg.Trim());

                if (listarray.Count > 1)
                {
                    importreplace = "";
                    string _name = split[0].Trim();
                    string _host = split[1].Trim();
                    string _user = "";
                    string _pass = "";
                    int _type = 0;

                    if (listarray.Count > 2) _user = split[2];
                    if (listarray.Count > 3) _pass = split[3];
                    if (listarray.Count > 4) Int32.TryParse(split[4], out _type);

                    XmlElement newserver = xmldoc.CreateElement("Server");
                    XmlAttribute name = xmldoc.CreateAttribute("Name");
                    name.Value = _name;
                    newserver.SetAttributeNode(name);

                    if (_host != "")
                    {
                        XmlElement host = xmldoc.CreateElement("Host");
                        host.InnerText = cryptHelper.Encrypt(_host);
                        newserver.AppendChild(host);
                    }
                    if (_user != "")
                    {
                        XmlElement user = xmldoc.CreateElement("User");
                        user.InnerText = cryptHelper.Encrypt(_user);
                        newserver.AppendChild(user);
                    }
                    if (_pass != "")
                    {
                        XmlElement pass = xmldoc.CreateElement("Password");
                        pass.InnerText = cryptHelper.Encrypt(_pass);
                        newserver.AppendChild(pass);
                    }
                    if (_type > 0)
                    {
                        XmlElement type = xmldoc.CreateElement("Type");
                        type.InnerText = _type.ToString();
                        newserver.AppendChild(type);
                    }

                    if (mainform.tView.Nodes.Find(_name, true) != null) //duplicate
                    {
                        if (cbGSkip.Checked) //skip
                        {
                            c_skip++;
                        }
                        else //replace
                        {
                            if (cbGReplace.Checked || (!cbGReplace.Checked && ImportAskDuplicate(_name)))
                            {
                                XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@Name=" + mainform.XmlHelper.parseXpathString(_name) + "]");
                                if (xmldoc.DocumentElement != null)
                                {
                                    if (xmlnode != null) xmldoc.DocumentElement.ReplaceChild(newserver, xmlnode[0]);
                                }
                                //if (mainform.lbList.InvokeRequired) Invoke(new MethodInvoker(delegate
                                //{
                                //    mainform.lbList.Items.Remove(_name);
                                //    mainform.lbList.Items.Add(_name);
                                //}));
                                //else
                                //{
                                //    mainform.lbList.Items.Remove(_name);
                                //    mainform.lbList.Items.Add(_name);
                                //}
                                c_replace++;
                            }
                            else //cancel or skip
                            {
                                if (!importcancel) c_skip++;
                                else c_total--;
                            }
                        }
                    }
                    else //add
                    {
                        if (xmldoc.DocumentElement != null) xmldoc.DocumentElement.InsertAfter(newserver, xmldoc.DocumentElement.LastChild);
                        //if (mainform.lbList.InvokeRequired) Invoke(new MethodInvoker(delegate { mainform.lbList.Items.Add(_name); }));
                        //else mainform.lbList.Items.Add(_name);
                        c_add++;
                    }
                }
                args = new string[] { "import", c_total + " / " + lines.Count, c_add.ToString(), c_replace.ToString(), c_skip.ToString() };
                bwProgress.ReportProgress(((int)((double)c_total / (double)lines.Count * 100)), args);
            }
            xmldoc.Save(file);
            #if DEBUG
            Debug.WriteLine("Import duration :" + (DateTime.Now - time));
            #endif
            if (!importcancel && (c_add + c_replace + c_skip) < 1) importempty = true;
        }

        public bool ImportAskDuplicate(string n)
        {
            importpopup.ToggleDuplicateWarning(true, "Duplicate found: " + n);
            lock (locker) while (importreplace == "" && !importcancel) Monitor.Wait(locker);
            if (importreplace == "replace") return true;
            return false;
        }

        #endregion
    }
}