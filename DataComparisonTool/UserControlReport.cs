using DataComparisonTool.Model;
using DataComparisonTool.Utils;
using DataComparisonTool.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataComparisonTool
{
    public partial class UserControlReport : UserControl
    {
        private ReportData _data = new ReportData();

        private string _sourceTableName;

        private string _targetTableName;

        private BackgroundWorker reportWorker;

        private BackgroundWorker commonRecordsWorker;

        private BackgroundWorker duplicateSourceWorker;

        private BackgroundWorker duplicateTargetWorker;

        private BackgroundWorker nonMatchingValuesWorker;

        private IMainView _view;

        public UserControlReport()
        {
            InitializeComponent();
        }

        public UserControlReport(IMainView view)
        {
            InitializeComponent();

            _view = view;
        }

        public UserControlReport(ReportData data)
        {
            InitializeComponent();

            _data = data;

            _sourceTableName = _data.SourceTableName;
            _targetTableName = _data.TargetTableName;

            DisplayData();
        }

        public ReportData Data
        {
            set
            {
                if (value == null)
                {
                    _sourceTableName = null;
                    _targetTableName = null;

                    ClearData();

                    return;
                }

                _data = value;

                _sourceTableName = _data.SourceTableName;
                _targetTableName = _data.TargetTableName;

                DisplayData();
            }
        }

        private void ClearData()
        {
            lblMappedFieldsSource.Text = string.Empty;
            lblMappedFieldsTarget.Text = string.Empty;
            lblFieldsNoMapSource.Text = string.Empty;
            lblFieldsNoMapTarget.Text = string.Empty;
            lblKeyFieldsCount.Text = string.Empty;
            lblNoneCount.Text = string.Empty;
            lblSingleCount.Text = string.Empty;
            lblMultipleCount.Text = string.Empty;
            lblSourceNotInTarget.Text = string.Empty;
            lblTargetNonInSource.Text = string.Empty;
            lblCommonRecords.Text = string.Empty;
            lblDuplicateRecordsSource.Text = string.Empty;
            lblDuplicateRecordsTarget.Text = string.Empty;
            linkLabel6.Text = string.Empty;
            lblFieldsCompared.Text = string.Empty;
            lblNonMatchingValues.Text = string.Empty;
            lblRecordCountSource.Text = string.Empty;
            lblRecordCountTarget.Text = string.Empty;
            lblFieldCountSource.Text = string.Empty;
            lblFieldCountTarget.Text = string.Empty;
            lblSourceTableName.Text = string.Empty;
            lblTargetTableName.Text = string.Empty;
            lblFieldPerFieldSummary.Text = string.Empty;
            lblDuplicateHandling.Text = string.Empty;
        }

        private void UserControlReport_Load(object sender, EventArgs e)
        {
            reportWorker = new System.ComponentModel.BackgroundWorker();
            reportWorker.WorkerReportsProgress = true;
            reportWorker.WorkerSupportsCancellation = true;
            reportWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ReportWorker_DoWork);
            reportWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ReportWorker_ProgressChanged);
            reportWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ReportWorker_RunWorkerCompleted);

            commonRecordsWorker = new System.ComponentModel.BackgroundWorker();
            commonRecordsWorker.WorkerReportsProgress = true;
            commonRecordsWorker.WorkerSupportsCancellation = true;
            commonRecordsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.CommonRecordsWorker_DoWork);
            commonRecordsWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.CommonRecordsWorker_ProgressChanged);
            commonRecordsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.CommonRecordsWorker_RunWorkerCompleted);

            duplicateSourceWorker = new System.ComponentModel.BackgroundWorker();
            duplicateSourceWorker.WorkerReportsProgress = true;
            duplicateSourceWorker.WorkerSupportsCancellation = true;
            duplicateSourceWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DuplicateSourceWorker_DoWork);
            duplicateSourceWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DuplicateSourceWorker_ProgressChanged);
            duplicateSourceWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DuplicateSourceWorker_RunWorkerCompleted);

            duplicateTargetWorker = new System.ComponentModel.BackgroundWorker();
            duplicateTargetWorker.WorkerReportsProgress = true;
            duplicateTargetWorker.WorkerSupportsCancellation = true;
            duplicateTargetWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DuplicateTargetWorker_DoWork);
            duplicateTargetWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.DuplicateTargetWorker_ProgressChanged);
            duplicateTargetWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DuplicateTargetWorker_RunWorkerCompleted);

            nonMatchingValuesWorker = new System.ComponentModel.BackgroundWorker();
            nonMatchingValuesWorker.WorkerReportsProgress = true;
            nonMatchingValuesWorker.WorkerSupportsCancellation = true;
            nonMatchingValuesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.NonMatchingValuesWorker_DoWork);
            nonMatchingValuesWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.NonMatchingValuesWorker_ProgressChanged);
            nonMatchingValuesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.NonMatchingValuesWorker_RunWorkerCompleted);
        }

        private void dataGridViewFieldPerField_DataSourceChanged(object sender, EventArgs e)
        {
            //dataGridViewFieldPerField.Height = dataGridViewFieldPerField.RowCount * 15;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DisplayData()
        {
            if (_data == null)
                return;

            int recordCountSource = 0;
            int recordCountTarget = 0;
            int fieldCountSource = 0;
            int fieldCountTarget = 0;

            lblSourceTableName.Text = _sourceTableName;
            lblTargetTableName.Text = _targetTableName;

            // record count
            try
            {
                recordCountSource = _data.SourceDataSet.Tables[_sourceTableName].Rows.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Report error. " + ex.Message);
            }
            finally
            {
                lblRecordCountSource.Text = String.Format("{0:N0}", recordCountSource);
            }

            try
            {
                recordCountTarget = _data.TargetDataSet.Tables[_targetTableName].Rows.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Report error. " + ex.Message);
            }
            finally
            {
                lblRecordCountTarget.Text = String.Format("{0:N0}", recordCountTarget);
            }


            // field count
            try
            {
                fieldCountSource = _data.TargetDataSet.Tables[_targetTableName].Columns.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Report error. " + ex.Message);
            }
            finally
            {
                lblFieldCountSource.Text = String.Format("{0:N0}", fieldCountSource);
            }

            try
            {
                fieldCountTarget = _data.TargetDataSet.Tables[_targetTableName].Columns.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Report error. " + ex.Message);
            }
            finally
            {
                lblFieldCountTarget.Text = String.Format("{0:N0}", fieldCountTarget);
            }

            // mapped fields
            lblMappedFieldsSource.Text = String.Format("{0:N0}", GetMappedSourceFields().Count);
            lblMappedFieldsTarget.Text = String.Format("{0:N0}", GetMappedTargetFields().Count);

            // fields without mapping
            lblFieldsNoMapSource.Text = String.Format("{0:N0}", GetSourceFieldsWithNoMapping().Count);
            lblFieldsNoMapTarget.Text = String.Format("{0:N0}", GetTargetFieldsWithNoMapping().Count);

            // match summary
            lblKeyFieldsCount.Text = String.Format("{0:N0}", GetKeyFields().Count);
            DisplayMatchSummary();

            // records found in A but not in B,  vice-versa
            lblSourceNotInTarget.Text = String.Format("{0:N0}", GetRecordsFoundInSourceButNotInTarget().Count);
            lblTargetNonInSource.Text = String.Format("{0:N0}", GetRecordsFoundInTargetButNotInSource().Count);
            CountCommonRecords();

            // duplicate stuff
            CountDuplicateSourceRecords();
            CountDuplicateTargetRecords();

            // fields compared
            lblFieldsCompared.Text = String.Format("{0:N0}", GetFieldsCompared().Count);

            // non-matching values
            CountNonMatchingValues();

            // field-per-field summary
            lblFieldPerFieldSummary.Text = "See report";

            // duplicate handling
            lblDuplicateHandling.Text = Properties.Settings.Default.DefaultDuplicateRecordHandling;
        }





        





        public void CountCommonRecords()
        {
            if (!commonRecordsWorker.IsBusy)
            {
                commonRecordsWorker.RunWorkerAsync(0);
            }
            else
            {

            }
        }

        public void CountDuplicateSourceRecords()
        {
            if (!duplicateSourceWorker.IsBusy)
            {
                duplicateSourceWorker.RunWorkerAsync(0);
            }
            else
            {

            }
        }

        public void CountDuplicateTargetRecords()
        {
            if (!duplicateTargetWorker.IsBusy)
            {
                duplicateTargetWorker.RunWorkerAsync(0);
            }
            else
            {

            }
        }

        private void CountNonMatchingValues()
        {
            if (!nonMatchingValuesWorker.IsBusy)
            {
                nonMatchingValuesWorker.RunWorkerAsync(0);
            }
            else
            {

            }
        }

        private List<FieldMapping> GetMappedSourceFields()
        {
            return _data.Mapping.AsEnumerable().Where(x => x.Mapped
                && x.SourceField.TableName == _sourceTableName
                && x.SourceField.DataSetName == "source").ToList<FieldMapping>();
        }

        private List<FieldMapping> GetMappedTargetFields()
        {
            return _data.Mapping.AsEnumerable().Where(x => x.Mapped
                && x.TargetField.TableName == _targetTableName
                && x.TargetField.DataSetName == "target").ToList<FieldMapping>();
        }
        private List<FieldMapping> GetSourceFieldsWithNoMapping()
        {
            return _data.Mapping.AsEnumerable().Where(x => !x.Mapped
                && !x.IsKey
                && x.SourceField.TableName == _sourceTableName
                && x.SourceField.DataSetName == "source").ToList<FieldMapping>();
        }

        private List<FieldMapping> GetTargetFieldsWithNoMapping()
        {
            return _data.Mapping.AsEnumerable().Where(x => !x.Mapped
                && !x.IsKey
                && x.SourceField.TableName == _targetTableName
                && x.SourceField.DataSetName == "target").ToList<FieldMapping>();
        }

        private List<FieldMapping> GetKeyFields()
        {
            return _data.Mapping.AsEnumerable().Where(x => x.Mapped
                && x.IsKey
                && x.SourceField.TableName == _sourceTableName
                && x.TargetField.TableName == _targetTableName
                && x.SourceField.DataSetName == "source").ToList<FieldMapping>();
        }

        private List<MatchingResult> GetRecordsFoundInSourceButNotInTarget()
        {
            return _data.MatchResult.Where(x => x.Matches == null || x.Matches.ToList().Count <= 0).ToList<MatchingResult>();
        }

        private List<MatchingResult> GetRecordsFoundInTargetButNotInSource()
        {
            return _data.MatchResultRightTable.Where(x => x.Matches == null || x.Matches.ToList().Count <= 0).ToList<MatchingResult>();
        }

        private List<MatchingResult> GetDuplicateSourceRecords()
        {
            var table = _data.SourceDataSet.Tables[_sourceTableName];
            var keys = GetKeyFields();

            return table.AsEnumerable().GroupBy(x => x, new EqualityComparer(keys))
                .Where(g => g.Count() > 1)
                .SelectMany(x => x.DefaultIfEmpty(table.NewRow()),
                    (x, y) => new MatchingResult { Left = x.Key, Matches = new List<DataRow> { y } })
                        .ToList<MatchingResult>();
        }

        private List<MatchingResult> GetDuplicateTargetRecords()
        {
            var table = _data.TargetDataSet.Tables[_targetTableName];
            var keys = GetKeyFields();

            return table.AsEnumerable().GroupBy(x => x, new EqualityComparer(keys))
                .Where(g => g.Count() > 1)
                .SelectMany(x => x.DefaultIfEmpty(table.NewRow()),
                    (x, y) => new MatchingResult { Left = x.Key, Matches = new List<DataRow> { y } })
                        .ToList<MatchingResult>();
        }

        private void DisplayMatchSummary()
        {
            int noneCount = 0;
            int singleCount = 0;
            int multipleCount = 0;

            foreach (var row in _data.MatchResult)
            {
                int matchCount = 0;

                if (row.Matches != null)
                {
                    matchCount = row.Matches.ToList().Count;
                }

                if (matchCount == 0)
                    noneCount++;
                else if (matchCount == 1)
                    singleCount++;
                else if (matchCount > 1)
                    multipleCount++;
            }

            lblNoneCount.Text = String.Format("{0:N0}", noneCount);
            lblSingleCount.Text = String.Format("{0:N0}", singleCount);
            lblMultipleCount.Text = String.Format("{0:N0}", multipleCount);
        }

        private List<MatchingResult> GetNonMatchingResult()
        {
            return _data.MatchResult.Where(x => x.Matches == null || x.Matches.ToList().Count <= 0).ToList<MatchingResult>();
        }

        private List<MatchingResult> GetSingleMatchingResult()
        {
            return _data.MatchResult.Where(x => x.Matches != null && x.Matches.ToList().Count == 1).ToList<MatchingResult>();
        }

        private List<MatchingResult> GetMultipleMatchingResult()
        {
            return _data.MatchResult.Where(x => x.Matches != null && x.Matches.ToList().Count > 1).ToList<MatchingResult>();
        }

        private List<object> GetCommonRecords()
        {
            var keyFields = GetKeyFields();

            var result = _data.MatchResult
                .Where(x => x.Matches != null
                    && x.Matches.ToList().Count > 0)
                        .Select(x => keyFields.Select(k => x.Left[k.Source].ToString()));

            var factory = new DynamicTypeFactory();
            var dynamicProperties = new List<DynamicProperty>();
            var records = new List<object>();

            foreach (var field in keyFields)
            {
                dynamicProperties.Add(new DynamicProperty
                {
                    PropertyName = StringExtensions.ToCamelCase(field.Source),
                    DisplayName = field.Source,
                    SystemTypeName = "System.String"
                });
            }

            var extendType = factory.CreateNewTypeWithDynamicProperties(typeof(SourceMatch), dynamicProperties);

            foreach (var row in result)
            {
                var extendedObject = Activator.CreateInstance(extendType);

                for (int index = 0; index < dynamicProperties.Count; index++)
                {
                    extendType.GetProperty($"{nameof(DynamicProperty)}_{dynamicProperties[index].PropertyName}").SetValue(extendedObject, row.ToList()[index].ToString(), null);
                }

                records.Add(extendedObject);
            }

            return records;
        }
        
        public List<FieldMapping> GetFieldsCompared()
        {
            return _data.Mapping.Where(x => !x.IsKey
                            && x.Mapped
                            && x.SourceField.TableName == _sourceTableName
                            && x.SourceField.DataSetName == "source").ToList<FieldMapping>();
        }

        public List<ValueMatch> GetNonMatchingValues()
        {
            return _data.ComparisonResult
                .Where(x => x.None > 0)
                    .Select(x => x.Values.Where(i => !i.IsEqual()))
                        .Select(x => x.FirstOrDefault()).ToList();
           
        }














        /*
         * Clicked events
        */

        private void lblFieldCountSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportSourceFields();
        }

        private void lblFieldCountTarget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportTargetFields();
        }

        private void lblFieldsNoMapSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportFieldsWithoutMappingSource();
        }

        private void lblFieldsNoMapTarget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportFieldsWithoutMappingTarget();
        }

        private void lblMappedFieldsSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportMappedSourceFields();
        }

        private void lblMappedFieldsTarget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportMappedTargetFields();
        }

        private void lblKeyFieldsCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportKeyFields();
        }

        private void lblNoneCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportNoneMatches();
        }

        private void lblSingleCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportSingleMatches();
        }
        private void lblMultipleCount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportMultipleMatches();
        }

        private void lblSourceNotInTarget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportRecordsInSourceButNotInTarget();
        }

        private void lblTargetNonInSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportRecordsInTargetButNotInSource();
        }

        private void lblCommonRecords_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportCommonRecords();
        }

        private void lblDuplicateRecordsSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportDuplicateSource();
        }

        private void lblDuplicateRecordsTarget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportDuplicateTarget();
        }

        private void lblFieldsCompared_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportFieldsCompared();
        }

        private void lblNonMatchingValues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportNonMatchingValues();
        }

        private void lblSeeReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReportFieldPerFieldSummary();
        }

        










        /**
         * Reports
         */

        private void ReportSourceFields()
        {
            try
            {
                var sourceColumns = _data.TargetDataSet.Tables[_sourceTableName].Columns;
                StringBuilder sb = new StringBuilder();
                string header = "Table (Source)\tField" + Environment.NewLine;

                sb.Append(header);

                foreach (DataColumn column in sourceColumns)
                {
                    sb.Append($"{_sourceTableName}\t{column.ColumnName}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Report error. " + ex.Message);
            }
        }

        private void ReportTargetFields()
        {
            try
            {
                var targetColumns = _data.SourceDataSet.Tables[_targetTableName].Columns;
                StringBuilder sb = new StringBuilder();
                string header = "Table (Target)\tField" + Environment.NewLine;

                sb.Append(header);

                foreach (DataColumn column in targetColumns)
                {
                    sb.Append($"{_targetTableName}\t{column.ColumnName}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Report error. " + ex.Message);
            }
        }

        private void ReportFieldsWithoutMappingSource()
        {
            var fields = GetSourceFieldsWithNoMapping();

            if (fields != null)
            {
                StringBuilder sb = new StringBuilder();
                string header = "Table (Source)\tField" + Environment.NewLine;

                sb.Append(header);

                foreach (var field in fields)
                {
                    sb.Append($"{field.SourceField.TableName}\t{field.Source}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
        }

        private void ReportFieldsWithoutMappingTarget()
        {
            var fields = GetTargetFieldsWithNoMapping();

            if (fields != null)
            {
                StringBuilder sb = new StringBuilder();
                string header = "Table (Target)\tField" + Environment.NewLine;

                sb.Append(header);

                foreach (var field in fields)
                {
                    sb.Append($"{field.SourceField.TableName}\t{field.Source}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
        }

        private void ReportMappedSourceFields()
        {
            var fields = GetMappedSourceFields();

            if (fields != null)
            {
                StringBuilder sb = new StringBuilder();
                string header = $"From Source ({_sourceTableName})\tMapped to Target ({_targetTableName})" + Environment.NewLine;

                sb.Append(header);

                foreach (var field in fields)
                {
                    sb.Append($"{field.Source}\t{field.Target}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
        }

        private void ReportMappedTargetFields()
        {
            var fields = GetMappedTargetFields();

            if (fields != null)
            {
                StringBuilder sb = new StringBuilder();
                string header = $"From Target ({_targetTableName})\tMapped to Source ({_sourceTableName})" + Environment.NewLine;

                sb.Append(header);

                foreach (var field in fields)
                {
                    sb.Append($"{field.Target}\t{field.Source}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
        }

        private void ReportKeyFields()
        {
            var keyFields = GetKeyFields();

            if (keyFields != null)
            {
                StringBuilder sb = new StringBuilder();
                string header = "Key Field (Source)\tKey Field (Target)" + Environment.NewLine;

                sb.Append(header);

                foreach (var field in keyFields)
                {
                    sb.Append($"{field.Source}\t{field.Target}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
        }

        private void ReportNoneMatches()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(0);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportSingleMatches()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(1);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportMultipleMatches()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(2);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportRecordsInSourceButNotInTarget()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(3);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportRecordsInTargetButNotInSource()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(4);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportCommonRecords()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(5);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportDuplicateSource()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(6);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportDuplicateTarget()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(7);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        private void ReportFieldsCompared()
        {
            var fields = GetFieldsCompared();

            if (fields != null)
            {
                StringBuilder sb = new StringBuilder();
                string header = "Source Field\tCompared To (Target Field)" + Environment.NewLine;

                sb.Append(header);

                foreach (var field in fields)
                {
                    sb.Append($"{field.Source}\t{field.Target}{Environment.NewLine}");
                }

                Notepad.SendText(sb.ToString());
            }
        }

        private void ReportNonMatchingValues()
        {
            if (!reportWorker.IsBusy)
            {
                _view.StatusMessage = "Generating report...";
                reportWorker.RunWorkerAsync(8);
            }
            else
            {
                _view.StatusMessage = "A report is still being generated from a previous request.";
            }
        }

        public void ReportFieldPerFieldSummary()
        {
            if (_data.ComparisonResult != null && _data.ComparisonResult.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                string header = "Field (Source)\tMapped to (Target Field)\tMatched Values\tNon-Matching Values\tMatch %" + Environment.NewLine;

                sb.Append(header);

                foreach (var data in _data.ComparisonResult)
                {
                    sb.Append(data.SourceField.ColumnName);
                    sb.Append("\t");
                    sb.Append(data.TargetField.ColumnName);
                    sb.Append("\t");
                    sb.Append(data.MatchCount);
                    sb.Append("\t");
                    sb.Append(data.NonMatchCount);
                    sb.Append("\t");
                    sb.Append(data.PercentSuccessMatch);
                    sb.Append(Environment.NewLine);
                }

                Notepad.SendText(sb.ToString());
            }
        }











        /**
         * Background Workers
         */

        private void ReportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!reportWorker.CancellationPending)
            {
                try
                {
                    if (e.Argument == null)
                        return;

                    int option = (int)e.Argument;

                    reportWorker.ReportProgress(1);

                    switch (option)
                    {
                        case 0:
                            var nonMatchingResult = GetNonMatchingResult();

                            if (nonMatchingResult != null && nonMatchingResult.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = nonMatchingResult[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var r in nonMatchingResult)
                                {
                                    string row = string.Empty;

                                    foreach (object item in r.Left.ItemArray)
                                    {
                                        if (!string.IsNullOrEmpty(row))
                                            row += "\t";

                                        row += item.ToString();
                                    }

                                    sb.Append(row + Environment.NewLine);
                                }

                                e.Result = sb.ToString();
                            }
                            break;
                        case 1:
                            var singleResult = GetSingleMatchingResult();

                            if (singleResult != null && singleResult.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = singleResult[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var r in singleResult)
                                {
                                    string row = string.Empty;

                                    foreach (object item in r.Matches.ToList()[0].ItemArray)
                                    {
                                        if (!string.IsNullOrEmpty(row))
                                            row += "\t";

                                        row += item.ToString();
                                    }

                                    sb.Append(row + Environment.NewLine);
                                }

                                e.Result = sb.ToString();
                            }
                            break;
                        case 2:
                            var multipleResult = GetMultipleMatchingResult();

                            if (multipleResult != null && multipleResult.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = multipleResult[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var r in multipleResult)
                                {
                                    foreach (DataRow m in r.Matches)
                                    {
                                        string row = string.Empty;

                                        foreach (object item in m.ItemArray)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                                row += "\t";

                                            row += item.ToString();
                                        }

                                        sb.Append(row + Environment.NewLine);
                                    }
                                }

                                e.Result = sb.ToString();
                            }
                            break;
                        case 3:
                            var foundInSource = GetRecordsFoundInSourceButNotInTarget();

                            if (foundInSource != null && foundInSource.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = foundInSource[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var r in foundInSource)
                                {
                                    string row = string.Empty;

                                    foreach (object item in r.Left.ItemArray)
                                    {
                                        if (!string.IsNullOrEmpty(row))
                                            row += "\t";

                                        row += item.ToString();
                                    }

                                    sb.Append(row + Environment.NewLine);
                                }

                                e.Result = sb.ToString();
                            }
                            break;
                        case 4:
                            var foundInTarget = GetRecordsFoundInTargetButNotInSource();

                            if (foundInTarget != null && foundInTarget.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = foundInTarget[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var r in foundInTarget)
                                {
                                    string row = string.Empty;

                                    foreach (object item in r.Left.ItemArray)
                                    {
                                        if (!string.IsNullOrEmpty(row))
                                            row += "\t";

                                        row += item.ToString();
                                    }

                                    sb.Append(row + Environment.NewLine);
                                }

                                e.Result = sb.ToString();
                            }
                            break;
                        case 5:
                            var commonRecords = GetCommonRecords();

                            if (commonRecords != null && commonRecords.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = GetKeyFields();

                                foreach (var column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.Source;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var r in commonRecords)
                                {
                                    string row = string.Empty;
                                    var properties = r.GetType().GetProperties();

                                    foreach (PropertyInfo property in properties)
                                    {
                                        var value = property.GetValue(r);

                                        if (!string.IsNullOrEmpty(row))
                                            row += "\t";

                                        row += (value != null ? value.ToString() : string.Empty);
                                    }

                                    sb.Append(row + Environment.NewLine);
                                }

                                e.Result = sb.ToString();
                            }

                            break;
                        case 6:
                            var duplicateSource = GetDuplicateSourceRecords();

                            if (duplicateSource != null && duplicateSource.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = duplicateSource[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var dup in duplicateSource)
                                {
                                    foreach (var m in dup.Matches)
                                    {
                                        string row = string.Empty;

                                        foreach (var i in m.ItemArray)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                                row += "\t";

                                            row += i.ToString();
                                        }

                                        sb.Append(row + Environment.NewLine);
                                    }
                                }

                                e.Result = sb.ToString();
                            }
                            break;
                        case 7:
                            var duplicateTarget = GetDuplicateTargetRecords();

                            if (duplicateTarget != null && duplicateTarget.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = duplicateTarget[0].Left.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column.ColumnName;
                                }

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var dup in duplicateTarget)
                                {
                                    foreach (var m in dup.Matches)
                                    {
                                        string row = string.Empty;

                                        foreach (var i in m.ItemArray)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                                row += "\t";

                                            row += i.ToString();
                                        }

                                        sb.Append(row + Environment.NewLine);
                                    }
                                }

                                e.Result = sb.ToString();
                            }

                            break;
                        case 8:
                            var nonMatchingValues = GetNonMatchingValues();

                            if (nonMatchingValues != null && nonMatchingValues.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                string header = string.Empty;

                                var columns = nonMatchingValues[0].RecordKeys.Select(x => x.Key.ColumnName).ToArray();

                                foreach (var column in columns)
                                {
                                    if (!string.IsNullOrEmpty(header))
                                        header += "\t";

                                    header += column;
                                }

                                header += "\t";
                                header += "Field (Source)" + "\t";
                                header += "Field (Target)" + "\t";
                                header += "Value (Source)" + "\t";
                                header += "Value (Target)" + "\t";

                                header += Environment.NewLine;

                                sb.Append(header);

                                foreach (var record in nonMatchingValues)
                                {
                                    string row = string.Empty;

                                    foreach (var key in record.RecordKeys)
                                    {
                                        if (!string.IsNullOrEmpty(row))
                                            row += "\t";

                                        row += key.Value;
                                    }

                                    row += "\t";
                                    row += record.SourceField.ColumnName + "\t";
                                    row += record.TargetField.ColumnName + "\t";
                                    row += record.SourceValue + "\t";
                                    row += record.TargetValue;

                                    sb.Append(row + Environment.NewLine);
                                }

                                e.Result = sb.ToString();
                            }

                            break;
                        case 9:

                            break;
                        case 10:

                            break;
                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error has occured while generating the report. " + ex.Message);
                }
            } //if 
        }

        private void ReportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {

            }
            else
            {
                _view.StatusMessage = e.UserState.ToString();
            }
        }

        private void ReportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    try
                    {
                        decimal megabyteSize = ((decimal)Encoding.Unicode.GetByteCount(e.Result.ToString()) / 1048576);
                        string sb = e.Result.ToString();

                        if (megabyteSize > 30)
                        {
                            int counter = 1;
                            string file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\report";
                            string newFileName = file;

                            while (File.Exists(newFileName + ".txt"))
                            {
                                newFileName = $"{file} ({counter})";
                                counter++;
                            }

                            var size = (Decimal.ToInt32(megabyteSize) * .4) - (Decimal.ToInt32(megabyteSize) * .4) % 10;

                            bool confirm = _view.ShowMessageYesNoBox($"The estimated file size of the " +
                                $"report is around {String.Format("{0:N0}", size + 10)} MB, which may take a while to open.\n\n" +
                                $"Do you want to open the file now? \n\n" +
                                $"You may later access the file from {newFileName + ".txt"}", "Successful Export");

                            File.WriteAllText(newFileName + ".txt", sb.ToString());

                            if (confirm)
                            {
                                Process notepad = Process.Start(newFileName + ".txt");
                            }
                            else
                            {
                                string filePath = newFileName + ".txt";
                                if (!File.Exists(filePath))
                                {
                                    return;
                                }

                                // combine the arguments together
                                // it doesn't matter if there is a space after ','
                                string argument = "/select, \"" + filePath + "\"";

                                System.Diagnostics.Process.Start("explorer.exe", argument);
                            }

                        }
                        else
                        {
                            Notepad.SendText(sb);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("An error has occured while extracting the report to file. " + ex.Message);
                    }
                }
            }

            _view.StatusMessage = string.Empty;
        }





        private void CommonRecordsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!commonRecordsWorker.CancellationPending)
            {
                try
                {
                    e.Result = GetCommonRecords().Count;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            } //if 
        }

        private void CommonRecordsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void CommonRecordsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    lblCommonRecords.Text = String.Format("{0:N0}", (int)e.Result);
                }
            }
        }

        private void DuplicateSourceWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!duplicateSourceWorker.CancellationPending)
            {
                try
                {
                    e.Result = GetDuplicateSourceRecords().Count;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            } //if 
        }

        private void DuplicateSourceWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void DuplicateSourceWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    lblDuplicateRecordsSource.Text = String.Format("{0:N0}", (int)e.Result);
                }
            }
        }




        private void DuplicateTargetWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!duplicateTargetWorker.CancellationPending)
            {
                try
                {
                    e.Result = GetDuplicateTargetRecords().Count;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            } //if 
        }

        private void DuplicateTargetWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void DuplicateTargetWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    lblDuplicateRecordsTarget.Text = String.Format("{0:N0}", (int)e.Result);
                }
            }
        }





        private void NonMatchingValuesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!nonMatchingValuesWorker.CancellationPending)
            {
                try
                {
                    e.Result = GetNonMatchingValues().Count;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            } //if 
        }

        private void NonMatchingValuesWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void NonMatchingValuesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                if (e.Result != null)
                {
                    lblNonMatchingValues.Text = String.Format("{0:N0}", (int)e.Result);
                }
            }
        }



































    }
}
