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
    public partial class CustomMessageBox : Form
    {
        private string _message;
        private System.Windows.Forms.Timer _timer1;
        private int hr;
        private int min;
        private int sec;
        public CustomMessageBox()
        {
            InitializeComponent();

            _timer1 = new System.Windows.Forms.Timer(new System.ComponentModel.Container());
            _timer1.Interval = 1000;
            _timer1.Tick += new System.EventHandler(this.timer1_Tick);

            _timer1.Enabled = true;
            _timer1.Start();
        }

        public CustomMessageBox(string message, string caption = "Please wait")
        {
            InitializeComponent();

            this.Text = caption;

            _message = message;

            _timer1 = new System.Windows.Forms.Timer(new System.ComponentModel.Container());
            _timer1.Interval = 1000;
            _timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }

        private void CustomMessageBox_Load(object sender, EventArgs e)
        {
            _timer1.Stop();
            _timer1.Enabled = false;

            hr = 0;
            min = 0;
            sec = 0;

            _timer1.Enabled = true;
            _timer1.Start();

            //lblMessage.Text = _message;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sec++;

            this.Text = "Elapsed time: " + string.Format("{0:00}:{1:00}:{2:00}", hr, min, sec);

            if (sec == 59)
            {
                sec = 0;
                min++;
            }
            if (min == 59)
            {
                min = 0;
                hr++;
            }
        }

        public string Message
        {
            set
            {
                lblMessage.Text = value;
            }
        }

        public string Caption
        {
            set
            {
                this.Text = value;
            }
        }

        private void CustomMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer1.Stop();
            _timer1.Enabled = false;
            
            hr = 0;
            min = 0;
            sec = 0;
        }
    }
}
