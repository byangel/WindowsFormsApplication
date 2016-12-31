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
            this.input_loginPass = new System.Windows.Forms.TextBox();
            this.input_publicPass = new System.Windows.Forms.TextBox();
            this.mf_loginBtn = new System.Windows.Forms.Button();
            this.logOutBtn = new System.Windows.Forms.Button();
            this.grd_searchBuy = new System.Windows.Forms.DataGridView();
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
            this.shcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.close = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.diff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grd_searchBuy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds_server)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable3)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "로그인ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "비밀 번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "공인인증";
            // 
            // input_loginId
            // 
            this.input_loginId.Location = new System.Drawing.Point(121, 45);
            this.input_loginId.Name = "input_loginId";
            this.input_loginId.Size = new System.Drawing.Size(116, 21);
            this.input_loginId.TabIndex = 7;
            // 
            // input_loginPass
            // 
            this.input_loginPass.Location = new System.Drawing.Point(121, 67);
            this.input_loginPass.Name = "input_loginPass";
            this.input_loginPass.Size = new System.Drawing.Size(116, 21);
            this.input_loginPass.TabIndex = 8;
            // 
            // input_publicPass
            // 
            this.input_publicPass.Location = new System.Drawing.Point(121, 89);
            this.input_publicPass.Name = "input_publicPass";
            this.input_publicPass.Size = new System.Drawing.Size(116, 21);
            this.input_publicPass.TabIndex = 9;
            // 
            // mf_loginBtn
            // 
            this.mf_loginBtn.Location = new System.Drawing.Point(65, 137);
            this.mf_loginBtn.Name = "mf_loginBtn";
            this.mf_loginBtn.Size = new System.Drawing.Size(83, 29);
            this.mf_loginBtn.TabIndex = 11;
            this.mf_loginBtn.Text = "로그인";
            this.mf_loginBtn.UseVisualStyleBackColor = true;
            this.mf_loginBtn.Click += new System.EventHandler(this.mf_loginBtn_Click);
            // 
            // logOutBtn
            // 
            this.logOutBtn.Location = new System.Drawing.Point(154, 137);
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
            this.grd_searchBuy.Location = new System.Drawing.Point(339, 32);
            this.grd_searchBuy.Name = "grd_searchBuy";
            this.grd_searchBuy.RowTemplate.Height = 23;
            this.grd_searchBuy.Size = new System.Drawing.Size(526, 245);
            this.grd_searchBuy.TabIndex = 13;
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(339, 283);
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
            this.label5.Location = new System.Drawing.Point(649, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "로그 기록창";
            // 
            // combox_targetServer
            // 
            this.combox_targetServer.FormattingEnabled = true;
            this.combox_targetServer.Location = new System.Drawing.Point(121, 112);
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
            this.label6.Location = new System.Drawing.Point(65, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "접속서버";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(337, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 19;
            this.label4.Text = "검색종목";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 633);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.combox_targetServer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.grd_searchBuy);
            this.Controls.Add(this.logOutBtn);
            this.Controls.Add(this.mf_loginBtn);
            this.Controls.Add(this.input_publicPass);
            this.Controls.Add(this.input_loginPass);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox input_loginId;
        private System.Windows.Forms.TextBox input_loginPass;
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
    }
}

