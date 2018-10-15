using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudPosIE 
{
    /*
     *  This UI wrapper provides a class that the CloudPOS.POS can drive the 
     *  BrowserWindow through, without having to reference System.Windows.Forms
     *  or PresentationCore & Framework (for WPF apps), etc.
     */ 
    public class UI : CloudPosUI.ICloudPosUI
    {
        private BrowserForm _browserForm;

        public event EventHandler<string> Notify;
        public event EventHandler<string> Loaded;
        public event EventHandler Unloaded;


        public UI(string title, bool closeable)
        {
            _browserForm = new BrowserForm(title + " (.NET WebBrowser)", closeable);
            _browserForm.Notify += BrowserForm_Notify;
            _browserForm.Loaded += BrowserForm_Loaded;
            _browserForm.Unloaded += BrowserForm_Unloaded;
        }

        ~UI() // Destructor
        {
            Dispose();
        }

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

        public void SetPosition(int left, int top)
        {
            _browserForm.SetDesktopLocation(left, top);
        }

        public void SetClientSize(int width, int height)
        {
            _browserForm.ClientSize = new System.Drawing.Size(width, height);
        }

        public void Show()
        {
            _browserForm.Invoke(new MethodInvoker(() =>
            {
                _browserForm.Show();
            }));
        }

        public void Hide()
        {
            _browserForm.Invoke(new MethodInvoker(() =>
            {
                _browserForm.Hide();
            }));
        }

        public void Navigate(string url)
        {
            _browserForm.Navigate(url);
        }

        public void SendMessage(string json)
        {
            _browserForm.SendMessage(json);
        }

        private void BrowserForm_Notify(object sender, string json)
        {
            Notify?.Invoke(this, json);
        }

        private void BrowserForm_Loaded(object sender, string url)
        {
            Loaded?.Invoke(this, url);
        }

        private void BrowserForm_Unloaded(object sender, EventArgs e)
        {
            Unloaded?.Invoke(this, e);
        }


    }
}
