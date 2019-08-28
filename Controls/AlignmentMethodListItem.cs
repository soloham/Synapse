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
using static Synapse.Core.Templates.Template;

namespace Synapse.Controls
{
    public partial class AlignmentMethodListItem : UserControl
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
        [Description("Gets the Method List Item Name."), Category("Options")]
        public string AlignmentMethodName { get { return alignmentMethodName; } set { alignmentMethodName = value; methodNameLabel.Text = value; } }
        private string alignmentMethodName = "Alignment Method";
        [Description("Gets the Registration Method Type Icon."), Category("Options")]
        public Bitmap RegistrationMethodIcon { get { return registrationMethodIcon; } set { registrationMethodIcon = value; } }
        private Bitmap registrationMethodIcon = Properties.Resources._3D_shape__01_WF;
        [Description("Gets the Anchor Method Type Icon."), Category("Options")]
        public Bitmap AnchorMethodIcon { get { return anchorMethodIcon; } set { anchorMethodIcon = value; } }
        private Bitmap anchorMethodIcon = Properties.Resources.Anchor_01;
        #endregion
        #region Main
        [Description("Gets or Sets the Alignmen tMethod Type."), Category("Main Options")]
        internal AlignmentMethodType AlignmentMethodType { get { return alignmentMethodType;  } set { alignmentMethodType = value; SetConfigType(value); } }
        private AlignmentMethodType alignmentMethodType;
        [Description("Gets or Sets the Alignment Method Name."), Category("Main Options")]
        public string MethodName { get { return methodName; } set { methodName = value; methodNameLabel.Text = value; } }
        private string methodName = "Alignment Method Name";
        [Description("Gets or Sets the value that represents IsSelected."), Category("Main Options")]
        public bool IsSelected { get { return isSelected; } set { ToggleSelect(value); OnSelectedChangedEvent?.Invoke(this, value); } }
        private bool isSelected = false;
        #endregion
        #endregion

        #region Variables
        public int listIndex = 0;
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
        private void DeleteMethodBtn_Click(object sender, EventArgs e)
        {
            OnControlButtonPressedEvent?.Invoke(this, ControlButton.Delete);
        }
        private void MoveUpMethodBtn_Click(object sender, EventArgs e)
        {
            OnControlButtonPressedEvent?.Invoke(this, ControlButton.MoveUp);
        }
        private void MoveDownMethodBtn_Click(object sender, EventArgs e)
        {
            OnControlButtonPressedEvent?.Invoke(this, ControlButton.MoveDown);
        }
        private void ConfigureMethodBtn_Click(object sender, EventArgs e)
        {
            OnControlButtonPressedEvent?.Invoke(this, ControlButton.Configure);
        }

        private void MethodListItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                BackColor = NormalColor;
                methodNameLabel.BackColor = NormalColor;

                methodNameLabel.ForeColor = NormalForeColor;

                CurrentState = State.Normal;
            }
            else
            {
                CurrentState = State.Selected;
            }

        }
        private void MethodNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                BackColor = HighlightColor;
                methodNameLabel.BackColor = HighlightColor;

                methodNameLabel.ForeColor = HighlightForeColor;

                CurrentState = State.Highlighted;
            }
            else
            {
                CurrentState = State.Selected;
            }
        }
        private void MethodNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                BackColor = PressedColor;
                methodNameLabel.BackColor = PressedColor;

                methodNameLabel.ForeColor = PressedForeColor;

            }
            else
            {
                BackColor = PressedColor;
                methodNameLabel.BackColor = PressedColor;

                methodNameLabel.ForeColor = PressedForeColor;
            }

            CurrentState = State.Pressed;
        }
        private void MethodNameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                BackColor = HighlightColor;
                methodNameLabel.BackColor = HighlightColor;

                methodNameLabel.ForeColor = HighlightForeColor;

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
                methodNameLabel.BackColor = SelectedColor;

                methodNameLabel.ForeColor = SelectedForeColor;

                CurrentState = State.Selected;
            }
            else
            {
                BackColor = NormalColor;
                methodNameLabel.BackColor = NormalColor;

                methodNameLabel.ForeColor = NormalForeColor;

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
        void SetConfigType(AlignmentMethodType methodType)
        {
            switch (methodType)
            {
                case AlignmentMethodType.Registration:
                    methodTypeIcon.Image = RegistrationMethodIcon;
                    break;
                case AlignmentMethodType.Anchors:
                    methodTypeIcon.Image = AnchorMethodIcon;
                    break;
            }
        }
        #endregion

        #region Public Methods
        private AlignmentMethodListItem()
        {
            InitializeComponent();
        }
        internal static AlignmentMethodListItem Create(AlignmentMethod method)
        {
            AlignmentMethodListItem alignmentMethodListItem = new AlignmentMethodListItem();
            alignmentMethodListItem.MethodName = method.MethodName;
            alignmentMethodListItem.AlignmentMethodType = method.GetAlignmentMethodType;
            alignmentMethodListItem.listIndex = method.PipelineIndex;

            return alignmentMethodListItem;
        }
        #endregion
    }
}
