using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static Synapse.Utilities.Enums.Basic;
using Synapse.Core.Configurations;
using Synapse.Core;

namespace Synapse.Controls
{
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
        public State CurrentState { get { return currentState; } set { SwitchState(value); OnStateChangedEvent?.Invoke(this, value); } }
        private State currentState = State.Normal;
        #region Foreground & Background States
        [Description("Gets or Sets the Normal Background Color."), Category("Options")]
        public Color NormalColor { get { return normalColor; } set { normalColor = value; } }
        private Color normalColor = Color.White;
        [Description("Gets or Sets the Background Color on Mouse Enter."), Category("Options")]
        public Color HighlightColor { get { return highlightColor; } set { highlightColor = value; } }
        private Color highlightColor = Color.Linen;
        [Description("Gets or Sets the Background Color on Mouse Down."), Category("Options")]
        public Color PressedColor { get { return pressedColor; } set { pressedColor = value; } }
        private Color pressedColor = Color.Gainsboro;
        [Description("Gets or Sets the Background Color on Mouse Down."), Category("Options")]
        public Color SelectedColor { get { return selectedColor; } set { selectedColor = value; } }
        private Color selectedColor = Color.FromArgb(255, 22, 165, 220);
        [Description("Gets or Sets the Normal Foreground Color."), Category("Options")]
        public Color NormalForeColor { get { return normalForeColor; } set { normalForeColor = value; } }
        private Color normalForeColor = Color.FromArgb(255, 68, 68, 68);
        [Description("Gets or Sets the Foreground Color on Mouse Enter."), Category("Options")]
        public Color HighlightForeColor { get { return highlightForeColor; } set { highlightForeColor = value; } }
        private Color highlightForeColor = Color.FromArgb(255, 68, 68, 68);
        [Description("Gets or Sets the Foreground Color on Mouse Down."), Category("Options")]
        public Color PressedForeColor { get { return pressedForeColor; } set { pressedForeColor = value; } }
        private Color pressedForeColor = Color.FromArgb(255, 68, 68, 68);
        [Description("Gets or Sets the Foreground Color on Mouse Down."), Category("Options")]
        public Color SelectedForeColor { get { return selectedForeColor; } set { selectedForeColor = value; } }
        private Color selectedForeColor = Color.White;
        #endregion
        #endregion
        #region Main
        [Description("Gets or Sets the Paper Code."), Category("Main Options")]
        public int PaperCode { get { return paperCode;  } set { paperCode = value; } }
        private int paperCode;
        [Description("Gets or Sets the Paper Title."), Category("Main Options")]
        public string PaperTitle { get { return paperTitle; } set { paperTitle = value; paperTitleLabel.Text = value; } }
        private string paperTitle = "Exam Paper";
        [Description("Gets or Sets the value that represents IsSelected."), Category("Main Options")]
        public bool IsSelected { get { return isSelected; } set { ToggleSelect(value); OnSelectedChangedEvent?.Invoke(this, value); } }
        private bool isSelected = false;
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
            OnControlButtonPressedEvent?.Invoke(this, ControlButton.Delete);
        }
        private void ConfigurePaperBtn_Click(object sender, EventArgs e)
        {
            OnControlButtonPressedEvent?.Invoke(this, ControlButton.Configure);
        }

        private void PaperDataListItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                BackColor = NormalColor;
                paperTitleLabel.BackColor = NormalColor;

                paperTitleLabel.ForeColor = NormalForeColor;

                CurrentState = State.Normal;
            }
            else
            {
                CurrentState = State.Selected;
            }

        }
        private void PaperNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                BackColor = HighlightColor;
                paperTitleLabel.BackColor = HighlightColor;

                paperTitleLabel.ForeColor = HighlightForeColor;

                CurrentState = State.Highlighted;
            }
            else
            {
                CurrentState = State.Selected;
            }
        }
        private void PaperNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                BackColor = PressedColor;
                paperTitleLabel.BackColor = PressedColor;

                paperTitleLabel.ForeColor = PressedForeColor;

            }
            else
            {
                BackColor = PressedColor;
                paperTitleLabel.BackColor = PressedColor;

                paperTitleLabel.ForeColor = PressedForeColor;
            }

            CurrentState = State.Pressed;
        }
        private void PaperNameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                BackColor = HighlightColor;
                paperTitleLabel.BackColor = HighlightColor;

                paperTitleLabel.ForeColor = HighlightForeColor;

                CurrentState = State.Highlighted;
            }
            else
            {
                CurrentState = State.Selected;
            }
            IsSelected = !IsSelected;
        }
        void ToggleSelect(bool isSelected)
        {
            this.isSelected = isSelected;

            if (isSelected)
            {
                BackColor = SelectedColor;
                paperTitleLabel.BackColor = SelectedColor;

                paperTitleLabel.ForeColor = SelectedForeColor;

                CurrentState = State.Selected;
            }
            else
            {
                BackColor = NormalColor;
                paperTitleLabel.BackColor = NormalColor;

                paperTitleLabel.ForeColor = NormalForeColor;

                CurrentState = State.Normal;
            }
        }
        void SwitchState(State newState)
        {
            currentState = newState;

            switch (CurrentState)
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
            InitializeComponent();
        }
        public static PaperDataListItem Create(Paper paper)
        {
            PaperDataListItem paperListItem = new PaperDataListItem();
            paperListItem.PaperTitle = paper.Title;
            paperListItem.PaperCode = paper.Code;

            return paperListItem;
        }
        #endregion
    }
}
