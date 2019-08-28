using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Synapse.Controls;
using Synapse.Core.Templates;
using Synapse.Utilities;
using Synapse.Utilities.Memory;
using Syncfusion.WinForms.Controls;

namespace Synapse.Modules
{
    public partial class TemplatesHub : SfForm
    {
        #region Properties
        public bool EditTemplateToggle { get { return editTemplateToggle; } set { editTemplateToggle = value; ToggleEditTemplate(value); } }
        private bool editTemplateToggle;
        public bool IsEditNameValid { get { return isEditNameValid; } set { isEditNameValid = value; ToggleNameValid(); } }
        private bool isEditNameValid = true;
        public string EditTemplateName { get { return editTemplateName; } set { editTemplateName = value; IsEditNameValid = ValidateName(value); } }
        private string editTemplateName = "Template Name";
        public bool NewTemplateToggle { get { return newTemplateToggle; } set { newTemplateToggle = value; ToggleCreateTemplate(value); } }
        private bool newTemplateToggle;
        public bool IsNewNameValid { get { return isNewNameValid; } set { isNewNameValid = value; ToggleNameValid(); } }
        private bool isNewNameValid = true;
        public string NewTemplateName { get { return newTemplateName; } set { newTemplateName = value; IsNewNameValid = ValidateName(value); } }
        private string newTemplateName = "New Template";
        public TemplateListItem SelectedTemplate { get { return selectedTemplate; } set { selectedTemplate = value; OnSelectedTemplateChangedEvent?.Invoke(this, value.TemplateName); } }
        private TemplateListItem selectedTemplate;

        public List<TemplateListItem> TemplateListItems = new List<TemplateListItem>();
        public int PinnedCount { get { return pinnedCount; } set { pinnedCount = value; } }
        private int pinnedCount = 0;

        private bool exitApp = true;
        #endregion

        #region Events
        public delegate void OnTemplateCallback(object sender, string templateName);
        public event OnTemplateCallback OnSelectedTemplateChangedEvent;
        public event OnTemplateCallback OnLoadTemplateEvent;
        #endregion

        #region General Methods
        public TemplatesHub()
        {
            InitializeComponent();

            Awake();
        }
        private async void Awake()
        {
            await LoadAllTemplates();

            if (TemplateListItems.Count == 0)
                emptyListLabel.Visible = true;

            OnLoadTemplateEvent += LoadTemplate;
        }
        private void TemplatesHub_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (exitApp)
                Application.Exit();
        }
        private void TemplateSelect(object sender, bool isSelected)
        {
            EditTemplateToggle = false;

            if (SelectedTemplate == (TemplateListItem)sender)
                SelectedTemplate = null;

            if (SelectedTemplate != null)
                SelectedTemplate.IsSelected = false;

            if (isSelected)
                SelectedTemplate = (TemplateListItem)sender;
        }
        private void TemplatePin(object sender, bool isPinned)
        {
            if (isPinned)
            {
                PinnedCount++;
                templatesLayoutPanel.Controls.SetChildIndex((Control)sender, Math.Max(0, PinnedCount-1));
            }
            else
            {
                PinnedCount--;
                templatesLayoutPanel.Controls.SetChildIndex((Control)sender, PinnedCount);
            }

            SaveTemplateItems();
        }
        private void ToggleNameValid()
        {
            if (IsNewNameValid)
            {
                createTemplateNameTextBox.BorderColor = Color.Gainsboro;
                createTemplateNameTextBox.ForeColor = Color.FromArgb(255, 68, 68, 68);
                addTemplateBtn.BackColor = Color.MediumTurquoise;
            }
            else
            {
                createTemplateNameTextBox.BorderColor = Color.Crimson;
                createTemplateNameTextBox.ForeColor = Color.Crimson;
                addTemplateBtn.BackColor = Color.Crimson;
            }

            if (IsEditNameValid)
            {
                editTemplateNameTextBoxExt.BorderColor = Color.Gainsboro;
                editTemplateNameTextBoxExt.ForeColor = Color.FromArgb(255, 68, 68, 68);
                setTemplateNameBtn.BackColor = Color.CornflowerBlue;
            }
            else
            {
                editTemplateNameTextBoxExt.BorderColor = Color.Crimson;
                editTemplateNameTextBoxExt.ForeColor = Color.Crimson;
                setTemplateNameBtn.BackColor = Color.Crimson;
            }
        }
        private bool ValidateName(string name)
        {
            bool isValid;
            isValid = name != "" && name[0] != ' ' && name[name.Length-1] != ' ' && !TemplateListItems.Exists(x => x.TemplateName == name) && name != "" && name[0] != ' ' && name[name.Length-1] != ' ';
            return isValid;
        }
        private void ToggleCreateTemplate(bool isTrue)
        {
            if (isTrue)
            {
                containerPanel.Controls.SetChildIndex(createTemplatePanel, 0);
                createTemplatePanel.Visible = true;
            }
            else
            {
                createTemplatePanel.Visible = false;
                containerPanel.Controls.SetChildIndex(createTemplatePanel, 1);
            }
        }
        private void ToggleEditTemplate(bool isTrue)
        {
            if (isTrue)
            {
                containerPanel.Controls.SetChildIndex(editTemplatePanel, 0);
                editTemplatePanel.Visible = true;
            }
            else
            {
                editTemplatePanel.Visible = false;
                containerPanel.Controls.SetChildIndex(editTemplatePanel, 1);
            }
        }
        #endregion

        #region UI Methods
        private void LoadTemplateBtn_Click(object sender, EventArgs e)
        {
            if (SelectedTemplate == null)
                return;

            exitApp = false;
            OnLoadTemplateEvent?.Invoke(this, SelectedTemplate.TemplateName);
        }

        private void CreateTemplateBtn_Click(object sender, EventArgs e)
        {
            EditTemplateToggle = false;

            NewTemplateToggle = !NewTemplateToggle;
            createTemplateNameTextBox.ResetText();
            createTemplateNameTextBox.Text = "New Template";
        }
        private void AddTemplateBtn_Click(object sender, EventArgs e)
        {
            if (!IsNewNameValid)
                return;

            CreateNewTemplate(NewTemplateName);
        }

        private void EditTemplateBtn_Click(object sender, EventArgs e)
        {
            NewTemplateToggle = false;

            if (SelectedTemplate == null)
                return;

            EditTemplateToggle = !EditTemplateToggle;
            editTemplateNameTextBoxExt.ResetText();
            editTemplateNameTextBoxExt.Text = SelectedTemplate.TemplateName;
        }
        private void SetTemplateNameBtn_Click(object sender, EventArgs e)
        {
            if (!IsEditNameValid)
                return;

            SetTemplateName(EditTemplateName);
        }

        private void DeleteTemplateBtn_Click(object sender, EventArgs e)
        {
            if (SelectedTemplate == null)
                return;

            DeleteTemplate(SelectedTemplate);
        }
        private void ImportTemplateBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            string selectedPath = folderBrowserDialog.SelectedPath;
            if (selectedPath == "")
                return;
            ImportTemplate(selectedPath);
        }
        #endregion

        #region Main Methods
        private void CreateNewTemplate(string templateName)
        {
            Template.CreateTemplate(templateName);

            TemplateListItem templateListItem = TemplateListItem.Create(templateName);
            templateListItem.OnSelectedChangedEvent += TemplateSelect;
            templateListItem.OnPinnedChangedEvent += TemplatePin;

            templatesLayoutPanel.Controls.Add(templateListItem);
            TemplateListItems.Add(templateListItem);

            SaveTemplateItems();

            emptyListLabel.Visible = false;
            NewTemplateToggle = false;
        }
        private async Task LoadAllTemplates()
        {
            var objectDatas = await LSTM.LoadTemplateListItemsAsync();
            for (int i = 0; i < objectDatas.Count; i++)
            {
                TemplateListItem templateListItem = TemplateListItem.Create(objectDatas[i]);
                templateListItem.OnSelectedChangedEvent += TemplateSelect;
                templateListItem.OnPinnedChangedEvent += TemplatePin;

                TemplateListItems.Add(templateListItem);
                templatesLayoutPanel.Controls.Add(templateListItem);
            }
        }   
        private async void LoadTemplate(object sender, string templateName)
        {
            Template template = await Template.LoadTemplate(templateName);
            if (template == null && Messages.ShowQuestion("Template was not found at its location and cannot be loaded, Would you like to remove this template from the list as well?") == DialogResult.Yes)
            {
                DeleteTemplate(SelectedTemplate);
            }
            else
            {
                Hide();
                SynapseMain.RunTemplate(template);
            }
        }
        private async void SaveTemplateItems()
        {
            Task.Run(() =>
            {
                List<TemplateListItem.ObjectData> templateListItemsObjects = new List<TemplateListItem.ObjectData>();
                for (int i = 0; i < templatesLayoutPanel.Controls.Count; i++)
                {
                    TemplateListItem templateListItem = (TemplateListItem)templatesLayoutPanel.Controls[i];
                    TemplateListItem.ObjectData objectData = templateListItem.GetObjectData();
                    objectData.ListIndex = i;
                    templateListItemsObjects.Add(objectData);
                }
                LSTM.SaveTemplateListItems(templateListItemsObjects);
            });
        }
        private void SetTemplateName(string templateName)
        {
            Template.ChangeTemplateName(SelectedTemplate.TemplateName, templateName);
            SelectedTemplate.TemplateName = templateName;

            SaveTemplateItems();

            EditTemplateToggle = false;
        }
        private async void DeleteTemplate(TemplateListItem template)
        {
            if (!TemplateListItems.Contains(template))
                return;

            await Template.DeleteTemplate(template.TemplateName);

            templatesLayoutPanel.Controls.Remove(template);
            TemplateListItems.Remove(template);
            template.Dispose();
            SelectedTemplate = null;

            SaveTemplateItems();

            if (TemplateListItems.Count == 0)
                emptyListLabel.Visible = true;
        }
        private async void ImportTemplate(string path)
        {
            Template tmp = await Template.ImportTemplate(path);
            if (tmp == null)
            {
                Messages.ShowError("The selected folder doesn't follow a template signature.");
                return;
            }
            else if(TemplateListItems.Exists(x => x.TemplateName == tmp.GetTemplateName))
            {
                Messages.ShowError("Unable to import the template as another template with the same name already exists.");
                return;
            }

            TemplateListItem templateListItem = TemplateListItem.Create(new TemplateListItem.ObjectData(tmp.TemplateData.TemplateName, false));
            templateListItem.OnSelectedChangedEvent += TemplateSelect;
            templateListItem.OnPinnedChangedEvent += TemplatePin;
            TemplateListItems.Add(templateListItem);
            templatesLayoutPanel.Controls.Add(templateListItem);

            emptyListLabel.Visible = false;

            Template.SaveTemplate(tmp.TemplateData, true);
            SaveTemplateItems();
        }
        #endregion
    }
}