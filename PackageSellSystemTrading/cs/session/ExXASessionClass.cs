
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;

namespace PackageSellSystemTrading {
    public class ExXASessionClass : XASessionClass {

        public MainForm mainForm;

        public String account { get; set; }
        public String accountPw { get; set; }

        // 생성자
        public ExXASessionClass() {
            //이벤트등록 문법이 어떤 구조인지 잘모르겠다.
            base._IXASessionEvents_Event_Login += loginEventHandler;

            //xASessionClass._IXASessionEvents_Event_Logout += new _IXASessionEvents_LogoutEventHandler(clsXASession__IXASessionEvents_Event_Logout);
            //xASessionClass.Disconnect += new _IXASessionEvents_DisconnectEventHandler(clsXASession_Disconnect);

        }   // end function

        // 소멸자
        ~ExXASessionClass() {
            //this.iXASession.Logout();
            //this.iXASession.DisconnectServer();
        }



        #region XASession 이벤트 핸들러
        private void loginEventHandler(string szCode, string szMsg) {
            String msg = "";

            // 정상적으로 로그인 되었으면...
            if (szCode == "0000") {

                //Log.WriteLine(szCode + " :: " + szMsg);

                // 로그인 버튼 비활성
                mainForm.btn_login.Enabled = false;

                // 자동로그인 타이머 멈춤
                //mainForm.TimerLogin.Stop();

                // 장 운영 정보 실시간 등록
                //mainForm.mxRealJif.call_advise();

                // HTS -> API 연동 등록
                // mainForm.mxRealJif.call_advise_link_from_hts();

                // 뉴스 정보 실시간 등록
                //mainForm.mxRealNws.call_advise();



                //msg = "성공적으로 로그인 하였습니다.";
                // 로그인          

                // 서버의 시간 검색 타이머 스타트 - 여기서 PC의 시간을 서버 시간과 동기화 시킴
                //mainForm.Timer0167.Start();

                // 디비에 백그라운드 데이터 저장 타이머 스타트
                //mainForm.TimerDB.Start();

                // 메인프로그램은 자동거래 시작
                //if (FormMain.mProgramId == 0)
                //{
                //  Log.WriteLine("Trading Start..!!");

                // mainForm.TimerBuyRun.Start();

                //mainForm.ButtonAutoBuyStart.Enabled = false;
                //mainForm.ButtonAutoBuyStop.Enabled = true;
                // }

                //로그인 성공시 계좌 목록을 콤보박스에 출력
                //계좌정보 설정 폼 팝업 호출
                //AccountForm accountForm = new AccountForm();
                //accountForm.mainForm = this.mainForm;
                //accountForm.exXASessionClass = this;

                mainForm.accountForm.ShowDialog();

                if (this.account == "" || this.accountPw == "") {
                    msg = "계좌 및 계좌 비밀번호를 설정해주세요.";
                }else{
                    //로그인후 프로그램 초기화.
                    // 계좌정보 조회.
                    //mainForm.xing_t0424.call_request(this.account, this.accountPw);
                    //설정 저장버튼 활성화 
                    //mainForm.btn_config_save.Enabled = true;

                    // 계좌정보 조회.
                    //xing_t0424.call_request(this.exXASessionClass.account, this.exXASessionClass.accountPw);
                    //mainForm.xing_t0424_config.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw);
                    //mainForm.xing_CSPAQ12200.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw);
                    //날자 및 시간 타이머 시작.
                    //mainForm.timer_dateTime.Enabled = true;

                    //this.sunamt1 = this.GetFieldData("t0424OutBlock", "sunamt1", 0);// D1예수금
                    
                    //String sunamt1 = this.mainForm.xing_CSPAQ12200.D2Dps;// D1예수금
                    //String DpsastTotamt = this.mainForm.xing_CSPAQ12200.DpsastTotamt;//예탁자산 총액

                   

                    //MessageBox.Show(sunamt1);
                    //1.종목을 매수할때 매수할 금액을 정의 하는데 자본금이 늘어남에따라  효율적 투자를 목적으로 
                    //매입금액과 예수금을 이용하여 프로그램 시작시 한번 동적으로 그값을 구한다.
                    //소수점제거(예수금+매입금액)/500 = 배팅금액 --최소투자금액 1천만원
                    //decimal totalAmt = int.Parse(DpsastTotamt) / 10000000;

                    //소수점제거 후 배팅금액 구한다.
                    //decimal battingAmt = (Math.Floor(totalAmt) * 10000000) / 500;//

                    //mainForm.textBox_battingAtm.Text = battingAmt.ToString();
                }
                
            } else if (szCode == "5201") {
                // 프로그램 재시작
                msg = szCode + ":" + szMsg + ": [  -13] Request ID가 이상합니다.";
                //Log.WriteLine(szCode + " :: " + szMsg);
                // mainForm.fnRestartProgram();
            } else if (szCode == "2003") { // 인증서가 없습니다.
                msg = szCode + ":" + szMsg + ": [공인인증] Full 인증과정에서 다음과 같은 오류가 발생하였습니다.";
                // mainForm.TimerLogin.Stop();

            } else {
                //Log.WriteLine(szCode + " :: " + szMsg);
                msg = szCode + " :: " + szMsg;
            }
            Log.WriteLine("ExXASession :: " + szCode + " :: " + szMsg);

            if (msg != ""){
                MessageBox.Show(msg);
            }
            
        }


        #endregion
    } //end class 

}// end namespace