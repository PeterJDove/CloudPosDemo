using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Touch.DummyPos
{
    public partial class FormMain : Form
    {
        public CloudPos.CloudPos CloudPOS;
        public CloudPos.WebPos WebPOS;


        #region Form Instantiation and Closing
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            var ini = Program.IniFile();
            var width = ini.GetInt("MAIN", "width");
            var height = ini.GetInt("MAIN", "height");
            if (width > 0 && height > 0)
            {
                this.Size = new Size(width, height);
                this.Left = ini.GetInt("MAIN", "left");
                this.Top = ini.GetInt("MAIN", "top");
            }
            else
            {
                this.Top = 50;
                this.Left = Screen.PrimaryScreen.Bounds.Width - this.Top - this.Width;
            }
            this.TopMost = ini.GetBoolean("GENERAL", "topmost", false);
            tabControl.SelectedIndex = ini.GetInt("GENERAL", "tab", 0);

            InitializeOptions();
            RefreshButtonStates();

            _cloudPosButtons.Add(btnShowGui);
            _cloudPosButtons.Add(btnShowGuiAt);
            _cloudPosButtons.Add(btnRefund);
            _cloudPosButtons.Add(btnAddItem);
            _cloudPosButtons.Add(btnCommit);
            _cloudPosButtons.Add(btnRemoveItem);
            _cloudPosButtons.Add(btnClearBasket);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var ini = Program.IniFile();
            ini.Write("MAIN", "width", this.Width);
            ini.Write("MAIN", "height", this.Height);
            ini.Write("MAIN", "left", this.Left);
            ini.Write("MAIN", "top", this.Top);
        }

        public void UpdateGUI(MethodInvoker methodInvoker)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(methodInvoker);
            }
            else
            {
                methodInvoker.Invoke();
            }
        }
        #endregion

        #region CloudPOS Instantiation
        private void chkCloudPos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCloudPos.Checked)
            {
                chkWebPos.Checked = false;
                optCloudPOS.Checked = true;
                ClearLogs();
                if (CloudPOS == null)
                {
                    if (EnsureCloudPosReady() == false)
                        chkCloudPos.Checked = false;
                }
            }
            else
            {
                if (CloudPOS != null)
                {
                    DestroyCloudPos();
                    ClearBasket();
                }
            }
            RefreshButtonStates();
        }

        private bool EnsureCloudPosReady(bool playWhenReady = false)
        {
            UpdateGUI(() =>
            {
                this.Cursor = Cursors.WaitCursor;
                tabControl.SelectedIndex = 0;
                chkCloudPos.BackColor = Color.Yellow;
            });

            var config = Program.Options().CloudPosConfiguration();
            InstantiateCloudPos(config.Operator);
            try
            {
                CloudPOS.InitPosWindow(config);
            }
            catch (ApplicationException appEx)
            {
                var message = "Failed to Activate:\n\n" + appEx.Message
                    + "\n\nCheck CloudPOS URL and Secret in Options.";
                MessageBox.Show(message, "Activate CloudPOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                UpdateGUI(() => {
                    chkCloudPos.BackColor = SystemColors.Control;
                    this.Cursor = Cursors.Default;
                });
                return false;
            }
            UpdateGUI(() => this.Cursor = Cursors.Default);
            return true;
        }

        private void InstantiateCloudPos(string posOperator)
        {
            CloudPOS = new CloudPos.CloudPos();
            CloudPOS.Ready += CloudPos_Ready;
            CloudPOS.ShowGUI += CloudPos_ShowGUI;
            CloudPOS.HideGUI += CloudPos_HideGUI;
            CloudPOS.ItemAdded += CloudPos_ItemAdded;
            CloudPOS.ItemRemoved += CloudPos_ItemRemoved;
            CloudPOS.BasketCommitted += CloudPos_BasketCommitted;
            CloudPOS.BasketCommitFailed += CloudPos_BasketCommitFailed;
            CloudPOS.VoucherAvailable += CloudPos_VoucherAvailable;
            CloudPOS.DisplayMessage += CloudPos_DisplayMessage;
            CloudPOS.StartDevice += CloudPos_StartDevice;
            CloudPOS.StopDevice += CloudPos_StopDevice;
            CloudPOS.Error += CloudPos_Error;
            CloudPOS.Operator = posOperator;
        }

        private void DestroyCloudPos()
        {
            if (CloudPOS != null)
            {
                CloudPOS.Dispose();
                CloudPOS = null;
                chkCloudPos.Checked = false;
                chkCloudPos.BackColor = SystemColors.Control;
                Log("CloudPOS Released");
            }
            EnableDevice(btnScanBarcode, false);
            EnableDevice(btnSwipeCard, false);
            RefreshButtonStates();
        }
        #endregion

        #region CloudPOS ButtonStates
        private List<Button> _cloudPosButtons = new List<Button>();

        /*
         *  This method simply enables or disables the buttons on the main
         *  tab, depending on whether or not CloudPOS is instantiated and,
         *  if it is, on the state of the Basket: whether it contains items,
         *  and whether the basket is committed.
         */
        private void RefreshButtonStates()
        {
            if (CloudPOS == null)
            {
                foreach (Button button in _cloudPosButtons)
                {
                    button.Enabled = false;
                    button.FlatAppearance.BorderColor = Color.Silver;
                }
                btnClearBasket.Text = "Clear Basket";
                btnRemoveItem.Text = "Remove Item";
                return;
            }

            var itemsInBasket = (listBasket.Items.Count > 0);
            var itemSelected = itemsInBasket && (listBasket.SelectedItems.Count > 0);
            var committed = CloudPOS.Basket.Committed;

            btnShowGui.Enabled = !committed;
            btnShowGuiAt.Enabled = !committed;
            btnRefund.Enabled = !committed;
            btnAddItem.Enabled = !committed;
            btnCommit.Enabled = !committed && itemsInBasket;
            if (committed)
            {
                btnClearBasket.Enabled = itemsInBasket;
                btnClearBasket.Text = "Clear Basket";
                btnRemoveItem.Enabled = false;
                btnRemoveItem.Text = "Remove Item";
            }
            else
            {
                btnClearBasket.Enabled = itemsInBasket;
                btnClearBasket.Text = "VOID Basket";
                btnRemoveItem.Enabled = itemsInBasket;
                btnRemoveItem.Text = itemSelected ? "Remove Item" : "Remove Last";
            }
            foreach (Button b in _cloudPosButtons)
            {
                b.FlatAppearance.BorderColor = b.Enabled ? Color.Black : Color.Silver;
            }
        }
        #endregion

        #region CloudPOS Event Handling
        /*
         *  This region handles the EVENTS that may be raised from CloudPOS.
         * 
         *  First, CloudPos_Ready is raised when CloudPos is instantiated successfully
         *  and has been able to log on to the server.
         */
        private void CloudPos_Ready(object sender, EventArgs e)
        {
            chkCloudPos.BackColor = Color.Lime;
            CloudPOS.FailOnCommit = chkFailOnCommit.Checked;
            Log("CloudPOS Ready");
        }

        /*
         *  CloudPos_ShowGUI is raised when the GUI is presented to the User.
         *  There is nothing to be done here except to know that the screen
         *  is probably dominated by the CloudPOS GUI.
         */
        private void CloudPos_ShowGUI(object sender, EventArgs e)
        {
            Log("CloudPOS GUI Shown");
        }

        /*
         *  CloudPos_HideGUI is raised when the GUI vanishes, and you know
         *  you have the screen back.
         *  
         *  Here, we also disable the Barcode and Mag Stripe inputs (in
         *  case CloudPOS has not already told us to do so.
         */
        private void CloudPos_HideGUI(object sender, EventArgs e)
        {
            Log("CloudPOS GUI Hidden");
            EnableDevice(btnScanBarcode, false);
            EnableDevice(btnSwipeCard, false);
        }

        /*
         *  CloudPos_BasketCommitted is raised in response to calling CloudPOS.CommitBasket.
         *  
         *  We can also expect one or more CloudPos_VoucherAvailable to be raised next, 
         *  one for each voucher to be printed.
         */
        private void CloudPos_BasketCommitted(object sender, CloudPos.PosBasket posBasket)
        {
            Log("Basket Committed (event)");
            Log(" - " + posBasket.NumberOfVouchers + " voucher/s expected.");
            RefreshButtonStates();
        }

        /*
         *  CloudPos_BasketCommitFailed may be raised in response to calling CloudPOS.CommitBasket
         *  if the Commit fails completely, or times out.
         */
        private void CloudPos_BasketCommitFailed(object sender, CloudPos.PosBasket posBasket)
        {
            if (CloudPOS.IsFailedCommitRefundDue(posBasket.Id))
            {
                var refundDue = CloudPOS.FailedCommitRefundAmount(posBasket.Id).ToString("C");
                Log("Basket Commit Failed (event)");
                Log(" - Refund Amount = " + refundDue);
                MessageBox.Show("The BasketCommit failed.\n\nYou need to immediately refund the customer " + refundDue + ".",
                    btnCommit.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                CloudPOS.FailedCommitRefundComplete(posBasket.Id);
            }
            RefreshButtonStates();
        }

        /*
         *  CloudPos_ItemAdded is raised when an item is added to the Basket, including
         *  both PURCHASE items and REFUND items.
         *  
         *  Here, we update listBasket with the new item.
         *  
         *  NOTE: If the BasketItem to be added contains SubItems (within .Product.Items),
         *  we iterate through the .Product.Items collection and add each SubItem to the POS
         *  basket, rather than the parent Product.   This is true even if the .Product.Items 
         *  collection contains just one SubItem.   This is because we use this as a way of 
         *  overriding the EAN and/or Description of the product to be added to the basket.
         */
        private void CloudPos_ItemAdded(object sender, CloudPos.BasketItem item)
        {
            UpdateGUI(() =>
            {
                if (item.Type == "PURCHASE")
                {
                    CloudPos.PurchaseBasketItem purchase = (CloudPos.PurchaseBasketItem)item;
                    if (purchase.Product.Items == null || purchase.Product.Items.Count == 0)
                    {
                        ListViewItem lvItem = new ListViewItem(purchase.Product.Description);
                        lvItem.Tag = purchase.Id;
                        lvItem.SubItems.Add(purchase.Product.Price.Amount.ToString("C"));
                        lvItem.SubItems.Add(purchase.Product.Ean);
                        lvItem.SubItems.Add(purchase.Id.ToString());
                        listBasket.Items.Add(lvItem);
                    }
                    else // we have at least one SubItem in the .Product.Items collection
                    {
                        foreach (CloudPos.SubItem subItem in purchase.Product.Items)
                        {
                            ListViewItem lvItem = new ListViewItem(subItem.Description);
                            lvItem.Tag = purchase.Id;
                            lvItem.SubItems.Add(subItem.Amount.ToString("C"));
                            lvItem.SubItems.Add(subItem.Ean);
                            lvItem.SubItems.Add(purchase.Id.ToString());
                            listBasket.Items.Add(lvItem);
                        }
                    }
                }
                else if (item.Type == "REFUND")
                {
                    CloudPos.RefundBasketItem refund = (CloudPos.RefundBasketItem)item;
                    if (refund.Product.Items == null || refund.Product.Items.Count == 0)
                    {
                        ListViewItem lvItem = new ListViewItem(refund.Product.Description);
                        lvItem.Tag = refund.Id;
                        lvItem.SubItems.Add(refund.Product.Price.Amount.ToString("C"));
                        lvItem.SubItems.Add(refund.Product.Ean);
                        lvItem.SubItems.Add(refund.Id.ToString());
                        listBasket.Items.Add(lvItem);
                    }
                    else // we have at least one SubItem in the .Product.Items collection
                    {
                        foreach (CloudPos.SubItem subItem in refund.Product.Items)
                        {
                            ListViewItem lvItem = new ListViewItem(subItem.Description);
                            lvItem.Tag = refund.Id;
                            lvItem.SubItems.Add(subItem.Amount.ToString("C"));
                            lvItem.SubItems.Add(subItem.Ean);
                            lvItem.SubItems.Add(refund.Id.ToString());
                            listBasket.Items.Add(lvItem);
                        }
                    }
                }
                RefreshButtonStates();
            });

            if (item.Type == "PURCHASE")
            {
                CloudPos.PurchaseBasketItem p = (CloudPos.PurchaseBasketItem)item;
                Log(item.Type + ": " + p.Product.Description);
            }
            else if (item.Type == "REFUND")
            {
                CloudPos.RefundBasketItem r = (CloudPos.RefundBasketItem)item;
                Log(item.Type + ": " + r.Product.Description);
            }
        }

        /*
         *  CloudPos_ItemRemoved is raised when an item is removed from the Basket.
         *  
         *  Again, we update listBasket, by removing the matching item.
         */
        private void CloudPos_ItemRemoved(object sender, CloudPos.BasketItem item)
        {
            UpdateGUI(() =>
            {
                foreach (ListViewItem lvItem in listBasket.Items)
                {
                    if (lvItem.Tag.ToString() == item.Id.ToString())
                    {
                        lvItem.Remove();
                        break;
                    }
                }
                RefreshButtonStates();
            });

            if (item.Type == "PURCHASE")
            {
                CloudPos.PurchaseBasketItem p = (CloudPos.PurchaseBasketItem)item;
                Log(item.Type + " Removed: " + p.Product.Description);
            }
            else if (item.Type == "REFUND")
            {
                CloudPos.RefundBasketItem r = (CloudPos.RefundBasketItem)item;
                Log(item.Type + " Removed: " + r.Product.Description);
            }
        }

        /*
         *  CloudPos_VoucherAvailable is raised as each voucher is released for printing.
         *  
         *  Here we show the HTML voucher in a modeless dialog window.
         */
        private void CloudPos_VoucherAvailable(object sender, string voucherXml)
        {
            Log("VoucherAvailable (event)");
            new FormVoucherPreview().ViewHtml(voucherXml);
        }

        /*
         *  CloudPos_StartDevice is raised when CloudPOS wants the POS to turn on 
         *  either the Bar Code reader, or the Mag Stripe Card Reader.
         *  
         *  If either device is used to capture data, it is posted back to CloudPOS
         *  using the CloudPOS.DeviceData() method.
         */
        private void CloudPos_StartDevice(object sender, string device)
        {
            Log("Start: " + device);

            if (device == CloudPos.CloudPos.DEV_BARCODE)
                EnableDevice(btnScanBarcode, true);
            else if (device == CloudPos.CloudPos.DEV_MAGSTRIPE)
                EnableDevice(btnSwipeCard, true);
        }

        /*
         *  CloudPos_StartDevice is raised when CloudPOS wants the POS to turn on 
         *  either the Bar Code reader, or the Mag Stripe Card Reader.
         */  
         private void CloudPos_StopDevice(object sender, string device)
        {
            Log("Stop: " + device);

            if (device == CloudPos.CloudPos.DEV_BARCODE)
                EnableDevice(btnScanBarcode, false);
            else if (device == CloudPos.CloudPos.DEV_MAGSTRIPE)
                EnableDevice(btnSwipeCard, false);
        }

        /*
         *  Display a message from CloudPOS
         */ 
        private void CloudPos_DisplayMessage(object sender, string message)
        {
            MessageBox.Show(message, "Message from CloudPOS");
            Log("MESSAGE: " + message);
        }

        /*
         *  CloudPos_Error is raised when something goes wrong.
         */
        private void CloudPos_Error(object sender, string reason)
        {
            Log("ERROR: " + reason);
        }
        #endregion

        #region CloudPOS Buttons
        /*
         *  This method is called when we get the CloudPos_StartDevice or
         *  CloudPos_StopDevice event.  It enables a button on the GUI to 
         *  simulate a device read.
         */
        private void EnableDevice(Button button, bool enabled)
        {
            UpdateGUI(() =>
            {
                button.ImageKey = enabled ? button.Tag.ToString() : button.Tag.ToString() + "_x";
                button.ForeColor = enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                button.FlatAppearance.BorderColor = enabled ? Color.Green : Color.Silver;
                button.FlatAppearance.BorderSize = enabled ? 2 : 1;
                button.Enabled = enabled;
            });
        }

        private void btnScanBarcode_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormDeviceData())
            {
                if (dlg.ShowDialog(this, CloudPos.CloudPos.DEV_BARCODE) == DialogResult.OK)
                {
                    Log("Inject Bar Code: " + dlg.Data);
                    CloudPOS.DeviceData(CloudPos.CloudPos.DEV_BARCODE, dlg.Data);
                }
            }
        }

        private void btnSwipeCard_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormDeviceData())
            {
                if (dlg.ShowDialog(this, CloudPos.CloudPos.DEV_MAGSTRIPE) == DialogResult.OK)
                {
                    Log("Inject Mag Stripe Data: " + dlg.Data);
                    CloudPOS.DeviceData(CloudPos.CloudPos.DEV_MAGSTRIPE, dlg.Data);
                }
            }
        }

        private void btnShowGui_Click(object sender, EventArgs e)
        {
            Log("Clicked: " + ((Button)sender).Text);
            CloudPOS.ShowTouchpointUI(string.Empty);
        }

        private void btnShowGuiAt_Click(object sender, EventArgs e)
        {
            Log("Clicked: " + ((Button)sender).Text);
            using (var formAddItem = new FormAddItem(CloudPOS))
            {
                if (formAddItem.ShowShowGuiDialog(this) == DialogResult.OK)
                    CloudPOS.ShowTouchpointUI(formAddItem.Args[0].ToString());
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            Log("Clicked: " + ((Button)sender).Text);
            using (var formAddItem = new FormAddItem(CloudPOS))
            {
                if (formAddItem.ShowAddItemDialog(this) == DialogResult.OK)
                    CloudPOS.AddItem(formAddItem.Args[0].ToString());
            }
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            Log("Clicked: " + ((Button)sender).Text);
            CloudPOS.CommitBasket();
        }

        private void btnRefund_Click(object sender, EventArgs e)
        {
            Log("Clicked: " + ((Button)sender).Text);
            using (var formRefund = new FormRefund())
            {
                if (formRefund.ShowRefundDialog(this) == DialogResult.OK)
                    CloudPOS.Refund(formRefund.TransactionID, formRefund.Reason);
            }
        }

        private void btnClearBasket_Click(object sender, EventArgs e)
        {
            Log("Clicked: " + ((Button)sender).Text);
            ClearBasket();
        }

        private void listBasket_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtonStates();
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvItem = null;
            if (listBasket.SelectedItems.Count > 0)
                lvItem = listBasket.SelectedItems[0];

            if (chkCloudPos.Checked)
            {
                if (lvItem == null)
                    lvItem = listBasket.Items[listBasket.Items.Count - 1];

                CloudPOS.RemoveItem(lvItem.Tag.ToString(), "Removed by Operator");
            }
        }

        public void ClearBasket()
        {
            if (CloudPOS != null)
            {
                CloudPOS.ClearBasket(); // VOID Basket
                UpdateGUI(() =>
                {
                    listBasket.Items.Clear();
                    RefreshButtonStates();
                });
            }
        }
        #endregion Cloud POS Buttons

        #region WebPOS
        private void chkWebPos_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWebPos.Checked)
            {
                chkCloudPos.Checked = false;
                optWebPos.Checked = true;
                ClearLogs();
                if (WebPOS == null)
                    if (Program.FormMain().ShowWebPos(Program.Options().Url))
                        Log("Show WebPos: OK");
                    else
                        Log("Show WebPos: Failed");
            }
            else
            {
                if (WebPOS != null)
                    if (Program.FormMain().DestroyWebPos())
                        Log("Destroy WebPos: OK");
                    else
                        Log("Destroy WebPos: Failed");
            }
        }

        public bool ShowWebPos(string url)
        {
            // 
            //  Run this as a delegate inside UpdateGUI() to ensure GUI operations all work
            //
            UpdateGUI(() =>
            {
                flowCloudPos.Visible = false;
                listBasket.Visible = false;

                chkCloudPos.Checked = false;
                if (WebPOS == null)
                {
                    WebPOS = new CloudPos.WebPos();
                    WebPOS.Loaded += WebPos_Loaded;
                    WebPOS.Unloaded += WebPos_Unloaded;
                    var opt = Program.Options();
                    WebPOS.InitPosWindow(opt.Left, opt.Top,
                        opt.ClientSize.Width, opt.ClientSize.Height, url);

                    chkWebPos.Checked = true; // In case it isn't
                    chkWebPos.BackColor = Color.FromArgb(200, 255, 0);
                }
            });
            return true;
        }

        private void WebPos_Loaded(object sender, string url)
        {
            UpdateGUI(() => chkWebPos.BackColor = Color.Lime);
            Log("WebPOS Ready");
        }

        private void WebPos_Unloaded(object sender, EventArgs e)
        {
            DestroyWebPos();
            Log("WebPOS Window closed");
        }

        public bool DestroyWebPos()
        {
            // Run this as a delegate inside UpdateGUI() to ensure GUI operations all work
            UpdateGUI(() =>
            {
                flowCloudPos.Visible = true;
                listBasket.Visible = true;

                chkWebPos.BackColor = SystemColors.Control;
                if (WebPOS != null)
                {
                    WebPOS.Dispose();
                    WebPOS = null;
                    chkWebPos.Checked = false;
                }
            });
            return true;
        }
        #endregion

        #region Logging
        private void Log(string message)
        {
            UpdateGUI(() =>
            {
                listMainLog.Items.Add(message);
                listMiniLog.Items.Add(message);
                if (listMiniLog.Items.Count > 6)
                    listMiniLog.Items.RemoveAt(0);
            });
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            ClearLogs();
        }

        private void ClearLogs()
        {
            UpdateGUI(() =>
            {
                listMainLog.Items.Clear();
                listMiniLog.Items.Clear();
            });
        }
        #endregion Logging

        #region Options Tab
        private Dictionary<string, string> _knownSecrets = new Dictionary<string, string>();
        private bool _connectionNameDirty = false;


        private void InitializeOptions()
        {
            var ini = Program.IniFile();

            foreach (var combo in new[] { cboConnection, cboSkin, cboLocale, cboClientSize })
            {
                combo.Items.Clear();
            }
            var maxConnection = ini.GetInt("GENERAL", "max_connection");
            for (int i = 1; i <= maxConnection; i++)
            {
                string name = ini.GetString(Options.ConnSection(i), "name");
                if (!string.IsNullOrEmpty(name))
                    cboConnection.Items.Add(name);
            }
            var dict = ini.GetSection("SKINS");
            foreach (var key in dict.Keys)
            {
                cboSkin.Items.Add(key);
            }
            dict = ini.GetSection("LOCALES");
            foreach (var key in dict.Keys)
            {
                cboLocale.Items.Add(key);
            }
            Touch.DummyPos.ClientSize.PopulateComboBox(cboClientSize);


            var options = Program.Options();
            optCloudPOS.Checked = true;
            chkKeepOnTop.Checked = options.KeepOnTop;
            cboConnection.Text = options.ConnectionName;

            numLeft.Value = options.Left;
            numTop.Value = options.Top;
            if (options.ClientSize.Id == "custom")
            {
                var x = options.ClientSize.Width;
                var y = options.ClientSize.Height;
                cboClientSize.SelectedIndex = 0;
                numWidth.Value = x;
                numHeight.Value = y;
            }
            else
            {
                cboClientSize.Text = options.ClientSize.Description;
            }
        }

        private void optPOS_CheckedChanged(object sender, EventArgs e)
        {
            bool cloudPos = optCloudPOS.Checked;
            if (cloudPos)
                Program.Options().PosType = PosType.CloudPOS;
            else
                Program.Options().PosType = PosType.WebPOS;

            lblSecret.Visible = cloudPos;
            txtSecret.Visible = cloudPos;
            lblSkin.Visible = cloudPos;
            cboSkin.Visible = cloudPos;
            lblLocale.Visible = cloudPos;
            cboLocale.Visible = cloudPos;
            lblOperator.Visible = cloudPos;
            txtOperator.Visible = cloudPos;

            chkCloudPos.Visible = cloudPos;
            chkWebPos.Visible = !cloudPos;
        }

        private void cboConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            var options = Program.Options();

            options.LoadConnection(((ComboBox)sender).Text);
            if (options.PosType == PosType.CloudPOS)
                optCloudPOS.Checked = true;
            else if (options.PosType == PosType.WebPOS)
                optWebPos.Checked = true;

            txtURL.Text = options.Url;
            txtSecret.Text = options.Secret;
            cboSkin.Text = options.Skin;
            cboLocale.Text = options.Locale;
            txtOperator.Text = options.Operator;
            _connectionNameDirty = false;
        }

        private void cboConnection_TextChanged(object sender, EventArgs e)
        {
            _connectionNameDirty = true;
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            Program.Options().Url = txtURL.Text;
        }

        private void txtSecret_TextChanged(object sender, EventArgs e)
        {
            Program.Options().Secret = txtSecret.Text;
        }
        
        private void txtOperator_TextChanged(object sender, EventArgs e)
        {
            Program.Options().Operator = txtOperator.Text;
        }

        private void cboSkin_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Options().Skin = cboSkin.Text;
        }

        private void cboLocale_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Options().Locale = cboLocale.Text;
        }

        private void cboClientSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            var clientSize = (ClientSize)cboClientSize.SelectedItem;
            var customSize = (clientSize.Id == "custom");
            lblWidth.Enabled = customSize;
            numWidth.Enabled = customSize;
            lblHeight.Enabled = customSize;
            numHeight.Enabled = customSize;
            if (!customSize)
            {
                numWidth.Value = clientSize.Width;
                numHeight.Value = clientSize.Height;
            }
            Program.Options().ClientSize = clientSize;
        }

        private void posSize_ValueChanged(object sender, EventArgs e)
        {
            var options = Program.Options();

            int value = (int)((NumericUpDown)sender).Value;
            if (sender == numLeft)
                options.Left = value;
            else if (sender == numTop)
                options.Top = value;
            else if (sender == numWidth)
                options.ClientSize.Width = value;
            else if (sender == numHeight)
                options.ClientSize.Height = value;
        }

        private void chkKeepOnTop_CheckedChanged(object sender, EventArgs e)
        {
            Program.Options().KeepOnTop = chkKeepOnTop.Checked;
        }

        private void btnPersistChanges_Click(object sender, EventArgs e)
        {
            var name = cboConnection.Text;
            var options = Program.Options();
            if (_connectionNameDirty)
            {
                var dialog = new FormNameChange();
                switch (dialog.Show(name))
                {
                    case NameChangeResult.SaveNewName:
                        options.ConnectionName = name; // Change name
                        break;
                    case NameChangeResult.SaveNewConnection:
                        options.ConnectionName = name; // Change name...
                        options.ConnectionId = 0; // ...and force new connection
                        break;
                    case NameChangeResult.SaveUndoNameChange:
                        name = options.ConnectionName; // Undo name change
                        break; 
                    default:
                        return;
                }
            }
            options.PersistChanges();
            InitializeOptions();
            if (!string.IsNullOrEmpty(txtURL.Text))
                cboConnection.Text = name;
            else if (cboConnection.Items.Count > 0)
                cboConnection.SelectedIndex = 0;
        }

        #endregion Options Tab

        #region Debug Tab
        private void chkFailOnCommit_CheckedChanged(object sender, EventArgs e)
        {
            if (CloudPOS != null)
                CloudPOS.FailOnCommit = chkFailOnCommit.Checked;
        }
        #endregion
    }
}
