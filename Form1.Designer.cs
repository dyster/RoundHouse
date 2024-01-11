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
            this.components = new System.ComponentModel.Container();
            this.startupWorker = new System.ComponentModel.BackgroundWorker();
            this.timerAddBuffer = new System.Windows.Forms.Timer(this.components);
            this.dataListView1 = new BrightIdeasSoftware.DataListView();
            this.timerSlow = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._status = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkBoxWaitForPing = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // startupWorker
            // 
            this.startupWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.startupWorker_DoWork);
            // 
            // timerAddBuffer
            // 
            this.timerAddBuffer.Enabled = true;
            this.timerAddBuffer.Interval = 1000;
            this.timerAddBuffer.Tick += new System.EventHandler(this.timerAddBuffer_Tick);
            // 
            // dataListView1
            // 
            this.dataListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataListView1.CellEditUseWholeCell = false;
            this.dataListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataListView1.DataSource = null;
            this.dataListView1.HideSelection = false;
            this.dataListView1.Location = new System.Drawing.Point(162, 12);
            this.dataListView1.Name = "dataListView1";
            this.dataListView1.ShowGroups = false;
            this.dataListView1.Size = new System.Drawing.Size(626, 413);
            this.dataListView1.TabIndex = 1;
            this.dataListView1.UseCompatibleStateImageBehavior = false;
            this.dataListView1.UseFiltering = true;
            this.dataListView1.View = System.Windows.Forms.View.Details;
            // 
            // timerSlow
            // 
            this.timerSlow.Enabled = true;
            this.timerSlow.Interval = 2000;
            this.timerSlow.Tick += new System.EventHandler(this.timerSlow_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _status
            // 
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(118, 17);
            this._status.Text = "toolStripStatusLabel1";
            // 
            // checkBoxWaitForPing
            // 
            this.checkBoxWaitForPing.AutoSize = true;
            this.checkBoxWaitForPing.Checked = global::RoundHouse.Properties.Settings.Default.WaitForPing;
            this.checkBoxWaitForPing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWaitForPing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::RoundHouse.Properties.Settings.Default, "WaitForPing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxWaitForPing.Location = new System.Drawing.Point(12, 12);
            this.checkBoxWaitForPing.Name = "checkBoxWaitForPing";
            this.checkBoxWaitForPing.Size = new System.Drawing.Size(144, 17);
            this.checkBoxWaitForPing.TabIndex = 3;
            this.checkBoxWaitForPing.Text = "Wait for ping/arp to scan";
            this.checkBoxWaitForPing.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBoxWaitForPing);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataListView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataListView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

