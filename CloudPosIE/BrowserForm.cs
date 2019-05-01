/*
 *  Un-Comment the following #define to enable SmartCard Support
 */
//#define SMARTCARDS_SUPPORTED

 using System;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
#if SMARTCARDS_SUPPORTED
using CloudSmartCards; // Make sure this is included in Project References
#endif

namespace Touch.CloudPosIE
{
    /// <summary>
    /// <para>
    /// The BrowserForm is that which presents the CloudPOS GUI.
    /// It is a borderless window, containing a single UI widget, a .NET WebBrowser
    /// which renders HTML and behaves like Internet Explorer.
    /// </para>
    /// <para>
    /// This class must be declared <i>public</i> as its needs to be exposed via 
    /// COM to allow communication between the C# code within, and javascipt running
    /// within the browser.
    /// </para>
    /// </summary>
    // These are required to support the Browser <-> .NET App communications
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public partial class BrowserForm : Form
    {
        /// <summary>
        /// Raised when the javascript within the browser sends a JSON message via the <see cref="notify(string)"/> callback.
        /// </summary>
        public event EventHandler<string> Notify;

        /// <summary>
        /// Raised when DocumentCompleted, i.e. when a HTML page is fully loaded into the browser.
        /// </summary>
        public event EventHandler<string> Loaded;

        /// <summary>
        /// Raised when this form is closed.
        /// </summary>
        public event EventHandler Unloaded;

        private bool _closeable = false;
        //private string _url;
        //private object[] _args;

        /// <summary>
        /// Creates a new BrowserForm, and sets the new form object as the "ObjectForScripting" for the WebBrowser.
        /// </summary>
        /// <param name="title">Window Title. Not seen, as this form has no borders or title bar.</param>
        /// <param name="closeable">Sets a flag which control whether form may be closed by operator.</param>       
        internal BrowserForm(string title, bool closeable)
        {
            InitializeComponent();
            this.Text = title;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Width - this.Height) / 2;
            _closeable = closeable;
            webBrowser.ObjectForScripting = this;
        }

        /// <summary>
        /// Tells the browser to load a web page.
        /// </summary>
        /// <param name="url">The web address of the CloudPOS server</param>
        public void Navigate(string url)
        {
            webBrowser.Navigate(url);
            //_url = url;
            //Debug.Assert(!webBrowser.IsBusy); // Ensures BrowserForm and webBrowser is loaded
            //new Thread(NavigateWorker).Start();
        }

        //private void NavigateWorker()
        //{
        //    this.Invoke(new MethodInvoker(() =>
        //    {
        //        webBrowser.Navigate(_url);
        //    }));
        //}

        /// <summary>
        /// Injects a javascript command into the WebBrowser, by way of a call to
        /// Document.InvokeScript("eval"...)
        /// </summary>
        /// <param name="json"></param>
        public void SendMessage(string json)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                    object[] args = new object[] { "window.postMessage(" + json + ", \"*\");" };
                    webBrowser.Document.InvokeScript("eval", args);
            }));
        }

        /// <summary>
        /// This is the callback for receiving JSON messages from the CloudPOS javascript application.
        /// This must be public, and must be called "notify".
        /// </summary>
        /// <param name="json"></param>
        public void notify(string json)
        {
            Notify?.Invoke(this, json);
        }

        #if SMARTCARDS_SUPPORTED
        /// <summary>
        /// <para>Checks to see whether a smartcard is currently being presented in front of the SmartCard reader.</para>
        /// <para>This is a public method, called by the smartcard processing in the CloudPOS javascript application.</para>
        /// </summary>
        /// <returns><c>true</c> if a card is present, or <c>false</c> if not.</returns>
        public bool isCardPresent() // no args
        {
            var cardPresent = SmartCardApi.IsCardPresent();
            // Debug.Print("cardPresent = " + cardPresent);
            return cardPresent;
        }

        /// <summary>
        /// <para>Reads the smartcard identity, and returns it to the javascript application.</para>
        /// <para>This is a public method, called by the smartcard processing in the CloudPOS javascript application.</para>
        /// </summary>
        /// <returns>The Smartcard ID</returns>
        public string readCardIdentity() // no args
        {
            var cardIdentity = SmartCardApi.ReadCardIdentity();
            // Debug.Print("cardIdentity = " + cardIdentity);
            return cardIdentity;
        }

        /// <summary>
        /// <para>Performs a sequence of smartcard commands to unlock, read, and write sectors on the card.</para>
        /// <para>This is a public method, called by the smartcard processing in the CloudPOS javascript application.</para>
        /// </summary>
        /// <param name="cardActions">A sequence of smartcard commands</param>
        /// <returns>A sequence of results for each of the actions in <paramref name="cardActions"/></returns>
        public string executeCardActions(string cardActions)
        {
            // Debug.Print("cardActions = " + cardActions);
            var results = SmartCardApi.ProcessCardActions(cardActions);
            // Debug.Print("    results = " + results);
            return results;
        }

        /// <summary>
        /// <para>Returns the version number of the Smartcard API.</para>
        /// <para>This is a public method, which may bee called by the smartcard processing in the CloudPOS javascript application.</para>/ 
        /// </summary>
        /// <returns>Version number</returns>
        public int getSmartcardPluginVersionNo() // no args
        {
            var version = SmartCardApi.GetSmartcardPluginVersionNo();
            return version;
        }
        #endif

        /// <summary>
        /// Called when Form's Closing event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (!_closeable)
            {
                e.Cancel = true; // Do not allow to Close
                this.Hide();
            }
            else
                Unloaded?.Invoke(this, null);
        }

        /// <summary>
        /// Called when WebBrowser's DocumentCompleted event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Loaded?.Invoke(this, e.Url.ToString());
        }


    }
}
