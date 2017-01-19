
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

        public Boolean completeAt = true;//완료여부.
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
		/// 주식 체결/미체결(T0425) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode) {

            String time = mainForm.xing_t0167.time;
            //if (time == "" ) { time = "1530"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60) + int.Parse(time.Substring(2, 2));//현재 시간

            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("t0425OutBlock1");

            mainForm.grd_t0425_chegb1.Rows.Clear();
            mainForm.grd_t0425_chegb2.Rows.Clear();
            string[] row = new string[10];
            int addIndex;
            String ordrem;    //미체결잔량
            String medosu;    //매매구분
            String cheqty;    //체결수량
            String ordno; //주문번호
            int chegb1Cnt = 0;
            int chegb2Cnt = 0;
            for (int i = 0; i < iCount; i++) {

                row[0] = base.GetFieldData("t0425OutBlock1", "ordtime" , i); //주문시간
                row[1] = base.GetFieldData("t0425OutBlock1", "medosu"  , i); //매매구분 - 0:전체|1:매수|2:매도
                row[2] = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                row[3] = ""; //종목명
                row[4] = base.GetFieldData("t0425OutBlock1", "qty"     , i); //주문수량
                row[5] = base.GetFieldData("t0425OutBlock1", "price"   , i); //주문가격
                row[6] = base.GetFieldData("t0425OutBlock1", "cheqty"  , i); //체결수량
                row[7] = base.GetFieldData("t0425OutBlock1", "cheprice", i); //체결가격
                row[8] = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결잔량
                row[9] = base.GetFieldData("t0425OutBlock1", "status"  , i); //상태

                ordrem = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결 잔량 - 매도또는 매수 주문후  잔량이 있다면 걔좌에 종목이 있다는뜻이므로 미체결 목록에 뿌려준다.
                ordno  = base.GetFieldData("t0425OutBlock1", "ordno"   , i); //주문번호
                //미체결목록 -- 미체결 잔량이 있다면...
                if (int.Parse(ordrem) > 0){
                    //1.그리드 데이터 추가
                    addIndex = mainForm.grd_t0425_chegb2.Rows.Add(row);
                    chegb2Cnt++;

                    //미체결 시간이 1분 이상이면 취소주문 한다.
                   
                    int tmpTime = (int.Parse(row[0].Substring(0, 2)) * 60) + int.Parse(row[0].Substring(2, 2));//현재 시간
                    if ((cTime - tmpTime) > 1)
                    {
                        /// <summary>
                        /// 현물 취소 주문
                        /// </summary>
                        /// <param name="OrgOrdNo">원주문번호</param>
                        /// <param name="IsuNo">종목번호</param>
                        /// <param name="OrdQty">주문수량</param>
                        mainForm.xing_CSPAT00800.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw, ordno, row[2], "");
                        Log.WriteLine("[" + mainForm.input_time.Text + "]t0425 ::[" + row[2] + "]  " + ordno + " 취소주문");
                    }
                }

                //매수 체결 내역 그리드 추가 -체결수량이 있다면.
                medosu = base.GetFieldData("t0425OutBlock1", "medosu", i);//매매구분
                cheqty = base.GetFieldData("t0425OutBlock1", "cheqty", i);//체결수량
                //if (medosu == "매수" && int.Parse(cheqty) > 0){
                if (int.Parse(cheqty) > 0){
                    addIndex = mainForm.grd_t0425_chegb1.Rows.Add(row); // 체결구분 - 0:전체|1:체결|2|미체결
                    chegb1Cnt++;
                }

            }
            //매수체결 목록
            mainForm.grd_t0425_chegb1_cnt.Text = chegb1Cnt.ToString();

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
                //Thread.Sleep(5000);
                completeAt = true;
                mainForm.input_t0425_log2.Text = "[" + mainForm.input_time.Text + "]t0425 :: 채결/미채결 요청완료";
            }
            

        }//end

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
            if (nMessageCode == "00000") {
               ;
            }else {
                //Thread.Sleep(3000);
                completeAt = true;//중복호출 방지
                Log.WriteLine("[" + mainForm.input_time.Text + "]t0425 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t0425_log2.Text = "[" + mainForm.input_time.Text + "]t0425 :: " + nMessageCode + " :: " + szMessage;
               
            }

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw )
        {

            if (completeAt) {
                completeAt = false;//중복호출 방지
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("t0425InBlock", "accno" ,    0, account);    // 계좌번호
                base.SetFieldData("t0425InBlock", "passwd",    0, accountPw);  // 비밀번호
                base.SetFieldData("t0425InBlock", "expcode",   0, "");         // 종목번호
                base.SetFieldData("t0425InBlock", "chegb" ,    0, "0");        // 체결구분 - 0:전체|1:체결|2|미체결
                base.SetFieldData("t0425InBlock", "medosu" ,   0, "0");        // 매매구분 - 0:전체|1:매수|2:매도  
                base.SetFieldData("t0425InBlock", "sortgb",    0, "1");        // 정렬순서
                base.SetFieldData("t0425InBlock", "cts_ordno", 0, "");         // 주문번호

                // 계좌잔고 그리드 초기화

                //멤버변수 초기화
                base.Request(false);  //연속조회일경우 true
                
                //폼 메세지.
                mainForm.input_t0425_log.Text = "[" + mainForm.input_time.Text + "]t0425::체결/미체결 요청";

            } else {
                mainForm.input_t0425_log.Text = "[" + mainForm.input_time.Text + "][중복]t0425::체결/미체결 요청";
            }
        }	// end function
    } //end class   
}   // end namespace
