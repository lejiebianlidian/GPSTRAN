namespace GPSTran
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StartBtn = new System.Windows.Forms.Button();
            this.PortLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cacheStatusValue = new System.Windows.Forms.Label();
            this.cacheStatus = new System.Windows.Forms.Label();
            this.GPSStatisticsLabel = new System.Windows.Forms.Label();
            this.DebugCheck = new System.Windows.Forms.CheckBox();
            this.GPSLogCheck = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.OperateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GPSLogItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DebugItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Config_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.Config_import = new System.Windows.Forms.ToolStripMenuItem();
            this.Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.SafetyExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InstructionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.protolBaseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.TransferType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Protocol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Destination = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EntityID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Statistics = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "GPSTran";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // ShowToolStripMenuItem
            // 
            this.ShowToolStripMenuItem.Name = "ShowToolStripMenuItem";
            this.ShowToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.ShowToolStripMenuItem.Text = "显示";
            this.ShowToolStripMenuItem.Click += new System.EventHandler(this.ShowToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.ExitToolStripMenuItem.Text = "退出";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StartBtn);
            this.groupBox1.Controls.Add(this.PortLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 171);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "消息";
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(63, 104);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(75, 23);
            this.StartBtn.TabIndex = 4;
            this.StartBtn.Text = "开始";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(33, 40);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(107, 12);
            this.PortLabel.TabIndex = 4;
            this.PortLabel.Text = "端口：       8000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cacheStatusValue);
            this.groupBox2.Controls.Add(this.cacheStatus);
            this.groupBox2.Controls.Add(this.GPSStatisticsLabel);
            this.groupBox2.Controls.Add(this.DebugCheck);
            this.groupBox2.Controls.Add(this.GPSLogCheck);
            this.groupBox2.Location = new System.Drawing.Point(12, 248);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 242);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "控制";
            // 
            // cacheStatusValue
            // 
            this.cacheStatusValue.AutoSize = true;
            this.cacheStatusValue.Location = new System.Drawing.Point(123, 211);
            this.cacheStatusValue.Name = "cacheStatusValue";
            this.cacheStatusValue.Size = new System.Drawing.Size(29, 12);
            this.cacheStatusValue.TabIndex = 5;
            this.cacheStatusValue.Text = "正常";
            // 
            // cacheStatus
            // 
            this.cacheStatus.AutoSize = true;
            this.cacheStatus.Location = new System.Drawing.Point(32, 211);
            this.cacheStatus.Name = "cacheStatus";
            this.cacheStatus.Size = new System.Drawing.Size(65, 12);
            this.cacheStatus.TabIndex = 6;
            this.cacheStatus.Text = "缓存状态：";
            // 
            // GPSStatisticsLabel
            // 
            this.GPSStatisticsLabel.AutoSize = true;
            this.GPSStatisticsLabel.Location = new System.Drawing.Point(33, 183);
            this.GPSStatisticsLabel.Name = "GPSStatisticsLabel";
            this.GPSStatisticsLabel.Size = new System.Drawing.Size(119, 12);
            this.GPSStatisticsLabel.TabIndex = 4;
            this.GPSStatisticsLabel.Text = "GPS接收计数：  1234";
            // 
            // DebugCheck
            // 
            this.DebugCheck.AutoSize = true;
            this.DebugCheck.Location = new System.Drawing.Point(35, 117);
            this.DebugCheck.Name = "DebugCheck";
            this.DebugCheck.Size = new System.Drawing.Size(72, 16);
            this.DebugCheck.TabIndex = 5;
            this.DebugCheck.Text = "调试日志";
            this.DebugCheck.UseVisualStyleBackColor = true;
            this.DebugCheck.CheckedChanged += new System.EventHandler(this.DebugCheck_CheckedChanged);
            // 
            // GPSLogCheck
            // 
            this.GPSLogCheck.AutoSize = true;
            this.GPSLogCheck.Location = new System.Drawing.Point(35, 53);
            this.GPSLogCheck.Name = "GPSLogCheck";
            this.GPSLogCheck.Size = new System.Drawing.Size(90, 16);
            this.GPSLogCheck.TabIndex = 4;
            this.GPSLogCheck.Text = "GPS接收日志";
            this.GPSLogCheck.UseVisualStyleBackColor = true;
            this.GPSLogCheck.CheckedChanged += new System.EventHandler(this.GPSLogCheck_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OperateMenuItem,
            this.ConfigMenuItem,
            this.HelpMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1148, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // OperateMenuItem
            // 
            this.OperateMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartItem,
            this.GPSLogItem,
            this.DebugItem,
            this.Config_Export,
            this.Config_import,
            this.Exit,
            this.SafetyExit});
            this.OperateMenuItem.Name = "OperateMenuItem";
            this.OperateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OperateMenuItem.Size = new System.Drawing.Size(44, 21);
            this.OperateMenuItem.Text = "操作";
            // 
            // StartItem
            // 
            this.StartItem.Name = "StartItem";
            this.StartItem.Size = new System.Drawing.Size(171, 22);
            this.StartItem.Text = "开始";
            this.StartItem.Click += new System.EventHandler(this.StartItem_Click);
            // 
            // GPSLogItem
            // 
            this.GPSLogItem.Name = "GPSLogItem";
            this.GPSLogItem.Size = new System.Drawing.Size(171, 22);
            this.GPSLogItem.Text = "开启GPS接收日志";
            this.GPSLogItem.Click += new System.EventHandler(this.GPSLogItem_Click);
            // 
            // DebugItem
            // 
            this.DebugItem.Name = "DebugItem";
            this.DebugItem.Size = new System.Drawing.Size(171, 22);
            this.DebugItem.Text = "开启DEBUG日志";
            this.DebugItem.Click += new System.EventHandler(this.DebugItem_Click);
            // 
            // Config_Export
            // 
            this.Config_Export.Name = "Config_Export";
            this.Config_Export.Size = new System.Drawing.Size(171, 22);
            this.Config_Export.Text = "配置导出";
            this.Config_Export.Click += new System.EventHandler(this.Config_Export_Click);
            // 
            // Config_import
            // 
            this.Config_import.Name = "Config_import";
            this.Config_import.Size = new System.Drawing.Size(171, 22);
            this.Config_import.Text = "配置导入";
            this.Config_import.Click += new System.EventHandler(this.Config_import_Click);
            // 
            // Exit
            // 
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(171, 22);
            this.Exit.Text = "退出";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // SafetyExit
            // 
            this.SafetyExit.Name = "SafetyExit";
            this.SafetyExit.Size = new System.Drawing.Size(171, 22);
            this.SafetyExit.Text = "安全退出";
            this.SafetyExit.Click += new System.EventHandler(this.SafetyExit_Click);
            // 
            // ConfigMenuItem
            // 
            this.ConfigMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConfigItem});
            this.ConfigMenuItem.Name = "ConfigMenuItem";
            this.ConfigMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.ConfigMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ConfigMenuItem.Text = "配置";
            // 
            // ConfigItem
            // 
            this.ConfigItem.Name = "ConfigItem";
            this.ConfigItem.Size = new System.Drawing.Size(124, 22);
            this.ConfigItem.Text = "配置选项";
            this.ConfigItem.Click += new System.EventHandler(this.ConfigItem_Click);
            // 
            // HelpMenuItem
            // 
            this.HelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InstructionMenuItem,
            this.protolBaseMenuItem,
            this.AboutMenuItem});
            this.HelpMenuItem.Name = "HelpMenuItem";
            this.HelpMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.HelpMenuItem.Size = new System.Drawing.Size(44, 21);
            this.HelpMenuItem.Text = "帮助";
            // 
            // InstructionMenuItem
            // 
            this.InstructionMenuItem.Name = "InstructionMenuItem";
            this.InstructionMenuItem.Size = new System.Drawing.Size(136, 22);
            this.InstructionMenuItem.Text = "操作手册";
            this.InstructionMenuItem.Click += new System.EventHandler(this.InstructionMenuItem_Click);
            // 
            // protolBaseMenuItem
            // 
            this.protolBaseMenuItem.Name = "protolBaseMenuItem";
            this.protolBaseMenuItem.Size = new System.Drawing.Size(136, 22);
            this.protolBaseMenuItem.Text = "协议库说明";
            this.protolBaseMenuItem.Click += new System.EventHandler(this.protolBaseMenuItem_Click);
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.Size = new System.Drawing.Size(136, 22);
            this.AboutMenuItem.Text = "关于";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TransferType,
            this.Protocol,
            this.Destination,
            this.Status,
            this.EntityID,
            this.Statistics});
            this.dataGridView1.Location = new System.Drawing.Point(242, 33);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(906, 457);
            this.dataGridView1.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TransferType
            // 
            this.TransferType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TransferType.HeaderText = "转发方式";
            this.TransferType.Name = "TransferType";
            this.TransferType.ReadOnly = true;
            // 
            // Protocol
            // 
            this.Protocol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Protocol.HeaderText = "协议";
            this.Protocol.Name = "Protocol";
            this.Protocol.ReadOnly = true;
            this.Protocol.Visible = false;
            // 
            // Destination
            // 
            this.Destination.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Destination.HeaderText = "IP端口/表名称";
            this.Destination.Name = "Destination";
            this.Destination.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.HeaderText = "状态";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // EntityID
            // 
            this.EntityID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.EntityID.HeaderText = "单位过滤";
            this.EntityID.Name = "EntityID";
            this.EntityID.ReadOnly = true;
            // 
            // Statistics
            // 
            this.Statistics.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Statistics.HeaderText = "计数";
            this.Statistics.Name = "Statistics";
            this.Statistics.ReadOnly = true;
            this.Statistics.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1148, 500);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GPSTran";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ShowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem OperateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Exit;
        private System.Windows.Forms.ToolStripMenuItem SafetyExit;
        private System.Windows.Forms.ToolStripMenuItem DebugItem;
        private System.Windows.Forms.ToolStripMenuItem GPSLogItem;
        private System.Windows.Forms.ToolStripMenuItem ConfigMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConfigItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InstructionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem protolBaseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.ToolStripMenuItem StartItem;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Label GPSStatisticsLabel;
        private System.Windows.Forms.CheckBox DebugCheck;
        private System.Windows.Forms.CheckBox GPSLogCheck;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label cacheStatus;
        private System.Windows.Forms.Label cacheStatusValue;
        private System.Windows.Forms.ToolStripMenuItem Config_Export;
        private System.Windows.Forms.ToolStripMenuItem Config_import;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Protocol;
        private System.Windows.Forms.DataGridViewTextBoxColumn Destination;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntityID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Statistics;
    }
}

