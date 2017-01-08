
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
    public class Xing_t0424 : XAQueryClass {

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public int mamt;      //매입금액
        public int tappamt;   //평가금액
        public int tdtsunik;  //평가손익
        public int sunamt;    //추정자산
        public int h_totalCount;

        public int testCount = 0;
        // 생성자
        public Xing_t0424()
        {
            base.ResFileName = "₩res₩t0424.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

        }   // end function

        // 소멸자
        ~Xing_t0424()
        {

        }

        /// <summary>
		/// 주식잔고2(T0424) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode) {

            testCount += 1;
            Log.WriteLine("t0424 dataEventHandler call:: " + testCount);

            String cts_expcode = base.GetFieldData("t0424OutBlock", "cts_expcode", 0);//CTS_종목번호-연속조회키


            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("t0424OutBlock1");
 
            DataRow[] dataRowArray;
            String expcode;
            DataRow tmpDataRow;
            for (int i = 0; i < iCount; i++) {
                expcode = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드

                //종목이 기존 그리드에 존재여부에따라 row 추가 또는 수정 분기를 해준다.
                dataRowArray = mainForm.dataTable_t0424.Select("expcode = '"+ expcode +"'");
                if (dataRowArray.Length > 0){
                    tmpDataRow = dataRowArray[0];
                }else{
                    tmpDataRow = mainForm.dataTable_t0424.NewRow();
                }
          
                tmpDataRow["expcode"]  = base.GetFieldData("t0424OutBlock1", "expcode" , i); //종목코드
                tmpDataRow["hname"]    = base.GetFieldData("t0424OutBlock1", "hname"   , i); //종목명
                tmpDataRow["mdposqt"]  = base.GetFieldData("t0424OutBlock1", "mdposqt" , i); //매도가능
                tmpDataRow["price"]    = base.GetFieldData("t0424OutBlock1", "price"   , i); //현재가
                tmpDataRow["appamt"]   = base.GetFieldData("t0424OutBlock1", "appamt"  , i); //평가금액
                tmpDataRow["dtsunik"]  = base.GetFieldData("t0424OutBlock1", "dtsunik" , i); //평가손익
                tmpDataRow["sunikrt"]  = base.GetFieldData("t0424OutBlock1", "sunikrt" , i); //수익율
                tmpDataRow["pamt"]     = base.GetFieldData("t0424OutBlock1", "pamt"    , i); //평균단가
                tmpDataRow["mamt"]     = base.GetFieldData("t0424OutBlock1", "mamt"    , i); //매입금액
                tmpDataRow["msat"]     = base.GetFieldData("t0424OutBlock1", "msat"    , i); //당일매수금액
                //newrow["mpms"]     = base.GetFieldData("t0424OutBlock1", "mpms"  , i); //당일매수단가
                tmpDataRow["mdat"]     = base.GetFieldData("t0424OutBlock1", "mdat"    , i); //당일매도금액
                //newrow["mpmd"]     = base.GetFieldData("t0424OutBlock1", "mpmd"  , i); //당일매도단가
                tmpDataRow["fee"]      = base.GetFieldData("t0424OutBlock1", "fee"     , i); //수수료
                tmpDataRow["tax"]      = base.GetFieldData("t0424OutBlock1", "tax"     , i); //제세금
                tmpDataRow["sininter"] = base.GetFieldData("t0424OutBlock1", "sininter", i); //신용이자

                //1.그리드에 없던 새로 매수된 종목이면 테이블에 추가해준다.
                if (dataRowArray.Length == 0){
                    mainForm.dataTable_t0424.Rows.Add(tmpDataRow);
                }
                //if ((i % 2) == 0)
                // {
                //     tmpDataRow["hname"] = "xxx";
                //}
                //MessageBox.Show("2");
                //mainForm.dataTable_t0424.AcceptChanges();
                //Thread.Sleep(100);
            }


            // 계좌정보 써머리 계산 - 연속 조회이기때문에 합산후 마지막에 폼으로 출력.
            this.mamt     += int.Parse(base.GetFieldData("t0424OutBlock", "mamt"    , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "mamt"    , 0));//매입금액
            this.tappamt  += int.Parse(base.GetFieldData("t0424OutBlock", "tappamt" , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tappamt" , 0));//평가금액
            this.tdtsunik += int.Parse(base.GetFieldData("t0424OutBlock", "tdtsunik", 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tdtsunik", 0));//평가손익

            this.h_totalCount += iCount;

            //2.연속 데이타 정보가 남아있는지 구분
            if (base.IsNext) 
            //if (cts_expcode != "")
            {
                //연속 데이타 정보를 호출.
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, cts_expcode);      // CTS종목번호 : 처음 조회시는 SPACE
                base.Request(true); //연속조회일경우 true
                //mainForm.input_t0424_log.Text = "[연속조회]잔고조회를 요청을 하였습니다.";
            } else {//마지막 데이타일때 메인폼에 출력해준다.

                mainForm.input_mamt.Text     = Util.GetNumberFormat(this.mamt);    // 매입금액
                mainForm.input_tappamt.Text  = Util.GetNumberFormat(this.tappamt); // 평가금액
                mainForm.input_tdtsunik.Text = Util.GetNumberFormat(this.tdtsunik);// 평가손익

                String sunamt1 = this.GetFieldData("t0424OutBlock", "sunamt1", 0);// D1예수금
                String dtsunik = this.GetFieldData("t0424OutBlock", "dtsunik", 0);// 실현손익
                mainForm.input_sunamt1.Text = Util.GetNumberFormat(sunamt1);      // D1예수금
                mainForm.input_dtsunik.Text = Util.GetNumberFormat(dtsunik);      // 실현손익
                mainForm.input_sunamt.Text  = Util.GetNumberFormat((int.Parse(sunamt1) + this.tappamt).ToString()); // 추정순자산 - sunamt 값이 이상해서  추정순자산 = 평가금액 + D1예수금 
                mainForm.h_totalCount.Text  = this.h_totalCount.ToString();       //종목수

                //로그 및 중복 요청 처리
                //mainForm.input_t0424_log.Text = "잔고조회 요청을 완료 하였습니다.";
                completeAt = true;   
            }

           
        }//end

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
                if (nMessageCode == "00000") {
                    ;
                }else {
               
                Log.WriteLine("t0424 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지

                }
            
            


        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw )
        {

            if (completeAt) {
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
                this.h_totalCount = 0;//보유종목수

                base.Request(false);  //연속조회일경우 true
                completeAt = false;//중복호출 방지
                //폼 메세지.
                mainForm.input_t0424_log.Text = "잔고조회를 요청을 하였습니다.";

            } else {
                mainForm.input_t0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
            }


        }	// end function


    } //end class 


   
}   // end namespace
