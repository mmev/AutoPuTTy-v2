using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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
                formMain.CurrentFormMain.changePbPingIcon(tryPingHost(_serverIP)
                    ? Resources.greed_icon
                    : Resources.red_icon);
            }).Start();

            new Thread(() =>
            {
                formMain.CurrentFormMain.changePbOpenPortIcon(checkOpenPort(_serverIP, _serverPort)
                    ? Resources.greed_icon
                    : Resources.red_icon);
            }).Start();
        }

        private static bool checkOpenPort(string serverHost, string serverPort)
        {
            Socket socket = null;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType
                    .Stream, ProtocolType.Tcp);

                IAsyncResult result = socket.BeginConnect(serverHost, Int32.Parse(serverPort), null, null);

                bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (socket.Connected)
                {
                    socket.EndConnect(result);
                    return true;
                }
                else return false;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    return false;
                }

                //An error occurred when attempting to access the socket
                return false;
            }
            finally
            {
                if (socket?.Connected ?? false)
                {
                    socket?.Disconnect(false);
                }

                socket?.Close();
            }
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
