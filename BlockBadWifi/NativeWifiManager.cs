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
        private List<AvailableNetworkPack> networks;
        private List<NetworkModel> networkModels = new List<NetworkModel>();

        public IEnumerable<NetworkModel> NetworkModels
        {
            get => networkModels;
        }

        public Task ScanNetworks()
        {
            return NativeWifi.ScanNetworksAsync(TimeSpan.FromMilliseconds(3000));
        }

        public void RefreshNetworkInfomations()
        {
            networkModels.Clear();
            
            networks = NativeWifi.EnumerateAvailableNetworks().ToList();         

            foreach (var e in networks)
            {
                NetworkType networkType = NetworkType.None;
                string authentication = "";
                string encryption = "";

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
        }

        string ConvertAuthAlgorithm(AuthAlgorithm algo)
        {
            switch (algo)
            {
                case AuthAlgorithm.Open:
                    return "Open";

                case AuthAlgorithm.SharedKey:
                    return "Shared Key";

                case AuthAlgorithm.WPA:
                    return "WPA-Enterprise";
                    
                case AuthAlgorithm.WPA_NONE:
                    return "Other";
                    
                case AuthAlgorithm.WPA_PSK:
                    return "WPA-Personal";
                    
                case AuthAlgorithm.RSNA:
                    return "WPA2-Enterprise";
                    
                case AuthAlgorithm.RSNA_PSK:
                    return "WPA2-Personal";
                    
                case AuthAlgorithm.IHV_START:
                    return "Other";
                    
                case AuthAlgorithm.IHV_END:
                    return "Other";
            }

            return "";
        }

        string ConvertCipherAlgorithm(CipherAlgorithm algo)
        {
            switch (algo)
            {
                case CipherAlgorithm.None:
                    return "None";
                    
                case CipherAlgorithm.CCMP:
                    return "CCMP(AES)";
                    
                case CipherAlgorithm.TKIP:
                    return "TKIP";
                    
                case CipherAlgorithm.WEP:
                    return "WEP";
                    
                case CipherAlgorithm.WEP40:
                    return "WEP-40";
                    
                case CipherAlgorithm.WEP104:
                    return "WEP-104";
                    
                case CipherAlgorithm.WPA_USE_GROUP:
                    return "Other";
                    
                case CipherAlgorithm.RSN_USE_GROUP:
                    return "Other";
                    
                case CipherAlgorithm.IHV_START:
                    return "Other";
                    
                case CipherAlgorithm.IHV_END:
                    return "Other";
            }

            return "";
        }
    }
}
