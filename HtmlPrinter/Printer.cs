using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Tools;


namespace Touch.HtmlPrinter
{
    public class Printer
    {
        public Surface Surface { get; private set; }

        public string PrinterName { get; set; }

        public Printer() : this(string.Empty) { }

        public Printer(string name)
        {
            Surface = new Surface();
            PrinterName = name;
        }


        public void Print(string html)
        {
            var voucherDocument = new VoucherDocument(PrinterName);
            voucherDocument.Print(html);
        }
                

        public void Print(string html, Surface surface)
        {
            HtmlParser parser = new HtmlParser(surface);
            Document document = parser.Parse(html);
            document.Render(surface);
        }






    }
}
