using DataComparisonTool.Common;
using DataComparisonTool.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.View
{
    public interface IFileImportView
    {
        void ShowOpenFileDialog();

        List<ImportSuggestion> Suggestions { set; }

        string FileName { set; }

        object Data { get; }

        string FormText { set; }

        string ViewText { set; }

        bool CancelButtonEnabled { set; }

        bool OkButtonEnabled { set; }

        bool GroupBoxGridVisible { set; }

        string Status { set; }

        bool StatusVisible { set; }
    }
}
