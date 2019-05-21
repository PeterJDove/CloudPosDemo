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

        public override void Render(Canvas canvas, float left, float width)
        {
            Graphics g = canvas.Graphics;
            Font font = canvas.BaseFont;
            float xHeight = g.MeasureString("X", font).Height;

            Pen pen = new Pen(Color.Black, 1);
            if (cutLine)
            {
                xHeight *= 3;
                pen = new Pen(Color.Red, 2);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            }
            float y = canvas.CurrentY + xHeight / 2;
            g.DrawLine(pen, left, y, left + width, y);
            canvas.CurrentY += xHeight;
        }




    }
}
