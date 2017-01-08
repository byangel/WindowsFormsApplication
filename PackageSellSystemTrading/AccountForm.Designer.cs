namespace PackageSellSystemTrading
{
    partial class AccountForm
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
            this.input_accountPw = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_account_check = new System.Windows.Forms.Button();
            this.listBox_account = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // input_accountPw
            // 
            this.input_accountPw.Location = new System.Drawing.Point(117, 36);
            this.input_accountPw.Name = "input_accountPw";
            this.input_accountPw.Size = new System.Drawing.Size(116, 21);
            this.input_accountPw.TabIndex = 26;
            this.input_accountPw.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_accountPw_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 25;
            this.label8.Text = "계좌비밀번호";
            // 
            // btn_account_check
            // 
            this.btn_account_check.Location = new System.Drawing.Point(97, 220);
            this.btn_account_check.Name = "btn_account_check";
            this.btn_account_check.Size = new System.Drawing.Size(83, 29);
            this.btn_account_check.TabIndex = 29;
            this.btn_account_check.Text = "확인";
            this.btn_account_check.UseVisualStyleBackColor = true;
            this.btn_account_check.Click += new System.EventHandler(this.btn_account_check_Click);
            // 
            // listBox_account
            // 
            this.listBox_account.FormattingEnabled = true;
            this.listBox_account.ItemHeight = 12;
            this.listBox_account.Location = new System.Drawing.Point(36, 60);
            this.listBox_account.Name = "listBox_account";
            this.listBox_account.Size = new System.Drawing.Size(197, 136);
            this.listBox_account.TabIndex = 30;
            // 
            // AccountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.listBox_account);
            this.Controls.Add(this.btn_account_check);
            this.Controls.Add(this.input_accountPw);
            this.Controls.Add(this.label8);
            this.Name = "AccountForm";
            this.Text = "게좌목록";
            this.Load += new System.EventHandler(this.AccountForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TextBox input_accountPw;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_account_check;
        private System.Windows.Forms.ListBox listBox_account;
    }
}