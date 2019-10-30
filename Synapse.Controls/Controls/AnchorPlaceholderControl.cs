using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Synapse.Core.Templates.Template.AnchorAlignmentMethod;
using Emgu.CV;

namespace Synapse.Controls
{
    public partial class AnchorPlaceholderControl : UserControl
    {
        #region Properties
        public Color AscentColor { get => ascentColor; set { ascentColor = value; OnAscentColorChangedCallback(value); } }
        private Color ascentColor = Color.Crimson;

        public Anchor GetAnchor { get => anchor; }
        private Anchor anchor;
        public int Index { get; set; }
        public bool IsCurrent { get => isCurrent; set => ToggleIsCurrent(value); }
        private bool isCurrent;
        public bool IsInitialized { get; set; }
        #endregion

        #region Actions
        private Action<AnchorPlaceholderControl> deleteAnchorAction;
        #endregion

        #region Variables
        #endregion

        #region Methods
        #region Public
        public AnchorPlaceholderControl()
        {
            InitializeComponent();
        }

        public void Initialize(RectangleF anchorRegion, Mat anchorImage, Action<AnchorPlaceholderControl> deleteAnchorAction)
        {
            anchor = new Anchor(anchorRegion, anchorImage);
            anchorImagePBox.Image = anchorImage.Bitmap;
            deleteAnchorBtn.Enabled = true;

            this.deleteAnchorAction = deleteAnchorAction;

            IsInitialized = true;
        }
        public void Reset()
        {
            anchor = null;
            anchorImagePBox.Image = null;
            deleteAnchorBtn.Enabled = false;

            IsInitialized = false;
        }
        #endregion

        #region Private
        private void DeleteAnchorBtn_Click(object sender, EventArgs e)
        {
            deleteAnchorAction(this);
        }

        private void ToggleIsCurrent(bool isCurrent)
        {
            this.isCurrent = isCurrent;

            if(isCurrent)
                BorderStyle = BorderStyle.FixedSingle;
            else
                BorderStyle = BorderStyle.None;
        }

        private void OnAscentColorChangedCallback(Color color)
        {
            deleteAnchorBtn.BackColor = color;
        }
        #endregion
        #endregion
    }
}
