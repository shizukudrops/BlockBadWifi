using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ManagedNativeWifi;

namespace BlockBadWifi
{
    class NativeWifiManager
    {
        private List<NetworkModel> networkModels = new List<NetworkModel>();

        public IEnumerable<NetworkModel> NetworkModels => networkModels;

        public Task ScanNetworks()
        {
            return NativeWifi.ScanNetworksAsync(TimeSpan.FromMilliseconds(3000));
        }

        /// <summary>
        /// Windowsが保持するwifiネットワークのリストを取得する
        /// </summary>
        public void RefreshNetworkInfomations()
        {
            networkModels.Clear();
            
            var networks = NativeWifi.EnumerateAvailableNetworks().ToList();

            networkModels.AddRange(networks.Select(e => new NetworkModel()
            {
                Ssid = Encoding.UTF8.GetString(e.Ssid.ToBytes()),
                NetworkType = ConvertBssType(e.BssType),
                Authentication = ConvertAuthAlgorithm(e.AuthAlgorithm),
                Encryption = ConvertCipherAlgorithm(e.CipherAlgorithm)
            }));

#if DEBUG
            AddTestDataOfNetworks();
#endif
        }

        NetworkType ConvertBssType(BssType type)
        {
            return type switch
            {
                BssType.Infrastructure => NetworkType.Infrastructure,
                BssType.Independent => NetworkType.Adhoc,
                _ => NetworkType.Undefined
            };
        }

        Authentication ConvertAuthAlgorithm(AuthAlgorithm algo)
        {
            return algo switch
            {
                AuthAlgorithm.Open => Authentication.Open,

                AuthAlgorithm.SharedKey => Authentication.SharedKey,

                AuthAlgorithm.WPA => Authentication.WPA_Enterprise,

                AuthAlgorithm.WPA_NONE => Authentication.Other,

                AuthAlgorithm.WPA_PSK => Authentication.WPA_Personal,

                AuthAlgorithm.RSNA => Authentication.WPA2_Enterprise,

                AuthAlgorithm.RSNA_PSK => Authentication.WPA2_Personal,

                AuthAlgorithm.IHV_START => Authentication.Other,

                AuthAlgorithm.IHV_END => Authentication.Other,

                _ => Authentication.Undefined
            };
        }

        Encryption ConvertCipherAlgorithm(CipherAlgorithm algo)
        {
            return algo switch
            {
                CipherAlgorithm.None => Encryption.None,

                CipherAlgorithm.CCMP => Encryption.CCMP,

                CipherAlgorithm.TKIP => Encryption.TKIP,

                CipherAlgorithm.WEP => Encryption.WEP,

                CipherAlgorithm.WEP40 => Encryption.WEP_40,

                CipherAlgorithm.WEP104 => Encryption.WEP_104,

                CipherAlgorithm.WPA_USE_GROUP => Encryption.Other,

                CipherAlgorithm.RSN_USE_GROUP => Encryption.Other,

                CipherAlgorithm.IHV_START => Encryption.Other,

                CipherAlgorithm.IHV_END => Encryption.Other,

                _ => Encryption.Undefined
            };
        }

        #region Test

        private void AddTestDataOfNetworks()
        {
            NetworkModel[] testData =
            {
                new NetworkModel()
                {
                    Ssid = "testtest",
                    Authentication = Authentication.SharedKey,
                    Encryption = Encryption.WEP_104,
                    NetworkType = NetworkType.Infrastructure
                },
                new NetworkModel()
                {
                    Ssid = "てすとてすと",
                    Authentication = Authentication.Other,
                    Encryption = Encryption.WEP_40
                    
                },
                new NetworkModel()
                {
                    Ssid="testtest*+=~?",
                    Encryption = Encryption.TKIP
                },
                new NetworkModel()
                {
                    Ssid="test test test",
                    Encryption = Encryption.Other,
                    NetworkType = NetworkType.Infrastructure
                },
                new NetworkModel()
                {
                    Ssid="\0\0\0\0\0\0abcabc",
                    Encryption = Encryption.Undefined,
                    NetworkType = NetworkType.Adhoc
                }
            };

            networkModels.AddRange(testData);
        }

        #endregion
    }
}
