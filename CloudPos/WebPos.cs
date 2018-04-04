using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CloudPos
{
    /*
     *  Strictly speaking, WebPOS, is a different creature to CloudPOS and this 
     *  class does not play a part in using CloudPOS.
     *  
     *  WebPOS is the alternative browser-based way of accessing Afterpay Touch 
     *  eServices when there is _no_ POS integration.   It is a stand-alone product
     *  which offers no API.
     *  
     *  It is included here is for demonstration purposes only.
     * 
     */ 
    public class WebPos : IDisposable
    {
        private CloudPosUI.ICloudPosUI _ui;

        public EventHandler<string> Loaded;
        public EventHandler Unloaded;

        public WebPos()
        {
            InstantiateBrowser("ie", "Web POS");
        }

        ~WebPos() // Destructor
        {
            Dispose();
        }

        public void InitPosWindow(int left, int top, int width, int height, string url)
        {
            _ui.SetPosition(left, top);
            _ui.SetClientSize(width, height);
            _ui.Navigate(url);
            _ui.Show();
        }

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
