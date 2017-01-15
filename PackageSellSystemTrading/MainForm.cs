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


namespace PackageSellSystemTrading{
    public partial class MainForm : System.Windows.Forms.Form{

        //배팅금액
        public decimal battingAmt;

        public ExXASessionClass exXASessionClass;
        public Xing_t1833 xing_t1833;//조건검색
        public Xing_t0424 xing_t0424;//잔고2
        public Xing_t0424_config xing_t0424_config;
        public Xing_t0425 xing_t0425;//체결/미체결
        public Xing_t0167 xing_t0167;//
        public Xing_CSPAT00600 xing_CSPAT00600;//주식주문

        //table
        public DataTable dataTable_t0424;

        public MainForm(){
            InitializeComponent();
        }
       
        //1회 주문시 매입금액

        private void MainForm_Load(object sender, EventArgs e)
        {
            //접속가능
            //실운영 : hts.ebestsec.co.kr
            //모의투자 : demo.ebestsec.co.kr
            exXASessionClass = new ExXASessionClass();//로그인
            exXASessionClass.mainForm = this;

            this.xing_t1833 = new Xing_t1833();//종목검색
            this.xing_t1833.mainForm = this;
            this.xing_t0424 = new Xing_t0424();// 주식잔고2
            this.xing_t0424.mainForm = this;
            this.xing_t0425 = new Xing_t0425();// 체결/미체결
            this.xing_t0425.mainForm = this;
            this.xing_t0167 = new Xing_t0167();// 체결/미체결
            this.xing_t0167.mainForm = this;
            this.xing_CSPAT00600 = new Xing_CSPAT00600();// 정상주문
            this.xing_CSPAT00600.mainForm = this;

            this.xing_t0424_config = new Xing_t0424_config();// 주식잔고2
            this.xing_t0424_config.mainForm = this;

            //폼 초기화
            initForm();
 
        }

        //프로그램시작시 폼 초기화
        private void initForm(){
            //개발완료시 제거해주자.
            input_loginId.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_ID);
            input_loginPw.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_PW);
            input_publicPw.Text = Util.Decrypt(Properties.Settings.Default.PUBLIC_PW);

            //서버 선택 콤보 초기화
            combox_targetServer.SelectedIndex = Properties.Settings.Default.SERVER_INDEX;

            //계좌잔고 그리드 초기화
            dataTable_t0424 = new DataTable("dataTable_t0424");
            grd_t0424.DataSource = dataTable_t0424;
            dataTable_t0424.Columns.Add("expcode" , typeof(String)); //코드
            dataTable_t0424.Columns.Add("hname"   , typeof(String)); //종목명
            dataTable_t0424.Columns.Add("mdposqt" , typeof(String)); //매도가능
            dataTable_t0424.Columns.Add("price"   , typeof(int));    //현재가
            dataTable_t0424.Columns.Add("appamt"  , typeof(int));    //평가금액
            dataTable_t0424.Columns.Add("dtsunik" , typeof(int));    //평가손익
            dataTable_t0424.Columns.Add("sunikrt" , typeof(String)); //수익율
            dataTable_t0424.Columns.Add("pamt"    , typeof(int));    //평균단가
            dataTable_t0424.Columns.Add("mamt"    , typeof(int));    //매입금액
            dataTable_t0424.Columns.Add("msat"    , typeof(int));    //당일매수금액
            dataTable_t0424.Columns.Add("mpms"    , typeof(int));    //당일매수단가
            dataTable_t0424.Columns.Add("mdat"    , typeof(int));    //당일매도금액
            dataTable_t0424.Columns.Add("mpmd"    , typeof(int));    //당일매도단가
            dataTable_t0424.Columns.Add("fee"     , typeof(int));    //수수료
            dataTable_t0424.Columns.Add("tax"     , typeof(int));    //제세금
            dataTable_t0424.Columns.Add("sininter", typeof(String)); //신용이자
            dataTable_t0424.Columns.Add("deleteAt", typeof(Boolean)); //삭제여부

        }
       

        //properties 저장
        private void btn_config_save_Click(object sender, EventArgs e)  {


            // UI 필드 값을 읽어서 변수에 담음 - 비번등은 암호화 시킴
            int    serverIndex   = combox_targetServer.SelectedIndex;
            string loginId       = Util.Encrypt(input_loginId.Text.Trim());
            string loginPw       = Util.Encrypt(input_loginPw.Text.Trim());
            string publicPw      = Util.Encrypt(input_publicPw.Text.Trim());
 
        

            // 설정 파일에 저장
            Properties.Settings.Default.SERVER_INDEX = serverIndex;
            Properties.Settings.Default.LOGIN_ID     = loginId;
            Properties.Settings.Default.LOGIN_PW     = loginPw;
            Properties.Settings.Default.PUBLIC_PW    = publicPw;
            Properties.Settings.Default.ACCOUNT      = this.exXASessionClass.account;
            Properties.Settings.Default.ACCOUNT_PW   = this.exXASessionClass.accountPw;
            //Properties.Settings.Default.AUTO_LOGIN = CheckAutoLogin.Checked;
            //Properties.Settings.Default.TRAY_YN = CheckTrayYN.Checked;

            Properties.Settings.Default.Save();
            MessageBox.Show("로그인 설정을 저장했습니다..!!");

        }

        //로그아웃 버튼 클릭 이벤트
        //private void btn_loginOut_Click(object sender, EventArgs e)
        //{
        //    //로그인이면
        //    if (this.exXASessionClass.IsConnected())
        //    {
        //        this.exXASessionClass.Logout();
        //    }
        //    else
        //    {
        //        MessageBox.Show("접속되어있지 않습니다.");
        //    }
        //}


        //매수 할 종목 검색
        private void btn_search_Click(object sender, EventArgs e)
        {
            xing_t1833.call_request("Condition.ADF");
        }


        //로그인 버튼 클릭 이벤트
        private void btn_login_click(object sender, EventArgs e)
        {
           
            String mServerAddress = "";

            String loginId    = input_loginId.Text;
            String loginPass  = input_loginPw.Text;
            String publicPass = input_publicPw.Text;
            //String accountPass = input_accountPass.Text;
            if (loginId == "" && loginPass == ""){

            }

            switch (combox_targetServer.SelectedIndex){
                case 0: mServerAddress = "demo.ebestsec.co.kr"; break;
                case 1: mServerAddress = "hts.ebestsec.co.kr"; break;
            }
            //MessageBox.Show(mServerAddress);
            //서버접속
            if (exXASessionClass.IsConnected() == false){
                this.exXASessionClass.ConnectServer(mServerAddress, 20001);
            }

            // 로그인 호출
            bool loginAt = exXASessionClass.Login(loginId, loginPass, publicPass, 0, false);
           
        }

        //주식 잔고2
        private void btn_accountSearch_Click(object sender, EventArgs e) {

            if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == ""){
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
                xing_t0424.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
            }
            
        }


        //주문
        private void btn_buyTest_Click(object sender, EventArgs e)
        {
            if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == ""){
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
                /// <summary>
                /// 현물정상주문
                /// </summary>
                /// <param name="IsuNo">종목번호</param>
                /// <param name="Quantity">수량</param>
                /// <param name="Price">가격</param>
                /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>  
                xing_CSPAT00600.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw, "000030", "10", "14000", "1");
                xing_CSPAT00600.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw, "048870", "235" , "4000", "1");//
            }
 
        }


        private void button2_Click(object sender, EventArgs e)
        {

            //String 선물미체결수량 = (String)dataTable_t0424.Compute("Sum(mamt)", "expcode LIKE '0*'");
            
            //int 선물미체결수량 = Convert.ToInt32(dataTable_t0424.Compute("Sum(mamt)", "*"));

            //MessageBox.Show(선물미체결수량);
            //xing_CSPAQ12300.call_request();
        }

        
        

        //미체결내역
        private void btn_t0425_Click(object sender, EventArgs e)
        {
            xing_t0425.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
        }

        //시작버튼 클릭 이벤트
        private void btn_start_Click(object sender, EventArgs e)
        {
            timer_enterSearch.Enabled    = true;
            timer_accountSearch.Enabled = true;
            btn_start.Enabled = false;
            btn_stop.Enabled  = true;
        }
       

        //정지버튼 클릭 이벤트
        private void btn_stop_Click(object sender, EventArgs e)
        {
            timer_enterSearch.Enabled    = false;
            timer_accountSearch.Enabled = false;
            btn_stop.Enabled  = false;
            btn_start.Enabled = true;
        }

       


        //타이머 진입검색
        private void timer_enterSearch_Tick(object sender, EventArgs e){
            if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == "")
            {
                timer_enterSearch.Enabled = false;
                btn_stop.Enabled = false;//종료버튼 활성
                btn_start.Enabled = true;//시작버튼 비활성
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
         
                //조건검색
                xing_t1833.call_request("Condition.ADF");
               
            }
        }

        //서버 시간
        private void timer_dateTime_Tick(object sender, EventArgs e)
        {
            xing_t0167.call_request();
        }

        //타이머 계좌정보
        private void timer_accountSearch_Tick(object sender, EventArgs e)
        {
            if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == "")
            {
                timer_enterSearch.Enabled = false;
                btn_stop.Enabled = false;//종료버튼 활성
                btn_start.Enabled = true;//시작버튼 비활성
                MessageBox.Show("계좌 정보가 없습니다.");
            }
            else
            {

                //주식잔고2
                xing_t0424.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
                //미체결내역
                xing_t0425.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
                //Log.WriteLine("xing_t1833:");

            }
        }
    }//end class
}//end namespace

