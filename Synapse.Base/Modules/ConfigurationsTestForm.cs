using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Synapse.Core.Configurations;
using Synapse.Core.Managers;
using Synapse.Core.Templates;
using Syncfusion.WinForms.Controls;
using System.Threading;
using System.Windows.Threading;
using Synapse.Utilities.Objects;
using System.Threading.Tasks;
using Synapse.Utilities;

namespace Synapse.Modules
{
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

        List<RectangleF> highlightedRects = new List<RectangleF>();
        RectangleF curHighlightedRect = RectangleF.Empty;
        Color curHighlightedRectColor;
        Color primaryRectColor = Color.FromArgb(120, Color.DarkSlateGray);
        Color secondaryRectColor = Color.FromArgb(200, Color.MediumTurquoise);
        #endregion

        #region Events
        #endregion

        #region General Methods
        public ConfigurationsTestForm(List<ConfigurationBase> configurations)
        {
            InitializeComponent();
            Configurations = configurations;
            Template = SynapseMain.GetCurrentTemplate;
            MainProcessingManager = new ProcessingManager(SynapseMain.IsMainDashing, Template, null, null);

            Awake();
        }
        private void Awake()
        {
            synchronizationContext = SynchronizationContext.Current;

            speedTrackBar.MinimumSize = new Size(169, 30);
            statusLabel.Text = "Scan directory containing sheets to begin...";
        }
        #endregion

        #region UI Methods
        private async void ScanDirBtn_ClickAsync(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                bool includeSubDirs = includeSubFoldersToggleBtn.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;

                await InitializeSheetsToRead(selectedFolder, includeSubDirs);
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
                Messages.ShowError("Unable to start procesing as there are no sheets loaded. \n\n Please scan a directory to load sheets in order to start processing.");
                return;
            }

            int totalSheets = loadedSheetsData.GetSheetsPath.Length;
            MainProcessingManager.LoadSheets(loadedSheetsData);
            Func<double> GetWaitMS = () => { return (double)speedTrackBar.Value / 3; };
            Func<bool> GetAlignSheet = () => { return sheetAlignmentToggle.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active; };
            Action<Bitmap> OnSheetAligned = (Bitmap alignedSheet) => {
                synchronizationContext.Send(new SendOrPostCallback(
                delegate (object state)
                {
                    highlightedRects.Clear();
                    imageBox.Image = alignedSheet;
                    if(MainProcessingManager.GetCurProcessingIndex == 0)
                        imageBox.ZoomToFit();
                    int curIndex = MainProcessingManager.GetCurProcessingIndex;
                    statusLabel.Text = $"[{curIndex+1}/{totalSheets}] Processing...";
                }), null);
            };
            Action<RectangleF, bool> OnOptionProcessed = (RectangleF optionRect, bool isFilled) => {
                curHighlightedRect = optionRect;
                curHighlightedRectColor = isFilled ? secondaryRectColor : primaryRectColor;
                //outputValueLabel.Text = "Output: " + (isFilled ? "Marked" : "Unmarked");
                if (isFilled) highlightedRects.Add(curHighlightedRect); imageBox.Invalidate();
            };
            Action<string> OnRegionProcessed = (string regionOutput) => {
                synchronizationContext.Send(new SendOrPostCallback(
                delegate (object state)
                {
                    outputValueLabel.Text = "Output: " + regionOutput;
                }), null);
            };
            Action OnProcessFinished = () =>
            {
                synchronizationContext.Send(new SendOrPostCallback(
                delegate (object state)
                {
                    statusLabel.Text = $"Finished Processing {totalSheets} Sheets";
                }), null);
            };
            await MainProcessingManager.StartProcessingRaw(GetAlignSheet, OnSheetAligned, OnOptionProcessed, OnRegionProcessed, GetWaitMS, OnProcessFinished);
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
                    Functions.DrawBox(e.Graphics, imageBox.GetOffsetRectangle(curHighlightedRect), imageBox.ZoomFactor, curHighlightedRectColor, 1);

                if (highlightedRects.Count > 0)
                {
                    for (int i = 0; i < highlightedRects.Count; i++)
                    {
                        Functions.DrawBox(e.Graphics, imageBox.GetOffsetRectangle(highlightedRects[i]), imageBox.ZoomFactor, secondaryRectColor, 1);
                    }
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
            selectionToolStripStatusLabel.Text = Utilities.Functions.FormatRectangle(imageBox.SelectionRegion);
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
            string error = "";
            bool success = false;

            loadedSheetsData = new SheetsList(UpdateTestStatus);
            success = await Task.Run(() => loadedSheetsData.Scan(path, incSubDirs, out error));

            //err = error;
            return success;
        }

        #endregion
    }
}