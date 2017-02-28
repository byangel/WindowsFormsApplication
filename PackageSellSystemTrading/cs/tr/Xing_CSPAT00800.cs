
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;

namespace PackageSellSystemTrading{
    //현물 취소주문
    public class Xing_CSPAT00800 : XAQueryClass{

        public MainForm mainForm;
        // 생성자
        public Xing_CSPAT00800()
        {
            base.ResFileName = "₩res₩CSPAT00800.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_CSPAT00800()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            //로그 및 중복 요청 처리
            Log.WriteLine("[" + mainForm.input_time.Text + "]CSPAT00800 :: 데이터 응답 처리가 완료 되었습니다.");

        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("[" + mainForm.input_time.Text + "]CSPAT00800 :: " + nMessageCode + " :: " + szMessage);
                //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage; 
            }
            

        }

        /// <summary>
		/// 현물 취소 주문
		/// </summary>
		/// <param name="OrgOrdNo">원주문번호</param>
		/// <param name="IsuNo">종목번호</param>
		/// <param name="OrdQty">주문수량</param>
		public void call_request(String account, String accountPw, string OrgOrdNo, string shcode, string OrdQty)
        {
            //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + shcode;
            }

            //2.실시간 체결 정보요청 취소.
            mainForm.real_SC1.UnadviseRealDataWithKey(shcode);

            //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
            //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

            base.SetFieldData("CSPAT00800InBlock1", "AcntNo"  , 0, account);  // 계좌번호
            base.SetFieldData("CSPAT00800InBlock1", "InptPwd" , 0, accountPw);// 입력비밀번호
            base.SetFieldData("CSPAT00800InBlock1", "OrgOrdNo", 0, OrgOrdNo); // 원주문번호
            base.SetFieldData("CSPAT00800InBlock1", "IsuNo"   , 0, shcode);    // 종목번호
            base.SetFieldData("CSPAT00800InBlock1", "OrdQty"  , 0, OrdQty);   // 주문수량

            base.Request(false);
        }


    } //end class 
   
}   // end namespace
