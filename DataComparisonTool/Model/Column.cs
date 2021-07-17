using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class Column
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [Browsable(false)]
        public bool IsKey { get; set; }

        public override string ToString()
        {
            return Name;
        }


    }
}
