using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Common
{
    public enum EnumImportFileTypes { Source = 1, Target }

    public static class Enums
    {
        public static EnumImportFileTypes GetImportFileType(string name)
        {
            return (EnumImportFileTypes)Enum.Parse(typeof(EnumImportFileTypes), name, true);
        }
    }
}
