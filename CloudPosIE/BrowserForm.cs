using System;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CloudPosIE
{
    // These are required to support the Browser <-> .NET App communications
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]

    public partial class BrowserForm : Form
    {
        public event EventHandler<string> Notify;
        public event EventHandler<string> Loaded;
        public event EventHandler Unloaded;


        private bool _closeable = false;

        internal BrowserForm(string title, bool closeable)
        {
            InitializeComponent();
            this.Text = title;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            this.Top = (Screen.PrimaryScreen.Bounds.Width - this.Height) / 2;
            _closeable = closeable;
            webBrowser.ObjectForScripting = this;
        }

        public void Navigate(string url)
        {
            webBrowser.Navigate(url);
        }

        public void SendMessage(string json)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                object[] args = new object[] { "window.postMessage(" + json + ", \"*\");" };
                webBrowser.Document.InvokeScript("eval", args);
            }));
        }

        public void notify(string json)
        {
            Notify?.Invoke(this, json);
        }

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

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Loaded?.Invoke(this, e.Url.ToString());
        }
    }
}
