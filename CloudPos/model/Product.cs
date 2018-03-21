using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CloudPos
{
    [Serializable]
    public class Product
    {
        [DataMember(Name = "productSku")]
        public string ProductSku { set; get; }

        [DataMember(Name = "ean")]
        public string Ean { set; get; }

        [DataMember(Name = "description")]
        public string Description { set; get; }

        [DataMember(Name = "price")]
        public Price Price { set; get; }

        [DataMember(Name = "items")]
        public List<SubItem> Items { set; get; }
    }

    [Serializable]
    public class SubItem
    {
        [DataMember(Name = "description")]
        public string Description { set; get; }

        [DataMember(Name = "amount")]
        public Decimal Amount { set; get; }
    }
}
