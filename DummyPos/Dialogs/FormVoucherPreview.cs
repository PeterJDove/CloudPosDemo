using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Touch.HtmlPrinter;

namespace Touch.DummyPos
{
    /// <summary>
    /// This modeless dialogue is used to render a <see cref="CloudPos.Model.Voucher"/>
    /// instead of printing it on a real printer.
    /// </summary>
    partial class FormVoucherPreview : Form
    {
        private string _voucherTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormVoucherPreview"/> form.
        /// </summary>
        public FormVoucherPreview()
        {
            InitializeComponent();
            _voucherTemplate = getVoucherTemplate();
        }

        private string getVoucherTemplate()
        {
            var resourceName = "Resources/VoucherPreview.html";

            using (StreamReader reader = new StreamReader(resourceName))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        /// <summary>
        /// Shows the dialogue.
        /// </summary>
        /// <param name="html">The voucher content</param>
        public void ViewHtml(string html)
        {
            //
            //  First first line of text, to use as Window Title
            //
            var regex = new Regex(@"<([p|h])[^>]*>(.*?)<\/[p|h]");
            var matches = regex.Matches(html);
            foreach (Match match in matches)
            {
                var text = match.Groups[2].Value;
                if (!string.IsNullOrEmpty(text) && text != "&nbsp;")
                {
                    regex = new Regex("<[^>]>");
                    this.Text = regex.Replace(text, "");
                    break;
                }
            }
            if (_voucherTemplate != null)
            {
                txtHTML.Text = html;
                webBrowser.DocumentText = _voucherTemplate.Replace("{content}", html);
                tabs.SelectedIndex = 0;
                Show();
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void picPrintPreview_Paint(object sender, PaintEventArgs e)
        {
            if (e.Graphics != null)
            {
                e.Graphics.Clear(Color.Linen);
                if (txtHTML.Text.Length > 0)
                {
                    Surface surface = new Surface()
                    {
                        Graphics = e.Graphics,
                        Width = picPrintPreview.Width - 5,
                    };
                    new Printer().Print(txtHTML.Text, surface);
                }
            }
        }
    }
}
