using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Syncfusion.WinForms.Controls;

namespace Synapse.Modules
{
    public partial class TemplatesHub : SfForm
    {
        public TemplatesHub()
        {
            InitializeComponent();
            sfListView1.Controls.Add(new Controls.TemplateListItem());
        }
    }
}
