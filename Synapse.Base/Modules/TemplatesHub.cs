namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Synapse.Controls;
    using Synapse.Core.Templates;
    using Synapse.Utilities;
    using Synapse.Utilities.Memory;

    using Syncfusion.WinForms.Controls;

    public partial class TemplatesHub : SfForm
    {
        #region Properties

        public bool EditTemplateToggle
        {
            get => editTemplateToggle;
            set
            {
                editTemplateToggle = value;
                this.ToggleEditTemplate(value);
            }
        }

        private bool editTemplateToggle;

        public bool IsEditNameValid
        {
            get => isEditNameValid;
            set
            {
                isEditNameValid = value;
                this.ToggleNameValid();
            }
        }

        private bool isEditNameValid = true;

        public string EditTemplateName
        {
            get => editTemplateName;
            set
            {
                editTemplateName = value;
                this.IsEditNameValid = this.ValidateName(value);
            }
        }

        private string editTemplateName = "Template Name";

        public bool NewTemplateToggle
        {
            get => newTemplateToggle;
            set
            {
                newTemplateToggle = value;
                this.ToggleCreateTemplate(value);
            }
        }

        private bool newTemplateToggle;

        public bool IsNewNameValid
        {
            get => isNewNameValid;
            set
            {
                isNewNameValid = value;
                this.ToggleNameValid();
            }
        }

        private bool isNewNameValid = true;

        public string NewTemplateName
        {
            get => newTemplateName;
            set
            {
                newTemplateName = value;
                this.IsNewNameValid = this.ValidateName(value);
            }
        }

        private string newTemplateName = "New Template";

        public TemplateListItem SelectedTemplate
        {
            get => selectedTemplate;
            set
            {
                selectedTemplate = value;
                this.OnSelectedTemplateChangedEvent?.Invoke(this, value.TemplateName);
            }
        }

        private TemplateListItem selectedTemplate;

        public List<TemplateListItem> TemplateListItems = new List<TemplateListItem>();
        public int PinnedCount { get; set; }

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
            Program.DefaultSplashScreen.Hide();

            this.InitializeComponent();

            this.Awake();
        }

        private async void Awake()
        {
            await this.LoadAllTemplates();

            if (TemplateListItems.Count == 0)
            {
                emptyListLabel.Visible = true;
            }

            this.OnLoadTemplateEvent += this.LoadTemplate;
        }

        private void TemplatesHub_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (exitApp)
            {
                Application.Exit();
            }
        }

        private void TemplateSelect(object sender, bool isSelected)
        {
            this.EditTemplateToggle = false;

            if (this.SelectedTemplate == (TemplateListItem)sender)
            {
                this.SelectedTemplate = null;
            }

            if (this.SelectedTemplate != null)
            {
                this.SelectedTemplate.IsSelected = false;
            }

            if (isSelected)
            {
                this.SelectedTemplate = (TemplateListItem)sender;
            }
        }

        private void TemplatePin(object sender, bool isPinned)
        {
            if (isPinned)
            {
                this.PinnedCount++;
                templatesLayoutPanel.Controls.SetChildIndex((Control)sender, Math.Max(0, this.PinnedCount - 1));
            }
            else
            {
                this.PinnedCount--;
                templatesLayoutPanel.Controls.SetChildIndex((Control)sender, this.PinnedCount);
            }

            this.SaveTemplateItems();
        }

        private void ToggleNameValid()
        {
            if (this.IsNewNameValid)
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

            if (this.IsEditNameValid)
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
            isValid = name != "" && name[0] != ' ' && name[name.Length - 1] != ' ' &&
                      !TemplateListItems.Exists(x => x.TemplateName == name) && name != "" && name[0] != ' ' &&
                      name[name.Length - 1] != ' ';
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
            if (this.SelectedTemplate == null)
            {
                return;
            }

            exitApp = false;
            this.OnLoadTemplateEvent?.Invoke(this, this.SelectedTemplate.TemplateName);
        }

        private void CreateTemplateBtn_Click(object sender, EventArgs e)
        {
            this.EditTemplateToggle = false;

            this.NewTemplateToggle = !this.NewTemplateToggle;
            createTemplateNameTextBox.ResetText();
            createTemplateNameTextBox.Text = "New Template";
        }

        private void AddTemplateBtn_Click(object sender, EventArgs e)
        {
            if (!this.IsNewNameValid)
            {
                return;
            }

            this.CreateNewTemplate(this.NewTemplateName);
        }

        private void EditTemplateBtn_Click(object sender, EventArgs e)
        {
            this.NewTemplateToggle = false;

            if (this.SelectedTemplate == null)
            {
                return;
            }

            this.EditTemplateToggle = !this.EditTemplateToggle;
            editTemplateNameTextBoxExt.ResetText();
            editTemplateNameTextBoxExt.Text = this.SelectedTemplate.TemplateName;
        }

        private void SetTemplateNameBtn_Click(object sender, EventArgs e)
        {
            if (!this.IsEditNameValid)
            {
                return;
            }

            this.SetTemplateName(this.EditTemplateName);
        }

        private void DeleteTemplateBtn_Click(object sender, EventArgs e)
        {
            if (this.SelectedTemplate == null)
            {
                return;
            }

            this.DeleteTemplate(this.SelectedTemplate);
        }

        private void ImportTemplateBtn_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            var selectedPath = folderBrowserDialog.SelectedPath;
            if (selectedPath == "")
            {
                return;
            }

            this.ImportTemplate(selectedPath);
        }

        #endregion

        #region Main Methods

        private void CreateNewTemplate(string templateName)
        {
            Template.CreateTemplate(templateName);

            var templateListItem = TemplateListItem.Create(templateName);
            templateListItem.OnSelectedChangedEvent += this.TemplateSelect;
            templateListItem.OnPinnedChangedEvent += this.TemplatePin;

            templatesLayoutPanel.Controls.Add(templateListItem);
            TemplateListItems.Add(templateListItem);

            this.SaveTemplateItems();

            emptyListLabel.Visible = false;
            this.NewTemplateToggle = false;
        }

        private async Task LoadAllTemplates()
        {
            var objectDatas = await LSTM.LoadTemplateListItemsAsync();
            for (var i = 0; i < objectDatas.Count; i++)
            {
                var templateListItem = TemplateListItem.Create(objectDatas[i]);
                templateListItem.OnSelectedChangedEvent += this.TemplateSelect;
                templateListItem.OnPinnedChangedEvent += this.TemplatePin;

                TemplateListItems.Add(templateListItem);
                templatesLayoutPanel.Controls.Add(templateListItem);
            }
        }

        private async void LoadTemplate(object sender, string templateName)
        {
            this.Hide();
            Program.DefaultSplashScreen.ShowScreen($"Loading Template: {templateName}...");
            var template = await Template.LoadTemplate(templateName);

            if (template == null &&
                Messages.ShowQuestion(
                    "Template was not found at its location and cannot be loaded, Would you like to remove this template from the list as well?") ==
                DialogResult.Yes)
            {
                this.DeleteTemplate(this.SelectedTemplate);
            }
            else
            {
                this.SelectedTemplate.LastActiveTimeStamp = DateTime.Now.ToString();
                Task.Run(() => this.SaveTemplateItems());

                SynapseMain.RunTemplate(template);
            }
        }

        private async void SaveTemplateItems()
        {
            Task.Run(() =>
            {
                var templateListItemsObjects = new List<TemplateListItem.ObjectData>();
                for (var i = 0; i < templatesLayoutPanel.Controls.Count; i++)
                {
                    var templateListItem = (TemplateListItem)templatesLayoutPanel.Controls[i];
                    var objectData = templateListItem.GetObjectData();
                    objectData.ListIndex = i;
                    templateListItemsObjects.Add(objectData);
                }

                LSTM.SaveTemplateListItems(templateListItemsObjects);
            });
        }

        private void SetTemplateName(string templateName)
        {
            Template.ChangeTemplateName(this.SelectedTemplate.TemplateName, templateName);
            this.SelectedTemplate.TemplateName = templateName;

            this.SaveTemplateItems();

            this.EditTemplateToggle = false;
        }

        private async void DeleteTemplate(TemplateListItem template)
        {
            if (!TemplateListItems.Contains(template))
            {
                return;
            }

            await Template.DeleteTemplate(template.TemplateName);

            templatesLayoutPanel.Controls.Remove(template);
            TemplateListItems.Remove(template);
            template.Dispose();
            this.SelectedTemplate = null;

            this.SaveTemplateItems();

            if (TemplateListItems.Count == 0)
            {
                emptyListLabel.Visible = true;
            }
        }

        private async void ImportTemplate(string path)
        {
            var tmp = await Template.ImportTemplate(path);
            if (tmp == null)
            {
                Messages.ShowError("The selected folder doesn't follow a template signature.");
                return;
            }

            if (TemplateListItems.Exists(x => x.TemplateName == tmp.GetTemplateName))
            {
                Messages.ShowError(
                    "Unable to import the template as another template with the same name already exists.");
                return;
            }

            var templateListItem =
                TemplateListItem.Create(new TemplateListItem.ObjectData(tmp.TemplateData.TemplateName, false, 0,
                    DateTime.Now.ToLongDateString()));
            templateListItem.OnSelectedChangedEvent += this.TemplateSelect;
            templateListItem.OnPinnedChangedEvent += this.TemplatePin;
            TemplateListItems.Add(templateListItem);
            templatesLayoutPanel.Controls.Add(templateListItem);

            emptyListLabel.Visible = false;

            Template.SaveTemplate(tmp.TemplateData, true);
            this.SaveTemplateItems();
        }

        #endregion
    }
}