
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

        public Boolean initAt = true;

        // 생성자
        public Xing_t0425()
        {
            base.ResFileName = "₩res₩t0425.res";


            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            this.t0425VoList = new EBindingList<T0425Vo>();

        }   // end function


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
               
              
                int    findIndex;
                
                T0425Vo tmpT0425Vo;
                String medosu;//매매구분
                var blockData = base.GetBlockData("t0425OutBlock1");
                for (int i = 0; i < blockCount; i++)
                {
                    ordno  = base.GetFieldData("t0425OutBlock1", "ordno"  , i); //주문번호
                    status = base.GetFieldData("t0425OutBlock1", "status" , i); //상태
                    qty    = base.GetFieldData("t0425OutBlock1", "qty"    , i); //주문수량
                    cheqty = base.GetFieldData("t0425OutBlock1", "cheqty" , i); //체결수량
                    medosu = base.GetFieldData("t0425OutBlock1", "medosu" , i); //매매구분 - 0:전체|1:매수|2:매도
                    if (medosu.IndexOf("취소") >= 0)
                    {//취소주문건은 그리드에 출력하지 않는다.
                        continue;
                    }
                    findIndex = this.t0425VoList.Find("ordno", ordno);


                    if (findIndex < 0)
                    {
                        tmpT0425Vo = new T0425Vo();
                        this.t0425VoList.Add(tmpT0425Vo);
                        findIndex = this.t0425VoList.Count() - 1;

                       
                    }
                    String ordtime = base.GetFieldData("t0425OutBlock1", "ordtime", i);
                    ordtime = ordtime.Substring(0, 2) + ":" + ordtime.Substring(2, 2) + ":" + ordtime.Substring(4, 2);
                    //mainForm.grd_t0425.Rows[findIndex].Cells["ordtime"      ].Value = base.GetFieldData(             "t0425OutBlock1", "ordtime"    , i);  //주문시간
                    mainForm.grd_t0425.Rows[findIndex].Cells["ordtime"      ].Value = ordtime; //주문시간
                    mainForm.grd_t0425.Rows[findIndex].Cells["medosu"       ].Value = base.GetFieldData(             "t0425OutBlock1", "medosu"     , i);  //매매구분 - 0:전체|1:매수|2:매도
                    mainForm.grd_t0425.Rows[findIndex].Cells["expcode"      ].Value = base.GetFieldData(             "t0425OutBlock1", "expcode"    , i);  //종목번호
                    //mainForm.grd_t0425.Rows[findIndex].Cells["t0425_hname"].Value       = "";                                                            //종목명
                    mainForm.grd_t0425.Rows[findIndex].Cells["qty"          ].Value = Double.Parse(base.GetFieldData("t0425OutBlock1", "qty"        , i)); //주문수량
                    mainForm.grd_t0425.Rows[findIndex].Cells["t0425_price"  ].Value = Double.Parse(base.GetFieldData("t0425OutBlock1", "price"      , i)); //주문가격
                    mainForm.grd_t0425.Rows[findIndex].Cells["cheqty"       ].Value = Double.Parse(base.GetFieldData("t0425OutBlock1", "cheqty"     , i)); //체결수량
                    mainForm.grd_t0425.Rows[findIndex].Cells["cheprice"     ].Value = Double.Parse(base.GetFieldData("t0425OutBlock1", "cheprice"   , i)); //체결가격
                    mainForm.grd_t0425.Rows[findIndex].Cells["ordrem"       ].Value = Double.Parse(base.GetFieldData("t0425OutBlock1", "ordrem"     , i)); //미체결잔량
                    mainForm.grd_t0425.Rows[findIndex].Cells["status"       ].Value = base.GetFieldData(             "t0425OutBlock1", "status"     , i);  //상태
                    mainForm.grd_t0425.Rows[findIndex].Cells["ordno"        ].Value = base.GetFieldData(             "t0425OutBlock1", "ordno"      , i);  //주문번호
                    mainForm.grd_t0425.Rows[findIndex].Cells["ordermtd"     ].Value = base.GetFieldData(             "t0425OutBlock1", "ordermtd"   , i);  //주문매체
                    
                    //확장 정보및 싱크
                    this.t0425Sync(findIndex);
                    

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
                }else{//마지막 데이타일때 메인폼에 출력해준다.
                    
                    //매수체결 목록
                    mainForm.grd_t0425_chegb1_cnt.Text = this.t0425VoList.Count().ToString();
                    //Thread.Sleep(5000);
                    //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
                    mainForm.input_t0425_log2.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "><t0425:채결/미채결 요청완료>";

                    //초기화 여부
                    if (initAt)
                    {
                        this.initAt = false;

                        foreach (T0425Vo t0425Vo in t0425VoList)
                        {
                            //실시간 현재가 종목  등록
                            //코스피
                            mainForm.real_S3.call_real(t0425Vo.expcode);
                            mainForm.real_K3.call_real(t0425Vo.expcode);

                        }
                    }
                    else
                    {
                        //2.주문취소
                        this.orderCancle();
                    }

                }
                completeAt = true;
            }
            catch (Exception ex){
                Log.WriteLine("t0425 : " + ex.Message);
                Log.WriteLine("t0425 : " + ex.StackTrace);
            }
        }//receiveData end

        public void t0425Sync(int rowIndex)
        {
            T0425Vo tmpT0425Vo = t0425VoList.ElementAt(rowIndex);
            
            //체결수량이 다르면 체결수량과 체결가격을 현행화해준다.
            var items = from item in mainForm.tradingHistory.getTradingHistoryDt().AsEnumerable()
                        where item["accno"].ToString() == mainForm.account
                           && item["Isuno"].ToString() == tmpT0425Vo.expcode.Replace("A", "")
                           && item["ordno"].ToString() == tmpT0425Vo.ordno
                        select item;

            if (items.Count() > 0)
            {
                /////////프로그램 재시작하는동안 체결된 정보는 DB에 저장이 안되기 때문에 체결수량이 DB정보와 다르면 DB정보를 수정해준다.///////////
                //체결수량이 다르면 체결수량과 체결가격을 현행화해준다.
                if (tmpT0425Vo.cheqty != Double.Parse(items.First()["execqty"].ToString()))
                {
                    items.First()["execqty"] = tmpT0425Vo.cheqty;
                    items.First()["execprc"] = tmpT0425Vo.cheprice;

                    //item.Isunm   = tmpT0425Vo.hname//tr에서 종목 이름이 넘어오지 않는다.
                    mainForm.tradingHistory.execqtyUpdate(items.First());//수량 업데이트

                }
                //2.매매목록 확장 데이타 출력
                
                mainForm.grd_t0425.Rows[rowIndex].Cells["ordptnDetail"  ].Value = items.First()["ordptnDetail"].ToString(); //매매상세구분
                mainForm.grd_t0425.Rows[rowIndex].Cells["t0425_hname"   ].Value = items.First()["Isunm"].ToString();        //종목명
                mainForm.grd_t0425.Rows[rowIndex].Cells["sellOrdAt"     ].Value = items.First()["sellOrdAt"].ToString();    //금일매도여부
                mainForm.grd_t0425.Rows[rowIndex].Cells["cancelOrdAt"   ].Value = items.First()["cancelOrdAt"].ToString();  //주문취소여부
                mainForm.grd_t0425.Rows[rowIndex].Cells["useYN"         ].Value = items.First()["useYN"].ToString();        //사용여부
                mainForm.grd_t0425.Rows[rowIndex].Cells["upOrdno"       ].Value = items.First()["upOrdno"].ToString();      //상위 매수 주문번호
                mainForm.grd_t0425.Rows[rowIndex].Cells["upExecprc"     ].Value = items.First()["upExecprc"].ToString();
                mainForm.grd_t0425.Rows[rowIndex].Cells["searchNm"      ].Value = items.First()["searchNm"].ToString();     //검색조건 이름
                //상위체결금액
                Double 재비용율 = mainForm.combox_targetServer.SelectedIndex == 0 ? 0.0099 : 0.0033;

                //mainForm.grd_t0425.Rows[findIndex].Cells["ordermtd"    ].Value = item.ordermtd;     //주문매체
                //실현손익: (당일매도금액 - 매도수수료 - 매도제세금) - (매입금액 + 추정매입수수료) - 신용이자
                if (items.First()["ordptncode"].ToString() == "01")
                {
                    Double 매입금액 = tmpT0425Vo.upExecprc * tmpT0425Vo.cheqty;
                    Double 매도금액 = tmpT0425Vo.cheprice  * tmpT0425Vo.cheqty;
                    매도금액 = 매도금액 - (매도금액 * 재비용율);

                    mainForm.grd_t0425.Rows[rowIndex].Cells["shSunik"].Value = 매도금액 - 매입금액;
                    Double todayLate = ((매도금액 / 매입금액) * 100) - 100;
                    mainForm.grd_t0425.Rows[rowIndex].Cells["toDaysunikrt"].Value = Math.Round(todayLate, 2).ToString();

                }
                //매수이면. 매수 기준으로 수익률 출력 --이벤트 대상
                if (items.First()["ordptncode"].ToString() == "02")
                {
                    int t0424FindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", tmpT0425Vo.expcode);
                    if (t0424FindIndex >= 0)
                    {
                        Double 현재가   = mainForm.xing_t0424.getT0424VoList().ElementAt(t0424FindIndex).price;//현재가
                        현재가          = 현재가 - (현재가 * 재비용율);
                        Double 체결가격 = tmpT0425Vo.cheprice;//금일체결가격
                                                                                            //1.현재가가 금일매수 값보다 3%이상 올랐으면 금일 매수 수량만큼 매도한다.
                        Double todayLate = ((현재가 / 체결가격) * 100) - 100;

                        mainForm.grd_t0425.Rows[rowIndex].Cells["toDaysunikrt"].Value = Math.Round(todayLate, 2).ToString();
                    }
                }

                //금일매도주문 색변경  --삭제및 이벤트 대상
                if (items.First()["sellOrdAt"].ToString() == "Y")
                {
                    mainForm.grd_t0425.Rows[rowIndex].DefaultCellStyle.BackColor = Color.DarkOrange;
                }
                //주문취소 색변경
                if (items.First()["cancelOrdAt"].ToString() == "Y"){
                    mainForm.grd_t0425.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gray;
                }

                //폰트색변경
                String 매매구분 = mainForm.grd_t0425.Rows[rowIndex].Cells["medosu"].Value.ToString();
                Color color = 매매구분.IndexOf("매도") >= 0 ? Color.Blue : Color.Red;
                mainForm.grd_t0425.Rows[rowIndex].Cells["medosu"].Style.ForeColor = color;//매매구분
                mainForm.grd_t0425.Rows[rowIndex].Cells["ordptnDetail"].Style.ForeColor = color;//매매상세구분
                var 금일수익율 = mainForm.grd_t0425.Rows[rowIndex].Cells["toDaysunikrt"].Value;
                금일수익율 = 금일수익율 == null ? "0" : 금일수익율;
                
                color = 금일수익율.ToString().IndexOf("-") >= 0 ? Color.Blue : Color.Red;
                mainForm.grd_t0425.Rows[rowIndex].Cells["toDaysunikrt"].Style.ForeColor = color;//금일수익율


            } else { //DB에 매매 이력 정보가 없을때

            }
           
        }


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
                base.SetFieldData("t0425InBlock", "sortgb"   , 0, "2");        // 정렬순서
                base.SetFieldData("t0425InBlock", "cts_ordno", 0, "");         // 주문번호

                // 계좌잔고 그리드 초기화

                //멤버변수 초기화
                base.Request(false);  //연속조회일경우 true

                //폼 메세지.
                mainForm.input_t0425_log.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + ">t0425:체결/미체결 요청>";

            } else {
                mainForm.input_t0425_log.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "><t0425:중복>";

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
            Double 미체결수량=0;
            Double 현재가격 = 0;
            String 매매구분;
            //현재시간.
            //String time = mainForm.xing_t0167.time;
            //if (time == "" || time == null) { time = "1530"; }//에러 안나게 기본값을 셋팅해준다.
            //int cTime = (int.Parse(time.Substring(0, 2)) * 60) + int.Parse(time.Substring(2, 2));//현재 시간

            
            //매수/매도 취소
            var varT0425VoList = from item in this.t0425VoList
                                    where item.qty != item.cheqty 
                                    && item.cancelOrdAt != "Y"//주문취소 Y가 아닌거.
                                    && item.ordermtd == "XING API"
                                    && item.ordrem > 0
                                 select item;
            
            for (int i = 0; i < varT0425VoList.Count(); i++){
              
                T0425Vo t0425Vo = varT0425VoList.ElementAt(i);
                종목명      = t0425Vo.hname;
                종목코드    = t0425Vo.expcode;
                주문번호    = t0425Vo.ordno;
                미체결수량  = t0425Vo.ordrem;
                현재가격    = t0425Vo.currentPrice;
                매매구분    = t0425Vo.medosu;
                //타임스펜
                //현재시간 
                TimeSpan nowTimeSpan = TimeSpan.Parse(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                // 주문시간
                TimeSpan ordTimeSpan = TimeSpan.Parse(t0425Vo.ordtime);
                double 타이머 = (nowTimeSpan.TotalSeconds - ordTimeSpan.TotalSeconds);
                if (매매구분.Equals("매수"))
                {
                    //Properties.Settings.Default.BUY_HO;
                    //Properties.Settings.Default.BUY_HO_CHANGE_TIMMER;
                    //Properties.Settings.Default.BUY_HO_CHANGE_SE;
                    
                    if (타이머 > Double.Parse(Properties.Settings.Default.BUY_HO_CHANGE_TIMMER))
                    {
                        if (Properties.Settings.Default.BUY_HO_CHANGE_SE.Equals("주문취소"))
                        {
                            // 현물 취소 주문
                            //OrgOrdNo(원주문번호), IsuNo(종목번호), OrdQty(주문수량)
                            Xing_CSPAT00800 xing_CSPAT00800 = mainForm.CSPAT00600Mng.get800();
                            xing_CSPAT00800.call_request(mainForm.account, mainForm.accountPw, 주문번호, 종목코드, "");
                            orderAfterCall(t0425Vo,"취소");
                        }
                        else{//시장가로정정
                             //호가 계산
                            if (!Properties.Settings.Default.BUY_HO.Equals("시장가"))
                            {
                                현재가격 = Double.Parse(Util.getTickPrice(현재가격.ToString(), double.Parse(Properties.Settings.Default.BUY_HO)));
                            }

                            //OrgOrdNo(원주문번호), IsuNo(종목번호), OrdQty(주문수량),겨걱
                            Xing_CSPAT00700 xing_CSPAT00700 = mainForm.CSPAT00600Mng.get700();
                             xing_CSPAT00700.call_request(mainForm.account, mainForm.accountPw, 주문번호, 종목코드, 미체결수량, 현재가격);
                             orderAfterCall(t0425Vo,"정정");
                           
                        }
                    }
                }

                if (매매구분.Equals("매도"))
                {
                    //Properties.Settings.Default.SELL_HO;
                    //Properties.Settings.Default.SELL_HO_CHANGE_TIMMER;
                    //Properties.Settings.Default.SELL_HO_CHANGE_SE;
                    if (타이머 > Double.Parse(Properties.Settings.Default.SELL_HO_CHANGE_TIMMER))
                    {
                        if (Properties.Settings.Default.SELL_HO_CHANGE_SE.Equals("주문취소")) {
                           
                            //취소 -OrgOrdNo(원주문번호), IsuNo(종목번호), OrdQty(주문수량)
                            Xing_CSPAT00800 xing_CSPAT00800 = mainForm.CSPAT00600Mng.get800();
                            xing_CSPAT00800.call_request(mainForm.account, mainForm.accountPw, 주문번호, 종목코드, "");
                            orderAfterCall(t0425Vo,"취소");
                        } else {//시장가로정정
                            //호가 계산
                            if (!Properties.Settings.Default.SELL_HO.Equals("시장가"))
                            {
                                현재가격 = Double.Parse(Util.getTickPrice(현재가격.ToString(), double.Parse(Properties.Settings.Default.SELL_HO)));
                            }
                            //OrgOrdNo(원주문번호), IsuNo(종목번호), OrdQty(주문수량),겨걱
                            Xing_CSPAT00700 xing_CSPAT00700 = mainForm.CSPAT00600Mng.get700();
                            xing_CSPAT00700.call_request(mainForm.account, mainForm.accountPw, 주문번호, 종목코드, 미체결수량, 현재가격);
                            orderAfterCall(t0425Vo,"정정");
                        }
                      
                    }
                }
                
            }
            
        }

        public void orderAfterCall(T0425Vo t0425Vo, String changeSe)
        {
            String 종목명;
            String 종목코드;
            String 주문번호;
            Double 미체결수량 = 0;
            Double 현재가격 = 0;
            String 매매구분;

            종목명 = t0425Vo.hname;
            종목코드 = t0425Vo.expcode;
            주문번호 = t0425Vo.ordno;
            미체결수량 = t0425Vo.ordrem;
            현재가격 = t0425Vo.currentPrice;
            매매구분 = t0425Vo.medosu;
            //회색으로
            t0425Vo.cancelOrdAt = "Y";
            //주문번호 주문취소여부 Y로 업데이트
            var items = from item in mainForm.tradingHistory.getTradingHistoryDt().AsEnumerable()
                        where item["ordno"].ToString() == 주문번호
                           && item["accno"].ToString() == mainForm.account
                        select item;
            if (items.Count() > 0)
            {
                items.First()["cancelOrdAt"] = "Y";
                mainForm.tradingHistory.cancelOrdAtUpdate(items.First());//매도주문 여부 상태 업데이트

                Log.WriteLine("<t0425:"+ 매매구분+ changeSe+"><" + 종목명 + ">");
                mainForm.insertListBoxLog("<" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + ">< t0425:"+ 매매구분+ changeSe+" >< " + 종목명 + " > ");
            } else {
                Log.WriteLine("<t0425:" + 매매구분 + changeSe + "><" + 종목명 + "> DB 매핑정보 없음.");
                mainForm.insertListBoxLog("<" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + ">< t0425:" + 매매구분 + changeSe + " >< " + 종목명 + " > DB 매핑정보 없음.");
            }
        }

    } //end class   
    
    public class T0425Vo {
        public String ordtime  { set; get; } //주문시간
        public String medosu   { set; get; } //매매구분 - 0:전체|1:매수|2:매도
        public String expcode  { set; get; } //종목번호
        public String hname    { set; get; } //종목명
        public Double qty      { set; get; } //주문수량
        public Double price    { set; get; } //주문가격
        public Double cheqty   { set; get; } //체결수량
        public Double cheprice { set; get; } //체결가격
        public Double ordrem   { set; get; } //미체결잔량
        public String status   { set; get; } //상태
        public String ordno    { set; get; } //주문번호
        public String ordermtd { set; get; } //주문매체

        public String ordptnDetail { set; get; }//상세 주문구분 신규매수|반복매수|금일매도|청산
        public String upOrdno      { set; get; }//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        public Double upExecprc    { set; get; }//상위체결금액
        public String sellOrdAt    { set; get; }//매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String cancelOrdAt  { set; get; }//주문 취소 여부.
        public String useYN        { set; get; }//사용여부
        public Double toDaysunikrt { set; get; }//금일 수익률
        public Double shSunik      { set; get; }//실현손익
        public String searchNm     { set; get; }//검색조건이름
        public Double currentPrice { set; get; }//현재가격
    }


}   // end namespace
