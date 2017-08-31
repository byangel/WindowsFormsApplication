
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
using System.Drawing;

namespace PackageSellSystemTrading {
    //주식 체결/미체결
    public class Xing_t0425 : XAQueryClass {

        private EBindingList<T0425Vo> t0425VoList;
        public EBindingList<T0425Vo> getT0425VoList()
        {
            return this.t0425VoList;
        }

        public Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public int totalCount;

        public Boolean initAt = false;

        // 생성자
        public Xing_t0425()
        {
            base.ResFileName = "₩res₩t0425.res";


            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            this.t0425VoList = new EBindingList<T0425Vo>();

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
            try
            {
                //응답 데이타 수
                int blockCount = base.GetBlockCount("t0425OutBlock1");
                if (blockCount == 0)
                {
                    this.t0425VoList.Clear();
                }

                //계좌잔고목록
                EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();

                String ordno;  //주문번호
                String status; //상태 체결|미체결
                String qty;    //주문수량
                String cheqty; //체결수량
                Double 체결가격;//금일체결가격
                Double 현재가;//0424 현재가
                Double todayLate;
                int    t0424FindIndex;
                int    findIndex;
                Double 매입금액;
                Double 매도금액;
                T0425Vo tmpT0425Vo;
                String medosu;//매매구분
                var blockData = base.GetBlockData("t0425OutBlock1");
                for (int i = 0; i < blockCount; i++)
                {
                    ordno  = base.GetFieldData("t0425OutBlock1", "ordno" , i); //주문번호
                    status = base.GetFieldData("t0425OutBlock1", "status", i); //상태
                    qty    = base.GetFieldData("t0425OutBlock1", "qty"   , i); //주문수량
                    cheqty = base.GetFieldData("t0425OutBlock1", "cheqty", i); //체결수량
                    medosu = base.GetFieldData("t0425OutBlock1", "medosu", i); //매매구분 - 0:전체|1:매수|2:매도
                    if (medosu.IndexOf("취소")>=0) {//취소주문건은 그리드에 출력하지 않는다.
                        continue;
                    }
                    findIndex = this.t0425VoList.Find("ordno", ordno);
                    if (findIndex >= 0)
                    {
                        tmpT0425Vo = this.t0425VoList.ElementAt(findIndex);

                        mainForm.grd_t0425.Rows[findIndex].Cells["ordtime"      ].Value = base.GetFieldData("t0425OutBlock1", "ordtime" , i); //주문시간
                        mainForm.grd_t0425.Rows[findIndex].Cells["medosu"       ].Value = base.GetFieldData("t0425OutBlock1", "medosu"  , i); //매매구분 - 0:전체|1:매수|2:매도
                        mainForm.grd_t0425.Rows[findIndex].Cells["expcode"      ].Value = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                        //mainForm.grd_t0425.Rows[findIndex].Cells["t0425_hname"].Value       = ""; //종목명
                        mainForm.grd_t0425.Rows[findIndex].Cells["qty"          ].Value = base.GetFieldData("t0425OutBlock1", "qty"     , i); //주문수량
                        mainForm.grd_t0425.Rows[findIndex].Cells["t0425_price"  ].Value = Util.GetNumberFormat(base.GetFieldData("t0425OutBlock1", "price", i)); //주문가격
                        mainForm.grd_t0425.Rows[findIndex].Cells["cheqty"       ].Value = base.GetFieldData("t0425OutBlock1", "cheqty"  , i); //체결수량
                        mainForm.grd_t0425.Rows[findIndex].Cells["cheprice"     ].Value = Util.GetNumberFormat(base.GetFieldData("t0425OutBlock1", "cheprice", i)); //체결가격
                        mainForm.grd_t0425.Rows[findIndex].Cells["ordrem"       ].Value = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결잔량
                        mainForm.grd_t0425.Rows[findIndex].Cells["status"       ].Value = base.GetFieldData("t0425OutBlock1", "status"  , i); //상태
                        mainForm.grd_t0425.Rows[findIndex].Cells["ordno"        ].Value = base.GetFieldData("t0425OutBlock1", "ordno"   , i); //주문번호
                        mainForm.grd_t0425.Rows[findIndex].Cells["ordermtd"     ].Value = base.GetFieldData("t0425OutBlock1", "ordermtd", i); //주문매체
                    }else{
                        tmpT0425Vo = new T0425Vo();
                        tmpT0425Vo.ordtime  = base.GetFieldData("t0425OutBlock1", "ordtime" , i); //주문시간
                        tmpT0425Vo.medosu   = base.GetFieldData("t0425OutBlock1", "medosu"  , i); //매매구분 - 0:전체|1:매수|2:매도
                        tmpT0425Vo.expcode  = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                        //tmpT0425Vo.hname    = ""; //종목명
                        tmpT0425Vo.qty      = base.GetFieldData("t0425OutBlock1", "qty"     , i); //주문수량
                        tmpT0425Vo.price    = Util.GetNumberFormat(base.GetFieldData("t0425OutBlock1", "price", i)); //주문가격
                        tmpT0425Vo.cheqty   = base.GetFieldData("t0425OutBlock1", "cheqty"  , i); //체결수량
                        tmpT0425Vo.cheprice = Util.GetNumberFormat(base.GetFieldData("t0425OutBlock1", "cheprice", i)); //체결가격
                        tmpT0425Vo.ordrem   = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결잔량
                        tmpT0425Vo.status   = base.GetFieldData("t0425OutBlock1", "status"  , i); //상태
                        tmpT0425Vo.ordno    = base.GetFieldData("t0425OutBlock1", "ordno"   , i); //주문번호
                        tmpT0425Vo.ordermtd = base.GetFieldData("t0425OutBlock1", "ordermtd", i); //주문매체

                        //목록추가
                        this.t0425VoList.Add(tmpT0425Vo);
                        //findIndex = this.t0425VoList.Count()-1;
                      
                    }

                    var items = from item in mainForm.tradingHistory.getTradingHistoryVoList()
                                where item.accno == mainForm.account
                                   && item.Isuno == tmpT0425Vo.expcode
                                   && item.ordno == tmpT0425Vo.ordno
                                select item;

                   
                    //2.DB에 해당 주문 정보가 없을때 처리해줘야한다. volist로 변경후 테스트한후에 필요하면 처리로직 추가해주자.
                    foreach (TradingHistoryVo item in items){
          
                        /////////프로그램 재시작하는동안 체결된 정보는 DB에 저장이 안되기 때문에 체결수량이 DB정보와 다르면 DB정보를 수정해준다.///////////
                        if (tmpT0425Vo.cheqty != item.execqty)//체결수량이 다르면 체결수량과 체결가격을 현행화해준다.
                        {
                            item.execqty = tmpT0425Vo.cheqty;
                            item.execprc = tmpT0425Vo.cheprice.Replace(", ", "");
                            //item.Isunm   = tmpT0425Vo.hname//tr에서 종목 이름이 넘어오지 않는다.
                            mainForm.tradingHistory.update(item);//수량 업데이트
                            //mainForm.dataLog.dbSync();
                            //mainForm.dataLog.getHistoryDataTable().Rows.Find(row);
                        }
                        //2.매매목록 확장 데이타 출력
                        
                        findIndex = this.t0425VoList.Find("ordno", item.ordno);
                        if (findIndex>=0)
                        {
                           
                            mainForm.grd_t0425.Rows[findIndex].Cells["ordptnDetail"].Value = item.ordptnDetail;//매매상세구분
                            mainForm.grd_t0425.Rows[findIndex].Cells["t0425_hname" ].Value = item.Isunm;       //종목명
                            mainForm.grd_t0425.Rows[findIndex].Cells["sellOrdAt"   ].Value = item.sellOrdAt;   //금일매도여부
                            mainForm.grd_t0425.Rows[findIndex].Cells["cancelOrdAt" ].Value = item.cancelOrdAt; //주문취소여부
                            mainForm.grd_t0425.Rows[findIndex].Cells["useYN"       ].Value = item.useYN;       //사용여부
                            mainForm.grd_t0425.Rows[findIndex].Cells["upOrdno"     ].Value = item.upOrdno;     //상위 매수 주문번호
                            mainForm.grd_t0425.Rows[findIndex].Cells["upExecprc"   ].Value = Util.GetNumberFormat(item.upExecprc);   //상위체결금액
                            //mainForm.grd_t0425.Rows[findIndex].Cells["ordermtd"    ].Value = item.ordermtd;     //주문매체
                            //실현손익: (당일매도금액 - 매도수수료 - 매도제세금) - (매입금액 + 추정매입수수료) - 신용이자
                            if (item.ordptncode == "01")
                            {
                                매입금액 = double.Parse(tmpT0425Vo.upExecprc) * double.Parse(tmpT0425Vo.cheqty);
                                매도금액 = double.Parse(tmpT0425Vo.cheprice) * double.Parse(tmpT0425Vo.cheqty);
                                매도금액 = 매도금액 - (매도금액 * 0.0033);
                                //tmpT0425Vo.shSunik = Util.GetNumberFormat(매도금액 - 매입금액);
                                mainForm.grd_t0425.Rows[findIndex].Cells["shSunik"].Value = Util.GetNumberFormat(매도금액 - 매입금액);
                                todayLate = ((매도금액 / 매입금액) * 100) - 100;
                                mainForm.grd_t0425.Rows[findIndex].Cells["toDaysunikrt"].Value = Math.Round(todayLate, 2).ToString();

                            }
                            //매수이면. 매수 기준으로 수익률 출력
                            if (item.ordptncode == "02")
                            {
                                t0424FindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", tmpT0425Vo.expcode);
                                if (t0424FindIndex >= 0)
                                {
                                    현재가 = Double.Parse(mainForm.xing_t0424.getT0424VoList().ElementAt(t0424FindIndex).price);//현재가
                                    현재가 = 현재가 - (현재가 * 0.0033);
                                    체결가격 = Double.Parse(tmpT0425Vo.cheprice.Replace(",", ""));//금일체결가격
                                                                                              //1.현재가가 금일매수 값보다 3%이상 올랐으면 금일 매수 수량만큼 매도한다.
                                    todayLate = ((현재가 / 체결가격) * 100) - 100;

                                    mainForm.grd_t0425.Rows[findIndex].Cells["toDaysunikrt"].Value = Math.Round(todayLate, 2).ToString();
                                }
                            }

                            //금일매도주문 색변경
                            if (item.sellOrdAt == "Y") {
                                if (findIndex >= 0) {
                                    mainForm.grd_t0425.Rows[findIndex].DefaultCellStyle.BackColor = Color.DarkOrange;
                                }
                            }
                            //주문취소 색변경
                            if (item.cancelOrdAt == "Y") {
                                if (findIndex >= 0) {
                                    //mainForm.grd_t0425.Rows[findIndex].Cells["t0425_hname"].Style.BackColor   = Color.Gray;
                                    mainForm.grd_t0425.Rows[findIndex].DefaultCellStyle.BackColor = Color.Gray;
                                }
                            }
                        }
                        

                    }//for end

                    




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
                }
                else
                {//마지막 데이타일때 메인폼에 출력해준다.
                    
                    //매수체결 목록
                    mainForm.grd_t0425_chegb1_cnt.Text = this.t0425VoList.Count().ToString();
                    //Thread.Sleep(5000);
                    //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
                    mainForm.input_t0425_log2.Text = "[" + mainForm.label_time.Text + "]t0425 :: 채결/미채결 요청완료";

                    ///////////////////////////////////////////////////////////////////////////////////////////
                    //1.금일매도 청산 각각 구분하여 출력해준다.  
                    mainForm.label_toDaysunik.Text = Util.GetNumberFormat(this.getToDayShSunik("금일매도"));
                    mainForm.label_dtsunik2.Text = Util.GetNumberFormat(this.getToDayShSunik("청산"));


                    if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 910 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1520)
                    {
                        //트레이딩 시작 여부.
                        if (mainForm.tradingAt == "Y")
                        {
                            //2.금일매도실행
                            this.toDaySellTest();

                            //3.주문취소
                            this.orderCancle();
                        }
                        
                    }
                }

                completeAt = true;
            }
            catch (Exception ex){
                Log.WriteLine("t0425 : " + ex.Message);
                Log.WriteLine("t0425 : " + ex.StackTrace);
            }
        }//receiveData end


        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {

            if (nMessageCode == "00000") {
                
            //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
            } else {
                //Thread.Sleep(3000);
                completeAt = true;//중복호출 방지
               
                //-2 :: 서버접속에 실패하였습니다.
                if (nMessageCode == "   -2")
                {
                    //mainForm.exXASessionClass.fnLogin();
                    //Log.WriteLine("t0425 :: 로그인 호출");
                }
                //서버접속 실패로인하여 로그인 여부를 false 로 설정한다.후에 접속실패 코드확보후 조건문 추가해주자.
                //mainForm.exXASessionClass.loginAt = false;
                //00007 :: 시스템 사정으로 자료 서비스를 받을 수 없습니다.
                //00008 :: 시스템 문제로 서비스가 불가능 합니다.
            }

        }

        private int callCnt = 0;
        /// <summary>
		/// 체결/미체결 요청
		/// </summary>
		public void call_request(String account, String accountPw){

            if (completeAt) {
                completeAt = false;//중복호출 방지

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
                mainForm.input_t0425_log.Text = "[" + mainForm.label_time.Text + "]t0425::체결/미체결 요청";

            } else {
                mainForm.input_t0425_log.Text = "[" + mainForm.label_time.Text + "][중복]t0425::체결/미체결 요청";

                callCnt++;
                if (callCnt == 6)
                {
                    this.completeAt = true;
                    callCnt = 0;
                }
            }
        }   // end function

        //미체결 주문취소
        public void orderCancle()
        {
            String 종목명;
            String 종목코드;
            String 주문번호;
            //현재시간.
            String time = mainForm.xing_t0167.time;
            if (time == "" || time == null) { time = "1530"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60) + int.Parse(time.Substring(2, 2));//현재 시간

           
            //매도취소만있다. 매수취소가 필요한지 모르겠다...
            var varT0425VoList = from item in this.t0425VoList
                                    where item.qty != item.cheqty 
                                    //&& item.medosu == "매도" 
                                    && item.cancelOrdAt != "Y"//주문취소 Y가 아닌거.
                                    && item.ordermtd == "XING API"
                                 select item;
            int 주문시간;
            for (int i = 0; i < varT0425VoList.Count(); i++)
            {

                //미체결 시간이 5분 이상이면 취소주문 한다.
                주문시간 = (int.Parse(varT0425VoList.ElementAt(i).ordtime.Substring(0, 2)) * 60) + int.Parse(varT0425VoList.ElementAt(i).ordtime.Substring(2, 2));//현재 시간
                if ((cTime - 주문시간) >= 5)
                {
                    종목명   = varT0425VoList.ElementAt(i).hname;
                    종목코드 = varT0425VoList.ElementAt(i).expcode;
                    주문번호 = varT0425VoList.ElementAt(i).ordno;
                    /// <summary>
                    /// 현물 취소 주문
                    /// </summary>
                    /// <param name="OrgOrdNo">원주문번호</param>
                    /// <param name="IsuNo">종목번호</param>
                    /// <param name="OrdQty">주문수량</param>
                    if (mainForm.xing_CSPAT00800.completeAt)//주문을 호출할수 있을때 호출하고 못하면 그냥 스킵한다.
                    {
                        mainForm.xing_CSPAT00800.call_request(mainForm.account, mainForm.accountPw, 주문번호, 종목코드, "");
                        //회색으로
                        varT0425VoList.ElementAt(i).cancelOrdAt= "Y";
                        //주문번호 주문취소여부 Y로 업데이트
                        var items = from item in mainForm.tradingHistory.getTradingHistoryVoList()
                                    where item.ordno == 주문번호
                                       && item.accno == mainForm.account
                                    select item;

                        foreach (TradingHistoryVo item in items)
                        {
                            item.cancelOrdAt = "Y";
                            mainForm.tradingHistory.update(item);//매도주문 여부 상태 업데이트
                        }

                        Log.WriteLine("t0425::취소주문:" + 종목명 + "(" + 종목코드 + "):[주문번호:" + 주문번호 + "]");
                        mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0, 5) + "]t0425::" + 종목명 + ":취소완료");
                    } else {
                        Log.WriteLine("t0425 ::주문스킵(취소)");
                        mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0, 5) + "]t0425:" + 종목명 + ":주문스킵(취소)");
                    }
                }
            }
  
        }

        //금일매도 - 반복매수 중에서 상승한종목을 매도한다.
        public void toDaySellTest()
        {

            String 투입비율   = mainForm.xing_t1833.getInputRate();
            String 제한비율   = Properties.Settings.Default.BUY_STOP_RATE;
            String 목표수익율 = Properties.Settings.Default.STOP_PROFIT_TARGET;

            //자본금이 제한비율 근처까지 투입이 된상태이면 빠른 매매 회전율을 위하여 목표수익율을 낮추어 준다.
            if (Double.Parse(투입비율) > (Double.Parse(제한비율) - 5))
            {
                목표수익율 = Properties.Settings.Default.STOP_PROFIT_TARGET2;
            }

            String price0424;//0424 현재가
            //String pamt20424;//0424 평균단가
            String 주문번호;//주문번호
            String expcode;//종목코드
            String hname;//종목명
            String cheqty;//체결수량
            String cheprice;//체결가격
            String ordtime;//주문시간
            String 금일수익율;
            String 매매상세구분;
            String 주문수량;
            String 체결수량;
                //금일 매수 and 매도여부:N and 반복매수 and 주문수량=체결수량
            var varT0425VoList = from item in this.t0425VoList
                                where item.sellOrdAt == "N" && item.ordptnDetail == "반복매수"
                               select item;
            int t0424Index;
            for (int i = 0; i < varT0425VoList.Count(); i++){
                주문번호    = varT0425VoList.ElementAt(i).ordno;   //주문번호
                expcode     = varT0425VoList.ElementAt(i).expcode; //종목코드
                hname       = varT0425VoList.ElementAt(i).hname;   //종목명
                cheqty      = varT0425VoList.ElementAt(i).cheqty;  //체결수량
                cheprice    = varT0425VoList.ElementAt(i).cheprice;//체결가격
                ordtime     = varT0425VoList.ElementAt(i).ordtime;//주문시간   
                금일수익율  = varT0425VoList.ElementAt(i).toDaysunikrt;
                매매상세구분 = varT0425VoList.ElementAt(i).ordptnDetail;
                주문수량    = varT0425VoList.ElementAt(i).qty;
                체결수량    = varT0425VoList.ElementAt(i).cheqty;
                //계좌잔고 그리드에서 해당종목 정보 참조.
                t0424Index = mainForm.xing_t0424.getT0424VoList().Find("expcode", expcode);
                if (t0424Index >= 0)
                {
                    price0424 = mainForm.xing_t0424.getT0424VoList().ElementAt(t0424Index).price;//현재가
                    

                    //금일매도 테스트
                    if (Properties.Settings.Default.TODAY_SELL_AT && 매매상세구분 == "반복매수" && 주문수량 == 체결수량)
                    {
                        if (Double.Parse(varT0425VoList.ElementAt(i).toDaysunikrt) >= double.Parse(목표수익율)) {
                                
                            //MessageBox.Show(hname);
                            /// <summary>
                            /// 현물정상주문
                            /// </summary>
                            /// <param name="ordptnDetail">상세주문구분-신규매수|반복매수|금일매도|청산</param>
                            /// <param name="upOrdno">상위매수주문번호-금일매도일때만 셋팅될것같다.</param>
                            /// <param name="upExecprc">상위체결금액</param>
                            /// <param name="hname">종목명</param>
                            /// <param name="IsuNo">종목코드</param>
                            /// <param name="Quantity">수량</param>
                            /// <param name="Price">가격</param>
                            if (mainForm.xing_CSPAT00600.completeAt)//주문을 호출할수 있을때 호출하고 못하면 그냥 스킵한다.
                            {
                                mainForm.xing_CSPAT00600.call_requestSell("금일매도", 주문번호, cheprice.Replace(",", ""), hname, expcode, cheqty, price0424);

                                //매수건에대해서 금일매도를 해주었으므로 매수건에 금일매도여부를 Y로 업데이트 해준다.
                                varT0425VoList.ElementAt(i).sellOrdAt="Y";  
   
                                //상위주문번호 주문여부 Y로 업데이트
                                var items = from item in mainForm.tradingHistory.getTradingHistoryVoList()
                                            where item.ordno == 주문번호
                                                && item.accno == mainForm.account
                                            //&& (String)item["useYN"] == "Y"
                                            select item;

                                foreach (TradingHistoryVo item in items)
                                {
                                    item.sellOrdAt = "Y";
                                    mainForm.tradingHistory.update(item);//매도주문 여부 상태 업데이트
                                }

                                Log.WriteLine("t0425::금일매도:" + hname + "(" + expcode + "): [주문가격:" + price0424.ToString() + "|주문수량:" + cheqty + "|금일수익율:" + 금일수익율 + " | 주문번호:" + 주문번호 + "]");
                                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0, 5) + "]t0425::" + hname + ":금일 매도.");
                            } else {
                                Log.WriteLine("t0425::주문스킵(금일매도):" + hname + "(" + expcode + "):[주문가격:" + price0424.ToString() + "|주문수량:" + cheqty + "|금일수익율:" + 금일수익율 + " | 주문번호:" + 주문번호 + "]");
                                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0, 5) + "]t0425::" + hname + ":주문 스킵.");
                            }

                        }

                            
                        
                        
                    }//t0424 index if end

                }
            }


            
        }//금일매도매수 end

        //금일매도 청산 과 금일매도 를 구분하여 건당 매도차익을 리턴하는 메소드.
        //parameta //상세 주문구분 [금일매도|청산]
        public String getToDayShSunik(String ordptnDetail)
        {
            double 실현손익 = 0;
            var resultDataLogVoList = from item in this.t0425VoList
                                      where item.ordptnDetail == ordptnDetail
                                      //&& item.date == DateTime.Now.ToString("yyyyMMdd")
                                      //&& item.accno == mainForm.accountForm.account
                                      select item;
            //Log.WriteLine("DataLog::  [카운트:" + resultDataLogVoList.Count()+"]");
            foreach (var item  in resultDataLogVoList)
            {
                //매도금액 - 매수금액
                //실현손익 = 실현손익 + ((double.Parse(item.cheprice) * double.Parse(item.cheqty)) - (double.Parse(item.cheqty) * double.Parse(item.upExecprc)));
                실현손익 = 실현손익 + double.Parse(item.shSunik);
            }
            return 실현손익.ToString();
        }

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
        public String ordermtd { set; get; } //주문매체

        public String ordptnDetail { set; get; }//상세 주문구분 신규매수|반복매수|금일매도|청산
        public String upOrdno      { set; get; }//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        public String upExecprc    { set; get; }//상위체결금액
        public String sellOrdAt    { set; get; }//매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String cancelOrdAt { set; get; }//매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String useYN        { set; get; }//사용여부
        public String toDaysunikrt { set; get; }//금일 수익률
        public String shSunik      { set; get; }//실현손익
    }


}   // end namespace
