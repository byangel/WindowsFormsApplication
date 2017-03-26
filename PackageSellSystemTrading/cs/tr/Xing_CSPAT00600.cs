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


        private String shcode;        // 종목번호
        private String quantity;      // 주문수량
        private String price;         // 주문가
        private String divideBuySell; // 매매구분: 1-매도, 2-매수
          


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
           //Log.WriteLine("CSPAT00600 :: 데이터 응답 처리가 완료 되었습니다.");
 
        }

        //현물정상주문 메세지 처리
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            
            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("CSPAT00600 :: " + nMessageCode + " :: " + szMessage+" - 종목코드|수량"+"["+ this.shcode +"|"+ this.quantity + "]");
                //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                // 01222 :: 모의투자 매도잔고가 부족합니다  
                // 00040 :: 모의투자 매수주문 입력이 완료되었습니다.
                // 00039 :: 모의투자 매도주문 입력이 완료되었습니다. 
                // 01221 :: 모의투자 증거금부족으로 주문이 불가능합니다
                // 01219 :: 모의투자 매매금지 종목
                
                //정규매매장이 종료되었습니다.
                if (nMessageCode=="03563")
                {
                    mainForm.marketAt = false;
                }
               
                //거래정지 종목으로 주문이 불가능합니다.
                if (nMessageCode == "01069")
                {
                    //잔고그리드 종목찾아서 에러상태 로 만들어서 매도 주문이 안나가도록 조치 하자. 
                    EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
                    var result_t0424 = from item in t0424VoList
                                       where item.expcode == this.shcode.Replace("A","")
                                       select item;
                    //MessageBox.Show(result_t0424.Count().ToString());
                    if (result_t0424.Count() > 0)
                    {
                       result_t0424.ElementAt(0).errorcd = "01069";
                    }  
                }


            }
            //mainForm.input_tmpLog.Text = "[" + mainForm.input_time.Text + "]CSPAT00600 :: [" + this.shcode + "]" + nMessageCode + " :: " + szMessage;
        }

        /// <summary>
        /// 현물정상주문
        /// </summary>
        /// <param name="IsuNo">종목번호</param>
        /// <param name="Quantity">수량</param>
        /// <param name="Price">가격</param>
        /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
        public void call_request(String account, String accountPw,String msg, String shcode, String quantity, String price, String divideBuySell)
        {
            
            //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + shcode;
            }
            this.shcode = shcode;        // 종목번호
            this.quantity = quantity;      // 주문수량
            this.price = price;         // 주문가
            this.divideBuySell = divideBuySell; // 매매구분: 1-매도, 2-매수
            //2.실시간 체결 정보 등록
            //mainForm.real_SC1.call_real(shcode);

            //if (completeAt){
            //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
            //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

            base.SetFieldData("CSPAT00600Inblock1", "AcntNo"       ,0, account);       // 계좌번호
            base.SetFieldData("CSPAT00600Inblock1", "InptPwd"      ,0, accountPw);     // 입력비밀번호
            base.SetFieldData("CSPAT00600Inblock1", "IsuNo"        ,0, shcode);        // 종목번호
            base.SetFieldData("CSPAT00600Inblock1", "OrdQty"       ,0, quantity);      // 주문수량
            base.SetFieldData("CSPAT00600Inblock1", "OrdPrc"       ,0, price);         // 주문가
            base.SetFieldData("CSPAT00600Inblock1", "BnsTpCode"    ,0, divideBuySell); // 매매구분: 1-매도, 2-매수
            base.SetFieldData("CSPAT00600Inblock1", "OrdprcPtnCode",0, "00");          // 호가유형코드: 00-지정가, 05-조건부지정가, 06-최유리지정가, 07-최우선지정가
            base.SetFieldData("CSPAT00600Inblock1", "MgntrnCode"   ,0, "000");         // 신용거래코드: 000-보통
            base.SetFieldData("CSPAT00600Inblock1", "LoanDt"       ,0, "");            // 대출일 : 신용주문이 아닐 경우 SPACE
            base.SetFieldData("CSPAT00600Inblock1", "OrdCndiTpCode",0, "0");           // 주문조건구분 : 0-없음

            if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1530) 
            {
                if (accountPw == "" || account == "")
                {
                    MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
                }
                else
                {
                    //if (int.Parse(mainForm.xing_t0167.time.Substring(0, 2)) > 9 && int.Parse(mainForm.xing_t0167.time.Substring(0, 2)) < 14)
                    //{
                    base.Request(false);  //연속조회일경우 true
                    Log.WriteLine(msg);

                    //if (divideBuySell == "1")
                    //{
                    //    Log.WriteLine("매입금액+D2예수금=" + (mainForm.xing_t0424.mamt + int.Parse(mainForm.xing_CSPAQ12200.D2Dps)));
                    //}
                    //  }
                    //  else {
                    //     mainForm.tempLog.Text = "["+mainForm.input_time.Text+"]정규매매장이 종료 되었습니다.";
                    //   }

                }
            }
        }	// end function


    } //end class 
   
}   // end namespace
