
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
    //주식 체결/미체결
    public class Xing_t0425 : XAQueryClass {

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

    
        public int totalCount;
        // 생성자
        public Xing_t0425()
        {
            base.ResFileName = "₩res₩t0425.res";


            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

        }   // end function

        // 소멸자
        ~Xing_t0425()
        {

        }

        /// <summary>
		/// 주식잔고2(T0424) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode) {

            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("t0424OutBlock1");

            mainForm.grd_t1833.Rows.Clear();
            string[] row = new string[20];
            int addIndex;

            for (int i = 0; i < iCount; i++) {
                row[0]  = base.GetFieldData("t0425OutBlock1", "ordtime" , i); //주문시간
                row[1]  = base.GetFieldData("t0425OutBlock1", "medosu"   , i); //구분
                row[2]  = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                row[3]  = ""; //종목명
                row[4]  = base.GetFieldData("t0425OutBlock1", "qty"  , i); //주문수량
                row[5]  = base.GetFieldData("t0425OutBlock1", "price" , i); //주문가격
                row[6]  = base.GetFieldData("t0425OutBlock1", "cheqty" , i); //체결수량
                row[7]  = base.GetFieldData("t0425OutBlock1", "cheprice"    , i); //체결가격
                row[8]  = base.GetFieldData("t0425OutBlock1", "ordrem"    , i); //미체결잔량
                row[9]  = base.GetFieldData("t0425OutBlock1", "status"    , i); //상태

                //1.그리드 데이터 추가
                addIndex = mainForm.grd_t1833.Rows.Add(row);
               
                
            }

            String cts_ordno = base.GetFieldData("t0425OutBlock", "cts_ordno", 0);//연속키
            //2.연속 데이타 정보가 남아있는지 구분
            //if (base.IsNext)
            if (cts_ordno != "")
            {
                //연속 데이타 정보를 호출.
                base.SetFieldData("t0425InBlock", "cts_ordno", 0, cts_ordno);      //처음 조회시는 SPACE
                base.Request(true); //연속조회일경우 true
                //mainForm.input_t0424_log.Text = "[연속조회]잔고조회를 요청을 하였습니다.";
            } else {//마지막 데이타일때 메인폼에 출력해준다.
                completeAt = true;   
            }

           
        }//end

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
            if (nMessageCode == "00000") {
                ;
            }else {
                Log.WriteLine("t0424 :: " + nMessageCode + " :: " + szMessage);
                completeAt = true;//중복호출 방지
            }
            mainForm.input_t0425_log.Text = nMessageCode + " :: " + szMessage;

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw )
        {

            if (completeAt) {
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("t0425InBlock", "accno" ,    0, account);    // 계좌번호
                base.SetFieldData("t0425InBlock", "passwd",    0, accountPw);  // 비밀번호
                base.SetFieldData("t0425InBlock", "expcode",   0, "1");        //종목번호
                base.SetFieldData("t0425InBlock", "chegb" ,    0, "0");        // 체결구분 - 0:전체|1:체결|2|미체결
                base.SetFieldData("t0425InBlock", "medosu" ,   0, "0");        // 매매구분 - 0:전체|1:매수|2|매도  
                base.SetFieldData("t0425InBlock", "sortgb",    0, "1");        // 정렬순서
                base.SetFieldData("t0425InBlock", "cts_ordno", 0, "");         // 주문번호

                // 계좌잔고 그리드 초기화
            

                //멤버변수 초기화
                base.Request(false);  //연속조회일경우 true
                completeAt = false;//중복호출 방지
                //폼 메세지.
                mainForm.input_t0424_log.Text = "체결/미체결 요청을 하였습니다.";

            } else {
                mainForm.input_t0424_log.Text = "[중복]요청을 하였습니다.";
            }


        }	// end function


    } //end class 


   
}   // end namespace
