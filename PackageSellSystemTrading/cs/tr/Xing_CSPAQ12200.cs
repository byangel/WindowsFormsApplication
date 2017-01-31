
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
    //현물계좌 예수금/주문가능금액/총평가 조회(API)
    public class Xing_CSPAQ12200 : XAQueryClass{

        public Boolean completeAt = true;//완료여부.
     
        public MainForm    mainForm;

        public String D2Dps;       //D2예수금
        public String Dps;         //예수금
        public String PnlRat;      //손익율 
        public String BalEvalAmt;  //잔고평가금액
        public String DpsastTotamt;//예탁자잔총액
        public String InvstOrgAmt; //투자원금
        public String InvstPlAmt;  //투자손익금액

        // 생성자
        public Xing_CSPAQ12200()
        {
            base.ResFileName = "₩res₩CSPAQ12200.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            completeAt = true;
        }   // end function

        // 소멸자
        ~Xing_CSPAQ12200()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            this.D2Dps        = base.GetFieldData("CSPAQ12200OutBlock2", "D2Dps"       , 0); //D2예수금
            this.Dps          = base.GetFieldData("CSPAQ12200OutBlock2", "Dps"         , 0); //예수금
            this.DpsastTotamt = base.GetFieldData("CSPAQ12200OutBlock2", "DpsastTotamt", 0); //예탁자잔총액
            this.InvstOrgAmt  = base.GetFieldData("CSPAQ12200OutBlock2", "InvstOrgAmt" , 0); //투자원금
            this.BalEvalAmt   = base.GetFieldData("CSPAQ12200OutBlock2", "BalEvalAmt"  , 0); //잔고평가금액
            this.InvstPlAmt   = base.GetFieldData("CSPAQ12200OutBlock2", "InvstPlAmt"  , 0); //투자손익금액
            this.PnlRat       = base.GetFieldData("CSPAQ12200OutBlock2", "PnlRat"      , 0); //손익률


            mainForm.input_Dps.Text          = Util.GetNumberFormat(this.Dps);          // 예수금
            mainForm.input_D2Dps.Text        = Util.GetNumberFormat(this.D2Dps);        // D2예수금
            mainForm.input_DpsastTotamt.Text = Util.GetNumberFormat(this.DpsastTotamt); //예탁자잔총액
            //mainForm.input_mamt.Text  = Util.GetNumberFormat(this.InvstOrgAmt); //투자원금 --미사용
            mainForm.input_BalEvalAmt.Text   = Util.GetNumberFormat(this.BalEvalAmt); //잔고평가금액
            //mainForm.input_InvstPlAmtt.Text  = Util.GetNumberFormat(this.InvstPlAmt); //투자손익금액
            mainForm.input_tdtsunik.Text = Util.GetNumberFormat(this.InvstPlAmt); //투자손익금액
            
            mainForm.input_PnlRat.Text       = Util.GetNumberFormat(this.PnlRat);   // 손익률

            /////////////////////////////////
            //MessageBox.Show(base.GetFieldData("CSPAQ12200OutBlock2", "PchsAmt", 0));
            //1.종목을 매수할때 매수할 금액을 정의 하는데 자본금이 늘어남에따라  효율적 투자를 목적으로 
            //매입금액과 예수금을 이용하여 프로그램 시작시 한번 동적으로 그값을 구한다.
            //소수점제거(예수금+매입금액)/500 = 배팅금액 --최소투자금액 1천만원
            //MessageBox.Show(this.DpsastTotamt);
            double totalAmt = double.Parse(this.DpsastTotamt) / 10000000;
            //소수점제거 후 배팅금액 구한다.
            double battingAmt = (Math.Floor(totalAmt) * 10000000) / 500;//
            mainForm.textBox_battingAtm.Text = battingAmt.ToString();

            completeAt = true;

        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
            if (nMessageCode == "00000")
            {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            }
            else
            {
                completeAt = true;//중복호출 방지
            }

        }

       

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw)
        {

            if (this.completeAt) {
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조
                base.SetFieldData("CSPAQ12200InBlock1", "RecCnt" , 0, "1"); 
                base.SetFieldData("CSPAQ12200InBlock1", "AcntNo" , 0, account);  // 계좌번호
                base.SetFieldData("CSPAQ12200InBlock1", "Pwd"    , 0, accountPw);// 비밀번호
               
                base.Request(false);  //연속조회일경우 true
                this.completeAt = false;//중복호출 방지

            } else {
                //mainForm.input_t0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
                MessageBox.Show("중복 조회 잠시후 시도해주세요.");
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
