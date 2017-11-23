namespace PackageSellSystemTrading
{
    partial class HistoryForm
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
            this.grd_history = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_search = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.input_searchString = new System.Windows.Forms.TextBox();
            this.btn_delete = new System.Windows.Forms.Button();
            this.check_flag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ordno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.useYn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grd_history)).BeginInit();
            this.SuspendLayout();
            // 
            // grd_history
            // 
            this.grd_history.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grd_history.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grd_history.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.check_flag,
            this.ordno,
            this.useYn});
            this.grd_history.Location = new System.Drawing.Point(12, 33);
            this.grd_history.Name = "grd_history";
            this.grd_history.RowTemplate.Height = 23;
            this.grd_history.Size = new System.Drawing.Size(694, 440);
            this.grd_history.TabIndex = 0;
            
            this.grd_history.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grd_history_CellValueChanged);
            this.grd_history.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.grd_history_RowsAdded);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 17;
            this.label5.Text = "매매이력:";
            // 
            // btn_search
            // 
            this.btn_search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_search.Font = new System.Drawing.Font("굴림", 9.25F);
            this.btn_search.Location = new System.Drawing.Point(530, 9);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(41, 20);
            this.btn_search.TabIndex = 18;
            this.btn_search.Text = "조회";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(577, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 20);
            this.button1.TabIndex = 19;
            this.button1.Text = "test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // input_searchString
            // 
            this.input_searchString.Location = new System.Drawing.Point(340, 8);
            this.input_searchString.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.input_searchString.Name = "input_searchString";
            this.input_searchString.Size = new System.Drawing.Size(183, 21);
            this.input_searchString.TabIndex = 22;
            // 
            // btn_delete
            // 
            this.btn_delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_delete.Location = new System.Drawing.Point(632, 9);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(74, 20);
            this.btn_delete.TabIndex = 23;
            this.btn_delete.Text = "선택삭제";
            this.btn_delete.UseVisualStyleBackColor = true;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // check_flag
            // 
            this.check_flag.FillWeight = 50F;
            this.check_flag.HeaderText = "선택";
            this.check_flag.Name = "check_flag";
            this.check_flag.Width = 50;
            // 
            // ordno
            // 
            this.ordno.DataPropertyName = "ordno";
            this.ordno.FillWeight = 70F;
            this.ordno.HeaderText = "주문번호";
            this.ordno.Name = "ordno";
            // 
            // useYn
            // 
            this.useYn.DataPropertyName = "useYn";
            this.useYn.HeaderText = "사용여부";
            this.useYn.Name = "useYn";
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 485);
            this.Controls.Add(this.btn_delete);
            this.Controls.Add(this.input_searchString);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_search);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.grd_history);
            this.Name = "HistoryForm";
            this.Text = "매매이력";
            ((System.ComponentModel.ISupportInitialize)(this.grd_history)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.DataGridView grd_history;
        private System.Windows.Forms.TextBox input_searchString;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.DataGridViewCheckBoxColumn check_flag;
        private System.Windows.Forms.DataGridViewTextBoxColumn ordno;
        private System.Windows.Forms.DataGridViewTextBoxColumn useYn;
    }
}