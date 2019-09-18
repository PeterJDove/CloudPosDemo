namespace Touch.EssentialPrinter
{
    partial class HiddenForm
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
            this.webControl = new EO.WinForm.WebControl();
            this.webView = new EO.WebBrowser.WebView();
            this.SuspendLayout();
            // 
            // webControl
            // 
            this.webControl.BackColor = System.Drawing.Color.White;
            this.webControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl.Location = new System.Drawing.Point(0, 0);
            this.webControl.Name = "webControl";
            this.webControl.Size = new System.Drawing.Size(526, 315);
            this.webControl.TabIndex = 0;
            this.webControl.Text = "webControl";
            this.webControl.WebView = this.webView;
            // 
            // webView
            // 
            this.webView.InputMsgFilter = null;
            this.webView.ObjectForScripting = null;
            // 
            // HiddenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 315);
            this.Controls.Add(this.webControl);
            this.Name = "HiddenForm";
            this.Text = "HiddenForm";
            this.ResumeLayout(false);

        }

        #endregion

        public EO.WinForm.WebControl webControl;
        public EO.WebBrowser.WebView webView;
    }
}