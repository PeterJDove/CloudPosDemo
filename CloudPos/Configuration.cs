using System;
using System.Web;

namespace CloudPos
{
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

        internal string GetBrowserUrl(string token)
        {
            var queries = HttpUtility.ParseQueryString("");
            queries["skinName"] = SkinName;
            queries["locale"] = Locale;
            queries["callBackService"] = "NPOS";  // ALERT!! may require a change to NPOS
            queries["accessToken"] = token;

            var builder = new UriBuilder(ApiUrl)
            {
                Fragment = "/blank-layout?" + queries.ToString()
            };

            return builder.ToString();
        }

        internal string GetActivationUrl()
        {
            var builder = new UriBuilder(ApiUrl)
            {
                Path = "/v1/devices/activate"
            };

            return builder.ToString();
        }

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
