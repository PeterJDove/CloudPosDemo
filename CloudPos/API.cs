using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Touch.CloudPos.Model;

/// <summary>
/// The main Class Library (DLL) that encapsulates the javascript API calls documented at http://esp-api-com/cloudpos/. 
/// </summary>
namespace Touch.CloudPos
{
    /// <summary>
    /// This class provides a .NET API by which the CloudPOS application can be accessed.
    /// It hides the complexity of creating the JSON messages passed into the javascript
    /// application, and deconstructing those that are received in return.
    /// 
    /// This class raises a number of .NET events that should be listened for by the
    /// POS application.
    /// 
    /// This class also manages the expiry of the access token, and will automatically
    /// renew it well before it is due to expire.
    /// </summary>
    public class API : IDisposable
    {
        private Configuration _config;
        private PosActivator.PosToken _posToken;
        private System.Timers.Timer _tokenTimer;
        private System.Timers.Timer _timeoutTimer;
        private string _pendingEvent;

        private CloudPosUI.ICloudPosUI _ui;
        private string _operator = "";
        private PosBasket _basket = new PosBasket();
        private RollbackHandler _rollbackHandler = new RollbackHandler();


        /// <summary>
        /// Raised when the javascript application reports that it is ready.
        /// </summary>
        /// <remarks>
        /// <b>Ready</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler">EventHander</see>
        /// delegate, passing no EventArgs.
        /// </remarks>
        public event EventHandler Ready;

        /// <summary>
        /// Raised when the browser window is to be (or has been) brought to the fore.
        /// </summary>
        /// <remarks>
        /// <b>ShowGUI</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler">EventHander</see>
        /// delegate, passing no EventArgs.
        /// </remarks>
        public event EventHandler ShowGUI;

        /// <summary>
        /// Raised when the browser window is hidden, surrendering control to the POS.
        /// </summary>
        /// <remarks>
        /// <b>HideGUI</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler">EventHander</see>
        /// delegate, passing no EventArgs.
        /// </remarks>
        public event EventHandler HideGUI;

        /// <summary>
        /// Raised when an Item is to be added to the POS basket.
        /// </summary>
        /// <remarks>
        /// <b>ItemAdded</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing the <see cref="BasketItem"/> to be added.
        /// </remarks>
        public event EventHandler<BasketItem> ItemAdded;

        /// <summary>
        /// Raised when an Item is to be taken out of the POS basket.
        /// </summary>
        /// <remarks>
        /// <b>ItemRemoved</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing the <see cref="BasketItem"/> as the EventArgs.
        /// </remarks>
        public event EventHandler<BasketItem> ItemRemoved;

        /// <summary>
        /// Raised when the basket of eServices items is finalised.
        /// </summary>
        /// <remarks>
        /// <b>BasketCommitted</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing the entire <see cref="PosBasket"/> with finalised items as the EventArgs.
        /// </remarks>
        public event EventHandler<PosBasket> BasketCommitted;

        /// <summary>
        /// Raised when the commit fails fatally, or times out. 
        /// </summary>
        /// <remarks>
        /// <b>BasketCommitFailed</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing the entire <see cref="PosBasket"/> with unfinalised items as the EventArgs.
        /// </remarks>
        public event EventHandler<PosBasket> BasketCommitFailed;

        /// <summary>
        /// Raised when there a voucher to be printed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>VoucherAvailable</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing a string containing the voucher HTML as the EventArgs.
        /// </para>
        /// <para>
        /// The most common use of this
        /// event is after the basket has been committed, returning the vouchers
        /// representing the products purchased: A separate event for each voucher.
        /// </para>
        /// <para>
        /// This event may also be raised pre-tender / pre-commit for vouchers that
        /// are available for immediate printing, even if no item is to be added to
        /// the basket, e.g. voucher reprints, or POS sales reports.
        /// </para>
        /// </remarks>
        public event EventHandler<string> VoucherAvailable;

        /// <summary>
        /// Raised when we want the POS to turn on an input device: Bar Code Reader or Magnetic Card reader.
        /// </summary>
        /// <remarks>
        /// <b>StartDevice</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing as the EventArgs:
        /// <list type="bullet">
        /// <item><see cref="DEV_BARCODE">DEV_BARCODE</see></item>
        /// <item><see cref="DEV_MAGSTRIPE">DEV_MAGSTRIPE</see></item>
        /// </list>
        /// </remarks>
        public event EventHandler<string> StartDevice;

        /// <summary>
        /// Raised when we want the POS to turn off an input device.
        /// </summary>
        /// <remarks>
        /// <b>StopDevice</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing as the EventArgs: <see cref="DEV_BARCODE">DEV_BARCODE</see> or <see cref="DEV_MAGSTRIPE">DEV_MAGSTRIPE</see>.
        /// </remarks>
        public event EventHandler<string> StopDevice;

        /// <summary>
        /// Raised when we want the POS to display a message.
        /// </summary>
        /// <remarks>
        /// <b>DisplayMessage</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing as the EventArgs: <see cref="DEV_BARCODE">DEV_BARCODE</see> or <see cref="DEV_MAGSTRIPE">DEV_MAGSTRIPE</see>.
        /// </remarks>
        public event EventHandler<string> DisplayMessage;


        /// <summary>
        /// Raised to return the result of a IsSimpleProduct request.
        /// </summary>
        /// <remarks>
        /// <b>SimpleProduct</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing <see cref="SimpleProductInfo"/> as the EventArgs.
        /// </remarks>
        public event EventHandler<SimpleProductInfo> SimpleProduct;

        /// <summary>
        /// Raised to trigger a SyncBasket procedure.   Not currently used.
        /// </summary>
        /// <remarks>
        /// <b>SyncBasket</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler">EventHander</see>
        /// delegate, passing no EventArgs.
        /// </remarks>
        public event EventHandler SyncBasket; // Not used. 

        /// <summary>
        /// Raised when a problem is encountered.
        /// </summary>
        /// <remarks>
        /// <b>Error</b> uses the 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.eventhandler-1">EventHander&lt;TEventArgs&gt;</see>
        /// delegate, passing the error description as the EventArgs.
        /// </remarks>
        public event EventHandler<string> Error;

        /// <summary>
        /// Used in <see cref="StartDevice"/> and <see cref="StopDevice"/> events, to indicate the physical keyboard.
        /// </summary>
        /// <remarks>
        /// Contains "KEYBOARD".
        /// </remarks>
        public static string DEV_KEYBOARD = "KEYBOARD";

        /// <summary>
        /// Used in <see cref="StartDevice"/> and <see cref="StopDevice"/> events, to indicate the Barcode Reader.
        /// </summary>
        /// <remarks>
        /// Contains "BARCODE_READER".
        /// </remarks>
        public static string DEV_BARCODE = "BARCODE_READER";

        /// <summary>
        /// Used in <see cref="StartDevice"/> and <see cref="StopDevice"/> events, to indicate the Magnetic Card Strip reader.
        /// </summary>
        /// <remarks>
        /// Contains "MAGNETIC_STRIPE_READER".
        /// </remarks>
        public static string DEV_MAGSTRIPE = "MAGNETIC_STRIPE_READER";

        /// <summary>
        /// Initializes a new instance of the <see cref="API"/> class, using a .NET WebBrowser (Internet Explorer) client.
        /// </summary>
        public API()
        {
            InstantiateBrowser("ie", "Cloud POS"); // Internet Explorer (.NET WebBrowser) 
        }

        ~API() // Destructor
        {
            Dispose();
        }

        /// <summary>
        /// Implements <c>Dispose</c> method of <c>IDisposable</c> interface.
        /// </summary>
        public void Dispose()
        {
            if (_ui != null)
            {
                _ui.Dispose();
                _ui = null;
            }
        }

        /// <summary>
        /// InitPosWindow must be called first to initiate a CloudPOS session running
        /// in the browser.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>InitPosWindow</b> takes a populated Configuration object to define which 
        /// URL to connect to, how to identify the device, where to put the browser
        /// window, etc.
        /// </para>
        /// <para>
        /// You might think that this could have been incorporated into the API
        /// Constructor; however the client (POS) needs to hook into the Ready event
        /// of the newly created CloudPos object, before calling this method.
        /// </para>
        /// </remarks>
        /// <param name="config">A <c>Configuration</c> object, containing position and size of the browser window.</param>
        public void InitPosWindow(Configuration config)
        {
            try
            {
                _config = config;
                string url = config.Url;
                if (_config.Secret == null || _config.Secret.ToLower() != "ignore")
                {
                    RefreshPosToken();
                    url = config.GetBrowserUrl(_posToken.AccessToken);
                }
                _ui.SetPosition(config.ClientRect.Left, config.ClientRect.Top);
                _ui.SetClientSize(config.ClientRect.Width, config.ClientRect.Height);
                _ui.Navigate(url);
            }
            catch (ApplicationException)
            {
                throw;
            }
        }

        /// <summary>
        /// RefreshPosToken is called from InitPosWindow (above) when the CloudPOS session
        /// is first started; and again from _tokenTimer_Elapsed (below) when the access token
        /// is about to expire.
        /// 
        /// This method also starts the Expiry timer, set to trigger 30 minutes before the 
        /// newly obtained token is due to expire.
        /// </summary>
        private void RefreshPosToken()
        {
            try
            {
                var posActivator = new PosActivator(_config);
                _posToken = posActivator.ActivateIfNeededAndRenewToken();

                if (_posToken.ShouldRenewTokenInSeconds > 1800) // 30 minutes
                {
                    _tokenTimer = new System.Timers.Timer();
                    _tokenTimer.AutoReset = false;
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

        /// <summary>
        /// This timer event fires when the access token is approaching expiry.
        /// </summary>
        /// <param name="sender">The Timer itself</param>
        /// <param name="e">and <c>ElapsedEventArgs</c> object.  Not used.</param>
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


        /// <summary>
        /// Gets or sets the name (or ID) of the POS Operator to be included in Host requests.
        /// </summary>
        public string Operator
        {
            get { return _operator; }
            set { _operator = value ?? ""; }
        }

        /// <summary>
        /// Gets the current POS Basket
        /// </summary>
        public PosBasket Basket
        {
            get { return _basket; }
        }

        /// <summary>
        /// Gets or sets a flag that forces the API to treat any Commit as failed.
        /// </summary>
        /// <remarks>
        /// This is for testing only, to force REFUND DUE vouchers to be produced, and the transaction to be rolled back.
        /// </remarks>
        public bool FailOnCommit { get; set; }

        /*
         *  _ui_Notify is the listener hooked to listen for messages from the CloudPOS
         *  application.  The real work is done in HandlePosEvent.
         */
        private void _ui_Notify(object sender, string json)
        {
            var posEvent = JsonConvert.DeserializeObject<PosEvent>(json, new PosEventJsonConverter());
            HandlePosEvent(posEvent);
        }

        /// <summary>
        /// ShowGui is called to make the browser window be displayed.
        /// </summary>
        /// <param name="shortcutOrEan">The application screen to be presented first.
        /// If empty, the default "home" or main menu is presented.</param>
        /// <param name="retailerTransactionId">Optionally allows a POS transaction reference
        /// to be attached to the eServices transaction/s.</param>
         public void ShowGui(string shortcutOrEan, string retailerTransactionId = "")
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

        /// <overloads>
        /// Adds an item to the basket <i>without</i> showing the CloudPOS GUI.
        /// </overloads>
        /// <summary>
        /// Adds a (simple) item to the basket <i>without</i> showing the CloudPOS GUI, optionally passing a POS Transaction reference.
        /// </summary>
        /// <param name="shortcutOrEan">the product to be added.</param>
        /// <param name="retailerTransactionId">Optionally allows a POS transaction reference
        /// to be attached to the eServices transaction.</param>
        public void AddItem(string shortcutOrEan, string retailerTransactionId = "")
        {
            AddItem(shortcutOrEan, retailerTransactionId, null);
        }

        /// <summary>
        /// Adds a (complex) item to the basket <i>without</i> showing the CloudPOS GUI, passing data about the item.
        /// </summary>
        /// <param name="shortcutOrEan">the product to be added.</param>
        /// <param name="data">A Dictionary of name/value pairs needed for the transaction</param>
        public void AddItem(string shortcutOrEan, Dictionary<string, string> data)
        {
            AddItem(shortcutOrEan, "", data);
        }

        /// <summary>
        /// Adds a (complex) item to the basket <i>without</i> showing the CloudPOS GUI, passing both a POS Transaction reference, and data about the item.
        /// </summary>
        /// <param name="shortcutOrEan">the product to be added.</param>
        /// <param name="retailerTransactionId">A POS transaction reference to be attached to the eServices transaction.</param>
        /// <param name="data">A Dictionary of name/value pairs needed for the transaction</param>
        public void AddItem(string shortcutOrEan, string retailerTransactionId, Dictionary<string, string> data)
        {
            if (_basket != null && _basket.Committed)
            {
                _basket.Clear();
            }
            var message = new AddToBasketMessage()
            {
                ShortcutOrEan = shortcutOrEan,
                OperatorId = _operator,
                RetailerTransactionId = retailerTransactionId,
                BasketId = _basket.StrId,
                Data = data,
            };
            SendJsonRequest(message);
        }

        /// <summary>
        /// Process a product Return / Refund.
        /// </summary>
        /// <remarks>
        /// <para>This method does not wrap a single, dedicated "ReturnEvent" call in the point-js API.</para>
        /// <para>Instead:</para>
        /// <para>
        /// If no arguments are provided, we simply call <see cref="ShowGUI"/> with the shortcut "refund".
        /// Otherwise, if a Touch Transaction ID and Reason is supplied, we call the GUI-less 
        /// <see cref="AddItem"/>
        /// with the shortcut = "refund" and a Dictionary containing the Touch Transaction ID and Reason.
        /// </para>
        /// </remarks>
        /// <param name="transactionId"></param>
        /// <param name="reason"></param>
        public void Refund(string transactionId = "", RefundReason reason = RefundReason.OTHER)
        {
            if (string.IsNullOrEmpty(transactionId))
                ShowGui("refund");
            else
            {
                var data = new Dictionary<string, string>();
                data.Add("transactionId", transactionId);
                data.Add("reason", reason.ToString());
                AddItem("refund", data);
            }
        }

        /// <summary>
        /// Gets <c>true</c> if the Basket referenced by the Basket ID has 
        /// one or more items for which a (non-zero) Refund Amount is Due.
        /// </summary>
        /// <remarks>
        /// If only a Basket ID is supplied, all items in the basket are considered.
        /// If an Item ID is supplied as well, then just that item (if it exists) is considered.
        /// </remarks>
        /// <param name="basketId">The ID of the Basket to check.</param>
        /// <param name="itemId">If not zero, specifies that only a single Item in the Basket is to be considered</param>
        /// <returns>Returns <c>true</c> if the Basket has an amount to be refunded.</returns>
        public bool IsFailedCommitRefundDue(int basketId, int itemId = 0)
        {
            return _rollbackHandler.IsFailedCommitRefundDue(basketId, itemId);
        }

        /// <summary>
        /// Gets the Refund Amount due for one or more items in the Basket
        /// referenced by the Basket ID.  
        /// </summary>
        /// <remarks>
        /// If only a Basket ID is supplied, all items in the basket are considered.
        /// If an Item ID is supplied as well, then just that item (if it exists) is considered.
        /// </remarks>
        /// <param name="basketId">The ID of the Basket to check.</param>
        /// <param name="itemId">If not zero, specifies that only a single Item in the Basket is to be considered</param>
        /// <returns>the amount of the Refund Due.</returns>
        public decimal FailedCommitRefundAmount(int basketId, int itemId = 0)
        {
            return _rollbackHandler.FailedCommitRefundAmount(basketId, itemId);
        }


        /// <summary>
        /// Tells CloudPOS to mark one or more BasketItems, for which there was a Refund Due,
        /// that the amount 
        /// </summary>
        /// <remarks>
        /// If only a Basket ID is supplied, all items in the basket are considered.
        /// If an Item ID is supplied as well, then just that item (if it exists) is considered.
        /// </remarks>
        /// <param name="basketId">The ID of the Basket to check.</param>
        /// <param name="itemId">If not zero, specifies that only a single Item in the Basket is to be considered</param>
        /// <returns>the amount of the Refund Due.</returns>
        public void FailedCommitRefundComplete(int basketId, int itemId = 0)
        {
            _rollbackHandler.FailedCommitRefundComplete(basketId, itemId);
        }

        /// <summary>
        /// CommitBasket finalises all pending transactions, committing all of the eService transactions in the basket.
        /// </summary>
        /// <remarks>
        /// This method should be called after the POS basket has been paid for (tendered).
        /// </remarks>
        public void CommitBasket()
        {
            var message = new CommitBasketMessage()
            {
                BasketId = _basket.StrId,
                OperatorId = _operator,
                CommittedAt = DateTimeOffset.Now
            };
            SendJsonRequest(message);
            StartTimeoutTimer(message.EventIdentifier, 60);
        }

        private void StartTimeoutTimer(string eventIdentifier, double timeoutSeconds)
        {
            Debug.Assert (_timeoutTimer == null);
            _pendingEvent = eventIdentifier;
            _timeoutTimer = new System.Timers.Timer();
            _timeoutTimer.AutoReset = false;
            _timeoutTimer.Interval = 1000 * timeoutSeconds;
            _timeoutTimer.Elapsed += _timeoutTimer_Elapsed;
            _timeoutTimer.Enabled = true;
        }

        /*
         *  This timer event fires when no response to a CommitBasket message is received in good time.
         */
        private void _timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timeoutTimer = null;
            switch (_pendingEvent)
            {
                case PosMessageType.COMMIT_BASKET:
                    ProcessFailedCommit();
                    return;
                default:
                    Debug.Assert(false);
                    return;
            }
        }

        /// <summary>
        /// Removes (i.e. cancels or voids) an item from the basket before the basket is committed.
        /// </summary>
        /// <remarks>
        /// This method should be used, for example, if an item is added in error, or the customer changes their mind.
        /// </remarks>
        /// <param name="purchaseId"></param>
        /// <param name="reason"></param>
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


        /// <summary>
        /// Readies the Basket for use by ensuring that it is empty, and refreshed with no "Basket ID".
        /// </summary>
        /// <remarks>
        /// This method may be called by the POS client before the basket 
        /// is committed, to cancel the entire customer transaction.
        /// In that case, a ClearBasketMessage is sent to the javascript application.   
        /// </remarks>
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


        /// <summary>
        /// Injects data read from a a peripheral device, in response to a <see cref="StartDevice"/> event.
        /// </summary>
        /// <param name="device">The name of the device: <see cref="DEV_KEYBOARD"/>, <see cref="DEV_BARCODE"/> or <see cref="DEV_MAGSTRIPE"/>.</param>
        /// <param name="data">The data read from the device</param>
        public void DeviceData(string device, string data)
        {
            var message = new DeviceDataMessage()
            {
                Device = device,
                Data = data
            };
            SendJsonRequest(message);
        }

    
        /// <summary>
        /// Enquires whether a known product is a simple product, or not.
        /// </summary>
        /// <param name="shortcutOrEan">Identifies the product being enquired on.</param>
        /// <remarks>
        /// This method is usually called to enquire whether a product can be added to the 
        /// basket with a call to <see cref="AddItem(string, string)"/>, using just the product ID 
        /// (i.e. shortcutOrEan), without extra data being needed. 
        /// </remarks>
        public void IsSimpleProduct(string shortcutOrEan)
        {
            var message = new SimpleProductMessage()
            {
                ShortcutOrEan = shortcutOrEan
            };
            SendJsonRequest(message);
        }

        /// <summary>
        /// Fetch a voucher (HTML) from a URL.
        /// </summary>
        /// <remarks>
        /// GetVoucher is called from within HandlePosEvent(below) when processing 
        /// the BASKET_COMMITTED event.  It is called to fetch each voucher resulting
        /// from the transactions in the basket.
        /// </remarks>
        /// <param name="voucherUrl"></param>
        private void GetVoucher(string voucherUrl)
        {
            var message = new GetVoucherMessage()
            {
                VoucherUrl = voucherUrl
            };
            SendJsonRequest(message);
        }



        /*
         *  SendJsonRequest serializes and sends the PosMessage resulting from the 
         *  methods above, into the javascript application running in the browser.
         */
        private void SendJsonRequest(PosMessage posMessage)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(posMessage, settings);
            _ui.SendMessage(json);
        }

        /*
         *  HandlePosEvent is called from _ui_Notify event listener (above).
         *  
         *  Most messages are simply turned into .NET events.  A few do more, 
         *  especially:
         *      ADD_TO_BASKET : Updates our own Basket object with new items
         *      REMOVE_FROM_BASKET : Removes item from our own Basket
         *      BASKET_COMMITTED : Updates the Basket state; and fetches all the vouchers
         */
        private void HandlePosEvent(PosEvent posEvent)
        {
            if (posEvent == null)
            {
                // Debug.Assert(false);
                return;
            }
            if (_timeoutTimer != null)
            {
                _timeoutTimer.Enabled = false;
                _timeoutTimer = null;
            }
            switch (posEvent.EventName)
            {
                case PosEventType.READY:
                    Ready?.Invoke(this, null);
                    _rollbackHandler.ProcessPendingRollbacks(_config.Url);
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
                    // Check:
                    //  (1) Incoming .BasketId is what we expect.  This is just a sanity check, and should never fail.
                    //  (2) basketCommittedEvent.Basket.Committed = TRUE
                    //  (3) this.FailOnCommit = FALSE.  This may be set to TRUE to test correct processing of a Commit failure.
                    if (_basket.Id == basketCommittedEvent.BasketId && basketCommittedEvent.Basket.Committed && !FailOnCommit) 
                    {
                        _basket.Clear();
                        _basket.Id = basketCommittedEvent.BasketId;
                        foreach(var basketItem in basketCommittedEvent.Basket.basketItems)
                        {
                            _basket.Items.Add(basketItem); // Now with Touch ID, and Vouchers (we trust!)
                            if (basketItem is PurchaseBasketItem)
                            {
                                var purchase = (PurchaseBasketItem)basketItem;
                                if (purchase.Vouchers?.Count > 0)
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
                    else
                    {
                        ProcessFailedCommit();
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
                case PosEventType.PRINT_VOUCHER: // This is an "unexpected" ad hoc voucher...
                    var PrintVoucherEvent = (PrintVoucherEvent)posEvent;
                    VoucherAvailable?.Invoke(this, PrintVoucherEvent.Data);
                    break;
                case PosEventType.VOUCHER_HTML: //...whereas this is the response to the BasketGetVoucherEvent message 
                    var voucherHtmlEvent = (VoucherHtmlEvent)posEvent;
                    if (!string.IsNullOrEmpty(voucherHtmlEvent.VoucherHtml))
                    {
                        VoucherAvailable?.Invoke(this, voucherHtmlEvent.VoucherHtml);
                    }
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
                case PosEventType.SIMPLE_PRODUCT:
                    var simpleProductEvent = (SimpleProductEvent)posEvent;
                    SimpleProduct?.Invoke(this, new SimpleProductInfo(simpleProductEvent));
                    break;
                case PosEventType.SYNC_BASKET:
                    var synBasketEvent = (SyncBasketEvent)posEvent;
                    SyncBasket?.Invoke(this, null);
                    break;
                case PosEventType.ERROR:
                    var errorEvent = (ErrorEvent)posEvent;
                    if (errorEvent.Method == "BasketCommitBasket")
                    {
                        ProcessFailedCommit();
                    }
                    Error?.Invoke(this, errorEvent.Reason);
                    break;
                default:
                    Debug.Assert(false); // Unknown PosEventType
                    break;
            }
        }

        private void ProcessFailedCommit()
        {
            _rollbackHandler.AddBasket(_basket);
            _rollbackHandler.ProcessPendingRollbacks(_config.Url);
            BasketCommitFailed(this, _basket);
        }


        /*
         *  InstantiateBrowser is called from the Constructor to instantiate an
         *  implementation of the ICloudPosUI interface.
         *  
         *  At present, only one implementation is supported: CloudPosIE.UI
         *  
         *  The commented-out code is for other browser types that were being tried,
         *  but which are not presently included.
         */
        private void InstantiateBrowser(string type, string title)
        {
            switch (type.ToLower())
            {
                case "ie":
                    _ui = new CloudPosIE.UI(title, false);
                    _ui.Notify += _ui_Notify;
                    break;
                //case "cefsharp":
                //    _ui = new CloudPosCef.UI(title, false);
                //    _ui.Notify += _ui_Notify;
                //    break;
                //case "essentialObjects":
                //    _ui = new CloudPosEO.UI(title, false);
                //    _ui.Notify += _ui_Notify;
                //    break;
                //case "awesomium":
                //    _ui = new CloudPosAwesomium.UI(title, false);
                //    _ui.Notify += _ui_Notify;
                //    break;
            }
        }



    }
}
