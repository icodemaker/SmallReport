using System;
using System.Security.Cryptography;
using System.Text;

namespace SmallReport.Assist
{
    public static class CommonHelper
    {
        #region WeChat

        public static int ConvertDateTimeInt(DateTime time)
        {
            var time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var span = time - time2;
            return (int)span.TotalSeconds;
        }

        public static string Md5Hash(string input)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            return sBuilder.ToString();
        }
        #endregion
    }
}
