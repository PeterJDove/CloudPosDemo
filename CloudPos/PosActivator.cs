using RestSharp;
using System;
using System.Net;

namespace CloudPos
{
    public class PosActivator
    {
        public class Credentials
        {
            public string DeviceId { get; set; }
            public string DeviceKey { get; set; }
            public string Sequence { get; set; }
        }
        
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

        public PosToken ActivateAndRenewToken()
        {
            PosToken token = null;
            Credentials credentials = _repo.GetIfPresent(_config.ApiUrl);
            try
            {
                // existing credentials
                if (credentials != null)
                {
                    token = RenewToken(credentials);
                    if (token == null)
                    {
                        // incorrect credentials, need to activate
                        credentials = Activate();
                        if (credentials != null)
                        {
                            token = RenewToken(credentials);
                        }
                    }
                }
                else
                {
                    // no existing credentials
                    credentials = Activate();
                    if (credentials != null)
                    {
                        token = RenewToken(credentials);
                    }
                    else
                    {
                        return null;
                    }
                }

                if (token != null)
                {
                    _repo.SaveCredentials(_config.ApiUrl, credentials);
                }
                return token;
            }
            catch (ApplicationException e)
            {
                e.ToString();
                throw;
            }
        }

        public Credentials Activate()
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
                    var message = "Login failed: " + response.StatusCode.ToString();
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
