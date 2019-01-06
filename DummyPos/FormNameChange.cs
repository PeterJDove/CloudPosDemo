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
    internal enum NameChangeResult
    {
        SaveNewName,
        SaveNewConnection,
        SaveUndoNameChange,
        Cancel,
    }


    public partial class FormNameChange : Form
    {
        NameChangeResult _result;

        public FormNameChange()
        {
            InitializeComponent();
        }

        internal NameChangeResult Show(string newName)
        {
            lblConnectionName.Text = newName;

            this.ShowDialog(Program.FormMain());

            return _result;
        }

        private void btnNewName_Click(object sender, EventArgs e)
        {
            _result = NameChangeResult.SaveNewName;
            this.Hide();
        }

        private void btnNewConnection_Click(object sender, EventArgs e)
        {
            _result = NameChangeResult.SaveNewConnection;
            this.Hide();
        }

        private void btnUndoNameChange_Click(object sender, EventArgs e)
        {
            _result = NameChangeResult.SaveUndoNameChange;
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _result = NameChangeResult.Cancel;
            this.Hide();
        }

        private void FormNameChange_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                _result = NameChangeResult.Cancel;
            }
        }
    }
}
