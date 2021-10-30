namespace Synapse.Controls.Custom
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Syncfusion.Windows.Forms;
    using Syncfusion.Windows.Forms.Tools;

    public partial class CheckboxEx : CheckBoxAdv
    {
        public CheckboxEx()
        {
            this.InitializeComponent();
        }

        public CheckboxEx(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var h = this.ClientSize.Height - 2;
            var rc = new Rectangle(new Point(0, 1), new Size(h, h));
            var themedCheckBoxDrawing = new ThemedCheckBoxDrawing();
            themedCheckBoxDrawing.DrawCheckBox(e.Graphics, rc, this.Checked ? ButtonState.Checked : ButtonState.Normal,
                false);
            //ControlPaint.DrawCheckBox(e.Graphics, rc, this.Checked ? ButtonState.Checked : ButtonState.Normal);
        }
    }
}