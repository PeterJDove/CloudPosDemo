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
    // These are required to support the Browser <-> .NET App communications
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    public partial class EssentialForm : Form
    {
        public event EventHandler<string> Notify;
        public event EventHandler<string> Loaded;
        public event EventHandler Unloaded;

        private bool _closeable = false;


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


        public void Navigate(string url)
        {
            this.Show(); // Needed to initiate things properly
            webView.LoadUrl(url);
            this.Hide();
        }

        public void SendMessage(string json)
        {
            //this.Invoke(new MethodInvoker(() =>
            //{
                string script = "window.postMessage(" + json + ", \"*\");";
                webControl.WebView.EvalScript(script);
            //}));
        }

        private void ObjectForScripting_Notify(object sender, string json)
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

        private void webView_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            Loaded?.Invoke(this, e.Url);
        }



    }


    public class ObjectForScripting
    {
        public event EventHandler<string> Notify;

        public void notify(string json)
        {
            Notify?.Invoke(this, json);
        }
    }

}
