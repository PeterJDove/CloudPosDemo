using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.HtmlPrinter
{
    class ImageNode : Node
    {
        public ImageNode(string id) : base(NodeType.Image, id)
        {
            Tag = "img";
        }

        public string Source { get; set; }

        public Image Image
        {
            get
            {
                try
                {
                    if (Source != null && Source.StartsWith("data"))
                    {
                        int n = Source.IndexOf("base64,");
                        if (n > 0)
                        {
                            string base64 = Source.Substring(n + 7);
                            base64 = base64.Replace("&#43;", "+"); // in case '+' is so encoded.
                            base64 = base64.Replace("&#47;", "/"); // in case '/' is so encoded.
                            base64 = base64.Replace("&#61;", "="); // in case '=' is so encoded.
                            byte[] bytes = Convert.FromBase64String(base64);
                            return System.Drawing.Image.FromStream(new MemoryStream(bytes));
                        }
                    }
                }
                catch (Exception e)
                {

                }
                return null;
            }
        }

        public override float Width
        {
            get
            {
                Image image = this.Image;
                return image != null ? Image.Width : 0;
            }
        }

        public override float Height
        {
            get
            {
                Image image = this.Image;
                return image != null ? Image.Height : 0;
            }
        }

        public override void Render(Surface surface, float left, float width)
        {
            throw new NotImplementedException();
        }



    }
}
