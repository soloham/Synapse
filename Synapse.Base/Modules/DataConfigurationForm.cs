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
    public partial class DataConfigurationForm : SfForm
    {
        #region Properties
        public List<ConfigurationBase> Configurations { get; set; }
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        private Dispatcher dispatcher;
        #endregion

        #region Events
        #endregion

        #region General Methods
        public DataConfigurationForm(List<ConfigurationBase> configurations)
        {
            InitializeComponent();
            Configurations = configurations;

            Awake();
        }
        private void Awake()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            synchronizationContext = SynchronizationContext.Current;

            PopulateListItems();
        }
        #endregion

        #region UI Methods
        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (Configurations.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (int i = 0; i < Configurations.Count; i++)
            {
                ConfigureDataListItem configureDataListItem = ConfigureDataListItem.Create(Configurations[i]);
                configureDataListItem.OnControlButtonPressedEvent += OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(configureDataListItem);
                configureDataListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }
        private void OnConfigControlButtonPressed(object sender, ControlButton controlButton)
        {
            ConfigureDataListItem configListItem = (ConfigureDataListItem)sender;
            switch (controlButton)
            {
                case ControlButton.Delete:
                    DeleteConfiguration(configListItem);
                    break;
                case ControlButton.MoveUp:
                    MoveConfiguration(configListItem, true);
                    break;
                case ControlButton.MoveDown:
                    MoveConfiguration(configListItem, false);
                    break;
                case ControlButton.Configure:
                    ConfigurationBase configuration = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);
                    ConfigureConfiguration(configuration);
                    break;
            }
        }
        private void DataConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (int i = 0; i < allConfigs.Count; i++)
            {
                bool isSaved = ConfigurationBase.Save(allConfigs[i], out Exception ex);

                if (!isSaved)
                    Messages.SaveFileException(ex);
            }
        }

        #endregion

        #region Main Methods
        private void DeleteConfiguration(ConfigureDataListItem configListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this configuration?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;

            ConfigurationBase configuration = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);

            bool isDeleted = ConfigurationBase.Delete(configuration, out Exception ex);

            if (isDeleted)
            {
                containerFlowPanel.Controls.Remove(configListItem);
                ConfigurationsManager.RemoveConfiguration(configListItem, configuration);

                if (Configurations.Count == 0)
                {
                    if (!containerFlowPanel.Controls.Contains(emptyListLabel))
                        containerFlowPanel.Controls.Add(emptyListLabel);

                    emptyListLabel.Visible = true;
                }
            }
            else
            {
                Messages.DeleteDirectoryException(ex);
            }
        }
        private void MoveConfiguration(ConfigureDataListItem configListItem, bool isUp)
        {
            ConfigurationBase config = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);
            int curIndex = containerFlowPanel.Controls.IndexOf(configListItem);
            int moveIndex = 0;
            if(isUp)
            {
                moveIndex = curIndex == 0? 0 : curIndex - 1;
            }
            else
            {
                moveIndex = curIndex == Configurations.Count-1? curIndex : curIndex + 1;
            }
            containerFlowPanel.Controls.SetChildIndex(configListItem, moveIndex);
            Configurations.RemoveAt(curIndex);
            Configurations.Insert(moveIndex, config);
            config.ProcessingIndex = moveIndex;
            Configurations[curIndex].ProcessingIndex = curIndex;
        }
        private void ConfigureConfiguration(ConfigurationBase configuration)
        {
            switch (configuration.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    OMRConfiguration omrConfiguration = (OMRConfiguration)configuration;
                    OMRConfigurationForm configurationForm = new OMRConfigurationForm(omrConfiguration, configuration.GetConfigArea.ConfigBitmap);
                    configurationForm.OnConfigurationFinishedEvent += async (string name, Orientation orientation, OMRRegionData regionData) =>
                    {
                        bool isSaved = false;
                        Exception ex = new Exception();

                        await Task.Run(() =>
                        {
                            omrConfiguration.Title = name;
                            omrConfiguration.Orientation = orientation;
                            omrConfiguration.RegionData = regionData;

                            isSaved = OMRConfiguration.Save(configuration, out ex);
                        });

                        if (!isSaved)
                            Messages.SaveFileException(ex);
                    };
                    configurationForm.OnFormInitializedEvent += (object sender, EventArgs args) =>
                    {
                    };
                    configurationForm.ShowDialog();
                    break;
                case MainConfigType.BARCODE:
                    break;
                case MainConfigType.ICR:
                    break;
            }
            
        }
        #endregion
    }
}