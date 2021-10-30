namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Synapse.Core.Configurations;
    using Synapse.Core.Managers;
    using Synapse.Core.Templates;
    using Synapse.Utilities;
    using Synapse.Utilities.Objects;

    using Syncfusion.Windows.Forms.Tools;
    using Syncfusion.WinForms.Controls;

    using Action = System.Action;

    public partial class ConfigurationsTestForm : SfForm
    {
        #region Properties

        public ProcessingManager MainProcessingManager { get; set; }
        public List<ConfigurationBase> Configurations { get; set; }
        public Template Template;

        #endregion

        #region Variables

        private SynchronizationContext synchronizationContext;
        private SheetsList loadedSheetsData = new SheetsList();

        private readonly List<RectangleF> highlightedRects = new List<RectangleF>();
        private RectangleF curHighlightedRect = RectangleF.Empty;
        private Color curHighlightedRectColor;
        private readonly Color primaryRectColor = Color.FromArgb(120, Color.DarkSlateGray);
        private readonly Color secondaryRectColor = Color.FromArgb(200, Color.MediumTurquoise);

        #endregion

        #region Events

        #endregion

        #region General Methods

        public ConfigurationsTestForm(List<ConfigurationBase> configurations)
        {
            this.InitializeComponent();
            this.Configurations = configurations;
            Template = SynapseMain.GetCurrentTemplate;
            this.MainProcessingManager = new ProcessingManager(SynapseMain.IsMainDashing, Template, null, null);

            this.Awake();
        }

        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            speedTrackBar.MinimumSize = new Size(169, 30);
            synchronizationContext.Post(state =>
            {
                //statusLabel.Text = "Scan directory containing sheets to begin...";
            }, null);
        }

        #endregion

        #region UI Methods

        private async void ScanDirBtn_ClickAsync(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedFolder = folderBrowserDialog.SelectedPath;
                var includeSubDirs = includeSubFoldersToggleBtn.ToggleState == ToggleButtonState.Active;

                await this.InitializeSheetsToRead(selectedFolder, includeSubDirs);
                if (loadedSheetsData.SheetsLoaded)
                {
                    statusLabel.Text = "Scan Directory Loaded, Successfully";
                }
                else
                {
                    statusLabel.Text = "Directory To Scan Failed To Load";
                }
            }
        }

        private async void StartReadingBtn_Click(object sender, EventArgs e)
        {
            if (!loadedSheetsData.SheetsLoaded)
            {
                Messages.ShowError(
                    "Unable to start procesing as there are no sheets loaded. \n\n Please scan a directory to load sheets in order to start processing.");
                return;
            }

            var totalSheets = loadedSheetsData.GetSheetsPath.Length;
            this.MainProcessingManager.LoadSheets(loadedSheetsData);
            Func<double> GetWaitMS = () => { return (double)speedTrackBar.Value / 3; };
            Func<bool> GetAlignSheet = () => { return sheetAlignmentToggle.ToggleState == ToggleButtonState.Active; };
            Action<Bitmap> OnSheetAligned = alignedSheet =>
            {
                synchronizationContext.Send(delegate
                {
                    highlightedRects.Clear();
                    imageBox.Image = alignedSheet;
                    if (this.MainProcessingManager.GetCurProcessingIndex == 0)
                    {
                        imageBox.ZoomToFit();
                    }

                    var curIndex = this.MainProcessingManager.GetCurProcessingIndex;
                    statusLabel.Text = $"[{curIndex + 1}/{totalSheets}] Processing...";
                }, null);
            };
            Action<RectangleF, bool> OnOptionProcessed = (optionRect, isFilled) =>
            {
                curHighlightedRect = optionRect;
                curHighlightedRectColor = isFilled ? secondaryRectColor : primaryRectColor;
                //outputValueLabel.Text = "Output: " + (isFilled ? "Marked" : "Unmarked");
                if (isFilled)
                {
                    highlightedRects.Add(curHighlightedRect);
                }

                imageBox.Invalidate();
            };
            Action<string> OnRegionProcessed = regionOutput =>
            {
                synchronizationContext.Send(delegate { outputValueLabel.Text = "Output: " + regionOutput; }, null);
            };
            Action OnProcessFinished = () =>
            {
                synchronizationContext.Send(
                    delegate { statusLabel.Text = $"Finished Processing {totalSheets} Sheets"; }, null);
            };
            await this.MainProcessingManager.StartProcessingRaw(GetAlignSheet, OnSheetAligned, OnOptionProcessed,
                OnRegionProcessed, GetWaitMS, OnProcessFinished);
        }

        private void UpdateTestStatus(string status)
        {
            statusLabel.Text = status;
        }


        #region  ImageBoxPanel Setup

        private void ImageBox_Paint(object sender, PaintEventArgs e)
        {
            if (imageBox.Image != null)
            {
                if (curHighlightedRect != RectangleF.Empty)
                {
                    Functions.DrawBox(e.Graphics, imageBox.GetOffsetRectangle(curHighlightedRect), imageBox.ZoomFactor,
                        curHighlightedRectColor, 1);
                }

                if (highlightedRects.Count > 0)
                {
                    for (var i = 0; i < highlightedRects.Count; i++)
                        Functions.DrawBox(e.Graphics, imageBox.GetOffsetRectangle(highlightedRects[i]),
                            imageBox.ZoomFactor, secondaryRectColor, 1);
                }
            }
        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
        {
            selectionToolStripStatusLabel.Text = Functions.FormatRectangle(imageBox.SelectionRegion);
        }

        private void imageBox_Selected(object sender, EventArgs e)
        {
        }

        private void ImageBox_SelectionResized(object sender, EventArgs e)
        {
        }

        private void actualSizeToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ActualSize();
        }

        private void selectAllToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.SelectAll();
        }

        private void selectNoneToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.SelectNone();
        }

        private void showImageRegionToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.Invalidate();
        }

        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomIn();
        }

        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomOut();
        }

        #endregion

        #endregion

        #region Main Methods

        public async Task<bool> InitializeSheetsToRead(string path, bool incSubDirs)
        {
            var error = "";
            var success = false;

            loadedSheetsData = new SheetsList(this.UpdateTestStatus);
            success = await Task.Run(() => loadedSheetsData.Scan(path, incSubDirs, out error));

            //err = error;
            return success;
        }

        #endregion
    }
}