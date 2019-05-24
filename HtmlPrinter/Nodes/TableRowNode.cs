using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class TableRowNode : Node, IEnumerable
    {
        public TableRowNode(string id) : base(NodeType.TableRow, id) { }

        public List<MinMax> MinMaxWidths(Surface surface, float width)
        {
            List<MinMax> list = new List<MinMax>();

            foreach(BlockNode block in childNodes)
            {
                list.Add(block.MinMax(surface, width));
            }
            return list;
        }


        public override void Render(Surface surface, float left, float width)
        {
            throw new NotImplementedException("Iterate through childNodes instead");
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            List<BlockNode> blocks = new List<BlockNode>();
            foreach (Node child in childNodes)
            {
                blocks.Add((BlockNode)child);
            }
            return blocks.GetEnumerator();
        }




    }
}
