using static Synapse.Utilities.Enums.Basic;

namespace Synapse.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Synapse.Shared.Properties;

    public partial class TemplateListItem : UserControl
    {
        [Serializable]
        public class ObjectData
        {
            public ObjectData(string templateName, bool isPinned, int listIndex = 0, string lastActiveTimeStamp = null)
            {
                this.TemplateName = templateName;
                this.IsPinned = isPinned;
                this.ListIndex = listIndex;
                this.LastActiveTimeStamp = lastActiveTimeStamp;
            }

            public string TemplateName { get; set; }
            public bool IsPinned { get; set; }
            public string LastActiveTimeStamp { get; set; }
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

        #region Pinned & Unpinned States

        [Description("Gets or Sets the Normal Unpinned Image.")]
        [Category("Options")]
        public Image NormalUnpinnedImage { get; set; } = SharedResources.Unpin;

        [Description("Gets or Sets the Normal Pinned Image.")]
        [Category("Options")]
        public Image NormalPinnedImage { get; set; } = SharedResources.Pin;

        [Description("Gets or Sets the Normal Pin Button Mouse Over Color.")]
        [Category("Options")]
        public Color NormalPinMouseOverColor { get; set; } = Color.LightGray;

        [Description("Gets or Sets the Normal Pin Button Mouse Down Color.")]
        [Category("Options")]
        public Color NormalPinMouseDownColor { get; set; } = Color.Gainsboro;

        [Description("Gets or Sets the Unpinned Image on Mouse Enter.")]
        [Category("Options")]
        public Image HighlightUnpinnedImage { get; set; } = SharedResources.Unpin;

        [Description("Gets or Sets the Pinned Image on Mouse Enter.")]
        [Category("Options")]
        public Image HighlightPinnedImage { get; set; } = SharedResources.Pin;

        [Description("Gets or Sets the Pressed Unpinned Image.")]
        [Category("Options")]
        public Image PressedUnpinnedImage { get; set; } = SharedResources.Unpin;

        [Description("Gets or Sets the Pressed Pinned Image.")]
        [Category("Options")]
        public Image PressedPinnedImage { get; set; } = SharedResources.Pin;

        [Description("Gets or Sets the Selected Unpinned Image.")]
        [Category("Options")]
        public Image SelectedUnpinnedImage { get; set; } = SharedResources.Unpin_White;

        [Description("Gets or Sets the Selected Pinned Image.")]
        [Category("Options")]
        public Image SelectedPinnedImage { get; set; } = SharedResources.Pin_White;

        [Description("Gets or Sets the Selected Pin Button Mouse Over Color.")]
        [Category("Options")]
        public Color SelectedPinMouseOverColor { get; set; } = Color.SkyBlue;

        [Description("Gets or Sets the Selected Pin Button Mouse Down Color.")]
        [Category("Options")]
        public Color SelectedPinMouseDownColor { get; set; } = Color.LightBlue;

        #endregion

        #endregion

        #region Main

        [Description("Gets or Sets the Template Name.")]
        [Category("Main Options")]
        public string TemplateName
        {
            get => templateName;
            set
            {
                templateName = value;
                templateNameLabel.Text = value;
            }
        }

        private string templateName = "Template Name";

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

        [Description("Gets or Sets the value that represents IsPinned.")]
        [Category("Main Options")]
        public bool IsPinned
        {
            get => isPinned;
            set
            {
                isPinned = value;
                this.OnPinnedChangedEvent?.Invoke(this, value);
                this.TogglePinned(value);
            }
        }

        private bool isPinned;

        [Description("Gets or Sets the Template Name.")]
        [Category("Main Options")]
        public string LastActiveTimeStamp
        {
            get => lastActiveTimeStamp;
            set
            {
                lastActiveTimeStamp = value;
                lastActiveLabel.Text = value;
            }
        }

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
                this.BackColor = this.NormalColor;
                templateNameLabel.BackColor = this.NormalColor;
                lastActiveLabel.BackColor = this.NormalColor;
                pinToggleBtn.BackColor = this.NormalColor;

                templateNameLabel.ForeColor = this.NormalForeColor;
                lastActiveLabel.ForeColor = this.NormalForeColor;
                pinToggleBtn.ForeColor = this.NormalForeColor;

                this.CurrentState = State.Normal;
            }
            else
            {
                this.CurrentState = State.Selected;
            }

            if (!isPinned && !this.ClientRectangle.Contains(this.PointToClient(MousePosition)))
            {
                pinToggleBtn.Visible = false;
            }
        }

        private void TemplateNameLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                templateNameLabel.BackColor = this.HighlightColor;
                lastActiveLabel.BackColor = this.HighlightColor;
                pinToggleBtn.BackColor = this.HighlightColor;

                templateNameLabel.ForeColor = this.HighlightForeColor;
                lastActiveLabel.ForeColor = this.HighlightForeColor;
                pinToggleBtn.ForeColor = this.HighlightForeColor;

                this.CurrentState = State.Highlighted;
            }
            else
            {
                this.CurrentState = State.Selected;
            }

            if (!isPinned)
            {
                pinToggleBtn.Visible = true;
            }
        }

        private void TemplateNameLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.PressedColor;
                templateNameLabel.BackColor = this.PressedColor;
                lastActiveLabel.BackColor = this.PressedColor;
                pinToggleBtn.BackColor = this.PressedColor;

                templateNameLabel.ForeColor = this.PressedForeColor;
                lastActiveLabel.ForeColor = this.PressedForeColor;
                pinToggleBtn.ForeColor = this.PressedForeColor;
            }
            else
            {
                this.BackColor = this.PressedColor;
                templateNameLabel.BackColor = this.PressedColor;
                lastActiveLabel.BackColor = this.PressedColor;
                pinToggleBtn.BackColor = this.PressedColor;

                templateNameLabel.ForeColor = this.PressedForeColor;
                lastActiveLabel.ForeColor = this.PressedForeColor;
                pinToggleBtn.ForeColor = this.PressedForeColor;
            }

            this.CurrentState = State.Pressed;
        }

        private void TemplateNameLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = this.HighlightColor;
                templateNameLabel.BackColor = this.HighlightColor;
                lastActiveLabel.BackColor = this.HighlightColor;
                pinToggleBtn.BackColor = this.HighlightColor;

                templateNameLabel.ForeColor = this.HighlightForeColor;
                lastActiveLabel.ForeColor = this.HighlightForeColor;
                pinToggleBtn.ForeColor = this.HighlightForeColor;

                this.CurrentState = State.Highlighted;
            }
            else
            {
                this.CurrentState = State.Selected;
            }

            this.IsSelected = !this.IsSelected;
        }

        private void PinToggleBtn_Click(object sender, EventArgs e)
        {
            this.IsPinned = !this.IsPinned;
        }

        private void ToggleSelect(bool isSelected)
        {
            this.isSelected = isSelected;

            if (isSelected)
            {
                this.BackColor = this.SelectedColor;
                templateNameLabel.BackColor = this.SelectedColor;
                lastActiveLabel.BackColor = this.SelectedColor;
                pinToggleBtn.BackColor = this.SelectedColor;

                templateNameLabel.ForeColor = this.SelectedForeColor;
                lastActiveLabel.ForeColor = this.SelectedForeColor;
                pinToggleBtn.ForeColor = this.SelectedForeColor;

                this.CurrentState = State.Selected;
            }
            else
            {
                this.BackColor = this.NormalColor;
                templateNameLabel.BackColor = this.NormalColor;
                lastActiveLabel.BackColor = this.NormalColor;
                pinToggleBtn.BackColor = this.NormalColor;

                templateNameLabel.ForeColor = this.NormalForeColor;
                lastActiveLabel.ForeColor = this.NormalForeColor;
                pinToggleBtn.ForeColor = this.NormalForeColor;

                this.CurrentState = State.Normal;
            }
        }

        private void TogglePinned(bool isPinned)
        {
            this.SwitchState(this.CurrentState);
            if (isPinned)
            {
                pinToggleBtn.Visible = true;
            }
            else
            {
                pinToggleBtn.Visible = false;
            }
        }

        private void SwitchState(State newState)
        {
            currentState = newState;

            switch (this.CurrentState)
            {
                case State.Normal:
                    pinToggleBtn.FlatAppearance.MouseOverBackColor = this.NormalPinMouseOverColor;
                    pinToggleBtn.FlatAppearance.MouseDownBackColor = this.NormalPinMouseDownColor;
                    break;

                case State.Selected:
                    pinToggleBtn.FlatAppearance.MouseOverBackColor = this.SelectedPinMouseOverColor;
                    pinToggleBtn.FlatAppearance.MouseDownBackColor = this.SelectedPinMouseDownColor;
                    break;
            }

            if (isPinned)
            {
                switch (this.CurrentState)
                {
                    case State.Normal:
                        pinToggleBtn.Image = this.NormalPinnedImage;
                        break;

                    case State.Highlighted:
                        pinToggleBtn.Image = this.HighlightPinnedImage;
                        break;

                    case State.Pressed:
                        pinToggleBtn.Image = this.PressedPinnedImage;
                        break;

                    case State.Selected:
                        pinToggleBtn.Image = this.SelectedPinnedImage;
                        break;
                }
            }
            else
            {
                switch (this.CurrentState)
                {
                    case State.Normal:
                        pinToggleBtn.Image = this.NormalUnpinnedImage;
                        break;

                    case State.Highlighted:
                        pinToggleBtn.Image = this.HighlightUnpinnedImage;
                        break;

                    case State.Pressed:
                        pinToggleBtn.Image = this.PressedUnpinnedImage;
                        break;

                    case State.Selected:
                        pinToggleBtn.Image = this.SelectedUnpinnedImage;
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        private TemplateListItem()
        {
            this.InitializeComponent();
        }

        public static TemplateListItem Create(string templateName)
        {
            var templateListItem = new TemplateListItem();
            templateListItem.TemplateName = templateName;
            templateListItem.LastActiveTimeStamp = DateTime.Now.ToString();

            return templateListItem;
        }

        public static TemplateListItem Create(ObjectData objectData)
        {
            var templateListItem = new TemplateListItem();
            templateListItem.TemplateName = objectData.TemplateName;
            templateListItem.IsPinned = objectData.IsPinned;
            templateListItem.LastActiveTimeStamp = objectData.LastActiveTimeStamp;

            return templateListItem;
        }

        public ObjectData GetObjectData()
        {
            return new ObjectData(this.TemplateName, isPinned, 0, this.LastActiveTimeStamp);
        }

        #endregion
    }
}