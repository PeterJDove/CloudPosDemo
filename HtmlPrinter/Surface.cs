using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Tools;
using static System.Environment;


namespace Touch.HtmlPrinter
{
    public class Surface
    {
        public string PrinterName { get; set; }
        public float LeftMargin { get; set; }
        public float TopMargin { get; set; }
        public float MaxWidth { get; set; }
        private float _width = 0;

        private Dictionary<string, FontSettings> Fonts = new Dictionary<string, FontSettings>();

        public Graphics Graphics { get; set; }

        public float CurrentX { get; set; }
        public float CurrentY { get; set; }

        public Surface() // Graphics g)
        {
            Initialize();
        }

        public float Width
        {
            get
            {
                if (MaxWidth == 0 || _width < MaxWidth)
                    return _width;
                else
                    return MaxWidth;
            }
            set { _width = value; }
        }



        public Font Font(string tag = "")
        {
            if (tag != null)
                tag = tag.ToLower();

            string family = "Arial";
            float size = 9;
            FontStyle style = FontStyle.Regular;
            float f = 0;

            if (!string.IsNullOrEmpty(Fonts["base"].Family))
            {
                family = Fonts["base"].Family;
            }
            if (!string.IsNullOrEmpty(Fonts["base"].Size))
            {
                if (float.TryParse(Fonts["base"].Size, out f))
                    size = f;
            }
            if (!string.IsNullOrEmpty(Fonts["base"].Style))
            {
                if (float.TryParse(Fonts["base"].Style, out f))
                    style = (FontStyle)f;
            }

            if (Fonts.ContainsKey(tag) && tag != "base")
            {
                if (!string.IsNullOrEmpty(Fonts["base"].Family))
                {
                    family = Fonts[tag].Family;
                }
                if (!string.IsNullOrEmpty(Fonts[tag].Size))
                {
                    string s = Fonts[tag].Size;
                    if (s.StartsWith("+") && float.TryParse(s.Substring(1), out f))
                        size += f;
                    else if (s.StartsWith("-") && float.TryParse(s.Substring(1), out f))
                        size -= f;
                    else if (float.TryParse(s, out f))
                        size = f;
                }
                if (!string.IsNullOrEmpty(Fonts[tag].Style))
                {
                    string s = Fonts[tag].Style;
                    if (s.StartsWith("+") && float.TryParse(s.Substring(1), out f))
                        style &= (FontStyle)f; // AND
                    else if (s.StartsWith("-") && float.TryParse(s.Substring(1), out f))
                        style &= ~(FontStyle)f; // AND NOT
                    else if (float.TryParse(s, out f))
                        style = (FontStyle)f;
                }
            }
            return new Font(family, size, style);
        }



        private void Initialize()
        {
            IniFile ini = GetIniFile();
            string section = "GENERAL";

            PrinterName = ini.GetString(section, "printer_name", "");
            LeftMargin = (float)ini.GetDouble(section, "left_margin", 0);
            TopMargin = (float)ini.GetDouble(section, "top_margin", 0);
            MaxWidth = (float)ini.GetDouble(section, "max_width", 0);

            section = "FONTS";
            foreach (string key in new[] { "base", "h1", "h2", "h3", "h4", "h5", "fixed" })
            {
                var prefix = key + "_";
                if (key == "base")
                    prefix = "";

                Fonts.Add(key, new FontSettings()
                {
                    Family = ini.GetString(section, prefix + "family"),
                    Size = ini.GetString(section, prefix + "size"),
                    Style = ini.GetString(section, prefix + "style"),
                });
            }
        }





        /// <summary>
        /// Returns a references to the singleton INI file used to save printer settings.
        /// </summary>
        /// <returns></returns>
        public static IniFile GetIniFile()
        {
            var appData = Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "CloudPOS");
            string iniFileName = Path.Combine(appData, "HtmlPrinter.ini");
            if (File.Exists(iniFileName) == false)
            {
                var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(iniFileName));
                if (File.Exists(source))
                {
                    Util.EnsureFolderExists(appData);
                    File.Copy(source, iniFileName);
                }
            }
            if (File.Exists(iniFileName))
                return new IniFile(iniFileName);
            else
                return null;
        }


        private class FontSettings
        {
            public string Family { get; set; }
            public string Size { get; set; }
            public string Style { get; set; }

            public FontSettings()
            {
                Family = "Arial";
                Size = "9";
                Style = "0"; // FontStyle.Regular;
            }
        }



    }
}
