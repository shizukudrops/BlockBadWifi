using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace BlockBadWifi
{
    public class StdOutputParser
    {
        public static IEnumerable<NetworkModel> ParseUserBlockList(string source)
        {
            var title = "Block list on the system (user)";

            var token = Parse.CharExcept(new char[] { '"', ','}).Except(Parse.LineEnd).AtLeastOnce();

            var header = from head in Parse.AnyChar.Until(Parse.String(title)).Token()
                         from line in Parse.Char('-').AtLeastOnce()
                         from space in Parse.WhiteSpace.Once()
                         from end in Parse.LineEnd
                         select head;

            var networkModel = from id_ssid in Parse.String("SSID:").Token()
                               from ssid in token.Contained(Parse.Char('"'), Parse.Char('"')).Token().Text()
                               from comma in Parse.Char(',')
                               from id_networktype in Parse.String("Type:").Token()
                               from networktype in token.Token().Text()

                               select new NetworkModel
                               { Ssid = ssid, NetworkType = (NetworkType)Enum.Parse(typeof(NetworkType), networktype) };

            var parser = from head in header
                         from models in networkModel.Many()
                         select models;

            try
            {
                var networkModels = parser.Parse(source);

                return networkModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
