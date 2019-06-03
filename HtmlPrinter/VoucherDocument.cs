using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class VoucherDocument : PrintDocument
    {
        private string _html;
        private Surface _surface;
        private PageSettings _pageSettings;

        public VoucherDocument(string printerName)
        {
            this.PrintController = new StandardPrintController(); // to suppress "Page n of n" dialogue.
            this.BeginPrint += _BeginPrint;
            this.QueryPageSettings += _QueryPageSettings;
            this.PrintPage += _PrintPage;

            _surface = new Surface();
            _surface.Width = 300;

            if (!string.IsNullOrEmpty(printerName))
                this.PrinterSettings.PrinterName = printerName;
            else
                this.PrinterSettings.PrinterName = _surface.PrinterName;

            float x = _surface.LeftMargin;
            float y = _surface.TopMargin;
            try
            {
                var dps = DefaultPageSettings;

                if (x < dps.HardMarginX)
                    x = dps.HardMarginX;

                if (y < dps.HardMarginY)
                    y = dps.HardMarginY;

                _surface.Width = dps.PaperSize.Width - (x * 2);
                x = (dps.PaperSize.Width - _surface.Width) / 2;
                dps.Margins = new Margins((int)x, (int)x, (int)y, (int)y);
            }
            catch (Exception)
            {
                // stick with Surface margins
            }
        }

        public void Print(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                _html = html;
                this.Print();
            }
        }


        private void _BeginPrint(object sender, PrintEventArgs e)
        {

        }

        private void _QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            _pageSettings = e.PageSettings;
        }

        private void _PrintPage(object sender, PrintPageEventArgs e)
        {
            PageSettings p = e.PageSettings;
            _surface.Graphics = e.Graphics;
            _surface.CurrentX = p.Margins.Left;
            _surface.CurrentY = p.Margins.Top;
            HtmlParser parser = new HtmlParser(_surface);
            Document document = parser.Parse(_html);
            document.Render(_surface);
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