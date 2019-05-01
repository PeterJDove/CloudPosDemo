using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Touch.CloudPos;
using Touch.DummyPos;
using Touch.Tools;


namespace Touch.DummyPos
{
    /// <summary>
    /// This class contains a bucket of properties that specify how to connect to the CloudPOS host, how to position the browser window, and so on.
    /// </summary>
    /// <remarks>
    /// <para>This class reads and writes the settings to a persistent store in an INI file.   
    /// The DummyPos <see cref="Program"/> object holds a single(ton) instance of this class, but can switch it between
    /// different CloudPOS "connections", containing settings for different hosts, and devices.</para>
    /// <para>The <see cref="CloudPosConfiguration"/> method of this class generates an instance of 
    /// <see cref="CloudPos.Configuration"/> using the currently loaded "connection" settings.</para>
    /// </remarks>
    class Options
    {
        // General Options

        /// <summary>
        /// Gets or sets whether the DummyPos <see cref="FormMain"/> should stay on top of other Windows.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the unique numeric ID of this particular Options collection.
        /// </summary>
        public int ConnectionId { get; set; }

        /// <summary>
        /// Gets or sets a name or description of this particular Options collection.
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets the type (purpose) of this Options collection: <see cref="ConfigurationType.CloudPOS"/> or <see cref="ConfigurationType.WebPOS"/>.
        /// </summary>
        public ConfigurationType ConfigurationType { get; set; }

        /// <summary>
        /// Gets or sets the base URL (protocol, domain &amp; port) upon which functional URLs are built.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Activation Code used to initially activate a device, to get its credentials.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the Activation Code to supply during device activation, along with the <see cref="Secret"/>.
        /// </summary>
        public string HardwareName { get; set; }

        /// <summary>
        /// Gets or sets the name of the skin to be requested when loading the CloudPOS POS application.
        /// </summary>
        public string Skin { get; set; }

        /// <summary>
        /// Gets or sets the current Locale when loading the CloudPOS POS application
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the Operator ID to include in host requests.
        /// </summary>
        public string Operator { get; set; }

        // Browser Options
        /// <summary>
        /// Gets or sets the size and position of the CloudPOS GUI window
        /// </summary>
        public ClientRect ClientRect { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        /// <param name="ini">The <see cref="IniFile"/> used to save all settings for DummyPos.</param>
        public Options (IniFile ini)
        {
            KeepOnTop = ini.GetBoolean("GENERAL", "topmost", false);
            ConnectionId = ini.GetInt("GENERAL", "connection");
            ConnectionName = ini.GetString(ConnSection(ConnectionId), "name");
            HardwareName = Environment.MachineName;

            ClientRect = new ClientRect(ini.GetString("BROWSER", "size", "xga"));
            if (ClientRect.Id == "custom")
            {
                ClientRect.Width = ini.GetInt("BROWSER", "width");
                ClientRect.Height = ini.GetInt("BROWSER", "height");
            }
            ClientRect.Left = ini.GetInt("BROWSER", "left", 0);
            ClientRect.Top = ini.GetInt("BROWSER", "top", 0);
        }


        /// <summary>
        /// Instructs the Options class to (re)load all connection specific settings for a named Connection.
        /// </summary>
        /// <param name="name"></param>
        public void LoadConnection(string name)
        {
            ConnectionName = name;
            if (!string.IsNullOrEmpty(name))
            {
                IniFile ini = Program.IniFile();
                ConnectionId = GetSectionId(name);
                string section = ConnSection(ConnectionId);

                ini.Write("GENERAL", "connection", ConnectionId);

                var posType = ini.GetString(section, "type", ConfigurationType.CloudPOS.ToString());
                ConfigurationType = (ConfigurationType)Enum.Parse(typeof(ConfigurationType), posType);
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

        /// <summary>
        /// Saves the <see cref="Options"/> settings to the INI file that passed into the Constructor.
        /// </summary>
        public void PersistChanges()
        {
            IniFile ini = Program.IniFile();
            ini.Write("GENERAL", "topmost", KeepOnTop);

            ini.Write("BROWSER", "size", ClientRect.Id);
            ini.Write("BROWSER", "width", ClientRect.Width);
            ini.Write("BROWSER", "height", ClientRect.Height);
            ini.Write("BROWSER", "left", ClientRect.Left);
            ini.Write("BROWSER", "top", ClientRect.Top);

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
            ini.Write(section, "type", ConfigurationType.ToString());
            ini.Write(section, "url", Url);
            ini.Write(section, "secret", Secret);
            ini.Write(section, "hardwarename", HardwareName, Environment.MachineName);
            ini.Write(section, "skin", Skin);
            ini.Write(section, "locale", Locale);
            ini.Write(section, "operator", Operator);
        }

        /// <summary>
        /// Generates an instance of <see cref="CloudPos.Configuration"/> that can be used 
        /// when calling <see cref="API.InitPosWindow"/> to set up a CloudPOS session.
        /// </summary>
        /// <returns>A fully populated<see cref="CloudPos.Configuration"/>.</returns>
        internal CloudPos.Configuration CloudPosConfiguration()
        {
            return new CloudPos.Configuration()
            {
                Url = Url,
                Secret = Secret,
                CredentialsKey = Url + "?" + Secret,
                Operator = Operator,
                HardwareName = HardwareName,
                SkinName = Skin,
                Locale = Locale,
                ClientRect = ClientRect
                //ClientRect = new CloudPos.ClientRect()
                //{
                //    Width = ClientRect.Width,
                //    Height = ClientRect.Height,
                //    Left = ClientRect.Left,
                //    Top = ClientRect.Top,
                //}
            };
        }

        /// <summary>
        /// Returns an INI section name for a numbered connection.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>The word "CONNECTION_" with the number appended.</returns>
        public static string ConnSection (int i)
        {
            return "CONNECTION_" + i.ToString();
        }

        /// <summary>
        /// Populates a <see cref="ComboBox"/> with a list of named POS window sizes.
        /// </summary>
        /// <param name="combo">The <see cref="ComboBox"/> to be populated.</param>
        /// <param name="id">The ID (name) of the item to show as selected.  If not valid, "fullscreen" will be selected.</param>
        public void PopulateClientRectCombo(ComboBox combo, string id)
        {
            combo.Items.Clear();
            combo.Items.Add(new ClientRect("fullscreen"));
            combo.Items.Add(new ClientRect("custom"));
            combo.Items.Add(new ClientRect("albert"));
            combo.Items.Add(new ClientRect("svga"));
            combo.Items.Add(new ClientRect("xga"));
            combo.Items.Add(new ClientRect("hd720"));
            combo.Items.Add(new ClientRect("sxga"));
            combo.Items.Add(new ClientRect("hd1080"));
            combo.Items.Add(new ClientRect("tablet"));
            combo.Items.Add(new ClientRect("7eleven"));
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (((ClientRect)combo.Items[i]).Id == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            combo.SelectedIndex = 0;
        }

    }
}
