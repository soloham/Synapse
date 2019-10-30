using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColSizeHelperSample;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Tools;
using Synapse.Core.Configurations;
using Syncfusion.Drawing;
using Synapse.Shared.Resources;

namespace Synapse.Controls.Answer_Key
{
    public partial class AnswerKeyFieldControl : UserControl
    {
        #region Properties
        [Browsable(true), Category("Options"), Description("Gets or sets total options for the field.")]
        public int TotalOptions { get => totalOptions; set { totalOptions = value; SetTotalOptions(value); } }
        private int totalOptions = 4;

        [Browsable(true), Category("Options"), Description("Gets or sets size of options.")]
        public Size OptionSize { get => optionSize; set { optionSize = value; SetTotalOptions(TotalOptions); } }
        private Size optionSize = new Size(30, 30);
        [Browsable(true), Category("Options"), Description("Gets or sets ValueDataType for the field.")]
        public Orientation FieldOrientation { get { return fieldOrientation; } set { fieldOrientation = value; SetOrientation(value); } }
        private Orientation fieldOrientation;

        [Browsable(true), Category("Options"), Description("Gets or sets ValueDataType for the field.")]
        public ValueDataType OptionsValueType { get { return optionsValueType; } set { optionsValueType = value; SetValueType(value); } }
        private ValueDataType optionsValueType = ValueDataType.Alphabet;
        public string SelectionValue { get => selectionValue; set { selectionValue = value; selectionLabel.Text = selectionValue; setOptionField.Text = selectionValue; } }
        public string selectionValue;

        public Size GetScaledOptionSize { 
            get {
                int optionsLength = fieldOrientation == Orientation.Horizontal? optionsTable.Size.Width : optionsTable.Size.Height;
                int options = TotalOptions;
                float scaleFactor = optionsLength / (float)options / 60;
                int scaledSize = (int)(OptionSize.Width * scaleFactor);
                var finalSize = scaledSize > 30? OptionSize : scaledSize < 10? new Size(10,10): new Size(scaledSize, scaledSize);
                return finalSize;
            }
        }
        #endregion

        #region Variables
        public List<CheckBoxAdv> OptionsUIList = new List<CheckBoxAdv>();
        public List<int> markedOptionsIndexes = new List<int>();
        #endregion

        #region Public Mehtods
        public AnswerKeyFieldControl()
        {
            InitializeComponent();
        }

        public void Initialize(int fieldIndex)
        {
            fieldIndexLabel.Text = $"{fieldIndex}.";
        }
        public int[] GetMarkedOptions() => markedOptionsIndexes.ToArray();
        public void SetOptions(int[] options)
        {
            if (options.Length != OptionsUIList.Count)
                return;

            for (int i = 0; i < options.Length; i++)
            {
                OptionsUIList[i].Checked = options[i] == 1;
            }
        }
        #endregion

        #region Private Methods
        #region Main
        private void SetOrientation(Orientation orientation)
        {
            MainTableLayoutPanel.Controls.Clear();
            MainTableLayoutPanel.RowStyles.Clear();
            MainTableLayoutPanel.ColumnStyles.Clear();

            //orientation = orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
            BrushInfo indexLabelBackroundColor = fieldIndexLabel.BackgroundColor;
            BrushInfo selectionLabelBackroundColor = selectionLabel.BackgroundColor;
            selectionLabel.ForeColor = Color.DodgerBlue;
            switch (orientation)
            {
                case Orientation.Horizontal:
                    Size = new Size(350, 50);
                    MainTableLayoutPanel.Size = new Size(350, 50);

                    MainTableLayoutPanel.ColumnCount = 3;
                    MainTableLayoutPanel.RowCount = 1;

                    MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                    MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45));
                    MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
                    MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                    MainTableLayoutPanel.Controls.Add(fieldIndexLabel);
                    MainTableLayoutPanel.SetColumn(fieldIndexLabel, 0);
                    MainTableLayoutPanel.SetRow(fieldIndexLabel, 0);

                    MainTableLayoutPanel.Controls.Add(fieldSelectionPanel);
                    MainTableLayoutPanel.SetColumn(fieldSelectionPanel, 1);
                    MainTableLayoutPanel.SetRow(fieldSelectionPanel, 0);

                    MainTableLayoutPanel.Controls.Add(optionsTable);
                    MainTableLayoutPanel.SetColumn(optionsTable, 2);
                    MainTableLayoutPanel.SetRow(optionsTable, 0);
                    SetTotalOptions(totalOptions);

                    fieldSelectionPanel.Dock = DockStyle.Fill;
                    fieldIndexLabel.Dock = DockStyle.Fill;
                    optionsTable.Dock = DockStyle.Fill;

                    fieldIndexLabel.BackgroundColor =  new BrushInfo(GradientStyle.Horizontal, indexLabelBackroundColor.GradientColors);
                    //selectionLabel.BackgroundColor = new BrushInfo(GradientStyle.Horizontal, indexLabelBackroundColor.GradientColors);
                    break;
                case Orientation.Vertical:
                    Size = new Size(50, 10);
                    MainTableLayoutPanel.Size = new Size(50, 10);

                    MainTableLayoutPanel.ColumnCount = 1;
                    MainTableLayoutPanel.RowCount = 3;

                    MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                    MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
                    MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                    MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                    MainTableLayoutPanel.Controls.Add(fieldIndexLabel);
                    MainTableLayoutPanel.SetRow(fieldIndexLabel, 0);
                    MainTableLayoutPanel.SetColumn(fieldIndexLabel, 0);

                    MainTableLayoutPanel.Controls.Add(fieldSelectionPanel);
                    MainTableLayoutPanel.SetRow(fieldSelectionPanel, 1);
                    MainTableLayoutPanel.SetColumn(fieldSelectionPanel, 0);

                    MainTableLayoutPanel.Controls.Add(optionsTable);
                    MainTableLayoutPanel.SetRow(optionsTable, 2);
                    MainTableLayoutPanel.SetColumn(optionsTable, 0);
                    SetTotalOptions(totalOptions);

                    optionsTable.Dock = DockStyle.Fill;
                    fieldIndexLabel.Dock = DockStyle.Fill;
                    fieldSelectionPanel.Dock = DockStyle.Fill;

                    fieldIndexLabel.BackgroundColor = new BrushInfo(GradientStyle.Vertical, indexLabelBackroundColor.GradientColors);
                    //selectionLabel.BackgroundColor = new BrushInfo(GradientStyle.Vertical, indexLabelBackroundColor.GradientColors);
                    break;
            }
        }
        private void SetTotalOptions(int count)
        {
            if (totalOptions != count)
            {
                TotalOptions = count;
                return;
            }

            for (int i = 0; i < optionsTable.Controls.Count; i++)
            {
                var cntrl = optionsTable.Controls[i];
                cntrl.Dispose();
            }
            optionsTable.Controls.Clear();
            OptionsUIList.Clear();
            optionsTable.ColumnStyles.Clear();
            optionsTable.RowStyles.Clear();

            float cellSize = 50;
            switch (FieldOrientation)
            {
                case Orientation.Horizontal:
                    optionsTable.RowCount = 1;
                    optionsTable.ColumnCount = count;
                    cellSize = 1 / (float)count * 100;
                    for (int i = 0; i < count; i++)
                    {
                        optionsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, cellSize));
                        var optionUI = GenerateOptionUI($"option_{i + 1}", OptionSize);
                        optionsTable.Controls.Add(optionUI);
                        optionsTable.SetColumn(optionUI, i);
                        OptionsUIList.Add(optionUI);
                    }
                    break;
                case Orientation.Vertical:
                    optionsTable.ColumnCount = 1;
                    optionsTable.RowCount = count;
                    cellSize = 1 / (float)count * 100;
                    for (int i = 0; i < count; i++)
                    {
                        optionsTable.RowStyles.Add(new RowStyle(SizeType.Percent, cellSize));
                        var optionUI = GenerateOptionUI($"option_{i + 1}", OptionSize);
                        optionsTable.Controls.Add(optionUI);
                        optionsTable.SetRow(optionUI, i);
                        OptionsUIList.Add(optionUI);
                        OptionsUIList[i].Size = new Size(optionsTable.Width, (int)cellSize);
                    }
                    break;
            }
            
            for (int i = 0; i < OptionsUIList.Count; i++)
            {
                OptionsUIList[i].ImageCheckBoxSize = GetScaledOptionSize;
                OptionsUIList[i].Dock = DockStyle.Fill;
            }
        }
        private void SetValueType(ValueDataType valueDataType)
        {
            if (optionsValueType != valueDataType)
            {
                OptionsValueType = optionsValueType;
                return;
            }

            SetPostSelectionUI();
        }
        private void SetPostSelectionUI()
        {
            selectionValue = "";
            for (int i = 0; i < markedOptionsIndexes.Count; i++)
            {
                switch (OptionsValueType)
                {
                    case ValueDataType.String:
                        break;
                    case ValueDataType.Text:
                        break;
                    case ValueDataType.Alphabet:
                        selectionValue += (char)(markedOptionsIndexes[i] + 65);
                        break;
                    case ValueDataType.WholeNumber:
                        selectionValue += (char)(markedOptionsIndexes[i] + 48);
                        break;
                    case ValueDataType.NaturalNumber:
                        selectionValue += (char)(markedOptionsIndexes[i] + 49);
                        break;
                    case ValueDataType.Integer:
                        break;
                }
            }
            SelectionValue = selectionValue;
        }
        private CheckBoxAdv GenerateOptionUI(string name, Size size)
        {
            CheckBoxAdv option = new CheckBoxAdv();

            option.Name = name;
            option.Style = CheckBoxAdvStyle.Office2016White;
            option.CheckAlign = ContentAlignment.MiddleCenter;
            option.ImageCheckBox = true;
            option.CheckedImage = Shared.Properties.SharedResources.CustomCheckBox_Checked;
            option.UncheckedImage = Shared.Properties.SharedResources.CustomCheckBox_Unchecked;
            option.MouseOverCheckedImage = Shared.Properties.SharedResources.CustomCheckBox_Checked_MouseOver;
            option.MouseOverUncheckedImage = Shared.Properties.SharedResources.CustomCheckBox_Unchecked_MouseOver;
            option.ImageCheckBoxSize = size;

            option.CheckedChanged += Option_CheckedChanged;

            return option;
        }
        private void OnOptionStateChanged(CheckBoxAdv option, int optionIndex, bool state)
        {
            if (state)
            {
                if(!markedOptionsIndexes.Contains(optionIndex))
                    markedOptionsIndexes.Add(optionIndex);
            }
            else
            {
                if (markedOptionsIndexes.Contains(optionIndex))
                    markedOptionsIndexes.Remove(optionIndex);
            }

            SetPostSelectionUI();
        }
        #endregion

        #region UI
        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            var option = (CheckBoxAdv)sender;
            OnOptionStateChanged(option, OptionsUIList.IndexOf(option), option.Checked);
        }
        private void AnswerKeyFieldControl_SizeChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < OptionsUIList.Count; i++)
            {
                OptionsUIList[i].ImageCheckBoxSize = GetScaledOptionSize;
                OptionsUIList[i].Dock = DockStyle.Fill;
            }
        }

        private void selectionLabel_DoubleClick(object sender, EventArgs e)
        {
            setOptionField.BringToFront();
        }
        private void setOptionField_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                selectionLabel.BringToFront();
            }
        }
        private void setOptionField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                List<int> markedIndexes = new List<int>();
                string _selectionValue = setOptionField.Text;
                string correctedSelectionValue = _selectionValue;
                try
                {
                    for (int i = 0; i < _selectionValue.Length; i++)
                    {
                        int curMarkIndex = 0;
                        switch (OptionsValueType)
                        {
                            case ValueDataType.String:
                                break;
                            case ValueDataType.Text:
                                break;
                            case ValueDataType.Alphabet:
                                curMarkIndex = (int)(_selectionValue[i] - 65);
                                break;
                            case ValueDataType.WholeNumber:
                                curMarkIndex = (int)(_selectionValue[i] - 48);
                                break;
                            case ValueDataType.NaturalNumber:
                                curMarkIndex = (int)(_selectionValue[i] - 49);
                                break;
                            case ValueDataType.Integer:
                                break;
                        }
                        if (markedIndexes.Contains(curMarkIndex))
                        {
                            correctedSelectionValue = correctedSelectionValue.Remove(i, 1);
                            continue;
                        }

                        markedIndexes.Add(curMarkIndex);
                        OptionsUIList[curMarkIndex].Checked = true;
                    }

                    for (int i = 0; i < OptionsUIList.Count; i++)
                    {
                        if (markedIndexes.Contains(i))
                            continue;

                        OptionsUIList[i].Checked = false;
                    }

                    SelectionValue = correctedSelectionValue;
                    markedOptionsIndexes = new List<int>(markedIndexes);

                    selectionLabel.BringToFront();
                }
                catch (Exception ex)
                {
                    setOptionField.Text = selectionValue;
                    for (int i = 0; i < OptionsUIList.Count; i++)
                    {
                        if (markedOptionsIndexes.Contains(i))
                            OptionsUIList[i].Checked = true;
                        else
                            OptionsUIList[i].Checked = false;
                    }
                }
            }
        }
        private void AnswerKeyFieldControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                selectionLabel.BringToFront();
            }
        }
        #endregion

        #endregion
    }
}
