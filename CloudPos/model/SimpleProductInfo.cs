using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPos
{
    public class SimpleProductInfo
    {
        internal SimpleProductInfo(SimpleProductEvent e)
        {
            ShortcutOrEan = e.ShortcutOrEan;
            Exists = e.Exists;
            Simple = e.Simple;
        }

        internal SimpleProductInfo(string ean, bool exists, bool simple)
        {
            ShortcutOrEan = ean;
            Exists = exists;
            Simple = simple;
        }

        public string ShortcutOrEan { get; private set; }
        public bool Exists { get; private set; }
        public bool Simple { get; private set; }
    }
}
