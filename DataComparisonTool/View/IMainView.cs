using DataComparisonTool.Common;
using DataComparisonTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataComparisonTool.View
{
    public interface IMainView
    {
        object ShowImportFileDialog(Form form);

        void AddTreeNodeToSource(string name, string details);

        void UpdateTreeNodeToSource(string name, string details);

        void AddItemToSourceList(string name);

        void RefreshSourceCount();

        void InitializeDataGridViews();

        void AddTreeNodeToTarget(string name, string details);

        void UpdateTreeNodeToTarget(string name, string details);

        void AddItemToTargetList(string name);

        void RefreshTargetCount();

        int SelectedTab { get; }

        object SourcePreviewDataSource { set; }

        object TargetPreviewDataSource { set; }

        string SelectedSourceTable { get; set; }

        string SelectedTargetTable { get; set; }

        string GetSelectedSourceTable();

        string GetSelectedTargetTable();

        object SelectedSourceObject { get; }

        object SelectedTargetObject { get; }

        object SourceColumnsDataSource { set; }

        object TargetColumnsDataSource { set; }

        object MatchingFieldsDataSource { set; }

        object SourceRecordsDataSource { set; }

        object TargetRecordsDataSource { set; }

        object FieldPerFieldsDataSource { set; }

        bool MapButtonEnabled { set; }

        bool UnmapButtonEnabled { set; }

        bool ApplyRefreshMatchingButtonEnabled { set; }

        int SelectedRowsCount(string dataGridViewName);

        void UpdateMapping(FieldMapping source, FieldMapping target, string action);

        int RowsCount(string dataGridViewName);

        bool ConfirmChangeTab();

        void ResetSelectedSourceTable();

        void ResetSelectedTargetTable();

        void HighlightSourceKeyFields(string[] columnNames);

        void DisplayCounts(int none, int single, int multiple);

        void ShowCustomMessageBox(string message, string caption);

        void CloseCustomMessageBox();

        string StatusMessage { set; }

        void ShowMessageBoxOk(string message, string caption);

        bool ShowMessageYesNoBox(string message, string caption);

        string[] ShowOpenFileDialog();

        void ShowReport(ReportData report);
    }
}
