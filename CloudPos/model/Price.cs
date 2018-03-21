using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CloudPos
{
    [Serializable]
    public class Price
    {
        [DataMember(Name ="id")]
        public long Id { get; set; }

        [DataMember(Name = "amount")]
        public Decimal Amount { get; set; }

        [DataMember(Name = "tax")]
        public Decimal Tax { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "normalAmount")]
        public Decimal NormalAmount { get; set; }
    }
}
