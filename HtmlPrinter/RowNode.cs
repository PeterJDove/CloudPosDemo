using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class RowNode : Node
    {
        private List<BlockNode> _blocks = new List<BlockNode>();


        public RowNode() : base(NodeType.Row) { }

        public void Add(BlockNode block)
        {
            _blocks.Add(block);
        }


        public override void Render(Canvas canvas)
        {
            int blockCount = _blocks.Count;
            if (blockCount > 0)
            {
                float savedCurrentY = canvas.CurrentY;
                float maxCurrentY = 0;

                float left = 0;
                float width = canvas.Width / blockCount;
                foreach(BlockNode block in _blocks)
                {
                    block.SetBounds(left, width);
                    left += width;

                    canvas.CurrentY = savedCurrentY;
                    block.Render(canvas);
                    if (maxCurrentY < canvas.CurrentY)
                        maxCurrentY = canvas.CurrentY;
                }
                canvas.CurrentY = maxCurrentY;
            }
        }



    }
}
