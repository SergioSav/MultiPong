using System;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts.Utils
{
    public class NativeNetTools
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string LocalHost => "127.0.0.1";
    }
}
