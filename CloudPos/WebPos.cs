using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// An alternative to the <see cref="Touch.CloudPos"/> for when there is <i>no</i> POS integration.
/// </summary>
namespace Touch.WebPos
{

    /// <summary>
    /// The WebPos Client provides an alternative to the CloudPOS <see cref="CloudPos.API"/>;
    /// which may be used when there is <i>no</i> POS integration.
    /// </summary>
    public class Client : IDisposable
    {
        private CloudPosUI.ICloudPosUI _ui;


        /// <summary>
        /// Raised when the web browser has loaded the HTML web application.
        /// </summary>
        /// <remarks>
        /// <b>Loaded</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing the URL that was loaded.
        /// </remarks>
        public event EventHandler<string> Loaded;


        /// <summary>
        /// Raised when the web browser has closed.
        /// </summary>
        /// <remarks>
        /// <b>Unloaded</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler">EventHander</see>
        /// delegate, passing no EventArgs.
        /// </remarks>
        public event EventHandler Unloaded;


        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
        {
            InstantiateBrowser("ie", "Web POS");
        }

        ~Client() // Destructor
        {
            Dispose();
        }

        /// <summary>
        /// Initialises and shows the WebPos browser window, setting its position and size, and loading the web application. 
        /// </summary>
        /// <param name="left">The required position of the left-hand side of the browser window, in pixels.</param>
        /// <param name="top">The required position of the top of the browser window, in pixels.</param>
        /// <param name="width">The required width of the browser window, in pixels.</param>
        /// <param name="height">The required height of the browser window, in pixels.</param>
        /// <param name="url">The address to load the CloudPOS web application.</param>
        public void InitPosWindow(int left, int top, int width, int height, string url)
        {
            _ui.SetPosition(left, top);
            _ui.SetClientSize(width, height);
            _ui.Navigate(url);
            _ui.Show();
        }

        /// <summary>
        /// Releases all resources associated with the WebPos browser window.
        /// </summary>
        public void Dispose()
        {
            if (_ui != null)
            {
                _ui.Dispose();
                _ui = null;
            }
        }


        private void InstantiateBrowser(string type, string title)
        {
            switch (type.ToLower())
            {
                case "ie":
                    _ui = new CloudPosIE.UI(title, true);
                    break;
                //case "essential":
                //    _ui = new CloudPosEO.UI(title);
                //    _ui.Notify += _ui_Notify;
                //    break;
                //case "awesomium":
                //    _ui = new CloudPosAwesomium.UI(title);
                //    _ui.Notify += _ui_Notify;
                //    break;
            }
            _ui.Loaded += _ui_Loaded;
            _ui.Unloaded += _ui_Unloaded;
        }

        private void _ui_Loaded(object sender, string url)
        {
            Loaded?.Invoke(this, url);
        }

        private void _ui_Unloaded(object sender, EventArgs e)
        {
            Unloaded?.Invoke(this, e);
        }




    }
}
