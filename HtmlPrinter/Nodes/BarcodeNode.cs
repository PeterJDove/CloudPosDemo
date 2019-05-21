using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class BarcodeNode : ImageNode
    {




        public BarcodeNode(string id) : base(id)
        {
            Type = NodeType.Barcode; // override NodeType.Image
        }




    }
}
