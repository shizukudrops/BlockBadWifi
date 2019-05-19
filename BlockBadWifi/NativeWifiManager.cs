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

        public IEnumerable<NetworkModel> NetworkModels
        {
            get => networkModels;
        }

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

            foreach (var e in networks)
            {
                var networkType = NetworkType.Undefined;
                var authentication = Authentication.Undefined;
                var encryption = Encryption.Undefined;

                switch (e.BssType) {
                    case BssType.Infrastructure:
                        networkType = NetworkType.Infrastructure;
                        break;
                    case BssType.Independent:
                        networkType = NetworkType.Adhoc;
                        break;
                }

                authentication = ConvertAuthAlgorithm(e.AuthAlgorithm);
                encryption = ConvertCipherAlgorithm(e.CipherAlgorithm);

                networkModels.Add(new NetworkModel()
                {
                    Ssid = Encoding.UTF8.GetString(e.Ssid.ToBytes()),
                    NetworkType = networkType,
                    Authentication = authentication,
                    Encryption = encryption
                });
            }

#if DEBUG
            AddTestDataOfNetworks();
#endif
        }

        Authentication ConvertAuthAlgorithm(AuthAlgorithm algo)
        {
            switch (algo)
            {
                case AuthAlgorithm.Open:
                    return Authentication.Open;

                case AuthAlgorithm.SharedKey:
                    return Authentication.SharedKey;

                case AuthAlgorithm.WPA:
                    return Authentication.WPA_Enterprise;
                    
                case AuthAlgorithm.WPA_NONE:
                    return Authentication.Other;
                    
                case AuthAlgorithm.WPA_PSK:
                    return Authentication.WPA_Personal;
                    
                case AuthAlgorithm.RSNA:
                    return Authentication.WPA2_Enterprise;
                    
                case AuthAlgorithm.RSNA_PSK:
                    return Authentication.WPA2_Personal;
                    
                case AuthAlgorithm.IHV_START:
                    return Authentication.Other;
                    
                case AuthAlgorithm.IHV_END:
                    return Authentication.Other;
            }

            return Authentication.Undefined;
        }

        Encryption ConvertCipherAlgorithm(CipherAlgorithm algo)
        {
            switch (algo)
            {
                case CipherAlgorithm.None:
                    return Encryption.None;
                    
                case CipherAlgorithm.CCMP:
                    return Encryption.CCMP;
                    
                case CipherAlgorithm.TKIP:
                    return Encryption.TKIP;
                    
                case CipherAlgorithm.WEP:
                    return Encryption.WEP;
                    
                case CipherAlgorithm.WEP40:
                    return Encryption.WEP_40;
                    
                case CipherAlgorithm.WEP104:
                    return Encryption.WEP_104;
                    
                case CipherAlgorithm.WPA_USE_GROUP:
                    return Encryption.Other;
                    
                case CipherAlgorithm.RSN_USE_GROUP:
                    return Encryption.Other;
                    
                case CipherAlgorithm.IHV_START:
                    return Encryption.Other;
                    
                case CipherAlgorithm.IHV_END:
                    return Encryption.Other;
            }

            return Encryption.Undefined;
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
                    Encryption = Encryption.Other
                },
                new NetworkModel()
                {
                    Ssid="\0\0\0\0\0\0",
                    Encryption = Encryption.Undefined,
                    NetworkType = NetworkType.Adhoc
                }
            };

            networkModels.AddRange(testData);
        }

        #endregion
    }
}
