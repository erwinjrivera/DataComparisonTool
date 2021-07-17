using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class MatchingResult
    {
        public DataRow Left { get; set; }

        public IEnumerable<DataRow> Matches { get; set; }
    }
}
