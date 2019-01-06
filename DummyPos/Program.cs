using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Touch.Tools;
using static System.Environment;

namespace Touch.DummyPos
{
    public static class Program
    {
        private static IniFile _iniFile;
        private static FormMain _formMain;
        private static Options _options;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _formMain = new FormMain();
            _options = new Options(IniFile());

             Application.Run(_formMain);
        }

        public static FormMain FormMain()
        {
            return _formMain;
        }

        public static Options Options()
        {
            return _options;
        }

        public static IniFile IniFile()
        {

            if (_iniFile == null)
            {
                var appData = Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "CloudPOS");
                string iniFileName = Path.Combine(appData, "DummyPos.ini");
                if (File.Exists(iniFileName) == false)
                {
                    var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(iniFileName));
                    if (File.Exists(source))
                    {
                        Util.EnsureFolderExists(appData);
                        File.Copy(source, iniFileName);
                    }
                }
                _iniFile = new IniFile(iniFileName);
            }
            return _iniFile;
        }




    }

}
