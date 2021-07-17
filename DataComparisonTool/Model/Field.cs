using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class Field : DataColumn
    {
        private Guid _id;

        public Field(DataColumn dataColumn)
        {
            _id = Guid.NewGuid();

            FromDataColumn(dataColumn);
        }

        public Guid Id 
        { 
            get
            {
                return _id;
            }
        }

        public bool IsKey { get; set; }

        public string TableName { get; set; }

        public int Position { get; set; }

        public string DataSetName { get; set; }

        public override string ToString()
        {
            return ColumnName;
        }

        public void FromDataColumn(DataColumn dataColumn)
        {
            if (_id == null)
                _id = Guid.NewGuid();

            ColumnName = dataColumn.ColumnName;
            DataType = dataColumn.DataType;
            Caption = dataColumn.Caption;
            TableName = dataColumn.Table.TableName;
            Position = dataColumn.Ordinal;
            DataSetName = dataColumn.Table.DataSet.DataSetName;
        }
    }
}
