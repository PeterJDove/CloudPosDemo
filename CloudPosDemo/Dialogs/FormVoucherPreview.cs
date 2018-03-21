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

namespace Touch.CloudPosDemo
{
    public partial class FormVoucherPreview : Form
    {
        private string _voucherTemplate;

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
                preview.DocumentText = _voucherTemplate.Replace("{content}", html);
                Show();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
