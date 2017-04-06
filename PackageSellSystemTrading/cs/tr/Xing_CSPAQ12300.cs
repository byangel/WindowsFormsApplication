
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


            //this.D2Dps = base.GetFieldData("CSPAQ12300OutBlock2", "D2Dps", 0); //D2예수금
            //this.Dps = base.GetFieldData("CSPAQ12300OutBlock2", "Dps", 0); //예수금
            //this.DpsastTotamt = base.GetFieldData("CSPAQ12300OutBlock2", "DpsastTotamt", 0); //예탁자잔총액
            
            //this.BalEvalAmt = base.GetFieldData("CSPAQ12300OutBlock2", "BalEvalAmt", 0); //잔고평가금액
            //this.InvstPlAmt = base.GetFieldData("CSPAQ12300OutBlock2", "InvstPlAmt", 0); //투자손익금액
            //this.PnlRat = base.GetFieldData("CSPAQ12300OutBlock2", "PnlRat", 0); //손익률
            //this.PchsAmt = base.GetFieldData("CSPAQ12300OutBlock2", "PchsAmt", 0);//매입금액

            //mainForm.input_Dps.Text = Util.GetNumberFormat(this.Dps);          // 예수금
            //mainForm.input_D2Dps.Text = Util.GetNumberFormat(this.D2Dps);        // D2예수금
            //mainForm.input_DpsastTotamt.Text = Util.GetNumberFormat(this.DpsastTotamt); //예탁자잔총액
            //mainForm.input_InvstOrgAmt.Text = Util.GetNumberFormat(this.PchsAmt); //투자원금 --미사용 --매입금액
            //mainForm.input_BalEvalAmt.Text = Util.GetNumberFormat(this.BalEvalAmt); //잔고평가금액
           
            //mainForm.input_tdtsunik.Text = Util.GetNumberFormat(this.InvstPlAmt); //투자손익금액

            //mainForm.input_PnlRat.Text = Util.GetNumberFormat(this.PnlRat);   // 손익률

            ///////////////////////////////////
            //MessageBox.Show(base.GetFieldData("CSPAQ12300OutBlock2", "PchsAmt", 0));
            ////1.종목을 매수할때 매수할 금액을 정의 하는데 자본금이 늘어남에따라  효율적 투자를 목적으로 
            ////매입금액과 예수금을 이용하여 프로그램 시작시 한번 동적으로 그값을 구한다.
            ////소수점제거(예수금+매입금액)/500 = 배팅금액 --최소투자금액 1천만원
            ////MessageBox.Show(this.DpsastTotamt);
            //decimal totalAmt = decimal.Parse(this.DpsastTotamt) / 10000000;
            ////소수점제거 후 배팅금액 구한다.
            //decimal battingAmt = (Math.Floor(totalAmt) * 10000000) / 500;//
            //mainForm.textBox_battingAtm.Text = battingAmt.ToString();



            //CSPAQ12300OutBlock2 실서버에서는 데이타가 없다.

            completeAt = true;

        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

            try {
                //특정 오류없이 정상적으로 실행이 되었다면 화면 및 초기화를 해준다.
                if (nMessageCode == "00136" || nMessageCode == "00133") {
                    try
                    {
                        //계좌정보가 정상 확인 되었으면 다른 프로그램에서 계좌번호와 비밀번호를 쓸수 있도록 메인폼 멤버변수에 저장한다.
                        accountForm.exXASessionClass.account = this.account;
                        accountForm.exXASessionClass.accountPw = this.accountPw;

                        //잔고정보
                        mainForm.xing_CSPAQ12200.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw);
                        //잔고목록
                        mainForm.xing_t0424.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw);
                        //체결미체결
                        mainForm.xing_t0425.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw);
                        //MessageBox.Show("계좌 정보가 정상확인 되었습니다.");

                        //설정저장 버튼 활성화.
                        mainForm.btn_config_save.Enabled = true;

                        // 로그인 버튼 비활성
                        mainForm.btn_login.Enabled = false;

                        //매수금지종목 조회
                        mainForm.xing_t1833Exclude.call_request();

                        accountForm.Close();

                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show("예외발생 로그 확인 요망.");
                        MessageBox.Show("예외 발생:{0}", e.Message);
                    }
                }
                else{
                    MessageBox.Show("CSPAQ12300 :: " + nMessageCode + " :: " + szMessage);
                    completeAt = true;//중복호출 방지
                }
               
            }
            catch (Exception ex){
                Log.WriteLine(ex.Message);
               
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
                MessageBox.Show("CSPAQ12300 :: 중복 조회 잠시후 시도해주세요.");
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
