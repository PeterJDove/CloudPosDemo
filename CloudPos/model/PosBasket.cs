using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPos
{
    public class PosBasket : IEnumerable<BasketItem>
    {
        public int Id { get; set; }
        public List<BasketItem> Items { get; set; }
        public bool Committed { get; internal set; }

        internal PosBasket()
        {
            Clear();
        }

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

        public IEnumerator<BasketItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int NumberOfVouchers
        {
            get
            {
                int count = 0;
                foreach (var item in Items)
                {
                    if (item is PurchaseBasketItem)
                        count += ((PurchaseBasketItem)item).Vouchers.Count;
                    else if (item is RefundBasketItem && ((RefundBasketItem)item).RefundVoucher != null)
                        count += 1;
                }
                return count;
            }
        }
    }
}
