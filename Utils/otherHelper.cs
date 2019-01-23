using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AutoPuTTY.Utils
{
    class otherHelper
    {
        public static void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
