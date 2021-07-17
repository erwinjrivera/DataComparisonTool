using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Utils
{
    public class DefaultDuplicateRecordConverter : StringConverter
    {
        public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<String> list = new List<String>();
            list.Add("");
            list.Add("Retain and consider the first occurence when matching records. Remove other duplicates.");
            list.Add("Remove all. Duplicate records will not be considered when matching records.");
            list.Add("Preserve all. All duplicate records are considered when matching records.");
            return new StandardValuesCollection(list);
        }
    }
}
