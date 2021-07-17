using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class ReportData
    {
        private string _sourceTableName;

        private string _targetTableName;

        private DataSet _sourceDataSet;

        private DataSet _targetDataSet;

        private Map _mapping;

        private List<MatchingResult> _matchingResult;

        private List<FieldPerFieldComparison> _comparisonResult;


        public string  SourceTableName 
        { 
            get { return _sourceTableName;  }
            set
            {
                _sourceTableName = value;
            }
        }

        public string TargetTableName 
        { 
            get { return _targetTableName;  }
            set
            {
                _targetTableName = value;
            }
        }

        public DataSet SourceDataSet 
        { 
            get { return _sourceDataSet; }
            set
            {
                _sourceDataSet = value;
            }
        }

        public DataSet TargetDataSet 
        { 
            get { return _targetDataSet; }
            set
            {
                _targetDataSet = value;
            }
        }

        public Map Mapping 
        { 
            get { return _mapping; }
            set
            {
                _mapping = value;
            }
        }

        public List<MatchingResult> MatchResult 
        { 
            get { return _matchingResult; }
            set
            {
                _matchingResult = value;
            }
        }

        public List<FieldPerFieldComparison> ComparisonResult 
        { 
            get { return _comparisonResult; }
            set
            {
                _comparisonResult = value;  
            }
        }

        public List<MatchingResult> MatchResultRightTable { get; set; }
    }
}
