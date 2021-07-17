using DataComparisonTool.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataComparisonTool
{
    public partial class Form1 : Form
    {
        List<FieldMap> maps = new List<FieldMap>();
        List<Column> targets = new List<Column>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            targets.Add(new Column
            {
                Id = "1",
                Name = "Id"
            });
            targets.Add(new Column
            {
                Id = "2",
                Name = "Pangalan"
            });
            targets.Add(new Column
            {
                Id = "3",
                Name = "Title"
            });
            targets.Add(new Column
            {
                Id = "4",
                Name = "Description"
            });

            dataGridView2.DataSource = targets;



           

            maps.Add(new FieldMap
            {
                Id = "1",
                Source = new Column
                {
                    Id = "1",
                    Name = "Id"
                },
            });
            maps.Add(new FieldMap
            {
                Id = "2",
                Source = new Column
                {
                    Id = "2",
                    Name = "Name"
                },
            });
            maps.Add(new FieldMap
            {
                Id = "3",
                Source = new Column
                {
                    Id = "3",
                    Name = "Title"
                },
            });
            maps.Add(new FieldMap
            {
                Id = "4",
                Source = new Column
                {
                    Id = "4",
                    Name = "Description"
                },
            });

            //maps.Select(x => x.Target.Name)

            dataGridView1.DataSource = maps;

            /*
             * var query = (from left in leftList
                        join right in rightList on left.Id equals right.Id into joinedList
                        from sub in joinedList.DefaultIfEmpty()
                        select new Person { 
                            Id = left.Id,
                            Name = left.Name,
                            Changed = sub == null ? left.Changed : sub.Changed }).ToList();
             */

            //1st approach
            //maps.ForEach(m => m.Target = target.FirstOrDefault(t => m.Source.Name == t.Name));

            //2nd
            //var d = target.ToDictionary(t => t.Name);
            //maps.ForEach(m => m.Target = d.ContainsKey(m.Source.Name) ? d[m.Source.Name] : null);

            //3rd
            /*
            var dict = target
                .GroupBy(map => map.Name)
                .ToDictionary(m => m.Key, m => m.First());

            foreach (var map in maps)
            {
                if (dict.TryGetValue(map.Source.Name, out var t))
                {
                    map.Target = t;
                }
            }
            */

            //4th
            /*
            Dictionary<string, Column> t = target.ToDictionary(x => x.Name, x => x);

            foreach (var map in maps)
                if (t.ContainsKey(map.Source.Name))
                    map.Target = t[map.Source.Name];
            */

            maps.ForEach(m => m.Target = targets.FirstOrDefault(t => m.Source.Name == t.Name));
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var x = dataGridView2.SelectedRows[0].DataBoundItem;

            var y = dataGridView1.SelectedRows[0].DataBoundItem;

            ((FieldMap)y).Target = (Column)x;


            //maps.ForEach(m => m.Target = target.FirstOrDefault(t => m.Source.Name == t.Name));

            /*
            Dictionary<string, Column> t = targets.ToDictionary(x => x.Name, x => x);

            foreach (var map in maps)
            {
                if (t.ContainsKey(map.Source.Name))
                {
                    var tf = t[map.Source.Name];

                    if (!tf.IsMapped)
                    {
                        map.Target = tf;
                        map.Source.IsMapped = true;

                        tf.IsMapped = true;
                    }
                }
            }
            */

            dataGridView1.Refresh();


            var table1 = CreateDataTable();
            table1.TableName = "source";

            var row = table1.NewRow();
            row[0] = "1";
            row[1] = "Name1";
            row[2] = "from table1 description1";
            table1.Rows.Add(row);

            row = table1.NewRow();
            row[0] = "2";
            row[1] = "Name2";
            row[2] = "from table1 description2";
            table1.Rows.Add(row);

            row = table1.NewRow();
            row[0] = "3";
            row[1] = "Name3";
            row[2] = "from table1 description3";
            table1.Rows.Add(row);

            row = table1.NewRow();
            row[0] = "4";
            row[1] = "XXX";
            row[2] = "from table1 description4";
            table1.Rows.Add(row);




            var table2 = CreateDataTable2();
            table2.TableName = "target";

            row = table2.NewRow();
            row[0] = "1";
            row[1] = "Name 1";
            row[2] = "tab2 desc 1";
            table2.Rows.Add(row);

            row = table2.NewRow();
            row[0] = "2";
            row[1] = "Name2";
            row[2] = "tab2 desc 2";
            table2.Rows.Add(row);

            row = table2.NewRow();
            row[0] = "3";
            row[1] = "Name3";
            row[2] = "tab2 desc 3";
            table2.Rows.Add(row);

            row = table2.NewRow();
            row[0] = "4";
            row[1] = "XXX";
            row[2] = "tab2 desc 4";
            table2.Rows.Add(row);

            row = table2.NewRow();
            row[0] = "5";
            row[1] = "Name5";
            row[2] = "tab2 desc 5";
            table2.Rows.Add(row);


            //var l = LeftOuterJoin(new List<string> { "Id", "Name", "Title" }, table1, table2);

            var r = LeftOuterJoin2(new List<FieldMap> { maps[0], maps[1] }, table1, table2);

            //maps.ForEach(m => )
        }

        private System.Data.DataTable CreateDataTable()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Description", typeof(string));
      
            return table;
        }

        private System.Data.DataTable CreateDataTable2()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("Pangalan", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Description", typeof(string));

            return table;
        }

        public class MyEqualityComparer : IEqualityComparer<DataRow>
        {
            private readonly string[] columnNames;

            public MyEqualityComparer(string[] columnNames)
            {
                this.columnNames = columnNames;
            }

            public bool Equals(DataRow x, DataRow y)
            {
                return columnNames.All(cn => x[cn].Equals(y[cn]));
            }

            public int GetHashCode(DataRow obj)
            {
                unchecked
                {
                    int hash = 19;
                    foreach (var value in columnNames.Select(cn => obj[cn]))
                    {
                        hash = hash * 31 + value.GetHashCode();
                    }
                    return hash;
                }
            }
        }

        public class MyEqualityComparer2 : IEqualityComparer<DataRow>
        {
            private readonly List<FieldMap> maps;

            public MyEqualityComparer2(List<FieldMap> maps)
            {
                this.maps = maps;
            }

            public bool Equals(DataRow x, DataRow y)
            {
                if (y.Table.TableName == "source")
                    return maps.All(c => x[c.Target.Name].Equals(y[c.Source.Name]));
                else
                    return maps.All(c => x[c.Source.Name].Equals(y[c.Target.Name]));


                //return columnNames.All(cn => x[cn].Equals(y[cn]));
            }

            public int GetHashCode(DataRow obj)
            {
                unchecked
                {
                    int hash = 19;

                    if (obj.Table.TableName == "target")
                    {
                        foreach (var value in maps.Select(cn => obj[cn.Target.Name]))
                        {
                            hash = hash * 31 + value.GetHashCode();
                        }
                    }
                    else
                    {
                        foreach (var value in maps.Select(cn => obj[cn.Source.Name]))
                        {
                            hash = hash * 31 + value.GetHashCode();
                        }
                    }
                    
                    return hash;
                }
            }
        }

        public class TwoRows
        {
            public DataRow Row1 { get; set; }
            public DataRow Row2 { get; set; }
        }

        private static List<TwoRows> LeftOuterJoin(
            List<string> joinColumnNames,
            DataTable leftTable,
            DataTable rightTable)
        {
            return leftTable
                .AsEnumerable()
                .GroupJoin(
                    rightTable.AsEnumerable(),
                    l => l,
                    r => r,
                    (l, rlist) => new { LeftValue = l, RightValues = rlist },
                    new MyEqualityComparer(joinColumnNames.ToArray()))
                .SelectMany(
                    x => x.RightValues.DefaultIfEmpty(rightTable.NewRow()),
                    (x, y) => new TwoRows { Row1 = x.LeftValue, Row2 = y })
                .ToList();
        }

        private static List<TwoRows> LeftOuterJoin2(
           List<FieldMap> maps,
           DataTable leftTable,
           DataTable rightTable)
        {
            return leftTable
                .AsEnumerable()
                .GroupJoin(
                    rightTable.AsEnumerable(),
                    l => l,
                    r => r,
                    (l, rlist) => new { LeftValue = l, RightValues = rlist },
                    new MyEqualityComparer2(maps))
                .SelectMany(
                    x => x.RightValues.DefaultIfEmpty(rightTable.NewRow()),
                    (x, y) => new TwoRows { Row1 = x.LeftValue, Row2 = y })
                .ToList();
        }

    }
}
