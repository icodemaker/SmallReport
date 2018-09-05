using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmallReport.Assist;
using SmallReport.Assist.WeChat;
using SmallReport.Service;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            var ip = IpAddressHelper.GetExtenalIpAddress();
        }
    }
}
