namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Synapse.Controls;
    using Synapse.Core.Configurations;
    using Synapse.Core.Engines.Data;
    using Synapse.Core.Managers;
    using Synapse.Utilities;
    using Synapse.Utilities.Enums;

    using Syncfusion.WinForms.Controls;

    public partial class AddFieldsForm : SfForm
    {
        #region Properties

        public List<CustomDataEntry> CustomDataEntries { get; }

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;

        #endregion

        #region Events

        #endregion

        #region General Methods

        public AddFieldsForm(List<CustomDataEntry> customDataEntries)
        {
            this.InitializeComponent();
            this.CustomDataEntries = customDataEntries;

            this.Awake();
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            newFieldType.DataSource = EnumHelper.ToList(typeof(CustomDataEntryType));
            newFieldType.DisplayMember = "Value";
            newFieldType.ValueMember = "Key";

            this.PopulateListItems();
        }

        #endregion

        #region UI Methods

        private void addFieldBtn_Click(object sender, EventArgs e)
        {
            var fieldName = newFieldNameTextBox.Text;
            var fieldType = (CustomDataEntryType)newFieldType.SelectedIndex;

            var customDataEntry = CustomDataEntry.CreateDefault(fieldName, fieldType);
            this.CustomDataEntries.Add(customDataEntry);

            this.PopulateListItems();
        }

        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (this.CustomDataEntries.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (var i = 0; i < this.CustomDataEntries.Count; i++)
            {
                var customDataEntryListItem = CustomDataEntryListItem.Create(this.CustomDataEntries[i]);
                customDataEntryListItem.OnControlButtonPressedEvent += this.OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(customDataEntryListItem);
                customDataEntryListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }

        private void OnConfigControlButtonPressed(object sender, CustomDataEntryListItem.ControlButton controlButton)
        {
            var customEntryListItem = (CustomDataEntryListItem)sender;
            switch (controlButton)
            {
                case CustomDataEntryListItem.ControlButton.Delete:
                    this.DeleteField(customEntryListItem);
                    break;

                case CustomDataEntryListItem.ControlButton.MoveUp:
                    this.MoveField(customEntryListItem, true);
                    break;

                case CustomDataEntryListItem.ControlButton.MoveDown:
                    this.MoveField(customEntryListItem, false);
                    break;

                case CustomDataEntryListItem.ControlButton.Configure:
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

        private void DeleteField(CustomDataEntryListItem customDataEntryListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this field?", "Hold On", MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation) == DialogResult.No)
            {
            }

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