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

namespace Synapse.Modules
{
    public partial class ICRConfigurationForm : SfForm
    {
        #region Events 

        public delegate void OnConfigurationFinshed(string regionName, Orientation regionOrientaion, OMRRegionData regionData);
        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region Properties
        public string RegionName { get; set; }
        public Bitmap RegionImage { get; set; }
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        #region Public Methods
        public ICRConfigurationForm(Bitmap regionImage)
        {
            InitializeComponent();

            Initialize(regionImage);

            OnConfigurationFinishedEvent += OnConfigurationFinishedCallback;

            OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }
        internal ICRConfigurationForm(ICRConfiguration icrConfig, Bitmap regionImage)
        {
            InitializeComponent();

            Initialize(icrConfig, regionImage);

            OnConfigurationFinishedEvent += OnConfigurationFinishedCallback;

            OnFormInitializedEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Private Methods
        private void Initialize(ICRConfiguration icrConfig, Bitmap regionImage)
        {
            synchronizationContext = SynchronizationContext.Current;

            SetupForConfigured(icrConfig, regionImage);
        }
        private void Initialize(Bitmap regionImage)
        {
            synchronizationContext = SynchronizationContext.Current;

            SetupForConfiguration(regionImage);
        }

        private void OnConfigurationFinishedCallback(string name, Orientation orientation, OMRRegionData _regionData)
        {

        }

        private void SetupForConfiguration(Bitmap region = null)
        {
            RegionImage = region;
        }
        private void SetupForConfigured(ICRConfiguration icrConfig, Bitmap region = null)
        {
           
        }
        #endregion
    }
}