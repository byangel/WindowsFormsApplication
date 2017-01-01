namespace PackageSellSystemTrading
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.input_loginId = new System.Windows.Forms.TextBox();
            this.input_loginPw = new System.Windows.Forms.TextBox();
            this.input_publicPass = new System.Windows.Forms.TextBox();
            this.mf_loginBtn = new System.Windows.Forms.Button();
            this.logOutBtn = new System.Windows.Forms.Button();
            this.grd_searchBuy = new System.Windows.Forms.DataGridView();
            this.shcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.close = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.diff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.searchBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.combox_targetServer = new System.Windows.Forms.ComboBox();
            this.ds_server = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataTable2 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataTable3 = new System.Data.DataTable();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grd_accoun0424 = new System.Windows.Forms.DataGridView();
            this.sellCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.expcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exhname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mdposqt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.appamt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dtsunik = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sunikrt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pamt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mamt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.msat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mpms = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mdat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mpmd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sininter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label7 = new System.Windows.Forms.Label();
            this.comBox_account = new System.Windows.Forms.ComboBox();
            this.input_accountPw = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grd_searchBuy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds_server)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_accoun0424)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1051, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "로그인ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1046, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "비밀 번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1050, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "공인인증";
            // 
            // input_loginId
            // 
            this.input_loginId.Location = new System.Drawing.Point(1105, 31);
            this.input_loginId.Name = "input_loginId";
            this.input_loginId.Size = new System.Drawing.Size(116, 21);
            this.input_loginId.TabIndex = 7;
            // 
            // input_loginPw
            // 
            this.input_loginPw.Location = new System.Drawing.Point(1105, 53);
            this.input_loginPw.Name = "input_loginPw";
            this.input_loginPw.Size = new System.Drawing.Size(116, 21);
            this.input_loginPw.TabIndex = 8;
            // 
            // input_publicPass
            // 
            this.input_publicPass.Location = new System.Drawing.Point(1105, 75);
            this.input_publicPass.Name = "input_publicPass";
            this.input_publicPass.Size = new System.Drawing.Size(116, 21);
            this.input_publicPass.TabIndex = 9;
            // 
            // mf_loginBtn
            // 
            this.mf_loginBtn.Location = new System.Drawing.Point(1049, 123);
            this.mf_loginBtn.Name = "mf_loginBtn";
            this.mf_loginBtn.Size = new System.Drawing.Size(83, 29);
            this.mf_loginBtn.TabIndex = 11;
            this.mf_loginBtn.Text = "로그인";
            this.mf_loginBtn.UseVisualStyleBackColor = true;
            this.mf_loginBtn.Click += new System.EventHandler(this.btn_login_click);
            // 
            // logOutBtn
            // 
            this.logOutBtn.Location = new System.Drawing.Point(1138, 123);
            this.logOutBtn.Name = "logOutBtn";
            this.logOutBtn.Size = new System.Drawing.Size(83, 29);
            this.logOutBtn.TabIndex = 12;
            this.logOutBtn.Text = "로그아웃";
            this.logOutBtn.UseVisualStyleBackColor = true;
            this.logOutBtn.Click += new System.EventHandler(this.logOutBtn_Click);
            // 
            // grd_searchBuy
            // 
            this.grd_searchBuy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd_searchBuy.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.shcode,
            this.hname,
            this.close,
            this.sign,
            this.change,
            this.diff,
            this.volume});
            this.grd_searchBuy.Location = new System.Drawing.Point(12, 31);
            this.grd_searchBuy.Name = "grd_searchBuy";
            this.grd_searchBuy.RowTemplate.Height = 23;
            this.grd_searchBuy.Size = new System.Drawing.Size(532, 245);
            this.grd_searchBuy.TabIndex = 13;
            // 
            // shcode
            // 
            this.shcode.FillWeight = 24.82159F;
            this.shcode.HeaderText = "코드";
            this.shcode.Name = "shcode";
            this.shcode.Width = 70;
            // 
            // hname
            // 
            this.hname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.hname.FillWeight = 34.36586F;
            this.hname.HeaderText = "종목명";
            this.hname.Name = "hname";
            // 
            // close
            // 
            this.close.FillWeight = 49.17073F;
            this.close.HeaderText = "현재가";
            this.close.Name = "close";
            this.close.Width = 70;
            // 
            // sign
            // 
            this.sign.FillWeight = 72.13579F;
            this.sign.HeaderText = "구분";
            this.sign.Name = "sign";
            this.sign.Width = 50;
            // 
            // change
            // 
            this.change.FillWeight = 107.7587F;
            this.change.HeaderText = "전일대비";
            this.change.Name = "change";
            this.change.Width = 70;
            // 
            // diff
            // 
            this.diff.FillWeight = 163.0164F;
            this.diff.HeaderText = "등락율";
            this.diff.Name = "diff";
            this.diff.Width = 70;
            // 
            // volume
            // 
            this.volume.FillWeight = 248.731F;
            this.volume.HeaderText = "거래량";
            this.volume.Name = "volume";
            this.volume.Width = 70;
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(33, 282);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(83, 29);
            this.searchBtn.TabIndex = 14;
            this.searchBtn.Text = "종목검색";
            this.searchBtn.UseVisualStyleBackColor = true;
            this.searchBtn.Click += new System.EventHandler(this.searchBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(548, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "계좌잔고";
            // 
            // combox_targetServer
            // 
            this.combox_targetServer.FormattingEnabled = true;
            this.combox_targetServer.Location = new System.Drawing.Point(1105, 98);
            this.combox_targetServer.Name = "combox_targetServer";
            this.combox_targetServer.Size = new System.Drawing.Size(116, 20);
            this.combox_targetServer.TabIndex = 17;
            // 
            // ds_server
            // 
            this.ds_server.DataSetName = "ds_server";
            this.ds_server.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1,
            this.dataTable2,
            this.dataTable3});
            // 
            // dataTable1
            // 
            this.dataTable1.TableName = "Table1";
            // 
            // dataTable2
            // 
            this.dataTable2.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1});
            this.dataTable2.TableName = "Table2";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Column1";
            // 
            // dataTable3
            // 
            this.dataTable3.TableName = "Table3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1049, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "접속서버";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "검색종목";
            // 
            // grd_accoun0424
            // 
            this.grd_accoun0424.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd_accoun0424.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sellCheck,
            this.expcode,
            this.exhname,
            this.mdposqt,
            this.price,
            this.appamt,
            this.dtsunik,
            this.sunikrt,
            this.pamt,
            this.mamt,
            this.msat,
            this.mpms,
            this.mdat,
            this.mpmd,
            this.fee,
            this.tax,
            this.sininter});
            this.grd_accoun0424.Location = new System.Drawing.Point(550, 31);
            this.grd_accoun0424.Name = "grd_accoun0424";
            this.grd_accoun0424.RowTemplate.Height = 23;
            this.grd_accoun0424.Size = new System.Drawing.Size(465, 245);
            this.grd_accoun0424.TabIndex = 20;
            // 
            // sellCheck
            // 
            this.sellCheck.HeaderText = "선택";
            this.sellCheck.Name = "sellCheck";
            this.sellCheck.Width = 35;
            // 
            // expcode
            // 
            this.expcode.HeaderText = "코드";
            this.expcode.Name = "expcode";
            // 
            // exhname
            // 
            this.exhname.HeaderText = "종목명";
            this.exhname.Name = "exhname";
            // 
            // mdposqt
            // 
            this.mdposqt.HeaderText = "매도가능";
            this.mdposqt.Name = "mdposqt";
            // 
            // price
            // 
            this.price.HeaderText = "현재가";
            this.price.Name = "price";
            // 
            // appamt
            // 
            this.appamt.HeaderText = "평가금액";
            this.appamt.Name = "appamt";
            // 
            // dtsunik
            // 
            this.dtsunik.HeaderText = "평가손익";
            this.dtsunik.Name = "dtsunik";
            // 
            // sunikrt
            // 
            this.sunikrt.HeaderText = "수익율";
            this.sunikrt.Name = "sunikrt";
            // 
            // pamt
            // 
            this.pamt.HeaderText = "평균단가";
            this.pamt.Name = "pamt";
            // 
            // mamt
            // 
            this.mamt.HeaderText = "매입금액";
            this.mamt.Name = "mamt";
            // 
            // msat
            // 
            this.msat.HeaderText = "다일매수금액";
            this.msat.Name = "msat";
            // 
            // mpms
            // 
            this.mpms.HeaderText = "당일매수단가";
            this.mpms.Name = "mpms";
            // 
            // mdat
            // 
            this.mdat.HeaderText = "당일매도금액";
            this.mdat.Name = "mdat";
            // 
            // mpmd
            // 
            this.mpmd.HeaderText = "당일매도단가";
            this.mpmd.Name = "mpmd";
            // 
            // fee
            // 
            this.fee.HeaderText = "수수료";
            this.fee.Name = "fee";
            // 
            // tax
            // 
            this.tax.HeaderText = "제세금";
            this.tax.Name = "tax";
            // 
            // sininter
            // 
            this.sininter.HeaderText = "신용이자";
            this.sininter.Name = "sininter";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1049, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "계좌번호";
            // 
            // comBox_account
            // 
            this.comBox_account.FormattingEnabled = true;
            this.comBox_account.Location = new System.Drawing.Point(1105, 158);
            this.comBox_account.Name = "comBox_account";
            this.comBox_account.Size = new System.Drawing.Size(116, 20);
            this.comBox_account.TabIndex = 23;
            // 
            // input_accountPw
            // 
            this.input_accountPw.Location = new System.Drawing.Point(1134, 184);
            this.input_accountPw.Name = "input_accountPw";
            this.input_accountPw.Size = new System.Drawing.Size(88, 21);
            this.input_accountPw.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1051, 191);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "계좌비밀번호";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(550, 282);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 29);
            this.button1.TabIndex = 25;
            this.button1.Text = "계좌검색";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1105, 249);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(88, 21);
            this.textBox1.TabIndex = 27;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1038, 258);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "예수금(D2)";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(1105, 384);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(88, 21);
            this.textBox2.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1046, 390);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 28;
            this.label10.Text = "평가손익";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(1105, 357);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(88, 21);
            this.textBox3.TabIndex = 31;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1046, 363);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 30;
            this.label11.Text = "평가금액";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(1105, 330);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(88, 21);
            this.textBox4.TabIndex = 33;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1046, 336);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 32;
            this.label12.Text = "매입금액";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(1105, 303);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(88, 21);
            this.textBox5.TabIndex = 35;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1046, 309);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 34;
            this.label13.Text = "실현손익";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(1105, 276);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(88, 21);
            this.textBox6.TabIndex = 37;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(1046, 282);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 12);
            this.label14.TabIndex = 36;
            this.label14.Text = "추정자산";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 633);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comBox_account);
            this.Controls.Add(this.input_accountPw);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.grd_accoun0424);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.combox_targetServer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.grd_searchBuy);
            this.Controls.Add(this.logOutBtn);
            this.Controls.Add(this.mf_loginBtn);
            this.Controls.Add(this.input_publicPass);
            this.Controls.Add(this.input_loginPw);
            this.Controls.Add(this.input_loginId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "PackageSellSystemTrading";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grd_searchBuy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds_server)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_accoun0424)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox input_loginId;
        private System.Windows.Forms.TextBox input_loginPw;
        private System.Windows.Forms.TextBox input_publicPass;
        private System.Windows.Forms.Button mf_loginBtn;
        private System.Windows.Forms.Button logOutBtn;
        public  System.Windows.Forms.DataGridView grd_searchBuy;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox combox_targetServer;
        private System.Data.DataSet ds_server;
        private System.Data.DataTable dataTable1;
        private System.Data.DataTable dataTable2;
        private System.Data.DataColumn dataColumn1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Data.DataTable dataTable3;
        private System.Windows.Forms.DataGridViewTextBoxColumn shcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn hname;
        private System.Windows.Forms.DataGridViewTextBoxColumn close;
        private System.Windows.Forms.DataGridViewTextBoxColumn sign;
        private System.Windows.Forms.DataGridViewTextBoxColumn change;
        private System.Windows.Forms.DataGridViewTextBoxColumn diff;
        private System.Windows.Forms.DataGridViewTextBoxColumn volume;
        private System.Windows.Forms.DataGridViewCheckBoxColumn sellCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn expcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn exhname;
        private System.Windows.Forms.DataGridViewTextBoxColumn mdposqt;
        private System.Windows.Forms.DataGridViewTextBoxColumn price;
        private System.Windows.Forms.DataGridViewTextBoxColumn appamt;
        private System.Windows.Forms.DataGridViewTextBoxColumn dtsunik;
        private System.Windows.Forms.DataGridViewTextBoxColumn sunikrt;
        private System.Windows.Forms.DataGridViewTextBoxColumn pamt;
        private System.Windows.Forms.DataGridViewTextBoxColumn mamt;
        private System.Windows.Forms.DataGridViewTextBoxColumn msat;
        private System.Windows.Forms.DataGridViewTextBoxColumn mpms;
        private System.Windows.Forms.DataGridViewTextBoxColumn mdat;
        private System.Windows.Forms.DataGridViewTextBoxColumn mpmd;
        private System.Windows.Forms.DataGridViewTextBoxColumn fee;
        private System.Windows.Forms.DataGridViewTextBoxColumn tax;
        private System.Windows.Forms.DataGridViewTextBoxColumn sininter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.ComboBox comBox_account;
        public System.Windows.Forms.TextBox input_accountPw;
        public System.Windows.Forms.DataGridView grd_accoun0424;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label14;
    }
}

