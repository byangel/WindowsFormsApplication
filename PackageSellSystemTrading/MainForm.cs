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
        public Boolean marketAt = true;

        public ExXASessionClass exXASessionClass;
        public Xing_t1833       xing_t1833;        //조건검색
        public Xing_t0424       xing_t0424;        //잔고2
       
        public Xing_t0425       xing_t0425;        //체결/미체결
        public Xing_t0167       xing_t0167;        //시간조회
        public Xing_CSPAT00600  xing_CSPAT00600;   //주식주문
        public Xing_CSPAT00800  xing_CSPAT00800;   //현물 취소주문
        public Xing_CSPAQ12200  xing_CSPAQ12200;   //현물계좌예수금/주문가능금액/총평가 조회

        public AccountForm      accountForm;       //계좌 선택
        public OptionForm       optionForm;        //프로그램 설정 폼

        public Real_SC1         real_SC1; //실시간 체결
        
       
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


            //프로그램 설정 초기화
            if (Properties.Settings.Default.STOP_PROFIT_TARGET.ToString() == "")
            {
                optionForm.rollBack();
            }
            

            //폼 초기화
            initForm();
        }

        //프로그램시작시 폼 초기화
        private void initForm(){
            
            input_loginId.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_ID);
            input_loginPw.Text  = Util.Decrypt(Properties.Settings.Default.LOGIN_PW);
            input_publicPw.Text = Util.Decrypt(Properties.Settings.Default.PUBLIC_PW);

            //서버 선택 콤보 초기화
            combox_targetServer.SelectedIndex = int.Parse(Properties.Settings.Default.SERVER_INDEX);

            //계좌잔고 그리드 초기화
            grd_t0424.DataSource = new BindingList<T0424Vo>();
            //진입검색 그리드.
            grd_t1833.DataSource = new BindingList<T1833Vo>();
            //체결미체결 그리드 DataSource 설정
            grd_t0425_chegb1.DataSource = new BindingList<T0425Vo>();//체결 그리드
            grd_t0425_chegb2.DataSource = new BindingList<T0425Vo>();//미체결 그리드

        }


        //properties 저장
        private void btn_config_save_Click(object sender, EventArgs e)  {

            // UI 필드 값을 읽어서 변수에 담음 - 비번등은 암호화 시킴
            int    serverIndex   = combox_targetServer.SelectedIndex;
            string loginId       = Util.Encrypt(input_loginId.Text.Trim());
            string loginPw       = Util.Encrypt(input_loginPw.Text.Trim());
            string publicPw      = Util.Encrypt(input_publicPw.Text.Trim());
 
        

            // 설정 파일에 저장
            Properties.Settings.Default.SERVER_INDEX = serverIndex.ToString();
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


        
        //미체결내역
        private void btn_t0425_Click(object sender, EventArgs e)
        {
            xing_t0425.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
        }

        //시작버튼 클릭 이벤트
        private void btn_start_Click(object sender, EventArgs e)
        {
            if (this.exXASessionClass.IsConnected()){
                if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == ""){
                        //MessageBox.Show("계좌 정보가 없습니다.");
                        AccountForm accountForm = new AccountForm();
                        accountForm.ShowDialog();
                        
                        //세션클래스에 있는거 그대로 가져왔다.
                        if (this.exXASessionClass.account == "" || this.exXASessionClass.accountPw == "")
                        {
                            MessageBox.Show("계좌 및 계좌 비밀번호를 설정해주세요.");
                        } else  {
                            /************************************************************************/
                            String dpsastTotamt = this.xing_CSPAQ12200.DpsastTotamt;//예탁자산 총액

                            //배팅금액설정
                            this.textBox_battingAtm.Text = Util.getBattingAmt(dpsastTotamt);
                    }
                } else {
                
                    timer_enterSearch.Enabled = true;//진입검색 타이머
                    timer_accountSearch.Enabled = true;//계좌 및 미체결 검색 타이머
                    btn_start.Enabled = false;
                    btn_stop.Enabled = true;

                    //실시간 체결정보
                    real_SC1.AdviseRealData();

                }
            }else {
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

            //실시간 체결 정보 종료
            real_SC1.UnadviseRealData();
        }

       


        //타이머 진입검색
        private void timer_enterSearch_Tick(object sender, EventArgs e){
            
            //if (int.Parse(xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(xing_t0167.time.Substring(0, 4)) < 2202){
                //MessageBox.Show(xing_t0167.time.Substring(0, 4));
                //조건검색
                //xing_t1833.call_request("condition.ADF");
                xing_t1833.call_request("conSeven.ADF");
            //}else
            //{
    
            //    tempLog.Text = "[" + input_time.Text + "]t0425 ::정규장이 아님 ";
            //}

        }

        //서버 시간
        private void timer_dateTime_Tick(object sender, EventArgs e)
        {
            xing_t0167.call_request();

            /// <summary>
            /// 현재 시간을 포멧 입힌 형태로 리턴
            /// </summary>
            /// <param name="format">
            /// yyyy-MM-dd HH:mm:ss -> 2013-10.30 14:30:21
            /// </param>
            /// <returns>포멧 변환된 값</returns>


            //input_dateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        }

        //타이머 계좌정보
        private void timer_accountSearch_Tick(object sender, EventArgs e)
        {
            //주식잔고2
            this.xing_t0424.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
            //미체결내역
            this.xing_t0425.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
            //Log.WriteLine("xing_t1833:");

            //현물계좌예수금/주문가능금액/총평가 조회
            this.xing_CSPAQ12200.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
            
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

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btn_program_config_Click(object sender, EventArgs e)
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
           
            BindingList<T0424Vo> t0424VoList = ((BindingList<T0424Vo>)this.grd_t0424.DataSource);
            for (int i=0;i< grd_t0424.RowCount; i++)
            {
              
                if (grd_t0424.Rows[i].Cells[0].FormattedValue.ToString() == "True")
                {
                    
                    expcode = grd_t0424.Rows[i].Cells[1].FormattedValue.ToString(); //종목코드
                    //주문 여부를 true로 업데이트
                    var result_t0424 = from item in t0424VoList
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

                        /// <param name="IsuNo">종목번호</param>
                        /// <param name="Quantity">수량</param>
                        /// <param name="Price">가격</param>
                        /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                        String msg = "[" + this.input_time.Text + "]t0424 ::선택매도[" + hname + "(" + expcode + ")]  수익율:" + sunikrt + "%    주문수량및매도가능:" + mdposqt;
                        this.xing_CSPAT00600.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw, msg, expcode, mdposqt, price, "1");
                        //tmpT0424Vo.orderAt = true;//일괄 매도시 주문여부를 true로 설정  

                        result_t0424.ElementAt(0).orderAt = true;//일괄 매도시 주문여부를 true로 설정
                    }

                }
                
          

            }
        }
    }//end class
}//end namespace

