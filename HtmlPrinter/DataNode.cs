using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class DataNode : Node
    {
        public string Data { get; set; }

        public DataNode(string data) : base(NodeType.Data)
        {
            Data = data;
        }

    }
}
