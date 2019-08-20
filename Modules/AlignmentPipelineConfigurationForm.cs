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
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using static Synapse.Core.Templates.Template;
using static Synapse.Controls.AlignmentMethodListItem;
using Synapse.Utilities.Enums;

namespace Synapse.Modules
{
    public partial class AlignmentPipelineConfigurationForm : SfForm
    {
        #region Properties
        internal List<AlignmentMethod> AlignmentMethods { get; set; }
        public bool NewMethodToggle { get { return newMethodToggle; } set { newMethodToggle = value; ToggleCreateMethod(value); } }
        private bool newMethodToggle;
        #endregion

        #region Variables
        private SynchronizationContext synchronizationContext;
        #endregion

        #region Events
        #endregion

        #region General Methods
        internal AlignmentPipelineConfigurationForm(List<AlignmentMethod> alignmentMethods)
        {
            InitializeComponent();
            AlignmentMethods = alignmentMethods;

            Awake();
        }
        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(AlignmentMethodType));
            comboBoxStateComboBox.DisplayMember = "Value";
            comboBoxStateComboBox.ValueMember = "Key";

            NewMethodToggle = false;

            PopulateListItems();
        }
        #endregion

        #region UI Methods
        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (AlignmentMethods.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (int i = 0; i < AlignmentMethods.Count; i++)
            {
                AlignmentMethodListItem alignmentMethodListItem = AlignmentMethodListItem.Create(AlignmentMethods[i]);
                alignmentMethodListItem.OnControlButtonPressedEvent += OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(alignmentMethodListItem);
                alignmentMethodListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }
        private void OnConfigControlButtonPressed(object sender, ControlButton controlButton)
        {
            AlignmentMethodListItem alignmentMethodListItem = (AlignmentMethodListItem)sender;
            switch (controlButton)
            {
                case ControlButton.Delete:
                    DeleteMethod(alignmentMethodListItem);
                    break;
                case ControlButton.MoveUp:
                    MoveMethod(alignmentMethodListItem, true);
                    break;
                case ControlButton.MoveDown:
                    MoveMethod(alignmentMethodListItem, false);
                    break;
                case ControlButton.Configure:
                    AlignmentMethod alignmentMethod = AlignmentMethods[containerFlowPanel.Controls.IndexOf(alignmentMethodListItem)];
                    ConfigureMethod(alignmentMethod);
                    break;
            }
        }

        private void AddNewMethodBtn_Click(object sender, EventArgs e)
        {
            NewMethodToggle = !NewMethodToggle;
        }
        private void AddMethodBtn_Click(object sender, EventArgs e)
        {
            AlignmentMethodType alignmentMethodType = (AlignmentMethodType)comboBoxStateComboBox.SelectedIndex;
            switch (alignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    break;
                case AlignmentMethodType.Registration:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Main Methods
        private void DeleteMethod(AlignmentMethodListItem methodListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this method?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;

            AlignmentMethod method = AlignmentMethods[containerFlowPanel.Controls.IndexOf(methodListItem)];

            bool isDeleted = AlignmentMethods.Remove(method);

            if (isDeleted)
            {
                containerFlowPanel.Controls.Remove(methodListItem);

                if (AlignmentMethods.Count == 0)
                {
                    if (!containerFlowPanel.Controls.Contains(emptyListLabel))
                        containerFlowPanel.Controls.Add(emptyListLabel);

                    emptyListLabel.Visible = true;
                }
            }
        }
        private void MoveMethod(AlignmentMethodListItem methodListItem, bool isUp)
        {
            int curIndex = containerFlowPanel.Controls.IndexOf(methodListItem);
            AlignmentMethod method = AlignmentMethods[curIndex];
            int moveIndex = 0;
            if(isUp)
            {
                moveIndex = curIndex == 0? 0 : curIndex - 1;
            }
            else
            {
                moveIndex = curIndex == AlignmentMethods.Count-1? curIndex : curIndex + 1;
            }
            containerFlowPanel.Controls.SetChildIndex(methodListItem, moveIndex);
            AlignmentMethods.RemoveAt(curIndex);
            AlignmentMethods.Insert(moveIndex, method);
            method.PipelineIndex = moveIndex;
            AlignmentMethods[curIndex].PipelineIndex = curIndex;
        }
        private void ConfigureMethod(AlignmentMethod configuration)
        {
            switch (configuration.GetAlignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    break;
                case AlignmentMethodType.Registration:
                    break;
            }
            
        }

        private void ToggleCreateMethod(bool state)
        {
            if(state == true)
            {
                for (int i = 0; i < mainTablePanel.RowCount; i++)
                {
                    mainTablePanel.RowStyles[i].SizeType = i == 0? SizeType.Absolute : i == 1? SizeType.Percent : i == 2? SizeType.Absolute : SizeType.AutoSize;
                    mainTablePanel.RowStyles[i].Height = i == 0 ? 40 : i == 1 ? 100 : i == 2 ? 60 : 0;
                }
            }
            else
            {
                for (int i = 0; i < mainTablePanel.RowCount; i++)
                {
                    mainTablePanel.RowStyles[i].SizeType = SizeType.Percent;
                    mainTablePanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 80 : i == 2 ? 20 : 0;
                }
            }

            ComboBoxStatePanel.Visible = state;
        }
        #endregion
    }
}