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
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataComparisonTool
{
    public partial class MainForm : Form, IMainView
    {
        private MainController _controller;

        private CustomMessageBox _customMessageBox = new CustomMessageBox();

        private UserControlReport userControlReport1;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _controller = new MainController(this, new Map());
            _controller.Initialize();

            userControlReport1 = new UserControlReport(this);

            this.userControlReport1.AutoScroll = true;
            this.userControlReport1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.userControlReport1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlReport1.Location = new System.Drawing.Point(3, 3);
            this.userControlReport1.Name = "userControlReport1";
            this.userControlReport1.Size = new System.Drawing.Size(994, 466);
            this.userControlReport1.TabIndex = 0;

            tabPage4.Controls.Add(userControlReport1);

            lblStatus.Text = string.Empty;
        }

        public void InitializeDataGridViews()
        {
            InitilizeSourceDataGridView();
            InitilizeTargetDataGridView();
            InitilizeMappingFieldsDataGridView();
        }

        private void InitilizeSourceDataGridView()
        {
            // source fields
            dataGridViewSourceFields.AutoGenerateColumns = false;

            var imgSourceMappedColumn = new DataGridViewImageColumn();
            imgSourceMappedColumn.Name = "MappedColumn";
            imgSourceMappedColumn.HeaderText = "Mapped";
            //imgSourceMappedColumn.DataPropertyName = "Mapped";
            imgSourceMappedColumn.ReadOnly = true;
            imgSourceMappedColumn.Image = imageList1.Images["blank"];

            var txtPositionColumn = new DataGridViewTextBoxColumn();
            txtPositionColumn.Name = "PositionColumn";
            txtPositionColumn.HeaderText = "Pos";
            txtPositionColumn.DataPropertyName = "SourcePosition";
            txtPositionColumn.ReadOnly = true;

            var txtSourceColumn = new DataGridViewTextBoxColumn();
            txtSourceColumn.Name = "SourceColumn";
            txtSourceColumn.HeaderText = "Source Column";
            txtSourceColumn.DataPropertyName = "Source";
            txtSourceColumn.ReadOnly = true;

            var txtTargetColumn = new DataGridViewTextBoxColumn();
            txtTargetColumn.Name = "TargetColumn";
            txtTargetColumn.HeaderText = "Target Column";
            txtTargetColumn.DataPropertyName = "Target";
            txtTargetColumn.ReadOnly = true;

            dataGridViewSourceFields.Columns.Add(imgSourceMappedColumn);
            dataGridViewSourceFields.Columns.Add(txtPositionColumn);
            dataGridViewSourceFields.Columns.Add(txtSourceColumn);
            dataGridViewSourceFields.Columns.Add(txtTargetColumn);

            imgSourceMappedColumn.Width = 50;
            txtPositionColumn.Width = 35;

            dataGridViewSourceFields.DataSource = null;
        }

        private void InitilizeTargetDataGridView()
        {
            // source fields
            dataGridViewTargetFields.AutoGenerateColumns = false;

            var imgSourceMappedColumn = new DataGridViewImageColumn();
            imgSourceMappedColumn.Name = "MappedColumn";
            imgSourceMappedColumn.HeaderText = "Mapped";
            //imgSourceMappedColumn.DataPropertyName = "Mapped";
            imgSourceMappedColumn.ReadOnly = true;
            imgSourceMappedColumn.Image = imageList1.Images["blank"];

            var txtPositionColumn = new DataGridViewTextBoxColumn();
            txtPositionColumn.Name = "PositionColumn";
            txtPositionColumn.HeaderText = "Pos";
            txtPositionColumn.DataPropertyName = "SourcePosition";
            txtPositionColumn.ReadOnly = true;

            var txtSourceColumn = new DataGridViewTextBoxColumn();
            txtSourceColumn.Name = "SourceColumn";
            txtSourceColumn.HeaderText = "Source Column";
            txtSourceColumn.DataPropertyName = "Source";
            txtSourceColumn.ReadOnly = true;

            var txtTargetColumn = new DataGridViewTextBoxColumn();
            txtTargetColumn.Name = "TargetColumn";
            txtTargetColumn.HeaderText = "Target Column";
            txtTargetColumn.DataPropertyName = "Target";
            txtTargetColumn.ReadOnly = true;

            dataGridViewTargetFields.Columns.Add(imgSourceMappedColumn);
            dataGridViewTargetFields.Columns.Add(txtPositionColumn);
            dataGridViewTargetFields.Columns.Add(txtSourceColumn);
            dataGridViewTargetFields.Columns.Add(txtTargetColumn);

            imgSourceMappedColumn.Width = 60;
            txtPositionColumn.Width = 35;

            dataGridViewTargetFields.DataSource = null;
        }

        private void InitilizeMappingFieldsDataGridView()
        {
            // source fields
            dataGridViewMatchingFields.AutoGenerateColumns = false;

            var imgSourceMappedColumn = new DataGridViewCheckBoxColumn();
            imgSourceMappedColumn.Name = "Key";
            imgSourceMappedColumn.HeaderText = "";
            imgSourceMappedColumn.DataPropertyName = "IsKey";

            var txtPositionColumn = new DataGridViewTextBoxColumn();
            txtPositionColumn.Name = "MappedFieldSource";
            txtPositionColumn.HeaderText = "Mapped Field (Source)";
            txtPositionColumn.DataPropertyName = "Source";
            txtPositionColumn.ReadOnly = true;

            var txtSourceColumn = new DataGridViewTextBoxColumn();
            txtSourceColumn.Name = "MappedFieldTarget";
            txtSourceColumn.HeaderText = "Mapped Field (Target)";
            txtSourceColumn.DataPropertyName = "Target";
            txtSourceColumn.ReadOnly = true;


            dataGridViewMatchingFields.Columns.Add(imgSourceMappedColumn);
            dataGridViewMatchingFields.Columns.Add(txtPositionColumn);
            dataGridViewMatchingFields.Columns.Add(txtSourceColumn);

            imgSourceMappedColumn.Width = 60;

            dataGridViewMatchingFields.DataSource = null;
        }

        private void InitilizeSourceRecordsDataGridView()
        {
            // source fields
            //dataGridViewSourceRecords.DefaultCellStyle.SelectionForeColor = 
        }

        private void sourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //_controller.OpenFile("source");

            _controller.ImportSourceFiles();
        }

        private void targetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //_controller.OpenFile("target");

            _controller.ImportTargetFiles();
        }

        public string[] ShowOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls|Text Documents|*.txt|CSV (Comma delimited)|*.csv|All Files|*.*";
            openFileDialog.DefaultExt = ".xlsx";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileNames;
            }

            return null;
        }

        public object ShowImportFileDialog(Form form)
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                return ((FileImportForm)form).Data;
            }

            return null;
        }



        public void AddTreeNodeToSource(string name, string details)
        {
            treeView1.Nodes[0].Nodes.Add(new TreeNode
            {
                Name = name,
                Text = details
            });

            if (!treeView1.Nodes[0].IsExpanded)
                treeView1.ExpandAll();

            treeView1.Refresh();
        }

        public void RefreshSourceCount()
        {
            string count = String.Format("{0:N0} {1}",
                treeView1.Nodes[0].Nodes.Count,
                treeView1.Nodes[0].Nodes.Count > 1 ? "Tables" : "Table");

            treeView1.Nodes[0].Text = $"Source ({count})";
            treeView1.Refresh();
        }

        public void UpdateTreeNodeToSource(string name, string details)
        {
            TreeNode[] node = treeView1.Nodes[0].Nodes.Find(name, false);

            if (node != null)
            {
                node[0].Text = details;
            }

            treeView1.Refresh();
        }

        public void AddItemToSourceList(string name)
        {
            cbSource.Items.Add(name);
        }

        public void AddTreeNodeToTarget(string name, string details)
        {
            treeView2.Nodes[0].Nodes.Add(new TreeNode
            {
                Name = name,
                Text = details
            });

            if (!treeView2.Nodes[0].IsExpanded)
                treeView2.ExpandAll();

            treeView2.Refresh();
        }

        public void RefreshTargetCount()
        {
            string count = String.Format("{0:N0} {1}",
                treeView2.Nodes[0].Nodes.Count,
                treeView2.Nodes[0].Nodes.Count > 1 ? "Tables" : "Table");

            treeView2.Nodes[0].Text = $"Target ({count})";
            treeView2.Refresh();
        }

        public void UpdateTreeNodeToTarget(string name, string details)
        {
            TreeNode[] node = treeView2.Nodes[0].Nodes.Find(name, false);

            if (node != null)
            {
                node[0].Text = details;
            }

            treeView2.Refresh();
        }

        public void AddItemToTargetList(string name)
        {
            cbTarget.Items.Add(name);
        }

        public int SelectedTab
        {
            get
            {
                return tabControl1.SelectedIndex;
            }
        }

        private void cbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string tableName = cbSource.SelectedItem != null ? cbSource.SelectedItem.ToString() : string.Empty;

            //_controller.UpdateSourcePreview();
            //_controller.ReadSourceColumns();

            _controller.ChangeSourceTable();

            SelectedSourceTable = cbSource.SelectedItem.ToString();
        }

        private void cbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string tableName = cbTarget.SelectedItem != null ? cbTarget.SelectedItem.ToString() : string.Empty;

            //_controller.UpdateTargetPreview();
            //_controller.ReadTargetColumns();

            _controller.ChangeTargetTable();

            SelectedTargetTable = cbTarget.SelectedItem.ToString();
        }

        public object SourcePreviewDataSource
        {
            set
            {
                dataGridViewSourcePreview.DataSource = value;
            }
        }

        public object TargetPreviewDataSource
        {
            set
            {
                dataGridViewTargetPreview.DataSource = value;
            }
        }

        public object SourceRecordsDataSource
        {
            set
            {
                dataGridViewSourceRecords.DataSource = value;

                if (value == null)
                    return;

                if (dataGridViewSourceRecords.Columns != null && dataGridViewSourceRecords.Columns.Count > 1)
                {
                    dataGridViewSourceRecords.Columns[0].Width = 95;
                    dataGridViewSourceRecords.Columns[1].Width = 95;

                    foreach (DataGridViewRow column in dataGridViewSourceRecords.Rows)
                    {
                        try
                        {
                            if (column.Cells[0].Value != null)
                            {
                                string matchType = column.Cells[0].Value.ToString();

                                switch (matchType)
                                {
                                    case "None":
                                        column.Cells[0].Style.ForeColor = Color.FromArgb(249, 61, 60);
                                        break;
                                    case "Single":
                                        column.Cells[0].Style.ForeColor = Color.FromArgb(81, 169, 81);
                                        break;
                                    case "Multiple":
                                        column.Cells[0].Style.ForeColor = Color.FromArgb(49, 49, 248);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                    dataGridViewSourceRecords.Refresh();
                }

                dataGridViewSourceRecords.Columns[dataGridViewSourceRecords.ColumnCount - 1].Visible = false;
            }
        }


        public string SelectedSourceTable { get; set; }


        public string SelectedTargetTable { get; set; }

        public string GetSelectedSourceTable()
        {
            return cbSource.SelectedItem != null ? cbSource.SelectedItem.ToString() : string.Empty;
        }

        public string GetSelectedTargetTable()
        {
            return cbTarget.SelectedItem != null ? cbTarget.SelectedItem.ToString() : string.Empty;
        }

        public object SourceColumnsDataSource
        {
            set
            {
                dataGridViewSourceFields.DataSource = value;
                //dataGridViewSourceFields.Invalidate();
                //dataGridViewSourceFields.Refresh();

                if (value != null)
                {
                    dataGridViewSourceFields.Columns[0].Width = 60;
                }
            }
        }

        public object TargetColumnsDataSource
        {
            set
            {
                dataGridViewTargetFields.DataSource = value;
                //dataGridViewTargetFields.Invalidate();
                //dataGridViewTargetFields.Refresh();

                if (value != null)
                {
                    dataGridViewTargetFields.Columns[0].Width = 60;
                }
            }
        }

        public object FieldPerFieldsDataSource
        {
            set
            {
                //dataGridViewFieldPerField.DataSource = value;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            Map map = new Map();
            FieldMap fieldMapping = new FieldMap();

            MessageBox.Show(map.IsDirty.ToString());

            map.Add(fieldMapping);

            MessageBox.Show(map.IsDirty.ToString());

            fieldMapping.Source = new Column();

            MessageBox.Show(map.IsDirty.ToString());
            */
            //dataGridViewSourceFields.Columns[0].Width = 50;

            //_controller.CountMatchingRecords();

            _controller.CountDuplicateSourceRecords();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _controller.UpdateTabs();
        }

        private DataGridViewRow FindDataGridViewRow(DataGridView dataGridView, Field field)
        {
            if (dataGridView.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells[2].Value.ToString() == field.ColumnName) // SourceColumn
                    {
                        return row;
                    }
                }
            }

            return null;
        }

        public void UpdateMapping(FieldMapping source, FieldMapping target, string action)
        {
            lblStatus.Text = "Mapping fields. Please wait...";

            var s = FindDataGridViewRow(dataGridViewSourceFields, source.SourceField); //dataGridViewSourceFields.SelectedRows[0];
            var t = FindDataGridViewRow(dataGridViewTargetFields, target.SourceField); //dataGridViewTargetFields.SelectedRows[0];

            switch (action)
            {
                case "map":
                    s.Cells[3].Value = target.SourceField; // TargetColumn
                    t.Cells[3].Value = source.SourceField; // TargetColumn
                    break;
                case "unmap":
                    s.Cells[3].Value = null; // TargetColumn
                    t.Cells[3].Value = null; // TargetColumn
                    break;
            }

            if (source.TargetField != null)
                s.Cells[0].Value = imageList1.Images[0]; // green dot
            else
                s.Cells[0].Value = imageList1.Images[1]; // blank

            if (target.TargetField != null)
                t.Cells[0].Value = imageList1.Images[0]; // green dot
            else
                t.Cells[0].Value = imageList1.Images[1]; // blank

            //dataGridViewSourceFields.Refresh();
            //dataGridViewTargetFields.Refresh();

            _controller.Mappable();
            _controller.Unmappable();

            lblStatus.Text = string.Empty;
        }

        public int SelectedRowsCount(string dataGridViewName)
        {
            switch (dataGridViewName)
            {
                case "dataGridViewSourceFields":
                    if (dataGridViewSourceFields.Rows.Count > 0)
                        return dataGridViewSourceFields.SelectedRows.Count;
                    break;
                case "dataGridViewTargetFields":
                    if (dataGridViewTargetFields.Rows.Count > 0)
                        return dataGridViewTargetFields.SelectedRows.Count;
                    break;
            }

            return 0;
        }

        public int RowsCount(string dataGridViewName)
        {
            switch (dataGridViewName)
            {
                case "dataGridViewSourceFields":
                    return dataGridViewSourceFields.Rows.Count;
                case "dataGridViewTargetFields":
                    return dataGridViewTargetFields.Rows.Count;
            }

            return 0;
        }

        private void dataGridViewSourceFields_SelectionChanged(object sender, EventArgs e)
        {
            _controller.Mappable();
            _controller.Unmappable();

            if (dataGridViewSourceFields.Rows.Count > 0 && dataGridViewSourceFields.SelectedRows.Count > 0)
            {
                if (dataGridViewTargetFields.Rows.Count > 0)
                {
                    try
                    {
                        var s = (FieldMapping)dataGridViewSourceFields.SelectedRows[0].DataBoundItem;

                        foreach (DataGridViewRow row in dataGridViewTargetFields.Rows)
                        {
                            var t = (FieldMapping)row.DataBoundItem;

                            if (s.Mapped && t.Mapped && s.Target == t.Source)
                            {
                                //dataGridViewTargetFields.SelectedRows[0].Selected = false;
                                row.Selected = true;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void dataGridViewTargetFields_SelectionChanged(object sender, EventArgs e)
        {
            _controller.Mappable();
            _controller.Unmappable();

            if (dataGridViewTargetFields.Rows.Count > 0 && dataGridViewTargetFields.SelectedRows.Count > 0)
            {
                if (dataGridViewSourceFields.Rows.Count > 0)
                {
                    try
                    {
                        var s = (FieldMapping)dataGridViewTargetFields.SelectedRows[0].DataBoundItem;

                        foreach (DataGridViewRow row in dataGridViewSourceFields.Rows)
                        {
                            var t = (FieldMapping)row.DataBoundItem;

                            if (s.Mapped && t.Mapped && s.Target == t.Source)
                            {
                                //dataGridViewSourceFields.SelectedRows[0].Selected = false;
                                row.Selected = true;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public bool MapButtonEnabled
        {
            set
            {
                btnMap.Enabled = value;
            }
        }

        public bool UnmapButtonEnabled
        {
            set
            {
                btnUnmap.Enabled = value;
            }
        }

        public object SelectedSourceObject
        {
            get
            {
                if (dataGridViewSourceFields.Rows.Count > 0)
                    return dataGridViewSourceFields.SelectedRows[0].DataBoundItem;

                return null;
            }
        }

        public object SelectedTargetObject
        {
            get
            {
                if (dataGridViewTargetFields.Rows.Count > 0)
                    return dataGridViewTargetFields.SelectedRows[0].DataBoundItem;

                return null;
            }
        }

        public bool ConfirmChangeTab()
        {
            return MessageBox.Show("You are attempting to change the active source or target table. All field mapping details will be lost.\nDo you want to proceed?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            if (dataGridViewSourceFields.SelectedRows.Count > 0 && dataGridViewTargetFields.SelectedRows.Count > 0)
                _controller.Map(dataGridViewSourceFields.SelectedRows[0].DataBoundItem, dataGridViewTargetFields.SelectedRows[0].DataBoundItem);
        }

        private void btnUnmap_Click(object sender, EventArgs e)
        {
            if (dataGridViewSourceFields.SelectedRows.Count > 0 && dataGridViewTargetFields.SelectedRows.Count > 0)
                _controller.Unmap(dataGridViewSourceFields.SelectedRows[0].DataBoundItem, dataGridViewTargetFields.SelectedRows[0].DataBoundItem);
        }

        private void btnAutomap_Click(object sender, EventArgs e)
        {
            

            Thread.Sleep(100);
            /*
            if (dataGridViewSourceFields.Rows.Count > 0 && dataGridViewTargetFields.Rows.Count > 0)
            {
                foreach (DataGridViewRow rowSource in dataGridViewSourceFields.Rows)
                {
                    foreach (DataGridViewRow rowTarget in dataGridViewTargetFields.Rows)
                    {
                        _controller.AutoMap(rowSource.DataBoundItem, rowTarget.DataBoundItem);
                    }
                }
            }
            */

            _controller.AutoMap();
        }

        private void dataGridViewSourceFields_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //_controller.Mappable();
            //_controller.Unmappable();
        }

        private void dataGridViewTargetFields_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //_controller.Mappable();
            //_controller.Unmappable();
        }

        public void ResetSelectedSourceTable()
        {
            cbSource.SelectedItem = SelectedSourceTable;
        }

        public void ResetSelectedTargetTable()
        {
            cbTarget.SelectedItem = SelectedTargetTable;
        }

        public object MatchingFieldsDataSource
        {
            set
            {
                dataGridViewMatchingFields.DataSource = value;
            }
        }

        private void dataGridViewMatchingFields_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            /*
            if (dataGridViewMatchingFields.CurrentCell.ColumnIndex == 0 && e.Control is CheckBox)
            {
                CheckBox checkBox = e.Control as CheckBox;
                checkBox.CheckedChanged -= ColumnCheckedChanged;
                checkBox.CheckedChanged += ColumnCheckedChanged;
            }
            */
        }

        private void ColumnCheckedChanged(object sender, EventArgs e)
        {
            /*
            var currentcell = dataGridViewMatchingFields.CurrentCellAddress;
            var sendingCB = sender as DataGridViewChec;

            var row = dataGridViewMatchingFields.Rows[currentcell.Y];

            try
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            */

            //((ImportSuggestion)row.DataBoundItem).SelectedImportAction = sendingCB.EditingControlFormattedValue.ToString();
            //DataGridViewTextBoxCell cel = (DataGridViewTextBoxCell)dataGridView1.Rows[currentcell.Y].Cells[0];
            //cel.Value = sendingCB.EditingControlFormattedValue.ToString();
        }

        private void dataGridViewMatchingFields_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewMatchingFields.CommitEdit(DataGridViewDataErrorContexts.Commit);

            /*
            if (dgvApps.CurrentCell.GetType() == typeof(DataGridViewCheckBoxCell))
            {
                if (dgvApps.CurrentCell.IsInEditMode)
                {
                    if (dgvApps.IsCurrentCellDirty)
                    {
                        dgvApps.EndEdit();
                    }
                }
            }
            */
        }

        private void dataGridViewMatchingFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewMatchingFields.DataSource != null)
            {


                if (dataGridViewMatchingFields.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "True")
                {
                    //((FieldMapping)dataGridViewMatchingFields.Rows[e.RowIndex].DataBoundItem).Source
                }
                else
                {
                    //do something
                }

                _controller.UpdateIsKey(dataGridViewMatchingFields.Rows[e.RowIndex].DataBoundItem);
            }
        }

        private void dataGridViewSourceRecords_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSourceRecords.DataSource != null)
            {
                if (dataGridViewSourceRecords.SelectedRows != null)
                {
                    try
                    {
                        List<object> values = new List<object>();

                        foreach (DataGridViewRow row in dataGridViewSourceRecords.SelectedRows)
                        {
                            values.Add(row.Cells[dataGridViewSourceRecords.ColumnCount - 1].Value);
                        }

                        _controller.DisplayMatchingRecords(values.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public object TargetRecordsDataSource
        {
            set
            {
                dataGridViewTargetRecords.DataSource = value;
            }
        }

        public void HighlightSourceKeyFields(string[] columnNames)
        {
            if (dataGridViewSourceRecords.DataSource != null)
            {
                if (dataGridViewSourceRecords.Columns != null)
                {
                    foreach (DataGridViewColumn c in dataGridViewSourceRecords.Columns)
                    {
                        c.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
                    }

                    if (columnNames != null && columnNames.Length > 0)
                    {
                        try
                        {
                            foreach (string columnName in columnNames)
                            {
                                DataGridViewColumn column = dataGridViewSourceRecords.Columns[columnName];

                                if (column != null)
                                {
                                    column.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

                                    //column.CellTemplate.Style.ForeColor = Color.FromArgb(81, 169, 81);
                                    //column.CellTemplate.Style.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _controller.RemoveDuplicateSourceRecords();
        }

        private void btnApplyRefreshMatching_Click(object sender, EventArgs e)
        {
            _controller.ApplyRefreshMatching();
        }

        public bool ApplyRefreshMatchingButtonEnabled
        {
            set
            {
                btnApplyRefreshMatching.Enabled = value;
            }
        }

        public void DisplayCounts(int none, int single, int multiple)
        {
            dataGridViewCounts.Rows.Clear();

            dataGridViewCounts.Rows.Add(new object[] { String.Format("{0:N0}", none), "None" });
            dataGridViewCounts.Rows.Add(new object[] { String.Format("{0:N0}", single), "Single" });
            dataGridViewCounts.Rows.Add(new object[] { String.Format("{0:N0}", multiple), "Multiple" });
        }

        public void ShowCustomMessageBox(string message, string caption)
        {
            _customMessageBox = new CustomMessageBox();
            _customMessageBox.Message = message;
            //_customMessageBox.Caption = caption;

            _customMessageBox.ShowDialog();
        }

        public void CloseCustomMessageBox()
        {
            _customMessageBox.Close();
        }

        public string StatusMessage
        {
            set
            {
                lblStatus.Text = value;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            _controller.StartCompare();
        }

        public void ShowMessageBoxOk(string message, string caption)
        {
            MessageBox.Show(message,
                caption,
                MessageBoxButtons.OK,
                caption.Contains("Error") ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        public bool ShowMessageYesNoBox(string message, string caption)
        {
            return MessageBox.Show(message,
                caption,
                MessageBoxButtons.YesNo,
                caption.Contains("Error") ? MessageBoxIcon.Error : MessageBoxIcon.Information) == DialogResult.Yes;
        }

        public void ShowReport(ReportData report)
        {
            var reportUserControl = ((UserControlReport)tabPage4.Controls[0]).Data = report;
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurationForm config = new ConfigurationForm();

            config.ShowDialog();
        }
    }
}
