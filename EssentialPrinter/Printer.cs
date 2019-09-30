using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EO.WebBrowser;
using EO.WinForm;
using Touch.Tools;

namespace Touch.EssentialPrinter
{
    public class Printer
    {
        private PrinterSettings _printerSettings;
        private PageSettings _pageSettings;
        private HiddenForm _hiddenForm;

        private bool _testMode = false;

        static Printer()
        {
            SetupEOBrowser();
        }

        public Printer() : this(string.Empty) { }
        
        public Printer(string name)
        {
            IniFile iniFile = new IniFile("EssentialPrinter.ini");

            _printerSettings = new PrinterSettings();
            if (string.IsNullOrEmpty(name))
                _printerSettings.PrinterName = iniFile.GetString("GENERAL", "printer_name", "");
            else
                _printerSettings.PrinterName = name;

            _pageSettings = new PageSettings(_printerSettings);
            Margins m = _pageSettings.Margins;
            _pageSettings.Margins = new Margins(
                iniFile.GetInt("GENERAL", "left_margin", m.Left),
                iniFile.GetInt("GENERAL", "right_margin", m.Right),
                iniFile.GetInt("GENERAL", "top_margin", m.Top),
                iniFile.GetInt("GENERAL", "bottom_margin", m.Bottom));

            var css = iniFile.GetString("GENERAL", "css", "EssentialPrinter.css");
            var options = new EO.WebEngine.BrowserOptions();
            options.UserStyleSheet = File.ReadAllText(css);               

            _hiddenForm = new HiddenForm();
            _hiddenForm.webView.SetOptions(options);
            _hiddenForm.webView.LoadCompleted += LoadCompleted;

            _testMode = iniFile.GetBoolean("GENERAL", "test_mode", false);
            if (_testMode)
                _hiddenForm.Show();
        }

        public void Print(string html)
        {
            Debug.Print("LoadHtmlAndWait()");
            _hiddenForm.webView.LoadHtmlAndWait(html);
        }

        private void LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            Debug.Print("Print()");
            if (!_testMode)
                _hiddenForm.webView.Print(_printerSettings, _pageSettings);
        }

        private static void SetupEOBrowser()
        {
            EO.WebBrowser.Runtime.AddLicense(
                "bZnJ4NnCoenz/hChWe3pAx7oqOXBs9y4Z6emsdq9RoGkscufdabl/RfusLWR" +
                "m8ufWZfAAB3jnunN/xHuWdvlBRC8W62zw9qxaai9s8vyrtnJCRvoq9z30h+8" +
                "W62zw9qxaam0s8v1nun3+hrtdpm1ys2faLWRm8ufWZfABBTmp9j4Bh3kd/Dy" +
                "BPLSsKjN4x7Jf+a9wvXwo+a0wPy8drOzBBTmp9j4Bh3kd4SOzdrrotrp/x7k" +
                "d4SOdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW4wt64aa+5yeK3a7Oz/RTi" +
                "nuX39vTjd4SOscufWbPw+g7kp+rp9um7aOPt9BDtrNzpz7iJWZeksefgpePz" +
                "COmMQ5ekscufWZekzQzjnZf4Chvkdg==");
        }

    }
}
