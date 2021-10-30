namespace Synapse
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Syncfusion.Windows.Forms;

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
            this.InitializeComponent();

            this.Id = id;
            this.Value = value;
            this.AllowNulls = allowNulls;

            dataFieldValueField.Enabled = false;
            dataFieldValueField.Text = this.Value;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            dataFieldValueField.Enabled = true;
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
            this.RemoveField?.Invoke(this, this.Id, this.Value, isKey);
        }

        private async void dataFieldValueField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dataFieldValueField.Enabled)
            {
                if (this.Value == dataFieldValueField.Text && verifyPanel.Value == (storedIsKey ? 1 : 0))
                {
                    dataFieldValueField.Enabled = false;
                    return;
                }

                var newValue = dataFieldValueField.Text;
                if (this.ValidateCode(newValue))
                {
                    var updated = await Task.Run(() => this.EditField?.Invoke(this, this.Id, newValue, isKey));
                    if (updated.HasValue && updated == true)
                    {
                        this.Value = newValue;
                        dataFieldValueField.Text = this.Value;
                    }
                    else
                    {
                        dataFieldValueField.Text = this.Value;
                    }
                }

                dataFieldValueField.Enabled = false;
            }
        }

        public bool isKey;
        private bool storedIsKey;

        private async void VerifyPanel_Click(object sender, EventArgs e)
        {
            if (isKey)
            {
                verifyPanel.Reset();
            }

            isKey = !isKey;
            await Task.Run(() => this.EditField?.Invoke(this, this.Id, dataFieldValueField.Text, isKey));
        }

        private void DataFieldValueField_EnabledChanged(object sender, EventArgs e)
        {
            storedIsKey = isKey;
            verifyPanel.BackColor = dataFieldValueField.Enabled ? Color.White : SystemColors.Control;
        }

        public void InitializeLayout()
        {
            this.Height = 54;
            this.Margin = new Padding(3, 1, 3, 1);
            this.Padding = new Padding(0, 0, 0, 0);
            verifyPanel.Location = new Point(verifyPanel.Location.X, verifyPanel.Location.Y + 15);
            verifyPanel.Size = new Size(30, 25);
        }
    }
}