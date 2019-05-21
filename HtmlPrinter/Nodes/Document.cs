using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class Document : Node
    {

        public Document() : base(NodeType.Document, "root") { }


        public void Render(Canvas canvas)
        {
            foreach(Node node in childNodes)
            {
                node.Render(canvas, 0, canvas.Width);
            }
        }
        
        public override void Render(Canvas canvas, float left, float width)
        {
            throw new NotImplementedException();
        }
    }
}
