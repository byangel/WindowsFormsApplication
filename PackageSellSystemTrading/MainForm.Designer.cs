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
            this.searchBuyGrid = new System.Windows.Forms.DataGridView();
            this.searchBtn = new System.Windows.Forms.Button();
            this.mfLog = new System.Windows.Forms.ListView();
            this.time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.combox_targetServer = new System.Windows.Forms.ComboBox();
            this.ds_server = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataTable2 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.searchBuyGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds_server)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).BeginInit();
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
            // searchBuyGrid
            // 
            this.searchBuyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchBuyGrid.Location = new System.Drawing.Point(339, 32);
            this.searchBuyGrid.Name = "searchBuyGrid";
            this.searchBuyGrid.RowTemplate.Height = 23;
            this.searchBuyGrid.Size = new System.Drawing.Size(240, 150);
            this.searchBuyGrid.TabIndex = 13;
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(339, 188);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(83, 29);
            this.searchBtn.TabIndex = 14;
            this.searchBtn.Text = "종목검색";
            this.searchBtn.UseVisualStyleBackColor = true;
            this.searchBtn.Click += new System.EventHandler(this.searchBtn_Click);
            // 
            // mfLog
            // 
            this.mfLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.time,
            this.message});
            this.mfLog.Location = new System.Drawing.Point(621, 32);
            this.mfLog.Name = "mfLog";
            this.mfLog.Size = new System.Drawing.Size(255, 434);
            this.mfLog.TabIndex = 15;
            this.mfLog.UseCompatibleStateImageBehavior = false;
            // 
            // time
            // 
            this.time.Text = "시간";
            this.time.Width = 381;
            // 
            // message
            // 
            this.message.Text = "메세지";
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
            this.dataTable2});
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(65, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "접속서버";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 633);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.combox_targetServer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mfLog);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.searchBuyGrid);
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
            ((System.ComponentModel.ISupportInitialize)(this.searchBuyGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds_server)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).EndInit();
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
        private System.Windows.Forms.DataGridView searchBuyGrid;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.ListView mfLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader time;
        private System.Windows.Forms.ColumnHeader message;
        private System.Windows.Forms.ComboBox combox_targetServer;
        private System.Data.DataSet ds_server;
        private System.Data.DataTable dataTable1;
        private System.Data.DataTable dataTable2;
        private System.Data.DataColumn dataColumn1;
        private System.Windows.Forms.Label label6;
    }
}

