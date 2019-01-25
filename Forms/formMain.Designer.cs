namespace AutoPuTTY
{
    partial class formMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
            this.tbServerPass = new System.Windows.Forms.TextBox();
            this.pConfig = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.lPort = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bGroupDelete = new System.Windows.Forms.Button();
            this.bGroupAdd = new System.Windows.Forms.Button();
            this.bGroupModify = new System.Windows.Forms.Button();
            this.bGroupEye = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbGroupDefaultPassword = new System.Windows.Forms.TextBox();
            this.lGroupDefaultPassword = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbGroupDefaultUsername = new System.Windows.Forms.TextBox();
            this.lGroupDefaultUsername = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbGroupDefaultPort = new System.Windows.Forms.TextBox();
            this.lGroupDefaultPort = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbGroupDefaultHost = new System.Windows.Forms.TextBox();
            this.lGroupDefaultHost = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbGroupName = new System.Windows.Forms.TextBox();
            this.lGroupName = new System.Windows.Forms.Label();
            this.bServerEye = new System.Windows.Forms.PictureBox();
            this.lServerSep5 = new System.Windows.Forms.Label();
            this.lServerSep4 = new System.Windows.Forms.Label();
            this.lServerSep3 = new System.Windows.Forms.Label();
            this.lServerSep2 = new System.Windows.Forms.Label();
            this.lServerSep1 = new System.Windows.Forms.Label();
            this.bOptions = new System.Windows.Forms.Button();
            this.lHost = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.tbServerUser = new System.Windows.Forms.TextBox();
            this.tbServerHost = new System.Windows.Forms.TextBox();
            this.tbServerName = new System.Windows.Forms.TextBox();
            this.lServerType = new System.Windows.Forms.Label();
            this.bServerDelete = new System.Windows.Forms.Button();
            this.bServerAdd = new System.Windows.Forms.Button();
            this.bServerModify = new System.Windows.Forms.Button();
            this.lServerPass = new System.Windows.Forms.Label();
            this.lServerUser = new System.Windows.Forms.Label();
            this.lServerName = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmSystray = new System.Windows.Forms.ContextMenu();
            this.miRestore = new System.Windows.Forms.MenuItem();
            this.miClose = new System.Windows.Forms.MenuItem();
            this.cmList = new System.Windows.Forms.ContextMenu();
            this.tlMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlLeft = new System.Windows.Forms.TableLayoutPanel();
            this.pFind = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCase = new System.Windows.Forms.CheckBox();
            this.bClose = new System.Windows.Forms.PictureBox();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.lbListSep = new System.Windows.Forms.TableLayoutPanel();
            this.tView = new System.Windows.Forms.TreeView();
            this.pConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bGroupEye)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bServerEye)).BeginInit();
            this.tlMain.SuspendLayout();
            this.tlLeft.SuspendLayout();
            this.pFind.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bClose)).BeginInit();
            this.lbListSep.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbServerPass
            // 
            this.tbServerPass.Enabled = false;
            this.tbServerPass.Location = new System.Drawing.Point(2, 408);
            this.tbServerPass.Name = "tbServerPass";
            this.tbServerPass.Size = new System.Drawing.Size(126, 20);
            this.tbServerPass.TabIndex = 13;
            this.tbServerPass.UseSystemPasswordChar = true;
            this.tbServerPass.TextChanged += new System.EventHandler(this.tbPass_TextChanged);
            this.tbServerPass.Enter += new System.EventHandler(this.tbServerPass_Enter);
            this.tbServerPass.Leave += new System.EventHandler(this.tbServerPass_Leave);
            // 
            // pConfig
            // 
            this.pConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pConfig.Controls.Add(this.label8);
            this.pConfig.Controls.Add(this.lPort);
            this.pConfig.Controls.Add(this.textBox1);
            this.pConfig.Controls.Add(this.label7);
            this.pConfig.Controls.Add(this.bGroupDelete);
            this.pConfig.Controls.Add(this.bGroupAdd);
            this.pConfig.Controls.Add(this.bGroupModify);
            this.pConfig.Controls.Add(this.bGroupEye);
            this.pConfig.Controls.Add(this.label6);
            this.pConfig.Controls.Add(this.tbGroupDefaultPassword);
            this.pConfig.Controls.Add(this.lGroupDefaultPassword);
            this.pConfig.Controls.Add(this.label4);
            this.pConfig.Controls.Add(this.tbGroupDefaultUsername);
            this.pConfig.Controls.Add(this.lGroupDefaultUsername);
            this.pConfig.Controls.Add(this.label5);
            this.pConfig.Controls.Add(this.tbGroupDefaultPort);
            this.pConfig.Controls.Add(this.lGroupDefaultPort);
            this.pConfig.Controls.Add(this.label3);
            this.pConfig.Controls.Add(this.tbGroupDefaultHost);
            this.pConfig.Controls.Add(this.lGroupDefaultHost);
            this.pConfig.Controls.Add(this.label2);
            this.pConfig.Controls.Add(this.tbGroupName);
            this.pConfig.Controls.Add(this.lGroupName);
            this.pConfig.Controls.Add(this.bServerEye);
            this.pConfig.Controls.Add(this.lServerSep5);
            this.pConfig.Controls.Add(this.lServerSep4);
            this.pConfig.Controls.Add(this.lServerSep3);
            this.pConfig.Controls.Add(this.lServerSep2);
            this.pConfig.Controls.Add(this.lServerSep1);
            this.pConfig.Controls.Add(this.bOptions);
            this.pConfig.Controls.Add(this.lHost);
            this.pConfig.Controls.Add(this.cbType);
            this.pConfig.Controls.Add(this.tbServerPass);
            this.pConfig.Controls.Add(this.tbServerUser);
            this.pConfig.Controls.Add(this.tbServerHost);
            this.pConfig.Controls.Add(this.tbServerName);
            this.pConfig.Controls.Add(this.lServerType);
            this.pConfig.Controls.Add(this.bServerDelete);
            this.pConfig.Controls.Add(this.bServerAdd);
            this.pConfig.Controls.Add(this.bServerModify);
            this.pConfig.Controls.Add(this.lServerPass);
            this.pConfig.Controls.Add(this.lServerUser);
            this.pConfig.Controls.Add(this.lServerName);
            this.pConfig.Location = new System.Drawing.Point(249, 0);
            this.pConfig.Margin = new System.Windows.Forms.Padding(0);
            this.pConfig.Name = "pConfig";
            this.pConfig.Size = new System.Drawing.Size(130, 532);
            this.pConfig.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(2, 327);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 2);
            this.label8.TabIndex = 44;
            // 
            // lPort
            // 
            this.lPort.AutoSize = true;
            this.lPort.Location = new System.Drawing.Point(3, 313);
            this.lPort.Name = "lPort";
            this.lPort.Size = new System.Drawing.Size(26, 13);
            this.lPort.TabIndex = 43;
            this.lPort.Text = "Port";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(2, 330);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 45;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(2, 232);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 2);
            this.label7.TabIndex = 42;
            // 
            // bGroupDelete
            // 
            this.bGroupDelete.Enabled = false;
            this.bGroupDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGroupDelete.Image = global::AutoPuTTY.Properties.Resources.icondelete;
            this.bGroupDelete.Location = new System.Drawing.Point(97, 200);
            this.bGroupDelete.Margin = new System.Windows.Forms.Padding(0);
            this.bGroupDelete.Name = "bGroupDelete";
            this.bGroupDelete.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bGroupDelete.Size = new System.Drawing.Size(32, 30);
            this.bGroupDelete.TabIndex = 41;
            this.bGroupDelete.UseCompatibleTextRendering = true;
            this.bGroupDelete.UseVisualStyleBackColor = true;
            this.bGroupDelete.Click += new System.EventHandler(this.bGroupDelete_Click);
            // 
            // bGroupAdd
            // 
            this.bGroupAdd.Enabled = false;
            this.bGroupAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGroupAdd.Image = global::AutoPuTTY.Properties.Resources.iconadd;
            this.bGroupAdd.Location = new System.Drawing.Point(33, 200);
            this.bGroupAdd.Margin = new System.Windows.Forms.Padding(0);
            this.bGroupAdd.Name = "bGroupAdd";
            this.bGroupAdd.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bGroupAdd.Size = new System.Drawing.Size(64, 30);
            this.bGroupAdd.TabIndex = 40;
            this.bGroupAdd.UseCompatibleTextRendering = true;
            this.bGroupAdd.UseVisualStyleBackColor = true;
            this.bGroupAdd.Click += new System.EventHandler(this.bGroupAdd_Click);
            // 
            // bGroupModify
            // 
            this.bGroupModify.Enabled = false;
            this.bGroupModify.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGroupModify.Image = global::AutoPuTTY.Properties.Resources.iconmodify;
            this.bGroupModify.Location = new System.Drawing.Point(1, 200);
            this.bGroupModify.Margin = new System.Windows.Forms.Padding(0);
            this.bGroupModify.Name = "bGroupModify";
            this.bGroupModify.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bGroupModify.Size = new System.Drawing.Size(32, 30);
            this.bGroupModify.TabIndex = 39;
            this.bGroupModify.UseCompatibleTextRendering = true;
            this.bGroupModify.UseVisualStyleBackColor = true;
            this.bGroupModify.Click += new System.EventHandler(this.bGroupModify_Click);
            // 
            // bGroupEye
            // 
            this.bGroupEye.BackColor = System.Drawing.Color.Transparent;
            this.bGroupEye.Image = global::AutoPuTTY.Properties.Resources.iconeyeshow;
            this.bGroupEye.Location = new System.Drawing.Point(106, 159);
            this.bGroupEye.Margin = new System.Windows.Forms.Padding(0);
            this.bGroupEye.Name = "bGroupEye";
            this.bGroupEye.Size = new System.Drawing.Size(22, 15);
            this.bGroupEye.TabIndex = 38;
            this.bGroupEye.TabStop = false;
            this.bGroupEye.Click += new System.EventHandler(this.bGroupEye_Click);
            this.bGroupEye.MouseEnter += new System.EventHandler(this.bGroupEye_MouseEnter);
            this.bGroupEye.MouseLeave += new System.EventHandler(this.bGroupEye_MouseLeave);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(2, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 2);
            this.label6.TabIndex = 35;
            // 
            // tbGroupDefaultPassword
            // 
            this.tbGroupDefaultPassword.Location = new System.Drawing.Point(2, 177);
            this.tbGroupDefaultPassword.Name = "tbGroupDefaultPassword";
            this.tbGroupDefaultPassword.Size = new System.Drawing.Size(126, 20);
            this.tbGroupDefaultPassword.TabIndex = 36;
            this.tbGroupDefaultPassword.UseSystemPasswordChar = true;
            this.tbGroupDefaultPassword.TextChanged += new System.EventHandler(this.tbGroupDefaultPassword_TextChanged);
            // 
            // lGroupDefaultPassword
            // 
            this.lGroupDefaultPassword.AutoSize = true;
            this.lGroupDefaultPassword.Location = new System.Drawing.Point(3, 160);
            this.lGroupDefaultPassword.Name = "lGroupDefaultPassword";
            this.lGroupDefaultPassword.Size = new System.Drawing.Size(90, 13);
            this.lGroupDefaultPassword.TabIndex = 34;
            this.lGroupDefaultPassword.Text = "Default Password";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(2, 135);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 2);
            this.label4.TabIndex = 32;
            // 
            // tbGroupDefaultUsername
            // 
            this.tbGroupDefaultUsername.Location = new System.Drawing.Point(2, 138);
            this.tbGroupDefaultUsername.Name = "tbGroupDefaultUsername";
            this.tbGroupDefaultUsername.Size = new System.Drawing.Size(126, 20);
            this.tbGroupDefaultUsername.TabIndex = 33;
            this.tbGroupDefaultUsername.TextChanged += new System.EventHandler(this.tbGroupDefaultUsername_TextChanged);
            // 
            // lGroupDefaultUsername
            // 
            this.lGroupDefaultUsername.AutoSize = true;
            this.lGroupDefaultUsername.Location = new System.Drawing.Point(3, 121);
            this.lGroupDefaultUsername.Name = "lGroupDefaultUsername";
            this.lGroupDefaultUsername.Size = new System.Drawing.Size(92, 13);
            this.lGroupDefaultUsername.TabIndex = 31;
            this.lGroupDefaultUsername.Text = "Default Username";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(2, 96);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 2);
            this.label5.TabIndex = 29;
            // 
            // tbGroupDefaultPort
            // 
            this.tbGroupDefaultPort.Location = new System.Drawing.Point(2, 99);
            this.tbGroupDefaultPort.Name = "tbGroupDefaultPort";
            this.tbGroupDefaultPort.Size = new System.Drawing.Size(126, 20);
            this.tbGroupDefaultPort.TabIndex = 30;
            this.tbGroupDefaultPort.TextChanged += new System.EventHandler(this.tbGroupDefaultPort_TextChanged);
            // 
            // lGroupDefaultPort
            // 
            this.lGroupDefaultPort.AutoSize = true;
            this.lGroupDefaultPort.Location = new System.Drawing.Point(3, 82);
            this.lGroupDefaultPort.Name = "lGroupDefaultPort";
            this.lGroupDefaultPort.Size = new System.Drawing.Size(63, 13);
            this.lGroupDefaultPort.TabIndex = 28;
            this.lGroupDefaultPort.Text = "Default Port";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(2, 57);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 2);
            this.label3.TabIndex = 26;
            // 
            // tbGroupDefaultHost
            // 
            this.tbGroupDefaultHost.Location = new System.Drawing.Point(2, 60);
            this.tbGroupDefaultHost.Name = "tbGroupDefaultHost";
            this.tbGroupDefaultHost.Size = new System.Drawing.Size(126, 20);
            this.tbGroupDefaultHost.TabIndex = 27;
            this.tbGroupDefaultHost.TextChanged += new System.EventHandler(this.tbGroupDefaultHost_TextChanged);
            // 
            // lGroupDefaultHost
            // 
            this.lGroupDefaultHost.AutoSize = true;
            this.lGroupDefaultHost.Location = new System.Drawing.Point(3, 43);
            this.lGroupDefaultHost.Name = "lGroupDefaultHost";
            this.lGroupDefaultHost.Size = new System.Drawing.Size(111, 13);
            this.lGroupDefaultHost.TabIndex = 25;
            this.lGroupDefaultHost.Text = "Default Hostname (IP)";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(2, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 2);
            this.label2.TabIndex = 23;
            // 
            // tbGroupName
            // 
            this.tbGroupName.Location = new System.Drawing.Point(2, 21);
            this.tbGroupName.Name = "tbGroupName";
            this.tbGroupName.Size = new System.Drawing.Size(126, 20);
            this.tbGroupName.TabIndex = 24;
            this.tbGroupName.TextChanged += new System.EventHandler(this.tbGroupName_TextChanged);
            // 
            // lGroupName
            // 
            this.lGroupName.AutoSize = true;
            this.lGroupName.Location = new System.Drawing.Point(3, 4);
            this.lGroupName.Name = "lGroupName";
            this.lGroupName.Size = new System.Drawing.Size(67, 13);
            this.lGroupName.TabIndex = 22;
            this.lGroupName.Text = "Group Name";
            // 
            // bServerEye
            // 
            this.bServerEye.BackColor = System.Drawing.Color.Transparent;
            this.bServerEye.Image = global::AutoPuTTY.Properties.Resources.iconeyeshow;
            this.bServerEye.Location = new System.Drawing.Point(106, 390);
            this.bServerEye.Margin = new System.Windows.Forms.Padding(0);
            this.bServerEye.Name = "bServerEye";
            this.bServerEye.Size = new System.Drawing.Size(22, 15);
            this.bServerEye.TabIndex = 21;
            this.bServerEye.TabStop = false;
            this.bServerEye.Click += new System.EventHandler(this.bEye_Click);
            this.bServerEye.DoubleClick += new System.EventHandler(this.bEye_Click);
            this.bServerEye.MouseEnter += new System.EventHandler(this.bEye_MouseEnter);
            this.bServerEye.MouseLeave += new System.EventHandler(this.bEye_MouseLeave);
            // 
            // lServerSep5
            // 
            this.lServerSep5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lServerSep5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lServerSep5.Location = new System.Drawing.Point(2, 444);
            this.lServerSep5.Margin = new System.Windows.Forms.Padding(0);
            this.lServerSep5.Name = "lServerSep5";
            this.lServerSep5.Size = new System.Drawing.Size(126, 2);
            this.lServerSep5.TabIndex = 15;
            // 
            // lServerSep4
            // 
            this.lServerSep4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lServerSep4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lServerSep4.Location = new System.Drawing.Point(2, 405);
            this.lServerSep4.Name = "lServerSep4";
            this.lServerSep4.Size = new System.Drawing.Size(126, 2);
            this.lServerSep4.TabIndex = 12;
            // 
            // lServerSep3
            // 
            this.lServerSep3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lServerSep3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lServerSep3.Location = new System.Drawing.Point(2, 366);
            this.lServerSep3.Margin = new System.Windows.Forms.Padding(0);
            this.lServerSep3.Name = "lServerSep3";
            this.lServerSep3.Size = new System.Drawing.Size(126, 2);
            this.lServerSep3.TabIndex = 8;
            // 
            // lServerSep2
            // 
            this.lServerSep2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lServerSep2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lServerSep2.Location = new System.Drawing.Point(2, 288);
            this.lServerSep2.Margin = new System.Windows.Forms.Padding(0);
            this.lServerSep2.Name = "lServerSep2";
            this.lServerSep2.Size = new System.Drawing.Size(126, 2);
            this.lServerSep2.TabIndex = 5;
            // 
            // lServerSep1
            // 
            this.lServerSep1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lServerSep1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lServerSep1.Location = new System.Drawing.Point(2, 249);
            this.lServerSep1.Margin = new System.Windows.Forms.Padding(0);
            this.lServerSep1.Name = "lServerSep1";
            this.lServerSep1.Size = new System.Drawing.Size(126, 2);
            this.lServerSep1.TabIndex = 2;
            // 
            // bOptions
            // 
            this.bOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bOptions.Image = global::AutoPuTTY.Properties.Resources.iconoptions;
            this.bOptions.Location = new System.Drawing.Point(1, 499);
            this.bOptions.Margin = new System.Windows.Forms.Padding(0);
            this.bOptions.Name = "bOptions";
            this.bOptions.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bOptions.Size = new System.Drawing.Size(128, 30);
            this.bOptions.TabIndex = 20;
            this.bOptions.UseCompatibleTextRendering = true;
            this.bOptions.UseVisualStyleBackColor = true;
            this.bOptions.Click += new System.EventHandler(this.bOptions_Click);
            // 
            // lHost
            // 
            this.lHost.AutoSize = true;
            this.lHost.Location = new System.Drawing.Point(3, 274);
            this.lHost.Name = "lHost";
            this.lHost.Size = new System.Drawing.Size(55, 13);
            this.lHost.TabIndex = 4;
            this.lHost.Text = "Hostname";
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.Enabled = false;
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(2, 447);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(126, 21);
            this.cbType.TabIndex = 16;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // tbServerUser
            // 
            this.tbServerUser.Enabled = false;
            this.tbServerUser.Location = new System.Drawing.Point(2, 369);
            this.tbServerUser.Name = "tbServerUser";
            this.tbServerUser.Size = new System.Drawing.Size(126, 20);
            this.tbServerUser.TabIndex = 9;
            this.tbServerUser.TextChanged += new System.EventHandler(this.tbUser_TextChanged);
            this.tbServerUser.Enter += new System.EventHandler(this.tbServerUser_Enter);
            this.tbServerUser.Leave += new System.EventHandler(this.tbServerUser_Leave);
            // 
            // tbServerHost
            // 
            this.tbServerHost.Enabled = false;
            this.tbServerHost.Location = new System.Drawing.Point(2, 291);
            this.tbServerHost.Name = "tbServerHost";
            this.tbServerHost.Size = new System.Drawing.Size(126, 20);
            this.tbServerHost.TabIndex = 6;
            this.tbServerHost.TextChanged += new System.EventHandler(this.tbHost_TextChanged);
            this.tbServerHost.Enter += new System.EventHandler(this.tbServerHost_Enter);
            this.tbServerHost.Leave += new System.EventHandler(this.tbServerHost_Leave);
            // 
            // tbServerName
            // 
            this.tbServerName.Enabled = false;
            this.tbServerName.Location = new System.Drawing.Point(2, 252);
            this.tbServerName.Name = "tbServerName";
            this.tbServerName.Size = new System.Drawing.Size(126, 20);
            this.tbServerName.TabIndex = 3;
            this.tbServerName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // lServerType
            // 
            this.lServerType.AutoSize = true;
            this.lServerType.Location = new System.Drawing.Point(3, 430);
            this.lServerType.Name = "lServerType";
            this.lServerType.Size = new System.Drawing.Size(31, 13);
            this.lServerType.TabIndex = 14;
            this.lServerType.Text = "Type";
            // 
            // bServerDelete
            // 
            this.bServerDelete.Enabled = false;
            this.bServerDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bServerDelete.Image = global::AutoPuTTY.Properties.Resources.icondelete;
            this.bServerDelete.Location = new System.Drawing.Point(97, 469);
            this.bServerDelete.Margin = new System.Windows.Forms.Padding(0);
            this.bServerDelete.Name = "bServerDelete";
            this.bServerDelete.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bServerDelete.Size = new System.Drawing.Size(32, 30);
            this.bServerDelete.TabIndex = 19;
            this.bServerDelete.UseCompatibleTextRendering = true;
            this.bServerDelete.UseVisualStyleBackColor = true;
            this.bServerDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // bServerAdd
            // 
            this.bServerAdd.Enabled = false;
            this.bServerAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bServerAdd.Image = global::AutoPuTTY.Properties.Resources.iconadd;
            this.bServerAdd.Location = new System.Drawing.Point(33, 469);
            this.bServerAdd.Margin = new System.Windows.Forms.Padding(0);
            this.bServerAdd.Name = "bServerAdd";
            this.bServerAdd.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bServerAdd.Size = new System.Drawing.Size(64, 30);
            this.bServerAdd.TabIndex = 18;
            this.bServerAdd.UseCompatibleTextRendering = true;
            this.bServerAdd.UseVisualStyleBackColor = true;
            this.bServerAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // bServerModify
            // 
            this.bServerModify.Enabled = false;
            this.bServerModify.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bServerModify.Image = global::AutoPuTTY.Properties.Resources.iconmodify;
            this.bServerModify.Location = new System.Drawing.Point(1, 469);
            this.bServerModify.Margin = new System.Windows.Forms.Padding(0);
            this.bServerModify.Name = "bServerModify";
            this.bServerModify.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.bServerModify.Size = new System.Drawing.Size(32, 30);
            this.bServerModify.TabIndex = 17;
            this.bServerModify.UseCompatibleTextRendering = true;
            this.bServerModify.UseVisualStyleBackColor = true;
            this.bServerModify.Click += new System.EventHandler(this.bModify_Click);
            // 
            // lServerPass
            // 
            this.lServerPass.AutoSize = true;
            this.lServerPass.Location = new System.Drawing.Point(3, 391);
            this.lServerPass.Name = "lServerPass";
            this.lServerPass.Size = new System.Drawing.Size(53, 13);
            this.lServerPass.TabIndex = 10;
            this.lServerPass.Text = "Password";
            // 
            // lServerUser
            // 
            this.lServerUser.AutoSize = true;
            this.lServerUser.Location = new System.Drawing.Point(3, 352);
            this.lServerUser.Name = "lServerUser";
            this.lServerUser.Size = new System.Drawing.Size(55, 13);
            this.lServerUser.TabIndex = 7;
            this.lServerUser.Text = "Username";
            // 
            // lServerName
            // 
            this.lServerName.AutoSize = true;
            this.lServerName.Location = new System.Drawing.Point(3, 235);
            this.lServerName.Name = "lServerName";
            this.lServerName.Size = new System.Drawing.Size(35, 13);
            this.lServerName.TabIndex = 0;
            this.lServerName.Text = "Name";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "AutoPuTTY";
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // cmSystray
            // 
            this.cmSystray.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miRestore,
            this.miClose});
            // 
            // miRestore
            // 
            this.miRestore.Enabled = false;
            this.miRestore.Index = 0;
            this.miRestore.Text = "Restore";
            this.miRestore.Click += new System.EventHandler(this.miRestore_Click);
            // 
            // miClose
            // 
            this.miClose.Index = 1;
            this.miClose.Text = "Close";
            this.miClose.Click += new System.EventHandler(this.miClose_Click);
            // 
            // tlMain
            // 
            this.tlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlMain.ColumnCount = 2;
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tlMain.Controls.Add(this.tlLeft, 0, 0);
            this.tlMain.Controls.Add(this.pConfig, 1, 0);
            this.tlMain.Location = new System.Drawing.Point(0, 0);
            this.tlMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlMain.Name = "tlMain";
            this.tlMain.RowCount = 1;
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMain.Size = new System.Drawing.Size(379, 532);
            this.tlMain.TabIndex = 0;
            // 
            // tlLeft
            // 
            this.tlLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlLeft.ColumnCount = 1;
            this.tlLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlLeft.Controls.Add(this.pFind, 0, 1);
            this.tlLeft.Controls.Add(this.lbListSep, 0, 0);
            this.tlLeft.Location = new System.Drawing.Point(0, 0);
            this.tlLeft.Margin = new System.Windows.Forms.Padding(0);
            this.tlLeft.Name = "tlLeft";
            this.tlLeft.RowCount = 2;
            this.tlLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlLeft.Size = new System.Drawing.Size(249, 532);
            this.tlLeft.TabIndex = 0;
            // 
            // pFind
            // 
            this.pFind.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pFind.Controls.Add(this.label1);
            this.pFind.Controls.Add(this.cbCase);
            this.pFind.Controls.Add(this.bClose);
            this.pFind.Controls.Add(this.tbFilter);
            this.pFind.Location = new System.Drawing.Point(0, 507);
            this.pFind.Margin = new System.Windows.Forms.Padding(0);
            this.pFind.Name = "pFind";
            this.pFind.Size = new System.Drawing.Size(249, 25);
            this.pFind.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 1);
            this.label1.TabIndex = 8;
            // 
            // cbCase
            // 
            this.cbCase.AutoSize = true;
            this.cbCase.Location = new System.Drawing.Point(174, 5);
            this.cbCase.Name = "cbCase";
            this.cbCase.Size = new System.Drawing.Size(82, 17);
            this.cbCase.TabIndex = 7;
            this.cbCase.Text = "Match case";
            this.cbCase.UseVisualStyleBackColor = true;
            this.cbCase.CheckedChanged += new System.EventHandler(this.cbCase_CheckedChanged);
            // 
            // bClose
            // 
            this.bClose.BackColor = System.Drawing.Color.Transparent;
            this.bClose.Image = global::AutoPuTTY.Properties.Resources.close;
            this.bClose.Location = new System.Drawing.Point(2, 3);
            this.bClose.Margin = new System.Windows.Forms.Padding(0);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(20, 20);
            this.bClose.TabIndex = 6;
            this.bClose.TabStop = false;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            this.bClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bClose_MouseDown);
            this.bClose.MouseEnter += new System.EventHandler(this.bClose_MouseEnter);
            this.bClose.MouseLeave += new System.EventHandler(this.bClose_MouseLeave);
            // 
            // tbFilter
            // 
            this.tbFilter.Location = new System.Drawing.Point(24, 3);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(107, 20);
            this.tbFilter.TabIndex = 4;
            this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_Changed);
            this.tbFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbFilter_KeyDown);
            this.tbFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFilter_KeyPress);
            // 
            // lbListSep
            // 
            this.lbListSep.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbListSep.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lbListSep.ColumnCount = 2;
            this.lbListSep.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lbListSep.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.lbListSep.Controls.Add(this.tView, 0, 0);
            this.lbListSep.Location = new System.Drawing.Point(0, 0);
            this.lbListSep.Margin = new System.Windows.Forms.Padding(0);
            this.lbListSep.Name = "lbListSep";
            this.lbListSep.RowCount = 1;
            this.lbListSep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lbListSep.Size = new System.Drawing.Size(249, 507);
            this.lbListSep.TabIndex = 2;
            // 
            // tView
            // 
            this.tView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tView.FullRowSelect = true;
            this.tView.HideSelection = false;
            this.tView.Location = new System.Drawing.Point(0, 0);
            this.tView.Margin = new System.Windows.Forms.Padding(0);
            this.tView.Name = "tView";
            this.tView.ShowLines = false;
            this.tView.Size = new System.Drawing.Size(248, 507);
            this.tView.TabIndex = 0;
            this.tView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tView_NodeMouseClick);
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(379, 532);
            this.Controls.Add(this.tlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "formMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoPuTTY";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mainForm_KeyDown);
            this.Resize += new System.EventHandler(this.mainForm_Resize);
            this.pConfig.ResumeLayout(false);
            this.pConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bGroupEye)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bServerEye)).EndInit();
            this.tlMain.ResumeLayout(false);
            this.tlLeft.ResumeLayout(false);
            this.pFind.ResumeLayout(false);
            this.pFind.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bClose)).EndInit();
            this.lbListSep.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbServerPass;
        private System.Windows.Forms.Panel pConfig;
        private System.Windows.Forms.Button bServerAdd;
        private System.Windows.Forms.Button bServerDelete;
        private System.Windows.Forms.Button bServerModify;
        private System.Windows.Forms.Label lServerName;
        private System.Windows.Forms.TextBox tbServerName;
        private System.Windows.Forms.TextBox tbServerHost;
        private System.Windows.Forms.TextBox tbServerUser;
        private System.Windows.Forms.Label lServerUser;
        private System.Windows.Forms.Label lServerPass;
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu cmSystray;
        private System.Windows.Forms.MenuItem miRestore;
        private System.Windows.Forms.MenuItem miClose;
        private System.Windows.Forms.Label lServerType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.ContextMenu cmList;
        private System.Windows.Forms.Label lHost;
        private System.Windows.Forms.Button bOptions;
        private System.Windows.Forms.Label lServerSep5;
        private System.Windows.Forms.Label lServerSep4;
        private System.Windows.Forms.Label lServerSep3;
        private System.Windows.Forms.Label lServerSep2;
        private System.Windows.Forms.Label lServerSep1;
        private System.Windows.Forms.TableLayoutPanel tlMain;
        private System.Windows.Forms.TableLayoutPanel tlLeft;
        private System.Windows.Forms.Panel pFind;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.PictureBox bClose;
        private System.Windows.Forms.CheckBox cbCase;
        private System.Windows.Forms.TableLayoutPanel lbListSep;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox bServerEye;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbGroupName;
        private System.Windows.Forms.Label lGroupName;
        private System.Windows.Forms.Button bGroupDelete;
        private System.Windows.Forms.Button bGroupAdd;
        private System.Windows.Forms.Button bGroupModify;
        private System.Windows.Forms.PictureBox bGroupEye;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbGroupDefaultPassword;
        private System.Windows.Forms.Label lGroupDefaultPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbGroupDefaultUsername;
        private System.Windows.Forms.Label lGroupDefaultUsername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbGroupDefaultPort;
        private System.Windows.Forms.Label lGroupDefaultPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbGroupDefaultHost;
        private System.Windows.Forms.Label lGroupDefaultHost;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TreeView tView;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lPort;
        private System.Windows.Forms.TextBox textBox1;
    }
}

