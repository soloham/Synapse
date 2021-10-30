using static Synapse.Utilities.Enums.Basic;

namespace Synapse.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Synapse.Core.Engines.Data;
    using Synapse.Shared.Properties;

    public partial class CustomDataEntryListItem : UserControl
    {
        #region SystemHandCursorSnippet

        private const int WM_SETCURSOR = 0x0020;
        private const int IDC_HAND = 32649;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETCURSOR)
            {
                // Set the cursor to use the system hand cursor
                SetCursor(LoadCursor(IntPtr.Zero, IDC_HAND));

                // Indicate that the message has been handled
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region Enums

        public enum ControlButton
        {
            Delete,
            MoveUp,
            MoveDown,
            Configure
        }

        #endregion

        #region Properties

        #region UI

        public State CurrentState
        {
            get => currentState;
            set
            {
                this.SwitchState(value);
                this.OnStateChangedEvent?.Invoke(this, value);
            }
        }

        private State currentState = State.Normal;

        #region Foreground & Background States

        [Description("Gets or Sets the Normal Background Color.")]
        [Category("Options")]
        public Color NormalColor { get; set; } = Color.White;

        [Description("Gets or Sets the Background Color on Mouse Enter.")]
        [Category("Options")]
        public Color HighlightColor { get; set; } = Color.Linen;

        [Description("Gets or Sets the Background Color on Mouse Down.")]
        [Category("Options")]
        public Color PressedColor { get; set; } = Color.Gainsboro;

        [Description("Gets or Sets the Background Color on Mouse Down.")]
        [Category("Options")]
        public Color SelectedColor { get; set; } = Color.FromArgb(255, 22, 165, 220);

        [Description("Gets or Sets the Normal Foreground Color.")]
        [Category("Options")]
        public Color NormalForeColor { get; set; } = Color.FromArgb(255, 68, 68, 68);

        [Description("Gets or Sets the Foreground Color on Mouse Enter.")]
        [Category("Options")]
        public Color HighlightForeColor { get; set; } = Color.FromArgb(255, 68, 68, 68);

        [Description("Gets or Sets the Foreground Color on Mouse Down.")]
        [Category("Options")]
        public Color PressedForeColor { get; set; } = Color.FromArgb(255, 68, 68, 68);

        [Description("Gets or Sets the Foreground Color on Mouse Down.")]
        [Category("Options")]
        public Color SelectedForeColor { get; set; } = Color.White;

        #endregion

        [Description("Gets the File Name Type Icon.")]
        [Category("Options")]
        public Bitmap FileNameIcon { get; set; } = SharedResources.Text_Braille_WF;

        [Description("Gets the Folder Name Type Icon.")]
        [Category("Options")]
        public Bitmap FolderNameIcon { get; set; } = SharedResources.Text_Braille_WF;

        #endregion

        #region Main

        [Description("Gets or Sets the CustomDataEntryuration Name.")]
        [Category("Main Options")]
        public string FieldName
        {
            get => fieldName;
            set
            {
                fieldName = value;
                fieldNameLabel.Text = value;
            }
        }

        private string fieldName = "CustomDataEntryuration Name";

        [Description("Gets or Sets the value that represents IsSelected.")]
        [Category("Main Options")]
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                this.ToggleSelect(value);
                this.OnSelectedChangedEvent?.Invoke(this, value);
            }
        }

        private bool isSelected;

        #endregion

        #endregion

        #region Events

        public delegate void OnToggleChanged(object sender, bool state);

        public event OnToggleChanged OnSelectedChangedEvent;

        public delegate void OnStateChanged(object sender, State state);

        public event OnStateChanged OnStateChangedEvent;

        public delegate void OnControlButtonPressed(object sender, ControlButton controlButton);

        public event OnControlButtonPressed OnControlButtonPressedEvent;

        #endregion

        #region UI Methods

        private void DeleteCustomDataEntryBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Delete);
        }

        private void MoveUpCustomDataEntryBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.MoveUp);
        }

        private void MoveDownCustomDataEntryBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.MoveDown);
        }

        private void ConfigureFieldBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Configure);
        }

        private void ConfigureDataListItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.NormalColor;
                fieldNameLabel.BackColor = this.NormalColor;

                fieldNameLabel.ForeColor = this.NormalForeColor;

                this.CurrentState = State.Normal;
            }
            else
            {
                this.CurrentState = State.Selected;
            }
        }

        private void FieldNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                fieldNameLabel.BackColor = this.HighlightColor;

                fieldNameLabel.ForeColor = this.HighlightForeColor;

                this.CurrentState = State.Highlighted;
            }
            else
            {
                this.CurrentState = State.Selected;
            }
        }

        private void FieldNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.PressedColor;
                fieldNameLabel.BackColor = this.PressedColor;

                fieldNameLabel.ForeColor = this.PressedForeColor;
            }
            else
            {
                this.BackColor = this.PressedColor;
                fieldNameLabel.BackColor = this.PressedColor;

                fieldNameLabel.ForeColor = this.PressedForeColor;
            }

            this.CurrentState = State.Pressed;
        }

        private void FieldNameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                fieldNameLabel.BackColor = this.HighlightColor;

                fieldNameLabel.ForeColor = this.HighlightForeColor;

                this.CurrentState = State.Highlighted;
            }
            else
            {
                this.CurrentState = State.Selected;
            }

            this.IsSelected = !this.IsSelected;
        }

        private void ToggleSelect(bool isSelected)
        {
            this.isSelected = isSelected;

            if (isSelected)
            {
                this.BackColor = this.SelectedColor;
                fieldNameLabel.BackColor = this.SelectedColor;

                fieldNameLabel.ForeColor = this.SelectedForeColor;

                this.CurrentState = State.Selected;
            }
            else
            {
                this.BackColor = this.NormalColor;
                fieldNameLabel.BackColor = this.NormalColor;

                fieldNameLabel.ForeColor = this.NormalForeColor;

                this.CurrentState = State.Normal;
            }
        }

        private void SwitchState(State newState)
        {
            currentState = newState;

            switch (this.CurrentState)
            {
                case State.Normal:
                    break;

                case State.Selected:
                    break;
            }
        }

        private void SetCustomDataEntryType(CustomDataEntryType CustomDataEntryType)
        {
            switch (CustomDataEntryType)
            {
                case CustomDataEntryType.FileName:
                    fieldTypeIcon.Image = this.FileNameIcon;
                    break;

                case CustomDataEntryType.FolderName:
                    fieldTypeIcon.Image = this.FolderNameIcon;
                    break;
            }
        }

        #endregion

        #region Public Methods

        private CustomDataEntryListItem()
        {
            this.InitializeComponent();
        }

        public static CustomDataEntryListItem Create(CustomDataEntry CustomDataEntry)
        {
            var CustomDataEntryListItem = new CustomDataEntryListItem();
            CustomDataEntryListItem.FieldName = CustomDataEntry.FieldTitle;
            CustomDataEntryListItem.SetCustomDataEntryType(CustomDataEntry.CustomDataEntryType);

            return CustomDataEntryListItem;
        }

        #endregion
    }
}