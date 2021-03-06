﻿using System;
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
       
        public Xing_t1857Buy  xing_t1857Buy;  //매수검색
        public Xing_t1857Sell xing_t1857Sell; //매도검색
        public Xing_t1857Add  xing_t1857Add;  //조건검색
        public Xing_t1857Stop xing_t1857Stop;  //손절

        public Xing_t0424 xing_t0424;        //잔고2
        public Xing_t0425 xing_t0425;        //체결/미체결
        public Xing_t0167 xing_t0167;        //시간조회

        public Xing_CSPAQ12200 xing_CSPAQ12200; //현물계좌예수금/주문가능금액/총평가 조회
        public Xing_t8436 xing_t8436;           //상장된 전종목 조회 로그인시 최초 1회 실행
        public AccountForm accountForm;         //계좌 선택
        public OptionForm  optionForm;          //프로그램 설정 폼
        public HistoryForm historyForm;         //매매이력 폼

        public Real_SC1 real_SC1; //실시간 체결
        public Real_S3  real_S3;  //코스피 실시가 종목 현재가
        public Real_K3  real_K3;  //코스닥 실시가 종목 현재가
        public Real_jif real_jif; //장 정보
        public Real_IJ real_IJ; //지수 정보
        public TradingHistory tradingHistory; //데이타로그
        public ChartData chartData;//차트데이타

        Xing_LinkToHTS xing_LinkToHTS;
        
        //public Boolean buyAt;
        //public Boolean sellAt;

        public String account; //계좌번호
        public String accountPw;//계좌 비밀번호

        public DataTable tradingInfoDt = new DataTable();

        public CSPAT00600Mng CSPAT00600Mng;
        //생성자
        public MainForm(){
            InitializeComponent();
            //initForm();
        }
      

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
                
                this.xing_t1857Buy = new Xing_t1857Buy();//매수검색
                this.xing_t1857Buy.mainForm = this;
                
                this.xing_t1857Sell = new Xing_t1857Sell();//매도검색
                this.xing_t1857Sell.mainForm = this;

                this.xing_t1857Add = new Xing_t1857Add();//추가매수검색
                this.xing_t1857Add.mainForm = this;

                this.xing_t1857Stop = new Xing_t1857Stop();//손절
                this.xing_t1857Stop.mainForm = this;
                
                this.xing_t0424 = new Xing_t0424();//주식잔고2
                this.xing_t0424.mainForm = this;

                this.xing_t0425 = new Xing_t0425();//체결/미체결
                this.xing_t0425.mainForm = this;

                this.xing_t0167 = new Xing_t0167();//서버시간조회
                this.xing_t0167.mainForm = this;

                this.xing_CSPAQ12200 = new Xing_CSPAQ12200(); //현물계좌예수금/주문가능금액/총평가 조회
                this.xing_CSPAQ12200.mainForm = this;

                this.xing_t8436 = new Xing_t8436();//상장된 전종목 조회 로그인시 최초 1회 실행
                this.xing_t8436.mainForm = this;

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

                this.real_IJ = new Real_IJ(); //지수정보
                this.real_IJ.mainForm = this;

                this.xing_LinkToHTS = new Xing_LinkToHTS();
                //계좌잔고 그리드 초기화
                grd_t0424.DataSource = this.xing_t0424.getT0424VoList();
                grd_t0424Excl.DataSource = this.xing_t0424.getExcludeT0424VoList();//감시제외종목 바인딩

                //진입검색 그리드.바인딩
                grd_t1833_dt.DataSource = this.xing_t1857Buy.getSearchListDt();
                //체결미체결 그리드 DataSource 설정
                grd_t0425.DataSource = this.xing_t0425.getT0425VoList(); //체결/미체결 그리드
                //챠트(스냅샷) 그리드 바인딩
                grd_chart.DataSource = this.chartData.getChartVoList();
                //이력폼 데이타 바인딩--로그인후 바인딩이 끊긴다 나중에 이유를 찾아봐야한다.
                //historyForm.grd_history.DataSource = tradingHistory.getTradingHistoryDt();
                //트레이딩 정보 그리드 바인딩
                grid_traidingInfo.DataSource = this.tradingInfoDt;

                

                if (Properties.Settings.Default.LOGIN_ID != "")
                {
                    input_loginId.Text = Util.Decrypt(Properties.Settings.Default.LOGIN_ID);
                    input_loginPw.Text = Util.Decrypt(Properties.Settings.Default.LOGIN_PW);
                    input_publicPw.Text = Util.Decrypt(Properties.Settings.Default.PUBLIC_PW);
                }
                

                //서버 선택 콤보 초기화
                //combox_condition.SelectedIndex = 0;
                for (int i = 0; i < combox_targetServer.Items.Count; i++)
                {
                    if (combox_targetServer.Items[i].ToString() == Properties.Settings.Default.SERVER_ADDRESS)
                    {
                        combox_targetServer.SelectedIndex = i;
                    }
                }

                //this.tradingInfoDt = new DataTable();
                tradingInfoDt.Columns.Add("날자"            , typeof(String));
                tradingInfoDt.Columns.Add("예수금"          , typeof(double));
                tradingInfoDt.Columns.Add("예수금(D2)"      , typeof(double));
                tradingInfoDt.Columns.Add("예탁자산총액"    , typeof(double));
                tradingInfoDt.Columns.Add("매입금액"        , typeof(double));//매입금액합
                tradingInfoDt.Columns.Add("평가금액"        , typeof(double));//매입평가금액합
                tradingInfoDt.Columns.Add("수익률"          , typeof(double));//손익율합
                tradingInfoDt.Columns.Add("평가손익"        , typeof(double));//손익금합
                tradingInfoDt.Columns.Add("실현손익"        , typeof(double));
                tradingInfoDt.Columns.Add("투자율"          , typeof(double));
                tradingInfoDt.Columns.Add("누적수익"        , typeof(double));

                DataRow tmpRow = tradingInfoDt.NewRow();
                tradingInfoDt.Rows.Add(tmpRow);

                //누적수익율 출력
                tmpRow["누적수익"] = Util.GetNumberFormat(this.chartData.getSumDtsunik());


                //장상태 정보

                this.real_jif.init_jstatus();
                this.log(this.real_jif.mlabel);
            } catch (Exception ex)
            {
                this.log("mainForm.initForm : " + ex.Message);
                this.log("mainForm.initForm : " + ex.StackTrace);
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
                if (((Button)sender).Text == "시작")
                {
                    this.tradingStop();
                } else {
                    if (this.exXASessionClass.IsConnected()) {
                        //계좌번호까지있어야 로그인으로 간주하자.
                        if (this.account == null || this.accountPw == null)
                        {
                            MessageBox.Show("로그인 후 서비스 이용가능합니다.");
                        } else {
                            this.tradingStart();
                        }
                    } else {
                        MessageBox.Show("서버접속정보가 없습니다.");
                    }
                }
                
            }
            catch (Exception ex)
            {
                this.log("mainForm : " + ex.Message);
                this.log("mainForm : " + ex.StackTrace);
            }
        }
       
        //자동매매 시작
        public void tradingStart()
        {
            //타이머 시작 --여기서 타이머 시작해주면 타이머 스톱해줄일은 없어진다.그리고  잔고정보,잔고목록,매매이력 등등을 호출안해줘도 된다.
            this.timer_t1857.Start();//진입검색 타이머
            this.timer_common.Start();//계좌 및 미체결 검색 타이머
            //this.Timer0167.Start();//시간검색

            this.btn_start.Text = "시작";
            this.log("<Trading Start> ");
        }

        //자동매매 정지
        public void tradingStop()
        {
            this.timer_t1857.Stop();//진입검색 타이머
            this.timer_common.Stop();//계좌 및 미체결 검색 타이머
            //this.Timer0167.Stop();//시간검색

            this.btn_start.Text = "정지";
            this.log("<Trading Stop> ");
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
           

            Double 평균단가; //매도주문일경우 매수금액을 알기위해 넣어준다.
            String 종목코드;
            String 종목명;
            Double 현재가;
            String 매수전략명;
            Double 수량;
            Double 수익율;
            //String 상세매매구분 = null;
            //String 매매구분 = null;
            String 매도주문여부; //매도주문여부

            //EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)this.grd_t0424.DataSource);
            for (int i=0;i< grd_t0424.RowCount; i++)
            {
                if (this.grd_t0424.Rows[i].Cells["grd_t0424_check"].FormattedValue.ToString() == "True")
                {
                    T0424Vo t0424Vo = (T0424Vo)this.grd_t0424.Rows[i].DataBoundItem;
                    평균단가    = t0424Vo.pamt; //매도주문일경우 매수금액을 알기위해 넣어준다.
                    종목코드    = t0424Vo.expcode;
                    종목명      = t0424Vo.hname;
                    현재가      = t0424Vo.price;
                    매수전략명  = t0424Vo.searchNm;
                    수량        = t0424Vo.mdposqt;
                    매도주문여부= t0424Vo.orderAt;
                    수익율      = t0424Vo.sunikrt;
                    
                    if (매도주문여부 == "N"){
                            Xing_CSPAT00600 xing_CSPAT00600 = this.CSPAT00600Mng.get600();
                            xing_CSPAT00600.call_request(종목명, 종목코드, "매도", 수량, 현재가, 매수전략명, 평균단가, "선택매도");
                            t0424Vo.orderAt = "Y";//일괄 매도시 주문여부를 true로 설정
                            
                            this.log("<main:선택매도> " + 종목명 + " " + 수익율 + "% " + 수량 + "주 " + 현재가 + "원<" + 매수전략명 + ">");
                    }else{
                        //이미 매도주문 실행 종목
                        MessageBox.Show(종목명 + "이미 매도주문이 이루어 졌습니다.");
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

       

        //로그 클린
        private void button2_Click(object sender, EventArgs e)
        {
            listBox_log.Items.Clear();
        }

       
        //item 수가 500개 이상이면 마지막 item을 삭제한다.
        public void insertListBoxLog(String Message)
        {
            //this.listBox_log.Items.Insert(0, Message);


            if (listBox_log.InvokeRequired)
            {
                listBox_log.BeginInvoke(new MethodInvoker(delegate ()
                {
                    listBox_log.Items.Add(Message);
                }));
            }
            else
            {
                listBox_log.Items.Add(Message);
            }


            if (listBox_log.Items.Count > 500)
            {
                this.listBox_log.Items.RemoveAt(0);
            }
        }


        //서버시간 호출 타이머
        private void Timer0167_Tick(object sender, EventArgs e)
        {

            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                this.tradingStop();
                this.timerLogin.Start();

            } else{
                xing_t0167.call_request();
            }
        }
        
        

        //공통 타이머 계좌정보 --5초마다 호출됨.
        private void timer_common_Tick(object sender, EventArgs e)
        {
            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                //this.tradingStop();
                //this.timerLogin.Start();
                
            }else{

                //1.주식잔고2 --잠시 주석또는 아예삭제 test후 결정내리자. --청산
                this.xing_t0424.call_request(this.account, this.accountPw);

                //2.금일주문 이력
                this.xing_t0425.call_request(this.account, this.accountPw);
                
                //3.현물계좌예수금/주문가능금액/총평가 조회
                this.xing_CSPAQ12200.call_request(this.account, this.accountPw);
                
                //4.매도/매수 카운트
                var varT0425VoList = from item in this.xing_t0425.getT0425VoList()
                                     where item.medosu == "매도"  && item.cheqty > 0
                                     select item;
                this.label_sellCnt.Text = varT0425VoList.Count().ToString();
                varT0425VoList = from item in this.xing_t0425.getT0425VoList()
                                 where item.medosu == "매수" && item.cheqty > 0
                                 select item;
                this.label_buyCnt.Text = varT0425VoList.Count().ToString(); 
            }
        }

        private void timerLogin_Tick(object sender, EventArgs e)
        {
            this.log("로그인타이머 호출");
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
            if (e.ColumnIndex == 2 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9||e.ColumnIndex == 10) {
                if (e.Value != null)
                {

                    
                    if (e.Value != null)
                    {
                        e.CellStyle.ForeColor = Color.Red;
                        //e.CellStyle.SelectionForeColor = Color.Red;
                        e.Value = Util.GetNumberFormat(e.Value.ToString());
                    }
     
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
            DataRow tmpRow = tradingInfoDt.Rows[0];
  
            ChartVo chartVo = new ChartVo();
            chartVo.date            = DateTime.Now.ToString("yyyy-MM-dd");                    //날자              
            chartVo.d2Dps           = Double.Parse(tmpRow["예수금(D2)"].ToString());       //예수금(D2)         
            chartVo.dpsastTotamt    = Double.Parse(tmpRow["예탁자산총액"].ToString());//예탁자산총액          
            chartVo.mamt            = Double.Parse(tmpRow["매입금액"].ToString());        //매입금액            
            chartVo.balEvalAmt      = Double.Parse(tmpRow["평가금액"].ToString());  //매입평가금액          
            chartVo.pnlRat          = Double.Parse(tmpRow["수익률"].ToString());      //손익율             
            chartVo.tdtsunik        = Double.Parse(tmpRow["평가손익"].ToString());    //평가손익            
            chartVo.dtsunik         = Double.Parse(tmpRow["실현손익"].ToString());     //실현손익            
            chartVo.battingAtm      = double.Parse(Properties.Settings.Default.BUY_BATTING_AMT)*10000;  //배팅금액            
            //chartVo.toDaysunik      = Double.Parse(this.label_toDaysunik.Text.Replace(",", ""));  //당일매도 실현손익       
            //chartVo.dtsunik2        = Double.Parse(this.label_dtsunik2.Text.Replace(",", ""));    //실현손익2           
            chartVo.investmentRatio = Double.Parse(tmpRow["투자율"].ToString());              //투자율             
            chartVo.itemTotalCnt    = Double.Parse(this.h_totalCount.Text);                       //총 보유종목 수     
            //chartVo.buyFilterCnt    = Double.Parse(this.exCnt.Text);                              //매수금지종목수         
            chartVo.buyCnt          = Double.Parse(this.label_buyCnt.Text);                       //매수횟수            
            chartVo.sellCnt         = Double.Parse(this.label_sellCnt.Text.Replace(",", ""));     //매도횟수  

            int findIndex = this.chartData.getChartVoList().Find("date", chartVo.date);
            if (findIndex >= 0) {
                this.chartData.update(chartVo);
            }else{
                this.chartData.insert(chartVo);
            }
           
            this.chartData.dbSync();
            //누적수익율 출력
            //this.tradingInfoDt.Rows[0]["누적수익금"] = Util.GetNumberFormat(this.chartData.getSumDtsunik());
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
                tradingHistoryVo.accno = this.account;
                tradingHistoryVo.Isuno = this.grd_t0424Excl.Rows[i].Cells["e_expcode"].FormattedValue.ToString().Replace("A", ""); //종목코드;
                tradingHistoryVo.targClearPrc = this.grd_t0424Excl.Rows[i].Cells["e_targClearPrc"].FormattedValue.ToString().Replace(",", ""); //목표청산가격
                tradingHistoryVo.secEntPrc = this.grd_t0424Excl.Rows[i].Cells["e_secEntPrc"].FormattedValue.ToString().Replace(",", ""); //2차진입가격  
                tradingHistoryVo.secEntAmt = this.grd_t0424Excl.Rows[i].Cells["e_secEntAmt"].FormattedValue.ToString().Replace(",", ""); //2차진입비중가격  
                tradingHistoryVo.stopPrc = this.grd_t0424Excl.Rows[i].Cells["e_stopPrc"].FormattedValue.ToString().Replace(",", ""); //손절가격

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
            try {
                if(this.xing_t0424!=null) {

                    //if (!this.xing_t0424.initAt)
                    //{
                        if (e.RowIndex >= 0)
                        {
                            //MessageBox.Show(e.ColumnIndex.ToString());
                            int priceIndex = grd_t0424.Rows[e.RowIndex].Cells["price"].ColumnIndex;
                            if (e.ColumnIndex == priceIndex)
                            {
                                //MessageBox.Show(grd_t0424.Rows[e.RowIndex].Cells["price"].Value.ToString());
                                //현재 가격이 변경되면 
                                EBindingList<T0424Vo> t0424VoList = this.xing_t0424.getT0424VoList();
                                String 종목명 = t0424VoList.ElementAt(e.RowIndex).hname;
                                Double 현재가 = t0424VoList.ElementAt(e.RowIndex).price;
                                Double 평균단가 = t0424VoList.ElementAt(e.RowIndex).pamt;
                                Double 매도가능 = t0424VoList.ElementAt(e.RowIndex).mdposqt;
                                Double 매입금액 = t0424VoList.ElementAt(e.RowIndex).mamt;

                                Double 평가금액 = 현재가   * 매도가능;
                                Double 평가손익 = 평가금액 - 매입금액;
                                Double 수익율 = this.xing_t0424.getSunikrt(평가금액, 매입금액);
                                String 종목코드 = this.grd_t0424.Rows[e.RowIndex].Cells["c_expcode"].Value.ToString();
                                if (종목명 == "피엔티")
                                {
                                    String tessst = "3";
                                }

                                //현재가 * 매도가능 = 평가금액     현재가, 평균단가 = 수익율   매입금액 - 평가금액 = 손익금
                                this.grd_t0424.Rows[e.RowIndex].Cells["appamt"   ].Value = 평가금액;
                                this.grd_t0424.Rows[e.RowIndex].Cells["c_sunikrt"].Value = 수익율;
                                this.grd_t0424.Rows[e.RowIndex].Cells["dtsunik"  ].Value = 평가손익;

                                //트레이딩 정보 업데이트
                                this.tradingInfoUpdate();

                                //매매 프로세스 호출 --그리드색 지정 및 확정 요약정보 호출
                                priceChangedProcess(e.RowIndex);
                            }

                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                this.log("t0424 : " + ex.Message);
                this.log("t0424 : " + ex.StackTrace);
            }

        }//grd_t0424_CellValueChanged END

        public void tradingInfoUpdate()
        {
            //MessageBox.Show(grd_t0424.Rows[e.RowIndex].Cells["price"].Value.ToString());
            //현재 가격이 변경되면 
            EBindingList<T0424Vo> t0424VoList = this.xing_t0424.getT0424VoList();
            
            Double 예수금          = Double.Parse(xing_CSPAQ12200.Dps);
            Double 예수금D2        = Double.Parse(xing_CSPAQ12200.D2Dps);
            Double 실현손익        = xing_t0424.dtsunik;
            Double 종목수          = t0424VoList.Count();
            Double 매입금액        = t0424VoList.Select(item => item.mamt).Sum();
            Double 평가금액        = t0424VoList.Select(item => item.appamt).Sum();
            Double 평가손익        = 평가금액 - 매입금액;
            Double 수익률          = this.xing_t0424.getSunikrt(평가금액, 매입금액);
            Double 예탁자산총액    = 평가금액 + 예수금D2;

            Double 누적수익        = this.chartData.getSumDtsunik() + 실현손익;
            Double 투자율          = Util.getInputRate(매입금액, 예수금D2);
           
            DataRow tmpRow = tradingInfoDt.Rows[0];
            //tradingInfoDt.Columns.Add("날자", typeof(string));
            tmpRow["예수금"        ] = 예수금;
            tmpRow["예수금(D2)"    ] = 예수금D2;
            tmpRow["예탁자산총액"  ] = 예탁자산총액;
            tmpRow["매입금액"      ] = 매입금액;
            tmpRow["평가금액"      ] = 평가금액;
            tmpRow["수익률"        ] = 수익률;
            tmpRow["평가손익"      ] = 평가손익;
            tmpRow["실현손익"      ] = 실현손익;
            tmpRow["투자율"        ] = 투자율;
            tmpRow["누적수익"      ] = 누적수익;

            
        }
        //감시제외 그리드 현재가 변경 이벤트 - 그리드색 지정 및 확정 요약정보 호출
        public void t0424ExclPriceChangedProcess(int rowIndex)
        {
            String 종목코드 = this.grd_t0424Excl.Rows[rowIndex].Cells["e_expcode"].Value.ToString().Replace("A", "");
            int findIndex = this.xing_t0424.getT0424VoList().Find("expcode", 종목코드);
            Double 수익율 = this.xing_t0424.getT0424VoList().ElementAt(findIndex).sunikrt;

            Color color = 수익율 <  0 ? Color.Blue : Color.Red;

            grd_t0424Excl.Rows[rowIndex].Cells["e_price"].Style.ForeColor = color;
        }

        //그리드 현재가 변경 이벤트
        public void priceChangedProcess(int rowIndex)
        {
            T0424Vo t0424Vo     = (T0424Vo)this.grd_t0424.Rows[rowIndex].DataBoundItem;
            String 종목코드     = t0424Vo.expcode;
            Double 수익율       = t0424Vo.sunikrt; 
            String 에러코드     = t0424Vo.errorcd == null ? "" : t0424Vo.errorcd;
            Double 매도가능수량 = t0424Vo.mdposqt;

            String 최대수익율 = t0424Vo.maxRt == null ? "0" : t0424Vo.maxRt; 
            String 최소수익율 = t0424Vo.minRt == null ? "0" : t0424Vo.minRt;
            //매매이력정보 호출
            //if (종목코드=="081660")
            //{
            //    String ddd = "dd";
            //}
            //SummaryVo summaryVo = this.tradingHistory.getSummaryVo(종목코드);
            //if (summaryVo != null) {
            //    String 최대수익율 = Util.nvl(summaryVo.maxRt, "0");
            //    String 최소수익율 = Util.nvl(summaryVo.minRt, "0");

            //    this.grd_t0424.Rows[rowIndex].Cells["sellCnt"       ].Value = summaryVo.sellCnt;    //매도 횟수.
            //    this.grd_t0424.Rows[rowIndex].Cells["buyCnt"        ].Value = summaryVo.buyCnt;     //매수 횟수
            //    this.grd_t0424.Rows[rowIndex].Cells["firstBuyDt"    ].Value = summaryVo.firstBuyDt; //최초진입일시

            //    this.grd_t0424.Rows[rowIndex].Cells["sumMdposqt"    ].Value = summaryVo.sumMdposqt; //매도가능이력
            //    this.grd_t0424.Rows[rowIndex].Cells["c_ordermtd"    ].Value = summaryVo.ordermtd;    //주문매체
            //    this.grd_t0424.Rows[rowIndex].Cells["c_exclWatchAt" ].Value = summaryVo.exclWatchAt; //감시제외여부
            //    this.grd_t0424.Rows[rowIndex].Cells["searchNm"      ].Value = summaryVo.searchNm;    //검색조건 이름
            //    this.grd_t0424.Rows[rowIndex].Cells["maxRt"         ].Value = 최대수익율;            //최대도달 수익율
            //    this.grd_t0424.Rows[rowIndex].Cells["minRt"         ].Value = 최소수익율;            //최소도달 수익율

            //    //매도가능수량이 같지 않으면 에러표시 해주자.

            //    //String c_mdposqt = this.grd_t0424.Rows[rowIndex].Cells["c_mdposqt"].Value.ToString();
            //    if (매도가능수량.ToString() != summaryVo.sumMdposqt)
            //    {
            //        t0424Vo.errorcd = "mdposqt not equals";
            //        this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
            //    }
            //    else if (매도가능수량.ToString() == summaryVo.sumMdposqt)
            //    {
            //        if (errorcd.Equals("mdposqt not equals"))//기존 다른 에러코드가 존재하면 초기화 하지 않는다.
            //        {
            //            t0424Vo.errorcd = "";
            //            this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            //        }
            //    }
            //    //확장정보 에러일경우 에러상태를 풀어준다.
            //    if (errorcd != null && errorcd.Equals("notHistory")) {
            //        t0424Vo.errorcd = "";
            //        this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            //    }



            //String c_mdposqt = this.grd_t0424.Rows[rowIndex].Cells["c_mdposqt"].Value.ToString();
            //if (매도가능수량.ToString() != t0424Vo.sumMdposqt)
            //{
            //    //t0424Vo.errorcd = "mdposqt not equals";
            //    this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
            //}
            //else if (매도가능수량.ToString() == t0424Vo.sumMdposqt)
            //{
            //    if (t0424Vo.errorcd.Equals("mdposqt not equals"))//기존 다른 에러코드가 존재하면 초기화 하지 않는다.
            //    {
            //        //t0424Vo.errorcd = "";
            //        this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            //    }
            //}
            ////확장정보 에러일경우 에러상태를 풀어준다.
            //if (t0424Vo.errorcd != null && t0424Vo.errorcd.Equals("notHistory"))
            //{
            //    //t0424Vo.errorcd = "";
            //    this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            //}
            if (t0424Vo.errorcd == null || t0424Vo.errorcd == ""){
                this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            }
            else
            {
                this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
            }





            //그리드 수익에따라  폰트색 지정
            Color color = 수익율 < 0 ? Color.Blue : Color.Red;

                grd_t0424.Rows[rowIndex].Cells["price"      ].Style.ForeColor = color; //현재가
                grd_t0424.Rows[rowIndex].Cells["c_sunikrt"  ].Style.ForeColor = color; //손익율
                grd_t0424.Rows[rowIndex].Cells["dtsunik"    ].Style.ForeColor = color; //평가손익금
                grd_t0424.Rows[rowIndex].Cells["appamt"     ].Style.ForeColor = color; //평가금액

                //제외종목 가격도 같이 수정해주자.
                int findIndex = this.xing_t0424.getExcludeT0424VoList().Find("expcode", 종목코드);
                if (findIndex >= 0) {
                    grd_t0424Excl.Rows[findIndex].Cells["e_price"].Value = grd_t0424.Rows[rowIndex].Cells["price"].Value;
                }

                //수익율 최소 최대 업데이트
                //String 현재수익율 = this.grd_t0424.Rows[rowIndex].Cells["c_sunikrt"].Value.ToString();
                //double d현재수익 = double.Parse(현재수익율.Trim());
                //double d최대수익율 = double.Parse(최대수익율.Trim());

                if (최대수익율 == "∞")
                {
                    this.tradingHistory.maxHisRtUpdate(종목코드, 수익율.ToString());
                    this.log("∞ mainForm line 1020");
                }

                if (수익율 > double.Parse(최대수익율))
                {
                    
                    this.tradingHistory.maxHisRtUpdate(종목코드, 수익율.ToString());
                    this.grd_t0424.Rows[rowIndex].Cells["maxRt"].Value = 수익율;   //최대도달 수익율
                }
                if (수익율 < double.Parse(최소수익율))
                {
                    this.tradingHistory.minHisRtUpdate(종목코드, 수익율.ToString());
                    this.grd_t0424.Rows[rowIndex].Cells["minRt"].Value = 수익율;   //최소도달 수익율
                }
            //} else {
            //    //이력정보가 없으면 에러코드등록해준다.
            //    t0424Vo.errorcd = "notHistory";
            //    this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
            //}
            
        }
        
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
        
        //검색종목 셀 클릭시 hts 와연동
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
                this.log("mainForm : " + ex.Message);
            }
        }

        int searchCnt = 0;
        //매수금지종목 호출 타이머.
        private void timer_t1857_Tick(object sender, EventArgs e)
        {
            if (this.exXASessionClass.loginAt == false || exXASessionClass.IsConnected() != true)
            {
                this.log("timer_t1857Exclude_Tick:: 미접속");
            }  else {
                //매수여부를 확인후 조건검색 실행여부를 판단한다.
                if (this.cbx_sell_at.Checked)
                {
                    if (searchCnt == 0)
                    {
                        //매도검색 호출
                        xing_t1857Sell.call_index(0);
                        searchCnt = 1;
                    }
                    else if (searchCnt == 1)
                    {   //매수검색 호출
                        xing_t1857Buy.call_index(0);
                        searchCnt = 2;
                    }
                    else if (searchCnt == 2)
                    {   //매수검색 호출
                        xing_t1857Stop.call_index();
                        searchCnt = 3;
                    }
                    else
                    {   //추가매수 호출
                        xing_t1857Add.call_index();
                        searchCnt = 0;
                    }
                }
                    
            }
        }

        public void log(String msg)
        {
            Log.WriteLine(msg);
            this.insertListBoxLog("<" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + ">" + msg);
        }

        
    }//end class
    }//end namespace

