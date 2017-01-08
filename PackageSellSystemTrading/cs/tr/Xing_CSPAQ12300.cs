
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Threading;

namespace PackageSellSystemTrading{
    //현물계좌 잔고내역 조회(API) - 계좌 비밀번호 체크에 사용한다.
    public class Xing_CSPAQ12300 : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;
        private String  account;
        private String  accountPw;
        private AccountForm accountForm;
        // 생성자
        public Xing_CSPAQ12300()
        {
            base.ResFileName = "₩res₩CSPAQ12300.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            completeAt = true;
        }   // end function

        // 소멸자
        ~Xing_CSPAQ12300()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            //CSPAQ12300OutBlock2 실서버에서는 데이타가 없다.
            
            completeAt = true;

        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
            try{
                if (nMessageCode == "00136" || nMessageCode == "00133") {
                    //계좌정보가 정상 확인 되었으면 다른 프로그램에서 계좌번호와 비밀번호를 쓸수 있도록 메인폼 멤버변수에 저장한다.
                    mainForm.account   = this.account;
                    mainForm.accountPw = this.accountPw;

                    // 계좌정보 조회.
                    mainForm.xing_t0424.call_request(this.account, this.accountPw);

                    //MessageBox.Show("계좌 정보가 정상확인 되었습니다.");
                    accountForm.Close();
                }
                else{
                    MessageBox.Show("CSPAQ12300 :: " + nMessageCode + " :: " + szMessage);
                    completeAt = true;//중복호출 방지
                }
                Log.WriteLine("CSPAQ12300 :: " + nMessageCode + " :: " + szMessage);
            }
            catch (Exception ex){
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }

        }

       

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(AccountForm accountForm, String account, String accountPw)
        {

            this.account   = account;
            this.accountPw = accountPw;
            this.accountForm = accountForm;
            if (completeAt) {
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("CSPAQ12300InBlock1", "AcntNo"         , 0, account);  // 계좌번호
                base.SetFieldData("CSPAQ12300InBlock1", "Pwd"            , 0, accountPw);// 비밀번호
                base.SetFieldData("CSPAQ12300InBlock1", "RecCnt"         , 0, "1");// 레코드수
                base.SetFieldData("CSPAQ12300InBlock1", "BalCreTp"       , 0, "1");      // 잔고생성구분      - 0:전체 | 1:현물 | 9:선물대용
                base.SetFieldData("CSPAQ12300InBlock1", "CmsnAppTpCode"  , 0, "0");      // 수수료적용구분    - 0:평가시수수료미적용 | 1:평가시수수료적용 ?값을변경해도 무슨차이인지 모르겠음.
                base.SetFieldData("CSPAQ12300InBlock1", "D2balBaseQryTp" , 0, "0");      //D2잔고기준조회구분 - 0:전부조회 | 1:D2잔고0이상만조회
                base.SetFieldData("CSPAQ12300InBlock1", "UprcTpCode"     , 0, "1");      //단가구분          - 0:평균단가 | 1:BEP단가

                base.Request(false);  //연속조회일경우 true
                completeAt = false;//중복호출 방지

            } else {
                //mainForm.input_t0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
                MessageBox.Show("중복 조회 잠시후 시도해주세요.");
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
