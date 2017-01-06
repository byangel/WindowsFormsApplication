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


namespace PackageSellSystemTrading
{
    public partial class MainForm : System.Windows.Forms.Form
    {

        public MainForm()
        {
            InitializeComponent();
        }
        private ExXASessionClass exXASessionClass;
        private Xing_t1833 xing_t1833;
        private Xing_t0424 xing_t0424;
        private Xing_CSPAT00600 xing_CSPAT00600;
        private Xing_CSPAQ12300 xing_CSPAQ12300;

        public DataTable dataTable_t0424;

        // 로그인 정보
        public bool loginAt = false;

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
            formInit();

            //개발완료시 제거해주자.
            input_loginId.Text = "neloi";
            input_loginPw.Text = "neloi1"; 
            input_publicPass.Text = "";
            input_accountPw.Text = "0000";
        }

        private void formInit()
        {
            //서버 선택 콤보 초기화
            combox_targetServer.Items.Add("모의투자");
            combox_targetServer.Items.Add("실서버");
            combox_targetServer.SelectedIndex = 0;

            //종목검색 그리드 초기화
            dataTable_t0424 = new DataTable();
            grd_t0424.DataSource = dataTable_t0424;

       
            dataTable_t0424.Columns.Add("expcode"); //코드
            dataTable_t0424.Columns.Add("hname");   //종목명
            dataTable_t0424.Columns.Add("mdposqt"); //매도가능
            dataTable_t0424.Columns.Add("price"   , typeof(int));   //현재가
            dataTable_t0424.Columns.Add("appamt"  , typeof(int));  //평가금액
            dataTable_t0424.Columns.Add("dtsunik" , typeof(int)); //평가손익
            dataTable_t0424.Columns.Add("sunikrt"); //수익율
            dataTable_t0424.Columns.Add("pamt"    , typeof(int));    //평균단가
            dataTable_t0424.Columns.Add("mamt"    , typeof(int));    //매입금액
            dataTable_t0424.Columns.Add("msat"    , typeof(int));    //당일매수금액
            dataTable_t0424.Columns.Add("mpms"    , typeof(int));    //당일매수단가
            dataTable_t0424.Columns.Add("mdat"    , typeof(int));    //당일매도금액
            dataTable_t0424.Columns.Add("mpmd"    , typeof(int));    //당일매도단가
            dataTable_t0424.Columns.Add("fee"     , typeof(int));     //수수료
            dataTable_t0424.Columns.Add("tax"     , typeof(int));     //제세금
            dataTable_t0424.Columns.Add("sininter"); //신용이자

            //grd_t0424_.Columns[1].HeaderText = "코드";
            //grd_t0424_.Columns[2].HeaderText = "종목명";
            //grd_t0424_.Columns[3].HeaderText = "매도가능";
            //grd_t0424_.Columns[4].HeaderText = "현재가";
            //grd_t0424_.Columns[5].HeaderText = "평가금액";
            //grd_t0424_.Columns[6].HeaderText = "평가손익";
            //grd_t0424_.Columns[7].HeaderText = "수익율";
            //grd_t0424_.Columns[8].HeaderText = "평균단가";
            //grd_t0424_.Columns[9].HeaderText = "매입금액";
            //grd_t0424_.Columns[10].HeaderText = "당일매수금액";
            //grd_t0424_.Columns[11].HeaderText = "당일매수단가";
            //grd_t0424_.Columns[12].HeaderText = "당일매도금액";
            //grd_t0424_.Columns[13].HeaderText = "당일매도단가";
            //grd_t0424_.Columns[14].HeaderText = "수수료";
            //grd_t0424_.Columns[15].HeaderText = "제세금";
            //grd_t0424_.Columns[16].HeaderText = "신용이자";
        }


        //로그아웃 버튼 클릭 이벤트
        private void logOutBtn_Click(object sender, EventArgs e)  {
            //로그인이면
            if (this.exXASessionClass.IsConnected()) {
                this.exXASessionClass.Logout();
            }
            else {
                MessageBox.Show("접속되어있지 않습니다.");
            }
        }


        //매수 할 종목 검색
        private void searchBtn_Click(object sender, EventArgs e)  {
            xing_t1833.call_request("test");
        }


        //로그인 버튼 클릭 이벤트
        private void btn_login_click(object sender, EventArgs e)
        {
            try{
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
            }catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        //주식 잔고2
        private void btn_accountSearch_Click(object sender, EventArgs e) {
            xing_t0424.call_request();
        }


        //주문
        private void btn_buyTest_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// 현물정상주문
            /// </summary>
            /// <param name="IsuNo">종목번호</param>
            /// <param name="Quantity">수량</param>
            /// <param name="Price">가격</param>
            /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
            xing_CSPAT00600.call_request("005930", "2", "1830000", "2");
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

