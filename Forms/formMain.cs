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

            FindSwitch(false);
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
            #if DEBUG
            Debug.WriteLine("StartUp Time :" + (DateTime.Now - time));
            #endif
        }

        #endregion

        #region Button Events

        private void bModify_Click(object sender, EventArgs e)
        {
            

            remove = true;
            //tView.Items.RemoveAt(lbList.Items.IndexOf(lbList.SelectedItem));
            remove = false;
            tbServerName.Text = tbServerName.Text.Trim();
            //lbList.Items.Add(tbServerName.Text);
            //lbList.SelectedItems.Clear();
            //lbList.SelectedItem = tbServerName.Text;
            bServerModify.Enabled = false;
            bServerAdd.Enabled = false;
            //BeginInvoke(new InvokeDelegate(lbList.Focus));

            if (filtervisible) tbFilter_Changed(new object(), new EventArgs());
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            if (tbServerName.Text.Trim() != "" && tbServerHost.Text.Trim() != "")
            {
                string file = Settings.Default.cfgpath;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(file);

                XmlElement newserver = xmldoc.CreateElement("Server");
                XmlAttribute name = xmldoc.CreateAttribute("Name");
                name.Value = tbServerName.Text.Trim();
                newserver.SetAttributeNode(name);

                if (tbServerHost.Text.Trim() != "")
                {
                    XmlElement host = xmldoc.CreateElement("Host");
                    host.InnerText = cryptHelper.Encrypt(tbServerHost.Text.Trim());
                    newserver.AppendChild(host);
                }
                if (tbServerUser.Text != "")
                {
                    XmlElement user = xmldoc.CreateElement("User");
                    user.InnerText = cryptHelper.Encrypt(tbServerUser.Text);
                    newserver.AppendChild(user);
                }
                if (tbServerPass.Text != "")
                {
                    XmlElement pass = xmldoc.CreateElement("Password");
                    pass.InnerText = cryptHelper.Encrypt(tbServerPass.Text);
                    newserver.AppendChild(pass);
                }
                if (cbType.SelectedIndex > 0)
                {
                    XmlElement type = xmldoc.CreateElement("Type");
                    type.InnerText = Array.IndexOf(types, cbType.Text).ToString();
                    newserver.AppendChild(type);
                }

                if (xmldoc.DocumentElement != null) xmldoc.DocumentElement.InsertAfter(newserver, xmldoc.DocumentElement.LastChild);

                try
                {
                    xmldoc.Save(file);
                }
                catch (UnauthorizedAccessException)
                {
                    otherHelper.Error("Could not write to configuration file :'(\rModifications will not be saved\rPlease check your user permissions.");
                }

                tbServerName.Text = tbServerName.Text.Trim();
                //lbList.Items.Add(tbServerName.Text);
                //lbList.SelectedItems.Clear();
                //lbList.SelectedItem = tbServerName.Text;
                bServerModify.Enabled = false;
                bServerAdd.Enabled = false;
                bServerDelete.Enabled = true;
                //BeginInvoke(new InvokeDelegate(lbList.Focus));
            }
            else
            {
                otherHelper.Error("No name ?\nNo hostname ??\nTry again ...");
            }

            if (filtervisible) tbFilter_Changed(new object(), new EventArgs());
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
            //if (lbList.SelectedItems.Count > 0)
            //{
            //    XmlHelper.XmlDropNode("Name=" + xmlHelper.ParseXpathString(lbList.SelectedItems[0].ToString()));
            //    remove = true;
            //    lbList.Items.Remove(lbList.SelectedItems[0].ToString());
            //    remove = false;
            //    lbList.SelectedItems.Clear();
            //    tbName_TextChanged(this, e);
            //}
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            //FindSwitch(false);
            //if (tbFilter.Text == "") return;
            //XmlHelper.XmlToList(lbList);
            //if (lbList.Items.Count > 0) lbList.SelectedIndex = 0;
        }

        // "search" form change close button image on mouse hover
        private void bClose_MouseEnter(object sender, EventArgs e)
        {
            bClose.Image = Resources.closeh;
        }

        // "search" form change close button image on mouse leave
        private void bClose_MouseLeave(object sender, EventArgs e)
        {
            bClose.Image = Resources.close;
        }

        // "search" form change close button image on mouse down
        private void bClose_MouseDown(object sender, MouseEventArgs e)
        {
            bClose.Image = Resources.closed;
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
            if (filtervisible) bClose_Click(sender, e);
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
            if (indexchanged) return;
            //modify an existing item
            //if (lbList.SelectedItem != null && tbServerName.Text.Trim() != "" && tbServerHost.Text.Trim() != "")
            //{
            //    //changed name
            //    if (tbServerName.Text != lbList.SelectedItem.ToString())
            //    {
            //        //if new name doesn't exist in list, modify or add
            //        bServerModify.Enabled = xmlHelper.XmlGetServer(tbServerName.Text.Trim()).Count > 0 ? false : true;
            //        bServerAdd.Enabled = xmlHelper.XmlGetServer(tbServerName.Text.Trim()).Count > 0 ? false : true;
            //    }
            //    //changed other stuff
            //    else
            //    {
            //        bServerModify.Enabled = true;
            //        bServerAdd.Enabled = false;
            //    }
            //}
            ////create new item
            //else
            //{
            //    bServerModify.Enabled = false;
            //    if (tbServerName.Text.Trim() != "" && tbServerHost.Text.Trim() != "" && xmlHelper.XmlGetServer(tbServerName.Text.Trim()).Count < 1) bServerAdd.Enabled = true;
            //    else bServerAdd.Enabled = false;
            //}
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

        // update "search"
        private void tbFilter_Changed(object sender, EventArgs e)
        {
            //if (filtervisible) lbList_Filter(tbFilter.Text);
        }

        // close "search" form when pressing ESC
        private void tbFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                e.Handled = true;
                bClose_Click(sender, e);
            }
        }

        // prevent the beep sound when pressing ctrl + F in the search input
        private void tbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                e.SuppressKeyPress = true;
            }
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
            tbServerHost.Text = "";
        }
        private void tbServerHost_Leave(object sender, EventArgs e)
        {
            tbServerHost.ForeColor = Color.Gray;
            tbServerHost.Text = placeholderServerHost;
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.Black;
            textBox1.Text = "";
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.Gray;
            textBox1.Text = placeholderServerPort;
        }
        private void tbServerUser_Enter(object sender, EventArgs e)
        {
            tbServerUser.ForeColor = Color.Black;
            tbServerUser.Text = "";
        }
        private void tbServerUser_Leave(object sender, EventArgs e)
        {
            tbServerUser.ForeColor = Color.Gray;
            tbServerUser.Text = placeholderServerUsername;
        }
        private void tbServerPass_Enter(object sender, EventArgs e)
        {
            tbServerPass.ForeColor = Color.Black;
            tbServerPass.Text = "";
        }
        private void tbServerPass_Leave(object sender, EventArgs e)
        {

            tbServerPass.Text = placeholderServerPassword;
        }

        #endregion

        #region ListBox Events

        //private void lbList_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        //{
        //    if (e.Index < 0) return;
        //    // Draw the background of the ListBox control for each item.
        //    e.DrawBackground();
        //    // Define the default color of the brush as black.
        //    Brush myBrush = Brushes.Black;

        //    // Draw the current item text based on the current Font 
        //    // and the custom brush settings.
        //    Rectangle bounds = e.Bounds;
        //    if (bounds.X < 1) bounds.X = 1;
        //    //MessageBox.Show(this, bounds.Top.ToString());

        //    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) myBrush = Brushes.White;
        //    e.Graphics.DrawString(lbList.Items[e.Index].ToString(), e.Font, myBrush, bounds, StringFormat.GenericDefault);

        //    // If the ListBox has focus, draw a focus rectangle around the selected item.
        //    e.DrawFocusRectangle();
        //}

        //public void lbList_IndexChanged(object sender, EventArgs e)
        //{
        //    if (filter || selectall) return;
        //    if (remove || lbList.SelectedItem == null)
        //    {
        //        if (bServerDelete.Enabled) bServerDelete.Enabled = false;
        //        return;
        //    }
        //    indexchanged = true;

        //    ArrayList server = xmlHelper.XmlGetServer(lbList.SelectedItem.ToString());

        //    tbServerName.Text = (string)server[0];
        //    tbServerHost.Text = Cryptor.Decrypt((string)server[1]);
        //    tbServerUser.Text = Cryptor.Decrypt((string)server[2]);
        //    tbServerPass.Text = Cryptor.Decrypt((string)server[3]);
        //    cbType.SelectedIndex = Array.IndexOf(_types, types[Convert.ToInt32(server[4])]);
        //    lServerUser.Text = cbType.Text == "Remote Desktop" ? "[Domain\\] username" : "Username";

        //    if (bServerAdd.Enabled) bServerAdd.Enabled = false;
        //    if (bServerModify.Enabled) bServerModify.Enabled = false;
        //    if (!bServerDelete.Enabled) bServerDelete.Enabled = true;

        //    indexchanged = false;
        //}

        //private void lbList_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button != MouseButtons.Right) return;
        //    lbList_ContextMenu();
        //}

        //private void lbList_ContextMenu_Enable(bool status)
        //{
        //    for (int i = 0; i < cmList.MenuItems.Count; i++)
        //    {
        //        cmList.MenuItems[i].Enabled = status;
        //    }
        //}

        //private void lbList_ContextMenu()
        //{
        //    lbList_ContextMenu(false);
        //}

        //private void lbList_ContextMenu(bool keyboard)
        //{
        //    if (lbList.Items.Count > 0)
        //    {
        //        if (keyboard && lbList.SelectedItems.Count > 0)
        //        {
        //            lbList_ContextMenu_Enable(true);
        //        }
        //        else
        //        {
        //            int rightindex = lbList.IndexFromPoint(lbList.PointToClient(MousePosition));
        //            if (rightindex >= 0)
        //            {
        //                lbList_ContextMenu_Enable(true);
        //                if (lbList.GetSelected(rightindex))
        //                {
        //                    lbList.SelectedIndex = rightindex;
        //                }
        //                else
        //                {
        //                    lbList.SelectedIndex = -1;
        //                    lbList.SelectedIndex = rightindex;
        //                }
        //            }
        //            else
        //            {
        //                lbList_ContextMenu_Enable(false);
        //            }
        //        }
        //    }
        //    else lbList_ContextMenu_Enable(false);

        //    IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
        //    ShowWindowAsync(hWnd, 5); // SW_SHOW

        //    int loop = 0;
        //    while (!Visible)
        //    {
        //        loop++;
        //        Thread.Sleep(100);
        //        Show();
        //        if (loop > 10)
        //        {
        //            //let's crash
        //            MessageBox.Show("Something bad happened");
        //            break;
        //        }
        //    }
        //    cmList.Show(this, PointToClient(MousePosition));
        //}

        //private void lbList_DoubleClick(object sender, EventArgs e)
        //{
        //    Connect("-1");
        //}

        //public void lbList_Filter(string s)
        //{
        //    filter = true;
        //    XmlHelper.XmlToList(lbList);
        //    ListBox.ObjectCollection itemslist = new ListBox.ObjectCollection(lbList);
        //    itemslist.AddRange(lbList.Items);
        //    lbList.Items.Clear();

        //    foreach (string item in itemslist)
        //    {
        //        string _item = item;
        //        if (!cbCase.Checked)
        //        {
        //            s = s.ToLower();
        //            _item = _item.ToLower();
        //        }

        //        /*if (!filterpopup.cbWhole.Checked)
        //        {*/
        //        if (_item.IndexOf(s) >= 0 || s == "") lbList.Items.Add(item);
        //        /*}
        //        else
        //        {
        //            if (_item == s || s == "") lbList.Items.Add(item);
        //        }*/
        //    }

        //    filter = false;
        //    lbList.SelectedIndex = lbList.Items.Count > 0 ? 0 : -1;
        //    if (lbList.Items.Count < 1) lbList_IndexChanged(new object(), new EventArgs());
        //}

        #endregion

        #region TreeView Events

        private void tView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
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

            setPlaceholderTextBox(tbServerHost, groupInfos[0]);
            setPlaceholderTextBox(textBox1, groupInfos[1]);
            setPlaceholderTextBox(tbServerUser, groupInfos[2]);
            setPlaceholderTextBox(tbServerPass, groupInfos[3]);

            placeholderServerHost = groupInfos[0];
            placeholderServerPort = groupInfos[1];
            placeholderServerUsername = groupInfos[2];
            placeholderServerPassword = groupInfos[3];
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

            tbFilter.Width = tlLeft.Width - tbFilter.Left < tbfilterw ? tlLeft.Width - tbFilter.Left : tbfilterw;
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
            if (e.KeyCode == Keys.F && e.Control)
            {
                FindSwitch(true);
            }
            else if (e.KeyCode == Keys.O && e.Control)
            {
                bOptions_Click(sender, e);
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

        private void cbCase_CheckedChanged(object sender, EventArgs e)
        {
            if (tbFilter.Text != "") tbFilter_Changed(sender, e);
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
            Debug.WriteLine("Connect : type - " + type + " " + (type != "-1" ? types[Convert.ToInt16(type)] : ""));

            //// browsing files with OpenFileDialog() fucks with CurrentDirectory, lets fix it
            //Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //if (lbList.SelectedItems == null) return;

            //if (lbList.SelectedItems.Count > 0)
            //{
            //    if (lbList.SelectedItems.Count > 5)
            //    {
            //        if (MessageBox.Show(this, "Are you sure you want to connect to the " + lbList.SelectedItems.Count + " selected items ?", "Connection confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            //    }

            //    foreach (object item in lbList.SelectedItems)
            //    {
            //        ArrayList server = xmlHelper.XmlGetServer(item.ToString());

            //        string winscpprot = "sftp://";

            //        string _host = Cryptor.Decrypt(server[1].ToString());
            //        string _user = Cryptor.Decrypt(server[2].ToString());
            //        string _pass = Cryptor.Decrypt(server[3].ToString());
            //        string _type = type == "-1" ? server[4].ToString() : type;
            //        string[] f = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            //        string[] ps = { "/", "\\\\" };
            //        string[] pr = { "\\", "\\" };

            //        switch (_type)
            //        {
            //            case "1": //RDP
            //                string[] rdpextractpath = ExtractFilePath(Settings.Default.rdpath);
            //                string rdpath = rdpextractpath[0];
            //                string rdpargs = rdpextractpath[1];

            //                if (File.Exists(rdpath))
            //                {
            //                    Mstscpw mstscpw = new Mstscpw();
            //                    string rdppass = mstscpw.encryptpw(_pass);

            //                    ArrayList arraylist = new ArrayList();
            //                    string[] size = Settings.Default.rdsize.Split('x');

            //                    string rdpout = "";
            //                    if (Settings.Default.rdfilespath != "" && OtherHelper.ReplaceA(ps, pr, Settings.Default.rdfilespath) != "\\")
            //                    {
            //                        rdpout = OtherHelper.ReplaceA(ps, pr, Settings.Default.rdfilespath + "\\");

            //                        try
            //                        {
            //                            Directory.CreateDirectory(rdpout);
            //                        }
            //                        catch
            //                        {
            //                            MessageBox.Show(this, "Output path for generated \".rdp\" connection files doesn't exist.\nFiles will be generated in the current path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                            rdpout = "";
            //                        }
            //                    }

            //                    foreach (string width in size)
            //                    {
            //                        int num;
            //                        if (Int32.TryParse(width.Trim(), out num)) arraylist.Add(width.Trim());
            //                    }

            //                    TextWriter rdpfile = new StreamWriter(rdpout + OtherHelper.ReplaceU(f, server[0].ToString()) + ".rdp");
            //                    if (Settings.Default.rdsize == "Full screen") rdpfile.WriteLine("screen mode id:i:2");
            //                    else rdpfile.WriteLine("screen mode id:i:1");
            //                    if (arraylist.Count == 2)
            //                    {
            //                        rdpfile.WriteLine("desktopwidth:i:" + arraylist[0]);
            //                        rdpfile.WriteLine("desktopheight:i:" + arraylist[1]);
            //                    }
            //                    if (_host != "") rdpfile.WriteLine("full address:s:" + _host);
            //                    if (_user != "")
            //                    {
            //                        rdpfile.WriteLine("username:s:" + _user);
            //                        if (_pass != "") rdpfile.WriteLine("password 51:b:" + rdppass);
            //                    }
            //                    if (Settings.Default.rddrives) rdpfile.WriteLine("redirectdrives:i:1");
            //                    if (Settings.Default.rdadmin) rdpfile.WriteLine("administrative session:i:1");
            //                    if (Settings.Default.rdspan) rdpfile.WriteLine("use multimon:i:1");
            //                    rdpfile.Close();

            //                    Process myProc = new Process();
            //                    myProc.StartInfo.FileName = rdpath;
            //                    myProc.StartInfo.Arguments = "\"" + rdpout + OtherHelper.ReplaceU(f, server[0].ToString()) + ".rdp\"";
            //                    if (rdpargs != "") myProc.StartInfo.Arguments += " " + rdpargs;
            //                    //MessageBox.Show(myProc.StartInfo.FileName + myProc.StartInfo.FileName.IndexOf('"').ToString() + File.Exists(myProc.StartInfo.FileName).ToString());
            //                    try
            //                    {
            //                        myProc.Start();
            //                    }
            //                    catch (System.ComponentModel.Win32Exception)
            //                    {
            //                        //user canceled
            //                    }
            //                }
            //                else
            //                {
            //                    if (MessageBox.Show(this, "Could not find file \"" + rdpath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bRDPath_Click(type);
            //                }
            //                break;
            //            case "2": //VNC
            //                string[] vncextractpath = ExtractFilePath(Settings.Default.vncpath);
            //                string vncpath = vncextractpath[0];
            //                string vncargs = vncextractpath[1];

            //                if (File.Exists(vncpath))
            //                {
            //                    string host;
            //                    string port;
            //                    string[] hostport = _host.Split(':');
            //                    int split = hostport.Length;

            //                    if (split == 2)
            //                    {
            //                        host = hostport[0];
            //                        port = hostport[1];
            //                    }
            //                    else
            //                    {
            //                        host = _host;
            //                        port = "5900";
            //                    }

            //                    string vncout = "";

            //                    if (Settings.Default.vncfilespath != "" && OtherHelper.ReplaceA(ps, pr, Settings.Default.vncfilespath) != "\\")
            //                    {
            //                        vncout = OtherHelper.ReplaceA(ps, pr, Settings.Default.vncfilespath + "\\");

            //                        try
            //                        {
            //                            Directory.CreateDirectory(vncout);
            //                        }
            //                        catch
            //                        {
            //                            MessageBox.Show(this, "Output path for generated \".vnc\" connection files doesn't exist.\nFiles will be generated in the current path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //                            vncout = "";
            //                        }
            //                    }

            //                    TextWriter vncfile = new StreamWriter(vncout + OtherHelper.ReplaceU(f, server[0].ToString()) + ".vnc");
            //                    vncfile.WriteLine("[Connection]");
            //                    if (host != "") vncfile.WriteLine("host=" + host.Trim());
            //                    if (port != "") vncfile.WriteLine("port=" + port.Trim());
            //                    if (_user != "") vncfile.WriteLine("username=" + _user);
            //                    if (_pass != "") vncfile.WriteLine("password=" + cryptVNC.EncryptPassword(_pass));
            //                    vncfile.WriteLine("[Options]");
            //                    if (Settings.Default.vncfullscreen) vncfile.WriteLine("fullscreen=1");
            //                    if (Settings.Default.vncviewonly)
            //                    {
            //                        vncfile.WriteLine("viewonly=1"); //ultravnc
            //                        vncfile.WriteLine("sendptrevents=0"); //realvnc
            //                        vncfile.WriteLine("sendkeyevents=0"); //realvnc
            //                        vncfile.WriteLine("sendcuttext=0"); //realvnc
            //                        vncfile.WriteLine("acceptcuttext=0"); //realvnc
            //                        vncfile.WriteLine("sharefiles=0"); //realvnc
            //                    }

            //                    if (_pass != "" && _pass.Length > 8) vncfile.WriteLine("protocol3.3=1"); // fuckin vnc 4.0 auth
            //                    vncfile.Close();

            //                    Process myProc = new Process();
            //                    myProc.StartInfo.FileName = Settings.Default.vncpath;
            //                    myProc.StartInfo.Arguments = "-config \"" + vncout + OtherHelper.ReplaceU(f, server[0].ToString()) + ".vnc\"";
            //                    if (vncargs != "") myProc.StartInfo.Arguments += " " + vncargs;
            //                    try
            //                    {
            //                        myProc.Start();
            //                    }
            //                    catch (System.ComponentModel.Win32Exception)
            //                    {
            //                        //user canceled
            //                    }
            //                }
            //                else
            //                {
            //                    if (MessageBox.Show(this, "Could not find file \"" + vncpath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bVNCPath_Click(type);
            //                }
            //                break;
            //            case "3": //WinSCP (SCP)
            //                winscpprot = "scp://";
            //                goto case "4";
            //            case "4": //WinSCP (SFTP)
            //                string[] winscpextractpath = ExtractFilePath(Settings.Default.winscppath);
            //                string winscppath = winscpextractpath[0];
            //                string winscpargs = winscpextractpath[1];

            //                if (File.Exists(winscppath))
            //                {
            //                    string host;
            //                    string port;
            //                    string[] hostport = _host.Split(':');
            //                    int split = hostport.Length;

            //                    if (split == 2)
            //                    {
            //                        host = hostport[0];
            //                        port = hostport[1];
            //                    }
            //                    else
            //                    {
            //                        host = _host;
            //                        port = "";
            //                    }

            //                    Process myProc = new Process();
            //                    myProc.StartInfo.FileName = Settings.Default.winscppath;
            //                    myProc.StartInfo.Arguments = winscpprot;
            //                    if (_user != "")
            //                    {
            //                        string[] s = { "%", " ", "+", "/", "@", "\"", ":", ";" };
            //                        _user = OtherHelper.ReplaceU(s, _user);
            //                        _pass = OtherHelper.ReplaceU(s, _pass);
            //                        myProc.StartInfo.Arguments += _user;
            //                        if (_pass != "") myProc.StartInfo.Arguments += ":" + _pass;
            //                        myProc.StartInfo.Arguments += "@";
            //                    }
            //                    if (host != "") myProc.StartInfo.Arguments += HttpUtility.UrlEncode(host);
            //                    if (port != "") myProc.StartInfo.Arguments += ":" + port;
            //                    if (winscpprot == "ftp://") myProc.StartInfo.Arguments += " /passive=" + (Settings.Default.winscppassive ? "on" : "off");
            //                    if (Settings.Default.winscpkey && Settings.Default.winscpkeyfile != "") myProc.StartInfo.Arguments += " /privatekey=\"" + Settings.Default.winscpkeyfile + "\"";
            //                    Debug.WriteLine(myProc.StartInfo.Arguments);
            //                    if (winscpargs != "") myProc.StartInfo.Arguments += " " + winscpargs;
            //                    try
            //                    {
            //                        myProc.Start();
            //                    }
            //                    catch (System.ComponentModel.Win32Exception)
            //                    {
            //                        //user canceled
            //                    }
            //                }
            //                else
            //                {
            //                    if (MessageBox.Show(this, "Could not find file \"" + winscppath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bWSCPPath_Click(type);
            //                }
            //                break;
            //            case "5": //WinSCP (FTP)
            //                winscpprot = "ftp://";
            //                goto case "4";
            //            default: //PuTTY
            //                string[] puttyextractpath = ExtractFilePath(Settings.Default.puttypath);
            //                string puttypath = puttyextractpath[0];
            //                string puttyargs = puttyextractpath[1];
            //                // for some reason you only have escape \ if it's followed by "
            //                // will "fix" up to 3 \ in a password like \\\", then screw you with your maniac passwords
            //                string[] passs = { "\"", "\\\\\"", "\\\\\\\\\"", "\\\\\\\\\\\\\"", };
            //                string[] passr = { "\\\"", "\\\\\\\"", "\\\\\\\\\\\"", "\\\\\\\\\\\\\\\"", };

            //                if (File.Exists(puttypath))
            //                {
            //                    string host;
            //                    string port;
            //                    string[] hostport = _host.Split(':');
            //                    int split = hostport.Length;

            //                    if (split == 2)
            //                    {
            //                        host = hostport[0];
            //                        port = hostport[1];
            //                    }
            //                    else
            //                    {
            //                        host = _host;
            //                        port = "";
            //                    }

            //                    Process myProc = new Process();
            //                    myProc.StartInfo.FileName = Settings.Default.puttypath;
            //                    myProc.StartInfo.Arguments = "-ssh ";
            //                    if (_user != "") myProc.StartInfo.Arguments += _user + "@";
            //                    if (host != "") myProc.StartInfo.Arguments += host;
            //                    if (port != "") myProc.StartInfo.Arguments += " " + port;
            //                    if (_user != "" && _pass != "") myProc.StartInfo.Arguments += " -pw \"" + OtherHelper.ReplaceA(passs, passr, _pass) + "\"";
            //                    if (Settings.Default.puttyexecute && Settings.Default.puttycommand != "") myProc.StartInfo.Arguments += " -m \"" + Settings.Default.puttycommand + "\"";
            //                    if (Settings.Default.puttykey && Settings.Default.puttykeyfile != "") myProc.StartInfo.Arguments += " -i \"" + Settings.Default.puttykeyfile + "\"";
            //                    if (Settings.Default.puttyforward) myProc.StartInfo.Arguments += " -X";
            //                    //MessageBox.Show(this, myProc.StartInfo.Arguments);
            //                    if (puttyargs != "") myProc.StartInfo.Arguments += " " + puttyargs;
            //                    try
            //                    {
            //                        myProc.Start();
            //                    }
            //                    catch (System.ComponentModel.Win32Exception)
            //                    {
            //                        //user canceled
            //                    }
            //                }
            //                else
            //                {
            //                    if (MessageBox.Show(this, "Could not find file \"" + puttypath + "\".\nDo you want to change the configuration ?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) optionsform.bPuTTYPath_Click(type);
            //                }
            //                break;
            //        }
            //    }
            //}
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

        // toggle "search" form
        private void FindSwitch(bool status)
        {
            // reset the search input text
            if (status && !filtervisible) tbFilter.Text = "";
            // show the "search" form
            tlLeft.RowStyles[1].Height = status ? 25 : 0;
            filtervisible = status;
            // focus the filter input
            tbFilter.Focus();
            // pressed ctrl + F twice, select the search input text so we can search again over last one
            if (status && filtervisible && tbFilter.Text != "") tbFilter.SelectAll();
        }

        private void updateTreeView()
        {
            tView.Nodes.Clear();

            groupList = xmlHelper.getGroups();

            foreach (string[] group in groupList)
            {
                string currentGroupName = group[0];

                tView.Nodes.Add(currentGroupName);
            }
        }

        private void setPlaceholderTextBox(TextBox textBox, String text)
        {
            textBox.ForeColor = Color.Gray;
            textBox.Text = text;
        }

        #endregion

        #region Nested type: InvokeDelegate

        private delegate bool InvokeDelegate();



        #endregion

        
        
    }
}