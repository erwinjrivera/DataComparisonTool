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
    public partial class ConfigurationForm : Form
    {
        private Configuration _config = new Configuration();

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            _config.ExportReportsAs = Properties.Settings.Default.ExportReportsAs;
            _config.DefaultDuplicateRecordHandling = Properties.Settings.Default.DefaultDuplicateRecordHandling;

            propertyGrid1.SelectedObject = _config;
        }

        private void ConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ExportReportsAs = _config.ExportReportsAs;
            Properties.Settings.Default.DefaultDuplicateRecordHandling = _config.DefaultDuplicateRecordHandling;

            Properties.Settings.Default.Save();
        }
    }
}
