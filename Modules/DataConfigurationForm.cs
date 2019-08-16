using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Synapse.Modules
{
    public partial class DataConfigurationForm : SfForm
    {
        #region Properties
        internal List<ConfigurationBase> Configurations { get; set; }
        #endregion

        #region Events
        #endregion

        #region General Methods
        internal DataConfigurationForm(List<ConfigurationBase> configurations)
        {
            InitializeComponent();
            Configurations = configurations;

            Awake();
        }
        private void Awake()
        {
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

            foreach (var config in Configurations)
            {
                ConfigureDataListItem configureDataListItem = ConfigureDataListItem.Create(config);
                configureDataListItem.OnControlButtonPressedEvent += OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(configureDataListItem);
                configureDataListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }
        private void OnConfigControlButtonPressed(object sender, ControlButton controlButton)
        {
            ConfigureDataListItem configListItem = (ConfigureDataListItem)sender;
            ConfigurationBase configuration = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);
            switch (controlButton)
            {
                case ControlButton.Delete:
                    if (Messages.ShowQuestion("Are you sure you want to delete this configuration?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                        return;

                    bool isDeleted = ConfigurationBase.Delete(configuration, out Exception ex);

                    if(isDeleted)
                    {
                        containerFlowPanel.Controls.Remove(configListItem);
                        ConfigurationsManager.RemoveConfiguration(configuration);

                        if (Configurations.Count == 0)
                        {
                            if(!containerFlowPanel.Controls.Contains(emptyListLabel))
                                containerFlowPanel.Controls.Add(emptyListLabel);

                            emptyListLabel.Visible = true;
                        }
                    }
                    else
                    {
                        Messages.DeleteDirectoryException(ex);
                    }
                    break;
                case ControlButton.MoveUp:
                    break;
                case ControlButton.MoveDown:
                    break;
                case ControlButton.Configure:
                    break;
            }
        }
        #endregion

        #region Main Methods
        
        #endregion
    }
}