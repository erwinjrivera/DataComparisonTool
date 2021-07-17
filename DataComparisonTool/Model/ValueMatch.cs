using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class ValueMatch
    {
        public List<RecordKey> RecordKeys { get; set; }

        public Field SourceField { get; set; }

        public Field TargetField { get; set; }

        public object SourceValue { get; set; }

        public object TargetValue { get; set; }

        public bool IsEqual()
        {
            return SourceValue.Equals(TargetValue);
        }
    }
}
