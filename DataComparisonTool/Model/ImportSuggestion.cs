using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class ImportSuggestion
    {
        public string Name { get; set; }

        [Browsable(false)]
        public string OriginalTableName { get; set; }

        public string[] AvailableImportActions { get; set; }

        [Browsable(false)]
        public string SelectedImportAction { get; set; }

        [Browsable(false)]
        public DataTable Table { get; set; }
    }

}
