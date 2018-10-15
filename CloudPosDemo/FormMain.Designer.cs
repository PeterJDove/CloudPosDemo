namespace Touch.CloudPosDemo
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCloudPos = new System.Windows.Forms.TabPage();
            this.tableLayoutCloudPos = new System.Windows.Forms.TableLayoutPanel();
            this.listBasket = new System.Windows.Forms.ListView();
            this.colItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEan = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTouchID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.flowCloudPos = new System.Windows.Forms.FlowLayoutPanel();
            this.btnScanBarcode = new System.Windows.Forms.Button();
            this.iml32x32 = new System.Windows.Forms.ImageList(this.components);
            this.btnSwipeCard = new System.Windows.Forms.Button();
            this.btnShowGui = new System.Windows.Forms.Button();
            this.btnShowGuiAt = new System.Windows.Forms.Button();
            this.btnRefund = new System.Windows.Forms.Button();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnClearBasket = new System.Windows.Forms.Button();
            this.btnRemoveItem = new System.Windows.Forms.Button();
            this.flowCloudWeb = new System.Windows.Forms.FlowLayoutPanel();
            this.chkCloudPos = new System.Windows.Forms.CheckBox();
            this.chkWebPos = new System.Windows.Forms.CheckBox();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listMainLog = new System.Windows.Forms.ListBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.txtSecret = new System.Windows.Forms.TextBox();
            this.cboConnection = new System.Windows.Forms.ComboBox();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.lblOperator = new System.Windows.Forms.Label();
            this.optWebPos = new System.Windows.Forms.RadioButton();
            this.optCloudPOS = new System.Windows.Forms.RadioButton();
            this.lblLocale = new System.Windows.Forms.Label();
            this.cboLocale = new System.Windows.Forms.ComboBox();
            this.cboSkin = new System.Windows.Forms.ComboBox();
            this.lblSkin = new System.Windows.Forms.Label();
            this.lblSecret = new System.Windows.Forms.Label();
            this.btnPersistChanges = new System.Windows.Forms.Button();
            this.chkKeepOnTop = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblTop = new System.Windows.Forms.Label();
            this.lblLeft = new System.Windows.Forms.Label();
            this.numLeft = new System.Windows.Forms.NumericUpDown();
            this.numTop = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.cboClientSize = new System.Windows.Forms.ComboBox();
            this.lblWindowSize = new System.Windows.Forms.Label();
            this.listMiniLog = new System.Windows.Forms.ListBox();
            this.tableLayoutMain.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabCloudPos.SuspendLayout();
            this.tableLayoutCloudPos.SuspendLayout();
            this.flowCloudPos.SuspendLayout();
            this.flowCloudWeb.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 1;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutMain.Controls.Add(this.tabControl, 0, 1);
            this.tableLayoutMain.Controls.Add(this.listMiniLog, 0, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 1;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Size = new System.Drawing.Size(334, 532);
            this.tableLayoutMain.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControl.Controls.Add(this.tabCloudPos);
            this.tabControl.Controls.Add(this.tabLog);
            this.tabControl.Controls.Add(this.tabOptions);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ItemSize = new System.Drawing.Size(80, 22);
            this.tabControl.Location = new System.Drawing.Point(3, 113);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(328, 416);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 11;
            // 
            // tabCloudPos
            // 
            this.tabCloudPos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabCloudPos.Controls.Add(this.tableLayoutCloudPos);
            this.tabCloudPos.Location = new System.Drawing.Point(4, 4);
            this.tabCloudPos.Name = "tabCloudPos";
            this.tabCloudPos.Padding = new System.Windows.Forms.Padding(3);
            this.tabCloudPos.Size = new System.Drawing.Size(298, 408);
            this.tabCloudPos.TabIndex = 1;
            this.tabCloudPos.Text = "Main";
            // 
            // tableLayoutCloudPos
            // 
            this.tableLayoutCloudPos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tableLayoutCloudPos.ColumnCount = 1;
            this.tableLayoutCloudPos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutCloudPos.Controls.Add(this.listBasket, 0, 2);
            this.tableLayoutCloudPos.Controls.Add(this.flowCloudPos, 0, 1);
            this.tableLayoutCloudPos.Controls.Add(this.flowCloudWeb, 0, 0);
            this.tableLayoutCloudPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutCloudPos.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutCloudPos.Name = "tableLayoutCloudPos";
            this.tableLayoutCloudPos.RowCount = 3;
            this.tableLayoutCloudPos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutCloudPos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 215F));
            this.tableLayoutCloudPos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutCloudPos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutCloudPos.Size = new System.Drawing.Size(292, 402);
            this.tableLayoutCloudPos.TabIndex = 1;
            // 
            // listBasket
            // 
            this.listBasket.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colItem,
            this.colPrice,
            this.colEan,
            this.colTouchID});
            this.listBasket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBasket.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBasket.FullRowSelect = true;
            this.listBasket.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listBasket.Location = new System.Drawing.Point(3, 254);
            this.listBasket.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBasket.MultiSelect = false;
            this.listBasket.Name = "listBasket";
            this.listBasket.Size = new System.Drawing.Size(286, 144);
            this.listBasket.TabIndex = 11;
            this.listBasket.UseCompatibleStateImageBehavior = false;
            this.listBasket.View = System.Windows.Forms.View.Details;
            // 
            // colItem
            // 
            this.colItem.Text = "Item";
            this.colItem.Width = 170;
            // 
            // colPrice
            // 
            this.colPrice.Text = "Price";
            this.colPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colPrice.Width = 70;
            // 
            // colEan
            // 
            this.colEan.Text = "EAN";
            this.colEan.Width = 120;
            // 
            // colTouchID
            // 
            this.colTouchID.Text = "Touch ID";
            this.colTouchID.Width = 160;
            // 
            // flowCloudPos
            // 
            this.flowCloudPos.Controls.Add(this.btnScanBarcode);
            this.flowCloudPos.Controls.Add(this.btnSwipeCard);
            this.flowCloudPos.Controls.Add(this.btnShowGui);
            this.flowCloudPos.Controls.Add(this.btnShowGuiAt);
            this.flowCloudPos.Controls.Add(this.btnRefund);
            this.flowCloudPos.Controls.Add(this.btnAddItem);
            this.flowCloudPos.Controls.Add(this.btnCommit);
            this.flowCloudPos.Controls.Add(this.btnClearBasket);
            this.flowCloudPos.Controls.Add(this.btnRemoveItem);
            this.flowCloudPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowCloudPos.Location = new System.Drawing.Point(3, 38);
            this.flowCloudPos.Name = "flowCloudPos";
            this.flowCloudPos.Size = new System.Drawing.Size(286, 209);
            this.flowCloudPos.TabIndex = 0;
            // 
            // btnScanBarcode
            // 
            this.btnScanBarcode.Enabled = false;
            this.btnScanBarcode.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnScanBarcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanBarcode.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnScanBarcode.ImageKey = "scanner_x";
            this.btnScanBarcode.ImageList = this.iml32x32;
            this.btnScanBarcode.Location = new System.Drawing.Point(3, 3);
            this.btnScanBarcode.Name = "btnScanBarcode";
            this.btnScanBarcode.Size = new System.Drawing.Size(120, 60);
            this.btnScanBarcode.TabIndex = 0;
            this.btnScanBarcode.Tag = "scanner";
            this.btnScanBarcode.Text = "Scan Barcode";
            this.btnScanBarcode.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnScanBarcode.UseVisualStyleBackColor = true;
            this.btnScanBarcode.Click += new System.EventHandler(this.btnScanBarcode_Click);
            // 
            // iml32x32
            // 
            this.iml32x32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iml32x32.ImageStream")));
            this.iml32x32.TransparentColor = System.Drawing.Color.Transparent;
            this.iml32x32.Images.SetKeyName(0, "scanner");
            this.iml32x32.Images.SetKeyName(1, "scanner_x");
            this.iml32x32.Images.SetKeyName(2, "magstripe");
            this.iml32x32.Images.SetKeyName(3, "magstripe_x");
            // 
            // btnSwipeCard
            // 
            this.btnSwipeCard.Enabled = false;
            this.btnSwipeCard.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSwipeCard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwipeCard.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSwipeCard.ImageKey = "magstripe_x";
            this.btnSwipeCard.ImageList = this.iml32x32;
            this.btnSwipeCard.Location = new System.Drawing.Point(129, 3);
            this.btnSwipeCard.Name = "btnSwipeCard";
            this.btnSwipeCard.Size = new System.Drawing.Size(120, 60);
            this.btnSwipeCard.TabIndex = 1;
            this.btnSwipeCard.Tag = "magstripe";
            this.btnSwipeCard.Text = "Swipe Card";
            this.btnSwipeCard.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSwipeCard.UseVisualStyleBackColor = true;
            this.btnSwipeCard.Click += new System.EventHandler(this.btnSwipeCard_Click);
            // 
            // btnShowGui
            // 
            this.btnShowGui.Enabled = false;
            this.btnShowGui.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnShowGui.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowGui.Location = new System.Drawing.Point(3, 69);
            this.btnShowGui.Name = "btnShowGui";
            this.btnShowGui.Size = new System.Drawing.Size(120, 30);
            this.btnShowGui.TabIndex = 2;
            this.btnShowGui.Text = "Show GUI";
            this.btnShowGui.UseVisualStyleBackColor = true;
            this.btnShowGui.Click += new System.EventHandler(this.btnShowGui_Click);
            // 
            // btnShowGuiAt
            // 
            this.btnShowGuiAt.Enabled = false;
            this.btnShowGuiAt.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnShowGuiAt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowGuiAt.Location = new System.Drawing.Point(129, 69);
            this.btnShowGuiAt.Name = "btnShowGuiAt";
            this.btnShowGuiAt.Size = new System.Drawing.Size(120, 30);
            this.btnShowGuiAt.TabIndex = 3;
            this.btnShowGuiAt.Text = "Show GUI (...)";
            this.btnShowGuiAt.UseVisualStyleBackColor = true;
            this.btnShowGuiAt.Click += new System.EventHandler(this.btnShowGuiAt_Click);
            // 
            // btnRefund
            // 
            this.btnRefund.Enabled = false;
            this.btnRefund.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRefund.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefund.Location = new System.Drawing.Point(3, 105);
            this.btnRefund.Name = "btnRefund";
            this.btnRefund.Size = new System.Drawing.Size(120, 30);
            this.btnRefund.TabIndex = 4;
            this.btnRefund.Text = "Refund";
            this.btnRefund.UseVisualStyleBackColor = true;
            this.btnRefund.Click += new System.EventHandler(this.btnRefund_Click);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Enabled = false;
            this.btnAddItem.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Location = new System.Drawing.Point(129, 105);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(120, 30);
            this.btnAddItem.TabIndex = 5;
            this.btnAddItem.Text = "Add Item (...)";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Enabled = false;
            this.btnCommit.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCommit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCommit.Location = new System.Drawing.Point(3, 141);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(246, 30);
            this.btnCommit.TabIndex = 6;
            this.btnCommit.Text = "Commit Basket";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnClearBasket
            // 
            this.btnClearBasket.Enabled = false;
            this.btnClearBasket.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClearBasket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearBasket.Location = new System.Drawing.Point(3, 177);
            this.btnClearBasket.Name = "btnClearBasket";
            this.btnClearBasket.Size = new System.Drawing.Size(120, 30);
            this.btnClearBasket.TabIndex = 7;
            this.btnClearBasket.Text = "Clear Basket";
            this.btnClearBasket.UseVisualStyleBackColor = true;
            this.btnClearBasket.Click += new System.EventHandler(this.btnClearBasket_Click);
            // 
            // btnRemoveItem
            // 
            this.btnRemoveItem.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRemoveItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveItem.Location = new System.Drawing.Point(129, 177);
            this.btnRemoveItem.Name = "btnRemoveItem";
            this.btnRemoveItem.Size = new System.Drawing.Size(120, 29);
            this.btnRemoveItem.TabIndex = 0;
            this.btnRemoveItem.Text = "Remove Item";
            this.btnRemoveItem.UseVisualStyleBackColor = true;
            this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
            // 
            // flowCloudWeb
            // 
            this.flowCloudWeb.Controls.Add(this.chkCloudPos);
            this.flowCloudWeb.Controls.Add(this.chkWebPos);
            this.flowCloudWeb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowCloudWeb.Location = new System.Drawing.Point(3, 3);
            this.flowCloudWeb.Name = "flowCloudWeb";
            this.flowCloudWeb.Size = new System.Drawing.Size(286, 29);
            this.flowCloudWeb.TabIndex = 1;
            // 
            // chkCloudPos
            // 
            this.chkCloudPos.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkCloudPos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkCloudPos.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCloudPos.Location = new System.Drawing.Point(3, 0);
            this.chkCloudPos.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkCloudPos.Name = "chkCloudPos";
            this.chkCloudPos.Size = new System.Drawing.Size(120, 29);
            this.chkCloudPos.TabIndex = 0;
            this.chkCloudPos.Text = "CloudPOS";
            this.chkCloudPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkCloudPos.UseVisualStyleBackColor = true;
            this.chkCloudPos.CheckedChanged += new System.EventHandler(this.chkCloudPos_CheckedChanged);
            // 
            // chkWebPos
            // 
            this.chkWebPos.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkWebPos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkWebPos.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkWebPos.Location = new System.Drawing.Point(129, 0);
            this.chkWebPos.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkWebPos.Name = "chkWebPos";
            this.chkWebPos.Size = new System.Drawing.Size(120, 29);
            this.chkWebPos.TabIndex = 1;
            this.chkWebPos.Text = "WebPOS";
            this.chkWebPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkWebPos.UseVisualStyleBackColor = true;
            this.chkWebPos.Visible = false;
            this.chkWebPos.CheckedChanged += new System.EventHandler(this.chkWebPos_CheckedChanged);
            // 
            // tabLog
            // 
            this.tabLog.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabLog.Controls.Add(this.tableLayoutPanel1);
            this.tabLog.Location = new System.Drawing.Point(4, 4);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(298, 408);
            this.tabLog.TabIndex = 3;
            this.tabLog.Text = "Log";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.listMainLog, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClearLog, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(292, 402);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listMainLog
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listMainLog, 3);
            this.listMainLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMainLog.FormattingEnabled = true;
            this.listMainLog.ItemHeight = 16;
            this.listMainLog.Location = new System.Drawing.Point(3, 3);
            this.listMainLog.Name = "listMainLog";
            this.listMainLog.Size = new System.Drawing.Size(286, 366);
            this.listMainLog.TabIndex = 0;
            // 
            // btnClearLog
            // 
            this.btnClearLog.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.Location = new System.Drawing.Point(96, 372);
            this.btnClearLog.Margin = new System.Windows.Forms.Padding(0);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(100, 30);
            this.btnClearLog.TabIndex = 3;
            this.btnClearLog.Text = "Clear Logs";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // tabOptions
            // 
            this.tabOptions.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tabOptions.Controls.Add(this.groupBox2);
            this.tabOptions.Controls.Add(this.btnPersistChanges);
            this.tabOptions.Controls.Add(this.chkKeepOnTop);
            this.tabOptions.Controls.Add(this.groupBox1);
            this.tabOptions.Location = new System.Drawing.Point(4, 4);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOptions.Size = new System.Drawing.Size(298, 408);
            this.tabOptions.TabIndex = 4;
            this.tabOptions.Text = "Options";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblURL);
            this.groupBox2.Controls.Add(this.txtURL);
            this.groupBox2.Controls.Add(this.txtSecret);
            this.groupBox2.Controls.Add(this.cboConnection);
            this.groupBox2.Controls.Add(this.txtOperator);
            this.groupBox2.Controls.Add(this.lblOperator);
            this.groupBox2.Controls.Add(this.optWebPos);
            this.groupBox2.Controls.Add(this.optCloudPOS);
            this.groupBox2.Controls.Add(this.lblLocale);
            this.groupBox2.Controls.Add(this.cboLocale);
            this.groupBox2.Controls.Add(this.cboSkin);
            this.groupBox2.Controls.Add(this.lblSkin);
            this.groupBox2.Controls.Add(this.lblSecret);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 240);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection";
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(6, 54);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(30, 16);
            this.lblURL.TabIndex = 36;
            this.lblURL.Text = "URL";
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(6, 73);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(274, 23);
            this.txtURL.TabIndex = 35;
            this.txtURL.TextChanged += new System.EventHandler(this.txtURL_TextChanged);
            // 
            // txtSecret
            // 
            this.txtSecret.Location = new System.Drawing.Point(8, 118);
            this.txtSecret.Name = "txtSecret";
            this.txtSecret.Size = new System.Drawing.Size(274, 23);
            this.txtSecret.TabIndex = 34;
            this.txtSecret.TextChanged += new System.EventHandler(this.txtSecret_TextChanged);
            // 
            // cboConnection
            // 
            this.cboConnection.FormattingEnabled = true;
            this.cboConnection.Location = new System.Drawing.Point(6, 22);
            this.cboConnection.Name = "cboConnection";
            this.cboConnection.Size = new System.Drawing.Size(275, 24);
            this.cboConnection.Sorted = true;
            this.cboConnection.TabIndex = 33;
            this.cboConnection.SelectedIndexChanged += new System.EventHandler(this.cboConnection_SelectedIndexChanged);
            this.cboConnection.TextChanged += new System.EventHandler(this.cboConnection_TextChanged);
            // 
            // txtOperator
            // 
            this.txtOperator.Location = new System.Drawing.Point(6, 209);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.Size = new System.Drawing.Size(273, 23);
            this.txtOperator.TabIndex = 28;
            this.txtOperator.TextChanged += new System.EventHandler(this.txtOperator_TextChanged);
            // 
            // lblOperator
            // 
            this.lblOperator.AutoSize = true;
            this.lblOperator.Location = new System.Drawing.Point(6, 190);
            this.lblOperator.Name = "lblOperator";
            this.lblOperator.Size = new System.Drawing.Size(87, 16);
            this.lblOperator.TabIndex = 26;
            this.lblOperator.Text = "POS Operator";
            // 
            // optWebPos
            // 
            this.optWebPos.AutoSize = true;
            this.optWebPos.Location = new System.Drawing.Point(204, 52);
            this.optWebPos.Name = "optWebPos";
            this.optWebPos.Size = new System.Drawing.Size(76, 20);
            this.optWebPos.TabIndex = 23;
            this.optWebPos.Text = "WebPOS";
            this.optWebPos.UseVisualStyleBackColor = true;
            this.optWebPos.CheckedChanged += new System.EventHandler(this.optPOS_CheckedChanged);
            // 
            // optCloudPOS
            // 
            this.optCloudPOS.AutoSize = true;
            this.optCloudPOS.Location = new System.Drawing.Point(116, 52);
            this.optCloudPOS.Name = "optCloudPOS";
            this.optCloudPOS.Size = new System.Drawing.Size(82, 20);
            this.optCloudPOS.TabIndex = 22;
            this.optCloudPOS.Text = "CloudPOS";
            this.optCloudPOS.UseVisualStyleBackColor = true;
            this.optCloudPOS.CheckedChanged += new System.EventHandler(this.optPOS_CheckedChanged);
            // 
            // lblLocale
            // 
            this.lblLocale.AutoSize = true;
            this.lblLocale.Location = new System.Drawing.Point(149, 144);
            this.lblLocale.Name = "lblLocale";
            this.lblLocale.Size = new System.Drawing.Size(112, 16);
            this.lblLocale.TabIndex = 31;
            this.lblLocale.Text = "Language / Locale";
            // 
            // cboLocale
            // 
            this.cboLocale.FormattingEnabled = true;
            this.cboLocale.Location = new System.Drawing.Point(152, 163);
            this.cboLocale.Name = "cboLocale";
            this.cboLocale.Size = new System.Drawing.Size(128, 24);
            this.cboLocale.TabIndex = 32;
            this.cboLocale.SelectedIndexChanged += new System.EventHandler(this.cboLocale_SelectedIndexChanged);
            // 
            // cboSkin
            // 
            this.cboSkin.FormattingEnabled = true;
            this.cboSkin.Location = new System.Drawing.Point(6, 163);
            this.cboSkin.Name = "cboSkin";
            this.cboSkin.Size = new System.Drawing.Size(140, 24);
            this.cboSkin.TabIndex = 30;
            this.cboSkin.SelectedIndexChanged += new System.EventHandler(this.cboSkin_SelectedIndexChanged);
            // 
            // lblSkin
            // 
            this.lblSkin.AutoSize = true;
            this.lblSkin.Location = new System.Drawing.Point(6, 144);
            this.lblSkin.Name = "lblSkin";
            this.lblSkin.Size = new System.Drawing.Size(32, 16);
            this.lblSkin.TabIndex = 29;
            this.lblSkin.Text = "Skin";
            // 
            // lblSecret
            // 
            this.lblSecret.AutoSize = true;
            this.lblSecret.Location = new System.Drawing.Point(5, 99);
            this.lblSecret.Name = "lblSecret";
            this.lblSecret.Size = new System.Drawing.Size(45, 16);
            this.lblSecret.TabIndex = 25;
            this.lblSecret.Text = "Secret";
            // 
            // btnPersistChanges
            // 
            this.btnPersistChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPersistChanges.Location = new System.Drawing.Point(149, 372);
            this.btnPersistChanges.Name = "btnPersistChanges";
            this.btnPersistChanges.Size = new System.Drawing.Size(136, 30);
            this.btnPersistChanges.TabIndex = 24;
            this.btnPersistChanges.Text = "Persist Changes";
            this.btnPersistChanges.UseVisualStyleBackColor = true;
            this.btnPersistChanges.Click += new System.EventHandler(this.btnPersistChanges_Click);
            // 
            // chkKeepOnTop
            // 
            this.chkKeepOnTop.AutoSize = true;
            this.chkKeepOnTop.Location = new System.Drawing.Point(8, 378);
            this.chkKeepOnTop.Name = "chkKeepOnTop";
            this.chkKeepOnTop.Size = new System.Drawing.Size(99, 20);
            this.chkKeepOnTop.TabIndex = 23;
            this.chkKeepOnTop.Text = "Keep on Top";
            this.chkKeepOnTop.UseVisualStyleBackColor = true;
            this.chkKeepOnTop.CheckedChanged += new System.EventHandler(this.chkKeepOnTop_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblHeight);
            this.groupBox1.Controls.Add(this.lblWidth);
            this.groupBox1.Controls.Add(this.lblTop);
            this.groupBox1.Controls.Add(this.lblLeft);
            this.groupBox1.Controls.Add(this.numLeft);
            this.groupBox1.Controls.Add(this.numTop);
            this.groupBox1.Controls.Add(this.numWidth);
            this.groupBox1.Controls.Add(this.numHeight);
            this.groupBox1.Controls.Add(this.cboClientSize);
            this.groupBox1.Controls.Add(this.lblWindowSize);
            this.groupBox1.Location = new System.Drawing.Point(5, 252);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(287, 114);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Browser";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(213, 54);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(44, 16);
            this.lblHeight.TabIndex = 20;
            this.lblHeight.Text = "Height";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(145, 54);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(41, 16);
            this.lblWidth.TabIndex = 19;
            this.lblWidth.Text = "Width";
            // 
            // lblTop
            // 
            this.lblTop.AutoSize = true;
            this.lblTop.Location = new System.Drawing.Point(76, 54);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(30, 16);
            this.lblTop.TabIndex = 15;
            this.lblTop.Text = "Top";
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(10, 54);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(29, 16);
            this.lblLeft.TabIndex = 13;
            this.lblLeft.Text = "Left";
            // 
            // numLeft
            // 
            this.numLeft.Location = new System.Drawing.Point(13, 73);
            this.numLeft.Maximum = new decimal(new int[] {
            3200,
            0,
            0,
            0});
            this.numLeft.Name = "numLeft";
            this.numLeft.Size = new System.Drawing.Size(60, 23);
            this.numLeft.TabIndex = 14;
            this.numLeft.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numLeft.ValueChanged += new System.EventHandler(this.posSize_ValueChanged);
            // 
            // numTop
            // 
            this.numTop.Location = new System.Drawing.Point(79, 73);
            this.numTop.Maximum = new decimal(new int[] {
            3200,
            0,
            0,
            0});
            this.numTop.Name = "numTop";
            this.numTop.Size = new System.Drawing.Size(60, 23);
            this.numTop.TabIndex = 16;
            this.numTop.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numTop.ValueChanged += new System.EventHandler(this.posSize_ValueChanged);
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(145, 73);
            this.numWidth.Maximum = new decimal(new int[] {
            3200,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(60, 23);
            this.numWidth.TabIndex = 17;
            this.numWidth.ValueChanged += new System.EventHandler(this.posSize_ValueChanged);
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(211, 73);
            this.numHeight.Maximum = new decimal(new int[] {
            3200,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(60, 23);
            this.numHeight.TabIndex = 18;
            this.numHeight.ValueChanged += new System.EventHandler(this.posSize_ValueChanged);
            // 
            // cboClientSize
            // 
            this.cboClientSize.FormattingEnabled = true;
            this.cboClientSize.Location = new System.Drawing.Point(144, 22);
            this.cboClientSize.Name = "cboClientSize";
            this.cboClientSize.Size = new System.Drawing.Size(137, 24);
            this.cboClientSize.TabIndex = 9;
            this.cboClientSize.SelectedIndexChanged += new System.EventHandler(this.cboClientSize_SelectedIndexChanged);
            // 
            // lblWindowSize
            // 
            this.lblWindowSize.AutoSize = true;
            this.lblWindowSize.Location = new System.Drawing.Point(29, 25);
            this.lblWindowSize.Name = "lblWindowSize";
            this.lblWindowSize.Size = new System.Drawing.Size(110, 16);
            this.lblWindowSize.TabIndex = 8;
            this.lblWindowSize.Text = "Position and Size:";
            // 
            // listMiniLog
            // 
            this.listMiniLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMiniLog.FormattingEnabled = true;
            this.listMiniLog.ItemHeight = 16;
            this.listMiniLog.Location = new System.Drawing.Point(3, 3);
            this.listMiniLog.Name = "listMiniLog";
            this.listMiniLog.Size = new System.Drawing.Size(328, 104);
            this.listMiniLog.TabIndex = 12;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(334, 532);
            this.Controls.Add(this.tableLayoutMain);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(1500, 100);
            this.MinimumSize = new System.Drawing.Size(350, 570);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CloudPOS Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tableLayoutMain.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabCloudPos.ResumeLayout(false);
            this.tableLayoutCloudPos.ResumeLayout(false);
            this.flowCloudPos.ResumeLayout(false);
            this.flowCloudWeb.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCloudPos;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutCloudPos;
        private System.Windows.Forms.FlowLayoutPanel flowCloudPos;
        protected internal System.Windows.Forms.Button btnScanBarcode;
        private System.Windows.Forms.Button btnSwipeCard;
        private System.Windows.Forms.FlowLayoutPanel flowCloudWeb;
        private System.Windows.Forms.CheckBox chkCloudPos;
        private System.Windows.Forms.CheckBox chkWebPos;
        private System.Windows.Forms.ImageList iml32x32;
        private System.Windows.Forms.Button btnShowGui;
        private System.Windows.Forms.Button btnShowGuiAt;
        private System.Windows.Forms.Button btnRefund;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.Button btnClearBasket;
        private System.Windows.Forms.Button btnRemoveItem;
        private System.Windows.Forms.ListView listBasket;
        private System.Windows.Forms.ColumnHeader colItem;
        private System.Windows.Forms.ColumnHeader colPrice;
        private System.Windows.Forms.ColumnHeader colEan;
        private System.Windows.Forms.ColumnHeader colTouchID;
        private System.Windows.Forms.ListBox listMiniLog;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblWindowSize;
        private System.Windows.Forms.ComboBox cboClientSize;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.NumericUpDown numLeft;
        private System.Windows.Forms.NumericUpDown numTop;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.CheckBox chkKeepOnTop;
        private System.Windows.Forms.Button btnPersistChanges;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listMainLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.TextBox txtSecret;
        private System.Windows.Forms.ComboBox cboConnection;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.Label lblOperator;
        private System.Windows.Forms.RadioButton optWebPos;
        private System.Windows.Forms.RadioButton optCloudPOS;
        private System.Windows.Forms.Label lblLocale;
        private System.Windows.Forms.ComboBox cboLocale;
        private System.Windows.Forms.ComboBox cboSkin;
        private System.Windows.Forms.Label lblSkin;
        private System.Windows.Forms.Label lblSecret;
    }
}