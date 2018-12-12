using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using Touch.Tools;
using static System.Environment;


namespace CloudPos
{
    internal static class RollbackHandler
    {
        private static IniFile _rollbacks;
        private static Timer _rollbackTimer;
        private static string _apiUrl;

        private const string TYPE = "type";
        private const string DESC = "description";
        private const string VALUE = "value";
        private const string PENDING_ROLLBACK = "pending_rollback";
        private const string PENDING_REFUND = "pending_refund";

        static RollbackHandler()
        {
            var appData = Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "CloudPOS");
            string iniFileName = Path.Combine(appData, "Rollbacks.ini");
            if (File.Exists(iniFileName) == false)
            {
                var fileStream = File.Create(iniFileName);
                fileStream.Close();
                fileStream.Dispose();
            }
            _rollbacks = new IniFile(iniFileName);
        }


        internal static void AddItem(BasketItem basketItem)
        {
            string basketItemId = basketItem.Id.ToString();
            if (basketItem is PurchaseBasketItem)
            {
                var purchase = (PurchaseBasketItem)basketItem;
                _rollbacks.Write(basketItemId, TYPE, purchase.Type);
                _rollbacks.Write(basketItemId, DESC, purchase.Product.Description);
                _rollbacks.Write(basketItemId, VALUE, purchase.Product.Price.Amount);
                _rollbacks.Write(basketItemId, PENDING_ROLLBACK, true);
                _rollbacks.Write(basketItemId, PENDING_REFUND, false);
            }
            else if (basketItem is RefundBasketItem)
            {
                var refund = (RefundBasketItem)basketItem;
                _rollbacks.Write(basketItemId, TYPE, refund.Type);
                _rollbacks.Write(basketItemId, DESC, refund.Product.Description);
                _rollbacks.Write(basketItemId, VALUE, refund.Product.Price.Amount);
                _rollbacks.Write(basketItemId, PENDING_ROLLBACK, true);
                _rollbacks.Write(basketItemId, PENDING_REFUND, false);
            }
        }


        internal static void ProcessPendingRollbacks(string apiUrl)
        {
            if (_rollbackTimer == null)
            {
                _apiUrl = apiUrl;
                _rollbackTimer = new Timer(1000);
                _rollbackTimer.Elapsed += _rollbackTimer_Elapsed;
                _rollbackTimer.Start();
            }
        }


        private static void _rollbackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _rollbackTimer.Stop();
            var sectionNames = _rollbacks.GetSectionNames();
            if (sectionNames.Length > 0)
            {
                foreach (var basketItemId in sectionNames)
                {
                    if (_rollbacks.GetBoolean(basketItemId, PENDING_ROLLBACK))
                    {
                        var uri = new UriBuilder(_apiUrl)
                        {
                            Path = "/v1/baskets/item/" + basketItemId + "/rollback"
                        };
                        var restClient = new RestClient();
                        restClient.BaseUrl = new Uri(uri.ToString());
                        var request = new RestRequest(Method.POST);

                        var response = restClient.Execute(request);
                        if (response.StatusCode == HttpStatusCode.NoContent // This worked: Item was rolled back
                         || response.StatusCode == HttpStatusCode.NotFound) // Nothing to be done: Item not found
                        {
                            ResetFlag(basketItemId, PENDING_ROLLBACK);
                        }
                        else // Unexpected result.   We should try again, later.
                        {
                            Debug.Assert(false); 
                        }
                        _rollbackTimer.Start();
                        return; // Just process one Rollback at a timeTHATS 
                    }
                    // loop for next Item, if this one did not need rolling back
                }
            }
            //
            //  There were no (more) Rollbacks pending, so destroy the Timer.
            //
            _rollbackTimer = null;
        }


        internal static bool IsRefundDue(int basketItemId)
        {
            string section = basketItemId.ToString();
            return _rollbacks.GetBoolean(section, PENDING_REFUND, false);
        }


        internal static decimal RefundDueAmount(int basketItemId)
        {
            string section = basketItemId.ToString();
            if (_rollbacks.GetBoolean(section, PENDING_REFUND))
                return _rollbacks.GetDecimal(section, VALUE);

            return 0.00M;
        }


        internal static void RefundComplete(int basketItemId)
        {
            ResetFlag(basketItemId.ToString(), PENDING_REFUND);
        }


        private static void ResetFlag(string basketItemId, string flag)
        {
            _rollbacks.Write(basketItemId, flag, false);
            if (_rollbacks.GetBoolean(basketItemId, PENDING_ROLLBACK) == false
             && _rollbacks.GetBoolean(basketItemId, PENDING_REFUND) == false)
            {
                _rollbacks.EraseSection(basketItemId);
            }
        }

    }
}
