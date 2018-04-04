using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static CloudPos.PosActivator;

namespace CloudPos
{
    /*
     *  This class provides a .NET API by which the CloudPOS application can be accessed.
     *  It hides the complexity of creating the JSON messages passed into the javascript
     *  application, and deconstructing those that are received in return.
     *  
     *  This class raises a number of .NET events that should be listened for by the
     *  POS application.
     *  
     * 
     * 
     * 
     *  This class also manages the expiry of the access token, and will automatically 
     *  renew it well before it is due to expire.
     */

    public class CloudPos : IDisposable
    {
        private Configuration _config;
        private PosToken _posToken;
        private Timer _tokenTimer;

        private CloudPosUI.ICloudPosUI _ui;
        private string _operator = "";
        public PosBasket _basket = new PosBasket();


        public EventHandler Ready;  // Raised when the javascript application reports that it is ready.
        public EventHandler ShowGUI; // Raised when the browser window is to be (or has been) brought to the fore.
        public EventHandler HideGUI; // Raised when the browser window is hidden, surrendering control to the POS.
        public EventHandler<BasketItem> ItemAdded; // Raised when an Item is to be added to the POS basket.
        public EventHandler<BasketItem> ItemRemoved; // Raised when an Item is to be taken out of the POS basket.
        public EventHandler<PosBasket> BasketCommitted; // Raised when the basket of eServices items is finalised.
        public EventHandler<string> VoucherAvailable; // Raised (usually after BasketCommitted) for each voucher to be printed.
        public EventHandler<string> StartDevice; // Raised when we want the POS to turn on an input device: Bar Code Reader or Card Swipe.
        public EventHandler<string> StopDevice; // Raised when we want the POS to turn off an input device.
        public EventHandler<string> DisplayMessage; // Raised when we want the POS to display a message.
        public EventHandler SyncBasket; // Not used. 
        public EventHandler<string> Error; // Raised when Touchpoint encounters a problem.

        public static string DEV_KEYBOARD = "KEYBOARD";
        public static string DEV_BARCODE = "BARCODE_SCANNER";
        public static string DEV_MAGSTRIPE = "MAGNETIC_STRIPE_READER";


        public CloudPos()
        {
            InstantiateBrowser("ie", "Cloud POS");
        }

        ~CloudPos() // Destructor
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_ui != null)
            {
                _ui.Dispose();
                _ui = null;
            }
        }


        public void InitPosWindow(Configuration config)
        {
            try
            {
                _config = config;
                RefreshPosToken();

                _ui.SetPosition(config.ClientLeft, config.ClientTop);
                _ui.SetClientSize(config.ClientWidth, config.ClientHeight);
                string url = config.GetBrowserUrl(_posToken.AccessToken);
                _ui.Navigate(url);
            }
            catch (ApplicationException)
            {
                throw;
            }
        }

        private void RefreshPosToken()
        {
            try
            {
                var posActivator = new PosActivator(_config);
                _posToken = posActivator.ActivateIfNeededAndRenewToken();

                if (_posToken.ShouldRenewTokenInSeconds > 1800) // 30 minutes
                {
                    _tokenTimer = new Timer();
                    _tokenTimer.Elapsed += _tokenTimer_Elapsed;
                    _tokenTimer.Interval = 1000 * (_posToken.ShouldRenewTokenInSeconds - 1800);
                    _tokenTimer.Enabled = true;
                }
            }
            catch (ApplicationException)
            {
                throw;
            }
        }

        private void _tokenTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _tokenTimer.Enabled = false;
            RefreshPosToken();

            var message = new SetDeviceAccessTokenMessage
            {
                AccessToken = _posToken.AccessToken
            };
            SendJsonRequest(message);
        }

        public string Operator
        {
            get { return _operator; }
            set { _operator = value ?? ""; }
        }

        public PosBasket Basket
        {
            get { return _basket; }
        }

        private void _ui_Notify(object sender, string json)
        {
            var posEvent = JsonConvert.DeserializeObject<PosEvent>(json, new PosEventJsonConverter());
            HandlePosEvent(posEvent);
        }

        public void ShowTouchpointUI(string shortcutOrEan, string retailerTransactionId = "")
        {
            if (_basket != null && _basket.Committed)
            {
                _basket.Clear();
            }
            var message = new ShowUserInterfaceMessage
            {
                BasketId = _basket.StrId,
                ShortcutOrEan = shortcutOrEan,
                RetailerTransactionId = retailerTransactionId,
                OperatorId = _operator,
            };
            SendJsonRequest(message);
            _ui.Show();
        }

        public void CommitBasket()
        {
            var message = new CommitBasketMessage()
            {
                BasketId = _basket.StrId,
                OperatorId = _operator,
                CommittedAt = DateTimeOffset.Now
            };
            SendJsonRequest(message);
        }

        public void RemoveItem(string purchaseId, string reason)
        {
            var message = new RemoveFromBasketMessage()
            {
                BasketId = _basket.StrId,
                OperatorId = _operator,
                PurchaseId = purchaseId,
                Reason = reason
            };
            SendJsonRequest(message);
        }

        public void ClearBasket()
        {
            if (_basket != null && !_basket.Committed)
            {
                var message = new ClearBasketMessage()
                {
                    BasketId = _basket.StrId
                };
                SendJsonRequest(message);
            }
            _basket = new PosBasket();
        }

        public void GetVoucher(string voucherUrl)
        {
            var message = new GetVoucherMessage()
            {
                VoucherUrl = voucherUrl
            };
            SendJsonRequest(message);
        }

        public void DeviceData(string device, string data)
        {
            var message = new DeviceDataMessage()
            {
                Device = device,
                Data = data
            };
            SendJsonRequest(message);
        }

        public void AddItem(string shortcutOrEan, string retailerTransactionId = "")
        {
            var message = new AddToBasketMessage()
            {
                ShortcutOrEan = shortcutOrEan,
                OperatorId = _operator,
                RetailerTransactionId = retailerTransactionId,
                BasketId = _basket.StrId,
            };
            SendJsonRequest(message);
        }
        
        private void SendJsonRequest(PosMessage posMessage)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(posMessage, settings);
            _ui.SendMessage(json);
        }


        private void HandlePosEvent(PosEvent posEvent)
        {
            if (posEvent == null)
            {
                // Debug.Assert(false);
                return;
            }
            switch (posEvent.EventName)
            {
                case PosEventType.READY:
                    Ready?.Invoke(this, null);
                    break;
                case PosEventType.SHOW_GUI:
                    _ui.Show();
                    ShowGUI?.Invoke(this, null);
                    break;
                case PosEventType.HIDE_GUI:
                    _ui.Hide();
                    HideGUI?.Invoke(this, null);
                    break;
                case PosEventType.ADD_TO_BASKET:
                    var addedItem = (AddToBasketEvent)posEvent;
                    _basket.Id = addedItem.BasketId;
                    _basket.Items.Add(addedItem.BasketItem);
                    ItemAdded?.Invoke(this, addedItem.BasketItem);
                    break;
                case PosEventType.BASKET_COMMITTED:
                    var basketCommittedEvent = (BasketCommittedEvent)posEvent;
                    if (basketCommittedEvent.Basket.Committed)
                    {
                        Debug.Assert(_basket.Id == basketCommittedEvent.BasketId); // Ensure same Basket!
                        _basket.Clear();
                        _basket.Id = basketCommittedEvent.BasketId;
                        foreach(var basketItem in basketCommittedEvent.Basket.basketItems)
                        {
                            _basket.Items.Add(basketItem); // Now with Touch ID, and Vouchers (we trust!)
                            if (basketItem is PurchaseBasketItem)
                            {
                                var purchase = (PurchaseBasketItem)basketItem;
                                if (purchase.Vouchers.Count > 0)
                                    GetVoucher(purchase.Vouchers[0].Link);
                            }
                            else if (basketItem is RefundBasketItem)
                            {
                                var refund = (RefundBasketItem)basketItem;
                                if (refund.RefundVoucher != null)
                                    GetVoucher(refund.RefundVoucher.Link);
                            }
                        }
                        _basket.Committed = true;
                        BasketCommitted?.Invoke(this, _basket);
                    }
                    // _basket.Clear();
                    break;
                case PosEventType.REMOVE_FROM_BASKET:
                    var removeFromBasketEvent = (RemoveFromBasketEvent)posEvent;
                    foreach (var basketItem in _basket.Items)
                    {
                        if (basketItem.Id == removeFromBasketEvent.BasketItemId)
                        {
                            _basket.Items.Remove(basketItem);
                            ItemRemoved?.Invoke(this, basketItem);
                            break;
                        }
                    }
                    break;
                case PosEventType.PRINT_VOUCHER: // I think this is an "unexpected" voucher...
                    var PrintVoucherEvent = (PrintVoucherEvent)posEvent;
                    VoucherAvailable?.Invoke(this, PrintVoucherEvent.Data);
                    break;
                case PosEventType.VOUCHER_HTML: //...whereas this is the response to the BasketGetVoucherEvent message 
                    var voucherHtmlEvent = (VoucherHtmlEvent)posEvent;
                    VoucherAvailable?.Invoke(this, voucherHtmlEvent.VoucherHtml);
                    break;
                case PosEventType.START_DEVICE:
                    var startDeviceEvent = (StartDeviceEvent)posEvent;
                    StartDevice?.Invoke(this, startDeviceEvent.Device);
                    break;
                case PosEventType.STOP_DEVICE:
                    var stopDeviceEvent = (StopDeviceEvent)posEvent;
                    StopDevice?.Invoke(this, stopDeviceEvent.Device);
                    break;
                case PosEventType.DISPLAY_MESSAGE:
                    var displayMessageEvent = (DisplayMessageEvent)posEvent;
                    DisplayMessage?.Invoke(this, displayMessageEvent.Message);
                    break;
                case PosEventType.SYNC_BASKET:
                    var synBasketEvent = (SyncBasketEvent)posEvent;
                    SyncBasket?.Invoke(this, null);
                    break;
                case PosEventType.ERROR:
                    var errorEvent = (ErrorEvent)posEvent;
                    Error?.Invoke(this, errorEvent.Reason);
                    break;
                default:
                    Debug.Assert(false); // Unknown PosEventType
                    break;
            }
        }

        private void InstantiateBrowser(string type, string title)
        {
            switch (type.ToLower())
            {
                case "ie":
                    _ui = new CloudPosIE.UI(title, false);
                    _ui.Notify += _ui_Notify;
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
        }


    }
}
