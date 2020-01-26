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
    public partial class DatabaseFieldCustom : UserControl
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Key { get; set; }

        public delegate bool DataFieldDelegate(object sender, int id, string key, string value);
        public event DataFieldDelegate RemoveField;
        public event DataFieldDelegate EditField;

        public bool AllowNulls { get; set; }

        public DatabaseFieldCustom(int id, string key, string value, bool allowNulls = false)
        {
            InitializeComponent();

            Id = id;
            Value = value;
            Key = key;
            AllowNulls = allowNulls;

            dataFieldValueField.Enabled = false;
            dataFieldValueField.Text = Value;
            dataFieldKeyField.Enabled = false;
            dataFieldKeyField.Text = Key;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            dataFieldValueField.Enabled = true;
            dataFieldKeyField.Enabled = true;
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
            RemoveField?.Invoke(this, Id, Key, Value);
        }

        private async void dataFieldValueField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dataFieldValueField.Enabled)
            {
                if (Value == dataFieldValueField.Text && Key == dataFieldKeyField.Text)
                {
                    dataFieldValueField.Enabled = false;
                    return;
                }

                var newValue = dataFieldValueField.Text;
                var newKey = dataFieldKeyField.Text;
                if (ValidateCode(newValue) && ValidateCode(newKey))
                {
                    bool? updated = await Task.Run(() => EditField?.Invoke(this, Id, newKey, newValue));
                    if (updated.HasValue && updated == true)
                    {
                        Key = newKey;
                        Value = newValue;
                        dataFieldKeyField.Text = Key;
                        dataFieldValueField.Text = Value;
                    }
                    else
                    {
                        dataFieldKeyField.Text = Key;
                        dataFieldValueField.Text = Value;
                    }
                }
                dataFieldValueField.Select(0, 0);
                dataFieldValueField.Enabled = false;
                dataFieldKeyField.Select(0, 0);
                dataFieldKeyField.Enabled = false;
            }
        }

        private async void dataFieldKeyField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dataFieldValueField.Enabled)
            {
                if (Value == dataFieldValueField.Text && Key == dataFieldKeyField.Text)
                {
                    dataFieldValueField.Enabled = false;
                    return;
                }

                var newValue = dataFieldValueField.Text;
                var newKey = dataFieldKeyField.Text;
                if (ValidateCode(newValue) && ValidateCode(newKey))
                {
                    bool? updated = await Task.Run(() => EditField?.Invoke(this, Id, newKey, newValue));
                    if (updated.HasValue && updated == true)
                    {
                        Key = newKey;
                        Value = newValue;
                        dataFieldKeyField.Text = Key;
                        dataFieldValueField.Text = Value;
                    }
                    else
                    {
                        dataFieldKeyField.Text = Key;
                        dataFieldValueField.Text = Value;
                    }
                }
                dataFieldValueField.Select(0, 0); 
                dataFieldValueField.Enabled = false;
                dataFieldKeyField.Select(0, 0);
                dataFieldKeyField.Enabled = false;
            }
        }
    }
}
