namespace BruteForce
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
            this.label1 = new System.Windows.Forms.Label();
            this.tb_Input = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Threads = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbCur = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbMax = new System.Windows.Forms.Label();
            this.lb_Perc = new System.Windows.Forms.Label();
            this.lb_ = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbTime = new System.Windows.Forms.Label();
            this.lb_Remain = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_Truc = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.OD = new System.Windows.Forms.OpenFileDialog();
            this.SD = new System.Windows.Forms.SaveFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Client\'s KeyData:";
            // 
            // tb_Input
            // 
            this.tb_Input.Location = new System.Drawing.Point(12, 36);
            this.tb_Input.Name = "tb_Input";
            this.tb_Input.Size = new System.Drawing.Size(304, 21);
            this.tb_Input.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Threads:";
            // 
            // tb_Threads
            // 
            this.tb_Threads.Location = new System.Drawing.Point(74, 66);
            this.tb_Threads.Name = "tb_Threads";
            this.tb_Threads.Size = new System.Drawing.Size(59, 21);
            this.tb_Threads.TabIndex = 3;
            this.tb_Threads.Text = "4";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(173, 139);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 31);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start New";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbCur
            // 
            this.lbCur.AutoSize = true;
            this.lbCur.Location = new System.Drawing.Point(10, 166);
            this.lbCur.Name = "lbCur";
            this.lbCur.Size = new System.Drawing.Size(11, 12);
            this.lbCur.TabIndex = 5;
            this.lbCur.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "of";
            // 
            // lbMax
            // 
            this.lbMax.AutoSize = true;
            this.lbMax.Location = new System.Drawing.Point(10, 190);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(11, 12);
            this.lbMax.TabIndex = 7;
            this.lbMax.Text = "0";
            // 
            // lb_Perc
            // 
            this.lb_Perc.AutoSize = true;
            this.lb_Perc.Location = new System.Drawing.Point(10, 214);
            this.lb_Perc.Name = "lb_Perc";
            this.lb_Perc.Size = new System.Drawing.Size(17, 12);
            this.lb_Perc.TabIndex = 8;
            this.lb_Perc.Text = "0%";
            // 
            // lb_
            // 
            this.lb_.AutoSize = true;
            this.lb_.Location = new System.Drawing.Point(12, 234);
            this.lb_.Name = "lb_";
            this.lb_.Size = new System.Drawing.Size(83, 12);
            this.lb_.TabIndex = 9;
            this.lb_.Text = "Elapsed Time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 254);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "Remaining:";
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Location = new System.Drawing.Point(99, 234);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(0, 12);
            this.lbTime.TabIndex = 11;
            // 
            // lb_Remain
            // 
            this.lb_Remain.AutoSize = true;
            this.lb_Remain.Location = new System.Drawing.Point(99, 254);
            this.lb_Remain.Name = "lb_Remain";
            this.lb_Remain.Size = new System.Drawing.Size(0, 12);
            this.lb_Remain.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "Truncs:";
            // 
            // tb_Truc
            // 
            this.tb_Truc.Location = new System.Drawing.Point(74, 90);
            this.tb_Truc.Name = "tb_Truc";
            this.tb_Truc.Size = new System.Drawing.Size(78, 21);
            this.tb_Truc.TabIndex = 14;
            this.tb_Truc.Text = "1000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "Result:";
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(74, 112);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(242, 21);
            this.tbResult.TabIndex = 16;
            // 
            // OD
            // 
            this.OD.Filter = "*.dat|*.dat";
            // 
            // SD
            // 
            this.SD.Filter = "*.dat|*.dat";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 138);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 29);
            this.button1.TabIndex = 17;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 275);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tb_Truc);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lb_Remain);
            this.Controls.Add(this.lbTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lb_);
            this.Controls.Add(this.lb_Perc);
            this.Controls.Add(this.lbMax);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbCur);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.tb_Threads);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_Input);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_Input;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Threads;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbCur;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbMax;
        private System.Windows.Forms.Label lb_Perc;
        private System.Windows.Forms.Label lb_;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label lb_Remain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_Truc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.OpenFileDialog OD;
        private System.Windows.Forms.SaveFileDialog SD;
        private System.Windows.Forms.Button button1;
    }
}

