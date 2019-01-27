using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoPuTTY
{
    internal class NativeMethods
    {
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        public static readonly int WmShowMe = RegisterWindowMessage("WM_SHOWME");
    }

    static class app
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static readonly Mutex Mutex = new Mutex(true, "Local\\AutoPuTTY");
        [STAThread]
        static void Main()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                formMain formMain = new formMain(false);
                string password = formMain.XmlHelper.configGet("password");

                if (password.Trim() != "")
                {
                    popupPassword popupPassword = new popupPassword(password);
                    Application.Run(popupPassword);
                    if (!popupPassword.auth) return;
                }

                Application.Run(new formMain(true));
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage((IntPtr) 0xffff, NativeMethods.WmShowMe, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}