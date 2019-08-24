namespace Synapse.Controls
{
    partial class AlignmentMethodResultControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlignmentMethodResultControl));
            this.differenceImageBoxPanel = new System.Windows.Forms.Panel();
            this.differenceImageBox = new Cyotek.Windows.Forms.Demo.ImageBoxEx();
            this.originalImageBoxPanel = new System.Windows.Forms.Panel();
            this.originalImageBox = new Cyotek.Windows.Forms.Demo.ImageBoxEx();
            this.resultImageBoxPanel = new System.Windows.Forms.Panel();
            this.resultImageBox = new Cyotek.Windows.Forms.Demo.ImageBoxEx();
            this.resultSummaryContainerPanel = new System.Windows.Forms.Panel();
            this.resultsDockingManager = new Syncfusion.Windows.Forms.Tools.DockingManager(this.components);
            this.differenceImageBoxPanel.SuspendLayout();
            this.originalImageBoxPanel.SuspendLayout();
            this.resultImageBoxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDockingManager)).BeginInit();
            this.SuspendLayout();
            // 
            // differenceImageBoxPanel
            // 
            this.differenceImageBoxPanel.Controls.Add(this.differenceImageBox);
            this.differenceImageBoxPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.differenceImageBoxPanel.Location = new System.Drawing.Point(303, 0);
            this.differenceImageBoxPanel.Name = "differenceImageBoxPanel";
            this.differenceImageBoxPanel.Size = new System.Drawing.Size(55, 380);
            this.differenceImageBoxPanel.TabIndex = 5;
            // 
            // differenceImageBox
            // 
            this.differenceImageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.differenceImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.differenceImageBox.DragHandleSize = 10;
            this.differenceImageBox.DropShadowSize = 5;
            this.differenceImageBox.Font = new System.Drawing.Font("Dosis", 14F);
            this.differenceImageBox.GridColor = System.Drawing.Color.White;
            this.differenceImageBox.ImageBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.differenceImageBox.ImageBorderStyle = Cyotek.Windows.Forms.ImageBoxBorderStyle.FixedSingleGlowShadow;
            this.differenceImageBox.Location = new System.Drawing.Point(0, 0);
            this.differenceImageBox.Margin = new System.Windows.Forms.Padding(0);
            this.differenceImageBox.Name = "differenceImageBox";
            this.differenceImageBox.SelectionMode = Cyotek.Windows.Forms.ImageBoxSelectionMode.Rectangle;
            this.differenceImageBox.Size = new System.Drawing.Size(55, 380);
            this.differenceImageBox.StepSize = new System.Drawing.Size(8, 8);
            this.differenceImageBox.TabIndex = 4;
            this.differenceImageBox.Text = "The image obtained from the difference of the output image from the input image w" +
    "ill appear here.";
            // 
            // originalImageBoxPanel
            // 
            this.originalImageBoxPanel.Controls.Add(this.originalImageBox);
            this.originalImageBoxPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.originalImageBoxPanel.Location = new System.Drawing.Point(358, 0);
            this.originalImageBoxPanel.Name = "originalImageBoxPanel";
            this.originalImageBoxPanel.Size = new System.Drawing.Size(55, 380);
            this.originalImageBoxPanel.TabIndex = 4;
            // 
            // originalImageBox
            // 
            this.originalImageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.originalImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originalImageBox.DragHandleSize = 10;
            this.originalImageBox.DropShadowSize = 5;
            this.originalImageBox.Font = new System.Drawing.Font("Dosis", 14F);
            this.originalImageBox.GridColor = System.Drawing.Color.White;
            this.originalImageBox.ImageBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.originalImageBox.ImageBorderStyle = Cyotek.Windows.Forms.ImageBoxBorderStyle.FixedSingleGlowShadow;
            this.originalImageBox.Location = new System.Drawing.Point(0, 0);
            this.originalImageBox.Margin = new System.Windows.Forms.Padding(0);
            this.originalImageBox.Name = "originalImageBox";
            this.originalImageBox.SelectionMode = Cyotek.Windows.Forms.ImageBoxSelectionMode.Rectangle;
            this.originalImageBox.Size = new System.Drawing.Size(55, 380);
            this.originalImageBox.StepSize = new System.Drawing.Size(8, 8);
            this.originalImageBox.TabIndex = 4;
            this.originalImageBox.Text = "The input/original image will appear here.";
            // 
            // resultImageBoxPanel
            // 
            this.resultImageBoxPanel.Controls.Add(this.resultImageBox);
            this.resultImageBoxPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.resultImageBoxPanel.Location = new System.Drawing.Point(413, 0);
            this.resultImageBoxPanel.Name = "resultImageBoxPanel";
            this.resultImageBoxPanel.Size = new System.Drawing.Size(37, 380);
            this.resultImageBoxPanel.TabIndex = 3;
            // 
            // resultImageBox
            // 
            this.resultImageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.resultImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultImageBox.DragHandleSize = 10;
            this.resultImageBox.DropShadowSize = 5;
            this.resultImageBox.Font = new System.Drawing.Font("Dosis", 14F);
            this.resultImageBox.GridColor = System.Drawing.Color.White;
            this.resultImageBox.ImageBorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.resultImageBox.ImageBorderStyle = Cyotek.Windows.Forms.ImageBoxBorderStyle.FixedSingleGlowShadow;
            this.resultImageBox.Location = new System.Drawing.Point(0, 0);
            this.resultImageBox.Margin = new System.Windows.Forms.Padding(0);
            this.resultImageBox.Name = "resultImageBox";
            this.resultImageBox.SelectionMode = Cyotek.Windows.Forms.ImageBoxSelectionMode.Rectangle;
            this.resultImageBox.Size = new System.Drawing.Size(37, 380);
            this.resultImageBox.StepSize = new System.Drawing.Size(8, 8);
            this.resultImageBox.TabIndex = 4;
            this.resultImageBox.Text = "The image obtained from the test will appear here.";
            // 
            // resultSummaryContainerPanel
            // 
            this.resultSummaryContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultSummaryContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.resultSummaryContainerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.resultSummaryContainerPanel.Name = "resultSummaryContainerPanel";
            this.resultSummaryContainerPanel.Size = new System.Drawing.Size(303, 380);
            this.resultSummaryContainerPanel.TabIndex = 6;
            // 
            // resultsDockingManager
            // 
            this.resultsDockingManager.ActiveCaptionFont = new System.Drawing.Font("Dosis", 11.2F);
            this.resultsDockingManager.AnimateAutoHiddenWindow = true;
            this.resultsDockingManager.AutoHideTabFont = new System.Drawing.Font("Dosis", 11.2F);
            this.resultsDockingManager.AutoHideTabForeColor = System.Drawing.Color.Empty;
            this.resultsDockingManager.BorderColor = System.Drawing.Color.Transparent;
            this.resultsDockingManager.BrowsingKey = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.resultsDockingManager.CloseEnabled = false;
            this.resultsDockingManager.CloseTabOnMiddleClick = false;
            this.resultsDockingManager.DockBehavior = Syncfusion.Windows.Forms.Tools.DockBehavior.VS2010;
            this.resultsDockingManager.DockedCaptionFont = new System.Drawing.Font("Dosis", 11.2F);
            this.resultsDockingManager.DockLayoutStream = ((System.IO.MemoryStream)(resources.GetObject("resultsDockingManager.DockLayoutStream")));
            this.resultsDockingManager.DockTabAlignment = Syncfusion.Windows.Forms.Tools.DockTabAlignmentStyle.Top;
            this.resultsDockingManager.DockTabFont = new System.Drawing.Font("Dosis", 11.2F);
            this.resultsDockingManager.DragProviderStyle = Syncfusion.Windows.Forms.Tools.DragProviderStyle.Office2016Colorful;
            this.resultsDockingManager.HostControl = this;
            this.resultsDockingManager.InActiveCaptionBackground = new Syncfusion.Drawing.BrushInfo(System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(211)))), ((int)(((byte)(212))))));
            this.resultsDockingManager.InActiveCaptionFont = new System.Drawing.Font("Dosis", 11.2F);
            this.resultsDockingManager.MetroButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.resultsDockingManager.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.resultsDockingManager.MetroSplitterBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(159)))), ((int)(((byte)(183)))));
            this.resultsDockingManager.PaintBorders = false;
            this.resultsDockingManager.ReduceFlickeringInRtl = false;
            this.resultsDockingManager.ShowDockTabScrollButton = true;
            this.resultsDockingManager.ShowMetroCaptionDottedLines = false;
            this.resultsDockingManager.ThemeName = "Metro";
            this.resultsDockingManager.ThemeStyle.SplitterColor = System.Drawing.Color.White;
            this.resultsDockingManager.VisualStyle = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.resultsDockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Close, "CloseButton"));
            this.resultsDockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Pin, "PinButton"));
            this.resultsDockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Maximize, "MaximizeButton"));
            this.resultsDockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Restore, "RestoreButton"));
            this.resultsDockingManager.CaptionButtons.Add(new Syncfusion.Windows.Forms.Tools.CaptionButton(Syncfusion.Windows.Forms.Tools.CaptionButtonType.Menu, "MenuButton"));
            // 
            // AlignmentMethodResultControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.resultSummaryContainerPanel);
            this.Controls.Add(this.differenceImageBoxPanel);
            this.Controls.Add(this.originalImageBoxPanel);
            this.Controls.Add(this.resultImageBoxPanel);
            this.Name = "AlignmentMethodResultControl";
            this.Size = new System.Drawing.Size(450, 380);
            this.differenceImageBoxPanel.ResumeLayout(false);
            this.originalImageBoxPanel.ResumeLayout(false);
            this.resultImageBoxPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultsDockingManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel differenceImageBoxPanel;
        private Cyotek.Windows.Forms.Demo.ImageBoxEx differenceImageBox;
        private System.Windows.Forms.Panel originalImageBoxPanel;
        private Cyotek.Windows.Forms.Demo.ImageBoxEx originalImageBox;
        private System.Windows.Forms.Panel resultImageBoxPanel;
        private Cyotek.Windows.Forms.Demo.ImageBoxEx resultImageBox;
        private System.Windows.Forms.Panel resultSummaryContainerPanel;
        private Syncfusion.Windows.Forms.Tools.DockingManager resultsDockingManager;
    }
}
