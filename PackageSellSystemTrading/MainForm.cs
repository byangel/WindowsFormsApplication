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

        // 로그인 정보
        public bool loginAt = false;

        private void MainForm_Load(object sender, EventArgs e)
        {
            //접속가능
            //실운영 : hts.ebestsec.co.kr
            //모의투자 : demo.ebestsec.co.kr
            exXASessionClass = new ExXASessionClass();//로그인

            xing_t1833 = new Xing_t1833();//종목검색
            this.xing_t1833.mainForm = this;

            //폼 초기화
            formInit();

            //개발완료시 제거해주자.
            input_loginId.Text = "neloi";
            input_loginPass.Text = "neloi1"; 
            input_publicPass.Text = "";
            //input_accountPass.Text = "0000";
        }

        private void formInit()
        {
            //서버 선택 콤보 초기화
            combox_targetServer.Items.Add("모의투자");
            combox_targetServer.Items.Add("실서버");
            combox_targetServer.SelectedIndex = 0;

            //종목검색 그리드 초기화
            //grd_searchBuy

        }

        //로그인 버튼 클릭 이벤트
        private void mf_loginBtn_Click(object sender, EventArgs e) {
            try {

                String mServerAddress="";

                String loginId     = input_loginId.Text;
                String loginPass   = input_loginPass.Text;
                String publicPass  = input_publicPass.Text;
                //String accountPass = input_accountPass.Text;
                if (loginId == "" && loginPass=="")
                {

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

                // 로그인
                bool loginAt = exXASessionClass.Login(loginId, loginPass, publicPass, 0, false);
                

            }catch (Exception ex) {
                MessageBox.Show(ex.Message);
            } 

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

       
    }//end class
}//end namespace

