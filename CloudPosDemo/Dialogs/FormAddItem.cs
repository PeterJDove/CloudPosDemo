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
using Touch.CloudPosDemo;

namespace Touch.CloudPosDemo
{
    public partial class FormAddItem : Form
    {
        public List<object> Args { get; private set; }

        private Label[] lblData;
        private TextBox[] txtData;

        public FormAddItem()
        {
            InitializeComponent();
            btnClearAll_Click(btnClearAll, null);
            lblData = new Label[] { lblData0, lblData1, lblData2, lblData3, lblData4 };
            txtData = new TextBox[] { txtData0, txtData1, txtData2, txtData3, txtData4 };
        }

        private void FormAddItem_Load(object sender, EventArgs e)
        {
            cboProduct.Items.Add("9316423024686");
            cboProduct.SelectedIndex = 0;
            RefreshDisplay(vScrollBar.Minimum);
        }

        public DialogResult ShowShowGuiDialog(Form owner)
        {
            groupData.Enabled = false;
            this.Text = "Show GUI(...)";

            return ShowDialog(owner);
        }


        public DialogResult ShowAddItemDialog(Form owner)
        {
            groupData.Enabled = false;
            this.Text = "Add Item(...)";

            return ShowDialog(owner);
        }

        private void cboProduct_TextChanged(object sender, EventArgs e)
        {
            if (Args.Count == 0)
                Args.Add(cboProduct.Text);
            else
                Args[0] = cboProduct.Text;

            btnOK.Enabled = (!string.IsNullOrEmpty(cboProduct.Text));
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            TextBox txtData = (TextBox)sender;
            var n = vScrollBar.Value + int.Parse(txtData.Tag.ToString());
            if (n < Args.Count)
                Args[n] = txtData.Text;
            else
                Args.Add(txtData.Text);
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            RefreshDisplay(vScrollBar.Value); 
        }

        private void RefreshDisplay(int n)
        {
            if (Args.Count > 0)
            {
                cboProduct.Text = Args[0].ToString();
                for (int i = 0; i < 5; i++)
                {
                    lblData[i].Text = (n + i).ToString();
                    if (n + i < Args.Count)
                        txtData[i].Text = Args[n + i].ToString(); 
                    else
                        txtData[i].Text = "";
                }
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            Args = new List<object>();
            vScrollBar.Value = 1;
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
