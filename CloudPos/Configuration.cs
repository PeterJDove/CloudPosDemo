using System;
using System.Web;
using System.Windows.Forms;

namespace Touch.CloudPos
{
    /// <summary>
    /// Denotes the purpose of a instance of the <see cref="Configuration"/> class.
    /// </summary>
    public enum ConfigurationType
    {
        /// <summary>
        /// The <see cref="Configuration"/> is for a fully integrated <b>CloudPOS</b> client.
        /// </summary>
        CloudPOS = 1,
        /// <summary>
        /// The <see cref="Configuration"/> is for a non-integrated, standalone <b>WebPOS</b> client.
        /// </summary>
        WebPOS = 2
    }


    /// <summary>
    /// A bucket of properties used to build the URLs by which the present device activates itself,
    /// and then brings the CloudPOS application into the browser.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets the name of this <see cref="Configuration"/>, as set by the Constructor.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of this <see cref="Configuration"/>: 
        /// <see cref="ConfigurationType.CloudPOS"/> (the default) or <see cref="ConfigurationType.WebPOS"/>
        /// </summary>
        public ConfigurationType Type { get; private set; }

        /// <summary>
        /// Gets or sets the base URL (protocol, domain &amp; port) upon which functional URLs are built.
        /// </summary>
        /// <seealso cref="GetActivationUrl"/>
        /// <seealso cref="GetBrowserUrl"/>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Operator ID to include in host requests.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the Activation Code used to initially activate a device, to get its credentials.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the Activation Code to supply during device activation, along with the <see cref="Secret"/>.
        /// </summary>
        public string HardwareName { get; set; }

        /// <summary>
        /// Gets or sets the full path name of the "auth" file, in which device credentials are saved.
        /// </summary>
        public string AuthFileName { get; set; }

        /// <summary>
        /// Gets or sets the skin to be requested when loading the CloudPOS POS application.
        /// </summary>
        /// <seealso cref="GetBrowserUrl"/>
        public string SkinName { get; set; }

        /// <summary>
        /// Gets or sets the current Locale when loading the CloudPOS POS application.
        /// </summary>
        /// <seealso cref="GetBrowserUrl"/>
        public string Locale { get; set; }


        /// <summary>
        /// Gets or sets the size and position of the CloudPOS GUI window.
        /// </summary>
        public ClientRect ClientRect { get; set; }

        /// <summary>
        /// Constant, used as ID of a <see cref="ClientRect"/> that occupies the full screen.
        /// </summary>
        public const string FullScreen = "fullscreen";

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class, assuming <see cref="ConfigurationType.CloudPOS"/>
        /// </summary>
        /// <overloads>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </overloads>
        public Configuration()
        {
            Type = ConfigurationType.CloudPOS;
            Name = "CloudPOS";
            ClientRect = new ClientRect(FullScreen);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class, for given <see cref="ConfigurationType"/>
        /// </summary>
        public Configuration(ConfigurationType type, string name)
        {
            Type = type;
            Name = name.ToUpper();
            ClientRect = new ClientRect(FullScreen);
        }

        /*
         *  The CloudPOS.auth file can carry multiple auth tokens, indexed by a key.
         *  AuthTokenKey is that key.   By default, it is just the ApiUrl, but may be 
         *  overridden to allow multiple auth tokens (for different retailers, stores,
         *  or devices) for the same ApiUrl.
         */


        private string _credentialsKey = null;

        /// <summary>
        /// Gets or sets the key by which the current Authorisation token is found in the "auth" file.
        /// </summary>
        /// <remarks>
        /// The CloudPos "auth" file can carry multiple auth tokens, indexed by a key.
        /// <see cref="CredentialsKey"/> is that key.   By default, it is just the 
        /// <see cref="Url"/>, but may be overridden to allow multiple auth tokens
        /// (for different retailers, stores, or devices) sharing the same <see cref="Url"/>.
        /// </remarks>
        public string CredentialsKey
        {
            get
            {
                if (string.IsNullOrEmpty(_credentialsKey))
                    return Url;
                else
                    return _credentialsKey;
            }
            set
            {
                _credentialsKey = value;
            }
        }

        /// <summary>
        /// Returns the URL needed to GET the CloudPos web application into the browser.
        /// </summary>
        /// <param name="token">Access token from <see cref="PosActivator"/></param>
        /// <returns>URL</returns>
        internal string GetBrowserUrl(string token)
        {
            var queries = HttpUtility.ParseQueryString("");
            if (!string.IsNullOrEmpty(SkinName))
            {
                queries["skinName"] = SkinName;
            }
            if (!string.IsNullOrEmpty(Locale))
            {
                queries["locale"] = Locale;
            }
            queries["callBackService"] = "NPOS";
            queries["accessToken"] = token;

            var builder = new UriBuilder(Url)
            {
                Fragment = "/blank-layout?" + queries.ToString()
            };
            return builder.ToString();
        }

        /// <summary>
        /// Returns the URL needed to activate the device.
        /// </summary>
        /// <remarks>
        /// No parameters are built into the URL returned, because it is POSTed rather than GET'd.
        /// </remarks>
        /// <returns>URL</returns>
        internal string GetActivationUrl()
        {
            var builder = new UriBuilder(Url)
            {
                Path = "/v1/devices/activate"
            };
            return builder.ToString();
        }

        /// <summary>
        /// Returns the URL needed to refresh the session token.
        /// </summary>
        /// <remarks>
        /// No parameters are built into the URL returned, because it is POSTed rather than GET'd.
        /// </remarks>
        /// <returns>URL</returns>
        internal string GetRefreshTokenUrl()
        {
            var builder = new UriBuilder(Url)
            {
                Path = "/v1/devices/tokens"
            };
            return builder.ToString();
        }


    }

    /// <summary>
    /// Represents the size and position of the CloudPOS GUI window.
    /// </summary>
    /// <remarks>
    /// This class differs from the standard
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.drawing.rectangle">System.Drawing.Rectangle</see>
    /// struct, in that it allows for different sizes to be named.
    /// </remarks>
    public class ClientRect
    {
        /// <summary>
        /// Gets the ID of a named <see cref="ClientRect"/>.
        /// </summary>
        /// <remarks>
        /// A <see cref="ClientRect"/> with the ID of <see cref="Configuration.FullScreen"/> has the special
        /// property of occupying the entire (primary) screen, regardless of its resolution.
        /// </remarks>
        public string Id { get; private set; }

        /// <summary>
        /// Gets a description, or explanation of purpose, of the <see cref="ClientRect"/>.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets the width of the CloudPOS GUI window, in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the CloudPOS GUI window, in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the left-most position of the CloudPOS GUI window, in pixels.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the top-most position of the CloudPOS GUI window, in pixels.
        /// </summary>
        public int Top { get; set; }

        /// <overloads>
        /// Initializes a new instance of the <see cref="ClientRect"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRect"/> class, of type <see cref="Configuration.FullScreen"/>.
        /// </summary>
        public ClientRect()
        {
            Init(Configuration.FullScreen, "Full-Screen", 0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRect"/> class, of a given ID.
        /// </summary>
        /// <remarks>
        /// Certain IDs are predefined, and cause the size (<see cref="Width"/> and <see cref="Height"/>) to be set automatically:
        /// <list type="bullet">
        /// <item><term>albert</term><description>1280 x 800</description></item>
        /// <item><term>svga</term><description>800 x 600</description></item>
        /// <item><term>xga</term><description>1024 x 768</description></item>
        /// <item><term>sxga</term><description>1400 x 1050</description></item>
        /// <item><term>hd720</term><description>1280 x 720</description></item>
        /// <item><term>hd1080</term><description>1920 x 1080</description></item>
        /// <item><term>tablet</term><description>800 x 1200</description></item>
        /// <item><term>tiny</term><description>400 x 300</description></item>
        /// <item><term>7eleven</term><description>484 x 326</description></item>
        /// <item><term>fullscreen</term>
        /// <description><see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen.primaryscreen">PrimaryScreen</see>.<see 
        /// href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen.bounds">Bounds</see>.<see 
        /// href="https://docs.microsoft.com/en-us/dotnet/api/system.drawing.rectangle.width">Width</see> x 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen.primaryscreen">PrimaryScreen</see>.<see 
        /// href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen.bounds">Bounds</see>.<see 
        /// href="https://docs.microsoft.com/en-us/dotnet/api/system.drawing.rectangle.height">Width</see></description>
        /// </item>
        /// </list>
        /// Any other ID is considered a custom ClientRect, with size set to 0 x 0 initially.
        /// </remarks>
        public ClientRect(string id)
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
                    Init(id, "7-Eleven Terminal", 484, 326);
                    break;
                case "tiny":
                    Init(id, "Tiny Testing", 400, 300);
                    break;
                case Configuration.FullScreen:
                    var rect = Screen.PrimaryScreen.Bounds;
                    Init(id, "Full-Screen", rect.Width, rect.Height);
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

        /// <summary>
        /// The <see cref="Description"/> of this <see cref="ClientRect"/>, for display in a dropdown
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox">ComboBox</see>
        /// </summary>
        /// <returns>The <see cref="Description"/> of this <see cref="ClientRect"/></returns>
        public override string ToString()
        {
            return Description;
        }


    }
}
