using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockBadWifi
{
    public enum Authentication
    {
        Undefined = 0,
        Open,
        SharedKey,
        WPA_Personal,
        WPA_Enterprise,
        WPA2_Personal,
        WPA2_Enterprise,
        Other
    }
}
