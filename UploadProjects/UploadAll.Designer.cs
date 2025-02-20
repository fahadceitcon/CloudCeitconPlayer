namespace UploadProjects
{
    partial class UploadAll
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LoadFile = new OpenFileDialog();
            BtnUpload = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            DTStartDate = new DateTimePicker();
            DTEndDate = new DateTimePicker();
            DTStartTime = new DateTimePicker();
            DTEndTime = new DateTimePicker();
            NumDuration = new NumericUpDown();
            GV_Schedules = new DataGridView();
            Select = new DataGridViewCheckBoxColumn();
            UniqueId = new DataGridViewTextBoxColumn();
            ProjectName = new DataGridViewTextBoxColumn();
            Version = new DataGridViewTextBoxColumn();
            StartTime = new DataGridViewTextBoxColumn();
            EndTime = new DataGridViewTextBoxColumn();
            LocationGroup = new DataGridViewTextBoxColumn();
            Location = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)NumDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)GV_Schedules).BeginInit();
            SuspendLayout();
            // 
            // LoadFile
            // 
            LoadFile.FileName = "LoadFile";
            LoadFile.Multiselect = true;
            // 
            // BtnUpload
            // 
            BtnUpload.Location = new Point(714, 111);
            BtnUpload.Name = "BtnUpload";
            BtnUpload.Size = new Size(248, 29);
            BtnUpload.TabIndex = 0;
            BtnUpload.Text = "Upload File";
            BtnUpload.UseVisualStyleBackColor = true;
            BtnUpload.Click += BtnUpload_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 21);
            label1.Name = "label1";
            label1.Size = new Size(76, 20);
            label1.TabIndex = 1;
            label1.Text = "Start Date";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 65);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 2;
            label2.Text = "End Date";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(570, 21);
            label3.Name = "label3";
            label3.Size = new Size(77, 20);
            label3.TabIndex = 3;
            label3.Text = "Start Time";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(570, 65);
            label4.Name = "label4";
            label4.Size = new Size(71, 20);
            label4.TabIndex = 4;
            label4.Text = "End Time";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(17, 113);
            label5.Name = "label5";
            label5.Size = new Size(67, 20);
            label5.TabIndex = 5;
            label5.Text = "Duration";
            // 
            // DTStartDate
            // 
            DTStartDate.Location = new Point(161, 16);
            DTStartDate.Name = "DTStartDate";
            DTStartDate.Size = new Size(248, 27);
            DTStartDate.TabIndex = 7;
            // 
            // DTEndDate
            // 
            DTEndDate.Location = new Point(161, 60);
            DTEndDate.Name = "DTEndDate";
            DTEndDate.Size = new Size(248, 27);
            DTEndDate.TabIndex = 8;
            // 
            // DTStartTime
            // 
            DTStartTime.Format = DateTimePickerFormat.Time;
            DTStartTime.Location = new Point(714, 16);
            DTStartTime.Name = "DTStartTime";
            DTStartTime.ShowUpDown = true;
            DTStartTime.Size = new Size(248, 27);
            DTStartTime.TabIndex = 9;
            // 
            // DTEndTime
            // 
            DTEndTime.Format = DateTimePickerFormat.Time;
            DTEndTime.Location = new Point(714, 60);
            DTEndTime.Name = "DTEndTime";
            DTEndTime.ShowUpDown = true;
            DTEndTime.Size = new Size(248, 27);
            DTEndTime.TabIndex = 10;
            // 
            // NumDuration
            // 
            NumDuration.Location = new Point(161, 111);
            NumDuration.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            NumDuration.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            NumDuration.Name = "NumDuration";
            NumDuration.Size = new Size(248, 27);
            NumDuration.TabIndex = 11;
            NumDuration.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // GV_Schedules
            // 
            GV_Schedules.AllowUserToAddRows = false;
            GV_Schedules.AllowUserToDeleteRows = false;
            GV_Schedules.AllowUserToOrderColumns = true;
            GV_Schedules.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GV_Schedules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GV_Schedules.Columns.AddRange(new DataGridViewColumn[] { Select, UniqueId, ProjectName, Version, StartTime, EndTime, LocationGroup, Location });
            GV_Schedules.Location = new Point(11, 207);
            GV_Schedules.Name = "GV_Schedules";
            GV_Schedules.RowHeadersWidth = 51;
            GV_Schedules.Size = new Size(1080, 235);
            GV_Schedules.TabIndex = 13;
            // 
            // Select
            // 
            Select.DataPropertyName = "Selected";
            Select.FalseValue = "false";
            Select.HeaderText = "Select";
            Select.MinimumWidth = 6;
            Select.Name = "Select";
            Select.TrueValue = "true";
            Select.Width = 125;
            // 
            // UniqueId
            // 
            UniqueId.DataPropertyName = "UniqueId";
            UniqueId.HeaderText = "UniqueId";
            UniqueId.MinimumWidth = 6;
            UniqueId.Name = "UniqueId";
            UniqueId.ReadOnly = true;
            UniqueId.Width = 125;
            // 
            // ProjectName
            // 
            ProjectName.DataPropertyName = "ProjectName";
            ProjectName.HeaderText = "ProjectName";
            ProjectName.MinimumWidth = 6;
            ProjectName.Name = "ProjectName";
            ProjectName.ReadOnly = true;
            ProjectName.Width = 125;
            // 
            // Version
            // 
            Version.DataPropertyName = "Version";
            Version.HeaderText = "Version";
            Version.MinimumWidth = 6;
            Version.Name = "Version";
            Version.ReadOnly = true;
            Version.Width = 125;
            // 
            // StartTime
            // 
            StartTime.DataPropertyName = "StartTime";
            StartTime.HeaderText = "StartTime";
            StartTime.MinimumWidth = 6;
            StartTime.Name = "StartTime";
            StartTime.ReadOnly = true;
            StartTime.Width = 125;
            // 
            // EndTime
            // 
            EndTime.DataPropertyName = "EndTime";
            EndTime.HeaderText = "EndTime";
            EndTime.MinimumWidth = 6;
            EndTime.Name = "EndTime";
            EndTime.ReadOnly = true;
            EndTime.Width = 125;
            // 
            // LocationGroup
            // 
            LocationGroup.DataPropertyName = "LocationGroup";
            LocationGroup.HeaderText = "LocationGroup";
            LocationGroup.MinimumWidth = 6;
            LocationGroup.Name = "LocationGroup";
            LocationGroup.ReadOnly = true;
            LocationGroup.Width = 125;
            // 
            // Location
            // 
            Location.DataPropertyName = "Location";
            Location.HeaderText = "Location";
            Location.MinimumWidth = 6;
            Location.Name = "Location";
            Location.ReadOnly = true;
            Location.Width = 125;
            // 
            // UploadAll
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1103, 453);
            Controls.Add(GV_Schedules);
            Controls.Add(NumDuration);
            Controls.Add(DTEndTime);
            Controls.Add(DTStartTime);
            Controls.Add(DTEndDate);
            Controls.Add(DTStartDate);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(BtnUpload);
            Name = "UploadAll";
            Text = "Upload All";
            Load += UploadAll_Load;
            ((System.ComponentModel.ISupportInitialize)NumDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)GV_Schedules).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog LoadFile;
        private Button BtnUpload;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private DateTimePicker DTStartDate;
        private DateTimePicker DTEndDate;
        private DateTimePicker DTStartTime;
        private DateTimePicker DTEndTime;
        private NumericUpDown NumDuration;
        private DataGridView GV_Schedules;
        private DataGridViewCheckBoxColumn Select;
        private DataGridViewTextBoxColumn UniqueId;
        private DataGridViewTextBoxColumn ProjectName;
        private DataGridViewTextBoxColumn Version;
        private DataGridViewTextBoxColumn StartTime;
        private DataGridViewTextBoxColumn EndTime;
        private DataGridViewTextBoxColumn LocationGroup;
        private DataGridViewTextBoxColumn Location;
    }
}
