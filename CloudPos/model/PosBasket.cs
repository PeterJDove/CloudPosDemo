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
    }
}
