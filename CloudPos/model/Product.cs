using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Touch.CloudPos.Model
{
    /// <summary>
    /// A product contained within a <see cref="PurchaseBasketItem"/> or <see cref="RefundBasketItem"/>
    /// </summary>
    [Serializable]
    public class Product
    {
        /// <summary>
        /// Gets or sets the SKU (Stock Keeping Unit) ID of the <see cref="Product"/>, as defined by the retailer?
        /// </summary>
        [DataMember(Name = "productSku")]
        public string ProductSku { set; get; }

        /// <summary>
        /// Gets or sets the ID of the <see cref="Product"/>, within the Touch system.  Typically a 13-digit number, meeting the EAN-13 standard.
        /// </summary>
        [DataMember(Name = "ean")]
        public string Ean { set; get; }

        /// <summary>
        /// Gets or sets the name of the <see cref="Product"/>.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets an object representing the <see cref="Price"/> of the <see cref="Product"/> charged to the customer.
        /// </summary>
        [DataMember(Name = "price")]
        public Price Price { set; get; }

        /// <summary>
        /// Gets or sets a <see href="https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1">List</see>
        /// of <see cref="SubItem"/> objects, if any exist for this <see cref="Product"/>.
        /// </summary>
        [DataMember(Name = "items")]
        public List<SubItem> Items { set; get; }
    }

    /// <summary>
    /// For a <see cref="Product"/> that has a number of separate component parts, or sub line-items, this is one of those components. 
    /// </summary>
    [Serializable]
    public class SubItem
    {
        /// <summary>
        /// Gets or sets the ID of the <see cref="SubItem"/>.  Typically a 13-digit number, meeting the EAN-13 standard.
        /// </summary>
        [DataMember(Name = "ean")]
        public string Ean { set; get; }

        /// <summary>
        /// Gets or sets the name, or description, of the <see cref="SubItem"/>.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets the value or price of the <see cref="SubItem"/>, including sales tax, if any.
        /// </summary>
        /// <remarks>
        /// The sum of all <see cref="SubItem"/> Amounts should equal the value of <see cref="Product.Price"/>
        /// </remarks>
        [DataMember(Name = "amount")]
        public Decimal Amount { set; get; }
    }
}
