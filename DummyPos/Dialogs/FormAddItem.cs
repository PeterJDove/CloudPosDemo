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
using CloudPos;
using Touch.DummyPos;

namespace Touch.DummyPos
{
    public partial class FormAddItem : Form
    {
        public CloudPos.CloudPos CloudPOS;
        public List<object> Args { get; private set; }

        private Label[] lblData;
        private TextBox[] txtData;

        public FormAddItem(CloudPos.CloudPos cloudPOS)
        {
            InitializeComponent();
            CloudPOS = cloudPOS;
            CloudPOS.SimpleProduct += CloudPos_SimpleProduct;

            btnClearAll_Click(btnClearAll, null);
            lblData = new Label[] { lblData0, lblData1, lblData2, lblData3, lblData4 };
            txtData = new TextBox[] { txtData0, txtData1, txtData2, txtData3, txtData4 };
        }

        private void CloudPos_SimpleProduct(object sender, SimpleProductInfo e)
        {
            if (e.Exists)
            {
                if (e.Simple)
                    lblProductInfo.Text = "is a Simple Product";
                else
                    lblProductInfo.Text = "is a Complex Product";
            }
            else
                lblProductInfo.Text = "is not a known product";
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

            if (string.IsNullOrEmpty(cboProduct.Text))
            {
                btnOK.Enabled = false;
                lblProductInfo.Text = "";
            }
            else
            {
                btnOK.Enabled = true;
                CloudPOS.IsSimpleProduct(cboProduct.Text);
                lblProductInfo.Text = "...";
            }
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

        private void FormAddItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloudPOS.SimpleProduct -= CloudPos_SimpleProduct;
        }
    }
}
