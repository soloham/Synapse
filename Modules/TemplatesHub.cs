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

            if (templatesLayoutPanel.Controls.Count == 0)
                emptyListLabel.Visible = true;
        }

        private void CreateTemplateBtn_Click(object sender, EventArgs e)
        {
            if (!createTemplatePanel.Visible)
            {
                containerPanel.Controls.SetChildIndex(createTemplatePanel, 0);
                createTemplatePanel.Visible = true;
            }
            else
            {
                createTemplatePanel.Visible = false;
                containerPanel.Controls.SetChildIndex(createTemplatePanel, 1);
            }
        }
    }
}
