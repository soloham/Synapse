namespace Synapse.Modules
{
    using System;

    using Syncfusion.WinForms.Controls;

    public partial class EnterValueForm : SfForm
    {
        #region Events

        public event EventHandler<string> OnValueSet;

        #endregion

        #region General Methods

        public EnterValueForm(bool showAsPassword = true)
        {
            this.InitializeComponent();

            if (!showAsPassword)
            {
                valueTextBox.UseSystemPasswordChar = false;
                valueTextBox.PasswordChar = '\0';
            }
        }

        #endregion

        private void finishBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.OnValueSet?.Invoke(this, valueTextBox.Text);
            this.Dispose();
        }
    }
}