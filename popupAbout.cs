using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AutoPuTTY
{
    public partial class popupAbout : Form
    {
        #region Element Init

        public popupAbout()
        {
            InitializeComponent();
            tVersion.Text = Properties.Settings.Default.version;
        }

        #endregion

        #region Other Events

        private void liWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(liWebsite.Text);
        }

        #endregion
    }
}