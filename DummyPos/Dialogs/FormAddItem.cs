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
using Touch.CloudPos.Model;

namespace Touch.DummyPos
{
    /// <summary>
    /// This dialogue window is displayed when the user clicks either the <b>Add Item(...)</b> button 
    /// or <b>Show GUI(...)</b> button on the Main Form.  It allows the the user to choose which product
    /// is to be added and, if it is a complex product, to enter any additional information required.
    /// </summary>
    partial class FormAddItem : Form
    {
        private CloudPos.API CloudPOS;
        private List<string> _argNames { get; set; }
        private List<string> _argData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormAddItem"/> form.
        /// </summary>
        /// <param name="cloudPOS">A <see cref="CloudPos.API"/> which may be used to determine whether a product is simple or complex.</param>
        public FormAddItem(CloudPos.API cloudPOS)
        {
            InitializeComponent();
            CloudPOS = cloudPOS;
            CloudPOS.SimpleProduct += CloudPos_SimpleProduct;

            btnClearAll_Click(btnClearAll, null);
        }


        /// <summary>
        /// Gets the shortcut or EAN that identifies the product to be added (available after the dialogue has been closed)
        /// </summary>
        public string ShortcutOrEan
        {
            get { return cboProduct.Text; }
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
            cboProduct.Items.Add("9332104021591");
            cboProduct.Items.Add("9316423024686");
            cboProduct.SelectedIndex = 0;
            RefreshDisplay(vScrollBar.Minimum);
        }

        private void FormAddItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloudPOS.SimpleProduct -= CloudPos_SimpleProduct;
        }


        /// <summary>
        /// Shows the dialogue, assuming <see cref="API.ShowGui"/> will be called, for the selected product.
        /// </summary>
        /// <param name="owner">The <see cref="FormMain"/> instance</param>
        /// <returns></returns>
        public DialogResult ShowShowGuiDialog(Form owner)
        {
            // groupData.Enabled = false;
            this.Text = "Show GUI(...)";

            return ShowDialog(owner);
        }


        /// <summary>
        /// Shows the dialogue, assuming <see cref="API.AddItem"/> will be called, for the selected product.
        /// </summary>
        /// <param name="owner">The <see cref="FormMain"/> instance</param>
        /// <returns></returns>
        public DialogResult ShowAddItemDialog(Form owner)
        {
            // groupData.Enabled = false;
            this.Text = "Add Item(...)";

            return ShowDialog(owner);
        }

        private void cboProduct_TextChanged(object sender, EventArgs e)
        {
            int result = 0;
            if (string.IsNullOrEmpty(cboProduct.Text))
            {
                btnOK.Enabled = false;
                lblProductInfo.Text = "";
            }
            else
            {
                btnOK.Enabled = true;
                lblProductInfo.Text = "...";
                if (cboProduct.Text.Length == 13 && int.TryParse(cboProduct.Text, out result))
                    CloudPOS.IsSimpleProduct(cboProduct.Text);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var n = vScrollBar.Value + tableLayoutData.GetRow(textBox); // Relies on first Arg being in Row 0
            while (_argData.Count <= n)
            {
                _argNames.Add("");
                _argData.Add("");
            }
            _argNames[n] = textBox.Text;
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var n = vScrollBar.Value + tableLayoutData.GetRow(textBox); // Relies on first Arg being in Row 0
            while (_argData.Count <= n)
            {
                _argNames.Add("");
                _argData.Add("");
            }
            _argData[n] = textBox.Text;
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            RefreshDisplay(vScrollBar.Value);
        }

        private void RefreshDisplay(int n)
        {
            var lblData = new Label[] { lblData0, lblData1, lblData2, lblData3, lblData4 };
            var txtName = new TextBox[] { txtName0, txtName1, txtName2, txtName3, txtName4 };
            var txtData = new TextBox[] { txtData0, txtData1, txtData2, txtData3, txtData4 };

            for (int i = 0; i < 5; i++)
            {
                lblData[i].Text = (n + i + 1).ToString();
                if (n + i < _argNames.Count)
                {
                    txtName[i].Text = _argNames[n + i].ToString();
                    txtData[i].Text = _argData[n + i].ToString();
                }
                else
                {
                    txtName[i].Text = "";
                    txtData[i].Text = "";
                }
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            _argNames = new List<string>();
            _argData = new List<string>();
            vScrollBar.Value = vScrollBar.Minimum;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.Text == "Show GUI(...)")
            {
                CloudPOS.ShowGui(ShortcutOrEan);
            }
            else
            {
                var args = ArgsDictionary();
                if (args.Count == 0)
                    CloudPOS.AddItem(ShortcutOrEan);
                else
                    CloudPOS.AddItem(ShortcutOrEan, args);
            }
            this.DialogResult = DialogResult.OK;
        }

        private Dictionary<string, string> ArgsDictionary()
        {
            var args = new Dictionary<string, string>();
            if (_argNames.Count > 0)
            {
                for (int n = 0; n < _argNames.Count; n++)
                {
                    if (!string.IsNullOrEmpty(_argNames[n]))
                    {
                        if (_argData[n] != null)
                            args.Add(_argNames[n], _argData[n]);
                        else
                            args.Add(_argNames[n], "");
                    }
                }
            }
            return args;
        }



    }
}
