using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using AutoPuTTY.Properties;
using AutoPuTTY.Utils;
using AutoPuTTY.Utils.Datas;
using ListBox=System.Windows.Forms.ListBox;
using MenuItem=System.Windows.Forms.MenuItem;

namespace AutoPuTTY
{
    public partial class formMain : Form
    {
        #region Conts Init

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        public const int IDM_ABOUT = 1000;
        public const int IDM_OPTIONS = 900;
        public const int MF_BYPOSITION = 0x400;
        public const int MF_SEPARATOR = 0x800;
        public const int WM_SYSCOMMAND = 0x112;
        public const int SW_RESTORE = 9;

        public static formOptions optionsform;

        public string[] types = { "PuTTY", "Remote Desktop", "VNC", "WinSCP (SCP)", "WinSCP (SFTP)", "WinSCP (FTP)" };
        public string[] _types;
        private const int tbfilterw = 145;

        private string laststate = "normal";

        private ArrayList groupList = new ArrayList();

        private xmlHelper xmlHelper;
        internal xmlHelper XmlHelper { get => xmlHelper; set => xmlHelper = value; }

        internal cryptHelper Cryptor { get; set; }

        internal otherHelper OtherHelper { get; set; }

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
            XmlHelper = new xmlHelper();
            Cryptor = new cryptHelper();
            OtherHelper = new otherHelper();


            #if DEBUG
            DateTime time = DateTime.Now;
            #endif

            //clone types array to have a sorted version
            _types = (string[])types.Clone();
            Array.Sort(_types);
            string cfgpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
            string userpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (File.Exists(cfgpath + "\\" + Settings.Default.cfgfilepath)) Settings.Default.cfgpath = cfgpath + "\\" + Settings.Default.cfgfilepath;
            else if (File.Exists(userpath + "\\" + Settings.Default.cfgfilepath)) Settings.Default.cfgpath = userpath + "\\" + Settings.Default.cfgfilepath;
            else
            {
                try
                {
                    Settings.Default.cfgpath = cfgpath + "\\" + Settings.Default.cfgfilepath;
                    xmlHelper.create();
                }
                catch (UnauthorizedAccessException)
                {
                    if (!File.Exists(userpath))
                    {
                        try
                        {
                            Settings.Default.cfgpath = userpath + "\\" + Settings.Default.cfgfilepath;
                            xmlHelper.create();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            otherHelper.Error("No really, I could not find nor write my configuration file :'(\rPlease check your user permissions.");
                            Environment.Exit(-1);
                        }
                    }
                }
            }

            if (!full) return;
            InitializeComponent();

            foreach (string type in _types)
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

            optionsform = new formOptions(this);

            IntPtr sysMenuHandle = GetSystemMenu(Handle, false);
            //It would be better to find the position at run time of the 'Close' item, but...

            InsertMenu(sysMenuHandle, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
            InsertMenu(sysMenuHandle, 6, MF_BYPOSITION, IDM_ABOUT, "About");

            notifyIcon.Visible = Settings.Default.minimize;
            notifyIcon.ContextMenu = cmSystray;

            int i = 0;
            MenuItem connectmenu = new MenuItem();
            connectmenu.Index = i;
            connectmenu.Text = "Connect";
            //connectmenu.Click += lbList_DoubleClick;
            cmList.MenuItems.Add(connectmenu);
            i++;
            MenuItem sepmenu1 = new MenuItem();
            sepmenu1.Index = i;
            sepmenu1.Text = "-";
            cmList.MenuItems.Add(sepmenu1);
            i++;
            foreach (string type in _types)
            {
                MenuItem listmenu = new MenuItem();
                listmenu.Index = i;
                listmenu.Text = type;
                string _type = Array.IndexOf(types, type).ToString();
                listmenu.Click += delegate { connect(_type); }; 
                cmList.MenuItems.Add(listmenu);
                i++;
            }
            MenuItem sepmenu2 = new MenuItem();
            sepmenu2.Index = i;
            sepmenu2.Text = "-";
            cmList.MenuItems.Add(sepmenu2);
            i++;
            MenuItem deletemenu = new MenuItem();
            deletemenu.Index = i;
            deletemenu.Text = "Delete";
            deletemenu.Click += mDelete_Click;
            cmList.MenuItems.Add(deletemenu);

            //XmlHelper.XmlToList(lbList);

            updateTreeView();

            AutoSize = false;
            MinimumSize = Size;
        }

        #endregion

        #region Button Events

        private void bModify_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode != null && tView.SelectedNode.Parent != null)
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

                string serverHostname = tbServerHost.Text.Trim();
                string serverPort = textBox1.Text.Trim();
                string serverUsername = tbServerUser.Text.Trim();
                string serverPassword = tbServerPass.Text.Trim();
                string serverType = Array.IndexOf(types, cbType.Text).ToString();

                xmlHelper.addServer(groupName, serverName, serverHostname, serverPort,
                    serverUsername, serverPassword, serverType);

                tbServerName.Text = tbServerName.Text.Trim();
                //lbList.Items.Add(tbServerName.Text);
                //lbList.SelectedItems.Clear();
                //lbList.SelectedItem = tbServerName.Text;
                bServerModify.Enabled = false;
                bServerAdd.Enabled = false;
                bServerDelete.Enabled = true;
                //BeginInvoke(new InvokeDelegate(lbList.Focus));

                updateTreeView();
            }
            else
            {
                otherHelper.Error("Enter server name and try again.");
            }
        }

        private void mDelete_Click(object sender, EventArgs e)
        {
            //if (lbList.SelectedItems.Count > 0)
            //{
            //    ArrayList _items = new ArrayList();
            //    string confirmtxt = "Are you sure you want to delete the selected item ?";
            //    if (lbList.SelectedItems.Count > 1) confirmtxt = "Are you sure you want to delete the " + lbList.SelectedItems.Count + " selected items ?";
            //    if (MessageBox.Show(confirmtxt, "Delete confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //    {
            //        remove = true;
            //        while (lbList.SelectedItems.Count > 0)
            //        {
            //            _items.Add("Name=" + xmlHelper.ParseXpathString(lbList.SelectedItem.ToString()));
            //            lbList.Items.Remove(lbList.SelectedItem);
            //        }
            //        remove = false;
            //        if (_items.Count > 0) XmlHelper.XmlDropNode(_items);
            //        tbName_TextChanged(this, e);
            //    }
            //}
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            if (tView.SelectedNode != null)
            {
                TreeNode currentNode = tView.SelectedNode;

                if (currentNode.Parent == null)
                {
                    otherHelper.Error("Please select server!");
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
            bServerEye.Image = ImageOpacity.Set(bServerEye.Image, (float)0.50);
        }
        private void bEye_MouseLeave(object sender, EventArgs e)
        {
            bServerEye.Image = (tbServerPass.UseSystemPasswordChar ? Resources.iconeyeshow : Resources.iconeyehide);
        }

        private void bOptions_Click(object sender, EventArgs e)
        {
            optionsform.ShowDialog(this);
        }

        /*
         * NEW EVENTS
         */

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
                otherHelper.Error("Enter group name and try again.");
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
                        bServerModify.Enabled = xmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) == null;
                        bServerAdd.Enabled = xmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) == null;
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

                    if (tbServerName.Text.Trim() != "" && xmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) == null)
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
                    bGroupModify.Enabled = xmlHelper.getGroupDefaultInfo(tbGroupName.Text.Trim()).Count <= 0;
                    bGroupAdd.Enabled = xmlHelper.getGroupDefaultInfo(tbGroupName.Text.Trim()).Count <= 0;
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
                if (tbGroupName.Text.Trim() != "")
                    bGroupAdd.Enabled = true;
                else
                    bGroupAdd.Enabled = false;
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

                ArrayList groupInfo = xmlHelper.getGroupDefaultInfo(e.Node.Text);
                string[] groupInfos = (string[])groupInfo[0];

                tbGroupName.Text = e.Node.Text;
                tbGroupDefaultHost.Text = groupInfos[0];
                tbGroupDefaultPort.Text = groupInfos[1];
                tbGroupDefaultUsername.Text = groupInfos[2];
                tbGroupDefaultPassword.Text = groupInfos[3];

                bGroupAdd.Enabled = false;
                bGroupDelete.Enabled = true;

                tbServerHost.Enabled = true;
                tbServerName.Enabled = true;
                tbServerUser.Enabled = true;
                textBox1.Enabled = true;
                tbServerPass.Enabled = true;
                cbType.Enabled = true;

                placeholderMode = true;

                setPlaceholderTextBox(tbServerHost, groupInfos[0]);
                setPlaceholderTextBox(textBox1, groupInfos[1]);
                setPlaceholderTextBox(tbServerUser, groupInfos[2]);
                setPlaceholderTextBox(tbServerPass, groupInfos[3]);

                placeholderServerHost = groupInfos[0];
                placeholderServerPort = groupInfos[1];
                placeholderServerUsername = groupInfos[2];
                placeholderServerPassword = groupInfos[3];

                bServerDelete.Enabled = false;
                bServerModify.Enabled = false;

                changeGroupState(true);

                tbServerName.Text = "";

                return;
            }


            TreeNode parent = e.Node.Parent;
            TreeNode currentNode = e.Node;

            currentGroup = parent.Text;

            ServerElement currentServer = xmlHelper.getServerByName(parent.Text, currentNode.Text);

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
            cbType.SelectedIndex = Array.IndexOf(_types, types[Convert.ToInt32(currentServer.Type)]);

            bServerDelete.Enabled = true;

            changeGroupState(false);

            bGroupAdd.Enabled = false;
            bGroupModify.Enabled = false;
            bGroupDelete.Enabled = false;
        }

        private void tView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            connect("-1");
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
                laststate = WindowState.ToString();
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
            else if (e.KeyCode == Keys.Enter && tView.SelectedNode != null && tView.SelectedNode.Parent != null)
            {
                connect("-1");
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lServerUser.Text = cbType.Text == "Remote Desktop" ? "[Domain\\] username" : "Username";
            tbName_TextChanged(this, e);
        }

        private void miRestore_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = laststate == "Maximized" ? FormWindowState.Maximized : FormWindowState.Normal;
            Activate();
            miRestore.Enabled = false;
        }

        private void miClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void liOptions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            optionsform.ShowDialog(this);
        }

        #endregion

        #region Methods

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                switch (m.WParam.ToInt32())
                {
                    case IDM_ABOUT:
                        popupAbout aboutpopup = new popupAbout();
                        aboutpopup.ShowDialog(this);
                        return;
                    default:
                        break;
                }
            }
            if (m.Msg == NativeMethods.WmShowMe)
            {
                miRestore_Click(new object(), new EventArgs());
            }
            base.WndProc(ref m);
        }

        private void TooglePassword(PictureBox bEye, TextBox tbPass, bool state)
        {
            if (state)
            {
                bEye.Image = ImageOpacity.Set(Resources.iconeyeshow, (float)0.50);
                tbPass.UseSystemPasswordChar = true;
            }
            else
            {
                bEye.Image = ImageOpacity.Set(Resources.iconeyehide, (float)0.50);
                tbPass.UseSystemPasswordChar = false;
            }
        }

        private void updateTreeView()
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
        }

        private void setPlaceholderTextBox(TextBox textBox, String text)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = text;
        }

        private void changeGroupState(bool state)
        {
            tbGroupName.Enabled = state;
            tbGroupDefaultHost.Enabled = state;
            tbGroupDefaultUsername.Enabled = state;
            tbGroupDefaultPassword.Enabled = state;
            tbGroupDefaultPort.Enabled = state;
        }

        public void connect(string type)
        {
            connectionHelper.StartConnect(type, tView.SelectedNode);
        }

        #endregion

        #region Nested type: InvokeDelegate

        private delegate bool InvokeDelegate();



        #endregion
    }
}