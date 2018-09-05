using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace SmallReport.Assist
{
    public static class IpAddressHelper
    {
        #region

        public static String GetLocalIp()
        {
            String[] Ips = GetLocalIpAddress();

            foreach (String ip in Ips) if (ip.StartsWith("10.80.")) return ip;
            foreach (String ip in Ips) if (ip.Contains(".")) return ip;

            return "127.0.0.1";
        }

        public static String[] GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            string[] IP = new string[addresses.Length];
            for (int i = 0; i < addresses.Length; i++) IP[i] = addresses[i].ToString();

            return IP;
        }

        public static string GetExtenalIpAddress()
        {
            String url = "http://hijoyusers.joymeng.com:8100/test/getNameByOtherIp";
            string IP = "未获取到外网ip";
            try
            {
                //从网址中获取本机ip数据  
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.Default;
                string str = client.DownloadString(url);
                client.Dispose();

                if (!str.Equals("")) IP = str;
            }
            catch (Exception) { }

            return IP;
        }
        #endregion
    }
}
