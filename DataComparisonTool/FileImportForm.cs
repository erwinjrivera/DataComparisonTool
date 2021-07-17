using DataComparisonTool.Common;
using DataComparisonTool.Controller;
using DataComparisonTool.Model;
using DataComparisonTool.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataComparisonTool
{
    public partial class FileImportForm : Form, IFileImportView
    {
        private ImportFileController _controller;

        private string[] _fileNames;

        public FileImportForm(DataSet existingDataSet, string[] fileNames)
        {
            InitializeComponent();

            _fileNames = fileNames;
            _controller = new ImportFileController(this, existingDataSet, fileNames);
        }

        private void ImportFileForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            
            dataGridView1.Visible = false;

            this.Height = 170;

            _controller.Initialize();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            //_controller.ImportFile();
            //_controller.ImportFile(_fileNames);
        }

        public string FormText
        {
            set
            {
                this.Text = value;
            }
        }

        public void ShowOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls|Text Documents|*.txt|CSV (Comma delimited)|*.csv|All Files|*.*";
            openFileDialog.DefaultExt = ".xlsx";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                    _controller.ReadFile(fileName);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        public object Data
        {
            get
            {
                return dataGridView1.DataSource;
            }
        }

        public List<ImportSuggestion> Suggestions
        {
            set
            {
                if (value != null)
                {
                    dataGridView1.Visible = true;
                    dataGridView1.DataSource = value;

                    DataGridViewTextBoxColumn tableColumn = new DataGridViewTextBoxColumn();
                    tableColumn.Name = "Table";
                    tableColumn.DataPropertyName = "Name";
                    tableColumn.ReadOnly = true;

                    DataGridViewComboBoxColumn importActionsColumn = new DataGridViewComboBoxColumn();
                    importActionsColumn.Name = "ImportAction";
                    importActionsColumn.HeaderText = "Import Action";

                    dataGridView1.Columns.Clear();
                    dataGridView1.Columns.Add(tableColumn);
                    dataGridView1.Columns.Add(importActionsColumn);

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        var item = (ImportSuggestion)row.DataBoundItem;

                        ((DataGridViewComboBoxCell)row.Cells["ImportAction"]).DataSource = item.AvailableImportActions;
                        ((DataGridViewComboBoxCell)row.Cells["ImportAction"]).Value = "Skip";
                    }

                    tableColumn.Width = 150;

                    dataGridView1.Refresh();

                    this.Height = 270;
                }
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 1 && e.Control is ComboBox)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged -= LastColumnComboSelectionChanged;
                comboBox.SelectedIndexChanged += LastColumnComboSelectionChanged;
            }
        }

        private void LastColumnComboSelectionChanged(object sender, EventArgs e)
        {
            var currentcell = dataGridView1.CurrentCellAddress;
            var sendingCB = sender as DataGridViewComboBoxEditingControl;

            var row = dataGridView1.Rows[currentcell.Y];

            ((ImportSuggestion)row.DataBoundItem).SelectedImportAction = sendingCB.EditingControlFormattedValue.ToString();
            //DataGridViewTextBoxCell cel = (DataGridViewTextBoxCell)dataGridView1.Rows[currentcell.Y].Cells[0];
            //cel.Value = sendingCB.EditingControlFormattedValue.ToString();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("-------------------------------");

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var item = (ImportSuggestion)row.DataBoundItem;

                Debug.WriteLine($"Name: {item.Name}");
                Debug.WriteLine($"Orignial name: {item.OriginalTableName}");
                Debug.WriteLine($"Available import actions: {item.AvailableImportActions}");
                Debug.WriteLine($"Selected import action: {item.SelectedImportAction}");
            }
        }

        public string FileName
        {
            set
            {
                txtFileName.Text = value;
            }
        }

        private void txtFileName_Click(object sender, EventArgs e)
        {
            _controller.ImportFile();
        }

        public string ViewText
        {
            set
            {
                this.Text = value;
            }
        }

        public bool CancelButtonEnabled
        {
            set
            {
                btnCancel.Visible = value;
                btnCancel.Enabled = value;
            }
        }

        public bool OkButtonEnabled
        {
            set
            {
                btnOk.Visible = value;
                btnOk.Enabled = value;
            }
        }

        public string Status
        {
            set
            {
                lblStatus.Text = value;
            }
        }

        public bool GroupBoxGridVisible
        {
            set
            {
                dataGridView1.Visible = value;
            }
        }

        public bool StatusVisible
        {
            set
            {
                lblStatus.Visible = value;
            }
        }











        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
