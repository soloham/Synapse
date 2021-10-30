using static Synapse.Core.Templates.Template;
using static Synapse.Controls.AlignmentMethodListItem;

namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Emgu.CV;

    using Synapse.Controls;
    using Synapse.Utilities;
    using Synapse.Utilities.Enums;

    using Syncfusion.WinForms.Controls;

    public partial class AlignmentPipelineConfigurationForm : SfForm
    {
        #region Properties

        public List<AlignmentMethod> AlignmentMethods { get; set; }

        public bool NewMethodToggle
        {
            get => newMethodToggle;
            set
            {
                newMethodToggle = value;
                this.ToggleCreateMethod(value);
            }
        }

        private bool newMethodToggle;

        public bool IsNewMethodNameValid
        {
            get => isNewMethodNameValid;
            set
            {
                isNewMethodNameValid = value;
                this.ToggleNewNameValid(value);
            }
        }

        private bool isNewMethodNameValid;

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;
        private readonly Mat templateImage;
        private string newMethodName;

        #endregion

        #region Events

        public delegate void OnConfigurationFinshed(List<AlignmentMethod> alignmentMethods);

        public event OnConfigurationFinshed OnConfigurationFinishedEvent;

        public event EventHandler OnFormInitializedEvent;

        #endregion

        #region General Methods

        public AlignmentPipelineConfigurationForm(List<AlignmentMethod> alignmentMethods, Mat templateImage)
        {
            this.InitializeComponent();
            this.templateImage = templateImage;
            this.AlignmentMethods = alignmentMethods;

            this.Awake();
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            comboBoxStateComboBox.DataSource = EnumHelper.ToList(typeof(AlignmentMethodType));
            comboBoxStateComboBox.DisplayMember = "Value";
            comboBoxStateComboBox.ValueMember = "Key";

            this.NewMethodToggle = false;

            this.PopulateListItems();
        }

        #endregion

        #region UI Methods

        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (this.AlignmentMethods.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (var i = 0; i < this.AlignmentMethods.Count; i++)
            {
                var alignmentMethodListItem = Create(this.AlignmentMethods[i]);
                alignmentMethodListItem.OnControlButtonPressedEvent += this.OnConfigControlButtonPressed;
                containerFlowPanel.Controls.Add(alignmentMethodListItem);
                alignmentMethodListItem.Size =
                    new Size(containerFlowPanel.Size.Width, alignmentMethodListItem.Size.Height);
                emptyListLabel.Visible = false;
            }
        }

        private void OnConfigControlButtonPressed(object sender, ControlButton controlButton)
        {
            var alignmentMethodListItem = (AlignmentMethodListItem)sender;
            switch (controlButton)
            {
                case ControlButton.Delete:
                    this.DeleteMethod(alignmentMethodListItem);
                    break;

                case ControlButton.MoveUp:
                    this.MoveMethod(alignmentMethodListItem, true);
                    break;

                case ControlButton.MoveDown:
                    this.MoveMethod(alignmentMethodListItem, false);
                    break;

                case ControlButton.Configure:
                    var alignmentMethod = this.AlignmentMethods[alignmentMethodListItem.listIndex];
                    this.ConfigureMethod(alignmentMethod);
                    break;
            }
        }

        private void AddNewMethodBtn_Click(object sender, EventArgs e)
        {
            this.NewMethodToggle = !this.NewMethodToggle;
        }

        private void AddMethodBtn_Click(object sender, EventArgs e)
        {
            newMethodName = createMethodNameTextBox.Text;
            if (!this.ValidateNewMethodName(newMethodName))
            {
                return;
            }

            var alignmentMethodType = (AlignmentMethodType)comboBoxStateComboBox.SelectedIndex;
            switch (alignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    var anchorAlignmentMethodTool =
                        new AnchorAlignmentMethodForm(templateImage, this.AlignmentMethods.Count, newMethodName);
                    anchorAlignmentMethodTool.OnConfigurationFinishedEvent += anchorAlignmentMethod =>
                    {
                        this.AddMethod(anchorAlignmentMethod);
                        anchorAlignmentMethodTool.Close();

                        this.NewMethodToggle = false;
                    };
                    anchorAlignmentMethodTool.ShowDialog();
                    break;

                case AlignmentMethodType.Registration:
                    var registrationAlignmentMethodForm =
                        new RegistrationAlignmentMethodForm(templateImage, this.AlignmentMethods.Count, newMethodName);
                    registrationAlignmentMethodForm.OnConfigurationFinishedEvent += registrationAlignmentMethod =>
                    {
                        this.AddMethod(registrationAlignmentMethod);
                        registrationAlignmentMethodForm.Close();

                        this.NewMethodToggle = false;
                    };
                    registrationAlignmentMethodForm.ShowDialog();
                    break;
            }
        }

        #endregion

        #region Main Methods

        private void DeleteMethod(AlignmentMethodListItem methodListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this method?", "Hold On",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }

            var method = this.AlignmentMethods[methodListItem.listIndex];

            var isDeleted = this.AlignmentMethods.Remove(method);

            if (isDeleted)
            {
                containerFlowPanel.Controls.Remove(methodListItem);

                if (this.AlignmentMethods.Count == 0)
                {
                    if (!containerFlowPanel.Controls.Contains(emptyListLabel))
                    {
                        containerFlowPanel.Controls.Add(emptyListLabel);
                    }

                    emptyListLabel.Visible = true;
                }
            }

            for (var i = 0; i < this.AlignmentMethods.Count; i++)
            {
                this.AlignmentMethods[i].PipelineIndex = i;
                var alignmentMethodListItem = (AlignmentMethodListItem)containerFlowPanel.Controls[i];
                alignmentMethodListItem.listIndex = i;
            }
        }

        private void MoveMethod(AlignmentMethodListItem methodListItem, bool isUp)
        {
            var curIndex = methodListItem.listIndex;
            var method = this.AlignmentMethods[curIndex];
            var moveIndex = 0;
            if (isUp)
            {
                moveIndex = curIndex == 0 ? 0 : curIndex - 1;
            }
            else
            {
                moveIndex = curIndex == this.AlignmentMethods.Count - 1 ? curIndex : curIndex + 1;
            }

            containerFlowPanel.Controls.SetChildIndex(methodListItem, moveIndex);
            this.AlignmentMethods.RemoveAt(curIndex);
            this.AlignmentMethods.Insert(moveIndex, method);
            method.PipelineIndex = moveIndex;
            this.AlignmentMethods[curIndex].PipelineIndex = curIndex;
            methodListItem.listIndex = moveIndex;
        }

        private void ConfigureMethod(AlignmentMethod alignmentMethod)
        {
            switch (alignmentMethod.GetAlignmentMethodType)
            {
                case AlignmentMethodType.Anchors:
                    var anchorAlignmentMethodTool =
                        new AnchorAlignmentMethodForm((AnchorAlignmentMethod)alignmentMethod, templateImage);
                    anchorAlignmentMethodTool.OnConfigurationFinishedEvent += anchorAlignmentMethod =>
                    {
                        this.AlignmentMethods[alignmentMethod.PipelineIndex] = anchorAlignmentMethod;
                        anchorAlignmentMethodTool.Close();
                    };
                    anchorAlignmentMethodTool.ShowDialog();
                    break;

                case AlignmentMethodType.Registration:
                    var registrationAlignmentMethodForm =
                        new RegistrationAlignmentMethodForm((RegistrationAlignmentMethod)alignmentMethod,
                            templateImage);
                    registrationAlignmentMethodForm.OnConfigurationFinishedEvent += registrationAlignmentMethod =>
                    {
                        this.AlignmentMethods[alignmentMethod.PipelineIndex] = registrationAlignmentMethod;
                        registrationAlignmentMethodForm.Close();
                    };
                    registrationAlignmentMethodForm.ShowDialog();
                    break;
            }
        }

        private void AddMethod(AlignmentMethod alignmentMethod)
        {
            var alignmentMethodListItem = Create(alignmentMethod);
            alignmentMethodListItem.OnControlButtonPressedEvent += this.OnConfigControlButtonPressed;
            containerFlowPanel.Controls.Add(alignmentMethodListItem);
            alignmentMethodListItem.Size = new Size(containerFlowPanel.Size.Width, alignmentMethodListItem.Size.Height);
            emptyListLabel.Visible = false;

            this.AlignmentMethods.Add(alignmentMethod);
        }

        private void ToggleCreateMethod(bool state)
        {
            if (state)
            {
                for (var i = 0; i < mainTablePanel.RowCount; i++)
                {
                    mainTablePanel.RowStyles[i].SizeType = i == 0 ? SizeType.Absolute :
                        i == 1 ? SizeType.Percent :
                        i == 2 ? SizeType.Absolute : SizeType.AutoSize;
                    mainTablePanel.RowStyles[i].Height = i == 0 ? 40 : i == 1 ? 100 : i == 2 ? 60 : 0;
                }
            }
            else
            {
                for (var i = 0; i < mainTablePanel.RowCount; i++)
                {
                    mainTablePanel.RowStyles[i].SizeType = SizeType.Percent;
                    mainTablePanel.RowStyles[i].Height = i == 0 ? 0 : i == 1 ? 80 : i == 2 ? 20 : 0;
                }
            }

            ComboBoxStatePanel.Visible = state;
        }

        private void ToggleNewNameValid(bool isValid)
        {
            if (!isValid)
            {
                createMethodNameTextBox.ForeColor = Color.Crimson;
            }
            else
            {
                createMethodNameTextBox.ForeColor = Color.FromArgb(128, 68, 68, 68);
            }
        }

        private bool ValidateNewMethodName(string name)
        {
            if (string.IsNullOrEmpty(name) || name[0] == ' ' || name[name.Length - 1] == ' ' ||
                this.AlignmentMethods.Exists(x => x.MethodName == name))
            {
                this.IsNewMethodNameValid = false;
            }
            else
            {
                this.IsNewMethodNameValid = true;
            }

            return this.IsNewMethodNameValid;
        }

        private void FinishBtn_Click(object sender, EventArgs e)
        {
            this.OnConfigurationFinishedEvent?.Invoke(this.AlignmentMethods);
        }

        private void AlignmentPipelineConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        #endregion
    }
}