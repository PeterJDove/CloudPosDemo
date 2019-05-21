using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    public class Printer
    {
        public string PrinterName { get; set; }

        public Printer()
        {
            PrinterName = string.Empty;
        }

        public Printer(string name)
        {
            PrinterName = name;
        }


        public void Print(string html)
        {
            var voucherDocument = new VoucherDocument(PrinterName);
            voucherDocument.Print(html);
        }
                

        public void Print(string html, Canvas canvas)
        {
            HtmlParser parser = new HtmlParser(canvas);
            Document document = parser.Parse(html);
            document.Render(canvas);
        }


    }
}
