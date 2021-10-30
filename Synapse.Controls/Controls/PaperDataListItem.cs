using static Synapse.Utilities.Enums.Basic;

namespace Synapse.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Synapse.Core;

    public partial class PaperDataListItem : UserControl
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

        #endregion

        #region Main

        [Description("Gets or Sets the Paper Code.")]
        [Category("Main Options")]
        public int PaperCode { get; set; }

        [Description("Gets or Sets the Paper Title.")]
        [Category("Main Options")]
        public string PaperTitle
        {
            get => paperTitle;
            set
            {
                paperTitle = value;
                paperTitleLabel.Text = value;
            }
        }

        private string paperTitle = "Exam Paper";

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

        private void DeleteConfigBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Delete);
        }

        private void ConfigurePaperBtn_Click(object sender, EventArgs e)
        {
            this.OnControlButtonPressedEvent?.Invoke(this, ControlButton.Configure);
        }

        private void PaperDataListItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.NormalColor;
                paperTitleLabel.BackColor = this.NormalColor;

                paperTitleLabel.ForeColor = this.NormalForeColor;

                this.CurrentState = State.Normal;
            }
            else
            {
                this.CurrentState = State.Selected;
            }
        }

        private void PaperNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                paperTitleLabel.BackColor = this.HighlightColor;

                paperTitleLabel.ForeColor = this.HighlightForeColor;

                this.CurrentState = State.Highlighted;
            }
            else
            {
                this.CurrentState = State.Selected;
            }
        }

        private void PaperNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.PressedColor;
                paperTitleLabel.BackColor = this.PressedColor;

                paperTitleLabel.ForeColor = this.PressedForeColor;
            }
            else
            {
                this.BackColor = this.PressedColor;
                paperTitleLabel.BackColor = this.PressedColor;

                paperTitleLabel.ForeColor = this.PressedForeColor;
            }

            this.CurrentState = State.Pressed;
        }

        private void PaperNameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                paperTitleLabel.BackColor = this.HighlightColor;

                paperTitleLabel.ForeColor = this.HighlightForeColor;

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
                paperTitleLabel.BackColor = this.SelectedColor;

                paperTitleLabel.ForeColor = this.SelectedForeColor;

                this.CurrentState = State.Selected;
            }
            else
            {
                this.BackColor = this.NormalColor;
                paperTitleLabel.BackColor = this.NormalColor;

                paperTitleLabel.ForeColor = this.NormalForeColor;

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

        private PaperDataListItem()
        {
            this.InitializeComponent();
        }

        public static PaperDataListItem Create(Paper paper)
        {
            var paperListItem = new PaperDataListItem();
            paperListItem.PaperTitle = paper.Title;
            paperListItem.PaperCode = paper.Code;

            return paperListItem;
        }

        #endregion
    }
}