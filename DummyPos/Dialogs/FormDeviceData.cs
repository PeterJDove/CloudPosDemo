using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Touch.DummyPos
{
    /// <summary>
    /// This dialogue window is displayed when the user clicks either the <b>Scan Barcode</b> button 
    /// or <b>Swipe card</b> button on the Main Form.  It allows the the user to simulate data entry
    /// from a peripheral device, such as Bar Code scanner, or Mag Stripe reader.
    /// </summary>
    partial class FormDeviceData : Form
    {
        string _device = null;

        /// <summary>
        /// Initalizes a new instance of the <see cref="FormDeviceData"/> form.
        /// </summary>
        public FormDeviceData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the dialogue window.
        /// </summary>
        /// <param name="owner">The <see cref="FormMain"/> instance</param>
        /// <param name="device">The name of the device to be simulated.</param>
        /// <returns></returns>
        public DialogResult ShowDialog(Form owner, string device)
        {
            _device = device;
            if (_device == CloudPos.API.DEV_BARCODE)
                lblPrompt.Text = "Enter Bar Code Data";
            else if (_device == CloudPos.API.DEV_MAGSTRIPE)
                lblPrompt.Text = "Enter Track 2 Data";

            return ShowDialog(owner);
        }
        
        /// <summary>
        /// Gets the Data that was "input" from the device (available after the dialogue has been closed)
        /// </summary>
        public string Data
        {
            get { return cboData.Text;}
        }

        private void cboData_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (cboData.Text.Length > 0);
        }

        private void cboData_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (cboData.Text.Length > 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }




    }
}
