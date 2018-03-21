using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Touch.CloudPosDemo;
using Touch.Tools;

namespace Touch.CloudPosDemo
{
    public enum HardwareRecording
    {
        Never,
        WhenGuiShowing,
        Always,
    }

    public class ClientSize
    {
        public string Id { get; private set; }
        public string Description { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ClientSize(string id)
        {
            switch (id)
            {
                case "albert":
                    Init(id, "Albert", 1280, 800);
                    break;
                case "svga":
                    Init(id, "Desktop SVGA", 800, 600);
                    break;
                case "xga":
                    Init(id, "Desktop XGA", 1024, 768);
                    break;
                case "hd720":
                    Init(id, "Desktop HD720", 1280, 720);
                    break;
                case "sxga":
                    Init(id, "Desktop SXGA", 1400, 1050);
                    break;
                case "hd1080":
                    Init(id, "Desktop HD1080", 1920, 1080);
                    break;
                case "tablet":
                    Init(id, "Tablet", 800, 1200);
                    break;
                case "7eleven":
                    Init(id, "7-Eleven POS", 484, 326);
                    break;
                default:
                    Init(id, "Custom", 0, 0);
                    break;
            }
        }

        private void Init(string id, string desc, int width, int height)
        {
            Id = id;
            Description = desc;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return Description;
        }

        public static void PopulateComboBox(ComboBox combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ClientSize("custom"));
            combo.Items.Add(new ClientSize("albert"));
            combo.Items.Add(new ClientSize("svga"));
            combo.Items.Add(new ClientSize("xga"));
            combo.Items.Add(new ClientSize("hd720"));
            combo.Items.Add(new ClientSize("sxga"));
            combo.Items.Add(new ClientSize("hd1080"));
            combo.Items.Add(new ClientSize("tablet"));
            combo.Items.Add(new ClientSize("7eleven"));
            combo.SelectedIndex = 3;
        }
    }


    public class Options
    {
        // General Options
        public bool KeepOnTop {
            get
            {
                return Program.FormMain().TopMost;
            }
            set
            {
                Program.FormMain().TopMost = value;
            }
        }
        public string Operator { get; set; }

        // Folders
        public string HomeFolder { get; set; }
        public string ScriptsFolder { get; set; }
        public string SnapshotsFolder { get; set; }
        public string LogsFolder { get; set; }


        // CloudPOS Options
        public string CloudPosUrl { get; set; }
        public string Secret { get; set; }
        public string HardwareName { get; set; }
        public string Skin { get; set; }
        public string Locale { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public ClientSize ClientSize { get; set; }

        // WebPOS Options
        public string WebPosUrl { get; set; }
        // Also: Left, Top, Width, Height 

        private static Dictionary<string, ClientSize> _clientSizes = new Dictionary<string, ClientSize>();

        public static string DefaultHomeFolder()
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(docs, "CloudPosDemo");
        }

        public static Dictionary<string, ClientSize> ClientSizes()
        {
            return _clientSizes;
        }

        public Options (IniFile ini)
        {
            Operator = ini.GetString("GENERAL", "operator");
            KeepOnTop = ini.GetBoolean("GENERAL", "topmost", false);
            if (string.IsNullOrEmpty(Operator))
                Operator = Environment.UserName;

            HomeFolder = ini.GetString("FOLDERS", "home", DefaultHomeFolder());
            ScriptsFolder = ini.GetString("FOLDERS", "scripts", Path.Combine(HomeFolder, "scripts"));
            SnapshotsFolder = ini.GetString("FOLDERS", "snapshots", Path.Combine(HomeFolder, "snapshots"));
            LogsFolder = ini.GetString("FOLDERS", "logs", Path.Combine(HomeFolder, "logs"));

            Util.EnsureFolderExists(HomeFolder);
            Util.EnsureFolderExists(ScriptsFolder);
            Util.EnsureFolderExists(SnapshotsFolder);
            Util.EnsureFolderExists(LogsFolder);

            CloudPosUrl = ini.GetString("CLOUDPOS", "url");
            Secret = ini.GetString("CLOUDPOS", "secret");
            HardwareName = ini.GetString("CLOUDPOS", "hardwarename", Environment.MachineName);
            Skin = ini.GetString("CLOUDPOS", "skin");
            Locale = ini.GetString("CLOUDPOS", "locale");

            Left = ini.GetInt("CLOUDPOS", "left", 0);
            Top = ini.GetInt("CLOUDPOS", "top", 0);
            ClientSize = new ClientSize(ini.GetString("CLOUDPOS", "size", "xga"));
            if (ClientSize.Id == "custom")
            {
                ClientSize.Width = ini.GetInt("CLOUDPOS", "width");
                ClientSize.Height = ini.GetInt("CLOUDPOS", "height");
            }

            WebPosUrl = ini.GetString("WEBPOS", "url");
        }

        public void PersistChanges()
        {
            IniFile ini = Program.IniFile();
            ini.Write("GENERAL", "operator", Operator);
            ini.Write("GENERAL", "topmost", KeepOnTop);

            ini.Write("FOLDERS", "home", HomeFolder, DefaultHomeFolder());
            ini.Write("FOLDERS", "scripts", ScriptsFolder, Path.Combine(HomeFolder, "scripts"));
            ini.Write("FOLDERS", "snapshots", SnapshotsFolder, Path.Combine(HomeFolder, "snapshots"));
            ini.Write("FOLDERS", "logs", LogsFolder, Path.Combine(HomeFolder, "logs"));

            ini.Write("CLOUDPOS", "url", CloudPosUrl);
            ini.Write("CLOUDPOS", "secret", Secret);
            ini.Write("CLOUDPOS", "hardwarename", HardwareName, Environment.MachineName);
            ini.Write("CLOUDPOS", "skin", Skin);
            ini.Write("CLOUDPOS", "locale", Locale);

            ini.Write("CLOUDPOS", "left", Left);
            ini.Write("CLOUDPOS", "top", Top);
            ini.Write("CLOUDPOS", "size", ClientSize.Id);
            ini.Write("CLOUDPOS", "width", ClientSize.Width);
            ini.Write("CLOUDPOS", "height", ClientSize.Height);

            ini.Write("WEBPOS", "url", WebPosUrl);
        }

        internal CloudPos.Configuration CloudPosConfiguration()
        {
            return new CloudPos.Configuration
            {
                ApiUrl = CloudPosUrl,
                Secret = Secret,
                Operator = Operator,
                HardwareName = HardwareName,
                SkinName = Skin,
                Locale = Locale,
                ClientLeft = Left,
                ClientTop = Top,
                ClientWidth = ClientSize.Width,
                ClientHeight = ClientSize.Height,
            };
        }


    }
}
