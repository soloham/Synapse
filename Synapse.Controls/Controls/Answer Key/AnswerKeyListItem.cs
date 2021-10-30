using static Synapse.Utilities.Enums.Basic;

namespace Synapse.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Synapse.Core.Keys;

    using Syncfusion.Windows.Forms.Tools;

    public partial class AnswerKeyListItem : UserControl
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
            Configure,
            Active,
            Inactive
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

        #endregion

        #region Main

        [Description("Gets or Sets the Key Title.")]
        [Category("Main Options")]
        public string KeyTitle
        {
            get => keyTitle;
            set
            {
                keyTitle = value;
                keyTitleLabel.Text = value;
            }
        }

        private string keyTitle = "Key Title";

        [Description("Gets or Sets the Configuration Title.")]
        [Category("Main Options")]
        public string ConfigTitle { get; set; }

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

        private void keyToggleBtn_ToggleStateChanged(object sender, ToggleStateChangedEventArgs e)
        {
            if (e.ToggleState == ToggleButtonState.Active)
            {
                this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Active);
            }
            else
            {
                this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Inactive);
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Delete);
        }

        private void ConfigureBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Configure);
        }

        private void AnswerKeyListItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.NormalColor;
                keyTitleLabel.BackColor = this.NormalColor;

                keyTitleLabel.ForeColor = this.NormalForeColor;

                this.CurrentState = State.Normal;
            }
            else
            {
                this.CurrentState = State.Selected;
            }
        }

        private void AnswerTitleLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                keyTitleLabel.BackColor = this.HighlightColor;

                keyTitleLabel.ForeColor = this.HighlightForeColor;

                this.CurrentState = State.Highlighted;
            }
            else
            {
                this.CurrentState = State.Selected;
            }
        }

        private void AnswerTitleLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.PressedColor;
                keyTitleLabel.BackColor = this.PressedColor;

                keyTitleLabel.ForeColor = this.PressedForeColor;
            }
            else
            {
                this.BackColor = this.PressedColor;
                keyTitleLabel.BackColor = this.PressedColor;

                keyTitleLabel.ForeColor = this.PressedForeColor;
            }

            this.CurrentState = State.Pressed;
        }

        private void AnswerKeyTitleLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                keyTitleLabel.BackColor = this.HighlightColor;

                keyTitleLabel.ForeColor = this.HighlightForeColor;

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
                keyTitleLabel.BackColor = this.SelectedColor;

                keyTitleLabel.ForeColor = this.SelectedForeColor;

                this.CurrentState = State.Selected;
            }
            else
            {
                this.BackColor = this.NormalColor;
                keyTitleLabel.BackColor = this.NormalColor;

                keyTitleLabel.ForeColor = this.NormalForeColor;

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

        #endregion

        #region Public Methods

        private AnswerKeyListItem(bool isActive)
        {
            this.InitializeComponent();
            keyToggleBtn.ToggleState = isActive ? ToggleButtonState.Active : ToggleButtonState.Inactive;
        }

        public static AnswerKeyListItem Create(string configTitle, AnswerKey answerKey,
            OnControlButtonPressed OnControllButoonPressed)
        {
            var keyListItem = new AnswerKeyListItem(answerKey.IsActive);
            keyListItem.KeyTitle = answerKey.Title;
            keyListItem.ConfigTitle = configTitle;
            keyListItem.OnControlButtonPressedEvent += OnControllButoonPressed;


            return keyListItem;
        }

        #endregion
    }
}