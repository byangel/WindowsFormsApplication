using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

using XA_DATASETLib;
using XA_SESSIONLib;
using System.Data.SQLite;

namespace PackageSellSystemTrading{
    public partial class MainForm : System.Windows.Forms.Form{

        //배팅금액
        public decimal battingAmt;

        public ExXASessionClass  exXASessionClass;
        public Xing_t1833        xing_t1833;        //조건검색
        public Xing_t1833Exclude xing_t1833Exclude; //매수금지종목
        public Xing_t0424        xing_t0424;        //잔고2
       
        public Xing_t0425        xing_t0425;        //체결/미체결
        public Xing_t0167        xing_t0167;        //시간조회
        public Xing_CSPAT00600   xing_CSPAT00600;   //주식주문
        public Xing_CSPAT00800   xing_CSPAT00800;   //현물 취소주문
        public Xing_CSPAQ12200   xing_CSPAQ12200;   //현물계좌예수금/주문가능금액/총평가 조회

        public AccountForm       accountForm;       //계좌 선택
        public OptionForm        optionForm;        //프로그램 설정 폼

        public Real_SC1          real_SC1; //실시간 체결
        public Real_S3           real_S3;  //코스피 체결체결
        public Real_K3           real_K3;  //코스닥 체결체결
        public Real_jif          real_jif; //장 정보

        public DataLog           dataLog; //데이타로그

        public String            tradingAt;//매매 여부 Y|N


        public String account; //계좌번호
        public String accountPw;//계좌 비밀번호
        //public String account { get; set; }
        //public String accountPw { get; set; }

        //생성자
        public MainForm(){
            InitializeComponent();
        }
      

        //1회 주문시 매입금액

        private void MainForm_Load(object sender, EventArgs e)
        {
            //폼 초기화
            initForm();
        }

        //프로그램시작시 폼 초기화
        private void initForm(){
            //접속가능
            //실운영 : hts.ebestsec.co.kr
            //모의투자 : demo.ebestsec.co.kr
            exXASessionClass = new ExXASessionClass();//로그인
            exXASessionClass.mainForm = this;

            this.xing_t1833 = new Xing_t1833();//종목검색
            this.xing_t1833.mainForm = this;
            this.xing_t1833Exclude = new Xing_t1833Exclude();//매수금지종목검색
            this.xing_t1833Exclude.mainForm = this;
            this.xing_t0424 = new Xing_t0424();//주식잔고2
            this.xing_t0424.mainForm = this;
            this.xing_t0425 = new Xing_t0425();//체결/미체결
            this.xing_t0425.mainForm = this;
            this.xing_t0167 = new Xing_t0167();//서버시간조회
            this.xing_t0167.mainForm = this;
            this.xing_CSPAT00600 = new Xing_CSPAT00600();//정상주문
            this.xing_CSPAT00600.mainForm = this;
            this.xing_CSPAT00800 = new Xing_CSPAT00800();//현물취소주문
            this.xing_CSPAT00800.mainForm = this;
            this.xing_CSPAQ12200 = new Xing_CSPAQ12200(); //현물계좌예수금/주문가능금액/총평가 조회
            this.xing_CSPAQ12200.mainForm = this;

            this.accountForm = new AccountForm();//계좌선택폼
            this.accountForm.mainForm = this;
            this.accountForm.exXASessionClass = exXASessionClass;
            this.optionForm = new OptionForm();
            this.optionForm.mainForm = this;

            this.real_SC1 = new Real_SC1();    //실시간 체결
            this.real_SC1.mainForm = this;
            this.real_S3 = new Real_S3();    //코스피 실시간 체결
            this.real_S3.mainForm = this;
            this.real_K3 = new Real_K3();    //코스닥 실시간 체결
            this.real_K3.mainForm = this;
            this.real_jif = new Real_jif();    //장정보
            this.real_jif.mainForm = this;

            this.dataLog = new DataLog();
            this.dataLog.mainForm = this;
            //계좌잔고 그리드 초기화
            grd_t0424.DataSource = this.xing_t0424.getT0424VoList();
            //진입검색 그리드.
            grd_t1833.DataSource = this.xing_t1833.getT1833VoList(); ;
            //체결미체결 그리드 DataSource 설정
            grd_t0425.DataSource = this.xing_t0425.getT0425VoList(); //체결/미체결 그리드

            input_loginId.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_ID);
            input_loginPw.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_PW);
            input_publicPw.Text = Util.Decrypt(Properties.Settings.Default.PUBLIC_PW);

            //서버 선택 콤보 초기화
            //combox_condition.SelectedIndex = 0;
            for (int i = 0; i < combox_targetServer.Items.Count; i++)
            {
                if (combox_targetServer.Items[i].ToString() == Properties.Settings.Default.SERVER_ADDRESS)
                {
                    combox_targetServer.SelectedIndex = i;
                }
            }


            //프로그램 최초 실행시 프로퍼티 설정이 안되어잇기 때문에 초기 셋팅값을 설정해준다.
            if (Properties.Settings.Default.CONDITION_ADF.ToString() == "")
            {
                optionForm.rollBack();
                optionForm.btn_config_save_Click(new object(), new EventArgs());
            }

        }

        
        //properties 저장
        private void btn_config_save_Click(object sender, EventArgs e)  {

            // UI 필드 값을 읽어서 변수에 담음 - 비번등은 암호화 시킴
            String serverAddress = combox_targetServer.Text;
            String loginId       = Util.Encrypt(input_loginId.Text.Trim());
            String loginPw       = Util.Encrypt(input_loginPw.Text.Trim());
            String publicPw      = Util.Encrypt(input_publicPw.Text.Trim());

            // 설정 파일에 저장
            Properties.Settings.Default.SERVER_ADDRESS = serverAddress.ToString();
            Properties.Settings.Default.LOGIN_ID       = loginId;
            Properties.Settings.Default.LOGIN_PW       = loginPw;
            Properties.Settings.Default.PUBLIC_PW      = publicPw;
            //Properties.Settings.Default.ACCOUNT        = this.exXASessionClass.account;
            //Properties.Settings.Default.ACCOUNT_PW     = this.exXASessionClass.accountPw;
            //Properties.Settings.Default.AUTO_LOGIN = CheckAutoLogin.Checked;
            //Properties.Settings.Default.TRAY_YN = CheckTrayYN.Checked;

            Properties.Settings.Default.Save();
            MessageBox.Show("로그인 설정을 저장했습니다..!!");

        }

        //종목검색 버튼
        private void btn_search_Click(object sender, EventArgs e)
        {
            xing_t1833.call_request();
        }

        //로그아웃 버튼
        private void btn_logout_Click(object sender, EventArgs e)
        {
            this.tradingStop();
            this.exXASessionClass.DisconnectServer();
            this.account = null;
            this.accountPw = null;
            // 로그인 버튼 활성
            this.btn_login.Enabled = true;
            MessageBox.Show("성공적으로 로그아웃 하였습니다.");
        }

        


        //로그인 버튼 클릭 이벤트
        private void btn_login_click(object sender, EventArgs e)
        {
            exXASessionClass.fnLogin();
        }

        //주식 잔고2
        private void btn_accountSearch_Click(object sender, EventArgs e) {

            if (this.account == "" || this.accountPw == ""){
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
                xing_t0424.call_request(this.account, this.accountPw);
            }

            //setRowNumber(grd_t0424);

        }



        //미체결내역
        private void btn_t0425_Click(object sender, EventArgs e)
        {
            xing_t0425.call_request(this.account, this.accountPw);
        }

        //시작버튼 클릭 이벤트
        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.exXASessionClass.IsConnected())
                {
                    //계좌번호까지있어야 로그인으로 간주하자.
                    if (this.account == null || this.accountPw == null)
                    {
                        MessageBox.Show("로그인 후 서비스 이용가능합니다."); 
                    } else {
                        this.tradingRun();
                    }
                } else {
                    MessageBox.Show("서버접속정보가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("mainForm : " + ex.Message);
                Log.WriteLine("mainForm : " + ex.StackTrace);
            }
        }
       
        //자동매매 시작
        public void tradingRun()
        {
            //timer_t1833Exclude.Start();//진입검색 타이머
            //timer_accountSearch.Start();//계좌 및 미체결 검색 타이머
            this.tradingAt = "Y";

            //타이머 시작 --여기서 타이머 시작해주면 타이머 스톱해줄일은 없어진다.그리고  잔고정보,잔고목록,매매이력 등등을 호출안해줘도 된다.
            this.timer_t1833Exclude.Start();//진입검색 타이머
            this.timer_common.Start();//계좌 및 미체결 검색 타이머
            this.Timer0167.Start();//시간검색

            btn_start.Enabled = false;//시작버튼 비활성
            btn_stop.Enabled = true;//종료버튼 활성
            Log.WriteLine("Trading Start..!!");
        }

        //자동매매 정지
        public void tradingStop()
        {
            //timer_t1833Exclude.Stop();//진입검색 타이머
            //timer_accountSearch.Stop();//계좌 및 미체결 검색 타이머
            this.tradingAt = "N";

            this.timer_t1833Exclude.Stop();//진입검색 타이머
            this.timer_common.Stop();//계좌 및 미체결 검색 타이머
            this.Timer0167.Stop();//시간검색

            btn_start.Enabled = true;
            btn_stop.Enabled = false;
            Log.WriteLine("Trading Stop..!!");
        }

        //정지버튼 클릭 이벤트
        private void btn_stop_Click(object sender, EventArgs e)
        {
            this.tradingStop();
        }

      



        //체결 그리드 row 번호 출력
        private void grd_t0425_chegb1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rect = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, grd_t0425.RowHeadersWidth - 4, e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics
                                , (e.RowIndex + 1).ToString()
                                , grd_t0425.RowHeadersDefaultCellStyle.Font
                                , rect
                                , grd_t0425.RowHeadersDefaultCellStyle.ForeColor
                                , TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        //잔고 그리드 row 번호 출력
        private void grd_t0424_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            
            Rectangle rect = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, grd_t0424.RowHeadersWidth - 4, e.RowBounds.Height);
         
            TextRenderer.DrawText(e.Graphics
                                ,(e.RowIndex+1).ToString()
                                , grd_t0424.RowHeadersDefaultCellStyle.Font
                                ,rect
                                , grd_t0424.RowHeadersDefaultCellStyle.ForeColor
                                ,TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }


        public void setRowNumber_(DataGridView dgv){
            //if (dgv.Rows.Count > 0) dgv.RowHeadersWidth = 50;
            dgv.RowHeadersWidth = 53;
            
            foreach (DataGridViewRow row in dgv.Rows) {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                
            }
            //dgv.AutoResizeRowHeadersWidth(  DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }


        //프로그램 설정 팝업 호출
        private void btn_option_config_Click(object sender, EventArgs e)
        {
            optionForm.ShowDialog();
        }
        
        //선택 매도
        private void btn_checkSell_Click(object sender, EventArgs e)
        {
            String expcode; //종목코드
            String hname;   //종목명
            String mdposqt; //주문가능수량수량
            String sunikrt; //수익율
            String price;   //현재가
            String pamt2;   //평균단가2
            //EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)this.grd_t0424.DataSource);
            for (int i=0;i< grd_t0424.RowCount; i++)
            {
               
                if (grd_t0424.Rows[i].Cells[0].FormattedValue.ToString() == "True") { 
                //{
                    //선택된 종목이 이미 매도주문이 나간 상태인지 체크하는 부분이 없다.(청산 관련 매도주문에는 DataLog 를 참조하지 않고 t0424를 참조한다.

                    expcode = grd_t0424.Rows[i].Cells[1].FormattedValue.ToString(); //종목코드
                    //주문 여부를 true로 업데이트
                    var result_t0424 = from   item in this.xing_t0424.getT0424VoList()
                                       where  item.expcode == expcode.Replace("A", "")
                                       select item;
                    int findIndex = this.xing_t0424.getT0424VoList().Find("expcode", expcode.Replace("A", ""));
                    
                    //MessageBox.Show(result_t0424.Count().ToString());

                    if (findIndex >= 0)
                    {
                        //MessageBox.Show(this.xing_t0424.getT0424VoList().ElementAt(findIndex).orderAt.ToString());
                        //주문여부
                        if (this.xing_t0424.getT0424VoList().ElementAt(findIndex).orderAt == false)
                        {
                            //MessageBox.Show(this.xing_t0424.getT0424VoList().ElementAt(findIndex).orderAt.ToString());
                            expcode = this.xing_t0424.getT0424VoList().ElementAt(findIndex).expcode; //종목코드
                            hname   = this.xing_t0424.getT0424VoList().ElementAt(findIndex).hname;   //종목명
                            sunikrt = this.xing_t0424.getT0424VoList().ElementAt(findIndex).sunikrt; //수익율
                            mdposqt = this.xing_t0424.getT0424VoList().ElementAt(findIndex).mdposqt; //주문가능수량수량
                            price   = this.xing_t0424.getT0424VoList().ElementAt(findIndex).price;   //현재가
                            pamt2 = this.xing_t0424.getT0424VoList().ElementAt(findIndex).pamt2;     //평균단가2

                            /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산(선택매도,손절매)</param>
                            /// <param name="upOrdno">상위매수주문번호</param>
                            /// <param name="upOrdno">상위체결금액</param>
                            /// <param name="IsuNo">종목명</param>
                            /// <param name="IsuNo">종목번호</param>
                            /// <param name="Quantity">수량</param>
                            /// <param name="Price">가격</param>
                            
                            xing_CSPAT00600.call_requestSell("선택매도", "none", pamt2, hname, expcode, mdposqt, price);
                          

                            this.xing_t0424.getT0424VoList().ElementAt(findIndex).orderAt = true;//일괄 매도시 주문여부를 true로 설정

                            Log.WriteLine("mainForm :: 선택매도" + hname + "(" + expcode + ")]  수익율:" + sunikrt + "%    주문수량및매도가능:" + mdposqt);
                        }
                        
                    }

                }
                
          

            }
        }

        //테스트 버튼 클릭 이벤트
        private void test_Click(object sender, EventArgs e)
        {
            try { 
                String dbFilePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\"+this.account+".db";

                SQLiteConnection conn = new SQLiteConnection("Data Source="+ dbFilePath+";Pooling=true;FailIfMissing=false");
                conn.Open();//파일이 없으면 자동 생성 //conn.Close();

                //테이블이 있는데 확인
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) cnt FROM sqlite_master WHERE name = 'trading_history'", conn);

                if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0){
                    cmd.CommandText = "CREATE TABLE tstring (tagname VARCHAR(16), dt DATETIME, val TINYTEXT, PRIMARY KEY(tagname, dt));";
                    cmd.ExecuteNonQuery();
                }


                //쿼리정의
                cmd.CommandText = "INSERT INTO trading_history  (tagname, dt, val) VALUES (@name, @dt, @val)";
                cmd.Parameters.Add("@name", DbType.String);
                cmd.Parameters.Add("@dt", DbType.DateTime);
                cmd.Parameters.Add("@val", DbType.Byte);
                cmd.Prepare();

                //String sql = "insert into members (name, age) values ('김도현', 6)";
                //SQLiteCommand command = new SQLiteCommand(sql, conn);
                //int result = command.ExecuteNonQuery();


                //......

                cmd.Parameters[0].Value = "1ㅇ";
                cmd.Parameters[1].Value = "2ㅇ";
                cmd.Parameters[2].Value = "3ㄹ";
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                Log.WriteLine("main : " + ex.Message);
                Log.WriteLine("main : " + ex.StackTrace);
            }


            //this.dataLogVoList = new EBindingList<DataLogVo>();



            //conn.Open();

            ////......

            //conn.Close();

            //SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) cnt FROM sqlite_master WHERE name = 'tstring'", conn);

            //if (Convert.ToInt32(cmd.ExecuteScalar()) <= 0)

            //{

            //    cmd.CommandText = "CREATE TABLE tstring (tagname VARCHAR(16), dt DATETIME, val TINYTEXT, PRIMARY KEY(tagname, dt));";

            //    cmd.ExecuteNonQuery();

            //}



            //cmd.CommandText = "INSERT INTO tstring  (tagname, dt, val) VALUES (@name, @dt, @val)";

            //cmd.Parameters.Add("@name", DbType.String);

            //cmd.Parameters.Add("@dt", DbType.DateTime);

            //cmd.Parameters.Add("@val", DbType.Byte);
            //cmd.Prepare();
            //cmd.Parameters[0].Value = tagid;
            //cmd.Parameters[1].Value = dtNow;
            //cmd.Parameters[2].Value = strvalue;
            //cmd.ExecuteNonQuery();




        }
        //취소
        private void button3_Click(object sender, EventArgs e)
        {
            xing_CSPAT00800.call_request(this.account, this.accountPw, "14074", "030270", "");
        }



        //grd_t0424 CellFormatting 
        private void grd_t0424_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //폰트색 지정
            if (e.ColumnIndex == 6 || e.ColumnIndex == 7)
            {
                if (e.Value != null)
                {

                    if (e.Value.ToString().IndexOf("-") >= 0)
                    {
                        e.CellStyle.ForeColor = Color.Blue;
                        e.CellStyle.SelectionForeColor = Color.Blue;
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.SelectionForeColor = Color.Red;

                    }
                }
            }
            
            ///Log.WriteLine("ㅇㅇㅇㅇ"+e.ColumnIndex.ToString()+"="+ e.Value);
            //4:현재가,14:수수료, 6:평가손익, 3:매도가능, 5:평가금액, 4:현재가
            //if (e.ColumnIndex != 1 && e.ColumnIndex != 2 && e.ColumnIndex != 7 && e.ColumnIndex != 8)
            //{
            //    if (e.Value != null)
            //    {
            //        //e.Value = String.Format("{0:#,##0}",e.Value.ToString());
            //        e.Value = Util.GetNumberFormat(e.Value.ToString());
            //    }
            //}

        }
        //폰트색 지정
        private void grd_t0425_chegb1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1 )
            {
                if (e.Value != null)
                {
                    if (e.Value.ToString().IndexOf("매도") >= 0)
                    {
                        e.CellStyle.ForeColor = Color.Blue;
                        e.CellStyle.SelectionForeColor = Color.Blue;
                    }else{
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.SelectionForeColor = Color.Red;
                    }
                }
            }

            if (e.ColumnIndex == 11)
            {
                if (e.Value != null)
                {
                    //if (e.Value.ToString().IndexOf("신규매수") >= 0)
                    if (e.Value.ToString() == "신규매수" || e.Value.ToString() == "반복매수")
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.SelectionForeColor = Color.Red;  
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Blue;
                        e.CellStyle.SelectionForeColor = Color.Blue;
                    }
                }
            }

            if (e.ColumnIndex == 6 || e.ColumnIndex == 8)
            {
                if (e.Value != null)
                {

                    e.Value = Util.GetNumberFormat(e.Value.ToString());
                }
            }
            //주문시간 :: 0 / 09094942
            //구분     :: 1 / 매도
            //상태     :: 9 / 체결
            //상세구분 :: 11 / 청산
            //종목코드 :: 2 / 057540
            //종목명   :: 3 / 옴니시스템
            //주문수량 :: 4 / 18
            //주문가격 :: 5 / 2840
            //제결수량 :: 6 / 18
            //Log.WriteLine(e.ColumnIndex.ToString()+"/"+ e.Value);
        }

        //조건검색 그리드
        private void grd_t1833_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (e.Value != null)
                {
                    e.Value = Util.GetNumberFormat(e.Value.ToString());
                }
            }
        }

        //잔고목록 검색
        private void input_searchText_TextChanged(object sender, EventArgs e)
        {
            int tmpIndexOf;
            TextBox tb = sender as TextBox;
            //MessageBox.Show(tb.Text.Count().ToString());
            //if (tb.Text.Count() > 2)
            //{
                for (int i = 0; i < grd_t0424.RowCount; i++)
                {
                   
                    if (tb.Text == "" || this.grd_t0424.Rows[i].Cells["c_hname"].Value == null) {

                        this.grd_t0424.Rows[i].Cells["c_hname"].Style.BackColor = Color.White;
                     }else{    
                       
                        tmpIndexOf = grd_t0424.Rows[i].Cells["c_hname"].Value.ToString().IndexOf(input_searchText.Text.ToString());
                       
                        if (tmpIndexOf >= 0){
                            this.grd_t0424.Rows[i].Cells["c_hname"].Style.BackColor = Color.Gray;
                        }else{
                            this.grd_t0424.Rows[i].Cells["c_hname"].Style.BackColor = Color.White;
                        }
                    }

                    if (tb.Text == "" || this.grd_t0424.Rows[i].Cells["c_expcode"].Value == null){
                        this.grd_t0424.Rows[i].Cells["c_expcode"].Style.BackColor = Color.White;
                    }else{

                        tmpIndexOf = grd_t0424.Rows[i].Cells["c_expcode"].Value.ToString().IndexOf(input_searchText.Text.ToString());

                        if (tmpIndexOf >= 0)
                        {
                            this.grd_t0424.Rows[i].Cells["c_expcode"].Style.BackColor = Color.Gray;
                        }
                        else
                        {
                            this.grd_t0424.Rows[i].Cells["c_expcode"].Style.BackColor = Color.White;
                        }
                }

            }
        }

       


        

        //실시간 현재가 정보 이벤트시 호출된다.
        public void realPriceCallBack(String shcode, String price)
        {
            String 당일매도금액;//매도수량
            String 당일매도단가;
            String 손익률2;
            String 수수료;
            String 세금;
            String 신용이자;
            String 매도가능수량;
            String 평균단가;
            String 평균단가2;
            String 평가손익;
            Double 손익률;
            Double 평가금액;
            Double 현재가 = Double.Parse(price);
            Double 매입금액;
            
            EBindingList<T0424Vo> t0424VoList = xing_t0424.getT0424VoList();
            int findIndex = t0424VoList.Find("expcode", shcode.Replace("A", ""));
            if (findIndex >= 0)
            {

                매도가능수량  = t0424VoList.ElementAt(findIndex).mdposqt;
                수수료       = t0424VoList.ElementAt(findIndex).fee;
                세금         = t0424VoList.ElementAt(findIndex).tax;
                신용이자     = t0424VoList.ElementAt(findIndex).sininter;
                평균단가     = t0424VoList.ElementAt(findIndex).pamt;
                평균단가2    = t0424VoList.ElementAt(findIndex).pamt2;
                당일매도금액 = t0424VoList.ElementAt(findIndex).mdat; //매도수량
                당일매도단가 = t0424VoList.ElementAt(findIndex).mpmd;
                //매입금액     = t0424VoList.ElementAt(findIndex).mamt;

                //평가금액: (보유수량 * 현재가) - 매도수수료(fee) - 매도제세금(tax) - 신용이자(sininter)
                ////평가금액 = (double.Parse(매도가능수량) * double.Parse(price)) - double.Parse(수수료) - double.Parse(세금) - double.Parse(신용이자);
                //매입금액: (매수수량 * 매수단가) + 매수수수료(fee) --매이금액은 변하지 않는 값이므로 계산하지 않고 기존데이타 활용
                //매입금액 = (double.Parse(매도가능수량) * double.Parse(평균단가)) + double.Parse(세금);
                //평가손익: 평가금액 - 매입금액
                /////평가손익 = 평가금액 - double.Parse(매입금액);
                //손익률: (평가손익 / 매입금액)*100
                ////손익률 = (평가손익 / double.Parse(매입금액)) * 100;
                //실현손익: (당일매도금액 - 매도수수료 - 매도제세금) - (매입금액 + 추정매입수수료) - 신용이자 
                ////실현손익 = ((double.Parse(당일매도금액) * double.Parse(당일매도단가)) - double.Parse(수수료) - double.Parse(세금)) - (double.Parse(매입금액) - double.Parse(수수료)) - double.Parse(신용이자);

                매입금액 = double.Parse(평균단가2) * double.Parse(매도가능수량);
                평가금액 = 현재가 * double.Parse(매도가능수량);
                평가금액 = 평가금액 - (평가금액 * 0.0033);
                평가손익 = Util.GetNumberFormat(평가금액 - 매입금액);

                현재가 = 현재가 - (현재가 * 0.0033);
                //1.현재가가 금일매수 값보다 3%이상 올랐으면 금일 매수 수량만큼 매도한다.
                손익률 = ((현재가 / Double.Parse(평균단가2)) * 100) - 100;

                grd_t0424.Rows[findIndex].Cells["price"].Value = Util.GetNumberFormat(현재가);
                grd_t0424.Rows[findIndex].Cells["appamt"].Value = Util.GetNumberFormat(평가금액);
                grd_t0424.Rows[findIndex].Cells["dtsunik"].Value = Util.GetNumberFormat(평가손익);
                grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = Math.Round(손익률, 2).ToString();
                //grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = Math.Round(테스트수익률, 2).ToString();
                //grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = 실현손익;


                //재계산 손익율
                손익률2 = Util.getSunikrt2(t0424VoList.ElementAt(findIndex));
                grd_t0424.Rows[findIndex].Cells["sunikrt2"].Value = 손익률2;

                //매도테스트 - 해당 보유종목 정보를 인자로 넘겨주어 매도가능인지 테스트한다.
                //this.xing_t0424.stopProFitTargetTest(t0424VoList.ElementAt(findIndex));

                
            }
        }//priceCallBack

        

        //실시간 체결(SC1) > 매도가능수량이 0이면 호출 
        public void deleteCallBack(String Isuno)
        {
            //dataLog 도 제거해준다.
            this.dataLog.deleteData(Isuno); //이상하게 반복매수에서 보유종목으로 통과되어서 에러난다. 그래서 아래 0424와 순서를 바꿔줘본다.1833에서 에러남

            //그리드삭제
            //this.grd_t0424.Rows.Remove(this.grd_t0424.Rows[findIndex]);//그리드에서 삭제하면 바인딩객체도 같이 삭제 되는지 잘모르겠어서 그냥 바인딩객체를 삭제로 바꿔준다.
            int findIndex = this.xing_t0424.getT0424VoList().Find("expcode", Isuno.Replace("A", ""));
            if (findIndex>=0)
            {
                this.xing_t0424.getT0424VoList().RemoveAt(findIndex);
            }

        }


        //조건검색 버튼 이벤트
        private void btn_search_Click_1(object sender, EventArgs e)
        {
            xing_t1833.call_request();
        }

    
        //로그 클린
        private void button2_Click(object sender, EventArgs e)
        {
            listBox_log.Items.Clear();
        }

       
        //item 수가 500개 이상이면 마지막 item을 삭제한다.
        public void insertListBoxLog(String Message)
        {
            this.listBox_log.Items.Insert(0, Message);
            if (listBox_log.Items.Count > 100)
            {
                this.listBox_log.Items.RemoveAt(100);
            }
        }


        //서버시간 호출 타이머
        private void Timer0167_Tick(object sender, EventArgs e)
        {
            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                //타이머중 접속 끊겼을경우 common timer 가 대표료 login 호출한다.
                Log.WriteLine("Timer0167_Tick:: 미접속");

            } else{
                xing_t0167.call_request();
            }
        }


        int test = 0;
        //매수금지종목 호출 타이머.
        private void timer_t1833Exclude_Tick(object sender, EventArgs e)
        {

            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                //타이머중 접속 끊겼을경우 common timer 가 대표료 login 호출한다.
                Log.WriteLine("timer_t1833Exclude_Tick:: 미접속");

            }  else{

                if (test == 0)
                {
                    xing_t1833Exclude.call_request();
                    test = 1;
                }
                else
                {
                    xing_t1833.call_request();
                    test = 0;
                }
               
            }

        }

        //공통 타이머 계좌정보 --5초마다 호출됨.
        private void timer_common_Tick(object sender, EventArgs e)
        {
            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                this.tradingStop();
                this.timerLogin.Start();
                
            }else{

                //3.주식잔고2 --잠시 주석또는 아예삭제 test후 결정내리자. --청산
                this.xing_t0424.call_request(this.account, this.accountPw);

                //4.금일주문 이력
                this.xing_t0425.call_request(this.account, this.accountPw);

                
                //1.검색조건식 호출 -신규매수및 반복매수 -  잔고가 먼저 호출되어야한다.(잘모르겠다.)
                //xing_t1833.call_request();

                //5.현물계좌예수금/주문가능금액/총평가 조회
                this.xing_CSPAQ12200.call_request(this.account, this.accountPw);

                //데이터 조회와 매수/매도 프로세스 분리하자.
            }
        }

        private void timerLogin_Tick(object sender, EventArgs e)
        {
            Log.WriteLine("로그인타이머 호출");
            
             Boolean b = exXASessionClass.fnLogin();
 
        }
    }//end class
}//end namespace

