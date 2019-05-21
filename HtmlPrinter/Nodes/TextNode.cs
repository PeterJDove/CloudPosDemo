using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class TextNode : Node
    {
        public string Text { get; set; }

        public TextNode(string text) : base(NodeType.Text, null)
        {
            Text = text;
        }

        public char[] Chars
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                    return new char[] { };
                else
                    return Text.ToCharArray();
            }
        }

    }
}
