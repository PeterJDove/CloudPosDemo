using System;
using System.Runtime.Serialization;

namespace Touch.CloudPos.Model
{
    /// <summary>
    /// The details of a voucher available (but yet to be fetched) when a <see cref="BasketItem"/> is committed.
    /// </summary>
    [Serializable]
    public class Voucher
    {
        /// <summary>
        /// Gets or sets the URL to use to fetch the voucher content.
        /// </summary>
        [DataMember(Name = "link")]
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the type of the content in the voucher, e.g. "html"
        /// </summary>
        [DataMember(Name = "contentType")]
        public string ContentType { get; set; }
    }
}
