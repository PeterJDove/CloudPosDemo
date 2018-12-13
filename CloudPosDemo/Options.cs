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

    public enum PosType
    {
        CloudPOS,
        WebPOS
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


        // Configuration Options
        public int ConnectionId { get; set; }
        public string ConnectionName { get; set; }
        public PosType PosType { get; set; }
        public string Url { get; set; }
        public string Secret { get; set; }
        public string HardwareName { get; set; }
        public string Skin { get; set; }
        public string Locale { get; set; }
        public string Operator { get; set; }

        // Browser Options
        public int Left { get; set; }
        public int Top { get; set; }
        public ClientSize ClientSize { get; set; }

        private static Dictionary<string, ClientSize> _clientSizes = new Dictionary<string, ClientSize>();

        public static Dictionary<string, ClientSize> ClientSizes()
        {
            return _clientSizes;
        }

        public Options (IniFile ini)
        {
            KeepOnTop = ini.GetBoolean("GENERAL", "topmost", false);
            ConnectionId = ini.GetInt("GENERAL", "connection");
            ConnectionName = ini.GetString(ConnSection(ConnectionId), "name");

            Left = ini.GetInt("BROWSER", "left", 0);
            Top = ini.GetInt("BROWSER", "top", 0);
            ClientSize = new ClientSize(ini.GetString("BROWSER", "size", "xga"));
            if (ClientSize.Id == "custom")
            {
                ClientSize.Width = ini.GetInt("BROWSER", "width");
                ClientSize.Height = ini.GetInt("BROWSER", "height");
            }
        }

        public void LoadConnection(string name)
        {
            ConnectionName = name;
            if (!string.IsNullOrEmpty(name))
            {
                IniFile ini = Program.IniFile();
                ConnectionId = GetSectionId(name);
                string section = ConnSection(ConnectionId);

                ini.Write("GENERAL", "connection", ConnectionId);

                var posType = ini.GetString(section, "type", PosType.CloudPOS.ToString());
                PosType = (PosType)Enum.Parse(typeof(PosType), posType);
                Url = ini.GetString(section, "url");
                Secret = ini.GetString(section, "secret");
                HardwareName = ini.GetString(section, "hardwarename", Environment.MachineName);
                Skin = ini.GetString(section, "skin", "vanilla");
                Locale = ini.GetString(section, "locale", "en-au");
                Operator = ini.GetString(section, "operator", Environment.UserName);
            }
        }

        private int GetSectionId(string connectionName)
        {
            var ini = Program.IniFile(); 
            int maxConnection = ini.GetInt("GENERAL", "max_connection");
            for (int i = 1; i <= maxConnection; i++)
            {
                if (ini.GetString(ConnSection(i), "name") == connectionName)
                    return i;
            }
            return maxConnection + 1;
        }


        public void PersistChanges()
        {
            IniFile ini = Program.IniFile();
            ini.Write("GENERAL", "topmost", KeepOnTop);

            ini.Write("BROWSER", "left", Left);
            ini.Write("BROWSER", "top", Top);
            ini.Write("BROWSER", "size", ClientSize.Id);
            ini.Write("BROWSER", "width", ClientSize.Width);
            ini.Write("BROWSER", "height", ClientSize.Height);

            if (ConnectionId == 0)
            {
                ConnectionId = ini.GetInt("GENERAL", "max_connection") + 1;
            }
            string section = ConnSection(ConnectionId);
            if (string.IsNullOrEmpty(Url))
            {
                ini.EraseSection(section);
                return;
            }
            ini.Write("GENERAL", "connection", ConnectionId);
            if (ConnectionId > ini.GetInt("GENERAL", "max_connection"))
                ini.Write("GENERAL", "max_connection", ConnectionId);

            ini.Write(section, "name", ConnectionName);
            ini.Write(section, "type", PosType.ToString());
            ini.Write(section, "url", Url);
            ini.Write(section, "secret", Secret);
            ini.Write(section, "hardwarename", HardwareName, Environment.MachineName);
            ini.Write(section, "skin", Skin);
            ini.Write(section, "locale", Locale);
            ini.Write(section, "operator", Operator);
        }



        internal CloudPos.Configuration CloudPosConfiguration()
        {
            return new CloudPos.Configuration
            {
                ApiUrl = Url,
                Secret = Secret,
                CredentialsKey = Url + "?" + Secret,
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

        public static string ConnSection (int i)
        {
            return "CONNECTION_" + i.ToString();
        }

    }
}
