
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

        public Boolean completeAt = true;//완료여부.
        private String  account;
        private String  accountPw;
        
        public AccountForm accountForm;
        public MainForm    mainForm;

        
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
            completeAt = true;
        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

        
                //특정 오류없이 정상적으로 실행이 되었다면 화면 및 초기화를 해준다.
                if (nMessageCode == "00136" || nMessageCode == "00133") {
                   
                    //계좌정보가 정상 확인 되었으면 다른 프로그램에서 계좌번호와 비밀번호를 쓸수 있도록 메인폼 멤버변수에 저장한다.
                    mainForm.account = this.account;
                    mainForm.accountPw = this.accountPw;

                    mainForm.accountAfter();

                    //계좌번호와 페스워드가 인증되었으면 계좌번호선택폼을 닫아준다.
                    accountForm.Close();


                    Log.WriteLine("CSPAQ12300::" + nMessageCode + " :: " + szMessage);

                }else{
                    //MessageBox.Show("CSPAQ12300 :: " + nMessageCode + " :: " + szMessage);
                    Log.WriteLine("CSPAQ12300:: 예외 발생:" + nMessageCode + " :: " + szMessage);
                }
        
            completeAt = true;

        }

       

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request( String account, String accountPw)
        {

            this.account   = account;
            this.accountPw = accountPw;
            
            if (completeAt) {
                completeAt = false;//중복호출 방지
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
                

            } else {
                //mainForm.input_t0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
                //MessageBox.Show("CSPAQ12300 :: 중복 조회 잠시후 시도해주세요.");
                Log.WriteLine("CSPAQ12300 :: 중복 조회 잠시후 시도해주세요.");
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
