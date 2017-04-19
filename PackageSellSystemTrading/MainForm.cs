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


namespace PackageSellSystemTrading{
    public partial class MainForm : System.Windows.Forms.Form{

        //배팅금액
        public decimal battingAmt;

        public ExXASessionClass  exXASessionClass;
        public Xing_t1833        xing_t1833;        //조건검색
        public Xing_t1833Exclude xing_t1833Exclude;  //매수금지종목
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
       
        //생성자
        public MainForm(){
            InitializeComponent();
        }
       
        //1회 주문시 매입금액

        private void MainForm_Load(object sender, EventArgs e)
        {
            //접속가능
            //실운영 : hts.ebestsec.co.kr
            //모의투자 : demo.ebestsec.co.kr
            exXASessionClass                = new ExXASessionClass();//로그인
            exXASessionClass.mainForm       = this;

            this.xing_t1833                 = new Xing_t1833();//종목검색
            this.xing_t1833.mainForm        = this;
            this.xing_t1833Exclude          = new Xing_t1833Exclude();//매수금지종목검색
            this.xing_t1833Exclude.mainForm = this;
            this.xing_t0424                 = new Xing_t0424();//주식잔고2
            this.xing_t0424.mainForm        = this;
            this.xing_t0425                 = new Xing_t0425();//체결/미체결
            this.xing_t0425.mainForm        = this;
            this.xing_t0167                 = new Xing_t0167();//서버시간조회
            this.xing_t0167.mainForm        = this;
            this.xing_CSPAT00600            = new Xing_CSPAT00600();//정상주문
            this.xing_CSPAT00600.mainForm   = this;
            this.xing_CSPAT00800            = new Xing_CSPAT00800();//현물취소주문
            this.xing_CSPAT00800.mainForm   = this;
            this.xing_CSPAQ12200            = new Xing_CSPAQ12200(); //현물계좌예수금/주문가능금액/총평가 조회
            this.xing_CSPAQ12200.mainForm   = this;

            this.accountForm                = new AccountForm();//계좌선택폼
            this.accountForm.mainForm       = this;
            this.accountForm.exXASessionClass = exXASessionClass;
            this.optionForm                 = new OptionForm();
            this.optionForm.mainForm        = this;

            this.real_SC1                   = new Real_SC1();    //실시간 체결
            this.real_SC1.mainForm          = this;
            this.real_S3 = new Real_S3();    //코스피 실시간 체결
            this.real_S3.mainForm = this;
            this.real_K3 = new Real_K3();    //코스닥 실시간 체결
            this.real_K3.mainForm = this;
            this.real_jif = new Real_jif();    //장정보
            this.real_jif.mainForm = this;
            
            this.dataLog                    = new DataLog();
            this.dataLog.mainForm           = this;

            
            //계좌잔고 그리드 초기화
            grd_t0424.DataSource = this.xing_t0424.getT0424VoList();
            //진입검색 그리드.
            grd_t1833.DataSource = this.xing_t1833.getT1833VoList(); ;
            //체결미체결 그리드 DataSource 설정
            grd_t0425_chegb1.DataSource = this.xing_t0425.getT0425VoList(); //체결/미체결 그리드

            //폼 초기화
            initForm();


            //프로그램 최초 실행시 프로퍼티 설정이 안되어잇기 때문에 초기 셋팅값을 설정해준다.
            if (Properties.Settings.Default.CONDITION_ADF.ToString() == "")
            {
                optionForm.rollBack();
                optionForm.btn_config_save_Click(new object(), new EventArgs());
            }
            
        }

        //프로그램시작시 폼 초기화
        private void initForm(){
            
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
            this.accountForm.account = null;
            this.accountForm.accountPw = null;
            // 로그인 버튼 활성
            this.btn_login.Enabled = true;
            MessageBox.Show("성공적으로 로그아웃 하였습니다.");
        }

        //서버 연결
        public Boolean login(){

            //MessageBox.Show(mServerAddress);
            //이미접속이되어있으면접속을끊는다
            //if (exXASessionClass.IsConnected())
            //{
            this.exXASessionClass.DisconnectServer();//무조건 끊었다가 접속
            //}
            //서버접속
            //if (exXASessionClass.IsConnected() == false){
            if (this.exXASessionClass.ConnectServer(combox_targetServer.Text, 20001)==false)
            {
                MessageBox.Show(this.exXASessionClass.GetErrorMessage(this.exXASessionClass.GetLastError()));
                return false;
            }
            // }

         
            String loginId = input_loginId.Text;
            String loginPass = input_loginPw.Text;
            String publicPass = input_publicPw.Text;
            //String accountPass = input_accountPass.Text;
            if (loginId == "" && loginPass == "") {
                MessageBox.Show("ID 또는 Pass 값을 참조할 수 없습니다.");
                return false;
            }else {
                // 로그인 호출
                bool loginAt = exXASessionClass.Login(loginId, loginPass, publicPass, 0, false);
            }
            
            return true;
        }


        //로그인 버튼 클릭 이벤트
        private void btn_login_click(object sender, EventArgs e)
        {
            login();
        }

        //주식 잔고2
        private void btn_accountSearch_Click(object sender, EventArgs e) {

            if (this.accountForm.account == "" || this.accountForm.accountPw == ""){
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
                xing_t0424.call_request(this.accountForm.account, this.accountForm.accountPw);
            }

            //setRowNumber(grd_t0424);

        }



        //미체결내역
        private void btn_t0425_Click(object sender, EventArgs e)
        {
            xing_t0425.call_request(this.accountForm.account, this.accountForm.accountPw);
        }

        //시작버튼 클릭 이벤트
        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.exXASessionClass.IsConnected())
                {
                    //계좌번호까지있어야 로그인으로 간주하자.
                    if (this.accountForm.account == null || this.accountForm.accountPw == null)
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
                MessageBox.Show("예외 발생:{0}", ex.Message);
            }
        }
       
        public void tradingRun()
        {
            timer_enterSearch.Start();//진입검색 타이머
            timer_accountSearch.Start();//계좌 및 미체결 검색 타이머
            btn_start.Enabled = false;
            btn_stop.Enabled = true;

            //실시간 체결정보 -트레이딩 시작시 - 트레이딩 로그인시점?
            real_SC1.AdviseRealData();
           
            Log.WriteLine("Trading Start..!!");
        }
        public void tradingStop()
        {
            timer_enterSearch.Stop();//진입검색 타이머
            timer_accountSearch.Stop();//계좌 및 미체결 검색 타이머
            btn_start.Enabled = true;
            btn_stop.Enabled = false;

            //실시간 체결정보
            real_SC1.UnadviseRealData();
           
            Log.WriteLine("Trading Stop..!!");
        }

        //정지버튼 클릭 이벤트
        private void btn_stop_Click(object sender, EventArgs e)
        {
            this.tradingStop();
        }

       


        //타이머 진입검색
        private void timer_enterSearch_Tick(object sender, EventArgs e){

            if (this.exXASessionClass.loginAt == false)
            {
                //login();
                //Log.WriteLine("timer_enterSearch_Tick:: 로그인 호출");
            }
            

            //condition2.ADF 기본 급등주 검색에서 거래량을 추가한 버전 오리지날 버전보다 보통 검색되는 종목수가 적다.
            xing_t1833.call_request();
            xing_t1833Exclude.call_request();

            //일단여기서 금일매수 건에한하여 일정시간 지나면 매도 기능을 호출한다.


        }

        //timerSell -- 일단 dataLog 부터 처리해야 이기능을 구현할 수 있을것 같다.
        public void timerSell()
        {

        }

        private void Timer0167_Tick(object sender, EventArgs e)
        {
            xing_t0167.call_request();
            //input_dateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
       

        //타이머 계좌정보
        private void timer_accountSearch_Tick(object sender, EventArgs e)
        {
            if (this.exXASessionClass.loginAt == false)
            {
                login();
                Log.WriteLine("timer_accountSearch_Tick:: 로그인 호출");
            }

            //주식잔고2 --잠시 주석또는 아예삭제 test후 결정내리자.
            //this.xing_t0424.call_request(this.accountForm.account, this.accountForm.accountPw);
            //미체결내역
            this.xing_t0425.call_request(this.accountForm.account, this.accountForm.accountPw);
            //Log.WriteLine("xing_t1833:");

            //현물계좌예수금/주문가능금액/총평가 조회
            this.xing_CSPAQ12200.call_request(this.accountForm.account, this.accountForm.accountPw);
        }

        

        //체결 그리드 row 번호 출력
        private void grd_t0425_chegb1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rect = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, grd_t0425_chegb1.RowHeadersWidth - 4, e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics
                                , (e.RowIndex + 1).ToString()
                                , grd_t0425_chegb1.RowHeadersDefaultCellStyle.Font
                                , rect
                                , grd_t0425_chegb1.RowHeadersDefaultCellStyle.ForeColor
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
            
            //EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)this.grd_t0424.DataSource);
            for (int i=0;i< grd_t0424.RowCount; i++)
            {
              
                if (grd_t0424.Rows[i].Cells[0].FormattedValue.ToString() == "True")
                {
                    
                    expcode = grd_t0424.Rows[i].Cells[1].FormattedValue.ToString(); //종목코드
                    //주문 여부를 true로 업데이트
                    var result_t0424 = from item in this.xing_t0424.getT0424VoList()
                                       where item.expcode == expcode.Replace("A", "")
                                       select item;
                    //MessageBox.Show(result_t0424.Count().ToString());
                    
                    if (result_t0424.Count() > 0)
                    {
                        expcode = result_t0424.ElementAt(0).expcode; //종목코드
                        hname   = result_t0424.ElementAt(0).hname; //종목명
                        sunikrt = result_t0424.ElementAt(0).sunikrt; //수익율
                        mdposqt = result_t0424.ElementAt(0).mdposqt; //주문가능수량수량
                        price   = result_t0424.ElementAt(0).price; //현재가

                        /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
                        /// <param name="upOrdno">상위매수주문번호-금일매도일때만 셋팅될것같다.</param>
                        /// <param name="IsuNo">종목번호</param>
                        /// <param name="Quantity">수량</param>
                        /// <param name="Price">가격</param>
                        /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                        this.xing_CSPAT00600.call_request(this.accountForm.account, this.accountForm.accountPw, "선택매도", "", expcode, mdposqt, price, "1");
                        Log.WriteLine("mainForm :: 선택매도" + hname + "(" + expcode + ")]  수익율:" + sunikrt + "%    주문수량및매도가능:" + mdposqt);
                        

                        result_t0424.ElementAt(0).orderAt = true;//일괄 매도시 주문여부를 true로 설정
                    }

                }
                
          

            }
        }

     
        private void test_Click(object sender, EventArgs e)
        {
            //this.grd_t0424.Rows[0].Cells["c_hname"].Value = "test";

            //grd_t0424.Rows[0].Cells["c_mdposqt"].Value = "test";
            //listBox1.Items.Insert(0,"dddd");

            //this.listBox_account.Items.Add(account);
            //EBindingList<T0424Vo> t0424VoList = xing_t0424.getT0424VoList();
            //int findIndex = t0424VoList.Find("expcode", "000890");
            //if (findIndex >= 0)
            //{
            //    grd_t0424.Rows.Remove(grd_t0424.Rows[findIndex]);
            //    MessageBox.Show("로우삭제");
            //    //t0424VoList.ElementAt(findIndex).price = price;
            //    //t0424VoList.ResetItem(findIndex);
            //    //Log.WriteLine("real S3 ::실시간 코스피 체결확인: 종목코드:" + shcode + "|현재가" + price);
            //}
            //MessageBox.Show(t0424VoList.Find("expcode", "000890").ToString());


            //for (int i=0;i<100;i++)
            //{
            //    //grd_t0424.Rows[0].Cells["price"].Style.BackColor = Color.Gray;
            //    //grd_t0424.Rows[0].Cells["price"].Value = i;
            //    //grd_t0424.Rows[0].Cells["price"].Style.BackColor = Color.White;
            //}
            //int findIndex = xing_t0424.getT0424VoList().Find("expcode", "002680");
            //if (findIndex >= 0)
            //{

            //    MessageBox.Show(this.grd_t0424.Rows[findIndex].Cells["c_hname"].Value.ToString());
            //    this.grd_t0424.Rows[findIndex].Cells["c_hname"].Value = "test";
            //    MessageBox.Show(xing_t0424.getT0424VoList().ElementAt(findIndex).hname);
            //    //t0424VoList.ElementAt(findIndex).price = price;
            //    //t0424VoList.ResetItem(findIndex);
            //    //Log.WriteLine("real S3 ::실시간 코스피 체결확인: 종목코드:" + shcode + "|현재가" + price);
            //}

            //real_S3.call_real("031310");
            //xing_t0424.getT0424VoList().ElementAt(0).price = "test";
            //xing_t0424.getT0424VoList().ResetItem(0);
            //거래이력 싱크
            //this.dataLog.init();


            //String expcode; //종목코드
            //String hname;   //종목명
            //String mdposqt; //주문가능수량수량
            //String sunikrt; //수익율
            //String price;   //현재가


            //expcode = "048830"; //종목코드
            //hname = "test"; //종목명
            //sunikrt = "3"; //수익율
            //mdposqt = "10"; //주문가능수량수량
            //price = "3490"; //현재가

            ///// <param name="IsuNo">종목번호</param>
            ///// <param name="Quantity">수량</param>
            ///// <param name="Price">가격</param>
            ///// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>

            //this.xing_CSPAT00600.call_request("20116440201", "1177", "dd", expcode, mdposqt, price, "1");
            ////tmpT0424Vo.orderAt = true;//일괄 매도시 주문여부를 true로 설정  

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

            if (e.ColumnIndex == 6 || e.ColumnIndex == 8)
            {
                if (e.Value != null)
                {

                    e.Value = Util.GetNumberFormat(e.Value.ToString());
                }
            }
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

        //로그인 후 작업
        public void loginAfter()
        {

        }

        //계좌 선택후 작업
        public void accountAfter()
        {
            //접속이 귾겼다가 접속했을때...문제가 있어서 추가해준다. 잔고목록을 클리어 해준다.
            xing_t0424.getT0424VoList().Clear();
            
            //잔고정보
            xing_CSPAQ12200.call_request(accountForm.account, accountForm.accountPw);
            //잔고목록
            xing_t0424.call_request(accountForm.account, accountForm.accountPw);
            //체결미체결
            xing_t0425.call_request(accountForm.account, accountForm.accountPw);
            //MessageBox.Show("계좌 정보가 정상확인 되었습니다.");

            //설정저장 버튼 활성화.
            btn_config_save.Enabled = true;

            // 로그인 버튼 비활성
            btn_login.Enabled = false;

            //매수금지종목 조회
            xing_t1833Exclude.call_request();
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
            Double 실현손익;
            Double 손익률;
            Double 평가손익;
            Double 평가금액;
            String 매입금액;
            EBindingList<T0424Vo> t0424VoList = xing_t0424.getT0424VoList();
            int findIndex = t0424VoList.Find("expcode", shcode.Replace("A", ""));
            if (findIndex >= 0)
            {

                매도가능수량  = t0424VoList.ElementAt(findIndex).mdposqt;
                수수료       = t0424VoList.ElementAt(findIndex).fee;
                세금         = t0424VoList.ElementAt(findIndex).tax;
                신용이자     = t0424VoList.ElementAt(findIndex).sininter;
                평균단가     = t0424VoList.ElementAt(findIndex).pamt;
                당일매도금액 = t0424VoList.ElementAt(findIndex).mdat; //매도수량
                당일매도단가 = t0424VoList.ElementAt(findIndex).mpmd;
                매입금액     = t0424VoList.ElementAt(findIndex).mamt;



                //평가금액: (보유수량 * 현재가) - 매도수수료(fee) - 매도제세금(tax) - 신용이자(sininter)
                평가금액 = (double.Parse(매도가능수량) * double.Parse(price)) - double.Parse(수수료) - double.Parse(세금) - double.Parse(신용이자);
                //매입금액: (매수수량 * 매수단가) + 매수수수료(fee) --매이금액은 변하지 않는 값이므로 계산하지 않고 기존데이타 활용
                //매입금액 = (double.Parse(매도가능수량) * double.Parse(평균단가)) + double.Parse(세금);
                //평가손익: 평가금액 - 매입금액
                평가손익 = 평가금액 - double.Parse(매입금액);
                //손익률: (평가손익 / 매입금액)*100
                손익률 = (평가손익 / double.Parse(매입금액)) * 100;
                //실현손익: (당일매도금액 - 매도수수료 - 매도제세금) - (매입금액 + 추정매입수수료) - 신용이자 
                실현손익 = ((double.Parse(당일매도금액) * double.Parse(당일매도단가)) - double.Parse(수수료) - double.Parse(세금)) - (double.Parse(매입금액) - double.Parse(수수료)) - double.Parse(신용이자);
                
                grd_t0424.Rows[findIndex].Cells["price"].Value = price;
                grd_t0424.Rows[findIndex].Cells["appamt"].Value = 평가금액;
                grd_t0424.Rows[findIndex].Cells["dtsunik"].Value = 평가손익;
                grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = Math.Round(손익률, 2).ToString();
                //grd_t0424.Rows[findIndex].Cells["sunikrt_"].Value = 실현손익;


                //재계산 손익율
                손익률2 = Util.getSunikrt2(t0424VoList.ElementAt(findIndex));
                grd_t0424.Rows[findIndex].Cells["sunikrt2"].Value = 손익률2;

                //매도테스트 - 해당 보유종목 정보를 인자로 넘겨주어 매도가능인지 테스트한다.
                this.stopProFitTargetTest(t0424VoList.ElementAt(findIndex));

                ///////////////////////////////////////////////////////////////////////
                //손익율2(sunikrt2) 금일매수/매도후 재계산된  평균단가를 기준으로 산출 된다.
                if (double.Parse(손익률2) > 2)
                {
                    //var resultT0424 = from item in xing_t0425.getT0425VoList()
                    //                  where item.expcode == shcode.Replace("A", "")
                    //                  select item;
                    //금일 매수된것중 2%에도달한 종목을 찾는데....매매 체결/미체결 그리드에 종목이 있다면 금일매수한것으로 간주한다. 
                    int t0425FindIndex  = xing_t0425.getT0425VoList().Find("expcode", shcode.Replace("A", ""));
                    if (t0425FindIndex >= 0)
                    {
                        if (!listBox1.Items.Contains(t0424VoList.ElementAt(findIndex).hname + "(" + shcode + ")"))
                        {
                            listBox1.Items.Insert(0, t0424VoList.ElementAt(findIndex).hname + "(" + shcode + ")");
                        }
    
                    }
                }//급등필터 end
               
            }
        }//priceCallBack

        //목표 수익율 도달 Test 후 도달여부에 따라 매도 호출
        public void stopProFitTargetTest(T0424Vo t0424Vo) {

            /////////////////////////////////////////////////////매매//////////////////////////////////////////////////////
            //거래가능여부 && 주문중상태가 아니고 && 종목거래 에러 여부
            if (t0424Vo.orderAt == false && (t0424Vo.errorcd == "" || t0424Vo.errorcd == null))
            {

                //1.매도 가능 &&  수익율 2% 이상 매도 Properties.Settings.Default.SELL_RATE
                //2.수익율2 로 변경함.
                String sunikrt = t0424Vo.sunikrt2 == null ? t0424Vo.sunikrt : t0424Vo.sunikrt2;

                if (float.Parse(sunikrt) >= float.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET))
                {
                    /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
                    /// <param name="upOrdno">상위매수주문번호-금일매도일때만 셋팅될것같다.</param>
                    /// <param name="IsuNo">종목번호</param>
                    /// <param name="Quantity">수량</param>
                    /// <param name="Price">가격</param>
                    /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                    this.xing_CSPAT00600.call_request(this.accountForm.account, this.accountForm.accountPw, "청산", "", t0424Vo.expcode, t0424Vo.mdposqt, t0424Vo.price, "1");
                    Log.WriteLine("MainForm stopProFitTargetTest ::청산[" + t0424Vo.hname + "(" + t0424Vo.expcode + ")]  수익율:" + t0424Vo.sunikrt + "%    주문수량:" + t0424Vo.mdposqt);
                    t0424Vo.orderAt = true;//일괄 매도시 주문여부를 true로 설정    

                }

                //손절
                if (Properties.Settings.Default.STOP_LOSS_AT && float.Parse(t0424Vo.sunikrt) <= float.Parse(Properties.Settings.Default.STOP_LOSS))
                {
                    /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
                    /// <param name="upOrdno">상위매수주문번호-금일매도일때만 셋팅될것같다.</param>
                    /// <param name="IsuNo">종목번호</param>
                    /// <param name="Quantity">수량</param>
                    /// <param name="Price">가격</param>
                    /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                    this.xing_CSPAT00600.call_request(this.accountForm.account, this.accountForm.accountPw, "손절", "", t0424Vo.expcode, t0424Vo.mdposqt, t0424Vo.price, "1");
                    Log.WriteLine("MainForm stopProFitTargetTest ::손절[" + t0424Vo.hname + "(" + t0424Vo.expcode + ")]  수익율:" + t0424Vo.sunikrt + "%  주문수량:" + t0424Vo.mdposqt);
                    t0424Vo.orderAt = true;//일괄 매도시 주문여부를 true로 설정
                }
            }

        }//stopProFitTarget end

        //종목을 매도하면 1.그리드에서삭제 2.dataLog삭제 해야한다. 
        public void deleteCallBack(String Isuno)
        {
            EBindingList<T0424Vo> t0424VoList = this.xing_t0424.getT0424VoList();
            int findIndex = t0424VoList.Find("expcode", Isuno.Replace("A", ""));
            //그리드삭제
            this.grd_t0424.Rows.Remove(this.grd_t0424.Rows[findIndex]);

            //dataLog 도 제거해준다.
            this.dataLog.deleteData(Isuno);

        }


        //조건검색 버튼 이벤트
        private void btn_search_Click_1(object sender, EventArgs e)
        {
            xing_t1833.call_request();
        }
    }//end class
}//end namespace

