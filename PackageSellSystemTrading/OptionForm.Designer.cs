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
            this.checkBox_limited = new System.Windows.Forms.CheckBox();
            this.close = new System.Windows.Forms.Button();
            this.checkBox_today_sell = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.input_max_amt_limit = new System.Windows.Forms.TextBox();
            this.input_buy_stop_rate = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.input_battingAtm = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.input_repeat_rate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.input_stop_profit_target = new System.Windows.Forms.TextBox();
            this.input_stop_profit_target2 = new System.Windows.Forms.TextBox();
            this.checkBox_stopLoss = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.input_stopLoss = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBox_exclStopLossAt = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btn_config_save
            // 
            this.btn_config_save.Location = new System.Drawing.Point(301, 218);
            this.btn_config_save.Name = "btn_config_save";
            this.btn_config_save.Size = new System.Drawing.Size(63, 26);
            this.btn_config_save.TabIndex = 13;
            this.btn_config_save.Text = "설정저장";
            this.btn_config_save.UseVisualStyleBackColor = true;
            this.btn_config_save.Click += new System.EventHandler(this.btn_config_save_Click);
            // 
            // btn_rollback
            // 
            this.btn_rollback.Location = new System.Drawing.Point(236, 218);
            this.btn_rollback.Name = "btn_rollback";
            this.btn_rollback.Size = new System.Drawing.Size(59, 26);
            this.btn_rollback.TabIndex = 14;
            this.btn_rollback.Text = "초기화";
            this.btn_rollback.UseVisualStyleBackColor = true;
            this.btn_rollback.Click += new System.EventHandler(this.btn_rollback_Click);
            // 
            // checkBox_limited
            // 
            this.checkBox_limited.AutoSize = true;
            this.checkBox_limited.Location = new System.Drawing.Point(15, 10);
            this.checkBox_limited.Name = "checkBox_limited";
            this.checkBox_limited.Size = new System.Drawing.Size(124, 16);
            this.checkBox_limited.TabIndex = 92;
            this.checkBox_limited.Text = "최대운영자금 제한";
            this.checkBox_limited.UseVisualStyleBackColor = true;
            this.checkBox_limited.CheckedChanged += new System.EventHandler(this.checkBox_limited_CheckedChanged);
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(370, 218);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(51, 26);
            this.close.TabIndex = 101;
            this.close.Text = "닫기";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // checkBox_today_sell
            // 
            this.checkBox_today_sell.AutoSize = true;
            this.checkBox_today_sell.Location = new System.Drawing.Point(144, 10);
            this.checkBox_today_sell.Name = "checkBox_today_sell";
            this.checkBox_today_sell.Size = new System.Drawing.Size(102, 16);
            this.checkBox_today_sell.TabIndex = 103;
            this.checkBox_today_sell.Text = "금일매수/매도";
            this.checkBox_today_sell.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(187, 111);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(183, 12);
            this.label9.TabIndex = 98;
            this.label9.Text = "원 이하.(예탁자산총액과 비교함)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 12);
            this.label5.TabIndex = 90;
            this.label5.Text = "3.자본금대비";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 12);
            this.label6.TabIndex = 91;
            this.label6.Text = "3.최대운영자금";
            // 
            // input_max_amt_limit
            // 
            this.input_max_amt_limit.Font = new System.Drawing.Font("굴림", 8F);
            this.input_max_amt_limit.Location = new System.Drawing.Point(103, 107);
            this.input_max_amt_limit.Name = "input_max_amt_limit";
            this.input_max_amt_limit.Size = new System.Drawing.Size(80, 20);
            this.input_max_amt_limit.TabIndex = 97;
            this.input_max_amt_limit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // input_buy_stop_rate
            // 
            this.input_buy_stop_rate.Font = new System.Drawing.Font("굴림", 8F);
            this.input_buy_stop_rate.Location = new System.Drawing.Point(96, 136);
            this.input_buy_stop_rate.Name = "input_buy_stop_rate";
            this.input_buy_stop_rate.Size = new System.Drawing.Size(44, 20);
            this.input_buy_stop_rate.TabIndex = 95;
            this.input_buy_stop_rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(139, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(241, 12);
            this.label8.TabIndex = 96;
            this.label8.Text = "% 이상 신규매수 금지.(매입금액+D2예수금)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(272, 197);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 110;
            this.label13.Text = "원";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 197);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(159, 12);
            this.label12.TabIndex = 109;
            this.label12.Text = "6. 배팅금액(default util use)";
            // 
            // input_battingAtm
            // 
            this.input_battingAtm.Font = new System.Drawing.Font("굴림", 8F);
            this.input_battingAtm.Location = new System.Drawing.Point(174, 195);
            this.input_battingAtm.Name = "input_battingAtm";
            this.input_battingAtm.Size = new System.Drawing.Size(92, 20);
            this.input_battingAtm.TabIndex = 108;
            this.input_battingAtm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(182, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 89;
            this.label4.Text = "% 이하 반복매수";
            // 
            // input_repeat_rate
            // 
            this.input_repeat_rate.Font = new System.Drawing.Font("굴림", 8F);
            this.input_repeat_rate.Location = new System.Drawing.Point(148, 163);
            this.input_repeat_rate.Name = "input_repeat_rate";
            this.input_repeat_rate.Size = new System.Drawing.Size(34, 20);
            this.input_repeat_rate.TabIndex = 94;
            this.input_repeat_rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 12);
            this.label1.TabIndex = 102;
            this.label1.Text = "4.개별종목별 수익율";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(396, 60);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 12);
            this.label11.TabIndex = 115;
            this.label11.Text = "%이상 매도.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(260, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 12);
            this.label2.TabIndex = 117;
            this.label2.Text = "투자율95%이상";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(186, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 85;
            this.label7.Text = "%이상 매도.";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 12);
            this.label10.TabIndex = 100;
            this.label10.Text = "1.개별종목 목표수익률";
            // 
            // input_stop_profit_target
            // 
            this.input_stop_profit_target.Font = new System.Drawing.Font("굴림", 8F);
            this.input_stop_profit_target.Location = new System.Drawing.Point(144, 56);
            this.input_stop_profit_target.Name = "input_stop_profit_target";
            this.input_stop_profit_target.Size = new System.Drawing.Size(37, 20);
            this.input_stop_profit_target.TabIndex = 99;
            this.input_stop_profit_target.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // input_stop_profit_target2
            // 
            this.input_stop_profit_target2.Font = new System.Drawing.Font("굴림", 8F);
            this.input_stop_profit_target2.Location = new System.Drawing.Point(353, 56);
            this.input_stop_profit_target2.Name = "input_stop_profit_target2";
            this.input_stop_profit_target2.Size = new System.Drawing.Size(37, 20);
            this.input_stop_profit_target2.TabIndex = 116;
            this.input_stop_profit_target2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_stopLoss
            // 
            this.checkBox_stopLoss.AutoSize = true;
            this.checkBox_stopLoss.Location = new System.Drawing.Point(252, 10);
            this.checkBox_stopLoss.Name = "checkBox_stopLoss";
            this.checkBox_stopLoss.Size = new System.Drawing.Size(96, 16);
            this.checkBox_stopLoss.TabIndex = 118;
            this.checkBox_stopLoss.Text = "손절기능사용";
            this.checkBox_stopLoss.UseVisualStyleBackColor = true;
            this.checkBox_stopLoss.CheckedChanged += new System.EventHandler(this.checkBox_stopLoss_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 12);
            this.label3.TabIndex = 119;
            this.label3.Text = "2.손절";
            // 
            // input_stopLoss
            // 
            this.input_stopLoss.Font = new System.Drawing.Font("굴림", 8F);
            this.input_stopLoss.Location = new System.Drawing.Point(56, 81);
            this.input_stopLoss.Name = "input_stopLoss";
            this.input_stopLoss.Size = new System.Drawing.Size(37, 20);
            this.input_stopLoss.TabIndex = 120;
            this.input_stopLoss.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(98, 86);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 12);
            this.label14.TabIndex = 121;
            this.label14.Text = "%이하 매도.";
            // 
            // checkBox_exclStopLossAt
            // 
            this.checkBox_exclStopLossAt.AutoSize = true;
            this.checkBox_exclStopLossAt.Location = new System.Drawing.Point(353, 10);
            this.checkBox_exclStopLossAt.Name = "checkBox_exclStopLossAt";
            this.checkBox_exclStopLossAt.Size = new System.Drawing.Size(120, 16);
            this.checkBox_exclStopLossAt.TabIndex = 122;
            this.checkBox_exclStopLossAt.Text = "매수금지종목손절";
            this.checkBox_exclStopLossAt.UseVisualStyleBackColor = true;
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 250);
            this.Controls.Add(this.checkBox_exclStopLossAt);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.input_stopLoss);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox_stopLoss);
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
            this.Name = "OptionForm";
            this.Text = "설정관리";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btn_config_save;
        public System.Windows.Forms.Button btn_rollback;
        private System.Windows.Forms.CheckBox checkBox_limited;
        public System.Windows.Forms.Button close;
        private System.Windows.Forms.CheckBox checkBox_today_sell;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox input_max_amt_limit;
        public System.Windows.Forms.TextBox input_buy_stop_rate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox input_battingAtm;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox input_repeat_rate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox input_stop_profit_target;
        public System.Windows.Forms.TextBox input_stop_profit_target2;
        private System.Windows.Forms.CheckBox checkBox_stopLoss;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox input_stopLoss;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBox_exclStopLossAt;
    }
}