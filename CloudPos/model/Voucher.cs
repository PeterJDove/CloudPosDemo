using System;
using System.Runtime.Serialization;

namespace CloudPos
{
    [Serializable]
    public class Voucher
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }
    }
}
