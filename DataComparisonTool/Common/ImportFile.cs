using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Common
{
    public class ImportFile
    {
        public string FileName { get; set; }

        public string TableName { get; set; }

        public string Delimiter { get; set; }

        public string Qualifier { get; set; }

        public EnumImportFileTypes ImportFileTyle { get; set; }
    }
}
