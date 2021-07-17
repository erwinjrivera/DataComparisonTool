using DataComparisonTool.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Utils
{
    public class EqualityComparer : IEqualityComparer<DataRow>
    {
        private readonly List<FieldMapping> maps;

        public EqualityComparer(List<FieldMapping> maps)
        {
            this.maps = maps;
        }

        public bool Equals(DataRow x, DataRow y)
        {
            if (y.Table.DataSet.DataSetName  == "source")
                return maps.All(c => x[c.Target].Equals(y[c.Source]));
            else
                return maps.All(c => x[c.Source].Equals(y[c.Target]));
        }

        public int GetHashCode(DataRow obj)
        {
            unchecked
            {
                int hash = 19;

                if (obj.Table.TableName == "target")
                {
                    foreach (var value in maps.Select(cn => obj[cn.Target]))
                    {
                        hash = hash * 31 + value.GetHashCode();
                    }
                }
                else
                {
                    foreach (var value in maps.Select(cn => obj[cn.Source]))
                    {
                        hash = hash * 31 + value.GetHashCode();
                    }
                }

                return hash;
            }
        }
    }
}
