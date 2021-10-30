namespace Synapse
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Syncfusion.Windows.Forms;

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
            this.InitializeComponent();

            this.Id = id;
            this.Value = value;
            this.Key = key;
            this.AllowNulls = allowNulls;

            dataFieldValueField.Enabled = false;
            dataFieldValueField.Text = this.Value;
            dataFieldKeyField.Enabled = false;
            dataFieldKeyField.Text = this.Key;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            dataFieldValueField.Enabled = true;
            dataFieldKeyField.Enabled = true;
        }

        private bool ValidateCode(string code)
        {
            var isValid = true;

            if (this.AllowNulls && code == "")
            {
                MessageBoxAdv.Show("Invalid Value \n \n Value cannot be empty.", "Hold On", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                isValid = false;
            }

            return isValid;
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            this.RemoveField?.Invoke(this, this.Id, this.Key, this.Value);
        }

        private async void dataFieldValueField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dataFieldValueField.Enabled)
            {
                if (this.Value == dataFieldValueField.Text && this.Key == dataFieldKeyField.Text)
                {
                    dataFieldValueField.Enabled = false;
                    return;
                }

                var newValue = dataFieldValueField.Text;
                var newKey = dataFieldKeyField.Text;
                if (this.ValidateCode(newValue) && this.ValidateCode(newKey))
                {
                    var updated = await Task.Run(() => this.EditField?.Invoke(this, this.Id, newKey, newValue));
                    if (updated.HasValue && updated == true)
                    {
                        this.Key = newKey;
                        this.Value = newValue;
                        dataFieldKeyField.Text = this.Key;
                        dataFieldValueField.Text = this.Value;
                    }
                    else
                    {
                        dataFieldKeyField.Text = this.Key;
                        dataFieldValueField.Text = this.Value;
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
                if (this.Value == dataFieldValueField.Text && this.Key == dataFieldKeyField.Text)
                {
                    dataFieldValueField.Enabled = false;
                    return;
                }

                var newValue = dataFieldValueField.Text;
                var newKey = dataFieldKeyField.Text;
                if (this.ValidateCode(newValue) && this.ValidateCode(newKey))
                {
                    var updated = await Task.Run(() => this.EditField?.Invoke(this, this.Id, newKey, newValue));
                    if (updated.HasValue && updated == true)
                    {
                        this.Key = newKey;
                        this.Value = newValue;
                        dataFieldKeyField.Text = this.Key;
                        dataFieldValueField.Text = this.Value;
                    }
                    else
                    {
                        dataFieldKeyField.Text = this.Key;
                        dataFieldValueField.Text = this.Value;
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