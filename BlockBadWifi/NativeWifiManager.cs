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
                string networkType = "";
                string authentication = "";
                string encryption = "";

                switch (e.BssType) {
                    case BssType.Infrastructure:
                        networkType = NetworkType.Infrastructure.ToString();
                        break;
                    case BssType.Independent:
                        networkType = NetworkType.Adhoc.ToString();
                        break;
                }

                switch(e.AuthAlgorithm){
                    case AuthAlgorithm.Open:
                        authentication = "Open";
                        break;
                    case AuthAlgorithm.SharedKey:
                        authentication = "Shared Key";
                        break;
                    case AuthAlgorithm.WPA:
                        authentication = "WPA-Enterprise";
                        break;
                    case AuthAlgorithm.WPA_NONE:
                        authentication = "Other";
                        break;
                    case AuthAlgorithm.WPA_PSK:
                        authentication = "WPA-Personal";
                        break;
                    case AuthAlgorithm.RSNA:
                        authentication = "WPA2-Enterprise";
                        break;
                    case AuthAlgorithm.RSNA_PSK:
                        authentication = "WPA2-Personal";
                        break;
                    case AuthAlgorithm.IHV_START:
                        authentication = "Other";
                        break;
                    case AuthAlgorithm.IHV_END:
                        authentication = "Other";
                        break;
                }

                switch (e.CipherAlgorithm)
                {
                    case CipherAlgorithm.None:
                        encryption = "None";
                        break;
                    case CipherAlgorithm.CCMP:
                        encryption = "CCMP(AES)";
                        break;
                    case CipherAlgorithm.TKIP:
                        encryption = "TKIP";
                        break;
                    case CipherAlgorithm.WEP:
                        encryption = "WEP";
                        break;
                    case CipherAlgorithm.WEP40:
                        encryption = "WEP-40";
                        break;
                    case CipherAlgorithm.WEP104:
                        encryption = "WEP-104";
                        break;
                    case CipherAlgorithm.WPA_USE_GROUP:
                        encryption = "Other";
                        break;
                    case CipherAlgorithm.RSN_USE_GROUP:
                        encryption = "Other";
                        break;
                    case CipherAlgorithm.IHV_START:
                        encryption = "Other";
                        break;
                    case CipherAlgorithm.IHV_END:
                        encryption = "Other";
                        break;
                }

                networkModels.Add(new NetworkModel()
                {
                    Ssid = Encoding.UTF8.GetString(e.Ssid.ToBytes()),
                    NetworkType = networkType,
                    Authentication = authentication,
                    Encryption = encryption
                });
            }
        }
    }
}
