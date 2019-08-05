using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Synapse.Controls;
using Synapse.Core;
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
        #endregion

        #region Main Methods
        private void CreateNewTemplate(string templateName)
        {
            Template.CreateTemplate(templateName);

            TemplateListItem templateListItem = new TemplateListItem();
            templateListItem.Initialize(templateName);
            templateListItem.OnSelectedChangedEvent += TemplateSelect;
            templateListItem.OnPinnedChangedEvent += TemplatePin;

            templatesLayoutPanel.Controls.Add(templateListItem);
            TemplateListItems.Add(templateListItem);

            emptyListLabel.Visible = false;
            NewTemplateToggle = false;
        }
        private async void LoadTemplate(object sender, string templateName)
        {
            Template template = await Template.LoadTemplate(templateName);
            Hide();
            SynapseMain.RunTemplate(template);
        }
        private void SetTemplateName(string templateName)
        {
            SelectedTemplate.TemplateName = templateName;

            //MAIN FUNCTIONALITY

            EditTemplateToggle = false;
        }
        private void DeleteTemplate(TemplateListItem template)
        {
            if (!TemplateListItems.Contains(template))
                return;

            templatesLayoutPanel.Controls.Remove(template);
            TemplateListItems.Remove(template);
            template.Dispose();
            if (TemplateListItems.Count == 0)
                emptyListLabel.Visible = true;

            //MAIN FUNCTIONALITY
        }
        #endregion
    }
}