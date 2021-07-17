
namespace DataComparisonTool
{
    partial class UserControlReport
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlReport));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblMappedFieldsSource = new System.Windows.Forms.LinkLabel();
            this.lblMappedFieldsTarget = new System.Windows.Forms.LinkLabel();
            this.lblKeyFieldsCount = new System.Windows.Forms.LinkLabel();
            this.lblNoneCount = new System.Windows.Forms.LinkLabel();
            this.lblSingleCount = new System.Windows.Forms.LinkLabel();
            this.lblMultipleCount = new System.Windows.Forms.LinkLabel();
            this.lblSourceNotInTarget = new System.Windows.Forms.LinkLabel();
            this.lblTargetNonInSource = new System.Windows.Forms.LinkLabel();
            this.lblCommonRecords = new System.Windows.Forms.LinkLabel();
            this.lblDuplicateRecordsSource = new System.Windows.Forms.LinkLabel();
            this.lblDuplicateRecordsTarget = new System.Windows.Forms.LinkLabel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.lblFieldsCompared = new System.Windows.Forms.LinkLabel();
            this.lblNonMatchingValues = new System.Windows.Forms.LinkLabel();
            this.lblFieldPerFieldSummary = new System.Windows.Forms.LinkLabel();
            this.lblRecordCountSource = new System.Windows.Forms.TextBox();
            this.lblRecordCountTarget = new System.Windows.Forms.TextBox();
            this.lblFieldCountSource = new System.Windows.Forms.LinkLabel();
            this.lblFieldCountTarget = new System.Windows.Forms.LinkLabel();
            this.lblSourceTableName = new System.Windows.Forms.TextBox();
            this.lblTargetTableName = new System.Windows.Forms.TextBox();
            this.lblFieldsNoMapTarget = new System.Windows.Forms.LinkLabel();
            this.lblFieldsNoMapSource = new System.Windows.Forms.LinkLabel();
            this.lblDuplicateHandling = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(15, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1021, 783);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblMappedFieldsSource
            // 
            this.lblMappedFieldsSource.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblMappedFieldsSource.Location = new System.Drawing.Point(229, 319);
            this.lblMappedFieldsSource.Name = "lblMappedFieldsSource";
            this.lblMappedFieldsSource.Size = new System.Drawing.Size(120, 14);
            this.lblMappedFieldsSource.TabIndex = 5;
            this.lblMappedFieldsSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMappedFieldsSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblMappedFieldsSource_LinkClicked);
            // 
            // lblMappedFieldsTarget
            // 
            this.lblMappedFieldsTarget.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblMappedFieldsTarget.Location = new System.Drawing.Point(354, 319);
            this.lblMappedFieldsTarget.Name = "lblMappedFieldsTarget";
            this.lblMappedFieldsTarget.Size = new System.Drawing.Size(108, 13);
            this.lblMappedFieldsTarget.TabIndex = 6;
            this.lblMappedFieldsTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMappedFieldsTarget.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblMappedFieldsTarget_LinkClicked);
            // 
            // lblKeyFieldsCount
            // 
            this.lblKeyFieldsCount.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblKeyFieldsCount.Location = new System.Drawing.Point(230, 493);
            this.lblKeyFieldsCount.Name = "lblKeyFieldsCount";
            this.lblKeyFieldsCount.Size = new System.Drawing.Size(118, 13);
            this.lblKeyFieldsCount.TabIndex = 9;
            this.lblKeyFieldsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblKeyFieldsCount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblKeyFieldsCount_LinkClicked);
            // 
            // lblNoneCount
            // 
            this.lblNoneCount.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNoneCount.Location = new System.Drawing.Point(353, 493);
            this.lblNoneCount.Name = "lblNoneCount";
            this.lblNoneCount.Size = new System.Drawing.Size(108, 13);
            this.lblNoneCount.TabIndex = 10;
            this.lblNoneCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNoneCount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblNoneCount_LinkClicked);
            // 
            // lblSingleCount
            // 
            this.lblSingleCount.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblSingleCount.Location = new System.Drawing.Point(466, 493);
            this.lblSingleCount.Name = "lblSingleCount";
            this.lblSingleCount.Size = new System.Drawing.Size(100, 13);
            this.lblSingleCount.TabIndex = 11;
            this.lblSingleCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSingleCount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSingleCount_LinkClicked);
            // 
            // lblMultipleCount
            // 
            this.lblMultipleCount.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblMultipleCount.Location = new System.Drawing.Point(569, 493);
            this.lblMultipleCount.Name = "lblMultipleCount";
            this.lblMultipleCount.Size = new System.Drawing.Size(99, 13);
            this.lblMultipleCount.TabIndex = 12;
            this.lblMultipleCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMultipleCount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblMultipleCount_LinkClicked);
            // 
            // lblSourceNotInTarget
            // 
            this.lblSourceNotInTarget.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblSourceNotInTarget.Location = new System.Drawing.Point(229, 581);
            this.lblSourceNotInTarget.Name = "lblSourceNotInTarget";
            this.lblSourceNotInTarget.Size = new System.Drawing.Size(335, 13);
            this.lblSourceNotInTarget.TabIndex = 13;
            this.lblSourceNotInTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSourceNotInTarget.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSourceNotInTarget_LinkClicked);
            // 
            // lblTargetNonInSource
            // 
            this.lblTargetNonInSource.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblTargetNonInSource.Location = new System.Drawing.Point(570, 581);
            this.lblTargetNonInSource.Name = "lblTargetNonInSource";
            this.lblTargetNonInSource.Size = new System.Drawing.Size(300, 13);
            this.lblTargetNonInSource.TabIndex = 14;
            this.lblTargetNonInSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTargetNonInSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblTargetNonInSource_LinkClicked);
            // 
            // lblCommonRecords
            // 
            this.lblCommonRecords.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblCommonRecords.Location = new System.Drawing.Point(877, 581);
            this.lblCommonRecords.Name = "lblCommonRecords";
            this.lblCommonRecords.Size = new System.Drawing.Size(145, 13);
            this.lblCommonRecords.TabIndex = 15;
            this.lblCommonRecords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCommonRecords.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCommonRecords_LinkClicked);
            // 
            // lblDuplicateRecordsSource
            // 
            this.lblDuplicateRecordsSource.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblDuplicateRecordsSource.Location = new System.Drawing.Point(234, 668);
            this.lblDuplicateRecordsSource.Name = "lblDuplicateRecordsSource";
            this.lblDuplicateRecordsSource.Size = new System.Drawing.Size(111, 13);
            this.lblDuplicateRecordsSource.TabIndex = 16;
            this.lblDuplicateRecordsSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDuplicateRecordsSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblDuplicateRecordsSource_LinkClicked);
            // 
            // lblDuplicateRecordsTarget
            // 
            this.lblDuplicateRecordsTarget.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblDuplicateRecordsTarget.Location = new System.Drawing.Point(352, 668);
            this.lblDuplicateRecordsTarget.Name = "lblDuplicateRecordsTarget";
            this.lblDuplicateRecordsTarget.Size = new System.Drawing.Size(110, 13);
            this.lblDuplicateRecordsTarget.TabIndex = 17;
            this.lblDuplicateRecordsTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDuplicateRecordsTarget.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblDuplicateRecordsTarget_LinkClicked);
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.BackColor = System.Drawing.Color.LemonChiffon;
            this.linkLabel6.Location = new System.Drawing.Point(660, 581);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(0, 13);
            this.linkLabel6.TabIndex = 18;
            this.linkLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFieldsCompared
            // 
            this.lblFieldsCompared.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFieldsCompared.Location = new System.Drawing.Point(231, 754);
            this.lblFieldsCompared.Name = "lblFieldsCompared";
            this.lblFieldsCompared.Size = new System.Drawing.Size(115, 13);
            this.lblFieldsCompared.TabIndex = 19;
            this.lblFieldsCompared.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFieldsCompared.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFieldsCompared_LinkClicked);
            // 
            // lblNonMatchingValues
            // 
            this.lblNonMatchingValues.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNonMatchingValues.Location = new System.Drawing.Point(364, 754);
            this.lblNonMatchingValues.Name = "lblNonMatchingValues";
            this.lblNonMatchingValues.Size = new System.Drawing.Size(185, 13);
            this.lblNonMatchingValues.TabIndex = 20;
            this.lblNonMatchingValues.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNonMatchingValues.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblNonMatchingValues_LinkClicked);
            // 
            // lblFieldPerFieldSummary
            // 
            this.lblFieldPerFieldSummary.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFieldPerFieldSummary.Location = new System.Drawing.Point(571, 754);
            this.lblFieldPerFieldSummary.Name = "lblFieldPerFieldSummary";
            this.lblFieldPerFieldSummary.Size = new System.Drawing.Size(170, 13);
            this.lblFieldPerFieldSummary.TabIndex = 21;
            this.lblFieldPerFieldSummary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFieldPerFieldSummary.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSeeReport_LinkClicked);
            // 
            // lblRecordCountSource
            // 
            this.lblRecordCountSource.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblRecordCountSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblRecordCountSource.Location = new System.Drawing.Point(230, 58);
            this.lblRecordCountSource.Name = "lblRecordCountSource";
            this.lblRecordCountSource.ReadOnly = true;
            this.lblRecordCountSource.Size = new System.Drawing.Size(118, 13);
            this.lblRecordCountSource.TabIndex = 22;
            this.lblRecordCountSource.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblRecordCountTarget
            // 
            this.lblRecordCountTarget.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblRecordCountTarget.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblRecordCountTarget.Location = new System.Drawing.Point(352, 58);
            this.lblRecordCountTarget.Name = "lblRecordCountTarget";
            this.lblRecordCountTarget.ReadOnly = true;
            this.lblRecordCountTarget.Size = new System.Drawing.Size(109, 13);
            this.lblRecordCountTarget.TabIndex = 23;
            this.lblRecordCountTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblFieldCountSource
            // 
            this.lblFieldCountSource.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFieldCountSource.Location = new System.Drawing.Point(229, 145);
            this.lblFieldCountSource.Name = "lblFieldCountSource";
            this.lblFieldCountSource.Size = new System.Drawing.Size(120, 13);
            this.lblFieldCountSource.TabIndex = 26;
            this.lblFieldCountSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFieldCountSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFieldCountSource_LinkClicked);
            // 
            // lblFieldCountTarget
            // 
            this.lblFieldCountTarget.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFieldCountTarget.Location = new System.Drawing.Point(352, 145);
            this.lblFieldCountTarget.Name = "lblFieldCountTarget";
            this.lblFieldCountTarget.Size = new System.Drawing.Size(110, 13);
            this.lblFieldCountTarget.TabIndex = 27;
            this.lblFieldCountTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFieldCountTarget.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFieldCountTarget_LinkClicked);
            // 
            // lblSourceTableName
            // 
            this.lblSourceTableName.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblSourceTableName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblSourceTableName.Location = new System.Drawing.Point(230, 233);
            this.lblSourceTableName.Name = "lblSourceTableName";
            this.lblSourceTableName.ReadOnly = true;
            this.lblSourceTableName.Size = new System.Drawing.Size(118, 13);
            this.lblSourceTableName.TabIndex = 28;
            this.lblSourceTableName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTargetTableName
            // 
            this.lblTargetTableName.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblTargetTableName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblTargetTableName.Location = new System.Drawing.Point(352, 233);
            this.lblTargetTableName.Name = "lblTargetTableName";
            this.lblTargetTableName.ReadOnly = true;
            this.lblTargetTableName.Size = new System.Drawing.Size(109, 13);
            this.lblTargetTableName.TabIndex = 29;
            this.lblTargetTableName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblFieldsNoMapTarget
            // 
            this.lblFieldsNoMapTarget.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFieldsNoMapTarget.Location = new System.Drawing.Point(354, 406);
            this.lblFieldsNoMapTarget.Name = "lblFieldsNoMapTarget";
            this.lblFieldsNoMapTarget.Size = new System.Drawing.Size(108, 13);
            this.lblFieldsNoMapTarget.TabIndex = 31;
            this.lblFieldsNoMapTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFieldsNoMapTarget.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFieldsNoMapTarget_LinkClicked);
            // 
            // lblFieldsNoMapSource
            // 
            this.lblFieldsNoMapSource.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFieldsNoMapSource.Location = new System.Drawing.Point(229, 406);
            this.lblFieldsNoMapSource.Name = "lblFieldsNoMapSource";
            this.lblFieldsNoMapSource.Size = new System.Drawing.Size(120, 14);
            this.lblFieldsNoMapSource.TabIndex = 30;
            this.lblFieldsNoMapSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFieldsNoMapSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFieldsNoMapSource_LinkClicked);
            // 
            // lblDuplicateHandling
            // 
            this.lblDuplicateHandling.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblDuplicateHandling.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblDuplicateHandling.Location = new System.Drawing.Point(469, 668);
            this.lblDuplicateHandling.Name = "lblDuplicateHandling";
            this.lblDuplicateHandling.ReadOnly = true;
            this.lblDuplicateHandling.Size = new System.Drawing.Size(400, 13);
            this.lblDuplicateHandling.TabIndex = 32;
            this.lblDuplicateHandling.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UserControlReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblDuplicateHandling);
            this.Controls.Add(this.lblFieldsNoMapTarget);
            this.Controls.Add(this.lblFieldsNoMapSource);
            this.Controls.Add(this.lblTargetTableName);
            this.Controls.Add(this.lblSourceTableName);
            this.Controls.Add(this.lblFieldCountTarget);
            this.Controls.Add(this.lblFieldCountSource);
            this.Controls.Add(this.lblRecordCountTarget);
            this.Controls.Add(this.lblRecordCountSource);
            this.Controls.Add(this.lblFieldPerFieldSummary);
            this.Controls.Add(this.lblNonMatchingValues);
            this.Controls.Add(this.lblFieldsCompared);
            this.Controls.Add(this.linkLabel6);
            this.Controls.Add(this.lblDuplicateRecordsTarget);
            this.Controls.Add(this.lblDuplicateRecordsSource);
            this.Controls.Add(this.lblCommonRecords);
            this.Controls.Add(this.lblTargetNonInSource);
            this.Controls.Add(this.lblSourceNotInTarget);
            this.Controls.Add(this.lblMultipleCount);
            this.Controls.Add(this.lblSingleCount);
            this.Controls.Add(this.lblNoneCount);
            this.Controls.Add(this.lblKeyFieldsCount);
            this.Controls.Add(this.lblMappedFieldsTarget);
            this.Controls.Add(this.lblMappedFieldsSource);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.Name = "UserControlReport";
            this.Size = new System.Drawing.Size(1047, 832);
            this.Load += new System.EventHandler(this.UserControlReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel lblMappedFieldsSource;
        private System.Windows.Forms.LinkLabel lblMappedFieldsTarget;
        private System.Windows.Forms.LinkLabel lblKeyFieldsCount;
        private System.Windows.Forms.LinkLabel lblNoneCount;
        private System.Windows.Forms.LinkLabel lblSingleCount;
        private System.Windows.Forms.LinkLabel lblMultipleCount;
        private System.Windows.Forms.LinkLabel lblSourceNotInTarget;
        private System.Windows.Forms.LinkLabel lblTargetNonInSource;
        private System.Windows.Forms.LinkLabel lblCommonRecords;
        private System.Windows.Forms.LinkLabel lblDuplicateRecordsSource;
        private System.Windows.Forms.LinkLabel lblDuplicateRecordsTarget;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel lblFieldsCompared;
        private System.Windows.Forms.LinkLabel lblNonMatchingValues;
        private System.Windows.Forms.LinkLabel lblFieldPerFieldSummary;
        private System.Windows.Forms.TextBox lblRecordCountSource;
        private System.Windows.Forms.TextBox lblRecordCountTarget;
        private System.Windows.Forms.LinkLabel lblFieldCountSource;
        private System.Windows.Forms.LinkLabel lblFieldCountTarget;
        private System.Windows.Forms.TextBox lblSourceTableName;
        private System.Windows.Forms.TextBox lblTargetTableName;
        private System.Windows.Forms.LinkLabel lblFieldsNoMapTarget;
        private System.Windows.Forms.LinkLabel lblFieldsNoMapSource;
        private System.Windows.Forms.TextBox lblDuplicateHandling;
    }
}
