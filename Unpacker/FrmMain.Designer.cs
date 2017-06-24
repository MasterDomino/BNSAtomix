namespace Unpacker
{
    partial class FrmMain
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
            this.button1 = new System.Windows.Forms.Button();
            this.OD = new System.Windows.Forms.OpenFileDialog();
            this.FD = new System.Windows.Forms.FolderBrowserDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tb_addr = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cBTVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBT1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBT2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBT3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oBTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retailV82ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retailV114ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.BetaUnpack = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(79, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "Unpack";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // OD
            // 
            this.OD.Filter = "*.dat|*.dat";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(79, 117);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(61, 41);
            this.button2.TabIndex = 1;
            this.button2.Text = "Repack";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(171, 55);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 40);
            this.button3.TabIndex = 2;
            this.button3.Text = "Extract Binary Data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(170, 117);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(101, 38);
            this.button4.TabIndex = 3;
            this.button4.Text = "Import XML To Binary";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(170, 171);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(101, 35);
            this.button5.TabIndex = 4;
            this.button5.Text = "Extract DB for server";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(281, 54);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox1.Size = new System.Drawing.Size(208, 160);
            this.listBox1.TabIndex = 5;
            // 
            // tb_addr
            // 
            this.tb_addr.Location = new System.Drawing.Point(33, 186);
            this.tb_addr.Name = "tb_addr";
            this.tb_addr.Size = new System.Drawing.Size(115, 20);
            this.tb_addr.TabIndex = 6;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(30, 218);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(118, 40);
            this.button6.TabIndex = 7;
            this.button6.Text = "Export Table Layout from exe";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(502, 55);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(87, 36);
            this.button7.TabIndex = 8;
            this.button7.Text = "Export sample Entry";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(600, 54);
            this.listBox2.Name = "listBox2";
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox2.Size = new System.Drawing.Size(203, 160);
            this.listBox2.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBTVersionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(820, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cBTVersionToolStripMenuItem
            // 
            this.cBTVersionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBT1ToolStripMenuItem,
            this.cBT2ToolStripMenuItem,
            this.cBT3ToolStripMenuItem,
            this.oBTToolStripMenuItem,
            this.retailV82ToolStripMenuItem,
            this.retailV114ToolStripMenuItem});
            this.cBTVersionToolStripMenuItem.Name = "cBTVersionToolStripMenuItem";
            this.cBTVersionToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.cBTVersionToolStripMenuItem.Text = "CBT Version";
            // 
            // cBT1ToolStripMenuItem
            // 
            this.cBT1ToolStripMenuItem.CheckOnClick = true;
            this.cBT1ToolStripMenuItem.Name = "cBT1ToolStripMenuItem";
            this.cBT1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cBT1ToolStripMenuItem.Text = "CBT1";
            this.cBT1ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.cBT1ToolStripMenuItem_CheckedChanged);
            // 
            // cBT2ToolStripMenuItem
            // 
            this.cBT2ToolStripMenuItem.CheckOnClick = true;
            this.cBT2ToolStripMenuItem.Name = "cBT2ToolStripMenuItem";
            this.cBT2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cBT2ToolStripMenuItem.Text = "CBT2";
            this.cBT2ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.cBT2ToolStripMenuItem_CheckedChanged);
            this.cBT2ToolStripMenuItem.Click += new System.EventHandler(this.cBT2ToolStripMenuItem_Click);
            // 
            // cBT3ToolStripMenuItem
            // 
            this.cBT3ToolStripMenuItem.CheckOnClick = true;
            this.cBT3ToolStripMenuItem.Name = "cBT3ToolStripMenuItem";
            this.cBT3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cBT3ToolStripMenuItem.Text = "CBT3";
            this.cBT3ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.cBT3ToolStripMenuItem_CheckedChanged);
            this.cBT3ToolStripMenuItem.Click += new System.EventHandler(this.cBT3ToolStripMenuItem_Click);
            // 
            // oBTToolStripMenuItem
            // 
            this.oBTToolStripMenuItem.CheckOnClick = true;
            this.oBTToolStripMenuItem.Name = "oBTToolStripMenuItem";
            this.oBTToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.oBTToolStripMenuItem.Text = "OBT";
            this.oBTToolStripMenuItem.CheckedChanged += new System.EventHandler(this.oBTToolStripMenuItem_CheckedChanged);
            this.oBTToolStripMenuItem.Click += new System.EventHandler(this.oBTToolStripMenuItem_Click);
            // 
            // retailV82ToolStripMenuItem
            // 
            this.retailV82ToolStripMenuItem.CheckOnClick = true;
            this.retailV82ToolStripMenuItem.Name = "retailV82ToolStripMenuItem";
            this.retailV82ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.retailV82ToolStripMenuItem.Text = "RetailV82";
            this.retailV82ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.retailV82ToolStripMenuItem_CheckedChanged);
            // 
            // retailV114ToolStripMenuItem
            // 
            this.retailV114ToolStripMenuItem.Checked = true;
            this.retailV114ToolStripMenuItem.CheckOnClick = true;
            this.retailV114ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.retailV114ToolStripMenuItem.Name = "retailV114ToolStripMenuItem";
            this.retailV114ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.retailV114ToolStripMenuItem.Text = "retailV114";
            this.retailV114ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.retailV114ToolStripMenuItem_CheckedChanged);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(173, 218);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(97, 53);
            this.button8.TabIndex = 11;
            this.button8.Text = "Translate Text from old Version";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(282, 218);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(109, 52);
            this.button9.TabIndex = 12;
            this.button9.Text = "Make Skill template";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // BetaUnpack
            // 
            this.BetaUnpack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BetaUnpack.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BetaUnpack.Location = new System.Drawing.Point(0, 66);
            this.BetaUnpack.Name = "BetaUnpack";
            this.BetaUnpack.Size = new System.Drawing.Size(73, 79);
            this.BetaUnpack.TabIndex = 13;
            this.BetaUnpack.Text = "Beta Unpack (only good with xml)";
            this.BetaUnpack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BetaUnpack.UseVisualStyleBackColor = true;
            this.BetaUnpack.CheckedChanged += new System.EventHandler(this.BetaUnpack_CheckedChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 284);
            this.Controls.Add(this.BetaUnpack);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.tb_addr);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog OD;
        private System.Windows.Forms.FolderBrowserDialog FD;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox tb_addr;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cBTVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cBT1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cBT2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cBT3ToolStripMenuItem;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ToolStripMenuItem oBTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retailV82ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retailV114ToolStripMenuItem;
        private System.Windows.Forms.CheckBox BetaUnpack;
    }
}

