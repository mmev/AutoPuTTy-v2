using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;

namespace AutoPuTTY.Utils
{
    class otherHelper
    {
        public  bool CheckWriteAccess(string path)
        {
            bool writeAllow = false;
            bool writeDeny = false;
            DirectorySecurity accessControlList = Directory.GetAccessControl(path);
            AuthorizationRuleCollection accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write) continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    writeAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    writeDeny = true;
            }

            return writeAllow && !writeDeny;
        }

        public  string ReplaceA(string[] s, string[] r, string str)
        {
            int i = 0;
            if (s.Length > 0 && r.Length > 0 && s.Length == r.Length)
            {
                while (i < s.Length)
                {
                    str = str.Replace(s[i], r[i]);
                    i++;
                }
            }
            return str;
        }

        public  string ReplaceU(string[] s, string str)
        {
            int i = 0;
            if (s.Length > 0)
            {
                while (i < s.Length)
                {
                    str = str.Replace(s[i], Uri.EscapeDataString(s[i]).ToUpper());
                    i++;
                }
            }
            str = str.Replace("*", "%2A");
            return str;
        }

        public static void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
