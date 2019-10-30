using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Synapse.Controls;
using Synapse.Core.Configurations;
using Synapse.Core.Managers;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Memory;
using Syncfusion.DataSource.Extensions;
using Syncfusion.WinForms.Controls;
using static Synapse.Controls.ConfigureDataListItem;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using Synapse.Core;
using Synapse.Utilities.Enums;

namespace Synapse.Modules
{
    public partial class EnterValueForm : SfForm
    {
        #region Events
        public event EventHandler<string> OnValueSet;
        #endregion

        #region General Methods
        public EnterValueForm()
        {
            InitializeComponent();
        }
        #endregion

        private void finishBtn_Click(object sender, EventArgs e)
        {
            Hide();
            OnValueSet?.Invoke(this, valueTextBox.Text);
            Dispose();
        }
    }
}