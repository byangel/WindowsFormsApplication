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
        public Xing_t1833 xing_t1833;               //조건검색
        public Xing_t0424 xing_t0424;               //잔고2
        public Xing_t0424_config xing_t0424_config; //로그인시 계좌정보
        public Xing_t0425 xing_t0425;              //체결/미체결
        public Xing_t0167 xing_t0167;              //시간조회
        public Xing_CSPAT00600 xing_CSPAT00600;   //주식주문
        public Xing_CSPAT00800 xing_CSPAT00800;   //현물 취소주문
        
        
       
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
            this.xing_t0167 = new Xing_t0167();// 서버시간조회
            this.xing_t0167.mainForm = this;
            this.xing_CSPAT00600 = new Xing_CSPAT00600();// 정상주문
            this.xing_CSPAT00600.mainForm = this;
            this.xing_CSPAT00800 = new Xing_CSPAT00800();// 현물취소주문
            this.xing_CSPAT00800.mainForm = this;

            this.xing_t0424_config = new Xing_t0424_config();// 주식잔고2
            this.xing_t0424_config.mainForm = this;

            //폼 초기화
            initForm();


 
        }

        //프로그램시작시 폼 초기화
        private void initForm(){
            
            input_loginId.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_ID);
            input_loginPw.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_PW);
            input_publicPw.Text = Util.Decrypt(Properties.Settings.Default.PUBLIC_PW);

            //서버 선택 콤보 초기화
            combox_targetServer.SelectedIndex = Properties.Settings.Default.SERVER_INDEX;

            //계좌잔고 그리드 초기화
            grd_t0424.DataSource = new BindingList<T0424Vo>();
           

            //체결미체결 그리드 DataSource 설정
            grd_t0425_chegb1.DataSource = new BindingList<T0425Vo>();//체결 그리드
            grd_t0425_chegb2.DataSource = new BindingList<T0425Vo>();//미체결 그리드

            //진입검색 그리드.
            grd_t1833.DataSource = new BindingList<T1833Vo>();


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

        private void btn_logout_Click(object sender, EventArgs e)
        {
            this.exXASessionClass.DisconnectServer();
            // 로그인 버튼 활성
            this.btn_login.Enabled = true;
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
            //이미접속이되어있으면접속을끊는다
            //if (exXASessionClass.IsConnected())
            //{
                this.exXASessionClass.DisconnectServer();//무조건 끊었다가 접속
            //}
            //서버접속
            //if (exXASessionClass.IsConnected() == false){
                if (!this.exXASessionClass.ConnectServer(mServerAddress, 20001))
                {
                    MessageBox.Show(this.exXASessionClass.GetErrorMessage(this.exXASessionClass.GetLastError()));
                }
           // }

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

            setRowNumber(grd_t0424);

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
                xing_CSPAT00600.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw,"", "003060", "238", "4000", "1");
                //xing_CSPAT00600.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw, "048870", "235" , "4000", "1");//
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
            if (this.exXASessionClass.IsConnected())
            {
                if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == "")
                {
                        //MessageBox.Show("계좌 정보가 없습니다.");
                        AccountForm accountForm = new AccountForm(exXASessionClass);
                        accountForm.ShowDialog();
                        
                        //세션클래스에 있는거 그대로 가져왔다.
                        if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == "")
                        {
                            MessageBox.Show("계좌 및 계좌 비밀번호를 설정해주세요.");
                        }
                        else
                        {
                            //로그인후 프로그램 초기화.
                            // 계좌정보 조회.
                            //mainForm.xing_t0424.call_request(this.account, this.accountPw);
                            //설정 저장버튼 활성화 
                            this.btn_config_save.Enabled = true;

                            // 계좌정보 조회.
                            //xing_t0424.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
                            this.xing_t0424_config.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);

                            //날자 및 시간 타이머 시작.
                            this.timer_dateTime.Enabled = true;
                        }
                }
                else
                {
                
                    timer_enterSearch.Enabled = true;//진입검색 타이머
                    timer_accountSearch.Enabled = true;//계좌 및 미체결 검색 타이머
                    btn_start.Enabled = false;
                    btn_stop.Enabled = true;

                }
            }else
            {
                MessageBox.Show("서버접속정보가 없습니다.");
            }

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
            
            if (int.Parse(xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(xing_t0167.time.Substring(0, 4)) < 2202){
                MessageBox.Show(xing_t0167.time.Substring(0, 4));

            }    
            //조건검색
            //xing_t1833.call_request("condition.ADF");
            //xing_t1833.call_request("conSeven.ADF");

        }

        //서버 시간
        private void timer_dateTime_Tick(object sender, EventArgs e)
        {
            xing_t0167.call_request();
        }

        //타이머 계좌정보
        private void timer_accountSearch_Tick(object sender, EventArgs e)
        {   
            //주식잔고2
            xing_t0424.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
            //미체결내역
            xing_t0425.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
            //Log.WriteLine("xing_t1833:");
        }



        private void grd_t0424_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //if (e.RowIndex >= 0)
            //{
            //    string NumberingText = (e.RowIndex + 1).ToString();

            //    // 글자 사이즈 구하기.
            //    SizeF stringSize = e.Graphics.MeasureString(NumberingText, Font);

            //    // 글자에 맞춰 좌표계산. 
            //    PointF StringPoint = new PointF
            //    (
            //        Convert.ToSingle(grd_t0424.RowHeadersWidth - 3 - stringSize.Width),
            //        Convert.ToSingle(e.RowBounds.Y) + grd_t0424[0, e.RowIndex].ContentBounds.Height * 0.3f
            //    );

            //    // 문자열 그리기.
            //    e.Graphics.DrawString
            //    (
            //        NumberingText

            //        Font,
            //        Brushes.Black,
            //        StringPoint.X,
            //        StringPoint.Y
            //    );
            //}


        }

        public void setRowNumber(DataGridView dgv){
            //if (dgv.Rows.Count > 0) dgv.RowHeadersWidth = 50;
            dgv.RowHeadersWidth = 53;
            
            foreach (DataGridViewRow row in dgv.Rows) {
                row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                
            }
            //dgv.AutoResizeRowHeadersWidth(  DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void grd_t0425_chegb1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // grd_t0425_chegb1.Rows[e.RowIndex].Cells["shcode"].Style.BackColor = Color.Gray;
            //MessageBox.Show(grd_t0425_chegb1.Rows.Count.ToString()+"/"+ e.RowIndex);
            //Log.WriteLine("grd_t0425_chegb1_RowsAdded" + grd_t0425_chegb1.Rows.Count + "/" + e.RowIndex);
           
            //폴로드시 데이타소스 개개체를 new 하고 삽입하면 이벤트가 3번일어나서 에러난다.첨엔 잘됐었는데 뭐가 문제인지 모르겠다.
            //String tmpMedosu = (String)grd_t0425_chegb1.Rows[0].Cells[1].Value;//거래구분   
            //if (tmpMedosu == "매수"){
            //    grd_t0425_chegb1.Rows[0].Cells[1].Style.ForeColor = Color.Red;
            //}else{
            //    grd_t0425_chegb1.Rows[0].Cells[1].Style.ForeColor = Color.Blue;
            //}
        }
    }//end class
}//end namespace

