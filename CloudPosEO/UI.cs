using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Touch.CloudPosEO
{
    /*
     *  This UI wrapper provides a class that the CloudPOS.POS can drive the 
     *  BrowserWindow through, without having to reference System.Windows.Forms
     *  or PresentationCore & Framework (for WPF apps), etc.
     */
    public class UI : CloudPosUI.ICloudPosUI
    {
        private EssentialForm _essentialForm;

        public event EventHandler<string> Notify;
        public event EventHandler<string> Loaded;
        public event EventHandler Unloaded;


        public UI(string title, bool closeable)
        {
            SetupEOBrowser();
            _essentialForm = new EssentialForm(title, closeable);
            _essentialForm.Notify += Form_Notify;
            _essentialForm.Loaded += Form_Loaded;
            _essentialForm.Unloaded += Form_Unloaded;
        }

        public void Dispose()
        {
            if (_essentialForm != null)
            {
                _essentialForm.Hide();
                _essentialForm.Dispose();
                _essentialForm = null;
            }
            GC.Collect(); // Start .NET CLR Garbage Collection
            GC.WaitForPendingFinalizers(); // Wait for Garbage Collection to finish
        }
        public void SetPosition(int left, int top)
        {
            _essentialForm.SetDesktopLocation(left, top);
        }

        public void SetClientSize(int width, int height)
        {
            _essentialForm.ClientSize = new System.Drawing.Size(width, height);
        }


        public void Show()
        {
            _essentialForm.Show();
        }

        public void Hide()
        {
            _essentialForm.Hide();
        }

        public void Navigate(string url)
        {
            _essentialForm.Navigate(url);
        }

        public void SendMessage(string json)
        {
            _essentialForm.SendMessage(json);
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


        private static void SetupEOBrowser()
        {
            EO.WebBrowser.Runtime.AddLicense(
                "bZnJ4NnCoenz/hChWe3pAx7oqOXBs9y4Z6emsdq9RoGkscufdabl/RfusLWR" +
                "m8ufWZfAAB3jnunN/xHuWdvlBRC8W62zw9qxaai9s8vyrtnJCRvoq9z30h+8" +
                "W62zw9qxaam0s8v1nun3+hrtdpm1ys2faLWRm8ufWZfABBTmp9j4Bh3kd/Dy" +
                "BPLSsKjN4x7Jf+a9wvXwo+a0wPy8drOzBBTmp9j4Bh3kd4SOzdrrotrp/x7k" +
                "d4SOdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW4wt64aa+5yeK3a7Oz/RTi" +
                "nuX39vTjd4SOscufWbPw+g7kp+rp9um7aOPt9BDtrNzpz7iJWZeksefgpePz" +
                "COmMQ5ekscufWZekzQzjnZf4Chvkdg==");
        }




    }
}
