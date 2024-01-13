namespace RoundHouse
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Properties.Settings settings1 = new Properties.Settings();
            startupWorker = new System.ComponentModel.BackgroundWorker();
            timerAddBuffer = new System.Windows.Forms.Timer(components);
            dataListView1 = new BrightIdeasSoftware.DataListView();
            timerSlow = new System.Windows.Forms.Timer(components);
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            _status = new System.Windows.Forms.ToolStripStatusLabel();
            checkBoxWaitForPing = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataListView1).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // startupWorker
            // 
            startupWorker.DoWork += startupWorker_DoWork;
            // 
            // timerAddBuffer
            // 
            timerAddBuffer.Enabled = true;
            timerAddBuffer.Interval = 1000;
            timerAddBuffer.Tick += timerAddBuffer_Tick;
            // 
            // dataListView1
            // 
            dataListView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataListView1.CellEditUseWholeCell = false;
            dataListView1.DataSource = null;
            dataListView1.Location = new System.Drawing.Point(189, 14);
            dataListView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataListView1.Name = "dataListView1";
            dataListView1.ShowGroups = false;
            dataListView1.Size = new System.Drawing.Size(730, 476);
            dataListView1.TabIndex = 1;
            dataListView1.UseCompatibleStateImageBehavior = false;
            dataListView1.UseFiltering = true;
            dataListView1.View = System.Windows.Forms.View.Details;
            // 
            // timerSlow
            // 
            timerSlow.Enabled = true;
            timerSlow.Interval = 5000;
            timerSlow.Tick += timerSlow_Tick;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _status });
            statusStrip1.Location = new System.Drawing.Point(0, 497);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(933, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // _status
            // 
            _status.Name = "_status";
            _status.Size = new System.Drawing.Size(118, 17);
            _status.Text = "toolStripStatusLabel1";
            // 
            // checkBoxWaitForPing
            // 
            checkBoxWaitForPing.AutoSize = true;
            checkBoxWaitForPing.Checked = true;
            checkBoxWaitForPing.CheckState = System.Windows.Forms.CheckState.Checked;
            settings1.SettingsKey = "";
            checkBoxWaitForPing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings1, "WaitForPing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBoxWaitForPing.Location = new System.Drawing.Point(14, 14);
            checkBoxWaitForPing.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxWaitForPing.Name = "checkBoxWaitForPing";
            checkBoxWaitForPing.Size = new System.Drawing.Size(158, 19);
            checkBoxWaitForPing.TabIndex = 3;
            checkBoxWaitForPing.Text = "Wait for ping/arp to scan";
            checkBoxWaitForPing.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(933, 519);
            Controls.Add(checkBoxWaitForPing);
            Controls.Add(statusStrip1);
            Controls.Add(dataListView1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataListView1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.ComponentModel.BackgroundWorker startupWorker;
        private System.Windows.Forms.Timer timerAddBuffer;
        private BrightIdeasSoftware.DataListView dataListView1;
        private System.Windows.Forms.Timer timerSlow;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel _status;
        private System.Windows.Forms.CheckBox checkBoxWaitForPing;
    }
}

