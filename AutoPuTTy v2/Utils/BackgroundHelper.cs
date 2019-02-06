using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using AutoPuTTY.Forms;
using AutoPuTTY.Properties;

namespace AutoPuTTY.Utils
{
    class BackgroundHelper
    {
        private string _serverIP;
        private string _serverPort;

        public BackgroundHelper(string serverIp, string serverPort)
        {
            _serverIP = serverIp;
            _serverPort = serverPort;

            new Thread(() =>
            {
                if (tryPingHost(_serverIP))
                    formMain.CurrentFormMain.changePbPingIcon(Resources.greed_icon);
                else
                    formMain.CurrentFormMain.changePbPingIcon(Resources.red_icon);


            }).Start();
        }

        private static bool tryPingHost(string serverHost)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(serverHost);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
    }
}
