using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockBadWifi
{
    public class NetworkViewModel : NetworkModel
    {
        public string NetworkTypeString { get; set; }
        public string AuthenticationString { get; set; }
        public string EncryptionString { get; set; }

        public NetworkViewModel(NetworkModel model)
            :base(model)
        {
            NetworkTypeString = Properties.Resources.ResourceManager.GetString(model.NetworkType.ToString());
            AuthenticationString = Properties.Resources.ResourceManager.GetString(model.Authentication.ToString());
            EncryptionString = Properties.Resources.ResourceManager.GetString(model.Encryption.ToString());
        }

        public override string ToString()
        {
            return $"{Ssid} : {Authentication} / {Encryption} / {NetworkType}";
        }
    }
}
