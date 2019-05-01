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
    /// The result returned from calling <see cref="FormNameChange.Show"/>
    /// </summary>
    internal enum NameChangeResult
    {
        /// <summary>
        /// Save the current connection with the new name.
        /// </summary>
        SaveNewName,
        /// <summary>
        /// Save a new connection (creating a clone or variant of the current one).
        /// </summary>
        SaveNewConnection,
        /// <summary>
        /// Save the current connection, without changing its name.
        /// </summary>
        SaveUndoNameChange,
        /// <summary>
        /// Do not save the connection at this time.
        /// </summary>
        Cancel,
    }


    /// <summary>
    /// This dialogue window is displayed when the user clicks the <b>Persist Changes</b> button 
    /// on the Options tab of the Main Form, and the connection name has been changed.
    /// </summary>
    /// <remarks>
    /// The purpose of this form is to clarify the user's intention:
    /// <list type="bullet">
    /// <item>Save the current connection with the new name</item>
    /// <item>Save a new connection (creating a clone or variant of the current one)</item>
    /// <item>Save the current connection, without changing its name</item>
    /// <item>Do not save the connection at this time</item>
    /// </list>
    /// </remarks>>
    internal partial class FormNameChange : Form
    {
        NameChangeResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormNameChange"/> form. 
        /// </summary>
        public FormNameChange()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the dialogue.
        /// </summary>
        /// <param name="newName">The new, or changed, name of the Connection</param>
        /// <returns></returns>
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
