using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockBadWifi
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> collection)
        {
            foreach(var e in collection)
            {
                source.Add(e);
            }
        }

        public static void Replace<T>(this ObservableCollection<T> source, IEnumerable<T> collection)
        {
            source.Clear();
            source.AddRange(collection);
        }
    }
}
