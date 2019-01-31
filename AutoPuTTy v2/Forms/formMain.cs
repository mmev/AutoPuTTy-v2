using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AutoPuTTY.Forms.Popups;
using AutoPuTTY.Properties;
using AutoPuTTY.Utils;
using AutoPuTTY.Utils.Data;

namespace AutoPuTTY.Forms
{
    public sealed partial class formMain : Form
    {
        #region Conts Init

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIdNewItem, string lpNewItem);

        public const int IdmAbout = 1000;
        public const int MfByPosition = 0x400;
        public const int MfSeparator = 0x800;
        public const int WmSysCommand = 0x112;

        public static formOptions optionsForm;

        public string[] types = { "PuTTY", StringResources.formMain_cbType_SelectedIndexChanged_Remote_Desktop, "VNC", "WinSCP (SCP)", "WinSCP (SFTP)", "WinSCP (FTP)" };
        public string[] Types;

        private string lastState = "normal";

        private ArrayList groupList = new ArrayList();

        private XmlHelper xmlHelper;
        internal XmlHelper XmlHelper { get => xmlHelper; set => xmlHelper = value; }

        internal CryptHelper Cryptor { get; set; }

        internal OtherHelper OtherHelper { get; set; }

        private string placeholderServerHost = "";
        string placeholderServerPort = "";
        string placeholderServerUsername = "";
        string placeholderServerPassword = "";

        bool placeholderMode = true;

        bool placeholderModeHost = true;
        bool placeholderModePort = true;
        bool placeholderModeUsername = true;
        bool placeholderModePassword = true;

        string currentGroup = "";

        #endregion

        #region Element Loading

        public formMain(bool full)
        {
            XmlHelper = new XmlHelper();
            Cryptor = new CryptHelper();
            OtherHelper = new OtherHelper();

            //clone types array to have a sorted version
            Types = (string[])types.Clone();
            Array.Sort(Types);
            string configPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)?.Replace("file:\\", "");
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (File.Exists(configPath + "\\" + Settings.Default.cfgfilepath)) Settings.Default.cfgpath = configPath + "\\" + Settings.Default.cfgfilepath;
            else if (File.Exists(userPath + "\\" + Settings.Default.cfgfilepath)) Settings.Default.cfgpath = userPath + "\\" + Settings.Default.cfgfilepath;
            else
            {
                try
                {
                    Settings.Default.cfgpath = configPath + "\\" + Settings.Default.cfgfilepath;
                    XmlHelper.CreateDefaultConfig();
                }
                catch (UnauthorizedAccessException)
                {
                    if (!File.Exists(userPath))
                    {
                        try
                        {
                            Settings.Default.cfgpath = userPath + "\\" + Settings.Default.cfgfilepath;
                            XmlHelper.CreateDefaultConfig();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            OtherHelper.Error("No really, I could not find nor write my configuration file :'(\rPlease check your user permissions.");
                            Environment.Exit(-1);
                        }
                    }
                }
            }

            if (!full) return;
            InitializeComponent();

            foreach (string type in Types)
            {
                cbType.Items.Add(type);
            }
            cbType.SelectedIndex = 0;
            if (XmlHelper.configGet("password") != "") Settings.Default.password = XmlHelper.configGet("password");
            if (XmlHelper.configGet("multicolumnwidth") != "") Settings.Default.multicolumnwidth = Convert.ToInt32(XmlHelper.configGet("multicolumnwidth"));
            if (XmlHelper.configGet("multicolumn").ToLower() == "true") Settings.Default.multicolumn = true;
            if (XmlHelper.configGet("minimize").ToLower() == "false") Settings.Default.minimize = false;
            if (XmlHelper.configGet("putty") != "") Settings.Default.puttypath = XmlHelper.configGet("putty");
            if (XmlHelper.configGet("puttyexecute").ToLower() == "true") Settings.Default.puttyexecute = true;
            if (XmlHelper.configGet("puttycommand") != "") Settings.Default.puttycommand = XmlHelper.configGet("puttycommand");
            if (XmlHelper.configGet("puttykey").ToLower() == "true") Settings.Default.puttykey = true;
            if (XmlHelper.configGet("puttykeyfile") != "") Settings.Default.puttykeyfile = XmlHelper.configGet("puttykeyfile");
            if (XmlHelper.configGet("puttyforward").ToLower() == "true") Settings.Default.puttyforward = true;
            if (XmlHelper.configGet("remotedesktop") != "") Settings.Default.rdpath = XmlHelper.configGet("remotedesktop");
            if (XmlHelper.configGet("rdfilespath") != "") Settings.Default.rdfilespath = XmlHelper.configGet("rdfilespath");
            if (XmlHelper.configGet("rdadmin").ToLower() == "true") Settings.Default.rdadmin = true;
            if (XmlHelper.configGet("rddrives").ToLower() == "true") Settings.Default.rddrives = true;
            if (XmlHelper.configGet("rdspan").ToLower() == "true") Settings.Default.rdspan = true;
            if (XmlHelper.configGet("rdsize") != "") Settings.Default.rdsize = XmlHelper.configGet("rdsize");
            if (XmlHelper.configGet("vnc") != "") Settings.Default.vncpath = XmlHelper.configGet("vnc");
            if (XmlHelper.configGet("vncfilespath") != "") Settings.Default.vncfilespath = XmlHelper.configGet("vncfilespath");
            if (XmlHelper.configGet("vncfullscreen").ToLower() == "true") Settings.Default.vncfullscreen = true;
            if (XmlHelper.configGet("vncviewonly").ToLower() == "true") Settings.Default.vncviewonly = true;
            if (XmlHelper.configGet("winscp") != "") Settings.Default.winscppath = XmlHelper.configGet("winscp");
            if (XmlHelper.configGet("winscpkey").ToLower() == "true") Settings.Default.winscpkey = true;
            if (XmlHelper.configGet("winscpkeyfile") != "") Settings.Default.winscpkeyfile = XmlHelper.configGet("winscpkeyfile");
            if (XmlHelper.configGet("winscppassive").ToLower() == "false") Settings.Default.winscppassive = false;

            optionsForm = new formOptions(this);

            IntPtr sysMenuHandle = GetSystemMenu(Handle, false);
            //It would be better to find the position at run time of the 'Close' item, but...

            InsertMenu(sysMenuHandle, 5, MfByPosition | MfSeparator, 0, string.Empty);
            InsertMenu(sysMenuHandle, 6, MfByPosition, IdmAbout, "About");

            notifyIcon.Visible = Settings.Default.minimize;
            notifyIcon.ContextMenu = cmSystray;

            updateTreeView();

            AutoSize = false;
            MinimumSize = Size;
        }

        #endregion

        #region Button Events

        private void bModify_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode?.Parent != null)
            {
                string groupName = tView.SelectedNode.Parent.Text;
                string oldServerName = tView.SelectedNode.Text;

                string newServerName = tbServerName.Text.Trim();
                string newServerHost = tbServerHost.Text.Trim();
                string newServerPort = textBox1.Text.Trim();
                string newServerUsername = tbServerUser.Text.Trim();
                string newServerPassword = tbServerPass.Text.Trim();
                string newServerType = Array.IndexOf(types, cbType.Text).ToString();

                ServerElement server = new ServerElement(newServerName, newServerHost, newServerPort, newServerUsername, newServerPassword, newServerType);

                xmlHelper.modifyServer(groupName, oldServerName, server);

                bServerModify.Enabled = false;
                bServerAdd.Enabled = false;

                updateTreeView();
            }
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode != null && tbServerName.Text.Trim() != "")
            {
                string groupName = tView.SelectedNode.Text;
                string serverName = tbServerName.Text.Trim();

                if (tView.SelectedNode.Parent != null)
                    groupName = tView.SelectedNode.Parent.Text;

                string serverHostname = tbServerHost.Text.Trim();
                string serverPort = textBox1.Text.Trim();
                string serverUsername = tbServerUser.Text.Trim();
                string serverPassword = tbServerPass.Text.Trim();
                string serverType = Array.IndexOf(types, cbType.Text).ToString();

                xmlHelper.addServer(groupName, serverName, serverHostname, serverPort,
                    serverUsername, serverPassword, serverType);

                tbServerName.Text = tbServerName.Text.Trim();

                bServerModify.Enabled = false;
                bServerAdd.Enabled = false;
                bServerDelete.Enabled = true;

                updateTreeView();
            }
            else
            {
                OtherHelper.Error("Enter server name and try again.");
            }
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode != null)
            {
                TreeNode currentNode = tView.SelectedNode;

                if (currentNode.Parent == null)
                {
                    OtherHelper.Error("Please select server!");
                    return;
                }

                string groupName = currentNode.Parent.Text;
                string serverName = currentNode.Text;

                xmlHelper.deleteServerByName(groupName, serverName);
                updateTreeView();
            }
        }

        private void bEye_Click(object sender, EventArgs e)
        {
            TooglePassword(bServerEye, tbServerPass, !tbServerPass.UseSystemPasswordChar);
        }
        private void bEye_MouseEnter(object sender, EventArgs e)
        {
            bServerEye.Image = OtherHelper.Set(bServerEye.Image, (float)0.50);
        }
        private void bEye_MouseLeave(object sender, EventArgs e)
        {
            bServerEye.Image = (tbServerPass.UseSystemPasswordChar ? Resources.iconeyeshow : Resources.iconeyehide);
        }

        private void bOptions_Click(object sender, EventArgs e)
        {
            optionsForm.ShowDialog(this);
        }

        private void bGroupEye_Click(object sender, EventArgs e)
        {
            TooglePassword(bGroupEye, tbGroupDefaultPassword, !tbGroupDefaultPassword.UseSystemPasswordChar);
        }
        private void bGroupEye_MouseEnter(object sender, EventArgs e)
        {
            bGroupEye.Image = (tbGroupDefaultPassword.UseSystemPasswordChar ? Resources.iconeyeshow : Resources.iconeyehide);
        }
        private void bGroupEye_MouseLeave(object sender, EventArgs e)
        {
            bGroupEye.Image = (tbGroupDefaultPassword.UseSystemPasswordChar ? Resources.iconeyeshow : Resources.iconeyehide);
        }

        // Creating new group by click
        private void bGroupAdd_Click(object sender, EventArgs e)
        {
            if (tbGroupName.Text.Trim() != "")
            {
                string groupName = tbGroupName.Text.Trim();
                string groupDefaultHost = tbGroupDefaultHost.Text.Trim();
                string groupDefaultPort = tbGroupDefaultPort.Text.Trim();
                string groupDefaultUsername = tbGroupDefaultUsername.Text.Trim();
                string groupDefaultPassword = tbGroupDefaultPassword.Text.Trim();

                xmlHelper.createGroup(groupName, groupDefaultHost, groupDefaultPort, groupDefaultUsername, groupDefaultPassword);

                TreeNode newNode = tView.Nodes.Add(groupName);
                tView.Focus();
                tView.SelectedNode = newNode;
                newNode.EnsureVisible();

                bGroupModify.Enabled = false;
                bGroupAdd.Enabled = false;
                bGroupDelete.Enabled = true;
            }
            else {
                OtherHelper.Error("Enter group name and try again.");
            }
        }

        // Deleting group by click
        private void bGroupDelete_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode != null)
            {
                xmlHelper.deleteGroup(tView.SelectedNode.Text);
                tView.Nodes.Remove(tView.SelectedNode);

                bGroupDelete.Enabled = false;
                bGroupAdd.Enabled = false;
                bGroupModify.Enabled = false;

                tView_NodeMouseClick(null, new TreeNodeMouseClickEventArgs(tView.SelectedNode, MouseButtons.Left, 1, 1, 1));
            }
        }

        // Modify group by click
        private void bGroupModify_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode != null)
            {
                string groupName = tView.SelectedNode.Text;

                string newGroupName = tbGroupName.Text;
                string newGroupDefaultHost = tbGroupDefaultHost.Text;
                string newGroupDefaultPort = tbGroupDefaultPort.Text;
                string newGroupDefaultUsername = tbGroupDefaultUsername.Text;
                string newGroupDefaultPassword = tbGroupDefaultPassword.Text;

                xmlHelper.modifyGroup(groupName, newGroupName, newGroupDefaultHost, newGroupDefaultPort,
                    newGroupDefaultUsername, newGroupDefaultPassword);

                int selectedNodeIndex = tView.SelectedNode.Index;
                updateTreeView();
                tView.SelectedNode = tView.Nodes[selectedNodeIndex];
                tView_NodeMouseClick(this, new TreeNodeMouseClickEventArgs(tView.Nodes[selectedNodeIndex], MouseButtons.Left, 1, 1, 1));
            }
        }

        #endregion

        #region TextBox Events

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (currentGroup != "")
            {
                if (tView.SelectedNode != null && tbServerName.Text.Trim() != "")
                {
                    //changed name
                    if (tbServerName.Text != tView.SelectedNode.Text)
                    {
                        //if new name doesn't exist in list, modify or add
                        bServerModify.Enabled = XmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) == null;
                        bServerAdd.Enabled = XmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) == null;
                    }
                    //changed other stuff
                    else
                    {
                        bServerModify.Enabled = true;
                        bServerAdd.Enabled = false;
                    }
                }
                //create new item
                else
                {
                    bServerModify.Enabled = false;

                    if (tbServerName.Text.Trim() != "" && XmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) == null)
                        bServerAdd.Enabled = true;
                    else
                        bServerAdd.Enabled = false;
                }
            }
        }

        private void tbHost_TextChanged(object sender, EventArgs e)
        {
            tbName_TextChanged(this, e);
        }

        private void tbUser_TextChanged(object sender, EventArgs e)
        {
            tbName_TextChanged(this, e);
        }

        private void tbPass_TextChanged(object sender, EventArgs e)
        {
            tbName_TextChanged(this, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tbName_TextChanged(null, e);
        }


        /*
         * NEW EVENTS
         */

        private void tbGroupName_TextChanged(object sender, EventArgs e)
        {
            //modify an existing item
            if (tView.SelectedNode != null && tbGroupName.Text.Trim() != "")
            {
                //changed name
                if (tbGroupName.Text.Trim() != tView.SelectedNode.Text.Trim())
                {
                    //if new name doesn't exist in list, modify or add
                    bGroupModify.Enabled = xmlHelper.getGroupDefaultInfo(tbGroupName.Text.Trim()) == null;
                    bGroupAdd.Enabled = xmlHelper.getGroupDefaultInfo(tbGroupName.Text.Trim()) == null;
                }
                //changed other stuff
                else
                {
                    bGroupModify.Enabled = true;
                    bGroupAdd.Enabled = false;
                }
            }
            //create new item
            else
            {
                bGroupModify.Enabled = false;
                bGroupAdd.Enabled = tbGroupName.Text.Trim() != "";
            }
        }

        private void tbGroupDefaultHost_TextChanged(object sender, EventArgs e)
        {
            tbGroupName_TextChanged(this, e);
        }
        private void tbGroupDefaultPort_TextChanged(object sender, EventArgs e)
        {
            tbGroupName_TextChanged(this, e);
        }
        private void tbGroupDefaultUsername_TextChanged(object sender, EventArgs e)
        {
            tbGroupName_TextChanged(this, e);
        }
        private void tbGroupDefaultPassword_TextChanged(object sender, EventArgs e)
        {
            tbGroupName_TextChanged(this, e);
        }

        private void tbServerHost_Enter(object sender, EventArgs e)
        {
            tbServerHost.ForeColor = Color.Black;

            if (placeholderMode)
            {
                if (placeholderModeHost)
                {
                    tbServerHost.Text = "";
                }
            }
        }
        private void tbServerHost_Leave(object sender, EventArgs e)
        {
            if (!tbServerHost.Text.Equals(placeholderServerHost) && tbServerHost.Text.Trim() != "")
                placeholderModeHost = false;
            else
                placeholderModeHost = true;

            if (placeholderMode)
            {
                if (placeholderModeHost)
                {
                    tbServerHost.ForeColor = Color.Gray;
                    tbServerHost.Text = placeholderServerHost;
                }
            }
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.Black;

            if (placeholderMode) {

                if (placeholderModePort)
                {
                    textBox1.Text = "";
                }
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(placeholderServerPort) && textBox1.Text.Trim() != "")
                placeholderModePort = false;
            else
                placeholderModePort = true;

            if (placeholderMode)
            {
                if (placeholderModePort)
                {
                    textBox1.ForeColor = Color.Gray;
                    textBox1.Text = placeholderServerPort;
                }
            }
        }
        private void tbServerUser_Enter(object sender, EventArgs e)
        {
            tbServerUser.ForeColor = Color.Black;

            if (placeholderMode)
            {
                if (placeholderModeUsername)
                {
                    tbServerUser.Text = "";
                }
            }
        }
        private void tbServerUser_Leave(object sender, EventArgs e)
        {
            if (!tbServerUser.Text.Equals(placeholderServerUsername) && tbServerUser.Text.Trim() != "")
                placeholderModeUsername = false;
            else
                placeholderModeUsername = true;

            if (placeholderMode)
            {
                if (placeholderModeUsername)
                {
                    tbServerUser.ForeColor = Color.Gray;
                    tbServerUser.Text = placeholderServerUsername;
                }
            }
        }
        private void tbServerPass_Enter(object sender, EventArgs e)
        {
            tbServerPass.ForeColor = Color.Black;

            if (placeholderMode)
            {
                if (placeholderModePassword)
                {
                    tbServerPass.Text = "";
                }
            }
        }
        private void tbServerPass_Leave(object sender, EventArgs e)
        {
            if (!tbServerPass.Text.Equals(placeholderServerPassword) && tbServerPass.Text.Trim() != "")
                placeholderModePassword = false;
            else
                placeholderModePassword = true;

            if (placeholderMode)
            {
                if (placeholderModePassword)
                {
                    tbServerPass.ForeColor = Color.Gray;
                    tbServerPass.Text = placeholderServerPassword;
                }
            }
        }

        #endregion

        #region TreeView Events

        private void tView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                currentGroup = e.Node.Text;

                GroupElement groupInfo = xmlHelper.getGroupDefaultInfo(e.Node.Text);

                tbGroupName.Text = e.Node.Text;
                tbGroupDefaultHost.Text = groupInfo.defaultHost;
                tbGroupDefaultPort.Text = groupInfo.defaultPort;
                tbGroupDefaultUsername.Text = groupInfo.defaultUsername;
                tbGroupDefaultPassword.Text = groupInfo.defaultPassword;

                bGroupAdd.Enabled = false;
                bGroupDelete.Enabled = true;

                tbServerHost.Enabled = true;
                tbServerName.Enabled = true;
                tbServerUser.Enabled = true;
                textBox1.Enabled = true;
                tbServerPass.Enabled = true;
                cbType.Enabled = true;

                placeholderMode = true;

                setPlaceholderTextBox(tbServerHost, groupInfo.defaultHost);
                setPlaceholderTextBox(textBox1, groupInfo.defaultPort);
                setPlaceholderTextBox(tbServerUser, groupInfo.defaultUsername);
                setPlaceholderTextBox(tbServerPass, groupInfo.defaultPassword);

                placeholderServerHost = groupInfo.defaultHost;
                placeholderServerPort = groupInfo.defaultPort;
                placeholderServerUsername = groupInfo.defaultUsername;
                placeholderServerPassword = groupInfo.defaultPassword;

                bServerDelete.Enabled = false;
                bServerModify.Enabled = false;

                changeGroupState(true);

                tbServerName.Text = "";

                return;
            }


            TreeNode parent = e.Node.Parent;
            TreeNode currentNode = e.Node;

            currentGroup = parent.Text;

            ServerElement currentServer = XmlHelper.getServerByName(parent.Text, currentNode.Text);

            placeholderMode = false;

            tbServerName.ForeColor = Color.Black;
            tbServerHost.ForeColor = Color.Black;
            textBox1.ForeColor = Color.Black;
            tbServerUser.ForeColor = Color.Black;
            tbServerPass.ForeColor = Color.Black;

            tbServerName.Text = currentServer.Name;
            tbServerHost.Text = currentServer.Host;
            textBox1.Text = currentServer.Port;
            tbServerUser.Text = currentServer.Username;
            tbServerPass.Text = currentServer.Password;
            cbType.SelectedIndex = Array.IndexOf(Types, types[Convert.ToInt32(currentServer.Type)]);

            bServerDelete.Enabled = true;

            changeGroupState(false);

            bGroupAdd.Enabled = false;
            bGroupModify.Enabled = false;
            bGroupDelete.Enabled = false;
        }

        private void tView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            connect();
        }

        #endregion

        #region Other Events

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if (Settings.Default.minimize && FormWindowState.Minimized == WindowState)
            {
                Hide();
                miRestore.Enabled = true;
            }
            else
            {
                lastState = WindowState.ToString();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            miRestore_Click(this, e);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                notifyIcon_MouseDoubleClick(this, e);
            }
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.O && e.Control)
            {
                bOptions_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Enter && tView.SelectedNode?.Parent != null)
            {
                connect();
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lServerUser.Text = cbType.Text == StringResources.formMain_cbType_SelectedIndexChanged_Remote_Desktop ? "[Domain\\] username" : "Username";
            tbName_TextChanged(this, e);
        }

        private void miRestore_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = lastState == "Maximized" ? FormWindowState.Maximized : FormWindowState.Normal;
            Activate();
            miRestore.Enabled = false;
        }

        private void miClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Methods

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmSysCommand)
            {
                switch (m.WParam.ToInt32())
                {
                    case IdmAbout:
                        popupAbout aboutpopup = new popupAbout();
                        aboutpopup.ShowDialog(this);
                        return;
                }
            }
            if (m.Msg == NativeMethods.WmShowMe)
            {
                miRestore_Click(new object(), new EventArgs());
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Change eye button and show/hide password
        /// </summary>
        /// <param name="bEye">link icon</param>
        /// <param name="tbPass">text box</param>
        /// <param name="state">true for hide, false for show</param>
        private void TooglePassword(PictureBox bEye, TextBox tbPass, bool state)
        {
            if (state)
            {
                bEye.Image = OtherHelper.Set(Resources.iconeyeshow, (float)0.50);
                tbPass.UseSystemPasswordChar = true;
            }
            else
            {
                bEye.Image = OtherHelper.Set(Resources.iconeyehide, (float)0.50);
                tbPass.UseSystemPasswordChar = false;
            }
        }

        /// <summary>
        /// Clear Tree View and read xml config after add groups and servers
        /// </summary>
        public void updateTreeView()
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                tView.Nodes.Clear();

                groupList = xmlHelper.getAllData();

                foreach (GroupElement group in groupList)
                {
                    string currentGroupName = group.groupName;
                    TreeNode groupNode = tView.Nodes.Add(currentGroupName);

                    if (group.servers.Count > 0)
                    {
                        foreach (ServerElement server in group.servers)
                        {
                            string currentServerName = server.Name;
                            groupNode.Nodes.Add(currentServerName);
                        }

                    }
                }
            }));
        }

        /// <summary>
        /// Set current text box text and color gray
        /// </summary>
        /// <param name="textBox">text box for set</param>
        /// <param name="text">text for set</param>
        private void setPlaceholderTextBox(TextBox textBox, String text)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = text;
        }

        /// <summary>
        /// Can enable or disable group textboxes
        /// </summary>
        /// <param name="state">enable or disable?</param>
        private void changeGroupState(bool state)
        {
            tbGroupName.Enabled = state;
            tbGroupDefaultHost.Enabled = state;
            tbGroupDefaultUsername.Enabled = state;
            tbGroupDefaultPassword.Enabled = state;
            tbGroupDefaultPort.Enabled = state;
        }

        /// <summary>
        /// Start launch any software 
        /// </summary>
        public void connect()
        {
            ConnectionHelper.StartConnect(tView.SelectedNode);
        }

        #endregion
    }
}