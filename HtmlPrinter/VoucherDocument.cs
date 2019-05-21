using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class VoucherDocument : PrintDocument
    {
        private string Html;
        private PageSettings PageSettings;

        public VoucherDocument(string printerName)
        {
            this.PrintController = new StandardPrintController(); // to suppress "Page n of n" dialogue.
            this.BeginPrint += _BeginPrint;
            this.QueryPageSettings += _QueryPageSettings;
            this.PrintPage += _PrintPage;

            if (!string.IsNullOrEmpty(printerName))
                this.PrinterSettings.PrinterName = printerName;

            var dps = DefaultPageSettings;
            float left = dps.HardMarginX;
            float right = left;
            float top = dps.HardMarginY;
            float bottom = top;

            float mm80 = MMto100(80);
            if (dps.PaperSize.Width > mm80)
            {
                left = (dps.PaperSize.Width - (mm80 - left * 2)) / 2;
                right = left;
            }
            dps.Margins = new Margins((int)left, (int)right, (int)top, (int)bottom);
        }

        public void Print(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                Html = html;
                this.Print();
            }
        }


        private void _BeginPrint(object sender, PrintEventArgs e)
        {

        }

        private void _QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            this.PageSettings = e.PageSettings;
        }

        private void _PrintPage(object sender, PrintPageEventArgs e)
        {
            PageSettings p = e.PageSettings;
            Graphics g = e.Graphics;
            Canvas canvas = new Canvas(g);
            canvas.CurrentY = p.Margins.Top;
            canvas.CurrentX = p.Margins.Left;
            canvas.Width = p.PaperSize.Width - p.Margins.Left - p.Margins.Right;

            HtmlParser parser = new HtmlParser(canvas);
            Document document = parser.Parse(this.Html);
            document.Render(canvas);
        }


        /// <summary>
        /// Converts a number of millimetres to HUNDREDTHS of an Inch.
        /// </summary>
        /// <param name="millimetres"></param>
        /// <returns>Hundredths of an Inch</returns>
        private int MMto100(double millimetres)
        {
            return (int)Math.Round(millimetres / .254F);
        }

    }
}