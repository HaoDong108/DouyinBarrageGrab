namespace BarrageGrab
{
    partial class FormView
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
            this.rich_output = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tab_filters = new System.Windows.Forms.TabControl();
            this.tabPage_Console = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPage_Ws = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.list_roomCaches = new System.Windows.Forms.ListBox();
            this.cbx_enableProxy = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbx_barrageLog = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_updateUpProxy = new System.Windows.Forms.Button();
            this.txb_upstreamProxy = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txb_wsaddr = new System.Windows.Forms.TextBox();
            this.tabPage_Log = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.tab_filters.SuspendLayout();
            this.tabPage_Console.SuspendLayout();
            this.tabPage_Ws.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // rich_output
            // 
            this.rich_output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rich_output.BackColor = System.Drawing.SystemColors.Desktop;
            this.rich_output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rich_output.Location = new System.Drawing.Point(90, 0);
            this.rich_output.Margin = new System.Windows.Forms.Padding(2);
            this.rich_output.Name = "rich_output";
            this.rich_output.ReadOnly = true;
            this.rich_output.Size = new System.Drawing.Size(592, 271);
            this.rich_output.TabIndex = 0;
            this.rich_output.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(629, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "🚀 代理抓取中";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tab_filters);
            this.panel1.Controls.Add(this.rich_output);
            this.panel1.Location = new System.Drawing.Point(0, 202);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 5, 10);
            this.panel1.Size = new System.Drawing.Size(687, 281);
            this.panel1.TabIndex = 5;
            // 
            // tab_filters
            // 
            this.tab_filters.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tab_filters.Controls.Add(this.tabPage_Console);
            this.tab_filters.Controls.Add(this.tabPage_Ws);
            this.tab_filters.Controls.Add(this.tabPage_Log);
            this.tab_filters.Dock = System.Windows.Forms.DockStyle.Left;
            this.tab_filters.Location = new System.Drawing.Point(0, 0);
            this.tab_filters.Multiline = true;
            this.tab_filters.Name = "tab_filters";
            this.tab_filters.SelectedIndex = 0;
            this.tab_filters.Size = new System.Drawing.Size(89, 271);
            this.tab_filters.TabIndex = 10;
            // 
            // tabPage_Console
            // 
            this.tabPage_Console.Controls.Add(this.flowLayoutPanel1);
            this.tabPage_Console.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage_Console.Location = new System.Drawing.Point(22, 4);
            this.tabPage_Console.Name = "tabPage_Console";
            this.tabPage_Console.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Console.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabPage_Console.Size = new System.Drawing.Size(63, 263);
            this.tabPage_Console.TabIndex = 0;
            this.tabPage_Console.Text = "控制台";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(57, 257);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // tabPage_Ws
            // 
            this.tabPage_Ws.Controls.Add(this.flowLayoutPanel2);
            this.tabPage_Ws.Location = new System.Drawing.Point(22, 4);
            this.tabPage_Ws.Name = "tabPage_Ws";
            this.tabPage_Ws.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Ws.Size = new System.Drawing.Size(63, 263);
            this.tabPage_Ws.TabIndex = 1;
            this.tabPage_Ws.Text = "推送 ";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(57, 257);
            this.flowLayoutPanel2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(248, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "配置";
            // 
            // list_roomCaches
            // 
            this.list_roomCaches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.list_roomCaches.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.list_roomCaches.FormattingEnabled = true;
            this.list_roomCaches.ItemHeight = 12;
            this.list_roomCaches.Location = new System.Drawing.Point(14, 33);
            this.list_roomCaches.Margin = new System.Windows.Forms.Padding(2);
            this.list_roomCaches.Name = "list_roomCaches";
            this.list_roomCaches.Size = new System.Drawing.Size(232, 158);
            this.list_roomCaches.TabIndex = 7;
            this.list_roomCaches.DoubleClick += new System.EventHandler(this.list_roomCaches_DoubleClick);
            // 
            // cbx_enableProxy
            // 
            this.cbx_enableProxy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_enableProxy.AutoSize = true;
            this.cbx_enableProxy.Location = new System.Drawing.Point(558, 6);
            this.cbx_enableProxy.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_enableProxy.Name = "cbx_enableProxy";
            this.cbx_enableProxy.Size = new System.Drawing.Size(120, 16);
            this.cbx_enableProxy.TabIndex = 8;
            this.cbx_enableProxy.Text = "启用系统代理抓取";
            this.cbx_enableProxy.UseVisualStyleBackColor = true;
            this.cbx_enableProxy.CheckedChanged += new System.EventHandler(this.cbx_enableProxy_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "房间缓存列表(0)";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbx_barrageLog);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.list_roomCaches);
            this.panel2.Controls.Add(this.cbx_enableProxy);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.panel2.Size = new System.Drawing.Size(687, 198);
            this.panel2.TabIndex = 10;
            // 
            // cbx_barrageLog
            // 
            this.cbx_barrageLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_barrageLog.AutoSize = true;
            this.cbx_barrageLog.Location = new System.Drawing.Point(477, 6);
            this.cbx_barrageLog.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_barrageLog.Name = "cbx_barrageLog";
            this.cbx_barrageLog.Size = new System.Drawing.Size(72, 16);
            this.cbx_barrageLog.TabIndex = 14;
            this.cbx_barrageLog.Text = "弹幕日志";
            this.cbx_barrageLog.UseVisualStyleBackColor = true;
            this.cbx_barrageLog.CheckedChanged += new System.EventHandler(this.cbx_barrageLog_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.txb_wsaddr);
            this.panel3.Controls.Add(this.btn_updateUpProxy);
            this.panel3.Controls.Add(this.txb_upstreamProxy);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(250, 33);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(429, 159);
            this.panel3.TabIndex = 13;
            // 
            // btn_updateUpProxy
            // 
            this.btn_updateUpProxy.Location = new System.Drawing.Point(254, 48);
            this.btn_updateUpProxy.Name = "btn_updateUpProxy";
            this.btn_updateUpProxy.Size = new System.Drawing.Size(64, 23);
            this.btn_updateUpProxy.TabIndex = 15;
            this.btn_updateUpProxy.Text = "确定";
            this.btn_updateUpProxy.UseVisualStyleBackColor = true;
            this.btn_updateUpProxy.Click += new System.EventHandler(this.btn_updateUpProxy_Click);
            // 
            // txb_upstreamProxy
            // 
            this.txb_upstreamProxy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txb_upstreamProxy.Location = new System.Drawing.Point(85, 48);
            this.txb_upstreamProxy.Margin = new System.Windows.Forms.Padding(2);
            this.txb_upstreamProxy.Name = "txb_upstreamProxy";
            this.txb_upstreamProxy.Size = new System.Drawing.Size(164, 21);
            this.txb_upstreamProxy.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label5.Location = new System.Drawing.Point(18, 52);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "上游代理：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 17);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "WS服务端口：";
            // 
            // txb_wsaddr
            // 
            this.txb_wsaddr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txb_wsaddr.Location = new System.Drawing.Point(85, 14);
            this.txb_wsaddr.Margin = new System.Windows.Forms.Padding(2);
            this.txb_wsaddr.Name = "txb_wsaddr";
            this.txb_wsaddr.ReadOnly = true;
            this.txb_wsaddr.Size = new System.Drawing.Size(164, 21);
            this.txb_wsaddr.TabIndex = 16;
            // 
            // tabPage_Log
            // 
            this.tabPage_Log.Controls.Add(this.flowLayoutPanel3);
            this.tabPage_Log.Location = new System.Drawing.Point(22, 4);
            this.tabPage_Log.Name = "tabPage_Log";
            this.tabPage_Log.Size = new System.Drawing.Size(63, 263);
            this.tabPage_Log.TabIndex = 2;
            this.tabPage_Log.Text = "日志 ";
            this.tabPage_Log.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(63, 263);
            this.flowLayoutPanel3.TabIndex = 7;
            // 
            // FormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 483);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FormView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "抖音弹幕抓取推送";
            this.Load += new System.EventHandler(this.FormView_Load);
            this.panel1.ResumeLayout(false);
            this.tab_filters.ResumeLayout(false);
            this.tabPage_Console.ResumeLayout(false);
            this.tabPage_Ws.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPage_Log.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rich_output;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox list_roomCaches;
        private System.Windows.Forms.CheckBox cbx_enableProxy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txb_upstreamProxy;
        private System.Windows.Forms.CheckBox cbx_barrageLog;
        private System.Windows.Forms.TabControl tab_filters;
        private System.Windows.Forms.TabPage tabPage_Console;
        private System.Windows.Forms.TabPage tabPage_Ws;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btn_updateUpProxy;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox txb_wsaddr;
        private System.Windows.Forms.TabPage tabPage_Log;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
    }
}