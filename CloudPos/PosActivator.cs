using RestSharp;
using System;
using System.Net;

namespace CloudPos
{
    /*
     *  CloudPOS is a secure system.  Each POS must be known to the system.
     *  
     *  Initially the POS is activated using a shared "Secret" (here contained
     *  in the _config.Secret.  A successful activation results in the return 
     *  of a "Credentials" object, returned as JSON, but modelled below.
     *  The Credentials object is persisted to disk, making the shared "Secret"
     *  redundant.  
     *  
     *  The Credentials are used periodically to a device access Token, which is
     *  forms part of the URL sent to the browser to open the CloudPOS session.
     *  It is then used within every request sent from the javascript application
     *  to the Afterpay Touch server.
     *  
     *  A token must be fetched each time this code is started; and periodically
     *  before it expires.  
     *  
     */
    internal class PosActivator
    {
        /*
         *  This inner class, PosActivator.Credentials, encapsulates the credentials
         *  obtained when you activate a device.  They are used to get or refresh 
         *  the token used for all subsequent requests. 
         */
        public class Credentials
        {
            public string DeviceId { get; set; }
            public string DeviceKey { get; set; }
            public string Sequence { get; set; }
        }

        /*
         *  This inner class, PosActivator.PosToken, encapuslates a newly fetched
         *  access token.  It also contains a number of seconds representing the 
         *  life of that token.  It is best to renew the token before it expires. 
         */
        public class PosToken
        {
            public string AccessToken { get; set; }
            public int ShouldRenewTokenInSeconds { get; set; }
        }





        private Configuration _config;
        private CredentialsRepository _repo = new CredentialsRepository();




        public PosActivator(Configuration config)
        {
            _config = config;
        }

        /*
         *  This method simply wraps the Activate and RenewToken methods
         *  (below) into one convenient form.   Activate is called if needed,
         *  and then RenewToken is called, and the renewed Token returned.
         */
        public PosToken ActivateIfNeededAndRenewToken()
        {
            PosToken token = null;
            Credentials credentials = _repo.GetIfPresent(_config.CredentialsKey);
            try
            {
                while (true)
                {
                    if (credentials == null)
                    {
                        credentials = Activate();
                    }
                    if (credentials != null)
                    {
                        token = RenewToken(credentials);
                        if (token != null)
                        {
                            _repo.SaveCredentials(_config.CredentialsKey, credentials);
                            break;
                        }
                        // incorrect credentials, need to activate
                        credentials = null; 
                    }
                }
                return token;
            }
            catch (ApplicationException)
            {
                throw;
            }
        }

        /*
         *  This method Activates the device using the "secret", and returns a
         *  new Credentials object which is later used to get a new access token.         * 
         */ 
        private Credentials Activate()
        {
            var activationUrl = _config.GetActivationUrl();
            var client = new RestClient();

            client.BaseUrl = new Uri(activationUrl);
            var request = new RestRequest(Method.POST);

            request.AddParameter("deviceSecret", _config.Secret);
            request.AddParameter("userName", _config.Operator);
            request.AddParameter("computerName", _config.HardwareName);

            var response = client.Execute<Credentials>(request);

            if (response.ErrorException == null)
            {
                if (response.StatusCode.Equals(HttpStatusCode.Created))
                {
                    return response.Data;
                }
                else
                {
                    var message = "Login failed: " + (int)response.StatusCode + " " + response.StatusCode;
                    // MessageBox.Show(message);
                    throw new ApplicationException(message);
                }
            }
            else
            {
                // MessageBox.Show(response.ErrorException.Message);
                throw new ApplicationException(response.ErrorException.Message);
            }
        }

        /*
         *  This method gets a new access token using the device Credentials.
         *  If the Credentials are invalid, nothing is returned, and the device
         *  will need to be (re)activated.        
         */
        public PosToken RenewToken(Credentials credentials)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(_config.GetRefreshTokenUrl())
            };

            var request = new RestRequest(Method.POST);

            request.AddParameter("deviceId", credentials.DeviceId);
            request.AddParameter("deviceKey", credentials.DeviceKey);

            var response = client.Execute<PosToken>(request);

            if (response.ErrorException == null)
            {
                if (response.StatusCode.Equals(HttpStatusCode.Forbidden))
                {
                    return null;
                }
                else if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    return response.Data;
                }
                throw new ApplicationException(response.StatusDescription);
            }
            else
            {
                throw new ApplicationException(response.ErrorException.Message, response.ErrorException);
            }
        }
    }
}
