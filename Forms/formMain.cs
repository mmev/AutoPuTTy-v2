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

        public formOptions optionsform;

        public string[] types = { "PuTTY", "Remote Desktop", "VNC", "WinSCP (SCP)", "WinSCP (SFTP)", "WinSCP (FTP)" };
        public string[] _types;
        private const int tbfilterw = 145;
        private bool indexchanged;
        private bool filter;
        private bool selectall;
        private bool remove;
        private bool filtervisible;
        private double unixtime;
        private double oldunixtime;
        private string laststate = "normal";
        private string keysearch = "";

        private ArrayList groupList = new ArrayList();

        private xmlHelper xmlHelper;
        internal xmlHelper XmlHelper { get => xmlHelper; set => xmlHelper = value; }

        private cryptHelper cryptor;
        internal cryptHelper Cryptor { get => cryptor; set => cryptor = value; }

        private otherHelper otherHelper;
        internal otherHelper OtherHelper { get => otherHelper; set => otherHelper = value; }

        string placeholderServerHost = "";
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
                listmenu.Click += delegate { Connect(_type); }; 
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

        protected void lbList_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Apps) lbList_ContextMenu(true);
            if (e.KeyCode == Keys.Delete) mDelete_Click(sender, e);
            if (e.KeyCode == Keys.A && e.Control)
            {
                //for (int i = 0; i < lbList.Items.Count; i++)
                //{
                //    //change index for the first item only
                //    if (i > 0) selectall = true;
                //    lbList.SetSelected(i, true);
                //}
                //selectall = false;
            }
        }

        protected void lbList_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            unixtime = Convert.ToInt64(ts.TotalMilliseconds);

            string key = e.KeyChar.ToString();

            if (e.KeyChar == (char)Keys.Return) Connect("-1");
            else if (key.Length == 1)
            {
                if (unixtime - oldunixtime < 1000)
                {
                    keysearch = keysearch + e.KeyChar;
                }
                else
                {
                    keysearch = e.KeyChar.ToString();
                }
                //if (lbList.FindString(keysearch) >= 0)
                //{
                //    lbList.SelectedIndex = -1;
                //    lbList.SelectedIndex = lbList.FindString(keysearch);
                //}
                //else
                //{
                //    keysearch = e.KeyChar.ToString();
                //    if (lbList.FindString(keysearch) >= 0)
                //    {
                //        lbList.SelectedIndex = -1;
                //        lbList.SelectedIndex = lbList.FindString(keysearch);
                //    }
                //}
            }

            oldunixtime = unixtime;
        }

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
                        bServerModify.Enabled = xmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) != null ? false : true;
                        bServerAdd.Enabled = xmlHelper.getServerByName(currentGroup, tbServerName.Text.Trim()) != null ? false : true;
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
                    bGroupModify.Enabled = xmlHelper.getGroupDefaultInfo(tbGroupName.Text.Trim()).Count > 0 ? false : true;
                    bGroupAdd.Enabled = xmlHelper.getGroupDefaultInfo(tbGroupName.Text.Trim()).Count > 0 ? false : true;
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

            tbServerName.Text = currentServer.serverName;
            tbServerHost.Text = currentServer.serverHost;
            textBox1.Text = currentServer.serverPort;
            tbServerUser.Text = currentServer.serverUsername;
            tbServerPass.Text = currentServer.serverPassword;
            cbType.SelectedIndex = Array.IndexOf(_types, types[Convert.ToInt32(currentServer.serverType)]);

            bServerDelete.Enabled = true;

            changeGroupState(false);

            bGroupAdd.Enabled = false;
            bGroupModify.Enabled = false;
            bGroupDelete.Enabled = false;
        }

        private void tView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Connect("-1");
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
                Connect("-1");
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
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                miRestore_Click(new object(), new EventArgs());
            }
            base.WndProc(ref m);
        }

        private static string[] ExtractFilePath(string path)
        {
            //extract file path and arguments
            if (path.IndexOf("\"") == 0)
            {
                int s = path.Substring(1).IndexOf("\"");
                if (s > 0) return new string[] { path.Substring(1, s), path.Substring(s + 2).Trim() };
                return new string[] { path.Substring(1), "" };
            }
            else
            {
                int s = path.Substring(1).IndexOf(" ");
                if (s > 0) return new string[] { path.Substring(0, s + 1), path.Substring(s + 2).Trim() };
                return new string[] { path.Substring(0), "" };
            }
        }

        public void Connect(string type)
        {
            // browsing files with OpenFileDialog() fucks with CurrentDirectory, lets fix it
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (tView.SelectedNode == null || tView.SelectedNode.Parent == null) return;

            string currentGroup = tView.SelectedNode.Parent.Text;
            string currentServer = tView.SelectedNode.Text;

            ServerElement server = xmlHelper.getServerByName(currentGroup, currentServer);
            if (server == null) return;

            string winscpprot = "sftp://";

            string _host = server.serverHost + ":" + server.serverPort;
            string _user = server.serverUsername;
            string _pass = server.serverPassword;
            string _type = type == "-1" ? server.serverType : type;
            string[] f = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            string[] ps = { "/", "\\\\" };
            string[] pr = { "\\", "\\" };

            switch (_type)
            {
                case "1": //RDP
                    string[] rdpextractpath = ExtractFilePath(Settings.Default.rdpath);
                    string rdpath = Environment.ExpandEnvironmentVariables(rdpextractpath[0]);
                    string rdpargs = rdpextractpath[1];

                    if (File.Exists(rdpath))
                    {
                        Mstscpw mstscpw = new Mstscpw();
                        string rdppass = mstscpw.encryptpw(_pass);

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
                                MessageBox.Show(this, "Output path for generated \".rdp\" connection files doesn't exist.\nFiles will be generated in the current path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                rdpout = "";
                            }
                        }

                        foreach (string width in size)
                        {
                            int num;
                            if (Int32.TryParse(width.Trim(), out num)) arraylist.Add(width.Trim());
                        }

                        TextWriter rdpfile = new StreamWriter(rdpout + otherHelper.ReplaceU(f, server.serverName.ToString()) + ".rdp");
                        if (Settings.Default.rdsize == "Full screen") rdpfile.WriteLine("screen mode id:i:2");
                        else rdpfile.WriteLine("screen mode id:i:1");
                        if (arraylist.Count == 2)
                        {
                            rdpfile.WriteLine("desktopwidth:i:" + arraylist[0]);
                            rdpfile.WriteLine("desktopheight:i:" + arraylist[1]);
                        }
                        if (_host != "") rdpfile.WriteLine("full address:s:" + _host);
                        if (_user != "")
                        {
                            rdpfile.WriteLine("username:s:" + _user);
                            if (_pass != "") rdpfile.WriteLine("password 51:b:" + rdppass);
                        }
                        if (Settings.Default.rddrives) rdpfile.WriteLine("redirectdrives:i:1");
                        if (Settings.Default.rdadmin) rdpfile.WriteLine("administrative session:i:1");
                        if (Settings.Default.rdspan) rdpfile.WriteLine("use multimon:i:1");
                        rdpfile.Close();

                        Process myProc = new Process();
                        myProc.StartInfo.FileName = rdpath;
                        myProc.StartInfo.Arguments = "\"" + rdpout + otherHelper.ReplaceU(f, server.serverName.ToString()) + ".rdp\"";
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
                        if (MessageBox.Show(this, "Could not find file \"" + rdpath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bRDPath_Click(type);
                    }
                    break;
                case "2": //VNC
                    string[] vncextractpath = ExtractFilePath(Settings.Default.vncpath);
                    string vncpath = vncextractpath[0];
                    string vncargs = vncextractpath[1];

                    if (File.Exists(vncpath))
                    {
                        string host;
                        string port;
                        string[] hostport = _host.Split(':');
                        int split = hostport.Length;

                        if (split == 2)
                        {
                            host = hostport[0];
                            port = hostport[1];
                        }
                        else
                        {
                            host = _host;
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
                                MessageBox.Show(this, "Output path for generated \".vnc\" connection files doesn't exist.\nFiles will be generated in the current path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                vncout = "";
                            }
                        }

                        TextWriter vncfile = new StreamWriter(vncout + otherHelper.ReplaceU(f, server.serverName.ToString()) + ".vnc");
                        vncfile.WriteLine("[Connection]");
                        if (host != "") vncfile.WriteLine("host=" + host.Trim());
                        if (port != "") vncfile.WriteLine("port=" + port.Trim());
                        if (_user != "") vncfile.WriteLine("username=" + _user);
                        if (_pass != "") vncfile.WriteLine("password=" + cryptVNC.EncryptPassword(_pass));
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

                        if (_pass != "" && _pass.Length > 8) vncfile.WriteLine("protocol3.3=1"); // fuckin vnc 4.0 auth
                        vncfile.Close();

                        Process myProc = new Process();
                        myProc.StartInfo.FileName = Settings.Default.vncpath;
                        myProc.StartInfo.Arguments = "-config \"" + vncout + otherHelper.ReplaceU(f, server.serverName.ToString()) + ".vnc\"";
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
                        if (MessageBox.Show(this, "Could not find file \"" + vncpath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bVNCPath_Click(type);
                    }
                    break;
                case "3": //WinSCP (SCP)
                    winscpprot = "scp://";
                    goto case "4";
                case "4": //WinSCP (SFTP)
                    string[] winscpextractpath = ExtractFilePath(Settings.Default.winscppath);
                    string winscppath = winscpextractpath[0];
                    string winscpargs = winscpextractpath[1];

                    if (File.Exists(winscppath))
                    {
                        string host;
                        string port;
                        string[] hostport = _host.Split(':');
                        int split = hostport.Length;

                        if (split == 2)
                        {
                            host = hostport[0];
                            port = hostport[1];
                        }
                        else
                        {
                            host = _host;
                            port = "";
                        }

                        Process myProc = new Process();
                        myProc.StartInfo.FileName = Settings.Default.winscppath;
                        myProc.StartInfo.Arguments = winscpprot;
                        if (_user != "")
                        {
                            string[] s = { "%", " ", "+", "/", "@", "\"", ":", ";" };
                            _user = otherHelper.ReplaceU(s, _user);
                            _pass = otherHelper.ReplaceU(s, _pass);
                            myProc.StartInfo.Arguments += _user;
                            if (_pass != "") myProc.StartInfo.Arguments += ":" + _pass;
                            myProc.StartInfo.Arguments += "@";
                        }
                        if (host != "") myProc.StartInfo.Arguments += HttpUtility.UrlEncode(host);
                        if (port != "") myProc.StartInfo.Arguments += ":" + port;
                        if (winscpprot == "ftp://") myProc.StartInfo.Arguments += " /passive=" + (Settings.Default.winscppassive ? "on" : "off");
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
                        if (MessageBox.Show(this, "Could not find file \"" + winscppath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bWSCPPath_Click(type);
                    }
                    break;
                case "5": //WinSCP (FTP)
                    winscpprot = "ftp://";
                    goto case "4";
                default: //PuTTY
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
                        string[] hostport = _host.Split(':');
                        int split = hostport.Length;

                        if (split == 2)
                        {
                            host = hostport[0];
                            port = hostport[1];
                        }
                        else
                        {
                            host = _host;
                            port = "";
                        }

                        Process myProc = new Process();
                        myProc.StartInfo.FileName = Settings.Default.puttypath;
                        myProc.StartInfo.Arguments = "-ssh ";
                        if (_user != "") myProc.StartInfo.Arguments += _user + "@";
                        if (host != "") myProc.StartInfo.Arguments += host;
                        if (port != "") myProc.StartInfo.Arguments += " " + port;
                        if (_user != "" && _pass != "") myProc.StartInfo.Arguments += " -pw \"" + otherHelper.ReplaceA(passs, passr, _pass) + "\"";
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
                        if (MessageBox.Show(this, "Could not find file \"" + puttypath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bPuTTYPath_Click(type);
                    }
                    break;
            }
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
                        string currentServerName = server.serverName;
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

        #endregion

        #region Nested type: InvokeDelegate

        private delegate bool InvokeDelegate();



        #endregion
    }
}