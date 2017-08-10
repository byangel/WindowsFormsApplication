namespace PackageSellSystemTrading
{
    partial class OptionForm
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
            this.btn_config_save = new System.Windows.Forms.Button();
            this.btn_rollback = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox_limited = new System.Windows.Forms.CheckBox();
            this.input_repeat_rate = new System.Windows.Forms.TextBox();
            this.input_buy_stop_rate = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.input_max_amt_limit = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.input_stop_profit_target = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_today_sell = new System.Windows.Forms.CheckBox();
            this.input_battingAtm = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.input_stop_profit_target2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_config_save
            // 
            this.btn_config_save.Location = new System.Drawing.Point(344, 272);
            this.btn_config_save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_config_save.Name = "btn_config_save";
            this.btn_config_save.Size = new System.Drawing.Size(72, 32);
            this.btn_config_save.TabIndex = 13;
            this.btn_config_save.Text = "설정저장";
            this.btn_config_save.UseVisualStyleBackColor = true;
            this.btn_config_save.Click += new System.EventHandler(this.btn_config_save_Click);
            // 
            // btn_rollback
            // 
            this.btn_rollback.Location = new System.Drawing.Point(270, 272);
            this.btn_rollback.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_rollback.Name = "btn_rollback";
            this.btn_rollback.Size = new System.Drawing.Size(67, 32);
            this.btn_rollback.TabIndex = 14;
            this.btn_rollback.Text = "초기화";
            this.btn_rollback.UseVisualStyleBackColor = true;
            this.btn_rollback.Click += new System.EventHandler(this.btn_rollback_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(225, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 15);
            this.label7.TabIndex = 85;
            this.label7.Text = "%이상 매도.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(190, 209);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 15);
            this.label4.TabIndex = 89;
            this.label4.Text = "% 이하 반복매수";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 15);
            this.label5.TabIndex = 90;
            this.label5.Text = "3.자본금대비";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 15);
            this.label6.TabIndex = 91;
            this.label6.Text = "3.최대운영자금";
            // 
            // checkBox_limited
            // 
            this.checkBox_limited.AutoSize = true;
            this.checkBox_limited.Location = new System.Drawing.Point(17, 12);
            this.checkBox_limited.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox_limited.Name = "checkBox_limited";
            this.checkBox_limited.Size = new System.Drawing.Size(151, 19);
            this.checkBox_limited.TabIndex = 92;
            this.checkBox_limited.Text = "최대운영자금 제한";
            this.checkBox_limited.UseVisualStyleBackColor = true;
            this.checkBox_limited.CheckedChanged += new System.EventHandler(this.checkBox_limited_CheckedChanged);
            // 
            // input_repeat_rate
            // 
            this.input_repeat_rate.Font = new System.Drawing.Font("Gulim", 8F);
            this.input_repeat_rate.Location = new System.Drawing.Point(151, 204);
            this.input_repeat_rate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_repeat_rate.Name = "input_repeat_rate";
            this.input_repeat_rate.Size = new System.Drawing.Size(38, 20);
            this.input_repeat_rate.TabIndex = 94;
            this.input_repeat_rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // input_buy_stop_rate
            // 
            this.input_buy_stop_rate.Font = new System.Drawing.Font("Gulim", 8F);
            this.input_buy_stop_rate.Location = new System.Drawing.Point(106, 169);
            this.input_buy_stop_rate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_buy_stop_rate.Name = "input_buy_stop_rate";
            this.input_buy_stop_rate.Size = new System.Drawing.Size(50, 20);
            this.input_buy_stop_rate.TabIndex = 95;
            this.input_buy_stop_rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(159, 175);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(301, 15);
            this.label8.TabIndex = 96;
            this.label8.Text = "% 이상 신규매수 금지.(매입금액+D2예수금)";
            // 
            // input_max_amt_limit
            // 
            this.input_max_amt_limit.Font = new System.Drawing.Font("Gulim", 8F);
            this.input_max_amt_limit.Location = new System.Drawing.Point(118, 134);
            this.input_max_amt_limit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_max_amt_limit.Name = "input_max_amt_limit";
            this.input_max_amt_limit.Size = new System.Drawing.Size(91, 20);
            this.input_max_amt_limit.TabIndex = 97;
            this.input_max_amt_limit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(214, 139);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(229, 15);
            this.label9.TabIndex = 98;
            this.label9.Text = "원 이하.(예탁자산총액과 비교함)";
            // 
            // input_stop_profit_target
            // 
            this.input_stop_profit_target.Font = new System.Drawing.Font("Gulim", 8F);
            this.input_stop_profit_target.Location = new System.Drawing.Point(181, 70);
            this.input_stop_profit_target.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_stop_profit_target.Name = "input_stop_profit_target";
            this.input_stop_profit_target.Size = new System.Drawing.Size(42, 20);
            this.input_stop_profit_target.TabIndex = 99;
            this.input_stop_profit_target.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(180, 15);
            this.label10.TabIndex = 100;
            this.label10.Text = "1.개별종목 목표 수익률이";
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(423, 272);
            this.close.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(58, 32);
            this.close.TabIndex = 101;
            this.close.Text = "닫기";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 15);
            this.label1.TabIndex = 102;
            this.label1.Text = "4.개별종목별 수익율";
            // 
            // checkBox_today_sell
            // 
            this.checkBox_today_sell.AutoSize = true;
            this.checkBox_today_sell.Location = new System.Drawing.Point(165, 12);
            this.checkBox_today_sell.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox_today_sell.Name = "checkBox_today_sell";
            this.checkBox_today_sell.Size = new System.Drawing.Size(122, 19);
            this.checkBox_today_sell.TabIndex = 103;
            this.checkBox_today_sell.Text = "금일매수/매도";
            this.checkBox_today_sell.UseVisualStyleBackColor = true;
            // 
            // input_battingAtm
            // 
            this.input_battingAtm.Font = new System.Drawing.Font("Gulim", 8F);
            this.input_battingAtm.Location = new System.Drawing.Point(199, 244);
            this.input_battingAtm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_battingAtm.Name = "input_battingAtm";
            this.input_battingAtm.Size = new System.Drawing.Size(105, 20);
            this.input_battingAtm.TabIndex = 108;
            this.input_battingAtm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 246);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(192, 15);
            this.label12.TabIndex = 109;
            this.label12.Text = "6. 배팅금액(default util use)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(311, 246);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 15);
            this.label13.TabIndex = 110;
            this.label13.Text = "원";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 15);
            this.label2.TabIndex = 117;
            this.label2.Text = "2.투자율95%이상 목표수익률";
            // 
            // input_stop_profit_target2
            // 
            this.input_stop_profit_target2.Font = new System.Drawing.Font("Gulim", 8F);
            this.input_stop_profit_target2.Location = new System.Drawing.Point(211, 101);
            this.input_stop_profit_target2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_stop_profit_target2.Name = "input_stop_profit_target2";
            this.input_stop_profit_target2.Size = new System.Drawing.Size(42, 20);
            this.input_stop_profit_target2.TabIndex = 116;
            this.input_stop_profit_target2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(256, 106);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 15);
            this.label11.TabIndex = 115;
            this.label11.Text = "%이상 매도.";
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 313);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.input_stop_profit_target2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.input_battingAtm);
            this.Controls.Add(this.checkBox_today_sell);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.close);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.input_stop_profit_target);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.input_max_amt_limit);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.input_buy_stop_rate);
            this.Controls.Add(this.input_repeat_rate);
            this.Controls.Add(this.checkBox_limited);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btn_rollback);
            this.Controls.Add(this.btn_config_save);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "OptionForm";
            this.Text = "설정관리";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btn_config_save;
        public System.Windows.Forms.Button btn_rollback;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox_limited;
        public System.Windows.Forms.TextBox input_repeat_rate;
        public System.Windows.Forms.TextBox input_buy_stop_rate;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox input_max_amt_limit;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox input_stop_profit_target;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.Button close;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_today_sell;
        public System.Windows.Forms.TextBox input_battingAtm;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox input_stop_profit_target2;
        private System.Windows.Forms.Label label11;
    }
}