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

namespace Synapse.Controls
{
    public partial class TemplateListItem : UserControl
    {
        [Serializable]
        public class ObjectData
        {
            public ObjectData(string templateName, bool isPinned, int listIndex = 0)
            {
                TemplateName = templateName;
                IsPinned = isPinned;
                ListIndex = listIndex;
            }

            public string TemplateName { get; set; }
            public bool IsPinned { get; set; }
            public int ListIndex { get; set; }
        }

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
        #region Pinned & Unpinned States
        [Description("Gets or Sets the Normal Unpinned Image."), Category("Options")]
        public Image NormalUnpinnedImage { get { return normalUnpinnedImage; } set { normalUnpinnedImage = value; } }
        private Image normalUnpinnedImage = Shared.Properties.SharedResources.Unpin;
        [Description("Gets or Sets the Normal Pinned Image."), Category("Options")]
        public Image NormalPinnedImage { get { return normalPinnedImage; } set { normalPinnedImage = value; } }
        private Image normalPinnedImage = Shared.Properties.SharedResources.Pin;
        [Description("Gets or Sets the Normal Pin Button Mouse Over Color."), Category("Options")]
        public Color NormalPinMouseOverColor { get { return normalPinMouseOverColor; } set { normalPinMouseOverColor = value; } }
        private Color normalPinMouseOverColor = Color.LightGray;
        [Description("Gets or Sets the Normal Pin Button Mouse Down Color."), Category("Options")]
        public Color NormalPinMouseDownColor { get { return normalPinMouseDownColor; } set { normalPinMouseDownColor = value; } }
        private Color normalPinMouseDownColor = Color.Gainsboro;
        [Description("Gets or Sets the Unpinned Image on Mouse Enter."), Category("Options")]
        public Image HighlightUnpinnedImage { get { return highlightUnpinnedImage; } set { highlightUnpinnedImage = value; } }
        private Image highlightUnpinnedImage = Shared.Properties.SharedResources.Unpin;
        [Description("Gets or Sets the Pinned Image on Mouse Enter."), Category("Options")]
        public Image HighlightPinnedImage { get { return highlighPinnedImage; } set { highlighPinnedImage = value; } }
        private Image highlighPinnedImage = Shared.Properties.SharedResources.Pin;
        [Description("Gets or Sets the Pressed Unpinned Image."), Category("Options")]
        public Image PressedUnpinnedImage { get { return pressedUnpinnedImage; } set { pressedUnpinnedImage = value; } }
        private Image pressedUnpinnedImage = Shared.Properties.SharedResources.Unpin;
        [Description("Gets or Sets the Pressed Pinned Image."), Category("Options")]
        public Image PressedPinnedImage { get { return pressedPinnedImage; } set { pressedPinnedImage = value; } }
        private Image pressedPinnedImage = Shared.Properties.SharedResources.Pin;
        [Description("Gets or Sets the Selected Unpinned Image."), Category("Options")]
        public Image SelectedUnpinnedImage { get { return selectedUnpinnedImage; } set { selectedUnpinnedImage = value; } }
        private Image selectedUnpinnedImage = Shared.Properties.SharedResources.Unpin_White;
        [Description("Gets or Sets the Selected Pinned Image."), Category("Options")]
        public Image SelectedPinnedImage { get { return selectedPinnedImage; } set { selectedPinnedImage = value; } }
        private Image selectedPinnedImage = Shared.Properties.SharedResources.Pin_White;
        [Description("Gets or Sets the Selected Pin Button Mouse Over Color."), Category("Options")]
        public Color SelectedPinMouseOverColor { get { return selectedPinMouseOverColor; } set { selectedPinMouseOverColor = value; } }
        private Color selectedPinMouseOverColor = Color.SkyBlue;
        [Description("Gets or Sets the Selected Pin Button Mouse Down Color."), Category("Options")]
        public Color SelectedPinMouseDownColor { get { return selectedPinMouseDownColor; } set { selectedPinMouseDownColor = value; } }
        private Color selectedPinMouseDownColor = Color.LightBlue;
        #endregion
        #endregion
        #region Main
        [Description("Gets or Sets the Template Name."), Category("Main Options")]
        public string TemplateName { get { return templateName; } set { templateName = value; templateNameLabel.Text = value; } }
        private string templateName = "Template Name";
        [Description("Gets or Sets the value that represents IsSelected."), Category("Main Options")]
        public bool IsSelected { get { return isSelected; } set { ToggleSelect(value); OnSelectedChangedEvent?.Invoke(this, value); } }
        private bool isSelected = false;
        [Description("Gets or Sets the value that represents IsPinned."), Category("Main Options")]
        public bool IsPinned { get { return isPinned; } set { isPinned = value; OnPinnedChangedEvent?.Invoke(this, value); TogglePinned(value); } }
        private bool isPinned = false;
        [Description("Gets or Sets the Template Name."), Category("Main Options")]
        public string LastActiveTimeStamp { get { return lastActiveTimeStamp; } set { lastActiveTimeStamp = value; lastActiveLabel.Text = value; } }
        private string lastActiveTimeStamp = "1/1/2000 24:00";
        #endregion
        #endregion

        #region Events
        public delegate void OnToggleChanged(object sender, bool state);
        public event OnToggleChanged OnSelectedChangedEvent;
        public event OnToggleChanged OnPinnedChangedEvent;

        public delegate void OnStateChanged(object sender, State state);
        public event OnStateChanged OnStateChangedEvent;
        #endregion

        #region UI Methods
        private void TemplateListItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                BackColor = NormalColor;
                templateNameLabel.BackColor = NormalColor;
                lastActiveLabel.BackColor = NormalColor;
                pinToggleBtn.BackColor = NormalColor;

                templateNameLabel.ForeColor = NormalForeColor;
                lastActiveLabel.ForeColor = NormalForeColor;
                pinToggleBtn.ForeColor = NormalForeColor;

                CurrentState = State.Normal;
            }
            else
            {
                CurrentState = State.Selected;
            }

            if (!isPinned && !ClientRectangle.Contains(PointToClient(MousePosition)))
                pinToggleBtn.Visible = false;
        }
        private void TemplateNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                BackColor = HighlightColor;
                templateNameLabel.BackColor = HighlightColor;
                lastActiveLabel.BackColor = HighlightColor;
                pinToggleBtn.BackColor = HighlightColor;

                templateNameLabel.ForeColor = HighlightForeColor;
                lastActiveLabel.ForeColor = HighlightForeColor;
                pinToggleBtn.ForeColor = HighlightForeColor;

                CurrentState = State.Highlighted;
            }
            else
            {
                CurrentState = State.Selected;
            }

            if (!isPinned)
                pinToggleBtn.Visible = true;
        }
        private void TemplateNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                BackColor = PressedColor;
                templateNameLabel.BackColor = PressedColor;
                lastActiveLabel.BackColor = PressedColor;
                pinToggleBtn.BackColor = PressedColor;

                templateNameLabel.ForeColor = PressedForeColor;
                lastActiveLabel.ForeColor = PressedForeColor;
                pinToggleBtn.ForeColor = PressedForeColor;

            }
            else
            {
                BackColor = PressedColor;
                templateNameLabel.BackColor = PressedColor;
                lastActiveLabel.BackColor = PressedColor;
                pinToggleBtn.BackColor = PressedColor;

                templateNameLabel.ForeColor = PressedForeColor;
                lastActiveLabel.ForeColor = PressedForeColor;
                pinToggleBtn.ForeColor = PressedForeColor;
            }

            CurrentState = State.Pressed;
        }
        private void TemplateNameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                BackColor = HighlightColor;
                templateNameLabel.BackColor = HighlightColor;
                lastActiveLabel.BackColor = HighlightColor;
                pinToggleBtn.BackColor = HighlightColor;

                templateNameLabel.ForeColor = HighlightForeColor;
                lastActiveLabel.ForeColor = HighlightForeColor;
                pinToggleBtn.ForeColor = HighlightForeColor;

                CurrentState = State.Highlighted;
            }
            else
            {
                CurrentState = State.Selected;
            }
            IsSelected = !IsSelected;
        }
        private void PinToggleBtn_Click(object sender, EventArgs e)
        {
            IsPinned = !IsPinned;
        }
        void ToggleSelect(bool isSelected)
        {
            this.isSelected = isSelected;

            if (isSelected)
            {
                BackColor = SelectedColor;
                templateNameLabel.BackColor = SelectedColor;
                lastActiveLabel.BackColor = SelectedColor;
                pinToggleBtn.BackColor = SelectedColor;

                templateNameLabel.ForeColor = SelectedForeColor;
                lastActiveLabel.ForeColor = SelectedForeColor;
                pinToggleBtn.ForeColor = SelectedForeColor;

                CurrentState = State.Selected;
            }
            else
            {
                BackColor = NormalColor;
                templateNameLabel.BackColor = NormalColor;
                lastActiveLabel.BackColor = NormalColor;
                pinToggleBtn.BackColor = NormalColor;

                templateNameLabel.ForeColor = NormalForeColor;
                lastActiveLabel.ForeColor = NormalForeColor;
                pinToggleBtn.ForeColor = NormalForeColor;

                CurrentState = State.Normal;
            }
        }
        void TogglePinned(bool isPinned)
        {
            SwitchState(CurrentState);
            if (isPinned)
            {
                pinToggleBtn.Visible = true;
            }
            else
            {
                pinToggleBtn.Visible = false;
            }
        }
        void SwitchState(State newState)
        {
            currentState = newState;

            switch (CurrentState)
            {
                case State.Normal:
                    pinToggleBtn.FlatAppearance.MouseOverBackColor = NormalPinMouseOverColor;
                    pinToggleBtn.FlatAppearance.MouseDownBackColor = NormalPinMouseDownColor;
                    break;
                case State.Selected:
                    pinToggleBtn.FlatAppearance.MouseOverBackColor = SelectedPinMouseOverColor;
                    pinToggleBtn.FlatAppearance.MouseDownBackColor = SelectedPinMouseDownColor;
                    break;
                default:
                    break;
            }

            if (isPinned)
            {
                switch (CurrentState)
                {
                    case State.Normal:
                        pinToggleBtn.Image = NormalPinnedImage;
                        break;
                    case State.Highlighted:
                        pinToggleBtn.Image = HighlightPinnedImage;
                        break;
                    case State.Pressed:
                        pinToggleBtn.Image = PressedPinnedImage;
                        break;
                    case State.Selected:
                        pinToggleBtn.Image = SelectedPinnedImage;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (CurrentState)
                {
                    case State.Normal:
                        pinToggleBtn.Image = NormalUnpinnedImage;
                        break;
                    case State.Highlighted:
                        pinToggleBtn.Image = HighlightUnpinnedImage;
                        break;
                    case State.Pressed:
                        pinToggleBtn.Image = PressedUnpinnedImage;
                        break;
                    case State.Selected:
                        pinToggleBtn.Image = SelectedUnpinnedImage;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Public Methods
        private TemplateListItem()
        {
            InitializeComponent();
        }
        public static TemplateListItem Create(string templateName)
        {
            TemplateListItem templateListItem = new TemplateListItem();
            templateListItem.TemplateName = templateName;

            return templateListItem;
        }
        public static TemplateListItem Create(ObjectData objectData)
        {
            TemplateListItem templateListItem = new TemplateListItem();
            templateListItem.TemplateName = objectData.TemplateName;
            templateListItem.IsPinned = objectData.IsPinned;

            return templateListItem;
        }
        public ObjectData GetObjectData()
        {
            return new ObjectData(TemplateName, isPinned);
        }
        #endregion
    }

}
