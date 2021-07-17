using DataComparisonTool.Common;
using DataComparisonTool.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using System.Diagnostics;
using DataComparisonTool.Model;
using System.ComponentModel;

namespace DataComparisonTool.Controller
{
    public class ImportFileController
    {
        private IFileImportView _view;

        private DataSet _dataSet;

        private DataSet _local;

        private BackgroundWorker bgWorkerJoin;

        private string[] _fileNames;

        private System.Windows.Forms.Timer _timer1;
        private int hr;
        private int min;
        private int sec;

        public ImportFileController(IFileImportView view, DataSet data, string[] fileNames)
        {
            _view = view;

            _dataSet = data;
            _fileNames = fileNames;
        }

        public void Initialize()
        {
            if (_dataSet.DataSetName.Contains("source"))
                _view.FormText = "Import source file";
            else
                _view.FormText = "Import target file";

            bgWorkerJoin = new System.ComponentModel.BackgroundWorker();
            bgWorkerJoin.WorkerReportsProgress = true;
            bgWorkerJoin.WorkerSupportsCancellation = true;
            bgWorkerJoin.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerJoin_DoWork);
            bgWorkerJoin.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerJoin_ProgressChanged);
            bgWorkerJoin.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerJoin_RunWorkerCompleted);

            _timer1 = new System.Windows.Forms.Timer(new System.ComponentModel.Container());
            _timer1.Interval = 1000;
            _timer1.Tick += new System.EventHandler(this.timer1_Tick);

            _timer1.Enabled = true;
            _timer1.Start();

            ImportFiles();
        }

        public void ImportFile()
        {
            //_view.ShowOpenFileDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sec++;

            _view.FormText = "Elapsed time: " + string.Format("{0:00}:{1:00}:{2:00}", hr, min, sec);

            if (sec == 59)
            {
                sec = 0;
                min++;
            }
            if (min == 59)
            {
                min = 0;
                hr++;
            }
        }

        public void ImportFiles()
        {
            //_view.ShowOpenFileDialog();

            if (!bgWorkerJoin.IsBusy)
            {
                _view.ViewText = $"Please wait";
                _view.CancelButtonEnabled = false;
                _view.OkButtonEnabled = false;

                _view.StatusVisible = true;
                _view.GroupBoxGridVisible = false;

                bgWorkerJoin.RunWorkerAsync(_fileNames);
            }
            else
            {
                //
            }
        }

        public void ReadFile(string fileName)
        {
            switch (Path.GetExtension(fileName))
            {
                case ".txt":
                case ".csv":
                    break;
                case ".xls":
                case ".xlsx":
                    
                    try
                    {
                        _local = ExcelToDataSet(fileName);

                        _view.Suggestions = CreateImportSuggestions();
                        _view.FileName = fileName;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    break;
            }
        }

        private bool CompareSchemas(DataTable table1, DataTable table2)
        {
            if (table1.Columns.Count != table2.Columns.Count)
                return false;

            foreach(DataColumn column in table1.Columns)
            {
                if (!table2.Columns.Contains(column.ColumnName))
                    return false;
            }

            return true;
        }

        private List<ImportSuggestion> CreateImportSuggestions()
        {
            List<ImportSuggestion> importSuggestions = new List<ImportSuggestion>();

            if (_local != null && _local.Tables != null)
            {
                foreach(DataTable newTable in _local.Tables)
                {
                    bool isNew = true;
                    

                    if (_dataSet != null && _dataSet.Tables.Count > 0)
                    {
                        for (int x = 0; x < _dataSet.Tables.Count; x++)
                        {
                            var existingTable = _dataSet.Tables[x];

                            // table exists
                            if (existingTable.TableName.ToLower() == newTable.TableName.ToLower())
                            {
                                // same structure
                                if (CompareSchemas(existingTable, newTable))
                                {
                                    string recordCountInNewTable = String.Format("{0:N0} {1}", newTable.Rows.Count, newTable.Rows.Count > 1 ? "records" : "record");
                                    string recordCountInExistingTable = String.Format("{0:N0} {1}", existingTable.Rows.Count, existingTable.Rows.Count > 1 ? "records" : "record");

                                    importSuggestions.Add(new ImportSuggestion
                                    {
                                        Name = newTable.TableName,
                                        OriginalTableName = newTable.TableName,
                                        Table = newTable,
                                        AvailableImportActions = new string[] {
                                            $"Import as new table ({recordCountInNewTable})",
                                            $"Append to - <{existingTable.TableName}> ({recordCountInExistingTable})",
                                            $"Overwrite existing - <{existingTable.TableName}> ({recordCountInExistingTable})",
                                            "Skip"
                                            },
                                        SelectedImportAction = "Skip"
                                    });

                                    isNew = false;
                                }
                            }

                        }//for
                    }

                    if (isNew)
                    {
                        string recordCount = String.Format("{0:N0} {1}", newTable.Rows.Count, newTable.Rows.Count > 1 ? "records" : "record");

                        importSuggestions.Add(new ImportSuggestion
                        {
                            Name = newTable.TableName,
                            OriginalTableName = newTable.TableName,
                            Table = newTable,
                            AvailableImportActions = new string[] {
                                $"Import ({recordCount})",
                                "Skip"
                            },
                            SelectedImportAction = "Skip"
                        });
                    }

                }//for
            }

            return importSuggestions;
        }



        private DataSet ExcelToDataSet(string filename)
        {
            using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader = null;
                string fileExtension = Path.GetExtension(filename);

                if (fileExtension == ".xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileExtension == ".xlsx")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                return excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
            } 
        }

        public void SelectDataTableFromName(string tableName)
        {
            //if (_dataSet != null && _dataSet.Tables.Contains(tableName))
             //   _view.Table = _dataSet.Tables[tableName];
        }

        private void BackgroundWorkerJoin_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string[] fileNames = (string[])e.Argument;

                if (fileNames != null)
                {
                    if (_local == null)
                        _local = new DataSet();

                    int fileCounter = 1;

                    foreach (var file in fileNames)
                    {
                        if (!bgWorkerJoin.CancellationPending)
                        {

                            switch (Path.GetExtension(file))
                            {
                                case ".txt":
                                case ".csv":
                                    break;
                                case ".xls":
                                case ".xlsx":
                                    bgWorkerJoin.ReportProgress(fileCounter, file);

                                    try
                                    {
                                        var dataSet = ExcelToDataSet(file);

                                        if (dataSet != null)
                                        {
                                            foreach (DataTable table in dataSet.Tables)
                                            {
                                                string tableName = table.TableName;
                                                int counter = 1;

                                                while (_local.Tables.Contains(tableName))
                                                {
                                                    tableName = $"{table.TableName} ({counter})";
                                                }

                                                table.TableName = tableName;
                                                
                                                _local.Tables.Add(table.Copy());
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("An error has occured while reading the file. " + ex.Message);
                                    }

                                    break;
                            }
                        } //if 
                        else
                        {
                            e.Cancel = true;
                        }

                        fileCounter++;

                    }// for
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("" + ex.Message);
            }

            
        }

        private void BackgroundWorkerJoin_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                _view.Status = $"Reading files(s):\n{e.ProgressPercentage} of {_fileNames.Length} - {e.UserState.ToString()}...";
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
                _timer1.Enabled = false;
                _timer1.Stop();

                _view.Suggestions = CreateImportSuggestions();
                //_view.FileName = fileName;

                _view.ViewText = "Select table(s) to import";
                
                _view.Status = string.Empty;
                _view.StatusVisible = false;

                _view.GroupBoxGridVisible = true;

                _view.CancelButtonEnabled = true;
                _view.OkButtonEnabled = true;
            }
        }
    }
}
