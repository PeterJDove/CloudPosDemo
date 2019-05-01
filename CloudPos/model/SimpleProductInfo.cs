using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.CloudPos.Model
{
    /// <summary>
    /// Represents the result of a call to <see cref="API.IsSimpleProduct"/>
    /// </summary>
    /// <remarks>
    /// <para>A Simple Product is one that may be purchased without any additional data being required.</para>
    /// <para>For example, a Telco voucher IS a Simple Product, because nothing need be known except that the customer wants one.</para>
    /// <para>A Gift Card is NOT a Simple Product, because the unique Gift Card barcode must be read to activate that particular card.</para>
    /// <para>Similiarly, a money transfer is not a Simple Product, because much information is required about the transfer.</para>
    /// </remarks>
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

        /// <summary>
        /// The ID of the product being considered.
        /// </summary>
        public string ShortcutOrEan { get; private set; }

        /// <summary>
        /// Contains <see langword="true"/> if the product is known, otherwise <see langword="false"/>
        /// </summary>
        public bool Exists { get; private set; }

        /// <summary>
        /// Assuming <see cref="Exists"/> equals <see langword="true"/>, this contains <see langword="true"/> if it is a Simple Product.
        /// </summary>
        public bool Simple { get; private set; }
    }
}
