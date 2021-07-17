using DataComparisonTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class Configuration
    {
        public class Application
        {
            
        }

        [Category("Matching Records")]
        [DisplayName("Default Duplicate Record Handling")]
        [Description("")]
        [DefaultValue("")]
        [TypeConverter(typeof(DefaultDuplicateRecordConverter))]
        public string DefaultDuplicateRecordHandling { get; set; }


        [Category("Report")]
        [DisplayName("Export Reports As")]
        [Description("")]
        [DefaultValue("Tab-delimited text file (*.txt)")]
        [TypeConverter(typeof(DefaultFileExtensionForReportsConverter))]
        public string ExportReportsAs { get; set; }
    }
}
