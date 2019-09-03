using System;
using System.Drawing;
using Syncfusion.WinForms.Controls;
using System.Threading;
using Synapse.Core.Managers;

namespace Synapse.Modules
{
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
        internal ICRConfigurationForm(Bitmap configAreaBmp, string regionName = "")
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            Initialize(configAreaBmp, regionName);
        }
        #endregion

        #region Private Methods
        private void Initialize(Bitmap configAreaBmp, string regionName = "")
        {
            imageBox.Image = configAreaBmp;
            RegionName = regionName == "" ? "ICR Region Name" : regionName;
            icrRegionNameTextBox.Text = RegionName;
        }

        private bool ValidateName(string name)
        {
            bool isValid = true;

            if (name == "" || name[0] == ' ' || name[name.Length - 1] == ' ')
                isValid = false;

            if (isValid)
                isValid = ConfigurationsManager.ValidateName(name);

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
            string name = icrRegionNameTextBox.Text;

            if (ValidateName(name))
            {
                RegionName = name;
                OnConfigurationFinishedEvent?.Invoke(RegionName);
            }
        }
        #endregion
    }
}