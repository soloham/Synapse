using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Synapse.Controls;
using Synapse.Core.Configurations;
using Synapse.Core.Managers;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Memory;
using Syncfusion.DataSource.Extensions;
using Syncfusion.WinForms.Controls;
using static Synapse.Controls.ConfigureDataListItem;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using Synapse.Core;
using Synapse.Utilities.Enums;

namespace Synapse.Modules
{
    public partial class ExamPapersConfigurationForm : SfForm
    {
        #region Properties
        public ExamPapers ExamPaper { get; set; }
        #endregion

        #region Variables
        Paper paperToEdit;
        bool isEditingPaper = false;
        #endregion

        #region Events
        #endregion

        #region General Methods
        public ExamPapersConfigurationForm(ExamPapers papers)
        {
            InitializeComponent();
            ExamPaper = papers;

            Awake();
        }
        private void Awake()
        {
            papersListTable.Dock = DockStyle.Fill;
            PopulateListItems();
        }
        #endregion

        #region UI Methods
        private void PopulateListItems()
        {
            containerFlowPanel.Controls.Clear();

            if (ExamPaper == null || ExamPaper.GetPapers.Count == 0)
            {
                containerFlowPanel.Controls.Add(emptyListLabel);
                emptyListLabel.Visible = true;
                return;
            }

            for (int i = 0; i < ExamPaper.GetPapers.Count; i++)
            {
                PaperDataListItem paperDataListItem = PaperDataListItem.Create(ExamPaper.GetPapers[i]);
                paperDataListItem.OnControlButtonPressedEvent += PaperDataListItem_OnControlButtonPressedEvent;
                containerFlowPanel.Controls.Add(paperDataListItem);
                paperDataListItem.Size = new Size(containerFlowPanel.Width, 48);
            }
        }

        private void PaperDataListItem_OnControlButtonPressedEvent(object sender, PaperDataListItem.ControlButton controlButton)
        {
            switch (controlButton)
            {
                case PaperDataListItem.ControlButton.Delete:
                    DeletePaper((PaperDataListItem)sender);
                    break;
                case PaperDataListItem.ControlButton.Configure:
                    ConfigurePaper((PaperDataListItem)sender);
                    break;
            }
        }

        private void DataConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var allConfigs = ConfigurationsManager.GetAllConfigurations;
            for (int i = 0; i < allConfigs.Count; i++)
            {
                bool isSaved = ConfigurationBase.Save(allConfigs[i], out Exception ex);

                if (!isSaved)
                    Messages.SaveFileException(ex);
            }
        }

        private async void finishPaperBtn_Click(object sender, EventArgs e)
        {
            int paperCode = (int)paperCodeField.IntegerValue;
            if(paperCodeField.Text == "")
            {
                Messages.ShowError("Paper code cannot be empty");
                return;
            }
            else if (!isEditingPaper && GeneralManager.GetExamPapers != null && GeneralManager.GetExamPapers.GetPapers.Exists(x => x.Code == paperCode))
            {
                Messages.ShowError("Paper code already exists");
                return;
            }
            string paperTitle = paperTitleField.Text;
            if (paperTitleField.Text == "")
            {
                Messages.ShowError("Paper title cannot be empty");
                return;
            }
            else if (!isEditingPaper && GeneralManager.GetExamPapers != null && GeneralManager.GetExamPapers.GetPapers.Exists(x => x.Title == paperTitle))
            {
                Messages.ShowError("Paper with that title already exists");
                return;
            }
            PaperDirection paperDirection = (PaperDirection)paperDirectionField.SelectedValue;
            int totalFields = (int)paperFieldsCountField.IntegerValue;
            int totalOptions = (int)paperOptionsCountField.IntegerValue;
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
            else if (totalOptions < 2)
            {
                Messages.ShowError("There must atleast be more than one option per field.");
                return;
            }

            Paper newPaper = new Paper(paperCode, paperTitle, totalFields, totalOptions, paperDirection);
            if (!isEditingPaper)
            {
                GeneralManager.GetExamPapers.GetPapers.Add(newPaper);

                PaperDataListItem paperDataListItem = PaperDataListItem.Create(newPaper);
                containerFlowPanel.Controls.Add(paperDataListItem);
                emptyListLabel.Visible = false;
            }
            else
            {
                var oldCode = paperToEdit.Code;
                paperToEdit.Reset(newPaper);
                isEditingPaper = false;

                for (int i = 0; i < containerFlowPanel.Controls.Count; i++)
                {
                    PaperDataListItem paperDataListItem = (PaperDataListItem)containerFlowPanel.Controls[i];
                    if(paperDataListItem.PaperCode == oldCode)
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
            if (Messages.ShowQuestion("Are you sure you want to delete this Paper?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                return;

            bool isDeleted = await GeneralManager.RemovePaper(paperListItem.PaperCode);

            try
            {
                if (isDeleted)
                {
                    containerFlowPanel.Controls.Remove(paperListItem);

                    if (GeneralManager.GetExamPapers.GetPapers.Count == 0)
                    {
                        if (!containerFlowPanel.Controls.Contains(emptyListLabel))
                            containerFlowPanel.Controls.Add(emptyListLabel);

                        emptyListLabel.Visible = true;
                    }

                    await LSTM.SavePapers(GeneralManager.GetExamPapers);
                }
            }
            catch(Exception ex)
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