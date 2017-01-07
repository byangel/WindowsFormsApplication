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
    //현물 정상주문
    public class Xing_CSPAT00600 : XAQueryClass{


        public MainForm mainForm;
        // 생성자
        public Xing_CSPAT00600() {
            base.ResFileName = "₩res₩CSPAT00600.res";

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_CSPAT00600()
        {
          
        }

        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            
           //로그 및 중복 요청 처리
           Log.WriteLine("CSPAT00600 :: 데이터 응답 처리가 완료 되었습니다.");
 
        }

        //현물정상주문 메세지 처리
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            
            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("CSPAT00600 :: " + nMessageCode + " :: " + szMessage);
                //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                // 01222 :: 모의투자 매도잔고가 부족합니다  
                // 00040 :: 모의투자 매수주문 입력이 완료되었습니다.
                // 00039 :: 모의투자 매도주문 입력이 완료되었습니다. 
                // 01221 :: 모의투자 증거금부족으로 주문이 불가능합니다
                // 01219 :: 모의투자 매매금지 종목
            }
   
        }

        /// <summary>
        /// 현물정상주문
        /// </summary>
        /// <param name="IsuNo">종목번호</param>
        /// <param name="Quantity">수량</param>
        /// <param name="Price">가격</param>
        /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
        public void call_request(string IsuNo, string Quantity, string Price, string DivideBuySell)
        {
            //모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                IsuNo = "A" + IsuNo;
            }

            //if (completeAt){
                String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("CSPAT00600Inblock1", "AcntNo"       ,0, account);       // 계좌번호
                base.SetFieldData("CSPAT00600Inblock1", "InptPwd"      ,0, accountPw);     // 입력비밀번호
                base.SetFieldData("CSPAT00600Inblock1", "IsuNo"        ,0, IsuNo);         // 종목번호
                base.SetFieldData("CSPAT00600Inblock1", "OrdQty"       ,0, Quantity);      // 주문수량
                base.SetFieldData("CSPAT00600Inblock1", "OrdPrc"       ,0, Price);         // 주문가
                base.SetFieldData("CSPAT00600Inblock1", "BnsTpCode"    ,0, DivideBuySell); // 매매구분: 1-매도, 2-매수
                base.SetFieldData("CSPAT00600Inblock1", "OrdprcPtnCode",0, "00");          // 호가유형코드: 00-지정가, 05-조건부지정가, 06-최유리지정가, 07-최우선지정가
                base.SetFieldData("CSPAT00600Inblock1", "MgntrnCode"   ,0, "000");         // 신용거래코드: 000-보통
                base.SetFieldData("CSPAT00600Inblock1", "LoanDt"       ,0, "");            // 대출일 : 신용주문이 아닐 경우 SPACE
                base.SetFieldData("CSPAT00600Inblock1", "OrdCndiTpCode",0, "0");           // 주문조건구분 : 0-없음

                if (accountPw == "" || account == ""){
                    MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
                }else{
                    base.Request(false);  //연속조회일경우 true
            
                }

            //}else{
            //    mainForm.input_CSPAT00600_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
            //}


        }	// end function


    } //end class 
   
}   // end namespace
