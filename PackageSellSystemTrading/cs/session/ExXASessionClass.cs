
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;

namespace PackageSellSystemTrading{
    public class ExXASessionClass : XASessionClass {

        public MainForm mainForm;

        // 생성자
        public ExXASessionClass()
        {
            //이벤트등록 문법이 어떤 구조인지 잘모르겠다.
            base._IXASessionEvents_Event_Login += loginEventHandler;

            //xASessionClass._IXASessionEvents_Event_Logout += new _IXASessionEvents_LogoutEventHandler(clsXASession__IXASessionEvents_Event_Logout);
            //xASessionClass.Disconnect += new _IXASessionEvents_DisconnectEventHandler(clsXASession_Disconnect);
           
        }   // end function

        // 소멸자
        ~ExXASessionClass()
        {
            //this.iXASession.Logout();
            //this.iXASession.DisconnectServer();
        }



        #region XASession 이벤트 핸들러
        private  void loginEventHandler(string szCode, string szMsg)
        { 
        //private new void _IXASessionEvents_Event_Login(string szCode, string szMsg) {
           
            String msg = null;
            try{
                // 정상적으로 로그인 되었으면...
                if (szCode == "0000"){

                    //Log.WriteLine(szCode + " :: " + szMsg);

                    // 로그인 버튼 비활성
                    //mainForm.ButtonLogin.Enabled = false;

                    // 자동로그인 타이머 멈춤
                    //mainForm.TimerLogin.Stop();

                    // 장 운영 정보 실시간 등록
                    //mainForm.mxRealJif.call_advise();

                    // HTS -> API 연동 등록
                    // mainForm.mxRealJif.call_advise_link_from_hts();

                    // 뉴스 정보 실시간 등록
                    //mainForm.mxRealNws.call_advise();

                    //로그인 성공시 계좌 목록을 콤보박스에 출력
                    String account;
                    int accountListCount = base.GetAccountListCount();
                    for (int i=0;i<accountListCount; i++) {
                        account = base.GetAccountList(i);
                        mainForm.comBox_account.Items.Add(account);
                    }
                    mainForm.comBox_account.SelectedIndex = 0;

                    msg = "성공적으로 로그인 하였습니다.";
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
                }else if (szCode == "5201"){
                    // 프로그램 재시작
                    msg = szCode + ":" + szMsg + ": [  -13] Request ID가 이상합니다.";
                    //Log.WriteLine(szCode + " :: " + szMsg);
                    // mainForm.fnRestartProgram();
                }else if (szCode == "2003"){ // 인증서가 없습니다.
                    msg = szCode + ":" + szMsg + ": [공인인증] Full 인증과정에서 다음과 같은 오류가 발생하였습니다.";
                    // mainForm.TimerLogin.Stop();

                }else{
                   //Log.WriteLine(szCode + " :: " + szMsg);
                    msg = szCode + " :: " + szMsg;
                }
                Log.WriteLine("ExXASession :: " + szCode + " :: " + szMsg);
                MessageBox.Show(msg);

            }
            catch (Exception ex)  {
                MessageBox.Show(ex.Message);
                //Log.WriteLine(ex.Message);
                // Log.WriteLine(ex.StackTrace);
            }

        }
        #endregion

    } //end class 
   
}   // end namespace
