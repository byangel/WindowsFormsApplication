
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

        public Boolean loginAt; //로그인 여부
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

                // 자동로그인 타이머 멈춤
                //mainForm.TimerLogin.Stop();

                // 장 운영 정보 실시간 등록
                mainForm.real_jif.call_advise();

                // HTS -> API 연동 등록
                //mainForm.real_jif.call_advise_link_from_hts();

                //실시간 체결정보 - 재접속했을때 안해주면 채결되도 그리드에서 종목을 제거해주지 못한다.
                mainForm.real_SC1.AdviseRealData();

                // 뉴스 정보 실시간 등록
                //mainForm.mxRealNws.call_advise();



                //msg = "성공적으로 로그인 하였습니다.";
                // 로그인          

                // 서버의 시간 검색 타이머 스타트 - 여기서 PC의 시간을 서버 시간과 동기화 시킴
                //mainForm.Timer0167.Start();
                mainForm.Timer0167.Start();

               

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

                //아침에 서버 접속이 끊겼을경우 재로그인 하는데 다이얼로그가 뜨지않아도 되기 때문에 이구문을 추가함
                if (mainForm.accountForm.account == "" || mainForm.accountForm.account == null ){
                    mainForm.accountForm.ShowDialog();
                }

                //계좌 선택및 비밀번호 선택 다이얼로그 이후 계좌번호와 비밀번호가 잘 설정 되어 있는지 체크한다.
                if (mainForm.accountForm.account == "" || mainForm.accountForm.accountPw == "" || mainForm.accountForm.account == null || mainForm.accountForm.accountPw == null) {
                    msg = "계좌 및 계좌 비밀번호를 설정해주세요.";
                }else {
                    loginAt = true;
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