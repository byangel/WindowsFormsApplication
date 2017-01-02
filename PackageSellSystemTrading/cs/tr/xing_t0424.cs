
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
    public class Xing_t0424 : XAQueryClass{

        private Boolean completeFlag;//실행 가능여부.
        public MainForm mainForm;

        public int mamt;      //매입금액
        public int tappamt;   //평가금액
        public int tdtsunik;  //평가손익
        //public int sunamt;    //추정자산
        public int h_totalCount;

        public int testCount = 0;
        // 생성자
        public Xing_t0424()
        {
            base.ResFileName = "₩res₩t0424.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            completeFlag = true;
        }   // end function

        // 소멸자
        ~Xing_t0424()
        {
          
        }


        /// <summary>
		/// 주식잔고2(T0424) 수신 처리부
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            testCount += 1;
            Log.WriteLine("t0424 dataEventHandler call:: " + testCount);  

            String cts_expcode = base.GetFieldData("t0424OutBlock", "cts_expcode", 0);//CTS_종목번호-연속조회키

           
            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("t0424OutBlock1");
            string[] row = new string[17];
            int addIndex;
            for (int i = 0; i < iCount; i++) {
                //row[0] = "false"; //판매 선택
                row[1]  = base.GetFieldData("t0424OutBlock1", "expcode", i); //코드
                row[2]  = base.GetFieldData("t0424OutBlock1", "hname"   , i); //종목명
                row[3]  = base.GetFieldData("t0424OutBlock1", "mdposqt" , i); //매도가능
                row[4]  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "price"   , i)); //현재가
                row[5]  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "appamt"  , i)); //평가금액
                row[6]  = base.GetFieldData("t0424OutBlock1", "dtsunik" , i); //평가손익
                row[7]  = base.GetFieldData("t0424OutBlock1", "sunikrt" , i); //수익율
                row[8]  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "pamt"    , i)); //평균단가
                row[9]  = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mamt"    , i)); //매입금액
                row[10] = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "msat"    , i)); //당일매수금액
                row[11] = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mpms"    , i)); //당일매수단가
                row[12] = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mdat"    , i)); //당일매도금액
                row[13] = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mpmd"    , i)); //당일매도단가
                row[14] = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "fee"     , i)); //수수료
                row[15] = base.GetFieldData("t0424OutBlock1", "tax"     , i); //제세금
                row[16] = base.GetFieldData("t0424OutBlock1", "sininter", i); //신용이자

                //1.그리드 데이터 추가
                addIndex = mainForm.grd_accoun0424.Rows.Add(row);
                //mainForm.grd_accoun0424.Rows[addIndex].HeaderCell.Value = addIndex;

            }


            // 계좌정보 써머리 계산 
            //String tmpSunamt   = base.GetFieldData("t0424OutBlock", "sunamt"  , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "sunamt", 0);
            String tmpMamt     = base.GetFieldData("t0424OutBlock", "mamt"    , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "mamt"    , 0);
            String tmpTappamt  = base.GetFieldData("t0424OutBlock", "tappamt" , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tappamt" , 0);
            String tmpTdtsunik = base.GetFieldData("t0424OutBlock", "tdtsunik", 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tdtsunik", 0);
            //this.sunamt    += int.Parse(tmpSunamt);  //추정순자산
            this.mamt      += int.Parse(tmpMamt);    //매입금액
            this.tappamt   += int.Parse(tmpTappamt); //평가금액
            this.tdtsunik  += int.Parse(tmpTdtsunik);//평가손익

            this.h_totalCount += iCount;

            //2.연속 데이타 정보가 남아있는지 구분
            if (cts_expcode != "")
            {
                //연속 데이타 정보를 호출.
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, cts_expcode);      // CTS종목번호 : 처음 조회시는 SPACE
                base.Request(true); //연속조회일경우 true
                mainForm.input_accoun0424_log.Text = "[연속조회]잔고조회를 요청을 하였습니다.";
            }else{//마지막 데이타일때 메인폼에 출력해준다.
                //mainForm.input_sunamt.Text   = Util.GetNumberFormat(this.sunamt);  // 추정순자산
                mainForm.input_mamt.Text     = Util.GetNumberFormat(this.mamt);    // 매입금액
                mainForm.input_tappamt.Text  = Util.GetNumberFormat(this.tappamt); // 평가금액
                mainForm.input_tdtsunik.Text = Util.GetNumberFormat(this.tdtsunik);// 평가손익
                
                mainForm.input_dtsunik.Text = Util.GetNumberFormat(this.GetFieldData("t0424OutBlock", "dtsunik", 0)); // 실현손익
                mainForm.input_sunamt1.Text = Util.GetNumberFormat(this.GetFieldData("t0424OutBlock", "sunamt1", 0)); // 추정D2예수금
                mainForm.input_sunamt.Text  = Util.GetNumberFormat(this.GetFieldData("t0424OutBlock", "sunamt", 0));  // 추정순자산

                mainForm.h_totalCount.Text = this.h_totalCount.ToString();

                mainForm.input_accoun0424_log.Text = "잔고조회 요청을 완료 하였습니다.";
                completeFlag = true;
            }

           
        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
            try{
                if (nMessageCode == "00000") {
                    ;
                }
                else{
                    Log.WriteLine("t0424 :: " + nMessageCode + " :: " + szMessage);
                    mainForm.input_accoun0424_log.Text = nMessageCode + " :: " + szMessage;
                    completeFlag = true;//중복호출 방지
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
            
            if (completeFlag) {
                String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("t0424InBlock", "accno", 0, account);    // 계좌번호
                base.SetFieldData("t0424InBlock", "passwd", 0, accountPw);  // 비밀번호
                base.SetFieldData("t0424InBlock", "prcgb", 0, "1");        // 단가구분 : 1-평균단가, 2:BEP단가
                base.SetFieldData("t0424InBlock", "chegb", 0, "2");        // 체결구분 : 0-결제기준, 2-체결기준
                base.SetFieldData("t0424InBlock", "dangb", 0, "0");        // 단일가구분 : 0-정규장, 1-시간외단일가 
                base.SetFieldData("t0424InBlock", "charge", 0, "1");        // 제비용포함여부 : 0-미포함, 1-포함
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, "");      // CTS종목번호 : 처음 조회시는 SPACE

                if (accountPw == "" || account == "") {
                    MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
                }else{
                    // 계좌잔고 그리드 초기화
                    mainForm.grd_accoun0424.Rows.Clear();

                    //멤버변수 초기화
                    //this.sunamt       = 0;  //추정순자산
                    this.mamt = 0;  //매입금액
                    this.tappamt = 0;  //평가금액
                    this.tdtsunik = 0;  //평가손익
                    this.h_totalCount = 0;  //보유종목수

                    base.Request(false);  //연속조회일경우 true
                    completeFlag = false;//중복호출 방지
                    //폼 메세지.
                    mainForm.input_accoun0424_log.Text = "잔고조회를 요청을 하였습니다.";
                    
                }

            } else {
                mainForm.input_accoun0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
