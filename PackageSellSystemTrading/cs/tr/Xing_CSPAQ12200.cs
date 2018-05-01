
using System;
using XA_DATASETLib;


namespace PackageSellSystemTrading
{
    //현물계좌 예수금/주문가능금액/총평가 조회(API)
    public class Xing_CSPAQ12200 : XAQueryClass{

        public Boolean completeAt = true;//완료여부.
     
        public MainForm    mainForm;

        public String D2Dps        = "0"; //D2예수금
        public String Dps          = "0"; //예수금
        public String PnlRat       = "0"; //손익율 
        public String BalEvalAmt   = "0"; //잔고평가금액
        public String DpsastTotamt = "0"; //예탁자잔총액
        public String InvstOrgAmt  = "0"; //투자원금
        public String InvstPlAmt   = "0"; //투자손익금액

        public Boolean initAt = false;
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


            //mainForm.input_Dps.Text            = Util.GetNumberFormat(this.Dps);          // 예수금
            //mainForm.input_D2Dps.Text          = Util.GetNumberFormat(this.D2Dps);        // D2예수금
            //mainForm.input_DpsastTotamt.Text   = Util.GetNumberFormat(this.DpsastTotamt); //예탁자잔총액
            //mainForm.input_mamt.Text         = Util.GetNumberFormat(this.InvstOrgAmt); //투자원금 --미사용
            //mainForm.input_BalEvalAmt.Text     = Util.GetNumberFormat(this.BalEvalAmt); //잔고평가금액
            //mainForm.input_InvstPlAmtt.Text  = Util.GetNumberFormat(this.InvstPlAmt); //투자손익금액
            //mainForm.input_tdtsunik.Text = Util.GetNumberFormat(this.InvstPlAmt); //투자손익금액

            //mainForm.input_PnlRat.Text       = Util.GetNumberFormat(this.PnlRat);   // 손익률

            //label 출력
            mainForm.label_Dps.Text          = Util.GetNumberFormat(this.Dps);          // 예수금
            mainForm.label_D2Dps.Text        = Util.GetNumberFormat(this.D2Dps);        // D2예수금
            mainForm.label_DpsastTotamt.Text = Util.GetNumberFormat(this.DpsastTotamt); //예탁자잔총액           
            mainForm.label_BalEvalAmt.Text   = Util.GetNumberFormat(this.BalEvalAmt); //잔고평가금액

            if (this.BalEvalAmt != "" && this.BalEvalAmt != null)
            {
                mainForm.label_tdtsunik.Text = Util.GetNumberFormat(double.Parse(this.BalEvalAmt) - mainForm.xing_t0424.mamt); //투자손익금액(잔고평가금액-매입금액 )
            }
            
            mainForm.label_PnlRat.Text = Util.GetNumberFormat(this.PnlRat);   // 손익률
            
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
                //00133 :: 조회가 계속 됩니다. 계속하시려면 연속버튼을 누르십시오.
                completeAt = true;//중복호출 방지
            }

        }


        private int callCnt = 0;
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
                Log.WriteLine("CSPAQ12200 :: 중복 조회 잠시후 시도해주세요.");
                callCnt++;
                if (callCnt == 5)
                {
                    this.completeAt = true;
                    callCnt = 0;
                }
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
