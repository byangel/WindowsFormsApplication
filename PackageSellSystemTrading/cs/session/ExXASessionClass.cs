
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


        public Boolean fnLogin()
        {

            Log.WriteLine("로그인 시도..!!");

            this.DisconnectServer();//무조건 끊었다가 접속
            if (this.ConnectServer(mainForm.combox_targetServer.Text, 20001) == false)
            {
                //MessageBox.Show(this.GetErrorMessage(this.GetLastError()));
                Log.WriteLine(this.GetErrorMessage(this.GetLastError()));
                return false;
            }
      
            // 로그인
            String loginId    = mainForm.input_loginId.Text;
            String loginPass  = mainForm.input_loginPw.Text;
            String publicPass = mainForm.input_publicPw.Text;
            
            if (loginId == "" && loginPass == ""){
                Log.WriteLine("ID 또는 Pass 값을 참조할 수 없습니다.");
                return false;
            }else{
                // 로그인 호출
                bool loginAt = Login(loginId, loginPass, publicPass, 0, false);
         
            }
           
            return true;
        }


        #region XASession 이벤트 핸들러
        private void loginEventHandler(string szCode, string szMsg) {
            String msg = "";

            // 정상적으로 로그인 되었으면...
            if (szCode == "0000")
            {

                

                //아침에 서버 접속이 끊겼을경우 재로그인 하는데 다이얼로그가 뜨지않아도 되기 때문에 이구문을 추가함
                if (mainForm.account == "" || mainForm.account == null)
                {
                    mainForm.accountForm.ShowDialog();
                }
                else
                {   //접속이 끊겨서 로그인을 호출했을경우 다이얼로그 박스를 호출하지 않고 바로 계좌 검증 함수를 호출해준다.
                    if (mainForm.accountForm.xing_CSPAQ12300.completeAt)
                    {
                        mainForm.accountForm.xing_CSPAQ12300.call_request(this.mainForm.account, mainForm.accountPw);
                    }
                   
                }

                //계좌 선택및 비밀번호 선택 다이얼로그 이후 계좌번호와 비밀번호가 잘 설정 되어 있는지 체크한다.
                if (mainForm.account == "" || mainForm.accountPw == "" || mainForm.account == null || mainForm.accountPw == null)
                {
                    msg = "계좌 및 계좌 비밀번호를 설정해주세요.";
                }
                else
                {
                    loginAt = true;
                }

            }else{
                if (szCode == "5201")
                {
                    // 프로그램 재시작
                    msg = szCode + ":" + szMsg + ": [  -13] Request ID가 이상합니다.";
                    //Log.WriteLine(szCode + " :: " + szMsg);
                    // mainForm.fnRestartProgram();
                }
                else if (szCode == "2003")
                { // 인증서가 없습니다.
                    msg = szCode + ":" + szMsg + ": [공인인증] Full 인증과정에서 다음과 같은 오류가 발생하였습니다.";
                    // mainForm.TimerLogin.Stop();

                } else {
                    //Log.WriteLine(szCode + " :: " + szMsg);
                    msg = szCode + " :: " + szMsg;
                }
                
                //MessageBox.Show(msg);
                Log.WriteLine("ExXASession :: " + msg);
                mainForm.insertListBoxLog(msg);
  
              
            }

            
            


        }


        #endregion
    } //end class 

}// end namespace