using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Touch.CloudPos;
using Touch.DummyPos;

namespace Touch.DummyPos
{
    /// <summary>
    /// This dialogue window is displayed when the user clicks the <b>Refund</b> button 
    /// on the Main Form.  It allows the the user to enter the transaction ID .
    /// </summary>
    partial class FormRefund : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormRefund"/> form.
        /// </summary>
        public FormRefund()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the transaction ID to be refunded (available after the dialogue has been closed)
        /// </summary>
        public string TransactionID
        {
            get { return txtTransactionID.Text; }
        }

        /// <summary>
        /// Gets the reason for the refund (available after the dialogue has been closed)
        /// </summary>
        public RefundReason Reason
        {
            get
            {
                if (txtTransactionID.Text == "")
                    return RefundReason.OTHER;
                else
                    return (RefundReason)cboReason.SelectedIndex;
            }
        }

        private void FormAddItem_Load(object sender, EventArgs e)
        {
            cboReason.Items.Add(RefundReason.INCORRECT_PRODUCT.ToString());
            cboReason.Items.Add(RefundReason.FAULTY_PRODUCT.ToString());
            cboReason.Items.Add(RefundReason.CUSTOMER_CHANGED_MIND.ToString());
            cboReason.Items.Add(RefundReason.OTHER.ToString());
        }

        /// <summary>
        /// Shows the dialogue.
        /// </summary>
        /// <param name="owner">The <see cref="FormMain"/> instance</param>
        /// <returns></returns>
        public DialogResult ShowRefundDialog(Form owner)
        {
            txtTransactionID.Text = "";
            cboReason.SelectedIndex = -1;

            return ShowDialog(owner);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void txtTransactionID_TextChanged(object sender, EventArgs e)
        {
            txtTransactionID.Text = txtTransactionID.Text.Trim();
            if (string.IsNullOrEmpty(txtTransactionID.Text))
            {
                lblReason.Enabled = false;
                cboReason.Enabled = false;
                cboReason.SelectedIndex = -1;
            }
            else
            {
                lblReason.Enabled = true;
                cboReason.Enabled = true;
                cboReason.SelectedIndex = 3; // OTHER
            }
        }
    }
}
