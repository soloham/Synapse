namespace Synapse.Modules
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Synapse.Core.Managers;
    using Synapse.Utilities;

    using Syncfusion.WinForms.Controls;

    public partial class ICRConfigurationForm : SfForm
    {
        #region Events

        public delegate void OnConfigurationFinshed(string regionName);

        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region Properties

        public string RegionName { get; set; }

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;

        #endregion

        #region Public Methods

        public ICRConfigurationForm(Bitmap configAreaBmp, string regionName = "")
        {
            this.InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            this.Initialize(configAreaBmp, regionName);
        }

        #endregion

        #region Private Methods

        private void Initialize(Bitmap configAreaBmp, string regionName = "")
        {
            imageBox.Image = configAreaBmp;
            this.RegionName = regionName == "" ? "ICR Region Name" : regionName;
            icrRegionNameTextBox.Text = this.RegionName;
        }

        private bool ValidateName(string name)
        {
            var isValid = true;

            if (name == "" || name[0] == ' ' || name[name.Length - 1] == ' ')
            {
                isValid = false;
            }

            if (isValid)
            {
                isValid = ConfigurationsManager.ValidateName(name);
            }

            if (isValid)
            {
                icrRegionNameLabel.ForeColor = Color.FromArgb(255, 68, 68, 68);
                icrRegionNameTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
            }
            else
            {
                icrRegionNameLabel.ForeColor = Color.Crimson;
                icrRegionNameTextBox.ForeColor = Color.Crimson;
            }

            return isValid;
        }

        private void FinishBtn_Click(object sender, EventArgs e)
        {
            var name = icrRegionNameTextBox.Text;

            if (this.ValidateName(name))
            {
                this.RegionName = name;
                this.OnConfigurationFinishedEvent?.Invoke(this.RegionName);
            }
        }

        #region  ImageBoxPanel Setup

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
        {
            selectionToolStripStatusLabel.Text = Functions.FormatRectangle(imageBox.SelectionRegion);
        }

        private void imageBox_Selected(object sender, EventArgs e)
        {
        }

        private void ImageBox_SelectionResized(object sender, EventArgs e)
        {
        }

        private void actualSizeToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ActualSize();
        }

        private void selectAllToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.SelectAll();
        }

        private void selectNoneToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.SelectNone();
        }

        private void showImageRegionToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.Invalidate();
        }

        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomIn();
        }

        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomOut();
        }

        #endregion

        #endregion
    }
}