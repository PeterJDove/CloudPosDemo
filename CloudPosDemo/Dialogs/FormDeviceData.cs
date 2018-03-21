using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Touch.CloudPosDemo
{
    public partial class FormDeviceData : Form
    {
        string _device = null;

        public FormDeviceData()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(Form owner, string device)
        {
            _device = device;
            if (_device == CloudPos.CloudPos.DEV_BARCODE)
                lblPrompt.Text = "Enter Bar Code Data";
            else if (_device == CloudPos.CloudPos.DEV_MAGSTRIPE)
                lblPrompt.Text = "Enter Track 2 Data";

            return ShowDialog(owner);
        }
        
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
