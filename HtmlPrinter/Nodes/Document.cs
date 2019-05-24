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


        public void Render(Surface surface)
        {
            foreach(Node node in childNodes)
            {
                node.Render(surface, surface.CurrentX, surface.Width);
            }
        }
        
        public override void Render(Surface surface, float left, float width)
        {
            throw new NotImplementedException();
        }
    }
}
