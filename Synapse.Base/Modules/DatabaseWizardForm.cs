using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Synapse.Controls;
using Synapse.Core.Configurations;
using Synapse.Core.Managers;
using Synapse.Utilities;
using Syncfusion.WinForms.Controls;
using static Synapse.Controls.ConfigureDataListItem;
using System.Threading;
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Syncfusion.Windows.Forms;

namespace Synapse.Modules
{
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
                Key = key;
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
                IsKey = isKey;
            }

            public List<string> Values = new List<string>();
            public bool IsKey { get; set; }
        }
        Dictionary<DTIKey, DTIValue> dataToInject;


        #region Variables
        private Color IdleStatusColor = Color.SlateGray;
        private Color ConnectingStatusColor = Color.FromArgb(22, 165, 220);
        private Color ConnectedStatusColor = Color.MediumAquamarine;
        private Color DisconnectedStatusColor = Color.Crimson;

        private string[] databaseTables;
        private string[] allFields;
        private string[] selectedFields;

        private DatabaseToInject databaseToInject;

        private SynchronizationContext synchronizationContext;
        #endregion

        #region Connections
        private string connectionString; 

        SqlConnection sqlConnection;

        public ObservableCollection<dynamic> DataToInject { get; }
        #endregion

        #region General Methods
        public DatabaseWizardForm(string[] selectableFields, ObservableCollection<dynamic> dataToInject)
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            allFields = selectableFields;

            DataToInject = dataToInject;

            try
            {
                string sqlConnectionString = (string)SynapseMain.GetCurrentTemplate.TemplateData.GetProperty("DefaultSQLCS");
                sqlConnectionStringField.Text = sqlConnectionString;
            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        public List<string> ListTables(SqlConnection sqlConnection)
        {
            List<string> tables = new List<string>();
            DataTable dt = sqlConnection.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            return tables;
        }

        private void selectFieldsBtn_Click(object sender, EventArgs e)
        {
            switch (databaseToInject)
            {
                case DatabaseToInject.SQL:
                    if (sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
                    {
                        Messages.ShowError("Database connection must first be established", icon: MessageBoxIcon.Warning);
                        return;
                    }
                    break;
                case DatabaseToInject.ORACLE:
                    break;
             }

            SelectFieldsForDBForm selectFieldsForDBForm = new SelectFieldsForDBForm(allFields, selectedFields);
            selectFieldsForDBForm.SelectFieldsEvent += (object s, string[] selectedFields) =>
            {
                this.selectedFields = selectedFields;
                SelectData();

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

                            if (sqlConnection.State == System.Data.ConnectionState.Open)
                            {
                                connectionStatusPanel.BackColor = ConnectedStatusColor;
                                Messages.ShowInformation("Database Connection Successfully Established");

                                sqlConnectionStringField.Enabled = false;
                                openDBConnectionBtn.MetroColor = Color.LightCoral;
                                openDBConnectionBtn.Text = "Close  Connection";

                                databaseTables = ListTables(sqlConnection).ToArray();
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

        int currentRow;
        private async void startInsertionBtn_Click(object sender, EventArgs e)
        {
            switch (databaseToInject)
            {
                case DatabaseToInject.SQL:
                    if (sqlConnection == null || sqlConnection.State != System.Data.ConnectionState.Open)
                    {
                        Messages.ShowError("Database connection must first be established", icon: MessageBoxIcon.Warning);
                        return;
                    }
                    break;
                case DatabaseToInject.ORACLE:
                    break;
            }

            injectionProgressBar.Value = 0;
            injectionProgressBar.BringToFront();

            string tableName = databaseTableCB.SelectedItem as string;
            string dbColumnTSQL = "";
            var keysList = dataToInject.Keys.ToList();
            for (int i = 0; i < keysList.Count; i++)
            {
                dbColumnTSQL += '[' + keysList[i].Key + ']' + ((i == keysList.Count - 1) ? "" : ", ");
            }

            bool isSuccessful = true;

            int skippedCount = 0;
            await Task.Run(() =>
            {
                SqlCommand cmnd = new SqlCommand();
                cmnd.Connection = sqlConnection;
                cmnd.StatementCompleted += (object s, StatementCompletedEventArgs _e) =>
                {
                    synchronizationContext.Send(new SendOrPostCallback(
                        delegate (object state)
                        {
                            int value = (int)(((float)currentRow / DataToInject.Count) * 100);
                            injectionProgressBar.Value = value;
                        }), null);
                };

                for (int i = 0; i < DataToInject.Count; i++)
                {
                    currentRow = i + 1;

                    List<DTIKey> keyDatae = new List<DTIKey>();
                    string dbValuesTSQL = "";
                    for (int j = 0; j < keysList.Count; j++)
                    {
                        DTIKey curDTIKey = keysList[j];
                        DTIValue curDTIValue = dataToInject[curDTIKey];
                        dbValuesTSQL += "'" + curDTIValue.Values[i] + ((j == keysList.Count - 1) ? "'" : "', ");

                        if (curDTIValue.IsKey)
                            keyDatae.Add(curDTIKey);
                    }

                    bool skipInjection = false;

                    if (keyDatae.Count > 0)
                    {
                        string sqlSearchCommand = $"SELECT TOP 1 [{keyDatae[0].Key}] FROM {tableName} WHERE";
                        for (int k = 0; k < keyDatae.Count; k++)
                        {
                            DTIKey dTIKey = keyDatae[k];
                            DTIValue dTIValue = dataToInject[dTIKey];

                            sqlSearchCommand += $"{(sqlSearchCommand[sqlSearchCommand.Length - 1] == '\'' ? " AND " : " ")}[{dTIKey.Key}] LIKE \'{dTIValue.Values[i]}\'";
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
                            if (MessageBoxAdv.Show($" Data Row Index: {i + 1} \n \n Error: {ex.Message} \n \n Would you like to terminate the process?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
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

                    string sqlCommand = $"INSERT INTO {tableName} ({dbColumnTSQL}) VALUES ({dbValuesTSQL})";
                    cmnd.CommandText = sqlCommand;
                    try
                    {
                        cmnd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        if (MessageBoxAdv.Show($" Data Row Index: {i + 1} \n \n Error: {ex.Message} \n \n Would you like to terminate the process?", "Hold On", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
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
                    MessageBoxAdv.Show($"Insertion Complete. \n \n Skipped {skippedCount} rows out of {DataToInject.Count} due to duplication.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBoxAdv.Show("All data has been successfuly inserted", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (sqlConnection.State != ConnectionState.Open)
            {
                connectionStatusPanel.BackColor = DisconnectedStatusColor;

                sqlConnectionStringField.Enabled = true;
                openDBConnectionBtn.Text = "Open  Connection";
                openDBConnectionBtn.MetroColor = Color.DodgerBlue;
            }

            injectionProgressBar.SendToBack();
            Enabled = true;
        }

        void SelectData()
        {
            dataToInject = new Dictionary<DTIKey, DTIValue>();
            for (int i = 0; i < DBFieldsPanel.Controls.Count; i++)
            {
                DBFieldsPanel.Controls[i].Dispose();
            }
            DBFieldsPanel.Controls.Clear();

            for (int i = 0; i < selectedFields.Length; i++)
            {
                string fieldTitle = selectedFields[i];
                dataToInject.Add(new DTIKey(i, fieldTitle), new DTIValue());

                DatabaseField databaseField = new DatabaseField(i, fieldTitle);

                databaseField.EditField += (object sender, int Id, string Value, bool isKey) =>
                {
                    bool isValid = true;

                    try
                    {
                        if (dataToInject.Keys.Any(x => x.Key == Value && x.uID != Id))
                        {
                            MessageBoxAdv.Show("This field already exists.", "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                databaseField.RemoveField += (object sender, int Id, string Value, bool isKey) =>
                {
                    bool isValid = true;

                    try
                    {
                        if (!dataToInject.Keys.Any(x => x.Key == Value))
                        {
                            MessageBoxAdv.Show("Invalid field.", "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            isValid = false;
                        }

                        dataToInject.Remove(dataToInject.Keys.First(x => x.Key == Value));

                        var toRemove = (DatabaseField)sender;
                        DBFieldsPanel.Controls.Remove(toRemove);
                        toRemove.Dispose();

                        if (dataToInject.Count == 0)
                            emptyListLabel.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        isValid = false;
                        MessageBoxAdv.Show(ex.Message, "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return isValid;
                };

                emptyListLabel.Visible = false;
                Controls.Add(databaseField);
                DBFieldsPanel.Controls.Add(databaseField);
                databaseField.Dock = DockStyle.Top;
                databaseField.InitializeLayout();
            }
            for (int i = 0; i < DataToInject.Count; i++)
            {
                for (int j = 0; j < selectedFields.Length; j++)
                {
                    string value = (DataToInject[i] as IDictionary<string, object>)[selectedFields[j]].ToString();
                    dataToInject.First(x => x.Key.Key == selectedFields[j]).Value.Values.Add(value);
                }
            }
        }

        private void viewCSBtn_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void viewCSBtn_Click(object sender, EventArgs e)
        {
            sqlConnectionStringField.UseSystemPasswordChar = !sqlConnectionStringField.UseSystemPasswordChar;

            if (!sqlConnectionStringField.UseSystemPasswordChar)
                viewCSBtn.BackgroundImage = Properties.Resources.Show_01_WF;
            else
                viewCSBtn.BackgroundImage = Properties.Resources.Hide;
        }

        private async void saveCSBtn_Click(object sender, EventArgs e)
        {
            if (sqlConnection == null || sqlConnection.State != ConnectionState.Open)
            {
                Messages.ShowError("Database connection must first be established", icon: MessageBoxIcon.Warning);
                return;
            }

            SynapseMain.GetCurrentTemplate.TemplateData.AddProperty("DefaultSQLCS", sqlConnectionStringField.Text);
            bool isSaved = await Task.Run(() => Core.Templates.Template.SaveTemplate(SynapseMain.GetCurrentTemplate.TemplateData, string.IsNullOrEmpty(SynapseMain.GetCurrentTemplate.GetTemplateImage.ImageLocation)));

            if (isSaved)
                saveCSBtn.MetroColor = Color.MediumAquamarine;
            else
                saveCSBtn.MetroColor = Color.Gray;
        }
    }
}