using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockBadWifi
{
    public enum Encryption
    {
        Undefined = 0,
        None,
        CCMP,
        TKIP,
        WEP,
        WEP_40,
        WEP_104,
        Other
    }
}
