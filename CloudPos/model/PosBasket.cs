using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.CloudPos.Model
{
    /// <summary>
    /// The collection of eService items within a single customer transaction: Their "basket" of goods &amp; services.
    /// </summary>
    public class PosBasket : IEnumerable<BasketItem>
    {
        /// <summary>
        /// Gets or sets the ID of the Basket
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Gets the List of BasketItems in this Basket, allowing new items to be added, and the existing items to be iterated over.
        /// </summary>
        public List<BasketItem> Items { get; private set; }

        /// <summary>
        /// Gets whether the Basket has been committed successfully.
        /// </summary>
        public bool Committed { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PosBasket"/> class,
        /// </summary>
        internal PosBasket()
        {
            Clear();
        }

        /// <summary>
        /// Removes all items from the Basket, and marks it as NOT Committed.
        /// </summary>
        internal void Clear()
        {
            Id = 0;
            Items = new List<BasketItem>();
            Committed = false;
        }

        internal string StrId
        {
            get { return Id > 0 ? Id.ToString() : ""; }
        }

        /// <summary>
        /// Implements <see cref="IEnumerable"/>&lt;<see cref="BasketItem"/>&gt;
        /// </summary>
        /// <returns>An Enumerator over the items in the basket.</returns>
        public IEnumerator<BasketItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }


        /// <summary>
        /// Implements <see cref="IEnumerable"/>
        /// </summary>
        /// <returns>An Enumerator over the items in the basket.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of Vouchers in all items (purchases and refunds) in the basket
        /// </summary>
        public int NumberOfVouchers
        {
            get
            {
                int count = 0;
                foreach (var item in Items)
                {
                    if (item is PurchaseBasketItem)
                    {
                        var purchaseItem = (PurchaseBasketItem)item;
                        if (purchaseItem.Vouchers != null)
                            count += purchaseItem.Vouchers.Count;
                    }
                    else if (item is RefundBasketItem)
                    {
                        var refundItem = (RefundBasketItem)item;
                        if (refundItem.RefundVoucher != null)
                            count += 1;
                    }
                }
                return count;
            }
        }
    }
}
