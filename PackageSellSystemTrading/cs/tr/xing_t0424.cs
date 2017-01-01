
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
    public class Xing_t0424 : XAQueryClass{


        public MainForm mainForm;
        // 생성자
        public Xing_t0424()
        {
            base.ResFileName = "₩res₩t0424.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_t0424()
        {
          
        }


        /// <summary>
		/// 미체결내역 조회 관련(T0434) 수신 처리부
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode)
        {


            // 계좌정보 써머리
            mForm.TextSunamt.Text   = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock", "sunamt"  , 0)); // 추정순자산
            mForm.TextDtsunik.Text  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock", "dtsunik" , 0)); // 실현손익
            mForm.TextMamt.Text     = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock", "mamt"    , 0)); // 매입금액
            mForm.TextSunamt1.Text  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock", "sunamt1" , 0)); // 추정D2예수금
            mForm.TextTappamt.Text  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock", "tappamt" , 0)); // 평가금액
            mForm.TextTdtsunik.Text = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock", "tdtsunik", 0)); // 평가손익



            int iCount = base.GetBlockCount("t0424OutBlock1");
            string[] row = new string[17];
            int addIndex;
            for (int i = 0; i < iCount; i++) {
                row[1] = base.GetFieldData("t0424OutBlock1", "sellCheck", i); //판매 선택
                row[1] = base.GetFieldData("t0424OutBlock1", "expcode"  , i); //코드
                row[1] = base.GetFieldData("t0424OutBlock1", "exhname"  , i); //종목명
                row[1] = base.GetFieldData("t0424OutBlock1", "mdposqt"  , i); //매도가능
                row[1] = base.GetFieldData("t0424OutBlock1", "price"    , i); //현재가
                row[1] = base.GetFieldData("t0424OutBlock1", "appamt"   , i); //평가금액
                row[1] = base.GetFieldData("t0424OutBlock1", "dtsunik"  , i); //평가손익
                row[1] = base.GetFieldData("t0424OutBlock1", "sunikrt"  , i); //수익율
                row[1] = base.GetFieldData("t0424OutBlock1", "pamt"     , i); //평균단가
                row[1] = base.GetFieldData("t0424OutBlock1", "mamt"     , i); //매입금액
                row[1] = base.GetFieldData("t0424OutBlock1", "msat"     , i); //당일매수금액
                row[1] = base.GetFieldData("t0424OutBlock1", "mpms"     , i); //당일매수단가
                row[1] = base.GetFieldData("t0424OutBlock1", "mdat"     , i); //당일매도금액
                row[1] = base.GetFieldData("t0424OutBlock1", "mpmd"     , i); //당일매도단가
                row[1] = base.GetFieldData("t0424OutBlock1", "fee"      , i); //수수료
                row[1] = base.GetFieldData("t0424OutBlock1", "tax"      , i); //제세금
                row[1] = base.GetFieldData("t0424OutBlock1", "sininter" , i); //신용이자

                //1.그리드 데이터 추가
                addIndex = mainForm.grd_accoun0424.Rows.Add(row);

            }
      
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
            try{
                if (nMessageCode == "00000") {
                    ;
                }
                else{
                    Log.WriteLine("t0424 :: " + nMessageCode + " :: " + szMessage);
                }
            }catch (Exception ex){
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request()
        {
            // 응답 결과를 아직 실행중이라면
            /*if (mStateRun == true) {
                mStateRunCount++;

                // 2초 정도는 기다려 줌
                if (mStateRunCount > 2){
                    mStateRun      = false;
                    mStateRunCount = 0;
                }
            }*/

            String account = mainForm.comBox_account.SelectedText;
            String accountPw = mainForm.input_accountPw.Text;
           
            base.SetFieldData("t0424InBlock", "accno"   , 0, account);    // 계좌번호
            base.SetFieldData("t0424InBlock", "passwd"  , 0, accountPw);  // 비밀번호
            base.SetFieldData("t0424InBlock", "prcgb"   , 0, "1");        // 단가구분 : 1-평균단가, 2:BEP단가
            base.SetFieldData("t0424InBlock", "chegb"   , 0, "2");        // 체결구분 : 0-결제기준, 2-체결기준
            base.SetFieldData("t0424InBlock", "dangb"   , 0, "0");        // 단일가구분 : 0-정규장, 1-시간외단일가 
            base.SetFieldData("t0424InBlock", "charge"  , 0, "1");        // 제비용포함여부 : 0-미포함, 1-포함
            base.SetFieldData("t0424InBlock", "cts_expcode", 0, "");      // CTS종목번호 : 처음 조회시는 SPACE

            if (accountPw == "") {
                MessageBox.Show("계좌 비밀번호가 없습니다.");
            }
            else {
                base.Request(false);
            }
            
        }	// end function


    } //end class 
   
}   // end namespace
