using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class HorizontalRuleNode : Node
    {
        bool cutLine = false;

        public HorizontalRuleNode(string id, bool cut = false) : base(NodeType.HorizontalRule, id)
        {
            Tag = "hr";
            cutLine = cut;
        }

        public override void Render(Surface surface, float left, float width)
        {
            Graphics g = surface.Graphics;
            Font font = surface.Font();
            float xHeight = g.MeasureString("X", font).Height;

            Pen pen = new Pen(Color.Black, 1);
            if (cutLine)
            {
                xHeight *= 3;
                pen = new Pen(Color.Red, 2);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            }
            float y = surface.CurrentY + xHeight / 2;
            g.DrawLine(pen, left, y, left + width, y);
            surface.CurrentY += xHeight;
        }




    }
}
