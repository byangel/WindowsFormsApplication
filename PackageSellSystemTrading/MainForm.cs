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
    public partial class MainForm : System.Windows.Forms.Form {

        //배팅금액exclWatchSave
        public decimal battingAmt;

        public ExXASessionClass exXASessionClass;
        public Xing_t1833 xing_t1833;        //조건검색
        public Xing_t1833Exclude xing_t1833Exclude; //매수금지종목
        public Xing_t0424 xing_t0424;        //잔고2

        public Xing_t0425 xing_t0425;        //체결/미체결
        public Xing_t0167 xing_t0167;        //시간조회
        //public Xing_CSPAT00600 xing_CSPAT00600;   //주식주문
        //public Xing_CSPAT00800 xing_CSPAT00800;   //현물 취소주문
        public Xing_CSPAQ12200 xing_CSPAQ12200;   //현물계좌예수금/주문가능금액/총평가 조회

        public AccountForm accountForm;       //계좌 선택
        public OptionForm optionForm;        //프로그램 설정 폼
        public HistoryForm historyForm;        //매매이력 폼

        public Real_SC1 real_SC1; //실시간 체결
        public Real_S3 real_S3;  //코스피 체결체결
        public Real_K3 real_K3;  //코스닥 체결체결
        public Real_jif real_jif; //장 정보

        public TradingHistory tradingHistory; //데이타로그
        public ChartData chartData;//차트데이타

        Xing_LinkToHTS xing_LinkToHTS;

        public String tradingAt;//매매 여부 Y|N
        public Boolean buyAt;
        public Boolean sellAt;

        public String account; //계좌번호
        public String accountPw;//계좌 비밀번호



        public CSPAT00600Mng CSPAT00600Mng;
        //생성자
        public MainForm(){
            InitializeComponent();
            //initForm();
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
            try { 
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
              
                this.xing_CSPAQ12200 = new Xing_CSPAQ12200(); //현물계좌예수금/주문가능금액/총평가 조회
                this.xing_CSPAQ12200.mainForm = this;

                this.tradingHistory = new TradingHistory();//매매이력정보
                this.tradingHistory.mainForm = this;
                this.chartData = new ChartData();//챠트데이타 저장
                this.chartData.mainForm = this;

                this.accountForm = new AccountForm();//계좌선택폼
                this.accountForm.mainForm = this;
                this.accountForm.exXASessionClass = exXASessionClass;
                this.optionForm = new OptionForm();
                this.optionForm.mainForm = this;
                this.historyForm = new HistoryForm();
                this.historyForm.mainForm = this;

                this.real_SC1 = new Real_SC1();    //실시간 체결
                this.real_SC1.mainForm = this;
                this.real_S3 = new Real_S3();    //코스피 실시간 체결
                this.real_S3.mainForm = this;
                this.real_K3 = new Real_K3();    //코스닥 실시간 체결
                this.real_K3.mainForm = this;
                this.real_jif = new Real_jif();    //장정보
                this.real_jif.mainForm = this;
                this.CSPAT00600Mng = new CSPAT00600Mng(this);

                this.xing_LinkToHTS = new Xing_LinkToHTS();
                //계좌잔고 그리드 초기화
                grd_t0424.DataSource = this.xing_t0424.getT0424VoList();
                grd_t0424Excl.DataSource = this.xing_t0424.getExcludeT0424VoList();//감시제외종목 바인딩

                //진입검색 그리드.바인딩
                //grd_t1833.DataSource = this.xing_t1833.getT1833VoList();
                grd_t1833_dt.DataSource = this.xing_t1833.gett1833Dt();
                //체결미체결 그리드 DataSource 설정
                grd_t0425.DataSource = this.xing_t0425.getT0425VoList(); //체결/미체결 그리드
                //챠트(스냅샷) 그리드 바인딩
                grd_chart.DataSource = this.chartData.getChartVoList();
                //이력폼 데이타 바인딩--로그인후 바인딩이 끊긴다 나중에 이유를 찾아봐야한다.
                //historyForm.grd_history.DataSource = tradingHistory.getTradingHistoryDt();

                //누적수익율 출력
                this.label_sum_dtsunik.Text = Util.GetNumberFormat(this.chartData.getSumDtsunik());

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


                ////프로그램 최초 실행시 프로퍼티 설정이 안되어잇기 때문에 초기 셋팅값을 설정해준다.
                //if (Properties.Settings.Default.CONDITION_ADF.ToString() == "")
                //{
                //    optionForm.rollBack();
                //    optionForm.btn_config_save_Click(new object(), new EventArgs());
                //}

                
            } catch (Exception ex)
            {
                Log.WriteLine("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.StackTrace);
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
            //Properties.Settings.Default.ACCOUNT      = this.exXASessionClass.account;
            //Properties.Settings.Default.ACCOUNT_PW   = this.exXASessionClass.accountPw;
            //Properties.Settings.Default.AUTO_LOGIN   = CheckAutoLogin.Checked;
            //Properties.Settings.Default.TRAY_YN      = CheckTrayYN.Checked;

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
            optionForm.init();
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
            String orderAt; //매도주문여부
            //EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)this.grd_t0424.DataSource);
            for (int i=0;i< grd_t0424.RowCount; i++)
            {

                if (this.grd_t0424.Rows[i].Cells["grd_t0424_check"].FormattedValue.ToString() == "True")
                {
                    //{
                    //선택된 종목이 이미 매도주문이 나간 상태인지 체크하는 부분이 없다.(청산 관련 매도주문에는 DataLog 를 참조하지 않고 t0424를 참조한다.

                    expcode = this.grd_t0424.Rows[i].Cells["c_expcode"].Value.ToString(); //종목코드
                    orderAt = this.grd_t0424.Rows[i].Cells["orderAt"].Value.ToString(); //매도주문여부
                    hname   = this.grd_t0424.Rows[i].Cells["c_hname"].Value.ToString();   //종목명                                                           

                    if (orderAt == "N"){

                        //주문 여부를 true로 업데이트
                        var result_t0424 = from item in this.xing_t0424.getT0424VoList()
                                           where item.expcode == expcode.Replace("A", "")
                                           select item;
                        int findIndex = this.xing_t0424.getT0424VoList().Find("expcode", expcode.Replace("A", ""));

                        //MessageBox.Show(result_t0424.Count().ToString());

                        if (findIndex >= 0)
                        {

                            //MessageBox.Show(this.xing_t0424.getT0424VoList().ElementAt(findIndex).orderAt.ToString());
                            expcode = this.xing_t0424.getT0424VoList().ElementAt(findIndex).expcode; //종목코드
                            hname   = this.xing_t0424.getT0424VoList().ElementAt(findIndex).hname;   //종목명
                            sunikrt = this.xing_t0424.getT0424VoList().ElementAt(findIndex).sunikrt2; //수익율
                            mdposqt = this.xing_t0424.getT0424VoList().ElementAt(findIndex).mdposqt; //주문가능수량수량
                            price   = this.xing_t0424.getT0424VoList().ElementAt(findIndex).price;   //현재가
                            pamt2   = this.xing_t0424.getT0424VoList().ElementAt(findIndex).pamt2;     //평균단가2

                            /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산(선택매도,손절매)</param>
                            /// <param name="upOrdno">상위매수주문번호</param>
                            /// <param name="upOrdno">상위체결금액</param>
                            /// <param name="IsuNo">종목명</param>
                            /// <param name="IsuNo">종목번호</param>
                            /// <param name="Quantity">수량</param>
                            /// <param name="Price">가격</param>
                            Xing_CSPAT00600 xing_CSPAT00600 = CSPAT00600Mng.get600();
                            xing_CSPAT00600.ordptnDetail    = "선택매도";      //상세 매매 구분.
                            xing_CSPAT00600.shcode          = expcode; //종목코드
                            xing_CSPAT00600.hname           = hname;   //종목명
                            xing_CSPAT00600.quantity        = mdposqt; //수량
                            xing_CSPAT00600.price           = price;   //가격
                            xing_CSPAT00600.divideBuySell   = "1";             // 매매구분: 1-매도, 2-매수
                            xing_CSPAT00600.upOrdno         = "";              //상위매수주문 - 금일매도매수일때만 값이 있다.
                            xing_CSPAT00600.upExecprc       = pamt2;   //상위체결금액  
                            xing_CSPAT00600.call_request();

                            this.xing_t0424.getT0424VoList().ElementAt(findIndex).orderAt = "Y";//일괄 매도시 주문여부를 true로 설정

                            Log.WriteLine("mainForm :: 선택매도" + hname + "(" + expcode + ")]  수익율:" + sunikrt + "%    |매도가능수량:" + mdposqt);

                        }

                    }
                    else
                    {
                        //이미 매도주문 실행 종목
                        MessageBox.Show(hname + "이미 매도주문이 이루어 졌습니다.");
                    }
                    //선택체크해제
                    this.grd_t0424.Rows[i].Cells["grd_t0424_check"].Value = false;

                }

            }
        }//btn_checkSell_Click END

       
        //취소
        private void button3_Click(object sender, EventArgs e)
        {
            Xing_CSPAT00800 xing_CSPAT00800 = CSPAT00600Mng.get800();
            xing_CSPAT00800.call_request(this.account, this.accountPw, "14074", "030270", "");
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
            //String 평가손익;
            //Double 손익률;
            //Double 평가금액;
            Double 현재가 = Double.Parse(price);
            //Double 매입금액;
            
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

                //매입금액 = double.Parse(평균단가2) * double.Parse(매도가능수량);
                //평가금액 = 현재가 * double.Parse(매도가능수량);
                //평가금액 = 평가금액 - (평가금액 * 0.0033);
                //평가손익 = Util.GetNumberFormat(평가금액 - 매입금액);

                //현재가 = 현재가 - (현재가 * 0.0033);
                //1.현재가가 금일매수 값보다 3%이상 올랐으면 금일 매수 수량만큼 매도한다.
                //손익률 = ((현재가 / Double.Parse(평균단가2)) * 100) - 100;

                grd_t0424.Rows[findIndex].Cells["price"].Value = Util.GetNumberFormat(현재가);
                //grd_t0424.Rows[findIndex].Cells["appamt"].Value = Util.GetNumberFormat(평가금액);
                //grd_t0424.Rows[findIndex].Cells["dtsunik"].Value = Util.GetNumberFormat(평가손익);
                //grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = Math.Round(손익률, 2).ToString();//이게뭐지? 에러가 안나네?그리드에 없는데.
                //grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = Math.Round(테스트수익률, 2).ToString();
                //grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = 실현손익;


                //재계산 손익율
                손익률2 = xing_t0424.getSunikrt2(t0424VoList.ElementAt(findIndex));
                grd_t0424.Rows[findIndex].Cells["sunikrt2"].Value = 손익률2;

                //매도테스트 - 해당 보유종목 정보를 인자로 넘겨주어 매도가능인지 테스트한다.
                //this.xing_t0424.stopProFitTargetTest(t0424VoList.ElementAt(findIndex));

                
            }
        }//priceCallBack

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

        
        int flag1833 = 0;
        //매수금지종목 호출 타이머.
        private void timer_t1833Exclude_Tick(object sender, EventArgs e)
        {

            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                //타이머중 접속 끊겼을경우 common timer 가 대표료 login 호출한다.
                Log.WriteLine("timer_t1833Exclude_Tick:: 미접속");

            }  else{

                if (flag1833 == 0)
                {
                    xing_t1833Exclude.call_request();
                    flag1833 = 1;
                }
                else
                {
                    xing_t1833.call_request();
                    flag1833 = 0;
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

                //1.주식잔고2 --잠시 주석또는 아예삭제 test후 결정내리자. --청산
                this.xing_t0424.call_request(this.account, this.accountPw);

                //2.금일주문 이력
                this.xing_t0425.call_request(this.account, this.accountPw);
                
                //3.현물계좌예수금/주문가능금액/총평가 조회
                this.xing_CSPAQ12200.call_request(this.account, this.accountPw);
                
                //4.매도/매수 카운트
                var varT0425VoList = from item in this.xing_t0425.getT0425VoList()
                                     where item.medosu == "매도" && item.cancelOrdAt != "Y"
                                     select item;
                this.label_sellCnt.Text = varT0425VoList.Count().ToString();
                varT0425VoList = from item in this.xing_t0425.getT0425VoList()
                                 where item.medosu == "매수" && item.cancelOrdAt != "Y"
                                 select item;
                this.label_buyCnt.Text = varT0425VoList.Count().ToString(); 
            }
        }

        private void timerLogin_Tick(object sender, EventArgs e)
        {
            Log.WriteLine("로그인타이머 호출");
            
             Boolean b = exXASessionClass.fnLogin();
 
        }

        private void btn_history_pop_Click(object sender, EventArgs e)
        {
            historyForm.grd_history.DataSource = tradingHistory.getTradingHistoryDt();
            historyForm.ShowDialog();
            
        }

        private void grd_chart_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            //폰트색 지정
            if (e.ColumnIndex == 1 || e.ColumnIndex == 3 || e.ColumnIndex == 4)
            {
                if (e.Value != null)
                {
                    e.Value = Util.GetNumberFormat(e.Value.ToString());
                }
            }
                if (e.ColumnIndex == 2 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9||e.ColumnIndex == 10)
            {
                if (e.Value != null)
                {

                    //if (e.Value.ToString().IndexOf("-") >= 0)
                    //{
                    //    e.CellStyle.ForeColor = Color.Blue;
                    //    e.CellStyle.SelectionForeColor = Color.Blue;
                    //}
                    //else
                    //{
                    if (e.Value != null)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        //e.CellStyle.SelectionForeColor = Color.Red;
                        e.Value = Util.GetNumberFormat(e.Value.ToString());
                    }
                    //}
                }
            }

            if (e.ColumnIndex == 6 || e.ColumnIndex == 5)
            {
                if (e.Value != null)
                {

                    //if (e.Value.ToString().IndexOf("-") >= 0)
                    //{
                    //    e.CellStyle.ForeColor = Color.Blue;
                    //    e.CellStyle.SelectionForeColor = Color.Blue;
                    //}
                    //else
                    //{
                    e.CellStyle.ForeColor = Color.Blue;
                    //e.CellStyle.SelectionForeColor = Color.Red;
                    e.Value = Util.GetNumberFormat(e.Value.ToString());

                    //}
                }
            }

          
        }
        //감시제외.
        private void btn_exclWatch_Click(object sender, EventArgs e)
        {
            String expcode; //종목코드
            String hname;   //종목명
            String orderAt; //매도주문여부
            //EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)this.grd_t0424.DataSource);
            for (int i = 0; i < grd_t0424.RowCount; i++)
            {

                if (this.grd_t0424.Rows[i].Cells["grd_t0424_check"].FormattedValue.ToString() == "True")
                {
                    //{
                    //선택된 종목이 이미 매도주문이 나간 상태인지 체크하는 부분이 없다.(청산 관련 매도주문에는 DataLog 를 참조하지 않고 t0424를 참조한다.

                    expcode = this.grd_t0424.Rows[i].Cells["c_expcode"  ].Value.ToString(); //종목코드
                    orderAt = this.grd_t0424.Rows[i].Cells["orderAt"    ].Value.ToString(); //매도주문여부
                    hname   = this.grd_t0424.Rows[i].Cells["c_hname"    ].Value.ToString(); //매도주문여부

                    //주문여부
                    if (orderAt == "N"){
                            //감시제외 상태 업데이트.
                            TradingHistoryVo tradingHistoryVo = new TradingHistoryVo();
                            tradingHistoryVo.accno = this.account;
                            tradingHistoryVo.Isuno = expcode;
                            tradingHistoryVo.exclWatchAt = "Y";
                            
                            this.tradingHistory.watchUpdate(tradingHistoryVo);

                    } else {
                        //이미 매도주문 실행 종목
                        MessageBox.Show(hname + "이미 매도주문이 이루어 졌습니다.");
                    }
                    //미리 Y로 해야 제외그리드로 바로 이동한다.
                    this.grd_t0424.Rows[i].Cells["c_exclWatchAt"].Value = 'Y';
                    //체크박스 초기화.
                    this.grd_t0424.Rows[i].Cells["grd_t0424_check"].Value = false;
                }
            }//grd_t0424 for END

            //DB -> 메모리 리스트 동기화
            this.tradingHistory.dbSync();
            //감시제외종목 동기화
            xing_t0424.exclWatchSync();
        }//btn_exclWatch_Click END

        //감시제외 대상을 감시대상으로 
        private void btn_exclWatchRollback_Click(object sender, EventArgs e)
        {
            EBindingList<T0424Vo> t0424VoList = this.xing_t0424.getT0424VoList();
            
            String expcode; //종목코드
            int findIndex;
            for (int i = 0; i < this.grd_t0424Excl.RowCount; i++)
            {

                if (this.grd_t0424Excl.Rows[i].Cells["grd_t0424_excl_check"].FormattedValue.ToString() == "True")
                {
                    expcode = this.grd_t0424Excl.Rows[i].Cells["e_expcode"].Value.ToString(); //종목코드

                    //감시제외 상태 업데이트.
                    TradingHistoryVo tradingHistoryVo = new TradingHistoryVo();
                    tradingHistoryVo.accno = this.account;
                    tradingHistoryVo.Isuno = expcode;
                    tradingHistoryVo.exclWatchAt = "N";

                    //감시여부 상태 업데이트 호출
                    this.tradingHistory.watchUpdate(tradingHistoryVo);

                    //0424그리드값을 미리 N으로 업데이트해줘야 바로 제외그리드에서 삭제가 된다.
                    findIndex = t0424VoList.Find("expcode", expcode);
                    this.grd_t0424.Rows[findIndex].Cells["c_exclWatchAt"].Value = 'N';

                }
            }
            this.tradingHistory.dbSync();

            //2.감시제외종목 그리드 동기화
            xing_t0424.exclWatchSync();
        }

        //일일 성과및 상태 저장
        public void dayCapture()
        {
            ChartVo chartVo = new ChartVo();
            chartVo.date            = DateTime.Now.ToString("yyyyMMdd");                    //날자              
            chartVo.d2Dps           = this.label_D2Dps.Text.Replace(",", "");       //예수금(D2)         
            chartVo.dpsastTotamt    = this.label_DpsastTotamt.Text.Replace(",", "");//예탁자산총액          
            chartVo.mamt            = this.label_mamt.Text.Replace(",", "");        //매입금액            
            chartVo.balEvalAmt      = this.label_BalEvalAmt.Text.Replace(",", "");  //매입평가금액          
            chartVo.pnlRat          = this.label_PnlRat.Text.Replace(",", "");      //손익율             
            chartVo.tdtsunik        = this.label_tdtsunik.Text.Replace(",", "");    //평가손익            
            chartVo.dtsunik         = this.label_dtsunik.Text.Replace(",", "");     //실현손익            
            chartVo.battingAtm      = this.label_battingAtm.Text.Replace(",", "");  //배팅금액            
            chartVo.toDaysunik      = this.label_toDaysunik.Text.Replace(",", "");  //당일매도 실현손익       
            chartVo.dtsunik2        = this.label_dtsunik2.Text.Replace(",", "");    //실현손익2           
            chartVo.investmentRatio = this.label_InvestmentRatio.Text;              //투자율             
            chartVo.itemTotalCnt    = this.h_totalCount.Text;                       //총 보유종목 수     
            chartVo.buyFilterCnt    = this.exCnt.Text;                              //매수금지종목수         
            chartVo.buyCnt          = this.label_buyCnt.Text;                       //매수횟수            
            chartVo.sellCnt         = this.label_sellCnt.Text.Replace(",", "");     //매도횟수  

            int findIndex = this.chartData.getChartVoList().Find("date", chartVo.date);
            if (findIndex >= 0) {
                this.chartData.update(chartVo);
            }else{
                this.chartData.insert(chartVo);
            }
           
            this.chartData.dbSync();
            //누적수익율 출력
            this.label_sum_dtsunik.Text = Util.GetNumberFormat(this.chartData.getSumDtsunik());
        }

        private void btn_Capture_Click(object sender, EventArgs e)
        {
            dayCapture();
        }

        //목표 설정및 손절 청산 저장
        private void btn_exclWatchSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.grd_t0424Excl.RowCount; i++)
            {
                //감시제외 상태 업데이트.
                TradingHistoryVo tradingHistoryVo = new TradingHistoryVo();
                tradingHistoryVo.accno          = this.account;
                tradingHistoryVo.Isuno          = this.grd_t0424Excl.Rows[i].Cells["e_expcode"      ].FormattedValue.ToString().Replace("A", ""); //종목코드;
                tradingHistoryVo.targClearPrc   = this.grd_t0424Excl.Rows[i].Cells["e_targClearPrc" ].FormattedValue.ToString().Replace(",", ""); //목표청산가격
                tradingHistoryVo.secEntPrc      = this.grd_t0424Excl.Rows[i].Cells["e_secEntPrc"    ].FormattedValue.ToString().Replace(",", ""); //2차진입가격  
                tradingHistoryVo.secEntAmt      = this.grd_t0424Excl.Rows[i].Cells["e_secEntAmt"    ].FormattedValue.ToString().Replace(",", ""); //2차진입비중가격  
                tradingHistoryVo.stopPrc        = this.grd_t0424Excl.Rows[i].Cells["e_stopPrc"      ].FormattedValue.ToString().Replace(",", ""); //손절가격

                //감시여부 상태 업데이트 호출
                this.tradingHistory.clearUpdate(tradingHistoryVo);
            }
            this.tradingHistory.dbSync();
            MessageBox.Show("감시가격을 저장하였습니다.");
        }

        private void btn_exclWatchSync_Click(object sender, EventArgs e)
        {
            //감시제외종목 동기화
            xing_t0424.exclWatchSync();
        }
        
        //잔고목록과 DB 동기화
        private void btn_sync_Click(object sender, EventArgs e)
        {
            //잔고 동기화.
            xing_t0424.t0424histoySync();
        }

        //감시제외 그리드 현재가 변경 이벤트
        private void grd_t0424Excl_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //MessageBox.Show(e.ColumnIndex.ToString());
                int priceIndex = grd_t0424Excl.Rows[e.RowIndex].Cells["e_price"].ColumnIndex;
                if (e.ColumnIndex == priceIndex)
                {
                    //MessageBox.Show(grd_t0424.Rows[e.RowIndex].Cells["price"].Value.ToString());
                    t0424ExclPriceChangedProcess(e.RowIndex);
                }

            }
        }
        //그리드 현재가 변경 이벤트
        private void grd_t0424_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                //MessageBox.Show(e.ColumnIndex.ToString());
                int priceIndex = grd_t0424.Rows[e.RowIndex].Cells["price"].ColumnIndex;
                if (e.ColumnIndex == priceIndex)
                {
                    //MessageBox.Show(grd_t0424.Rows[e.RowIndex].Cells["price"].Value.ToString());
                    priceChangedProcess(e.RowIndex);
                }
         
            }

        }//grd_t0424_CellValueChanged END


        //감시제외 그리드 현재가 변경 이벤트
        public void t0424ExclPriceChangedProcess(int rowIndex)
        {
            String 종목코드 = this.grd_t0424Excl.Rows[rowIndex].Cells["e_expcode"].Value.ToString().Replace("A", "");
            int findIndex = this.xing_t0424.getT0424VoList().Find("expcode", 종목코드);
            String 수익율 = this.xing_t0424.getT0424VoList().ElementAt(findIndex).sunikrt2;

            Color color = 수익율.IndexOf("-") >= 0 ? Color.Blue : Color.Red;

            grd_t0424Excl.Rows[rowIndex].Cells["e_price"].Style.ForeColor = color;
        }

        //그리드 현재가 변경 이벤트
        public void priceChangedProcess(int rowIndex)
        {
            String 종목코드 = this.grd_t0424.Rows[rowIndex].Cells["c_expcode"].Value.ToString();


            //여기서부터 감시제외
            var test = this.grd_t0424.Rows[rowIndex].Cells["c_exclWatchAt"].Value;
            String 감시제외여부 = test == null ? "" : test.ToString();


            if (감시제외여부 == "Y") {
                this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gray;
            }

            //매수금지이고 t0424에 빨간색으로 표시해주자.
            int t1833ExcludeVoListFindIndex = this.xing_t1833Exclude.getT1833ExcludeVoList().Find("shcode", 종목코드);
            if (t1833ExcludeVoListFindIndex >= 0)
            {
                this.grd_t0424.Rows[rowIndex].Cells["grd_t0424_check"].Style.BackColor = Color.Red;
            }

            //매매이력정보 호출
            SummaryVo summaryVo = this.tradingHistory.getSummaryVo(종목코드);
            if (summaryVo != null) {
                String 최대수익율 = Util.nvl(summaryVo.maxHisRt, "0");
                String 최소수익율 = Util.nvl(summaryVo.minHisRt, "0");
               
                Util.nvl(summaryVo.maxHisRt,"0");
                this.grd_t0424.Rows[rowIndex].Cells["pamt2"         ].Value = Util.GetNumberFormat(summaryVo.pamt2);    //평균단가2
                this.grd_t0424.Rows[rowIndex].Cells["sellCnt"       ].Value = summaryVo.sellCnt;  //매도 횟수.
                this.grd_t0424.Rows[rowIndex].Cells["buyCnt"        ].Value = summaryVo.buyCnt;   //매수 횟수
                this.grd_t0424.Rows[rowIndex].Cells["sellSunik"     ].Value = Util.GetNumberFormat(summaryVo.sellSunik);//중간매도손익
                this.grd_t0424.Rows[rowIndex].Cells["firstBuyDt"    ].Value = summaryVo.firstBuyDt;//최초진입일시

                this.grd_t0424.Rows[rowIndex].Cells["c_ordermtd"    ].Value = summaryVo.ordermtd;      //주문매체
                this.grd_t0424.Rows[rowIndex].Cells["c_targClearPrc"].Value = Util.GetNumberFormat(summaryVo.targClearPrc);   //목표청산가격
                this.grd_t0424.Rows[rowIndex].Cells["c_secEntPrc"   ].Value = Util.GetNumberFormat(summaryVo.secEntPrc);     //2차진입가격
                this.grd_t0424.Rows[rowIndex].Cells["c_secEntAmt"   ].Value = Util.GetNumberFormat(summaryVo.secEntAmt);     //2차진입비중가격
                this.grd_t0424.Rows[rowIndex].Cells["c_stopPrc"     ].Value = Util.GetNumberFormat(summaryVo.stopPrc);       //손절가격
                this.grd_t0424.Rows[rowIndex].Cells["c_exclWatchAt" ].Value = summaryVo.exclWatchAt;   //감시제외여부
                this.grd_t0424.Rows[rowIndex].Cells["eventNm"       ].Value = summaryVo.eventNm;   //검색조건 이름
                this.grd_t0424.Rows[rowIndex].Cells["maxHisRt"      ].Value = 최대수익율;   //최대도달 수익율
                this.grd_t0424.Rows[rowIndex].Cells["minHisRt"      ].Value = 최소수익율;   //최소도달 수익율
              
                //매도가능수량이 같지 않으면 에러표시 해주자.
                String errorcd = this.grd_t0424.Rows[rowIndex].Cells["errorcd"].Value == null ? "" : this.grd_t0424.Rows[rowIndex].Cells["errorcd"].Value.ToString();
                String c_mdposqt = this.grd_t0424.Rows[rowIndex].Cells["c_mdposqt"].Value.ToString();
                if (c_mdposqt != summaryVo.sumMdposqt)
                {
                    this.grd_t0424.Rows[rowIndex].Cells["errorcd"].Value = "mdposqt not equals";
                    this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (c_mdposqt == summaryVo.sumMdposqt)
                {
                    if (errorcd.Equals("mdposqt not equals"))//기존 다른 에러코드가 존재하면 초기화 하지 않는다.
                    {
                        this.grd_t0424.Rows[rowIndex].Cells["errorcd"].Value = "";
                        this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                //확장정보 에러일경우 에러상태를 풀어준다.
                if (errorcd != null && errorcd.Equals("notHistory")) {
                    this.grd_t0424.Rows[rowIndex].Cells["errorcd"].Value = "";
                    this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                }

                String 수익율 = this.grd_t0424.Rows[rowIndex].Cells["c_sunikrt"].Value.ToString().Replace(",", "");
                //그리드 수익에따라  폰트색 지정
                Color color = 수익율.IndexOf("-") >= 0 ? Color.Blue : Color.Red;

                grd_t0424.Rows[rowIndex].Cells["price"      ].Style.ForeColor = color;
                //grd_t0424.Rows[rowIndex].Cells["price"    ].Style.SelectionForeColor = Color.Red;
                grd_t0424.Rows[rowIndex].Cells["c_sunikrt"  ].Style.ForeColor = color;
                grd_t0424.Rows[rowIndex].Cells["sunikrt2"   ].Style.ForeColor = color;
                grd_t0424.Rows[rowIndex].Cells["dtsunik"    ].Style.ForeColor = color;
                grd_t0424.Rows[rowIndex].Cells["appamt"     ].Style.ForeColor = color;

                //제외종목 가격도 같이 수정해주자.
                int findIndex = this.xing_t0424.getExcludeT0424VoList().Find("expcode", 종목코드);
                if (findIndex >= 0) {
                    grd_t0424Excl.Rows[findIndex].Cells["e_price"].Value = Util.GetNumberFormat(grd_t0424.Rows[rowIndex].Cells["price"].Value.ToString());
                    //grd_t0424Excl.Rows[findIndex].Cells["e_price"].Style.ForeColor = color;
                }

                //수익율 최소 최대 업데이트
                String 현재수익율 = this.grd_t0424.Rows[rowIndex].Cells["sunikrt2"].Value == null ? this.grd_t0424.Rows[rowIndex].Cells["c_sunikrt"].Value.ToString(): this.grd_t0424.Rows[rowIndex].Cells["sunikrt2"].Value.ToString();
                double d현재수익 = double.Parse(현재수익율.Trim());
                double d최대수익율 = double.Parse(최대수익율.Trim());

                if (double.Parse(현재수익율.Trim()) > double.Parse(최대수익율)){
                    this.tradingHistory.maxHisRtUpdate(종목코드, 현재수익율);
                    this.grd_t0424.Rows[rowIndex].Cells["maxHisRt"].Value = 현재수익율;   //최대도달 수익율
                }
                if (double.Parse(현재수익율.Trim()) < double.Parse(최소수익율)){
                    this.tradingHistory.minHisRtUpdate(종목코드, 현재수익율);
                    this.grd_t0424.Rows[rowIndex].Cells["minHisRt"].Value = 현재수익율;   //최소도달 수익율
                }
            } else {
                //이력정보가 없으면 에러코드등록해준다.
                this.grd_t0424.Rows[rowIndex].Cells["errorcd"].Value = "notHistory";
                this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
            }
            
            //3.매매이력에 따른 손익율 재계산.
            this.grd_t0424.Rows[rowIndex].Cells["sunikrt2"].Value = this.xing_t0424.getSunikrt2(this.xing_t0424.getT0424VoList().ElementAt(rowIndex));

        }
        
        //HTS연동
        //private void grd_t0424_RowEnter(object sender, DataGridViewCellEventArgs e){
        //    try{
        //        //if (e.RowIndex >= 0){
        //        //    int columnIndex = grd_t0424.Rows[e.RowIndex].Cells["c_hname"].ColumnIndex;
        //            //if (e.ColumnIndex == columnIndex)
        //            //{
        //                var 종목코드 = this.grd_t0424.Rows[e.RowIndex].Cells["c_expcode"].Value;
        //                종목코드 = 종목코드 == null ? "" : 종목코드;
        //                bool test = this.xing_LinkToHTS.call_request(종목코드.ToString());
        //            //}
        //        //}
        //    }
        //    catch (ArgumentOutOfRangeException ex)
        //    {
        //        //Log.WriteLine("mainForm : " + ex.Message);
        //    }
        //}
        //HTS연동
        private void grd_t0424_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                int columnIndex = grd_t0424.Rows[e.RowIndex].Cells["c_hname"].ColumnIndex;
                if (e.ColumnIndex == columnIndex)
                {
                    var 종목코드 = this.grd_t0424.Rows[e.RowIndex].Cells["c_expcode"].Value;
                    종목코드 = 종목코드 == null ? "" : 종목코드;
                    bool test = this.xing_LinkToHTS.call_request(종목코드.ToString());
                }
            }
        }
        //HTS연동
        private void grd_t0425_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int columnIndex = grd_t0425.Rows[e.RowIndex].Cells["t0425_hname"].ColumnIndex;
                if (e.ColumnIndex == columnIndex)
                {
                    var 종목코드 = this.grd_t0425.Rows[e.RowIndex].Cells["expcode"].Value;
                    종목코드 = 종목코드 == null ? "" : 종목코드;
                    bool test = this.xing_LinkToHTS.call_request(종목코드.ToString());
                }
            }
        }
        //HTS연동
        //private void grd_t1833_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        if (e.RowIndex > 0)
        //        {
        //            int columnIndex = grd_t1833.Rows[e.RowIndex].Cells["hname"].ColumnIndex;
        //            if (e.ColumnIndex == columnIndex)
        //            {
        //                var 종목코드 = this.grd_t1833.Rows[e.RowIndex].Cells["shcode"].Value;
        //                종목코드 = 종목코드 == null ? "" : 종목코드;
        //                bool test = this.xing_LinkToHTS.call_request(종목코드.ToString());
        //                this.xing_LinkToHTS.RequestLinkToHTS("STOCK_CODE", 종목코드, "");
        //            }
        //        }
        //    }
        //    catch (ArgumentOutOfRangeException ex)
        //    {
        //        Log.WriteLine("mainForm : " + ex.Message);
        //    }



        //}

        //더블클릭 이력 정보

        private void grd_t0424_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int columnIndex = grd_t0424.Rows[e.RowIndex].Cells["c_hname"].ColumnIndex;
                if (e.ColumnIndex == columnIndex)
                {
                    var 종목코드 = this.grd_t0424.Rows[e.RowIndex].Cells["c_expcode"].Value;
                    종목코드 = 종목코드 == null ? "" : 종목코드;
                    historyForm.Search(종목코드.ToString());
                    historyForm.ShowDialog();
                }
            }
        }

        private void grd_t0425_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int columnIndex = grd_t0425.Rows[e.RowIndex].Cells["t0425_hname"].ColumnIndex;
                if (e.ColumnIndex == columnIndex)
                {
                    var 종목코드 = this.grd_t0425.Rows[e.RowIndex].Cells["expcode"].Value;
                    종목코드 = 종목코드 == null ? "" : 종목코드;
                    historyForm.Search(종목코드.ToString());
                    historyForm.ShowDialog();
                }
            }
        }

        //목표수익율을 리턴한다.
        public String getStopProfitTarget()
        {
            String returnVal = Properties.Settings.Default.STOP_PROFIT_TARGET;
            String 투입비율 = Util.getInputRate(this);
            String 제한비율 = Properties.Settings.Default.BUY_STOP_RATE;//투자 비중 제한
            Boolean 시간차목표수익율 = Properties.Settings.Default.TIME_PROFIT_TARGET_AT;


            //자본금이 제한비율 근처까지 투입이 된상태이면 빠른 매매 회전율을 위하여 목표수익율을 낮추어 준다.
            if (Double.Parse(투입비율) > (Double.Parse(제한비율) - 5))
            {
                returnVal = Properties.Settings.Default.STOP_PROFIT_TARGET2;
            }
            if (시간차목표수익율)
            {
                //int 나머지 = (int.Parse(mainForm.xing_t0167.minute) - ((int.Parse(mainForm.xing_t0167.minute) / 60) * 60));
                int 나머지 = int.Parse(this.xing_t0167.minute);
                //if (나머지 != 0){
                //    목표수익율 = "10";
                //}
                if (나머지 == 19 || 나머지 == 39 || 나머지 == 59)
                {

                }
                else if (int.Parse(this.xing_t0167.time.Substring(0, 4)) > 1500 && int.Parse(this.xing_t0167.time.Substring(0, 4)) < 1519)
                {    //3시 이후부터 3% 이상인것은 판매 (대부문 다음날 올라가지 않고 하락하는 관계로)
                    returnVal = "3";
                }
                else
                {
                    returnVal = "10";
                }
            }
            return returnVal;
        }

        private void btn_buyAt_Click(object sender, EventArgs e)
        {
            if (this.buyAt){
                buyAtUpate(false);
                MessageBox.Show("매수가 종료되었습니다.");
            }
            else{
                buyAtUpate(true);
                MessageBox.Show("매수가 시작되었습니다.");
            }
        }
        private void btn_sellAt_Click(object sender, EventArgs e)
        {
            if (this.sellAt){
                sellAtUpate(false);
                MessageBox.Show("매도 감시가 종료되었습니다.");
            }
            else{
                sellAtUpate(true);
                MessageBox.Show("매도 감시가 시작되었습니다.");
            }
        }
        private void buyAtUpate(Boolean flag)
        {
            if (flag){
                this.buyAt = flag;
                this.btn_buyAt.Text = "매수종료";

              
            }
            else{
                this.buyAt = flag;
                this.btn_buyAt.Text = "매수시작";
            }
        }
        private void sellAtUpate(Boolean flag)
        {
            if (flag)
            {
                this.sellAt = flag;
                this.btn_sellAt.Text = "매도종료";

                this.timer_common.Start();//계좌 및 미체결 검색 타이머
                this.Timer0167.Start();//시간검색
            }else
            {
                this.sellAt = flag;
                this.btn_sellAt.Text = "매도시작";

                this.timer_common.Stop();
            }
        }

        private void grd_t1833_dt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex > 0)
                {
                    int columnIndex = grd_t1833_dt.Rows[e.RowIndex].Cells["종목명"].ColumnIndex;
                    if (e.ColumnIndex == columnIndex)
                    {
                        var 종목코드 = this.grd_t1833_dt.Rows[e.RowIndex].Cells["종목코드"].Value;
                        종목코드 = 종목코드 == null ? "" : 종목코드;
                        this.xing_LinkToHTS.call_request(종목코드.ToString());
                        //this.xing_LinkToHTS.RequestLinkToHTS("STOCK_CODE", 종목코드, "");
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                //Log.WriteLine("mainForm : " + ex.Message);
            }
        }

        private void grd_t1833_dt_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            //MessageBox.Show("Dd");
        }

        //예탁자산 총금액이 변경이 되면 호출리된다.
        private void label_DpsastTotamt_TextChanged(object sender, EventArgs e)
        {
            double dpsastTotamt             = double.Parse(((Label)sender).Text.Replace(",", ""));//예탁자산총액
            String dpsastTotAmtMax          = Properties.Settings.Default.DPSASTTOTAMT_MAX.ToString();//최대 예탁자산 총액
            String dpsastTotamtGrowthRate   = Properties.Settings.Default.DPSASTTOTAMT_GROWTH_RATE.ToString();//예탁자산 총액 목표 증가율
            
            //1.최대 예탁자산 총금액이 메모리에 없다면 현재 예탁자산 값을 메모리에 설정한다.
            if (dpsastTotAmtMax == "")
            {
                Properties.Settings.Default.DPSASTTOTAMT_MAX = dpsastTotamt.ToString();   //예탁자산 총액 
                Properties.Settings.Default.Save();
            }
           
            //최대예탁자산 대비 수익율을 구한다.
            double growthRate = ((dpsastTotamt / double.Parse(dpsastTotAmtMax)) * 100) - 100;
            //2.최대예탁자산 총금액이 줄면 메모리에 저장
            if ((growthRate + double.Parse(dpsastTotamtGrowthRate)) <= double.Parse(dpsastTotamtGrowthRate))
            {
                Properties.Settings.Default.DPSASTTOTAMT_MAX = dpsastTotamt.ToString();   //예탁자산 총액 
                Properties.Settings.Default.Save();
            }

            //3.예탁자산 총액 증가율 사용일경우 예탁자산 증가율 달성시 매수금지종목 매도 처리
            if (Properties.Settings.Default.DPSASTTOTAMT_GROWTH_AT){
                //설정 증가율 만큼 증가 했다면 매수금지종목을 찾아서 매도 처리 해주자.
                if(growthRate >= double.Parse(dpsastTotamtGrowthRate))
                {
                    //1.매도 호출
                    foreach (T0424Vo t0424Vo in this.xing_t0424.getT0424VoList())
                    {
                        //매수금지종목
                        EBindingList<T1833Vo> t1833ExcludeVoList = this.xing_t1833Exclude.getT1833ExcludeVoList();
                        int t1833ExcludeVoListFindIndex = t1833ExcludeVoList.Find("shcode", t0424Vo.expcode);

                        //매수금지종 매도
                        if (t1833ExcludeVoListFindIndex >= 0)
                        {
                            this.xing_t0424.t0424Order(t0424Vo, "1", "GROWTH_매도");
                        }
                    }

                    //2.최대 예탁자산이 증가 하였음으로  메모리에 저장
                    Properties.Settings.Default.DPSASTTOTAMT_MAX = dpsastTotamt.ToString();   //예탁자산 총액 증가율
                    Properties.Settings.Default.Save();
                }
              
            }

            //배팅금액 화면 출력
            String battingRate = Properties.Settings.Default.BATTING_RATE.ToString();          //예탁자산 대비 진입 비중
            String maxAmtLimit = Properties.Settings.Default.MAX_AMT_LIMIT;//최대 운영 자금
            if (double.Parse(dpsastTotAmtMax) > double.Parse(maxAmtLimit))
            {
                this.label_battingAtm.Text = Util.GetNumberFormat(Util.getBattingAmt(Properties.Settings.Default.MAX_AMT_LIMIT, battingRate));
            }
            else
            {
                this.label_battingAtm.Text = Util.GetNumberFormat(Util.getBattingAmt(dpsastTotAmtMax, battingRate));
            }

            
        }
    }//end class
}//end namespace

