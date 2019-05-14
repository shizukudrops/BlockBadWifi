using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockBadWifi.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var model1 = new NetworkModel()
            {
                Ssid = "Test",
                NetworkType = NetworkType.Infrastructure,
                Authentication = Authentication.WPA2_Personal,
                Encryption = Encryption.WEP_104
            };

            var vm1 = new NetworkViewModel(model1);

            Assert.AreEqual(vm1.NetworkTypeString, "インフラストラクチャ");
        }
    }
}
