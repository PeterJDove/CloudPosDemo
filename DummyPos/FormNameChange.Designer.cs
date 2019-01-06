namespace Touch.DummyPos
{
    partial class FormNameChange
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNameChange));
            this.label1 = new System.Windows.Forms.Label();
            this.lblConnectionName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnNewName = new System.Windows.Forms.Button();
            this.btnNewConnection = new System.Windows.Forms.Button();
            this.btnUndoNameChange = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(294, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "You have changed the name of the Connection to:";
            // 
            // lblConnectionName
            // 
            this.lblConnectionName.AutoSize = true;
            this.lblConnectionName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionName.Location = new System.Drawing.Point(12, 41);
            this.lblConnectionName.Name = "lblConnectionName";
            this.lblConnectionName.Size = new System.Drawing.Size(119, 16);
            this.lblConnectionName.TabIndex = 1;
            this.lblConnectionName.Text = "Connection Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Choose what do you want to do:";
            // 
            // btnNewName
            // 
            this.btnNewName.Location = new System.Drawing.Point(15, 108);
            this.btnNewName.Name = "btnNewName";
            this.btnNewName.Size = new System.Drawing.Size(291, 27);
            this.btnNewName.TabIndex = 4;
            this.btnNewName.Text = "Save Current Connection with New Name";
            this.btnNewName.UseVisualStyleBackColor = true;
            this.btnNewName.Click += new System.EventHandler(this.btnNewName_Click);
            // 
            // btnNewConnection
            // 
            this.btnNewConnection.Location = new System.Drawing.Point(15, 141);
            this.btnNewConnection.Name = "btnNewConnection";
            this.btnNewConnection.Size = new System.Drawing.Size(291, 27);
            this.btnNewConnection.TabIndex = 5;
            this.btnNewConnection.Text = "Save a NEW Connection";
            this.btnNewConnection.UseVisualStyleBackColor = true;
            this.btnNewConnection.Click += new System.EventHandler(this.btnNewConnection_Click);
            // 
            // btnUndoNameChange
            // 
            this.btnUndoNameChange.Location = new System.Drawing.Point(15, 176);
            this.btnUndoNameChange.Name = "btnUndoNameChange";
            this.btnUndoNameChange.Size = new System.Drawing.Size(291, 27);
            this.btnUndoNameChange.TabIndex = 6;
            this.btnUndoNameChange.Text = "Undo Name Change, and Save";
            this.btnUndoNameChange.UseVisualStyleBackColor = true;
            this.btnUndoNameChange.Click += new System.EventHandler(this.btnUndoNameChange_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(15, 209);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(291, 27);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormNameChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 253);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUndoNameChange);
            this.Controls.Add(this.btnNewConnection);
            this.Controls.Add(this.btnNewName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblConnectionName);
            this.Controls.Add(this.label1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNameChange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Persist Changes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNameChange_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblConnectionName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnNewName;
        private System.Windows.Forms.Button btnNewConnection;
        private System.Windows.Forms.Button btnUndoNameChange;
        private System.Windows.Forms.Button btnCancel;
    }
}