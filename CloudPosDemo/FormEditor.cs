using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Touch.TestRecorder.Scripting;

namespace Touch.TestRecorder
{
    public partial class FormEditor : Form
    {
        private Script _script;


        public FormEditor()
        {
            InitializeComponent();
        }


        public void EditScript (Script script)
        {
            _script = script;
            txtName.Text = _script.Name;
            txtDescription.Text = _script.Description;
            SetTitle();

            listView.Items.Clear();
            foreach (Step step in _script)
            {
                var item = new ListViewItem(new[] { step.ToString(), step.Description });
                item.ImageKey = step.Icon;
                listView.Items.Add(item);
            }
            this.Show(Program.FormMain());
        }

        private void SetTitle()
        {
            if (_script != null && !string.IsNullOrEmpty(_script.FileName))
                this.Text = "Script Editor - [" + _script.FileName + "]";
            else
                this.Text = "Script Editor - [unsaved]";
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_script != null)
            {
                if (!string.IsNullOrEmpty(_script.FileName) && sender.Equals(btnSave))
                {
                    _script.SaveXML(_script.FileName);
                }
                else
                {
                    saveFileDialog.InitialDirectory = "C:\\scripts";
                    saveFileDialog.DefaultExt = "xml";
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 0;
                    if (!string.IsNullOrEmpty(_script.FileName))
                        saveFileDialog.FileName = Path.GetFileName(_script.FileName);
                    else if (!string.IsNullOrWhiteSpace(_script.Name))
                        saveFileDialog.FileName = _script.Name + ".xml";
                    else
                        saveFileDialog.FileName = "";

                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        _script.SaveXML(saveFileDialog.FileName);
                        SetTitle();
                    }
                }
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _script.Name = txtName.Text.Trim();
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            _script.Description = txtDescription.Text.Trim();
        }


    }
}
