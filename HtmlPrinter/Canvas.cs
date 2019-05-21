using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    public class Canvas
    {
        public Graphics Graphics { get; set; }
        public float CurrentX { get; set; }
        public float CurrentY { get; set; }

        public float Width { get; set; }

        public string FontFamily = "Arial";
        public float FontSize = 9;
        public FontStyle FontStyle = FontStyle.Regular;

        public Font BaseFont
        {
            get { return new Font(FontFamily, FontSize, FontStyle); }
        }

        public Canvas(Graphics g)
        {
            Graphics = g;
            CurrentX = 0;
            CurrentY = 0;
        }




    }
}
