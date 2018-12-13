using System;
using System.Web;

namespace CloudPos
{
    /*
     *  Apart from the CloudPos (API) class, and the Model classes, this is the only
     *  public class exposed by this Class Library.  It is primary a bucket of properties
     *  that are used to build the URLs by which the present device activates itself,
     *  and then brings the CloudPOS application into the browser.
     *  
     *  ApiUrl :
     *      The base URL (protocol, domain) upon which functional URLs are built     *  
     *  Secret, Operator, and HardwareName : 
     *      Used to activate a device
     *  SkinName and Locale : 
     *      Used to specify the look and language for the application
     *  ClientLeft, ClientTop, ClientWidth, and ClientHeight :
     *      Used to control the size and position of the browser window
     *      
     *  A populated instance of this class should be passed to the 
     *  CloudPos.InitPosWindow() method when a new CloudPOS session is established.
     */
    public class Configuration
    {
        public string ApiUrl { get; set; }
        public string Operator { get; set; }
        public string Secret { get; set; }
        public string HardwareName { get; set; }
        public string SkinName { get ; set ; }
        public string Locale { get; set; }
        public int ClientLeft { get; set; }
        public int ClientTop { get; set; }
        public int ClientWidth { get; set; }
        public int ClientHeight { get; set; }

        /*
         *  The CloudPOS.auth file can carry multiple auth tokens, indexed by a key.
         *  AuthTokenKey is that key.   By default, it is just the ApiUrl, but may be 
         *  overridden to allow multiple auth tokens (for different retailers, stores,
         *  or devices) for the same ApiUrl.
         */
        private string _credentialsKey = null;
        public string CredentialsKey
        {
            get
            {
                if (string.IsNullOrEmpty(_credentialsKey))
                    return ApiUrl;
                else
                    return _credentialsKey;
            }
            set
            {
                _credentialsKey = value;
            }
        }

        /*
         *  GetBrowserUrl builds the URL to start the CloudPOS application.
         *  
         *  It needs a token returned by one of the methods of the PosActivator.
         */
        internal string GetBrowserUrl(string token)
        {
            var queries = HttpUtility.ParseQueryString("");
            queries["skinName"] = SkinName;
            queries["locale"] = Locale;
            queries["callBackService"] = "NPOS";
            queries["accessToken"] = token;

            var builder = new UriBuilder(ApiUrl)
            {
                Fragment = "/blank-layout?" + queries.ToString()
            };

            return builder.ToString();
        }

        /*
         *  GetActivationUrl returns the URL needed to activate the present device.
         *  No parameters are built into the URL because it is POSTed rather than GET'd.
         */
        internal string GetActivationUrl()
        {
            var builder = new UriBuilder(ApiUrl)
            {
                Path = "/v1/devices/activate"
            };

            return builder.ToString();
        }

        /*
         *  GetActivationUrl returns the URL needed to refresh the session token.
         *  No parameters are built into the URL because it is POSTed rather than GET'd.
         */
        internal string GetRefreshTokenUrl()
        {
            var builder = new UriBuilder(ApiUrl)
            {
                Path = "/v1/devices/tokens"
            };
            return builder.ToString();
        }


    }
}
