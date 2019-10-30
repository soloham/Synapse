using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Controls.Custom
{
    public partial class CheckboxEx : Syncfusion.Windows.Forms.Tools.CheckBoxAdv
    {
        public CheckboxEx()
        {
            InitializeComponent();
        }
        public CheckboxEx(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int h = this.ClientSize.Height - 2;
            Rectangle rc = new Rectangle(new Point(0, 1), new Size(h, h));
            Syncfusion.Windows.Forms.ThemedCheckBoxDrawing themedCheckBoxDrawing = new Syncfusion.Windows.Forms.ThemedCheckBoxDrawing();
            themedCheckBoxDrawing.DrawCheckBox(e.Graphics, rc, this.Checked ? ButtonState.Checked : ButtonState.Normal, false);
            //ControlPaint.DrawCheckBox(e.Graphics, rc, this.Checked ? ButtonState.Checked : ButtonState.Normal);
        }
    }
}
