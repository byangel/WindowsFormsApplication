
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

        int chegb1Cnt;
       

        public int totalCount;

        public Boolean readyAt;
        // 생성자
        public Xing_t0425()
        {
            readyAt = false;

            base.ResFileName = "₩res₩t0425.res";


            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
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

            //응답 데이타 수
            int blockCount = base.GetBlockCount("t0425OutBlock1");


            //현재시간.
            String time = mainForm.xing_t0167.time;
            if (time == "" || time == null) { time = "1530"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60) + int.Parse(time.Substring(2, 2));//현재 시간

            //체결 목록
            EBindingList<T0425Vo> t0425VoListChegb1 = ((EBindingList<T0425Vo>)mainForm.grd_t0425_chegb1.DataSource);
            //미체결 목록
            //EBindingList<T0425Vo> t0425VoList_Chegb2 = ((EBindingList<T0425Vo>)mainForm.grd_t0425_chegb2.DataSource);

            //계좌잔고목록
            EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();

            int    t0424_price;   //현재가
            String t0424_mdposqt; //매도가능 수량
            String t0424_hname;   //종목명
            String t0424_sunikrt;  //수익율

            String toDaySellAmt;
            String ordno;  //주문번호
            String status; //상태 체결|미체결
            String qty;    //주문수량
            String cheqty; //체결수량
            T0425Vo tmpT0425Vo;
            for (int i = 0; i < blockCount; i++)
            {
                ordno  = base.GetFieldData("t0425OutBlock1", "ordno" , i); //주문번호
                status = base.GetFieldData("t0425OutBlock1", "status", i); //상태
                qty    = base.GetFieldData("t0425OutBlock1", "qty"   , i); //주문수량
                cheqty = base.GetFieldData("t0425OutBlock1", "cheqty", i); //체결수량
               
                var resultT0425 = from item in t0425VoListChegb1
                                    where item.ordno == ordno
                                    select item;
                if (resultT0425.Count() > 0)
                {
                    tmpT0425Vo = resultT0425.ElementAt(0);
                }
                else
                {
                    tmpT0425Vo = new T0425Vo();

                }

                tmpT0425Vo.ordtime  = base.GetFieldData("t0425OutBlock1", "ordtime" , i); //주문시간
                tmpT0425Vo.medosu   = base.GetFieldData("t0425OutBlock1", "medosu"  , i); //매매구분 - 0:전체|1:매수|2:매도
                tmpT0425Vo.expcode  = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                tmpT0425Vo.hname    =  ""; //종목명
                tmpT0425Vo.qty      = base.GetFieldData("t0425OutBlock1", "qty"     , i); //주문수량
                tmpT0425Vo.price    = base.GetFieldData("t0425OutBlock1", "price"   , i); //주문가격
                tmpT0425Vo.cheqty   = base.GetFieldData("t0425OutBlock1", "cheqty"  , i); //체결수량
                tmpT0425Vo.cheprice = base.GetFieldData("t0425OutBlock1", "cheprice", i); //체결가격
                tmpT0425Vo.ordrem   = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결잔량
                tmpT0425Vo.status   = base.GetFieldData("t0425OutBlock1", "status"  , i); //상태
                tmpT0425Vo.ordno    = base.GetFieldData("t0425OutBlock1", "ordno"   , i); //주문번호
                tmpT0425Vo.orgordno = base.GetFieldData("t0425OutBlock1", "orgordno", i); //원주문번호

                if (resultT0425.Count() == 0)
                {
                    t0425VoListChegb1.Insert(0, tmpT0425Vo);
                    chegb1Cnt++;
                }

                //1.미체결목록 -- 미체결 잔량이 있다면...매도또는 매수 주문후  잔량이 있다면 걔좌에 종목이 있다는뜻이므로 미체결 목록에 뿌려준다.
                //한주라도 체결되면 체결로 뜨기때문에 미체결에도 뿌려줘야할것같다
               
                if (status == "미체결")
                //if (int.Parse(cheqty) < int.Parse(qty))
                {

                    //미체결 시간이 1분 이상이면 취소주문 한다.
                    int tmpTime = (int.Parse(tmpT0425Vo.ordtime.Substring(0, 2)) * 60) + int.Parse(tmpT0425Vo.ordtime.Substring(2, 2));//현재 시간
                    if ((cTime - tmpTime) > 1)
                    {
                        /// <summary>
                        /// 현물 취소 주문
                        /// </summary>
                        /// <param name="OrgOrdNo">원주문번호</param>
                        /// <param name="IsuNo">종목번호</param>
                        /// <param name="OrdQty">주문수량</param>
                        mainForm.xing_CSPAT00800.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw, tmpT0425Vo.ordno, tmpT0425Vo.expcode, "");
                        Log.WriteLine("t0425::" + tmpT0425Vo.hname + "(" + tmpT0425Vo.expcode + ")::취소주문 [주문번호:" + tmpT0425Vo.ordno+"]");
                    }
                }

                //최종 반복매수할때 매도된 종목인데 아직 뿌려지지않아서 매수여부 판단시 오류발생한다. 최초 한번은 목록을 가져온후 그다음부터 반복매수 해준다.
                if (readyAt)
                {
                    //3.금일매수 금일매도 - 매수 && 미체결잔량 == 0  한하여 금일 매도 를 해주자.
                    if (Properties.Settings.Default.TODAY_SELL_AT)
                    {
                        if (tmpT0425Vo.medosu == "매수" && int.Parse(tmpT0425Vo.ordrem) == 0)
                        {
                            //계좌잔고 그리드에서 해당종목 정보 참조.
                            var resultT0424 = from item in t0424VoList
                                                where item.expcode == tmpT0425Vo.expcode
                                                select item;

                            if (resultT0424.Count() > 0)
                            {
                                //당일매수수량 - 당일매도수량 = 금일매도가능수량

                                t0424_price = int.Parse(resultT0424.ElementAt(0).price); //현재가
                                t0424_mdposqt = resultT0424.ElementAt(0).mdposqt == "" ? "0" : resultT0424.ElementAt(0).mdposqt;  //매도가능 수량
                                t0424_sunikrt = resultT0424.ElementAt(0).sunikrt;//수익률
                                t0424_hname = resultT0424.ElementAt(0).hname;//종목명

                                //1.매도가능수량 > 주문수량  ->체결수량과 매도가능수량이 같으면 신규매수겠지? 여기는 반복매수만 처리해준다. --체결수량으로 하고싶지만...
                                if (int.Parse(t0424_mdposqt) > int.Parse(tmpT0425Vo.qty))
                                {
                                    //1.현재가가 금일매수 값보다 2%이상 올랐으면 금일 매수 수량만큼 매도한다.
                                    Double late = ((t0424_price / Double.Parse(tmpT0425Vo.cheprice)) * 100) - 100;
                                    late = Math.Round(late, 2);

                                    if (late > float.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET))
                                    {
                                        //해당종목 매도 이력이 없으면 매수 한다.
                                        var result_t0425Vo = from t0425VoChegb1 in t0425VoListChegb1
                                                                where t0425VoChegb1.expcode == tmpT0425Vo.expcode
                                                                        && t0425VoChegb1.medosu == "매도"
                                                                        && t0425VoChegb1.qty == tmpT0425Vo.qty//주문수량   
                                                                select t0425VoChegb1;
                                        //매도 이력이 없다면 또는 매도체크 여부가 Y가 아니라면 매도해주자.-
                                        //빠른게 2번 매도가 이루어지는문제가 있다 그래서 매도하면 매도했다고 체크를 하는데 문제는 프로그램 재시작시 체크 정보가 사라진다 그래서 매도이력이 있는지도 체크해준다.
                                        if (result_t0425Vo.Count() == 0 && tmpT0425Vo.todaySellAt == false)
                                        {

                                            int tmpAmt = ((t0424_price - int.Parse(tmpT0425Vo.cheprice)) * int.Parse(tmpT0425Vo.qty));

                                            //당일매도 차익 합산.
                                            toDaySellAmt = (int.Parse(mainForm.input_toDayAtm.Text == "" ? "0" : mainForm.input_toDayAtm.Text) + tmpAmt).ToString();

                                            String msg = "t0425::" + tmpT0425Vo.hname + "(" + tmpT0425Vo.expcode + ")::금일매수/매도 [주문가격:" + t0424_price + "|주문수량:" + tmpT0425Vo.qty + "|금일수익율:" + late + "|차익:" + tmpAmt + "|매도가능수량:" + t0424_mdposqt + "|매도전수익률:" + t0424_sunikrt + "]";

                                            /// <param name="IsuNo">종목번호</param>
                                            /// <param name="Quantity">수량</param>
                                            /// <param name="Price">가격</param>
                                            /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                                            mainForm.xing_CSPAT00600.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw, msg, tmpT0425Vo.expcode, tmpT0425Vo.qty, t0424_price.ToString(), "1");
                                            tmpT0425Vo.todaySellAt = true;
                                            //당일매도 차익 합산.
                                            mainForm.input_toDayAtm.Text = toDaySellAmt;
                                        }

                                    }

                                }

                            }

                        }
                    }//금일매도매수 end

                    
                }

           

            }//for end


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
                readyAt = true;
                //매수체결 목록
                //mainForm.grd_t0425_chegb1_cnt.Text = chegb1Cnt.ToString();
                mainForm.grd_t0425_chegb1_cnt.Text = t0425VoListChegb1.Count().ToString();
                //Thread.Sleep(5000);
                //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
                mainForm.input_t0425_log2.Text = "[" + mainForm.input_time.Text + "]t0425 :: 채결/미채결 요청완료";
                
            }


        }//end

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {

            if (nMessageCode == "00000") {
                
            //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
            } else {
                    //Thread.Sleep(3000);
                    completeAt = true;//중복호출 방지
                    Log.WriteLine("[" + mainForm.input_time.Text + "]t0425 :: " + nMessageCode + " :: " + szMessage);
                    mainForm.input_t0425_log2.Text = "[" + mainForm.input_time.Text + "]t0425 :: " + nMessageCode + " :: " + szMessage;

                }

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw)
        {

            if (completeAt) {
                completeAt = false;//중복호출 방지

                this.chegb1Cnt = 0;
         

                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("t0425InBlock", "accno"    , 0, account);    // 계좌번호
                base.SetFieldData("t0425InBlock", "passwd"   , 0, accountPw);  // 비밀번호
                base.SetFieldData("t0425InBlock", "expcode"  , 0, "");         // 종목번호
                base.SetFieldData("t0425InBlock", "chegb"    , 0, "0");        // 체결구분 - 0:전체|1:체결|2|미체결
                base.SetFieldData("t0425InBlock", "medosu"   , 0, "0");        // 매매구분 - 0:전체|1:매수|2:매도  
                base.SetFieldData("t0425InBlock", "sortgb"   , 0, "1");        // 정렬순서
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

    public class T0425Vo {
        public String ordtime  { set; get; } //주문시간
        public String medosu   { set; get; } //매매구분 - 0:전체|1:매수|2:매도
        public String expcode  { set; get; } //종목번호
        public String hname    { set; get; } //종목명
        public String qty      { set; get; } //주문수량
        public String price    { set; get; } //주문가격
        public String cheqty   { set; get; } //체결수량
        public String cheprice { set; get; } //체결가격
        public String ordrem   { set; get; } //미체결잔량
        public String status   { set; get; } //상태
        public String ordno    { set; get; } //주문번호
        public String orgordno { set; get; } //원주문번호
        public Boolean todaySellAt { set; get; } //금일매도여부
    }
}   // end namespace
