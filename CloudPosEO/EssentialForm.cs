using System;
using System.Collections.Generic;
using System.ComponentModel;
using EO.WebBrowser;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Touch.CloudPosEO
{
    /// <summary>
    /// <para>
    /// The BrowserForm is that which presents the CloudPOS GUI.
    /// It is a borderless window, containing a single UI widget, an 
	/// Essential Objects (EO) WebBrowser, which renders HTML and 
	/// behaves like Google Chrome.
    /// </para>
    /// </summary>
    // These are required to support the Browser <-> .NET App communications
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    public partial class EssentialForm : Form
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


        /// <summary>
        /// Creates a new EssentialForm, and creates an instance of "ObjectForScripting".
		/// It then attaches a handler for that object's Notify event, by which this class will
		/// receive messages from the CloudPOS javascript application running in the webbrowser.
        /// </summary>
        /// <param name="title">Window Title. Not seen, as this form has no borders or title bar.</param>
        /// <param name="closeable">Sets a flag which control whether form may be closed by operator.</param>       
        internal EssentialForm(string title, bool closeable)
        {
            InitializeComponent();
            this.Text = title;
            _closeable = closeable;

            var ofs = new ObjectForScripting();
            ofs.Notify += ObjectForScripting_Notify;
            webView.ObjectForScripting = ofs;
            webView.UnloadDelay = int.MaxValue;
        }

        /// <summary>
        /// Tells the browser to load a web page.
        /// </summary>
        /// <param name="url">The web address of the CloudPOS server</param>
        public void Navigate(string url)
        {
            this.Show(); // Needed to initiate things properly
            webView.LoadUrl(url);
            this.Hide();
        }

        /// <summary>
        /// Injects a javascript command into the WebBrowser, by way of a call to
        /// webControl.WebView.EvalScript("eval"...)
        /// </summary>
        /// <param name="json"></param>
        public void SendMessage(string json)
        {
            //this.Invoke(new MethodInvoker(() =>
            //{
                string script = "window.postMessage(" + json + ", \"*\");";
                webControl.WebView.EvalScript(script);
            //}));
        }

        /// <summary>
        /// This is the callback for receiving JSON messages from the CloudPOS javascript application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="json"></param>
        private void ObjectForScripting_Notify(object sender, string json)
        {
            Notify?.Invoke(this, json);
        }


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
        /// Called when WebBrowser's Document (the javacsript app) is completely loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webView_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            Loaded?.Invoke(this, e.Url);
        }



    }


    /// <summary>
    /// <para>
    /// The ObjectForScripting is a tiny class that provides the link between the 
    /// Javascript application and the EssentialForm object above.  
	/// When its public <code>notify</code> method is called, it raises an event.
    /// </para>
    /// </summary>
	public class ObjectForScripting
    {
        public event EventHandler<string> Notify;

        public void notify(string json)
        {
            Notify?.Invoke(this, json);
        }
    }

}
