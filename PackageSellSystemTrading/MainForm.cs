using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices.ComTypes;

using XA_DATASETLib;
using XA_SESSIONLib;


namespace PackageSellSystemTrading{
    public partial class MainForm : System.Windows.Forms.Form{

        public MainForm(){
            InitializeComponent();
        }
        public ExXASessionClass exXASessionClass;
        public Xing_t1833 xing_t1833;
        public Xing_t0424 xing_t0424;
        public Xing_CSPAT00600 xing_CSPAT00600;
        public Xing_CSPAQ12300 xing_CSPAQ12300;

        //table
        public DataTable dataTable_t0424;

        // 로그인 정보
        public bool loginAt;
        public String account   = "";
        public String accountPw = "";
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
            this.xing_CSPAT00600 = new Xing_CSPAT00600();// 정상주문
            this.xing_CSPAT00600.mainForm = this;

            this.xing_CSPAQ12300 = new Xing_CSPAQ12300();// 현물계좌 잔고내역 조회
            this.xing_CSPAQ12300.mainForm = this;


            //폼 초기화
            initForm();

            //개발완료시 제거해주자.
            input_loginId.Text = "neloi";
            input_loginPw.Text = "neloi1"; 
            input_publicPass.Text = "";
            
        }

        private void initForm()
        {
            //서버 선택 콤보 초기화
            combox_targetServer.Items.Add("모의투자");
            combox_targetServer.Items.Add("실서버");
            combox_targetServer.SelectedIndex = 0;

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

        }


        //설정저장
        private void btn_config_save_Click(object sender, EventArgs e)  {


            // UI 필드 값을 읽어서 변수에 담음 - 비번등은 암호화 시킴
            string szServerAddress = TextServerAddress.Text.Trim();
            string szLoginId = Util.Encrypt(TextLoginId.Text.Trim());
            string szLoginPw = Util.Encrypt(TextLoginPw.Text.Trim());
            string szPublicPw = Util.Encrypt(TextPublicPw.Text.Trim());
            string szAccountPw = Util.Encrypt(TextAccountPw.Text.Trim());

            // 로그인 버튼이 사용가능한 상태라면 세션 TR 변수에 값을 세팅
            if (ButtonLogin.Enabled == true)
            {
                mxSession.mServerAddress = szServerAddress;
                mxSession.mLoginId = szLoginId;
                mxSession.mLoginPw = szLoginPw;
                mxSession.mPublicPw = szPublicPw;
                mxSession.mAccountPw = szAccountPw;
            }

            // 설정 파일에 저장
            Properties.Settings.Default.SERVER_ADDRESS = szServerAddress;
            Properties.Settings.Default.LOGIN_ID = szLoginId;
            Properties.Settings.Default.LOGIN_PW = szLoginPw;
            Properties.Settings.Default.PUBLIC_PW = szPublicPw;
            Properties.Settings.Default.ACCOUNT_PW = szAccountPw;
            Properties.Settings.Default.AUTO_LOGIN = CheckAutoLogin.Checked;
            Properties.Settings.Default.TRAY_YN = CheckTrayYN.Checked;

            Properties.Settings.Default.Save();

            MessageBox.Show("로그인 설정을 저장했습니다..!!");


            //로그인이면
            if (this.exXASessionClass.IsConnected()) {
                this.exXASessionClass.Logout();
            }
            else {
                MessageBox.Show("접속되어있지 않습니다.");
            }
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
            String publicPass = input_publicPass.Text;
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

            if (this.account == "" || this.accountPw == ""){
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
                xing_t0424.call_request(this.account, this.accountPw);
            }
            
        }


        //주문
        private void btn_buyTest_Click(object sender, EventArgs e)
        {
            if (this.account == "" || this.accountPw == ""){
                MessageBox.Show("계좌 정보가 없습니다.");
            }else{
                /// <summary>
                /// 현물정상주문
                /// </summary>
                /// <param name="IsuNo">종목번호</param>
                /// <param name="Quantity">수량</param>
                /// <param name="Price">가격</param>
                /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>  
                xing_CSPAT00600.call_request(this.account, this.accountPw, "005930", "2", "1830000", "2");
            }
 
        }


        private void button2_Click(object sender, EventArgs e)
        {

            //String 선물미체결수량 = (String)dataTable_t0424.Compute("Sum(mamt)", "expcode LIKE '0*'");
            
            //int 선물미체결수량 = Convert.ToInt32(dataTable_t0424.Compute("Sum(mamt)", "*"));

            //MessageBox.Show(선물미체결수량);
            //xing_CSPAQ12300.call_request();
        }

       
    }//end class
}//end namespace

