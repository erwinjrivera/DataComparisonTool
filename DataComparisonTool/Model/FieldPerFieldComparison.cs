using DataComparisonTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class FieldPerFieldComparison
    {
        private List<ValueMatch> _values = new List<ValueMatch>();

        [Browsable(false)]
        public Field SourceField { get; set; }

        [Browsable(false)]
        public Field TargetField { get; set; }



        [DisplayName("Field Name (Source)")]
        public string SourceFieldName
        {
            get
            {
                return SourceField.ToString();
            }
        }

        [DisplayName("Mapped to (Target Field)")]
        public string TargetFieldName
        {
            get
            {
                return TargetField.ToString();
            }
        }

        [DisplayName("Matching Values Count")]
        public string MatchCount
        {
            get
            {
                return String.Format("{0:N0}", Exact);
            }
        }

        [DisplayName("Non-Matching Values Count")]
        public string NonMatchCount
        {
            get
            {
                return String.Format("{0:N0}", None);
            }
        }

        [DisplayName("Percentage of Matching Values")]
        public string PercentSuccessMatch
        {
            get
            {
                return $"{(int)Math.Round( (double)(100 * Exact) / Total )}%";
            }
        }






        public List<ValueMatch> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        [Browsable(false)]
        public int Exact
        {
            get
            {
                return _values.Count(x => x.IsEqual());
            }
        }

        [Browsable(false)]
        public int None
        {
            get
            {
                return _values.Count(x => !x.IsEqual());
            }
        }

        [Browsable(false)]
        public int Total
        {
            get
            {
                return _values.Count;
            }
        }
    }
}
