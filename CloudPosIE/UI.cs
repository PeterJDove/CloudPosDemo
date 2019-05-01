using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/// <summary>
/// Provides a Windows Form containing a .NET WebBrowser in which the javascript application runs.
/// </summary>
namespace Touch.CloudPosIE 
{
    /// <summary>
    /// Provides a wrapper around the browser window, for <b>CloudPOS</b> to call.
    /// </summary>
    /// <remarks>
    /// By creating a simple wrapper, this class protects <b>CloudPOS</b> from
    /// having to include references to System.Windows.Forms or PresentationCore
    /// &amp; Framework (in the case of WPF apps), etc.
    /// </remarks>
    public class UI : CloudPosUI.ICloudPosUI
    {
        private BrowserForm _browserForm;

        /// <summary>
        /// Raised when the javascript application sends a message to be handled by the POS.
        /// </summary>        
        public event EventHandler<string> Notify;

        /// <summary>
        /// Raised when the web browser has loaded the HTML web application.
        /// </summary>
        public event EventHandler<string> Loaded;

        /// <summary>
        /// To be raised when the GUI window is closed.
        /// </summary>
        public event EventHandler Unloaded;


        /// <summary>
        /// Instatiates the UI class, creating a new GUI window, and linking to the events that the GUI window will raise.
        /// </summary>
        /// <param name="title">Window Title. Not seen, as the GUI Form has no borders or title bar.</param>
        /// <param name="closeable">Sets a flag which control whether the GUI Form may be closed by operator.</param>       
        public UI(string title, bool closeable)
        {
            _browserForm = new BrowserForm(title + " (.NET WebBrowser)", closeable);
            _browserForm.Notify += Form_Notify;
            _browserForm.Loaded += Form_Loaded;
            _browserForm.Unloaded += Form_Unloaded;
        }

        ~UI() // Destructor
        {
            Dispose();
        }

        /// <summary>
        /// Hides and cleans up resources by the the GUI window.
        /// </summary>
        public void Dispose()
        {
            if (_browserForm != null)
            {
                _browserForm.Hide();
                _browserForm.Dispose();
                _browserForm = null;
            }
            GC.Collect(); // Start .NET CLR Garbage Collection
            GC.WaitForPendingFinalizers(); // Wait for Garbage Collection to finish
        }

        /// <summary>
        /// Sets the position, on the screen, of the CloudPOS GUI window.
        /// </summary>
        /// <param name="left">The pixel position of the left of the GUI window</param>
        /// <param name="top">The pixel position of the top of the GUI window</param>
        public void SetPosition(int left, int top)
        {
            _browserForm.SetDesktopLocation(left, top);
        }

        /// <summary>
        /// Sets the size of the CloudPOS GUI window.
        /// </summary>
        /// <param name="width">The window width, in pixels</param>
        /// <param name="height">The window height, in pixels</param>
        public void SetClientSize(int width, int height)
        {
            _browserForm.ClientSize = new System.Drawing.Size(width, height);
        }

        /// <summary>
        /// Show the CloudPOS GUI window.
        /// </summary>
        public void Show()
        {
            _browserForm.Invoke(new MethodInvoker(() =>
            {
                _browserForm.Show();
            }));
        }

        /// <summary>
        /// Hide the CloudPOS GUI window.
        /// </summary>
        public void Hide()
        {
            _browserForm.Invoke(new MethodInvoker(() =>
            {
                _browserForm.Hide();
            }));
        }

        /// <summary>
        /// Instructs the browser encapsulated in the CloudPOS GUI window to load a web page (a web application).
        /// </summary>
        /// <param name="url">The address of the web page to load</param>
        public void Navigate(string url)
        {
            _browserForm.Navigate(url);
        }

        /// <summary>
        /// Sends a JSON command into CloudPOS; i.e. into the javascript application ruuning in the browser.
        /// </summary>
        /// <param name="json"></param>
        public void SendMessage(string json)
        {
            _browserForm.SendMessage(json);
        }

        private void Form_Notify(object sender, string json)
        {
            Notify?.Invoke(this, json);
        }

        private void Form_Loaded(object sender, string url)
        {
            Loaded?.Invoke(this, url);
        }

        private void Form_Unloaded(object sender, EventArgs e)
        {
            Unloaded?.Invoke(this, e);
        }


    }
}
