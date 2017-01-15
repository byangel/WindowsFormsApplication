
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
using System.Data;

namespace PackageSellSystemTrading {
    //주식 잔고2
    public class Xing_t0424_config : XAQueryClass {

        public Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public int mamt;      //매입금액
        public int tappamt;   //평가금액
        public int tdtsunik;  //평가손익
        public int sunamt;    //추정자산
        public String  sunamt1;   //d1예수금
  

        public int testCount = 0;
        // 생성자
        public Xing_t0424_config()
        {
            base.ResFileName = "₩res₩t0424.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

        }   // end function

        // 소멸자
        ~Xing_t0424_config()
        {

        }

        /// <summary>
		/// 주식잔고2(T0424) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode) {
  
            String cts_expcode = base.GetFieldData("t0424OutBlock", "cts_expcode", 0);//CTS_종목번호-연속조회키

            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("t0424OutBlock1");


            // 계좌정보 써머리 계산 - 연속 조회이기때문에 합산후 마지막에 폼으로 출력.
            this.mamt     += int.Parse(base.GetFieldData("t0424OutBlock", "mamt"    , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "mamt"    , 0));//매입금액
            this.tappamt  += int.Parse(base.GetFieldData("t0424OutBlock", "tappamt" , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tappamt" , 0));//평가금액
            this.tdtsunik += int.Parse(base.GetFieldData("t0424OutBlock", "tdtsunik", 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tdtsunik", 0));//평가손익

            

            //2.연속 데이타 정보가 남아있는지 구분
            if (base.IsNext) 
            //if (cts_expcode != "")
            {
                //연속 데이타 정보를 호출.
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, cts_expcode);      // CTS종목번호 : 처음 조회시는 SPACE
                base.Request(true); //연속조회일경우 true
                //mainForm.input_t0424_log.Text = "[연속조회]잔고조회를 요청을 하였습니다.";
            } else {//마지막 데이타일때 메인폼에 출력해준다.


                //this.sunamt1 = this.GetFieldData("t0424OutBlock", "sunamt1", 0);// D1예수금
                String sunamt1 = this.GetFieldData("t0424OutBlock", "sunamt1", 0);// D1예수금

                //1.종목을 매수할때 매수할 금액을 정의 하는데 자본금이 늘어남에따라  효율적 투자를 목적으로 
                //매입금액과 예수금을 이용하여 프로그램 시작시 한번 동적으로 그값을 구한다.
                //소수점제거(예수금+매입금액)/500 = 배팅금액 --최소투자금액 1천만원
                decimal totalAmt = (this.mamt + int.Parse(sunamt1))/10000000;

                //소수점제거 후 배팅금액 구한다.
                decimal battingAmt = (Math.Floor(totalAmt) * 10000000) / 500;//

                mainForm.textBox_battingAtm.Text = battingAmt.ToString();



                //응답처리 완료
                completeAt = true;   
            }

        }//end

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
            if (nMessageCode == "00000") {
                ;
            }else {
                Log.WriteLine("[" + mainForm.input_time.Text + "]t0424_config :: " + nMessageCode + " :: " + szMessage);
                
                completeAt = true;//중복호출 방지
            }

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw )
        {

            if (completeAt) {
                this.completeAt = false;//중복호출 방지
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("t0424InBlock", "accno" , 0, account);    // 계좌번호
                base.SetFieldData("t0424InBlock", "passwd", 0, accountPw);  // 비밀번호
                base.SetFieldData("t0424InBlock", "prcgb" , 0, "1");        // 단가구분 : 1-평균단가, 2:BEP단가
                base.SetFieldData("t0424InBlock", "chegb" , 0, "2");        // 체결구분 : 0-결제기준, 2-체결기준
                base.SetFieldData("t0424InBlock", "dangb" , 0, "0");        // 단일가구분 : 0-정규장, 1-시간외단일가 
                base.SetFieldData("t0424InBlock", "charge", 0, "1");        // 제비용포함여부 : 0-미포함, 1-포함
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, "");      // CTS종목번호 : 처음 조회시는 SPACE

                // 계좌잔고 그리드 초기화
                //mainForm.grd_t0424.Rows.Clear();
                //mainForm.dataTable_t0424.Clear();

                //멤버변수 초기화
                this.mamt = 0;        //매입금액
                this.tappamt = 0;     //평가금액
                this.tdtsunik = 0;    //평가손익
       

                base.Request(false);  //연속조회일경우 true
                
                //폼 메세지.
                mainForm.input_t0424_log.Text = "["+ mainForm.input_time.Text+"]잔고조회를 요청을 하였습니다.";

            } else {
                mainForm.input_t0424_log.Text = "[" + mainForm.input_time.Text + "][중복]요청을 하였습니다.";
            }


        }	// end function


    } //end class 


   
}   // end namespace
