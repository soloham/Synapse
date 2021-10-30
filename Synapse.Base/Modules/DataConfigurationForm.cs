using static Synapse.Controls.ConfigureDataListItem;

namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Threading;

    using Synapse.Controls;
    using Synapse.Core.Configurations;
    using Synapse.Core.Managers;
    using Synapse.Utilities;

    using Syncfusion.WinForms.Controls;

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
            this.InitializeComponent();
            this.Configurations = configurations;

            this.Awake();
        }

        private void Awake()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            synchronizationContext = SynchronizationContext.Current;

            this.PopulateListItems();
        }

        #endregion

        #region UI Methods

        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (this.Configurations.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (var i = 0; i < this.Configurations.Count; i++)
            {
                var configureDataListItem = Create(this.Configurations[i]);
                configureDataListItem.OnControlButtonPressedEvent += this.OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(configureDataListItem);
                configureDataListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }

        private void OnConfigControlButtonPressed(object sender, ControlButton controlButton)
        {
            var configListItem = (ConfigureDataListItem)sender;
            switch (controlButton)
            {
                case ControlButton.Delete:
                    this.DeleteConfiguration(configListItem);
                    break;

                case ControlButton.MoveUp:
                    this.MoveConfiguration(configListItem, true);
                    break;

                case ControlButton.MoveDown:
                    this.MoveConfiguration(configListItem, false);
                    break;

                case ControlButton.Configure:
                    var configuration = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);
                    this.ConfigureConfiguration(configuration);
                    break;
            }
        }

        private void DataConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (var i = 0; i < allConfigs.Count; i++)
            {
                var isSaved = ConfigurationBase.Save(allConfigs[i], out var ex);

                if (!isSaved)
                {
                    Messages.SaveFileException(ex);
                }
            }
        }

        #endregion

        #region Main Methods

        private void DeleteConfiguration(ConfigureDataListItem configListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this configuration?", "Hold On",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }

            var configuration = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);

            var isDeleted = ConfigurationBase.Delete(configuration, out var ex);

            if (isDeleted)
            {
                containerFlowPanel.Controls.Remove(configListItem);
                ConfigurationsManager.RemoveConfiguration(configListItem, configuration);

                if (this.Configurations.Count == 0)
                {
                    if (!containerFlowPanel.Controls.Contains(emptyListLabel))
                    {
                        containerFlowPanel.Controls.Add(emptyListLabel);
                    }

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
            var config = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);
            var curIndex = containerFlowPanel.Controls.IndexOf(configListItem);
            var moveIndex = 0;
            if (isUp)
            {
                moveIndex = curIndex == 0 ? 0 : curIndex - 1;
            }
            else
            {
                moveIndex = curIndex == this.Configurations.Count - 1 ? curIndex : curIndex + 1;
            }

            containerFlowPanel.Controls.SetChildIndex(configListItem, moveIndex);
            this.Configurations.RemoveAt(curIndex);
            this.Configurations.Insert(moveIndex, config);
            config.ProcessingIndex = moveIndex;
            this.Configurations[curIndex].ProcessingIndex = curIndex;
        }

        private void ConfigureConfiguration(ConfigurationBase configuration)
        {
            switch (configuration.GetMainConfigType)
            {
                case MainConfigType.OMR:
                    var omrConfiguration = (OMRConfiguration)configuration;
                    var configurationForm =
                        new OMRConfigurationForm(omrConfiguration, configuration.GetConfigArea.ConfigBitmap);
                    configurationForm.OnConfigurationFinishedEvent +=
                        async (name, orientation, regionData) =>
                        {
                            var isSaved = false;
                            var ex = new Exception();

                            await Task.Run(() =>
                            {
                                omrConfiguration.Title = name;
                                omrConfiguration.Orientation = orientation;
                                omrConfiguration.RegionData = regionData;

                                isSaved = ConfigurationBase.Save(configuration, out ex);
                            });

                            if (!isSaved)
                            {
                                Messages.SaveFileException(ex);
                            }
                        };
                    configurationForm.OnFormInitializedEvent += (sender, args) => { };
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