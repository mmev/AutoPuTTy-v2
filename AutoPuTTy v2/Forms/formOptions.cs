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

        private readonly formMain MainForm;
        private popupImport ImportPopup;
        private popupRecrypt ReCryptPopup;
        private readonly bool FirstRead = true;
        public bool ImportEmpty;

        public readonly object locker = new object();
        public string ImportReplace = "";

        #endregion

        #region Element Loading

        public formOptions(formMain form)
        {
            MainForm = form;
            InitializeComponent();
            ImportPopup = new popupImport(this);
            ReCryptPopup = new popupRecrypt(this);

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
                    Settings.Default.password = CryptHelper.Decrypt(Settings.Default.password, Settings.Default.pcryptkey);
                    tbGPassword.Text = Settings.Default.password;
                    tbGConfirm.Text = Settings.Default.password;
                    Settings.Default.cryptkey = Settings.Default.password;
                    cbGPassword.Checked = true;
                }
            }

            bGPassword.Enabled = false;
            FirstRead = false;
        }

        #endregion

        #region Button Events

        /// <summary>
        /// When user try connect SSH and PuTTy client not found
        /// this method launch event click select path and retry connect
        /// </summary>
        /// <param name="type">connection type</param>
        public void bPuTTYPath_Click()
        {
            bPuTTYPath_Click(new object(), new EventArgs());
            MainForm.connect();
        }

        /// <summary>
        /// Browse PuTTy (.exe) file path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPuTTYPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bPuTTYPath_Click_Select_PuTTY_executable,
                Filter = Resources.formOptions_bPuTTYPath_Click_EXE_File____exe____exe
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbPuTTYPath.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// When user try connect RDP and rdp client not found
        /// this method launch event click select path and retry connect
        /// </summary>
        /// <param name="type">connection type</param>
        public void bRDPath_Click()
        {
            bRDPath_Click(new object(), new EventArgs());
            MainForm.connect();
        }

        /// <summary>
        /// Open file dialog for select rdp (mstsc.exe) file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bRDPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bRDPath_Click_Select_Remote_Desktop_executable,
                Filter = Resources.formOptions_bPuTTYPath_Click_EXE_File____exe____exe
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbRDPath.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// When user try connect VNC and vnc client not found
        /// this method launch event click select path and retry connect
        /// </summary>
        /// <param name="type">connection type</param>
        public void bVNCPath_Click()
        {
            bVNCPath_Click(new object(), new EventArgs());
            MainForm.connect();
        }

        /// <summary>
        /// Open file dialog for select vnc client file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bVNCPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bVNCPath_Click_Select_VNC_Viewer_executable,
                Filter = Resources.formOptions_bPuTTYPath_Click_EXE_File____exe____exe
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbVNCPath.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// Open file dialog for select folder for saving .vnc files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bVNCKeep_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = Resources.formOptions_bVNCKeep_Click_Select__vnc_files_path
            };

            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tbVNCKeep.Text = folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// When user try connect with WinSCP and client not found
        /// this method launch event click select path and retry connect
        /// </summary>
        /// <param name="type">connection type</param>
        public void bWSCPPath_Click()
        {
            bWSCPPath_Click(new object(), new EventArgs());
            MainForm.connect();
        }

        /// <summary>
        /// Open file dialog for select WinSCP client exe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bWSCPPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bWSCPPath_Click_Select_WinSCP_executable,
                Filter = Resources.formOptions_bPuTTYPath_Click_EXE_File____exe____exe
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                if (browseFile.FileName.Contains(" ")) browseFile.FileName = "\"" + browseFile.FileName + "\"";
                tbWSCPPath.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// Open file dialog for select WinSCP client exe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bWSCPKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bWSCPKey_Click_Select_private_key_file,
                Filter = Resources.formOptions_bWSCPKey_Click_PuTTY_private_key_files____ppk____ppk_All_files__________
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                tbWSCPKey.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// Set new password for crypt group and server data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPassword_Click(object sender, EventArgs e)
        {
            if (tbGPassword.Text.Trim() == "")
            {
                MessageBox.Show(this, Resources.formOptions_bPassword_Click_Password_can_t_be_empty, Resources.connectionHelper_LaunchVnc_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGPassword.Text = "";
                tbGConfirm.Text = "";
            }
            else if (tbGConfirm.Text != tbGPassword.Text)
            {
                MessageBox.Show(this, Resources.formOptions_bPassword_Click_Password_confirmation_doesn_t_match, Resources.connectionHelper_LaunchVnc_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbGConfirm.Text = "";
            }
            else
            {
                if (Settings.Default.password != tbGPassword.Text)
                {
                    Settings.Default.password = tbGPassword.Text;
                    MainForm.XmlHelper.configSet("password", CryptHelper.Encrypt(Settings.Default.password, Settings.Default.pcryptkey));

                    string[] bwArgs = { "recrypt", Settings.Default.password };
                    bwProgress.RunWorkerAsync(bwArgs);
                    ReCryptPopup = new popupRecrypt(this);
                    ReCryptPopup.Text = Resources.formOptions_bPassword_Click_Applying + ReCryptPopup.Text;
                    ReCryptPopup.ShowDialog(this);

                    Settings.Default.cryptkey = Settings.Default.password;
                }
                bGPassword.Enabled = false;
            }
        }

        /// <summary>
        /// Browser .txt with server data
        /// Show import group|server popup   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bGImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bGImport_Click_Select_server_list,
                Filter = Resources.formOptions_bGImport_Click_TXT_File____txt____txt
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                string file = browseFile.FileName;
                if (File.Exists(file))
                {
                    ImportPopup = new popupImport(this);
                    object[] bwArgs = { "import", file };
                    bwProgress.RunWorkerAsync(bwArgs);
                    ImportPopup.ShowDialog(this);
                }
            }
        }

        /// <summary>
        /// Select .txt with command for putty start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPuTTYExecute_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bPuTTYExecute_Click_Select_commands_file,
                Filter = Resources.formOptions_bGImport_Click_TXT_File____txt____txt
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                tbPuTTYExecute.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// Select private key for ssh connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPuTTYKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Title = Resources.formOptions_bWSCPKey_Click_Select_private_key_file,
                Filter = Resources
                    .formOptions_bWSCPKey_Click_PuTTY_private_key_files____ppk____ppk_All_files__________
            };

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                tbPuTTYKey.Text = browseFile.FileName;
            }
        }

        /// <summary>
        /// Select folder for save .rdp files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bRDKeep_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog {Description = Resources.formOptions_bRDKeep_Click_Select__rdp_files_path};
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                tbRDKeep.Text = folderBrowserDialog.SelectedPath;
            }
        }

        #endregion

        #region TextBox Events

        
        private void tbPuTTY_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.puttypath = tbPuTTYPath.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("putty", Settings.Default.puttypath);
        }

        private void tbPuTTYExecute_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.puttycommand = tbPuTTYExecute.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("puttycommand", Settings.Default.puttycommand);
        }

        private void tbPuTTYKey_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.puttykeyfile = tbPuTTYKey.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("puttykeyfile", Settings.Default.puttykeyfile);
        }

        private void tbRD_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.rdpath = tbRDPath.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("remotedesktop", Settings.Default.rdpath);
        }

        private void tbRDKeep_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.rdfilespath = tbRDKeep.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("rdfilespath", Settings.Default.rdfilespath);
        }

        private void tbVNCPath_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.vncpath = tbVNCPath.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("vnc", Settings.Default.vncpath);
        }

        private void tbVNCKeep_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.vncfilespath = tbVNCKeep.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("vncfilespath", Settings.Default.vncfilespath);
        }

        private void tbWSCPPath_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.winscppath = tbWSCPPath.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("winscp", Settings.Default.winscppath);
        }

        private void tbWSCPKey_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.winscpkeyfile = tbWSCPKey.Text;
            if (!FirstRead) MainForm.XmlHelper.configSet("winscpkeyfile", Settings.Default.winscpkeyfile);
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
            if (!FirstRead) MainForm.XmlHelper.configSet("puttyexecute", Settings.Default.puttyexecute.ToString());

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
            if (!FirstRead) MainForm.XmlHelper.configSet("puttykey", Settings.Default.puttykey.ToString());

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
            if (!FirstRead) MainForm.XmlHelper.configSet("puttyforward", Settings.Default.puttyforward.ToString());
        }

        private void cbRDAdmin_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.rdadmin = cbRDAdmin.Checked;
            if (!FirstRead) MainForm.XmlHelper.configSet("rdadmin", Settings.Default.rdadmin.ToString());
        }

        private void cbDrives_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.rddrives = cbRDDrives.Checked;
            if (!FirstRead) MainForm.XmlHelper.configSet("rddrives", Settings.Default.rddrives.ToString());
        }

        private void cbRDSpan_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.rdspan = cbRDSpan.Checked;
            if (!FirstRead) MainForm.XmlHelper.configSet("rdspan", Settings.Default.rdspan.ToString());
            cbRDSize.Enabled = !cbRDSpan.Checked;
        }

        private void cbRDSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList arraylist = new ArrayList();
            string[] size = cbRDSize.Text.Split('x');

            foreach (string width in size)
            {
                if (Int32.TryParse(width.Trim(), out _)) arraylist.Add(width.Trim());
            }

            if (arraylist.Count == 2 || cbRDSize.Text.Trim() == cbRDSize.Items[cbRDSize.Items.Count - 1].ToString()) Settings.Default.rdsize = cbRDSize.Text.Trim();
            else Settings.Default.rdsize = "";
            if (!FirstRead) MainForm.XmlHelper.configSet("rdsize", Settings.Default.rdsize);
        }

        private void cbRDSize_TextChanged(object sender, EventArgs e)
        {
            cbRDSize_SelectedIndexChanged(sender, e);
        }

        private void cbVNCFullscreen_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.vncfullscreen = cbVNCFullscreen.Checked;
            if (!FirstRead) MainForm.XmlHelper.configSet("vncfullscreen", Settings.Default.vncfullscreen.ToString());
        }

        private void cbVNCView_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.vncviewonly = cbVNCViewonly.Checked;
            if (!FirstRead) MainForm.XmlHelper.configSet("vncviewonly", Settings.Default.vncviewonly.ToString());
        }

        private void cbWSCPKey_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.winscpkey = cbWSCPKey.Checked;
            if (!FirstRead) MainForm.XmlHelper.configSet("winscpkey", Settings.Default.winscpkey.ToString());

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
            if (!FirstRead) MainForm.XmlHelper.configSet("winscppassive", Settings.Default.winscppassive.ToString());
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
                    ReCryptPopup = new popupRecrypt(this);
                    ReCryptPopup.Text = Resources.formOptions_cbGPassword_CheckedChanged_Removing + ReCryptPopup.Text;
                    ReCryptPopup.ShowDialog(this);

                    MainForm.XmlHelper.dropNode("ID='password'");
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
            if (!FirstRead) MainForm.XmlHelper.configSet("minimize", Settings.Default.minimize.ToString());
            MainForm.notifyIcon.Visible = Settings.Default.minimize;
        }

        #endregion

        #region Other Events

        private void liGImport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(Resources.formOptions_liGImport_LinkClicked_, "Import list");
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
                    ImportPopup.ImportProgress(args);
                    break;
                case "recrypt":
                    args[0] = e.ProgressPercentage.ToString();
                    ReCryptPopup.RecryptProgress(args);
                    break;
            }
        }

        private void bwProgress_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            switch ((string)e.Result)
            {
                case "import":
                    ImportPopup.ImportComplete();
                    //MainForm.lbList.SelectedItems.Clear();
                    //if (MainForm.lbList.Items.Count > 0) MainForm.lbList.SelectedIndex = 0;
                    break;
                case "recrypt":
                    ReCryptPopup.RecryptComplete();
                    break;
            }
        }

        private void slMulti_Scroll(object sender, EventArgs e)
        {
            if (!FirstRead) MainForm.XmlHelper.configSet("multicolumnwidth", Settings.Default.multicolumnwidth.ToString());
            //MainForm.lbList.ColumnWidth = Settings.Default.multicolumnwidth * 10;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method for re crypt group default data and server data with new custom password
        /// </summary>
        /// <param name="newPassword">new password for encrypt</param>
        private void ReCryptServeList(string newPassword)
        {
            var count = 0;

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

                            string serverHost;
                            string serverPort;
                            string serverUsername;
                            string serverPassword;

                            foreach (XmlElement serverElements in xmlElement.ChildNodes)
                            {

                                switch (serverElements.Name)
                                {

                                    case "Host":
                                        serverHost = CryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = CryptHelper.Encrypt(serverHost, newPassword);
                                        break;

                                    case "Port":
                                        serverPort = CryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = CryptHelper.Encrypt(serverPort, newPassword);
                                        break;

                                    case "Username":
                                        serverUsername = CryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = CryptHelper.Encrypt(serverUsername, newPassword);
                                        break;

                                    case "Password":
                                        serverPassword = CryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = CryptHelper.Encrypt(serverPassword, newPassword);
                                        break;

                                    case "Type":
                                        serverType = CryptHelper.Decrypt(serverElements.InnerText);
                                        serverElements.InnerText = CryptHelper.Encrypt(serverType, newPassword);
                                        break;
                                }

                                string[] progressArgs = new string[] { "recrypt", count + " / " + MainForm.tView.GetNodeCount(true) };
                                bwProgress.ReportProgress(((int)((double)count / (double)MainForm.tView.GetNodeCount(true) * 100)), progressArgs);
                            }

                            break;
                        case "DefaultHost":
                            serverHost = CryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = CryptHelper.Encrypt(serverHost, newPassword);
                            break;

                        case "DefaultPort":
                            serverPort = CryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = CryptHelper.Encrypt(serverPort, newPassword);
                            break;

                        case "DefaultUsername":
                            serverUsername = CryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = CryptHelper.Encrypt(serverUsername, newPassword);
                            break;

                        case "DefaultPassword":
                            serverPassword = CryptHelper.Decrypt(xmlElement.InnerText);
                            xmlElement.InnerText = CryptHelper.Encrypt(serverPassword, newPassword);
                            break;
                    }

                    
                }
            }

            var args = new string[] { "recrypt", count + " / " + MainForm.tView.GetNodeCount(true) };
            bwProgress.ReportProgress(((int)((double)count / (double)MainForm.tView.GetNodeCount(true) * 100)), args);

            xmldoc.Save(Settings.Default.cfgpath);
        }

        private void ImportList(string f)
        {
            ImportEmpty = false;
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

            for (int i = 0; i < lines.Count; i++)
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
                    ImportReplace = "";
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
                        host.InnerText = CryptHelper.Encrypt(_host);
                        newserver.AppendChild(host);
                    }
                    if (_user != "")
                    {
                        XmlElement user = xmldoc.CreateElement("User");
                        user.InnerText = CryptHelper.Encrypt(_user);
                        newserver.AppendChild(user);
                    }
                    if (_pass != "")
                    {
                        XmlElement pass = xmldoc.CreateElement("Password");
                        pass.InnerText = CryptHelper.Encrypt(_pass);
                        newserver.AppendChild(pass);
                    }
                    if (_type > 0)
                    {
                        XmlElement type = xmldoc.CreateElement("Type");
                        type.InnerText = _type.ToString();
                        newserver.AppendChild(type);
                    }

                    if (MainForm.tView.Nodes.Find(_name, true) != null) //duplicate
                    {
                        if (cbGSkip.Checked) //skip
                        {
                            c_skip++;
                        }
                        else //replace
                        {
                            if (cbGReplace.Checked || (!cbGReplace.Checked && ImportAskDuplicate(_name)))
                            {
                                XmlNodeList xmlnode = xmldoc.SelectNodes("//*[@Name=" + MainForm.XmlHelper.parseXpathString(_name) + "]");
                                if (xmldoc.DocumentElement != null)
                                {
                                    if (xmlnode != null) xmldoc.DocumentElement.ReplaceChild(newserver, xmlnode[0]);
                                }
                                //if (MainForm.lbList.InvokeRequired) Invoke(new MethodInvoker(delegate
                                //{
                                //    MainForm.lbList.Items.Remove(_name);
                                //    MainForm.lbList.Items.Add(_name);
                                //}));
                                //else
                                //{
                                //    MainForm.lbList.Items.Remove(_name);
                                //    MainForm.lbList.Items.Add(_name);
                                //}
                                c_replace++;
                            }
                            else //cancel or skip
                            {
                                c_total--;
                            }
                        }
                    }
                }
                args = new string[] { "import", c_total + " / " + lines.Count, c_add.ToString(), c_replace.ToString(), c_skip.ToString() };
                bwProgress.ReportProgress(((int)((double)c_total / (double)lines.Count * 100)), args);
            }
            xmldoc.Save(file);
            if ((c_add + c_replace + c_skip) < 1) ImportEmpty = true;
        }

        public bool ImportAskDuplicate(string n)
        {
            ImportPopup.ToggleDuplicateWarning(true, "Duplicate found: " + n);
            lock (locker) while (ImportReplace == "") Monitor.Wait(locker);
            if (ImportReplace == "replace") return true;
            return false;
        }

        #endregion
    }
}