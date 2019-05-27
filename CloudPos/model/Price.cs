using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Touch.CloudPos.Model
{
    /// <summary>
    /// This represents the price of a <see cref="Product"/>, as charged within a transaction.
    /// </summary>
    [Serializable]
    public class Price
    {
        /// <summary>
        /// The unique ID of the price record at the Touch host.
        /// </summary>
        /// <remarks>
        /// Depending on a number of factors, different prices may apply to a given product at different 
        /// times.  Each has its own ID.  This property is probably not of much interest to the POS, 
        /// but may be used to help determine why a particular price was applied.
        /// </remarks>
        [DataMember(Name ="id")]
        public long Id { get; set; }

        /// <summary>
        /// This is the full amount of the price to be charged to the customer, including sales tax, if any.
        /// </summary>
        /// <remarks>
        /// This may differ from <see cref="NormalAmount"/>, if a special price was charged.
        /// </remarks>
        [DataMember(Name = "amount")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// This is the amount of sales tax, if any.
        /// </summary>
        [DataMember(Name = "tax")]
        public Decimal Tax { get; set; }

        /// <summary>
        /// This indicates the type of currency, e.g. "AUD" for Australian Dollars.         
        /// </summary>
        /// <remarks>
        /// <see cref="Currency"/> codes should be as defined in ISO 4217 standard.
        /// </remarks>
        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// This is the normal amount that the customer can expect to be charged for 
        /// this <see cref="Product"/>, if it had not been overridden by a special price.
        /// </summary>
        [DataMember(Name = "normalAmount")]
        public Decimal NormalAmount { get; set; }
    }
}
