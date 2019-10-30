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
using Synapse.Core.Engines.Data;
using Synapse.Utilities.Enums;
using Synapse.Utilities.Attributes;

namespace Synapse.Modules
{
    public partial class AddFieldsForm : SfForm
    {
        #region Properties
        public List<CustomDataEntry> CustomDataEntries { get; private set; }
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        #region Events
        #endregion

        #region General Methods
        public AddFieldsForm(List<CustomDataEntry> customDataEntries)
        {
            InitializeComponent();
            CustomDataEntries = customDataEntries;

            Awake();
        }
        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            newFieldType.DataSource = EnumHelper.ToList(typeof(CustomDataEntryType));
            newFieldType.DisplayMember = "Value";
            newFieldType.ValueMember = "Key";

            PopulateListItems();
        }
        #endregion

        #region UI Methods
        private void addFieldBtn_Click(object sender, EventArgs e)
        {
            string fieldName = newFieldNameTextBox.Text;
            CustomDataEntryType fieldType = (CustomDataEntryType)newFieldType.SelectedIndex;

            CustomDataEntry customDataEntry = CustomDataEntry.CreateDefault(fieldName, fieldType);
            CustomDataEntries.Add(customDataEntry);

            PopulateListItems();
        }
        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (CustomDataEntries.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (int i = 0; i < CustomDataEntries.Count; i++)
            {
                CustomDataEntryListItem customDataEntryListItem = CustomDataEntryListItem.Create(CustomDataEntries[i]);
                customDataEntryListItem.OnControlButtonPressedEvent += OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(customDataEntryListItem);
                customDataEntryListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }
        private void OnConfigControlButtonPressed(object sender, CustomDataEntryListItem.ControlButton controlButton)
        {
            CustomDataEntryListItem customEntryListItem = (CustomDataEntryListItem)sender;
            switch (controlButton)
            {
                case CustomDataEntryListItem.ControlButton.Delete:
                    DeleteField(customEntryListItem);
                    break;
                case CustomDataEntryListItem.ControlButton.MoveUp:
                    MoveField(customEntryListItem, true);
                    break;
                case CustomDataEntryListItem.ControlButton.MoveDown:
                    MoveField(customEntryListItem, false);
                    break;
                case CustomDataEntryListItem.ControlButton.Configure:
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
        private void DeleteField(CustomDataEntryListItem customDataEntryListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this field?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;

            //bool isDeleted = ConfigurationBase.Delete(configuration, out Exception ex);

            //if (isDeleted)
            //{
            //    containerFlowPanel.Controls.Remove(customDataEntryListItem);
            //    ConfigurationsManager.RemoveConfiguration(customDataEntryListItem, configuration);

            //    if (CustomDataEntries.Count == 0)
            //    {
            //        if (!containerFlowPanel.Controls.Contains(emptyListLabel))
            //            containerFlowPanel.Controls.Add(emptyListLabel);

            //        emptyListLabel.Visible = true;
            //    }
            //}
            //else
            //{
            //    Messages.DeleteDirectoryException(ex);
            //}
        }
        private void MoveField(CustomDataEntryListItem configListItem, bool isUp)
        {
            //ConfigurationBase config = ConfigurationsManager.GetConfiguration(configListItem.ConfigName);
            //int curIndex = containerFlowPanel.Controls.IndexOf(configListItem);
            //int moveIndex = 0;
            //if(isUp)
            //{
            //    moveIndex = curIndex == 0? 0 : curIndex - 1;
            //}
            //else
            //{
            //    moveIndex = curIndex == CustomDataEntries.Count-1? curIndex : curIndex + 1;
            //}
            //containerFlowPanel.Controls.SetChildIndex(configListItem, moveIndex);
            //CustomDataEntries.RemoveAt(curIndex);
            //CustomDataEntries.Insert(moveIndex, config);
            //config.ProcessingIndex = moveIndex;
            //CustomDataEntries[curIndex].ProcessingIndex = curIndex;
        }
        private void ConfigureField(CustomDataEntryListItem configuration)
        {
            
        }
        #endregion
    }
}