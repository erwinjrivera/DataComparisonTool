using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataComparisonTool.Common;
using DataComparisonTool.Model;
using DataComparisonTool.Utils;
using DataComparisonTool.View;
using ExcelDataReader;

namespace DataComparisonTool.Controller
{
    public class MainController
    {
        private IMainView _view;

        private Map _model;

        private DataSet _sourceDataSet;

        private DataSet _targetDataSet;

        private object _tempData;

        private List<MatchingResult> _matchingResult;

        private List<MatchingResult> _matchingResultRightTable;

        private List<FieldPerFieldComparison> _comparisonResult;

        private List<string> _errors;

        private BackgroundWorker bgWorkerJoin;

        private BackgroundWorker bgWorkerCompare;

        private Stopwatch stopWatch = new Stopwatch();


        public MainController(IMainView view, Map model)
        {
            _view = view;

            _model = model;
            _model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
        }

        public void Initialize()
        {
            _sourceDataSet = new DataSet("source");
            _targetDataSet = new DataSet("target");

            _view.RefreshSourceCount();
            _view.RefreshTargetCount();
            _view.InitializeDataGridViews();

            _matchingResult = new List<MatchingResult>();
            _matchingResultRightTable = new List<MatchingResult>();
            _comparisonResult = new List<FieldPerFieldComparison>();
            _errors = new List<string>();

            bgWorkerJoin = new System.ComponentModel.BackgroundWorker();
            bgWorkerJoin.WorkerReportsProgress = true;
            bgWorkerJoin.WorkerSupportsCancellation = true;
            bgWorkerJoin.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerJoin_DoWork);
            bgWorkerJoin.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerJoin_ProgressChanged);
            bgWorkerJoin.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerJoin_RunWorkerCompleted);

            bgWorkerCompare = new System.ComponentModel.BackgroundWorker();
            bgWorkerCompare.WorkerReportsProgress = true;
            bgWorkerCompare.WorkerSupportsCancellation = true;
            bgWorkerCompare.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerCompare_DoWork);
            bgWorkerCompare.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerCompare_ProgressChanged);
            bgWorkerCompare.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerCompare_RunWorkerCompleted);

            _model.IsDirty = false;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TargetField":
                    GetMappedFieldsForKeyFields();
                    break;
                case "IsKey":
                    //CountMatchingRecords();
                    _view.SourceRecordsDataSource = null;
                    _view.TargetRecordsDataSource = null;

                    _matchingResult.Clear();

                    _view.DisplayCounts(0, 0, 0);

                    _model.IsDirty = true;
                    break;
                case "IsDirty":
                    if (sender.GetType() == typeof(Map))
                    {
                        _view.ApplyRefreshMatchingButtonEnabled = _model.IsDirty;

                        if (!_model.IsDirty 
                            && (string.IsNullOrEmpty(_view.GetSelectedSourceTable()) || string.IsNullOrEmpty(_view.GetSelectedTargetTable())))
                        {
                            _view.ApplyRefreshMatchingButtonEnabled = true;
                        }
                    }
                    break;
            }
        }

        public void MatchRecords()
        {
            // get key fields
            var keyFields = _model.Where(x => x.SourceField.TableName == _view.GetSelectedSourceTable()
                                   && x.IsKey
                                   && x.SourceField.DataSetName == "source")
                                        .ToList<FieldMapping>();

            if (keyFields == null || keyFields.Count <= 0)
            {
                _view.SourceRecordsDataSource = null;
                _view.TargetRecordsDataSource = null;
                _matchingResult.Clear();

                _model.IsDirty = true;

                return;
            }

            if (!bgWorkerJoin.IsBusy)
            {
                stopWatch.Reset();
                stopWatch.Start();

                bgWorkerJoin.RunWorkerAsync(new object[] { keyFields, _view.GetSelectedSourceTable(), _view.GetSelectedTargetTable() });
            }
            else
            {
                //
            }
        }

        private static List<MatchingResult> LeftOuterJoin(List<FieldMapping> maps, DataTable leftTable, DataTable rightTable)
        {
            /* Original
              var Join = 
                from PX in pullX.AsEnumerable()
                join PY in pullY.AsEnumerable()
                on     string.Join("\0", joinColumnNames.Select(c => PX[c]))
                equals string.Join("\0", joinColumnNames.Select(c => PY[c]))
                into Outer
                from PY in Outer.DefaultIfEmpty<DataRow>(pullY.NewRow())
                select new { PX, PY };
            */

            /* Using LINQ
            var res =
                from PX in leftTable.AsEnumerable()
                join PY in rightTable.AsEnumerable()
                on string.Join("\0", maps.Select(c => PX[c.Source]))
                equals string.Join("\0", maps.Select(c => PY[c.Source]))
                into Outer
                select new
                {
                    F = "asdf",
                    G = from PY in Outer.DefaultIfEmpty<DataRow>(rightTable.NewRow())
                        select PY
                };
            */

            /* Original
            return leftTable
                .AsEnumerable()
                .GroupJoin(
                    rightTable.AsEnumerable(), // inner sequence
                    l => l, // outer key selector
                    r => r, // inner key selector
                    (l, rlist) => new { LeftValue = l, RightValues = rlist }, // result selector
                    new EqualityComparer(maps)) // comparer
                .SelectMany(
                    x => x.RightValues.DefaultIfEmpty(rightTable.NewRow()),
                    (x, y) => new RowMatch { Row1 = x.LeftValue, Row2 = y })
                .ToList();
            */

            /* Using expression */
            return leftTable
               .AsEnumerable()
               .GroupJoin(
                   rightTable.AsEnumerable(), // inner sequence
                   l => l, // outer key selector
                   r => r, // inner key selector
                   (l, rlist) => new { LeftValue = l, RightValues = rlist }, // result selector
                   new EqualityComparer(maps))
               .Select(x => new MatchingResult { Left = x.LeftValue, Matches = x.RightValues }).ToList();
        }

        public void CountDuplicateSourceRecords()
        {
            var table = _sourceDataSet.Tables[_view.GetSelectedSourceTable()];

            if (table != null)
            {
                var keys = _model.Where(x => x.IsKey && x.SourceField.DataSetName == "source" && x.SourceField.TableName == _view.GetSelectedSourceTable()).ToList<FieldMapping>();

                var duplicates = table.AsEnumerable().GroupBy(x => x, new EqualityComparer(keys))
                    .Where(g => g.Count() > 1)
                    .SelectMany(x => x.DefaultIfEmpty(table.NewRow()), 
                        (x, y) => new MatchingResult { Left = x.Key, Matches = new List<DataRow> { y } });
                    //.Select(x => x.Key);


            }
        }

        public void RemoveDuplicateSourceRecords()
        {
            var table = _sourceDataSet.Tables[_view.GetSelectedSourceTable()];

            if (table != null)
            {
                var keys = _model.Where(x => x.IsKey && x.SourceField.DataSetName == "source" && x.SourceField.TableName == _view.GetSelectedSourceTable()).ToList<FieldMapping>();

                var duplicates = table.AsEnumerable().GroupBy(x => x, new EqualityComparer(keys))
                    .Where(g => g.Count() > 1)
                    .SelectMany(x => x.DefaultIfEmpty(table.NewRow()), 
                        (x, y) => new MatchingResult { Left = x.Key, Matches = new List<DataRow> { y } });
                    //.Select(x => x.Key);

                if (duplicates != null)
                {
                    foreach (var m in duplicates)
                    {
                        if (Properties.Settings.Default.DefaultDuplicateRecordHandling.ToLower().Contains("retain"))
                        {
                            if (m.Left != m.Matches.ToList()[0]) // retain first instance 
                                table.Rows.Remove(m.Matches.ToList()[0]);
                        }
                        else if (Properties.Settings.Default.DefaultDuplicateRecordHandling.ToLower().Contains("remove all"))
                        {
                            table.Rows.Remove(m.Matches.ToList()[0]);
                        }
                    }
                }
            }
        }

        private DataTable RemoveDuplicateRecords(DataTable table, string dataSetName, string tableName)
        {
            if (Properties.Settings.Default.DefaultDuplicateRecordHandling.ToLower().Contains("preserve all"))
                return table;

            if (table != null)
            {
                var keys = _model.Where(x => x.IsKey 
                                && x.SourceField.DataSetName == dataSetName
                                && x.SourceField.TableName == tableName).ToList<FieldMapping>();

                var duplicates = table.AsEnumerable().GroupBy(x => x, new EqualityComparer(keys))
                                    .Where(g => g.Count() > 1)
                                    .SelectMany(x => x.DefaultIfEmpty(table.NewRow()),
                                        (x, y) => new MatchingResult { Left = x.Key, Matches = new List<DataRow> { y } });
                                    //.Select(x => x.Key);

                if (duplicates != null)
                {
                    foreach (var m in duplicates)
                    {
                        if (Properties.Settings.Default.DefaultDuplicateRecordHandling.ToLower().Contains("retain"))
                        {
                            if (m.Left != m.Matches.ToList()[0]) // retain first instance 
                                table.Rows.Remove(m.Matches.ToList()[0]);
                        }
                        else if (Properties.Settings.Default.DefaultDuplicateRecordHandling.ToLower().Contains("remove all"))
                        {
                            table.Rows.Remove(m.Matches.ToList()[0]);
                        }
                    }
                }
            }

            return table;
        }


        private void MarkDuplicateSourceRecords()
        {
            var t = _sourceDataSet.Tables[_view.GetSelectedSourceTable()];

            var table = t.Copy();

            if (table != null)
            {
                var keys = _model.Where(x => x.IsKey && x.SourceField.DataSetName == "source" && x.SourceField.TableName == _view.GetSelectedSourceTable()).ToList<FieldMapping>();

                var duplicates = table.AsEnumerable().GroupBy(x => x, new EqualityComparer(keys))
                    .Where(g => g.Count() > 1)
                    .SelectMany(x => x.DefaultIfEmpty(table.NewRow()),
                        (x, y) => new MatchingResult { Left = x.Key, Matches = new List<DataRow> { y } });
                //.Select(x => x.Key);

                if (duplicates != null)
                {
                    foreach (var m in duplicates)
                    {
                        if (m.Left != m.Matches.ToList()[0]) // first 
                            table.Rows.Remove(m.Matches.ToList()[0]);
                    }
                }
            }
        }

        public void UpdateIsKey(object obj)
        {
            try
            {
                var fieldMapping = (FieldMapping)obj;

                var f = _model.FirstOrDefault(x => x.SourceField.TableName == _view.GetSelectedSourceTable()
                        && x.SourceField.DataSetName == "source"
                        && x.Source == fieldMapping.Source);

                if (f != null)
                {
                    f.IsKey = fieldMapping.IsKey;

                    // enforce user to refresh record matching
                    //_view.SourceRecordsDataSource = null;
                    //_view.TargetRecordsDataSource = null;

                    //_matchingResult.Clear();

                    _model.IsDirty = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        public void ApplyRefreshMatching()
        {
            bool valid = Validate(true);

            if (!valid)
            {
                ShowErrors();

                return;
            }

            MatchRecords();
        }

       

        public object ImportedData
        {
            set
            {
                _tempData = null;
                _tempData = value;
            }
        }

        public void ImportSourceFiles()
        {
            string[] fileNames = _view.ShowOpenFileDialog();

            if (fileNames != null && fileNames.Length > 0)
            {
                this.ImportedData = _view.ShowImportFileDialog(new FileImportForm(_sourceDataSet, fileNames));

                ProcessImportedDataSet(ref _sourceDataSet);
            }
        }

        public void ImportTargetFiles()
        {
            string[] fileNames = _view.ShowOpenFileDialog();

            if (fileNames != null && fileNames.Length > 0)
            {
                this.ImportedData = _view.ShowImportFileDialog(new FileImportForm(_targetDataSet, fileNames));

                ProcessImportedDataSet(ref _targetDataSet);
            }
        }

        public void ImportTargetFile()
        {
            string[] fileNames = _view.ShowOpenFileDialog();

            //this.ImportedData = _view.ShowImportFileDialog(new FileImportForm(_targetDataSet));
        }

        public void OpenFile(string file)
        {
            /*
            switch (file)
            {
                case "source":
                    _view.ShowImportFileDialog(new FileImportForm(_sourceDataSet));

                    ProcessImportedDataSet(ref _sourceDataSet);
                    break;
                default:
                    _view.ShowImportFileDialog(new FileImportForm(_targetDataSet));

                    ProcessImportedDataSet(ref _targetDataSet);
                    break;
            }
            */
        }

        private void ProcessImportedDataSet(ref DataSet dataSet)
        {
            if (_tempData == null)
                return;

            try
            {
                var suggestions = (List<ImportSuggestion>)_tempData;

                if (suggestions != null)
                {
                    foreach (var s in suggestions)
                    {
                        Import(s.Table, ref dataSet, s.SelectedImportAction);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _tempData = null;
            }
        }

        private void Import(DataTable table, ref DataSet dataSet, string importAction)
        {
            if (importAction.ToLower().Contains("skip"))
                return;

            string tableName = table.TableName;

            if (importAction.ToLower().Contains("import"))
            {
                int count = 1;
                string tempName = table.TableName;

                while (TableExists(tempName, dataSet))
                {
                    tempName = $"{tableName} ({count})";
                }

                var copyTable = table.Copy();
                copyTable.TableName = tempName;

                dataSet.Tables.Add(copyTable);

                string rowCount = String.Format("{0:N0} {1}", copyTable.Rows.Count, copyTable.Rows.Count > 1 ? "Rows" : "Row");

                if (dataSet.DataSetName.Contains("source"))
                {
                    _view.AddItemToSourceList(tempName);
                    _view.AddTreeNodeToSource(tempName, $"{tempName} - ({rowCount})");
                    _view.RefreshSourceCount();
                }
                else
                {
                    _view.AddItemToTargetList(tempName);
                    _view.AddTreeNodeToTarget(tempName, $"{tempName} - ({rowCount})");
                    _view.RefreshTargetCount();
                }
            }
            else // append and overwrite
            {
                if (TableExists(tableName, dataSet))
                {
                    var t = dataSet.Tables[tableName];

                    if (importAction.ToLower().Contains("overwrite"))
                    {
                        t.Clear();
                    }

                    foreach (DataRow row in table.Rows)
                    {
                        t.ImportRow(row);
                    }

                    string rowCount = String.Format("{0:N0} {1}", t.Rows.Count, t.Rows.Count > 1 ? "Rows" : "Row");


                    if (dataSet.DataSetName.Contains("source"))
                    {
                        _view.UpdateTreeNodeToSource(tableName, $"{tableName} ({rowCount})");
                    }
                    else
                    {
                        _view.UpdateTreeNodeToTarget(tableName, $"{tableName} ({rowCount})");
                    }

                    // only clear mapping and comparison result since new records have been added to the  table
                    if (dataSet.DataSetName == "source")
                    {
                        if (tableName == _view.GetSelectedSourceTable()) // imported data is for the current selected source table
                        {
                            // then clear data

                            ClearMatchingResults();
                            ClearComparisonResult();

                            _view.ShowReport(null);
                            _view.DisplayCounts(0, 0, 0);
                            _view.SourceRecordsDataSource = null;
                            _view.TargetRecordsDataSource = null;

                            _view.StatusMessage = "The content of the source table has been altered. Match records again before comparing the data.";
                        }
                    }
                    else
                    {
                        if (tableName == _view.GetSelectedTargetTable()) // imported data is for the current selected source table
                        {
                            // then clear data
                            ClearMatchingResults();
                            ClearComparisonResult();

                            _view.ShowReport(null);
                            _view.DisplayCounts(0, 0, 0);
                            _view.SourceRecordsDataSource = null;
                            _view.TargetRecordsDataSource = null;

                            _view.StatusMessage = "The content of the target table has been altered. Match records again before comparing the data.";
                        }
                    }
                }
            }
        }

        private void ClearMatchingResults()
        {
            _matchingResult = new List<MatchingResult>();
            _matchingResultRightTable = new List<MatchingResult>();
        }

        private void ClearComparisonResult()
        {
            _comparisonResult = new List<FieldPerFieldComparison>();
        }

        private bool TableExists(string tableName, DataSet dataSet)
        {
            if (dataSet == null)
                return false;
            if (dataSet.Tables == null)
                return false;
            if (dataSet.Tables.Count <= 0)
                return false;

            foreach (DataTable t in dataSet.Tables)
            {
                if (t.TableName.ToLower() == tableName.ToLower())
                    return true;
            }

            return false;
        }

        private void UpdateSourcePreview()
        {
            if (_view.GetSelectedSourceTable() == _view.SelectedSourceTable)
                return;

            string tableName = _view.GetSelectedSourceTable();

            if (TableExists(tableName, _sourceDataSet))
            {
                _view.SourcePreviewDataSource = _sourceDataSet.Tables[tableName];
            }

            //_view.SelectedSourceTable = tableName;
            _model.IsDirty = false;
        }

        public void UpdateTargetPreview()
        {
            if (_view.GetSelectedTargetTable() == _view.SelectedTargetTable)
                return;

            string tableName = _view.GetSelectedTargetTable();

            if (TableExists(tableName, _targetDataSet))
            {
                _view.TargetPreviewDataSource = _targetDataSet.Tables[tableName];
            }

            //_view.SelectedTargetTable = tableName;
            _model.IsDirty = false;
        }

        private bool HasMappedFields()
        {
            return _model.Count(x => x.Mapped) > 0;
        }

        private void ReadSourceColumns()
        {
            /*
            if (_view.GetSelectedSourceTable() == _view.SelectedSourceTable)
            {
                return;
            }
            else
            {
                if (HasMappedFields())
                {
                    bool confirmToChange = _view.ConfirmChangeTab();

                    if (confirmToChange)
                    {
                        UnmapAll();
                        //ReadTargetColumns();
                    }
                    else
                    {
                        _view.ResetSelectedSourceTable();

                        return;
                    }
                }
            }
            */

            string tableName = _view.GetSelectedSourceTable();

            if (TableExists(tableName, _sourceDataSet))
            {
                var table = _sourceDataSet.Tables[tableName];

                if (table.Columns != null)
                {
                    //from s in _model where s.Description == "source" select s;
                    var sourceFields = _model.Where(x => x.SourceField.TableName == tableName && x.SourceField.DataSetName == "source").ToList<FieldMapping>();

                    if (sourceFields != null)
                        sourceFields.ToList<FieldMapping>().ForEach(item => _model.Remove(item));

                    foreach (DataColumn column in table.Columns)
                    {
                        FieldMapping fieldMapping = new FieldMapping
                        {
                            Description = "source",
                            SourceField = new Field(column)
                        };

                        fieldMapping.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);

                        _model.Add(fieldMapping);

                        if (_model.IsDirty)
                            _model.IsDirty = false;
                    }

                    //var x = from item in _model where item.Description == "source" select item;
                    //_view.SourceColumnsDataSource = _model.Where(x => x.SourceField.TableName == tableName && x.SourceField.DataSetName == "source").ToList<FieldMapping>();

                    UpdateDisplaySourceColumnsDataSource();
                }
            }



        }

        private void ReadTargetColumns()
        {
            //if (_view.GetSelectedTargetTable() == _view.SelectedTargetTable)
            //    return;
            //if (_model.IsDirty)
            //    return;

            /*
            if (_view.GetSelectedTargetTable() == _view.SelectedTargetTable)
            {
                return;
            }
            else
            {
                if (HasMappedFields())
                {
                    bool confirmToChange = _view.ConfirmChangeTab();

                    if (confirmToChange)
                    {
                        UnmapAll();
                        //ReadTargetColumns();
                    }
                    else
                    {
                        _view.ResetSelectedTargetTable();

                        return;
                    }
                }
            }
            */

            string tableName = _view.GetSelectedTargetTable();

            if (TableExists(tableName, _targetDataSet))
            {
                var table = _targetDataSet.Tables[tableName];

                if (table.Columns != null)
                {
                    //from s in _model where s.Description == "target" select s;
                    var targetFields = _model.Where(x => x.SourceField.TableName == tableName && x.SourceField.DataSetName == "target").ToList<FieldMapping>();

                    if (targetFields != null)
                        targetFields.ToList<FieldMapping>().ForEach(item => _model.Remove(item));

                    foreach (DataColumn column in table.Columns)
                    {
                        FieldMapping fieldMapping = new FieldMapping
                        {
                            Description = "target",
                            SourceField = new Field(column)
                        };

                        _model.Add(fieldMapping);

                        if (_model.IsDirty)
                            _model.IsDirty = false;
                    }

                    //var x = from item in _model where item.Description == "target" select item;

                    //x.ToList<FieldMapping>();
                    //_view.TargetColumnsDataSource = _model.Where(x => x.SourceField.TableName == tableName && x.SourceField.DataSetName == "target").ToList<FieldMapping>();

                    UpdateDisplayTargetColumnsDataSource();
                }
            }
        }

        private void UpdateDisplaySourceColumnsDataSource()
        {
            _view.SourceColumnsDataSource = _model.Where(x => x.SourceField.TableName == _view.GetSelectedSourceTable()
                && x.SourceField.DataSetName == "source").ToList<FieldMapping>();
        }

        private void UpdateDisplayTargetColumnsDataSource()
        {
            _view.TargetColumnsDataSource = _model.Where(x => x.SourceField.TableName == _view.GetSelectedTargetTable()
                && x.SourceField.DataSetName == "target").ToList<FieldMapping>();
        }

        public void Map(object source, object target)
        {
            try
            {
                /*
                var x = ((FieldMapping)source);
                var y = ((FieldMapping)target);

                if (x.TargetField == null && y.TargetField == null)
                {
                    _model.IsDirty = true;

                    x.TargetField = y.SourceField;
                    y.TargetField = x.SourceField;

                    _view.MatchingFieldsDataSource(x, y, "map");
                }
                */


                var sourceField = ((FieldMapping)source);
                var targetField = ((FieldMapping)target);

                // get the model for source
                var s = _model.Where(x => !x.Mapped && x.SourceField.DataSetName == "source"
                            && x.SourceField.TableName == _view.GetSelectedSourceTable()
                            && x.Source == sourceField.Source)
                                .ToList<FieldMapping>();

                // get the model for target
                var t = _model.Where(x => !x.Mapped && x.SourceField.DataSetName == "target"
                            && x.SourceField.TableName == _view.GetSelectedTargetTable()
                            && x.Source == targetField.Source)
                                .ToList<FieldMapping>();

                if (s != null && t != null)
                {
                    sourceField.TargetField = t[0].SourceField;
                    targetField.TargetField = s[0].SourceField;

                    _view.UpdateMapping(sourceField, targetField, "map");

                    _view.ShowReport(null);

                    //_model.IsDirty = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Unmap(object source, object target)
        {
            try
            {
                /*
                var x = ((FieldMapping)source);
                var y = ((FieldMapping)target);

                x.TargetField = null;
                y.TargetField = null;

                _view.UpdateMapping(x, y, "unmap");

                if (_model.Count(field => field.Mapped) <= 0)
                    _model.IsDirty = false;
                */

                var sourceField = ((FieldMapping)source);
                var targetField = ((FieldMapping)target);

                // get the model for source
                var s = _model.Where(x => !x.Mapped && x.SourceField.DataSetName == "source"
                    && x.SourceField.TableName == _view.GetSelectedSourceTable()
                    && x.Source == sourceField.Source)
                    .ToList<FieldMapping>();

                // get the model for target
                var t = _model.Where(x => !x.Mapped && x.SourceField.DataSetName == "target"
                    && x.SourceField.TableName == _view.GetSelectedTargetTable()
                    && x.Source == targetField.Source)
                    .ToList<FieldMapping>();

                if (s != null && t != null)
                {
                    // check if the field to be unmapped is key, then reset datasources
                    if (sourceField.IsKey)
                    {
                        // set datasources to null since field mapping is removed
                        // enfoce user to refresh matching
                        //_view.SourceRecordsDataSource = null;
                        //_view.TargetRecordsDataSource = null;

                        //_matchingResult.Clear();
                    }

                    sourceField.TargetField = null;

                    if (sourceField.IsKey)
                        sourceField.IsKey = false;

                    targetField.TargetField = null;

                    if (targetField.IsKey)
                        targetField.IsKey = false;

                    _view.UpdateMapping(sourceField, targetField, "unmap");

                    _view.ShowReport(null);

                    _view.StatusMessage = "Mapping details changed.";

                    //_model.IsDirty = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void AutoMap()//(object source, object target)
        {
            try
            {
                //_view.StatusMessage = "Automapping in progress. Please wait...";
                /*
                var x = ((FieldMapping)source);
                var y = ((FieldMapping)target);

                if (!x.Mapped && !y.Mapped)
                {
                    string sourceColumnName = x.Source.ToLower();
                    string targetColumnName = y.Source.ToLower();

                    if (sourceColumnName == targetColumnName)
                    {
                        _model.IsDirty = true;

                        x.TargetField = y.SourceField;
                        y.TargetField = x.SourceField;

                        _view.UpdateMapping(x, y, "map");
                    }
                }
                */

                // filter all fields of the current source table
                var s = _model.Where(x => !x.Mapped
                    && x.SourceField.DataSetName == "source"
                    && x.SourceField.TableName == _view.GetSelectedSourceTable()).ToList<FieldMapping>();

                // filter all fields of the current target table
                var t = _model.Where(x => !x.Mapped
                    && x.SourceField.DataSetName == "target"
                    && x.SourceField.TableName == _view.GetSelectedTargetTable()).ToList<FieldMapping>();

                if (s != null && t != null)
                {
                    bool isDirty = false;

                    foreach (var x in s)
                    {
                        foreach (var y in t)
                        {
                            if (x.Source.ToLower() == y.Source.ToLower() && !y.Mapped)
                            {
                                //_model.IsDirty = true;

                                x.TargetField = y.SourceField;
                                y.TargetField = x.SourceField;

                                _view.UpdateMapping(x, y, "map");

                                if (!isDirty)
                                    isDirty = true;

                                break;
                            }
                        }
                    }

                    if (s.Count <= 0 || t.Count <= 0)
                    {
                        if (_model.Count(x => x.Mapped) <= 0)
                            _view.StatusMessage = "Not enough fields information to perform auto mapping. Select the active source or target table, or import files with correct structure.";
                        else
                            _view.StatusMessage = "All fields are mapped already.";
                    }

                    //_model.IsDirty = isDirty;
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                //_view.StatusMessage = string.Empty;
            }
        }

        private void UnmapAll()
        {
            /*
            if (_model.IsDirty)
            {
                var s = _model.Where(x => x.SourceField.TableName == _view.SelectedSourceTable).ToList<FieldMapping>();

                if (s != null)
                {
                    foreach (var fieldMapping in s)
                    {
                        fieldMapping.TargetField = null;
                    }

                    _view.SourceColumnsDataSource = s;
                }

                var t = _model.Where(x => x.SourceField.TableName == _view.SelectedTargetTable).ToList<FieldMapping>();

                if (t != null)
                {
                    foreach (var fieldMapping in t)
                    {
                        fieldMapping.TargetField = null;
                    }

                    _view.TargetColumnsDataSource = s;
                }

                _model.IsDirty = false;
            }
            */

            if (HasMappedFields())
            {
                foreach (FieldMapping fieldMapping in _model)
                {
                    fieldMapping.TargetField = null;
                    fieldMapping.IsKey = false;
                }

                /*
                _view.SourceColumnsDataSource = _model.Where(x => x.SourceField.TableName == _view.GetSelectedSourceTable()
                    && x.SourceField.DataSetName == "source").ToList<FieldMapping>();

                _view.TargetColumnsDataSource = _model.Where(x => x.SourceField.TableName == _view.GetSelectedTargetTable()
                    && x.SourceField.DataSetName == "target").ToList<FieldMapping>();
                */
            }
        }

        public void Mappable()
        {
            if (_view.RowsCount("dataGridViewSourceFields") > 0 && _view.RowsCount("dataGridViewTargetFields") > 0)
            {
                bool mappable = _view.SelectedRowsCount("dataGridViewSourceFields") == 1 && _view.SelectedRowsCount("dataGridViewTargetFields") == 1;

                _view.MapButtonEnabled = mappable;

                if (!mappable)
                    return;

                try
                {
                    var source = (FieldMapping)_view.SelectedSourceObject;
                    var target = (FieldMapping)_view.SelectedTargetObject;

                    _view.MapButtonEnabled = source.TargetField == null && target.TargetField == null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void Unmappable()
        {
            if (_view.RowsCount("dataGridViewSourceFields") > 0 && _view.RowsCount("dataGridViewTargetFields") > 0)
            {
                bool unmappable = _view.SelectedRowsCount("dataGridViewSourceFields") == 1 && _view.SelectedRowsCount("dataGridViewTargetFields") == 1;

                _view.UnmapButtonEnabled = unmappable;

                if (!unmappable)
                    return;

                try
                {
                    var source = (FieldMapping)_view.SelectedSourceObject;
                    var target = (FieldMapping)_view.SelectedTargetObject;

                    _view.UnmapButtonEnabled = source.TargetField != null && target.TargetField != null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void UpdateTabs()
        {
            /*
            if (!_model.IsDirty)
            {
                //bool confirmed = _view.ConfirmChangeTab();

                ReadSourceColumns();
                ReadTargetColumns();
                UpdateSourcePreview();
                UpdateTargetPreview();
            }
            */

            ChangeSourceTable();
            ChangeTargetTable();
        }

        public void ChangeSourceTable(bool force = false)
        {
            if (!force && (_view.GetSelectedSourceTable() == _view.SelectedSourceTable))
                return;

            bool execute = true;

            if (HasMappedFields())
            {
                execute = _view.ConfirmChangeTab();

                if (!execute)
                {
                    _view.ResetSelectedSourceTable();
                }
            }

            if (execute)
            {
                UnmapAll();
                ReadSourceColumns();
                UpdateSourcePreview();

                UpdateDisplayTargetColumnsDataSource();

                ClearReport();
            }
        }

        private void ClearReport()
        {
            _view.ShowReport(null);
        }

        public void ChangeTargetTable(bool force = false)
        {
            if (!force && (_view.GetSelectedTargetTable() == _view.SelectedTargetTable))
                return;

            bool execute = true;

            if (HasMappedFields())
            {
                execute = _view.ConfirmChangeTab();

                if (!execute)
                {
                    _view.ResetSelectedTargetTable();
                }
            }

            if (execute)
            {
                UnmapAll();
                ReadTargetColumns();
                UpdateTargetPreview();

                UpdateDisplaySourceColumnsDataSource();
            }
        }

        public void GetMappedFieldsForKeyFields()
        {
            _view.MatchingFieldsDataSource = _model.Where(x => x.Mapped
                && x.SourceField.DataSetName == "source"
                && x.SourceField.TableName == _view.GetSelectedSourceTable()).ToList<FieldMapping>();
        }

        public void DisplayMatchingRecords(object[] objects)
        {
            _view.TargetRecordsDataSource = null;

            if (objects == null || objects.Length <= 0)
                return;

            try
            {
                var records = new SortableBindingList<object>();

                var dynamicProperties = new List<DynamicProperty>();
                DataColumnCollection columns = null;
                Type extendType = null;

                foreach (object obj in objects)
                {
                    var rows = ((IEnumerable<DataRow>)obj);

                    if (rows != null)
                    {
                        var factory = new DynamicTypeFactory();

                        if (columns == null)
                        {
                            foreach (DataRow row in rows)
                            {
                                columns = row.Table.Columns;

                                foreach (DataColumn column in columns)
                                {
                                    dynamicProperties.Add(new DynamicProperty
                                    {
                                        PropertyName = StringExtensions.ToCamelCase(column.ColumnName),
                                        DisplayName = column.ColumnName,
                                        SystemTypeName = "System.String"
                                    });
                                }
                            }
                        }

                        if (dynamicProperties.Count <= 0)
                            continue;

                        if (extendType == null)
                        {
                            extendType = factory.CreateNewTypeWithDynamicProperties(typeof(SourceMatch), dynamicProperties);
                            //var properties = extendType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            //                         .Where(p => p.CanRead && p.CanWrite);
                        }


                        foreach (DataRow row in rows)
                        {
                            var extendedObject = Activator.CreateInstance(extendType);

                            for (int index = 0; index < columns.Count; index++)
                            {
                                extendType.GetProperty($"{nameof(DynamicProperty)}_{dynamicProperties[index].PropertyName}").SetValue(extendedObject, row.ItemArray[index].ToString(), null);
                            }

                            records.Add(extendedObject);
                        }
                    }
                }// for

                _view.TargetRecordsDataSource = null;
                _view.TargetRecordsDataSource = records;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private SortableBindingList<object> PrepareSourceMatchingRecordsData()
        {
            if (_matchingResult == null || _matchingResult.Count <= 0)
                return null;

            var factory = new DynamicTypeFactory();
            var records = new SortableBindingList<object>();
            var dynamicProperties = new List<DynamicProperty>();
            var columns = _matchingResult[0].Left.Table.Columns;

            dynamicProperties.Add(new DynamicProperty
            {
                PropertyName = "MatchType",
                DisplayName = "Match Type",
                SystemTypeName = "System.String"
            });

            dynamicProperties.Add(new DynamicProperty
            {
                PropertyName = "MatchCount",
                DisplayName = "Match Count",
                SystemTypeName = "System.String"
            });

            foreach (DataColumn column in columns)
            {
                dynamicProperties.Add(new DynamicProperty
                {
                    PropertyName = StringExtensions.ToCamelCase(column.ColumnName),
                    DisplayName = column.ColumnName,
                    SystemTypeName = "System.String"
                });
            }

            dynamicProperties.Add(new DynamicProperty
            {
                PropertyName = "Matches",
                DisplayName = "Matches",
                SystemTypeName = "System.Object"
            });


            var extendType = factory.CreateNewTypeWithDynamicProperties(typeof(SourceMatch), dynamicProperties);

            int noneCount = 0;
            int singleCount = 0;
            int multipleCount = 0;

            foreach (var row in _matchingResult)
            {
                var extendedObject = Activator.CreateInstance(extendType);

                for (int index = 0; index < columns.Count; index++)
                {
                    extendType.GetProperty($"{nameof(DynamicProperty)}_{dynamicProperties[index + 2].PropertyName}").SetValue(extendedObject, row.Left.ItemArray[index].ToString(), null);
                }

                int matchCount = 0;

                if (row.Matches != null)
                {
                    matchCount = row.Matches.ToList().Count;
                }

                extendType.GetProperty($"{nameof(DynamicProperty)}_MatchType").SetValue(extendedObject, matchCount > 1 ? "Multiple" : matchCount == 0 ? "None" : "Single", null);
                extendType.GetProperty($"{nameof(DynamicProperty)}_MatchCount").SetValue(extendedObject, matchCount.ToString(), null);
                extendType.GetProperty($"{nameof(DynamicProperty)}_Matches").SetValue(extendedObject, row.Matches, null);

                if (matchCount == 0)
                    noneCount++;
                else if (matchCount == 1)
                    singleCount++;
                else if (matchCount > 1)
                    multipleCount++;

                records.Add(extendedObject);
            }

            //_model.IsDirty = false;

            return records;

            //_view.SourceRecordsDataSource = records;
            //_view.DisplayCounts(noneCount, singleCount, multipleCount);

            //var properties = extendType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);

            //_view.HighlightSourceKeyFields(keyFields.Select(x => $"DynamicProperty_{x.Source}").ToArray());
        }

        public void DisplayMatchSummary()
        {
            int noneCount = 0;
            int singleCount = 0;
            int multipleCount = 0;

            foreach (var row in _matchingResult)
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

            _view.DisplayCounts(noneCount, singleCount, multipleCount);
        }

        private void EmphasizeKeyFields()
        {
            var keyFields = _model.Where(x => x.IsKey 
                                && x.Mapped 
                                && x.SourceField.TableName == _view.GetSelectedSourceTable() 
                                && x.SourceField.DataSetName == "source")
                                    .Select(x => $"DynamicProperty_{x.Source}")
                                        .ToArray();

            _view.HighlightSourceKeyFields(keyFields);
        }

        public void StartCompare()
        {
            bool valid = Validate();

            if (!valid)
            {
                ShowErrors();

                return;
            }

            if (!bgWorkerCompare.IsBusy)
            {
                stopWatch.Reset();
                stopWatch.Start();

                _comparisonResult.Clear();
                _view.FieldPerFieldsDataSource = null;

                bgWorkerCompare.RunWorkerAsync(new object[] { _view.GetSelectedSourceTable() });
            }
            else
            {
                //
            }
        }

        private bool Validate(bool skipRecordMatchingValidation = false)
        {
            _errors.Clear();

            if (_sourceDataSet.Tables.Count <= 0|| _targetDataSet.Tables.Count <= 0)
                _errors.Add("Import source and target data.");
            if (_view.GetSelectedSourceTable() == null || string.IsNullOrEmpty(_view.GetSelectedSourceTable()))
                _errors.Add("Please select a source table.");
            if (_view.GetSelectedTargetTable() == null || string.IsNullOrEmpty(_view.GetSelectedTargetTable()))
                _errors.Add("Please select a target table.");
            
            if (_sourceDataSet.Tables.Contains(_view.GetSelectedSourceTable()))
            {
                if (_sourceDataSet.Tables[_view.GetSelectedSourceTable()].Rows.Count <= 0)
                    _errors.Add("The selected source table does not contain any record.");
            }

            if (_targetDataSet.Tables.Contains(_view.GetSelectedTargetTable()))
            {
                if (_targetDataSet.Tables[_view.GetSelectedTargetTable()].Rows.Count <= 0)
                    _errors.Add("The selected target table does not contain any record.");
            }

            if (_model != null)
            {
                if (_model.Count(x => x.Mapped) <= 0)
                    _errors.Add("Map fields accordingly.");
                if (_model.Count(x => x.IsKey) <= 0)
                    _errors.Add("Select at least one (1) key field.");
            }

            if (!skipRecordMatchingValidation)
            {
                if (_matchingResult == null || _matchingResult.Count <= 0)
                    _errors.Add("Match records first. Go to 'Match Records' tab and click 'Apply/Refresh Matching'.");

                var keyFields = _model.Count(x => x.IsKey
                                    && x.Mapped
                                    && x.SourceField.TableName == _view.GetSelectedSourceTable()
                                    && x.SourceField.DataSetName == "source");

                var mappedFields = _model.Count(x => !x.IsKey
                                            && x.Mapped
                                            && x.SourceField.TableName == _view.GetSelectedSourceTable()
                                            && x.SourceField.DataSetName == "source");

                if (keyFields == (keyFields + mappedFields))
                    _errors.Add("There must be at least one (1) non-key field to be able to compare data.");
            }

            return _errors.Count == 0;
        }

        private void ShowErrors()
        {
            if (_errors.Count > 0)
                _view.ShowMessageBoxOk(_errors[0], "Error");
        }

        #region 

        private void BackgroundWorkerJoin_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!bgWorkerJoin.CancellationPending)
            {
                try
                {
                    if (e.Argument == null)
                        return;

                    bgWorkerJoin.ReportProgress(1);

                    var parameters = (object[])e.Argument;

                    // create temp datasets to which copy of source and target tables will be associated with
                    DataSet tempDataSetSource = new DataSet
                    {
                        DataSetName = "source"
                    };

                    DataSet tempDataSetTarget = new DataSet
                    {
                        DataSetName = "target"
                    };

                    // create instances of datatables
                    var sourceTableCopy = new DataTable();
                    var targetTableCopy = new DataTable();

                    // associate datatables to the temp datasets
                    tempDataSetSource.Tables.Add(sourceTableCopy);
                    tempDataSetTarget.Tables.Add(targetTableCopy);

                    //sourceTableCopy.Columns.AddRange(_sourceDataSet.Tables[(string)parameters[1]].Copy().Columns);

                    bgWorkerJoin.ReportProgress(2, "Creating temporary tables...");

                    foreach (DataColumn column in _sourceDataSet.Tables[(string)parameters[1]].Copy().Columns)
                    {
                        sourceTableCopy.Columns.Add(new DataColumn
                        {
                            ColumnName = column.ColumnName,
                            DataType = column.DataType,
                            Caption = column.Caption                         
                        });
                    }

                    foreach (DataColumn column in _targetDataSet.Tables[(string)parameters[2]].Copy().Columns)
                    {
                        targetTableCopy.Columns.Add(new DataColumn
                        {
                            ColumnName = column.ColumnName,
                            DataType = column.DataType,
                            Caption = column.Caption
                        });
                    }

                    sourceTableCopy.TableName = (string)parameters[1];
                    targetTableCopy.TableName = (string)parameters[2];

                    foreach (DataRow row in _sourceDataSet.Tables[(string)parameters[1]].Copy().Rows)
                    {
                        sourceTableCopy.Rows.Add(row.ItemArray);
                    }

                    foreach (DataRow row in _targetDataSet.Tables[(string)parameters[2]].Copy().Rows)
                    {
                        targetTableCopy.Rows.Add(row.ItemArray);
                    }

                    RemoveDuplicateRecords(sourceTableCopy, "source", sourceTableCopy.TableName);
                    RemoveDuplicateRecords(targetTableCopy, "source", sourceTableCopy.TableName);

                    /*
                    _matchingResult = LeftOuterJoin((List<FieldMapping>)parameters[0],
                                        _sourceDataSet.Tables[(string)parameters[1]],
                                        _targetDataSet.Tables[(string)parameters[2]]);
                    */

                    bgWorkerJoin.ReportProgress(2, "Joining tables...");

                    _matchingResult = LeftOuterJoin((List<FieldMapping>)parameters[0],
                                        sourceTableCopy,
                                        targetTableCopy);

                    _matchingResultRightTable = LeftOuterJoin((List<FieldMapping>)parameters[0],
                                        targetTableCopy,
                                        sourceTableCopy);

                    bgWorkerJoin.ReportProgress(2, "Generating result...");

                    e.Result = PrepareSourceMatchingRecordsData();

                    bgWorkerJoin.ReportProgress(2, "Displaying result...");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error has occured while matching records. " + ex.Message);
                }

                
            } //if 
        }

        private void BackgroundWorkerJoin_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                _view.ShowCustomMessageBox("Matching records. Please wait...", string.Empty);
            }
            else
            {
                _view.StatusMessage = e.UserState.ToString();
            }     
        }

        private void BackgroundWorkerJoin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
               
            }
            else
            {
                _view.SourceRecordsDataSource = e.Result;

                DisplayMatchSummary();

                EmphasizeKeyFields();

                _view.StatusMessage = string.Empty;

                _view.CloseCustomMessageBox();

                ClearReport();

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                string time = string.Empty;

                if (ts.Hours > 0)
                    time += " " + ts.Hours + (ts.Hours > 1 ? " hrs" : " hr");
                if (ts.Minutes > 0)
                    time += " " + ts.Minutes + (ts.Minutes > 1 ? " mins" : " min");
                if (ts.Seconds > 0)
                    time += " " + ts.Seconds + (ts.Seconds > 1 ? " secs" : " sec");
                if (ts.Milliseconds > 0)
                    time += " " + ts.Milliseconds + " ms";

                /*
                _view.StatusMessage = "Done comparing data in: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds,
                                            ts.Milliseconds / 10);
                */

                _view.StatusMessage = "Done matching records in:" + time;

                GC.Collect();
            }
        }



        private void BackgroundWorkerCompare_DoWork(object sender, DoWorkEventArgs e)
        {
            bgWorkerCompare.ReportProgress(1);

            if (!bgWorkerCompare.CancellationPending)
            {
                try
                {
                    var parameters = (object[])e.Argument;

                    if (_matchingResult.Count > 0)
                    {
                        bgWorkerCompare.ReportProgress(2, "Fetching key fields...");

                        var keyFields = _model.Where(x => x.IsKey
                                            && x.Mapped
                                            && x.SourceField.TableName == parameters[0].ToString()
                                            && x.SourceField.DataSetName == "source");

                        bgWorkerCompare.ReportProgress(2, "Fetching mapped fields...");

                        var mappedFields = _model.Where(x => !x.IsKey
                                            && x.Mapped
                                            && x.SourceField.TableName == parameters[0].ToString()
                                            && x.SourceField.DataSetName == "source");

                        bgWorkerCompare.ReportProgress(2, "Validating...");

                        int counter = 1;
                        int totalRecordsCount = _matchingResult.Count;

                        foreach (var result in _matchingResult)
                        {
                            bgWorkerCompare.ReportProgress(2, "Processing records " + String.Format("{0:N0}", counter) + " of " + String.Format("{0:N0}", totalRecordsCount) + "...");

                            // create record keys

                            List<RecordKey> keys = new List<RecordKey>();

                            foreach (var key in keyFields)
                            {
                                int index = 0;

                                foreach (DataColumn column in result.Left.Table.Columns)
                                {
                                    if (key.Source == column.ColumnName)
                                    {
                                        var value = result.Left.ItemArray[index];

                                        keys.Add(new RecordKey
                                        {
                                            Key = key.SourceField,
                                            Value = value != null ? value.ToString() : string.Empty
                                        });

                                        index++;
                                    }
                                }
                            }

                            foreach (var mappedField in mappedFields)
                            {
                                try
                                {
                                    if (result.Matches != null && result.Matches.ToList().Count > 0)
                                    {
                                        foreach (var m in result.Matches)
                                        {
                                            var valueMatch = new ValueMatch
                                            {
                                                SourceField = mappedField.SourceField,
                                                TargetField = mappedField.TargetField,
                                                SourceValue = result.Left[mappedField.SourceField.ColumnName],
                                                TargetValue = m[mappedField.TargetField.ColumnName],
                                                RecordKeys = keys
                                            };

                                            var c = _comparisonResult.Where(x => x.SourceField.Id == mappedField.SourceField.Id).FirstOrDefault();

                                            if (c == null)
                                            {
                                                _comparisonResult.Add(new FieldPerFieldComparison
                                                {
                                                    SourceField = mappedField.SourceField,
                                                    TargetField = mappedField.TargetField,
                                                    Values = new List<ValueMatch> { valueMatch }
                                                });
                                            }
                                            else
                                            {
                                                c.Values.Add(valueMatch);
                                            }

                                        } //for
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("An error has occured while comparing data. " + ex.Message);
                                }
                            } // for

                            counter++;
                        } // for

                        e.Result = _comparisonResult;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error has occured while comparing records. " + ex.Message);
                }


            } //if 
        }

        private void BackgroundWorkerCompare_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage == 1)
            {
                _view.ShowCustomMessageBox("Comparing data. Please wait...", string.Empty);
            }
            else
            {
                _view.StatusMessage = e.UserState.ToString();
            }
                       
        }

        private void BackgroundWorkerCompare_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                //_view.FieldPerFieldsDataSource = e.Result;

                //_view.ShowReport(new ReportData { FieldPerFieldDataSource = e.Result });

                _view.ShowReport(new ReportData
                {
                    SourceTableName = _view.GetSelectedSourceTable(),
                    TargetTableName = _view.GetSelectedTargetTable(),
                    SourceDataSet = _sourceDataSet,
                    TargetDataSet = _targetDataSet,
                    Mapping = _model,
                    MatchResult = _matchingResult,
                    ComparisonResult = _comparisonResult,
                    MatchResultRightTable = _matchingResultRightTable
                });

                _view.CloseCustomMessageBox();

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                string time = string.Empty;

                if (ts.Hours > 0)
                    time += " " + ts.Hours + (ts.Hours > 1 ? " hrs" : " hr");
                if (ts.Minutes > 0)
                    time += " " + ts.Minutes + (ts.Minutes > 1 ? " mins" : " min");
                if (ts.Seconds > 0)
                    time += " " + ts.Seconds + (ts.Seconds > 1 ? " secs" : " sec");
                if (ts.Milliseconds > 0)
                    time += " " + ts.Milliseconds + " ms";

                /*
                _view.StatusMessage = "Done comparing data in: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds,
                                            ts.Milliseconds / 10);
                */

                _view.StatusMessage = "Done comparing data in:" + time;

                GC.Collect();
            }
        }

        #endregion









    }
}
