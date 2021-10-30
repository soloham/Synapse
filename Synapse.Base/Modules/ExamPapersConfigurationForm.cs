namespace Synapse.Modules
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Synapse.Controls;
    using Synapse.Core;
    using Synapse.Core.Configurations;
    using Synapse.Core.Managers;
    using Synapse.Utilities;
    using Synapse.Utilities.Enums;
    using Synapse.Utilities.Memory;

    using Syncfusion.WinForms.Controls;

    public partial class ExamPapersConfigurationForm : SfForm
    {
        #region Properties

        public ExamPapers ExamPaper { get; set; }

        #endregion

        #region Variables

        private Paper paperToEdit;
        private bool isEditingPaper;

        #endregion

        #region Events

        #endregion

        #region General Methods

        public ExamPapersConfigurationForm(ExamPapers papers)
        {
            this.InitializeComponent();
            this.ExamPaper = papers;

            this.Awake();
        }

        private void Awake()
        {
            papersListTable.Dock = DockStyle.Fill;
            this.PopulateListItems();
        }

        #endregion

        #region UI Methods

        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (this.ExamPaper == null || this.ExamPaper.GetPapers.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (var i = 0; i < this.ExamPaper.GetPapers.Count; i++)
            {
                var paperDataListItem = PaperDataListItem.Create(this.ExamPaper.GetPapers[i]);
                paperDataListItem.OnControlButtonPressedEvent += this.PaperDataListItem_OnControlButtonPressedEvent;
                containerFlowPanel.Controls.Add(paperDataListItem);
                paperDataListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }

        private void PaperDataListItem_OnControlButtonPressedEvent(object sender,
            PaperDataListItem.ControlButton controlButton)
        {
            switch (controlButton)
            {
                case PaperDataListItem.ControlButton.Delete:
                    this.DeletePaper((PaperDataListItem)sender);
                    break;

                case PaperDataListItem.ControlButton.Configure:
                    this.ConfigurePaper((PaperDataListItem)sender);
                    break;
            }
        }

        private void DataConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (var i = 0; i < allConfigs.Count; i++)
            {
                var isSaved = ConfigurationBase.Save(allConfigs[i], out var ex);

                if (!isSaved)
                {
                    Messages.SaveFileException(ex);
                }
            }
        }

        private async void finishPaperBtn_Click(object sender, EventArgs e)
        {
            var paperCode = (int)paperCodeField.IntegerValue;
            if (paperCodeField.Text == "")
            {
                Messages.ShowError("Paper code cannot be empty");
                return;
            }

            if (!isEditingPaper && GeneralManager.GetExamPapers != null &&
                GeneralManager.GetExamPapers.GetPapers.Exists(x => x.Code == paperCode))
            {
                Messages.ShowError("Paper code already exists");
                return;
            }

            var paperTitle = paperTitleField.Text;
            if (paperTitleField.Text == "")
            {
                Messages.ShowError("Paper title cannot be empty");
                return;
            }

            if (!isEditingPaper && GeneralManager.GetExamPapers != null &&
                GeneralManager.GetExamPapers.GetPapers.Exists(x => x.Title == paperTitle))
            {
                Messages.ShowError("Paper with that title already exists");
                return;
            }

            var paperDirection = (PaperDirection)paperDirectionField.SelectedValue;
            var totalFields = (int)paperFieldsCountField.IntegerValue;
            var totalOptions = (int)paperOptionsCountField.IntegerValue;
            if (paperFieldsCountField.Text == "")
            {
                Messages.ShowError("Paper fields cannot be empty");
                return;
            }

            if (paperOptionsCountField.Text == "")
            {
                Messages.ShowError("Paper options cannot be empty");
                return;
            }

            if (totalOptions < 2)
            {
                Messages.ShowError("There must atleast be more than one option per field.");
                return;
            }

            var newPaper = new Paper(paperCode, paperTitle, totalFields, totalOptions, paperDirection);
            if (!isEditingPaper)
            {
                GeneralManager.GetExamPapers.GetPapers.Add(newPaper);

                var paperDataListItem = PaperDataListItem.Create(newPaper);
                containerFlowPanel.Controls.Add(paperDataListItem);
                emptyListLabel.Visible = false;
            }
            else
            {
                var oldCode = paperToEdit.Code;
                paperToEdit.Reset(newPaper);
                isEditingPaper = false;

                for (var i = 0; i < containerFlowPanel.Controls.Count; i++)
                {
                    var paperDataListItem = (PaperDataListItem)containerFlowPanel.Controls[i];
                    if (paperDataListItem.PaperCode == oldCode)
                    {
                        paperDataListItem.PaperCode = paperCode;
                        paperDataListItem.PaperTitle = paperTitle;
                    }
                }
            }

            papersListTable.Visible = true;
            paperConfigurationPanel.Visible = false;
            papersListTable.Dock = DockStyle.Fill;
            await LSTM.SavePapers(GeneralManager.GetExamPapers);
        }

        #endregion

        #region Main Methods

        private async void DeletePaper(PaperDataListItem paperListItem)
        {
            if (Messages.ShowQuestion("Are you sure you want to delete this Paper?", "Hold On", MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }

            var isDeleted = await GeneralManager.RemovePaper(paperListItem.PaperCode);

            try
            {
                if (isDeleted)
                {
                    containerFlowPanel.Controls.Remove(paperListItem);

                    if (GeneralManager.GetExamPapers.GetPapers.Count == 0)
                    {
                        if (!containerFlowPanel.Controls.Contains(emptyListLabel))
                        {
                            containerFlowPanel.Controls.Add(emptyListLabel);
                        }

                        emptyListLabel.Visible = true;
                    }

                    await LSTM.SavePapers(GeneralManager.GetExamPapers);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowError(ex.Message);
            }
        }

        private void ConfigurePaper(PaperDataListItem paperDataListItem)
        {
            paperToEdit = GeneralManager.GetExamPapers.GetPapers.Find(x => x.Code == paperDataListItem.PaperCode);
            paperCodeField.IntegerValue = paperToEdit.Code;
            paperTitleField.Text = paperToEdit.Title;
            paperDirectionField.SelectedValue = paperToEdit.GetPaperDirection;
            paperFieldsCountField.IntegerValue = paperToEdit.GetFieldsCount;
            paperOptionsCountField.IntegerValue = paperToEdit.GetOptionsCount;

            isEditingPaper = true;

            papersListTable.Visible = false;
            paperConfigurationPanel.Visible = true;
            paperConfigurationPanel.Dock = DockStyle.Fill;

            paperDirectionField.DataSource = EnumHelper.ToList(typeof(PaperDirection));
            paperDirectionField.DisplayMember = "Value";
            paperDirectionField.ValueMember = "Key";
        }

        #endregion

        private void addNewPaperBtn_Click(object sender, EventArgs e)
        {
            papersListTable.Visible = false;
            paperConfigurationPanel.Visible = true;
            paperConfigurationPanel.Dock = DockStyle.Fill;

            paperDirectionField.DataSource = EnumHelper.ToList(typeof(PaperDirection));
            paperDirectionField.DisplayMember = "Value";
            paperDirectionField.ValueMember = "Key";
        }
    }
}