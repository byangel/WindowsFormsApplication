
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
    //현물계좌 잔고내역 조회(API)
    public class Xing_CSPAQ12300 : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public int mamt;      //매입금액
        public int tappamt;   //평가금액
        public int tdtsunik;  //평가손익
        //public int sunamt;    //추정자산
        public int h_totalCount;

        public int testCount = 0;
        // 생성자
        public Xing_CSPAQ12300()
        {
            base.ResFileName = "₩res₩CSPAQ12300.res";

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
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
            

            testCount += 1;
            Log.WriteLine("CSPAQ12300O dataEventHandler call:: " + testCount);  

            //String cts_expcode = base.GetFieldData("t0424OutBlock", "cts_expcode", 0);//CTS_종목번호-연속조회키

           
            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("CSPAQ12300OutBlock3");
            string[] row = new string[17];
            int addIndex;
            for (int i = 0; i < iCount; i++) {
                //row[0] = "false"; //판매 선택
                row[1]  = base.GetFieldData("CSPAQ12300OutBlock3", "IsuNo", i); //코드
                row[2]  = base.GetFieldData("CSPAQ12300OutBlock3", "IsuNm", i); //종목명
                row[3]  = base.GetFieldData("CSPAQ12300OutBlock3", "CrdayBuyExecAmt" , i); //매도가능
                //row[4]  = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "price"   , i)); //현재가
                //row[5]  = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "appamt"  , i)); //평가금액
                //row[6]  = base.GetFieldData("CSPAQ12300OutBlock3", "dtsunik" , i); //평가손익
                //row[7]  = base.GetFieldData("CSPAQ12300OutBlock3", "sunikrt" , i); //수익율
                //row[8]  = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "pamt"    , i)); //평균단가
                //row[9]  = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "mamt"    , i)); //매입금액
                //row[10] = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "msat"    , i)); //당일매수금액
                //row[11] = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "mpms"    , i)); //당일매수단가
                //row[12] = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "mdat"    , i)); //당일매도금액
                //row[13] = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "mpmd"    , i)); //당일매도단가
                //row[14] = Util.GetNumberFormat(base.GetFieldData("CSPAQ12300OutBlock3", "fee"     , i)); //수수료
                //row[15] = base.GetFieldData("CSPAQ12300OutBlock3", "tax"     , i); //제세금
                //row[16] = base.GetFieldData("CSPAQ12300OutBlock3", "sininter", i); //신용이자

                //1.그리드 데이터 추가
                addIndex = mainForm.grd_accoun0424.Rows.Add(row);  
            }


            // 계좌정보 써머리 계산 
            //String tmpSunamt   = base.GetFieldData("t0424OutBlock", "sunamt"  , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "sunamt", 0);
            //String tmpMamt     = base.GetFieldData("t0424OutBlock", "mamt"    , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "mamt"    , 0);
            //String tmpTappamt  = base.GetFieldData("t0424OutBlock", "tappamt" , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tappamt" , 0);
            //String tmpTdtsunik = base.GetFieldData("t0424OutBlock", "tdtsunik", 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tdtsunik", 0);
            //this.sunamt    += int.Parse(tmpSunamt);  //추정순자산
            //this.mamt      += int.Parse(tmpMamt);    //매입금액
            //this.tappamt   += int.Parse(tmpTappamt); //평가금액
            //this.tdtsunik  += int.Parse(tmpTdtsunik);//평가손익

            this.h_totalCount = iCount;

            String pchsAmt    = this.GetFieldData("CSPAQ12300OutBlock2", "PchsAmt"   , 0);//매입금액
            String balEvalAmt = this.GetFieldData("CSPAQ12300OutBlock2", "BalEvalAmt", 0);//평가금액
            String Dps        = this.GetFieldData("CSPAQ12300OutBlock2", "Dps"       , 0);//예수금
            String D2Dps      = this.GetFieldData("CSPAQ12300OutBlock2", "D2Dps", 0);     //D2예수금
            
            mainForm.input_Dps.Text     = Util.GetNumberFormat(Dps);          // 예수금
            mainForm.input_sunamt1.Text = Util.GetNumberFormat(D2Dps);        // D2예수금
            mainForm.input_mamt.Text     = Util.GetNumberFormat(pchsAmt);     // 매입금액 -제비용 미포함
            mainForm.input_tappamt.Text  = Util.GetNumberFormat(balEvalAmt);  // 평가금액 (잔고평가금액) -제비용 미포함

            mainForm.input_sunamt.Text   = Util.GetNumberFormat(int.Parse(Dps) + int.Parse(balEvalAmt));    // 추정순자산 (평가금액 + 예수금)
            mainForm.input_tdtsunik.Text = Util.GetNumberFormat(int.Parse(balEvalAmt) -int.Parse(pchsAmt) );// 평가손익  (평가금액 - 매입금액) 

            //mainForm.input_dtsunik.Text = Util.GetNumberFormat(this.GetFieldData("CSPAQ12300OutBlock2", "recCnt", 0)); // 실현손익
            mainForm.input_PnlRat.Text  = Util.GetNumberFormat(this.GetFieldData("CSPAQ12300OutBlock2", "PnlRat", 0));  // 손익율
            mainForm.input_Evrprc.Text = Util.GetNumberFormat(this.GetFieldData("CSPAQ12300OutBlock2", "Evrprc", 0));  // 제비용

            

            mainForm.h_totalCount.Text = this.h_totalCount.ToString();

            mainForm.input_t0424_log.Text = "잔고조회 요청을 완료 하였습니다.";
            completeAt = true;

        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
            try{
                if (nMessageCode == "00000") {
                    ;
                }
                else{
                    Log.WriteLine("t0424 :: " + nMessageCode + " :: " + szMessage);
                    mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                    completeAt = true;//중복호출 방지
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
            
            if (completeAt) {
                String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("CSPAQ12300InBlock1", "AcntNo"          , 0, account);  // 계좌번호
                base.SetFieldData("CSPAQ12300InBlock1", "Pwd"         , 0, accountPw);// 비밀번호
                base.SetFieldData("CSPAQ12300InBlock1", "RecCnt", 0, "100");// 레코드수
                base.SetFieldData("CSPAQ12300InBlock1", "BalCreTp"       , 0, "1");      // 잔고생성구분      - 0:전체 | 1:현물 | 9:선물대용
                base.SetFieldData("CSPAQ12300InBlock1", "CmsnAppTpCode"  , 0, "0");      // 수수료적용구분    - 0:평가시수수료미적용 | 1:평가시수수료적용 ?값을변경해도 무슨차이인지 모르겠음.
                base.SetFieldData("CSPAQ12300InBlock1", "D2balBaseQryTp" , 0, "0");      //D2잔고기준조회구분 - 0:전부조회 | 1:D2잔고0이상만조회
                base.SetFieldData("CSPAQ12300InBlock1", "UprcTpCode"     , 0, "1");      //단가구분          - 0:평균단가 | 1:BEP단가
              

                if (accountPw == "" || account == "") {
                    MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
                }else{
                    // 계좌잔고 그리드 초기화
                    mainForm.grd_accoun0424.Rows.Clear();

                    //멤버변수 초기화
                    //this.sunamt       = 0;  //추정순자산
                    this.mamt         = 0;  //매입금액
                    this.tappamt      = 0;  //평가금액
                    this.tdtsunik     = 0;  //평가손익
                    this.h_totalCount = 0;  //보유종목수

                    base.Request(false);  //연속조회일경우 true
                    completeAt = false;//중복호출 방지
                    //폼 메세지.
                    mainForm.input_t0424_log.Text = "잔고조회를 요청을 하였습니다.";
                    
                }

            } else {
                mainForm.input_t0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
