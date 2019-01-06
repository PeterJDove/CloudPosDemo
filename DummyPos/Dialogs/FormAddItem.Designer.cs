namespace Touch.DummyPos
{
    partial class FormAddItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddItem));
            this.label1 = new System.Windows.Forms.Label();
            this.cboProduct = new System.Windows.Forms.ComboBox();
            this.groupData = new System.Windows.Forms.GroupBox();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.txtData4 = new System.Windows.Forms.TextBox();
            this.lblData4 = new System.Windows.Forms.Label();
            this.txtData3 = new System.Windows.Forms.TextBox();
            this.lblData3 = new System.Windows.Forms.Label();
            this.txtData2 = new System.Windows.Forms.TextBox();
            this.lblData2 = new System.Windows.Forms.Label();
            this.txtData1 = new System.Windows.Forms.TextBox();
            this.lblData1 = new System.Windows.Forms.Label();
            this.txtData0 = new System.Windows.Forms.TextBox();
            this.lblData0 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProductInfo = new System.Windows.Forms.Label();
            this.groupData.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Shortcut or EAN";
            // 
            // cboProduct
            // 
            this.cboProduct.FormattingEnabled = true;
            this.cboProduct.Location = new System.Drawing.Point(15, 28);
            this.cboProduct.Name = "cboProduct";
            this.cboProduct.Size = new System.Drawing.Size(262, 24);
            this.cboProduct.TabIndex = 1;
            this.cboProduct.SelectedIndexChanged += new System.EventHandler(this.cboProduct_TextChanged);
            this.cboProduct.TextChanged += new System.EventHandler(this.cboProduct_TextChanged);
            // 
            // groupData
            // 
            this.groupData.Controls.Add(this.btnClearAll);
            this.groupData.Controls.Add(this.vScrollBar);
            this.groupData.Controls.Add(this.txtData4);
            this.groupData.Controls.Add(this.lblData4);
            this.groupData.Controls.Add(this.txtData3);
            this.groupData.Controls.Add(this.lblData3);
            this.groupData.Controls.Add(this.txtData2);
            this.groupData.Controls.Add(this.lblData2);
            this.groupData.Controls.Add(this.txtData1);
            this.groupData.Controls.Add(this.lblData1);
            this.groupData.Controls.Add(this.txtData0);
            this.groupData.Controls.Add(this.lblData0);
            this.groupData.Location = new System.Drawing.Point(15, 78);
            this.groupData.Name = "groupData";
            this.groupData.Size = new System.Drawing.Size(262, 202);
            this.groupData.TabIndex = 2;
            this.groupData.TabStop = false;
            this.groupData.Text = "Data Items";
            // 
            // btnClearAll
            // 
            this.btnClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClearAll.Location = new System.Drawing.Point(81, 167);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(88, 25);
            this.btnClearAll.TabIndex = 10;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // vScrollBar
            // 
            this.vScrollBar.LargeChange = 4;
            this.vScrollBar.Location = new System.Drawing.Point(239, 22);
            this.vScrollBar.Maximum = 15;
            this.vScrollBar.Minimum = 1;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(20, 139);
            this.vScrollBar.TabIndex = 3;
            this.vScrollBar.Value = 1;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
            // 
            // txtData4
            // 
            this.txtData4.Location = new System.Drawing.Point(27, 138);
            this.txtData4.Name = "txtData4";
            this.txtData4.Size = new System.Drawing.Size(209, 23);
            this.txtData4.TabIndex = 9;
            this.txtData4.Tag = "4";
            this.txtData4.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // lblData4
            // 
            this.lblData4.AutoSize = true;
            this.lblData4.Location = new System.Drawing.Point(6, 141);
            this.lblData4.Name = "lblData4";
            this.lblData4.Size = new System.Drawing.Size(15, 16);
            this.lblData4.TabIndex = 8;
            this.lblData4.Text = "5";
            // 
            // txtData3
            // 
            this.txtData3.Location = new System.Drawing.Point(27, 109);
            this.txtData3.Name = "txtData3";
            this.txtData3.Size = new System.Drawing.Size(209, 23);
            this.txtData3.TabIndex = 7;
            this.txtData3.Tag = "3";
            this.txtData3.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // lblData3
            // 
            this.lblData3.AutoSize = true;
            this.lblData3.Location = new System.Drawing.Point(6, 112);
            this.lblData3.Name = "lblData3";
            this.lblData3.Size = new System.Drawing.Size(15, 16);
            this.lblData3.TabIndex = 6;
            this.lblData3.Text = "4";
            // 
            // txtData2
            // 
            this.txtData2.Location = new System.Drawing.Point(27, 80);
            this.txtData2.Name = "txtData2";
            this.txtData2.Size = new System.Drawing.Size(209, 23);
            this.txtData2.TabIndex = 5;
            this.txtData2.Tag = "2";
            this.txtData2.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // lblData2
            // 
            this.lblData2.AutoSize = true;
            this.lblData2.Location = new System.Drawing.Point(6, 83);
            this.lblData2.Name = "lblData2";
            this.lblData2.Size = new System.Drawing.Size(15, 16);
            this.lblData2.TabIndex = 4;
            this.lblData2.Text = "3";
            // 
            // txtData1
            // 
            this.txtData1.Location = new System.Drawing.Point(27, 51);
            this.txtData1.Name = "txtData1";
            this.txtData1.Size = new System.Drawing.Size(209, 23);
            this.txtData1.TabIndex = 3;
            this.txtData1.Tag = "1";
            this.txtData1.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // lblData1
            // 
            this.lblData1.AutoSize = true;
            this.lblData1.Location = new System.Drawing.Point(6, 54);
            this.lblData1.Name = "lblData1";
            this.lblData1.Size = new System.Drawing.Size(15, 16);
            this.lblData1.TabIndex = 2;
            this.lblData1.Text = "2";
            // 
            // txtData0
            // 
            this.txtData0.Location = new System.Drawing.Point(27, 22);
            this.txtData0.Name = "txtData0";
            this.txtData0.Size = new System.Drawing.Size(209, 23);
            this.txtData0.TabIndex = 1;
            this.txtData0.Tag = "0";
            this.txtData0.TextChanged += new System.EventHandler(this.txtData_TextChanged);
            // 
            // lblData0
            // 
            this.lblData0.AutoSize = true;
            this.lblData0.Location = new System.Drawing.Point(6, 25);
            this.lblData0.Name = "lblData0";
            this.lblData0.Size = new System.Drawing.Size(15, 16);
            this.lblData0.TabIndex = 0;
            this.lblData0.Text = "1";
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Location = new System.Drawing.Point(63, 286);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 25);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Location = new System.Drawing.Point(149, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 25);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProductInfo
            // 
            this.lblProductInfo.Location = new System.Drawing.Point(15, 55);
            this.lblProductInfo.Name = "lblProductInfo";
            this.lblProductInfo.Size = new System.Drawing.Size(262, 20);
            this.lblProductInfo.TabIndex = 13;
            this.lblProductInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // FormAddItem
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(289, 325);
            this.Controls.Add(this.lblProductInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupData);
            this.Controls.Add(this.cboProduct);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddItem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAddItem_FormClosed);
            this.Load += new System.EventHandler(this.FormAddItem_Load);
            this.groupData.ResumeLayout(false);
            this.groupData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboProduct;
        private System.Windows.Forms.GroupBox groupData;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.TextBox txtData4;
        private System.Windows.Forms.Label lblData4;
        private System.Windows.Forms.TextBox txtData3;
        private System.Windows.Forms.Label lblData3;
        private System.Windows.Forms.TextBox txtData2;
        private System.Windows.Forms.Label lblData2;
        private System.Windows.Forms.TextBox txtData1;
        private System.Windows.Forms.Label lblData1;
        private System.Windows.Forms.TextBox txtData0;
        private System.Windows.Forms.Label lblData0;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProductInfo;
    }
}