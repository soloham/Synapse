namespace Synapse.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Synapse.Core.Templates;
    using Synapse.Properties;
    using Synapse.Utilities;

    using Syncfusion.Windows.Forms;
    using Syncfusion.WinForms.Controls;

    public partial class DatabaseWizardForm : SfForm
    {
        public enum DatabaseToInject
        {
            SQL,
            ORACLE
        }

        public struct DTIKey
        {
            public DTIKey(int uID, string key) : this()
            {
                this.uID = uID;
                this.Key = key;
            }

            public int uID { get; set; }
            public string Key { get; set; }
        }

        public class DTIValue
        {
            public DTIValue()
            {
            }

            public DTIValue(List<string> value, bool isKey = false)
            {
                Values = value;
                this.IsKey = isKey;
            }

            public List<string> Values = new List<string>();
            public bool IsKey { get; set; }
        }

        private Dictionary<DTIKey, DTIValue> dataToInject;


        #region Variables

        private Color IdleStatusColor = Color.SlateGray;
        private readonly Color ConnectingStatusColor = Color.FromArgb(22, 165, 220);
        private readonly Color ConnectedStatusColor = Color.MediumAquamarine;
        private readonly Color DisconnectedStatusColor = Color.Crimson;

        private string[] databaseTables;
        private readonly string[] allFields;
        private string[] selectedFields;

        private DatabaseToInject databaseToInject;

        private readonly SynchronizationContext synchronizationContext;

        #endregion

        #region Connections

        private string connectionString;

        private SqlConnection sqlConnection;

        public ObservableCollection<dynamic> DataToInject { get; }

        #endregion

        #region General Methods

        public DatabaseWizardForm(string[] selectableFields, ObservableCollection<dynamic> dataToInject)
        {
            this.InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            allFields = selectableFields;

            this.DataToInject = dataToInject;

            try
            {
                var sqlConnectionString =
                    (string)SynapseMain.GetCurrentTemplate.TemplateData.GetProperty("DefaultSQLCS");
                sqlConnectionStringField.Text = sqlConnectionString;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        public List<string> ListTables(SqlConnection sqlConnection)
        {
            var tables = new List<string>();
            var dt = sqlConnection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                var tablename = (string)row[2];
                tables.Add(tablename);
            }

            return tables;
        }

        private void selectFieldsBtn_Click(object sender, EventArgs e)
        {
            switch (databaseToInject)
            {
                case DatabaseToInject.SQL:
                    if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
                    {
                        Messages.ShowError("Database connection must first be established",
                            icon: MessageBoxIcon.Warning);
                        return;
                    }

                    break;

                case DatabaseToInject.ORACLE:
                    break;
            }

            var selectFieldsForDBForm = new SelectFieldsForDBForm(allFields, selectedFields);
            selectFieldsForDBForm.SelectFieldsEvent += (s, selectedFields) =>
            {
                this.selectedFields = selectedFields;
                this.SelectData();

                selectFieldsForDBForm.Hide();
            };
            selectFieldsForDBForm.ShowDialog();
        }

        private void databasesTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (databasesTabControl.SelectedTab.Name)
            {
                case "Microsoft SQL":
                    databaseToInject = DatabaseToInject.SQL;
                    break;

                case "Oracle":
                    databaseToInject = DatabaseToInject.ORACLE;
                    break;
            }
        }

        private void openDBConnectionBtn_Click(object sender, EventArgs e)
        {
            connectionString = sqlConnectionStringField.Text;

            switch (databaseToInject)
            {
                case DatabaseToInject.SQL:
                    if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                        connectionStatusPanel.BackColor = DisconnectedStatusColor;

                        sqlConnectionStringField.Enabled = true;
                        openDBConnectionBtn.Text = "Open  Connection";
                        openDBConnectionBtn.MetroColor = Color.DodgerBlue;

                        saveCSBtn.MetroColor = Color.Gray;
                    }
                    else
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        connectionStatusPanel.BackColor = ConnectingStatusColor;

                        try
                        {
                            sqlConnection.Open();

                            if (sqlConnection.State == ConnectionState.Open)
                            {
                                connectionStatusPanel.BackColor = ConnectedStatusColor;
                                Messages.ShowInformation("Database Connection Successfully Established");

                                sqlConnectionStringField.Enabled = false;
                                openDBConnectionBtn.MetroColor = Color.LightCoral;
                                openDBConnectionBtn.Text = "Close  Connection";

                                databaseTables = this.ListTables(sqlConnection).ToArray();
                                databaseTableCB.DataSource = databaseTables;
                            }
                            else
                            {
                                sqlConnectionStringField.BackColor = DisconnectedStatusColor;
                                Messages.ShowError("An error occured while establishing the connection");
                            }
                        }
                        catch (Exception ex)
                        {
                            connectionStatusPanel.BackColor = DisconnectedStatusColor;
                            Messages.ShowError("An error occured while establishing the connection: \n\n" + ex.Message);
                        }
                    }

                    break;

                case DatabaseToInject.ORACLE:
                    break;
            }
        }

        private int currentRow;

        private async void startInsertionBtn_Click(object sender, EventArgs e)
        {
            switch (databaseToInject)
            {
                case DatabaseToInject.SQL:
                    if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
                    {
                        Messages.ShowError("Database connection must first be established",
                            icon: MessageBoxIcon.Warning);
                        return;
                    }

                    break;

                case DatabaseToInject.ORACLE:
                    break;
            }

            injectionProgressBar.Value = 0;
            injectionProgressBar.BringToFront();

            var tableName = databaseTableCB.SelectedItem as string;
            var dbColumnTSQL = "";
            var keysList = dataToInject.Keys.ToList();
            for (var i = 0; i < keysList.Count; i++)
                dbColumnTSQL += '[' + keysList[i].Key + ']' + (i == keysList.Count - 1 ? "" : ", ");

            var isSuccessful = true;

            var skippedCount = 0;
            await Task.Run(() =>
            {
                var cmnd = new SqlCommand();
                cmnd.Connection = sqlConnection;
                cmnd.StatementCompleted += (s, _e) =>
                {
                    synchronizationContext.Send(delegate
                    {
                        var value = (int)((float)currentRow / this.DataToInject.Count * 100);
                        injectionProgressBar.Value = value;
                    }, null);
                };

                for (var i = 0; i < this.DataToInject.Count; i++)
                {
                    currentRow = i + 1;

                    var keyDatae = new List<DTIKey>();
                    var dbValuesTSQL = "";
                    for (var j = 0; j < keysList.Count; j++)
                    {
                        var curDTIKey = keysList[j];
                        var curDTIValue = dataToInject[curDTIKey];
                        dbValuesTSQL += "'" + curDTIValue.Values[i] + (j == keysList.Count - 1 ? "'" : "', ");

                        if (curDTIValue.IsKey)
                        {
                            keyDatae.Add(curDTIKey);
                        }
                    }

                    var skipInjection = false;

                    if (keyDatae.Count > 0)
                    {
                        var sqlSearchCommand = $"SELECT TOP 1 [{keyDatae[0].Key}] FROM {tableName} WHERE";
                        for (var k = 0; k < keyDatae.Count; k++)
                        {
                            var dTIKey = keyDatae[k];
                            var dTIValue = dataToInject[dTIKey];

                            sqlSearchCommand +=
                                $"{(sqlSearchCommand[sqlSearchCommand.Length - 1] == '\'' ? " AND " : " ")}[{dTIKey.Key}] LIKE \'{dTIValue.Values[i]}\'";
                            cmnd.CommandText = sqlSearchCommand;
                        }

                        try
                        {
                            using (var reader = cmnd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    skipInjection = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (MessageBoxAdv.Show(
                                $" Data Row Index: {i + 1} \n \n Error: {ex.Message} \n \n Would you like to terminate the process?",
                                "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                            {
                                isSuccessful = false;

                                break;
                            }
                        }
                    }

                    if (skipInjection)
                    {
                        skippedCount++;
                        continue;
                    }

                    var sqlCommand = $"INSERT INTO {tableName} ({dbColumnTSQL}) VALUES ({dbValuesTSQL})";
                    cmnd.CommandText = sqlCommand;
                    try
                    {
                        cmnd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        if (MessageBoxAdv.Show(
                            $" Data Row Index: {i + 1} \n \n Error: {ex.Message} \n \n Would you like to terminate the process?",
                            "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        {
                            isSuccessful = false;

                            break;
                        }
                    }

                    //if (TheGrader.notifyMe)
                    //{
                    //    if (gradedDatae[0].subGradedDatas.Exists(x => x.title == "Roll No" || x.title == "Roll Number" || x.title == "Roll #"))
                    //    {
                    //        Task.Run(() =>
                    //        {
                    //            List<GradedData> toNotifyDatae = gradedDatae.ToList().FindAll(x => x.subGradedDatas.Find(x1 => x1.title == "Roll No" || x1.title == "Roll Number" || x1.title == "Roll #").value == "512429" || x.subGradedDatas.Find(x1 => x1.title == "Roll No" || x1.title == "Roll Number" || x1.title == "Roll #").value == "560359");
                    //            if (toNotifyDatae.Count > 0)
                    //                Program.NotifyDataViaEmail(toNotifyDatae);
                    //        });
                    //    }
                    //}
                }
            });

            if (isSuccessful)
            {
                if (skippedCount > 0)
                {
                    MessageBoxAdv.Show(
                        $"Insertion Complete. \n \n Skipped {skippedCount} rows out of {this.DataToInject.Count} due to duplication.",
                        "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBoxAdv.Show("All data has been successfuly inserted", "", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }

            if (sqlConnection.State != ConnectionState.Open)
            {
                connectionStatusPanel.BackColor = DisconnectedStatusColor;

                sqlConnectionStringField.Enabled = true;
                openDBConnectionBtn.Text = "Open  Connection";
                openDBConnectionBtn.MetroColor = Color.DodgerBlue;
            }

            injectionProgressBar.SendToBack();
            this.Enabled = true;
        }

        private void SelectData()
        {
            dataToInject = new Dictionary<DTIKey, DTIValue>();
            for (var i = 0; i < DBFieldsPanel.Controls.Count; i++) DBFieldsPanel.Controls[i].Dispose();

            DBFieldsPanel.Controls.Clear();

            for (var i = 0; i < selectedFields.Length; i++)
            {
                var fieldTitle = selectedFields[i];
                dataToInject.Add(new DTIKey(i, fieldTitle), new DTIValue());

                var databaseField = new DatabaseField(i, fieldTitle);

                databaseField.EditField += (sender, Id, Value, isKey) =>
                {
                    var isValid = true;

                    try
                    {
                        if (dataToInject.Keys.Any(x => x.Key == Value && x.uID != Id))
                        {
                            MessageBoxAdv.Show("This field already exists.", "Hold On", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            isValid = false;
                        }

                        var dictionaryList = dataToInject.ToList();
                        var dictObject = dictionaryList.Find(x => x.Key.uID == Id);
                        var dictKey = dictObject.Key;
                        var dictValue = dictObject.Value.Values;

                        dataToInject.Remove(dictKey);
                        dataToInject.Add(new DTIKey(Id, Value), new DTIValue(dictValue, isKey));
                    }
                    catch (Exception ex)
                    {
                        isValid = false;
                        MessageBoxAdv.Show(ex.Message, "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return isValid;
                };
                databaseField.RemoveField += (sender, Id, Value, isKey) =>
                {
                    var isValid = true;

                    try
                    {
                        if (!dataToInject.Keys.Any(x => x.Key == Value))
                        {
                            MessageBoxAdv.Show("Invalid field.", "Hold On", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            isValid = false;
                        }

                        dataToInject.Remove(dataToInject.Keys.First(x => x.Key == Value));

                        var toRemove = (DatabaseField)sender;
                        DBFieldsPanel.Controls.Remove(toRemove);
                        toRemove.Dispose();

                        if (dataToInject.Count == 0)
                        {
                            emptyListLabel.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isValid = false;
                        MessageBoxAdv.Show(ex.Message, "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return isValid;
                };

                emptyListLabel.Visible = false;
                this.Controls.Add(databaseField);
                DBFieldsPanel.Controls.Add(databaseField);
                databaseField.Dock = DockStyle.Top;
                databaseField.InitializeLayout();
            }

            for (var i = 0; i < this.DataToInject.Count; i++)
            for (var j = 0; j < selectedFields.Length; j++)
            {
                var value = (this.DataToInject[i] as IDictionary<string, object>)[selectedFields[j]].ToString();
                dataToInject.First(x => x.Key.Key == selectedFields[j]).Value.Values.Add(value);
            }
        }

        private void viewCSBtn_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void viewCSBtn_Click(object sender, EventArgs e)
        {
            sqlConnectionStringField.UseSystemPasswordChar = !sqlConnectionStringField.UseSystemPasswordChar;

            if (!sqlConnectionStringField.UseSystemPasswordChar)
            {
                viewCSBtn.BackgroundImage = Resources.Show_01_WF;
            }
            else
            {
                viewCSBtn.BackgroundImage = Resources.Hide;
            }
        }

        private async void saveCSBtn_Click(object sender, EventArgs e)
        {
            if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
            {
                Messages.ShowError("Database connection must first be established", icon: MessageBoxIcon.Warning);
                return;
            }

            SynapseMain.GetCurrentTemplate.TemplateData.AddProperty("DefaultSQLCS", sqlConnectionStringField.Text);
            var isSaved = await Task.Run(() => Template.SaveTemplate(SynapseMain.GetCurrentTemplate.TemplateData,
                string.IsNullOrEmpty(SynapseMain.GetCurrentTemplate.GetTemplateImage.ImageLocation)));

            if (isSaved)
            {
                saveCSBtn.MetroColor = Color.MediumAquamarine;
            }
            else
            {
                saveCSBtn.MetroColor = Color.Gray;
            }
        }
    }
}