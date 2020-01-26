using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;

namespace Synapse
{
    public partial class DatabaseField : UserControl
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public delegate bool DataFieldDelegate(object sender, int id, string value, bool isKey);
        public event DataFieldDelegate RemoveField;
        public event DataFieldDelegate EditField;

        public bool AllowNulls { get; set; }

        public DatabaseField(int id, string value, bool allowNulls = false)
        {
            InitializeComponent();

            Id = id;
            Value = value;
            AllowNulls = allowNulls;

            dataFieldValueField.Enabled = false;
            dataFieldValueField.Text = Value;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            dataFieldValueField.Enabled = true;
        }

        bool ValidateCode(string code)
        {
            bool isValid = true;

            if (AllowNulls && code == "")
            {
                MessageBoxAdv.Show("Invalid Value \n \n Value cannot be empty.", "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                isValid = false;
            }

            return isValid;
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            RemoveField?.Invoke(this, Id, Value, isKey);
        }

        private async void dataFieldValueField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dataFieldValueField.Enabled)
            {
                if (Value == dataFieldValueField.Text && verifyPanel.Value == (storedIsKey ? 1 : 0))
                {
                    dataFieldValueField.Enabled = false;
                    return;
                }

                var newValue = dataFieldValueField.Text;
                if (ValidateCode(newValue))
                {
                    bool? updated = await Task.Run(() => EditField?.Invoke(this, Id, newValue, isKey));
                    if (updated.HasValue && updated == true)
                    {
                        Value = newValue;
                        dataFieldValueField.Text = Value;
                    }
                    else
                    {
                        dataFieldValueField.Text = Value;
                    }
                }
                dataFieldValueField.Enabled = false;
            }
        }

        public bool isKey;
        bool storedIsKey;
        private async void VerifyPanel_Click(object sender, EventArgs e)
        {
            if (isKey)
                verifyPanel.Reset();

            isKey = !isKey;
            await Task.Run(() => EditField?.Invoke(this, Id, dataFieldValueField.Text, isKey));
        }
        private void DataFieldValueField_EnabledChanged(object sender, EventArgs e)
        {
            storedIsKey = isKey;
            verifyPanel.BackColor = dataFieldValueField.Enabled? Color.White : SystemColors.Control;
        }

        public void InitializeLayout()
        {
            Height = 54;
            Margin = new Padding(3, 1, 3, 1);
            Padding = new Padding(0, 0, 0, 0);
            verifyPanel.Location = new Point(verifyPanel.Location.X, verifyPanel.Location.Y + 15);
            verifyPanel.Size = new Size(30, 25);
        }

    }
}
