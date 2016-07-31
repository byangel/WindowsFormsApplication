namespace WindowsFormsApplication2
{
    partial class mForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.realServerBtn = new System.Windows.Forms.RadioButton();
            this.demoServerBtn = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.loginIdInput = new System.Windows.Forms.TextBox();
            this.loginPassInput = new System.Windows.Forms.TextBox();
            this.publicPassInput = new System.Windows.Forms.TextBox();
            this.accountPassInput = new System.Windows.Forms.TextBox();
            this.loginBtn = new System.Windows.Forms.Button();
            this.logOutBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // realServerBtn
            // 
            this.realServerBtn.AutoSize = true;
            this.realServerBtn.Location = new System.Drawing.Point(133, 12);
            this.realServerBtn.Name = "realServerBtn";
            this.realServerBtn.Size = new System.Drawing.Size(59, 16);
            this.realServerBtn.TabIndex = 1;
            this.realServerBtn.TabStop = true;
            this.realServerBtn.Text = "실서버";
            this.realServerBtn.UseVisualStyleBackColor = true;
            // 
            // demoServerBtn
            // 
            this.demoServerBtn.AutoSize = true;
            this.demoServerBtn.Location = new System.Drawing.Point(198, 12);
            this.demoServerBtn.Name = "demoServerBtn";
            this.demoServerBtn.Size = new System.Drawing.Size(71, 16);
            this.demoServerBtn.TabIndex = 2;
            this.demoServerBtn.TabStop = true;
            this.demoServerBtn.Text = "데모서버";
            this.demoServerBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "로그인ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "비밀 번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "공인인증";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(62, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "계좌비번";
            // 
            // loginIdInput
            // 
            this.loginIdInput.Location = new System.Drawing.Point(121, 45);
            this.loginIdInput.Name = "loginIdInput";
            this.loginIdInput.Size = new System.Drawing.Size(148, 21);
            this.loginIdInput.TabIndex = 7;
            // 
            // loginPassInput
            // 
            this.loginPassInput.Location = new System.Drawing.Point(121, 73);
            this.loginPassInput.Name = "loginPassInput";
            this.loginPassInput.Size = new System.Drawing.Size(148, 21);
            this.loginPassInput.TabIndex = 8;
            // 
            // publicPassInput
            // 
            this.publicPassInput.Location = new System.Drawing.Point(121, 100);
            this.publicPassInput.Name = "publicPassInput";
            this.publicPassInput.Size = new System.Drawing.Size(148, 21);
            this.publicPassInput.TabIndex = 9;
            // 
            // accountPassInput
            // 
            this.accountPassInput.Location = new System.Drawing.Point(121, 127);
            this.accountPassInput.Name = "accountPassInput";
            this.accountPassInput.Size = new System.Drawing.Size(148, 21);
            this.accountPassInput.TabIndex = 10;
            // 
            // loginBtn
            // 
            this.loginBtn.Location = new System.Drawing.Point(65, 171);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Size = new System.Drawing.Size(83, 29);
            this.loginBtn.TabIndex = 11;
            this.loginBtn.Text = "로그인";
            this.loginBtn.UseVisualStyleBackColor = true;
            this.loginBtn.Click += new System.EventHandler(this.loginBtn_Click);
            // 
            // logOutBtn
            // 
            this.logOutBtn.Location = new System.Drawing.Point(154, 171);
            this.logOutBtn.Name = "logOutBtn";
            this.logOutBtn.Size = new System.Drawing.Size(83, 29);
            this.logOutBtn.TabIndex = 12;
            this.logOutBtn.Text = "로그아웃";
            this.logOutBtn.UseVisualStyleBackColor = true;
            this.logOutBtn.Click += new System.EventHandler(this.logOutBtn_Click);
            // 
            // mForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 478);
            this.Controls.Add(this.logOutBtn);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.accountPassInput);
            this.Controls.Add(this.publicPassInput);
            this.Controls.Add(this.loginPassInput);
            this.Controls.Add(this.loginIdInput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.demoServerBtn);
            this.Controls.Add(this.realServerBtn);
            this.Name = "mForm";
            this.Text = "angel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton realServerBtn;
        private System.Windows.Forms.RadioButton demoServerBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox loginIdInput;
        private System.Windows.Forms.TextBox loginPassInput;
        private System.Windows.Forms.TextBox publicPassInput;
        private System.Windows.Forms.TextBox accountPassInput;
        private System.Windows.Forms.Button loginBtn;
        private System.Windows.Forms.Button logOutBtn;
    }
}

