using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockBadWifi
{
    public class NetworkModel
    {
        public string Ssid { get; set; }
        public NetworkType NetworkType { get; set; }
        public Authentication Authentication { get; set; }
        public Encryption Encryption { get; set; }

        public override string ToString()
        {
            return $"{Ssid} : {Authentication} / {Encryption} / {NetworkType}";
        }
    }
}
