
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
    //주식 잔고2
    public class Xing_t0424 : XAQueryClass {

        private EBindingList<T0424Vo> t0424VoList;
        private EBindingList<ExcludeT0424Vo> excludeT0424VoList;//감시제외종목리스트

        public Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public Double sunamt;    //추정자산

        public Double dtsunik;   //실현손익
        
        public Boolean initAt = true;
        // 생성자
        public Xing_t0424()
        {
            base.ResFileName = "₩res₩t0424.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            t0424VoList        = new EBindingList<T0424Vo>();

            //감시제외 대상 목록
            excludeT0424VoList = new EBindingList<ExcludeT0424Vo>();

        }   // end function

        
        
        public EBindingList<T0424Vo> getT0424VoList()
        {
            return this.t0424VoList;
        }
        public EBindingList<ExcludeT0424Vo> getExcludeT0424VoList()
        {
            return this.excludeT0424VoList;
        }

        /// <summary>
		/// 주식잔고2(T0424) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode)
        {
            try
            {
                String cts_expcode = base.GetFieldData("t0424OutBlock", "cts_expcode", 0);//CTS_종목번호-연속조회키

                //1.계좌 잔고 목록을 그리드에 추가
                int blockCount = base.GetBlockCount("t0424OutBlock1");

                T0424Vo tmpT0424Vo=null;

                String expcode;//종목코드
                String mdposqt;//매도가능
                String 주문여부;
                int findIndex;
                for (int i = 0; i < blockCount; i++)
                {
                    expcode = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드
                    mdposqt = base.GetFieldData("t0424OutBlock1", "mdposqt", i); //매도가능
                 

                    //1.t0424 는 매도가능수량이 0 건인것은 없어야하는데 가끔 검색되어 나온다...그냥 스킵하자.
                    if (Double.Parse(mdposqt) <= 0)
                    {
                        //Log.WriteLine("t0424 는 매도가능수량이 0 건인것은 조회 되지 말아야하는데... 가끔 검색되어 나오는지 잘 모르겠다...");
                        //Log.WriteLine("t0424::" + base.GetFieldData("t0424OutBlock1", "hname", i) + "(" + expcode + ")t0424 는 매도가능수량이 0 건인것은 조회 되지 말아야하는데... ");
                        continue;
                    }
                    
                    findIndex = t0424VoList.Find("expcode", expcode);
                    
                    if (findIndex < 0){
                        tmpT0424Vo = new T0424Vo();
                        this.t0424VoList.Add(tmpT0424Vo);
                        findIndex = this.t0424VoList.Count() - 1;

                        //mainForm.grd_t0424.Rows[findIndex].Cells["orderAt"].Value = "N";
                        tmpT0424Vo.orderAt = "N";
                        tmpT0424Vo.errorcd = "";

                        tmpT0424Vo.expcode  = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드
                        tmpT0424Vo.hname    = base.GetFieldData("t0424OutBlock1", "hname", i); //종목명
                        tmpT0424Vo.mdposqt  = Double.Parse(base.GetFieldData("t0424OutBlock1", "mdposqt", i)); //매도가능 수량
                        tmpT0424Vo.price    = Double.Parse(base.GetFieldData("t0424OutBlock1", "price", i)); //현재가
                        tmpT0424Vo.appamt   = Double.Parse(base.GetFieldData("t0424OutBlock1", "appamt", i)); //평가금액
                        tmpT0424Vo.dtsunik  = Double.Parse(base.GetFieldData("t0424OutBlock1", "dtsunik", i)); //평가손익
                        tmpT0424Vo.sunikrt  = Double.Parse(base.GetFieldData("t0424OutBlock1", "sunikrt", i)); //수익율
                        tmpT0424Vo.pamt     = Double.Parse(base.GetFieldData("t0424OutBlock1", "pamt", i)); //평균단가
                        tmpT0424Vo.mamt     = Double.Parse(base.GetFieldData("t0424OutBlock1", "mamt", i)); //매입금액
                        tmpT0424Vo.fee      = Double.Parse(base.GetFieldData("t0424OutBlock1", "fee", i)); //수수료
                        tmpT0424Vo.tax      = Double.Parse(base.GetFieldData("t0424OutBlock1", "tax", i)); //제세금 
                        tmpT0424Vo.jonggb   = base.GetFieldData("t0424OutBlock1", "jonggb", i); //종목시장구분

                    } else {
                        tmpT0424Vo = t0424VoList.ElementAt(findIndex);
                        mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "mdposqt", i)); //매도가능 수량
                        mainForm.grd_t0424.Rows[findIndex].Cells["price"    ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "price"  , i)); //현재가
                        mainForm.grd_t0424.Rows[findIndex].Cells["appamt"   ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "appamt" , i)); //평가금액
                        mainForm.grd_t0424.Rows[findIndex].Cells["dtsunik"  ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "dtsunik", i)); //평가손익
                        mainForm.grd_t0424.Rows[findIndex].Cells["c_sunikrt"].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "sunikrt", i)); //수익율
                        mainForm.grd_t0424.Rows[findIndex].Cells["pamt"     ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "pamt"   , i)); //평균단가
                        mainForm.grd_t0424.Rows[findIndex].Cells["mamt"     ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "mamt"   , i)); //매입금액
                        mainForm.grd_t0424.Rows[findIndex].Cells["fee"      ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "fee"    , i)); //수수료
                        mainForm.grd_t0424.Rows[findIndex].Cells["tax"      ].Value = Double.Parse(base.GetFieldData("t0424OutBlock1", "tax"    , i)); //제세금 
                    }
                               
                    
                   
                    
                    //주문여부 = mainForm.grd_t0424.Rows[findIndex].Cells["orderAt"].Value.ToString();
                    주문여부 = tmpT0424Vo.orderAt;
                    //매도 주문후 취소된 종목은 N상태이고 종목 정보가 업데이트 되면 주문여부를 N 으로 다시 돌려놓는다.
                    if (주문여부 == "C")
                    {//주문이 나가 종목은 t0424에서 종목 정보가 넘어오지 않아서 주문한 시점부터 정보가 업데이트 되지 않느다.
                        //mainForm.grd_t0424.Rows[findIndex].Cells["orderAt"].Value = "N";
                        tmpT0424Vo.orderAt = "N";
                    }
                    //그리드에서 삭제여부
                    mainForm.grd_t0424.Rows[findIndex].Cells["deleteAt"].Value = "N";
                    tmpT0424Vo.deleteAt = "N";

                    //확장정보
                    this.getSummaryVo(tmpT0424Vo);
                  
                    mainForm.priceChangedProcess(findIndex);
                    


                }//for end

                //2.연속 데이타 정보가 남아있는지 구분
                if (base.IsNext){
                    //연속 데이타 정보를 호출.
                    base.SetFieldData("t0424InBlock", "cts_expcode", 0, cts_expcode);      // CTS종목번호 : 처음 조회시는 SPACE
                    base.Request(true); //연속조회일경우 true
                }else{//마지막 데이타일때 메인폼에 출력해준다.
   
   
                    //초기화 여부
                    if (initAt){
                        //1.매매이력 동기화 --그냥 에러만 표시하는걸로 확인수 폼에서 클릭
                        //2.감시제외종목 그리드 동기화
                        this.exclWatchSync();
                        foreach (T0424Vo t0424Vo in t0424VoList)
                        {
                            //실시간 현재가 종목  등록

                            //코스피
                            if (t0424Vo.jonggb == "3") mainForm.real_S3.call_real(t0424Vo.expcode);
                            //코스닥
                            if (t0424Vo.jonggb == "2")  mainForm.real_K3.call_real(t0424Vo.expcode);
                        }
                        this.initAt = false;
                    } else {
                        //매매거래 가능 시간이고 매매가능여부 값이 Y일때 체크후 매도 로직 호출
                        if (Double.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 901 && Double.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1519)
                        {
                            //목표수익율 설정
                            Double 목표수익율 = Double.Parse(Properties.Settings.Default.STOP_TARGET_RATE);
                            
                            for (int i = 0; i < this.t0424VoList.Count(); i++)
                            {
                                //1.거래가능여부 && 주문중상태가 아니고 && 종목거래 에러 여부
                                T0424Vo tmpt0424Vo = t0424VoList.ElementAt(i);
                                if(tmpt0424Vo.errorcd == "")
                                {
                                    this.tradingStopTest(t0424VoList.ElementAt(i), i, 목표수익율);
                                }
                                else
                                {
                                    ;
                                }
                                
                                this.t0424VoList.ElementAt(i).deleteAt = "Y";
                            }
                        }
                    }
                    this.dtsunik = Double.Parse(this.GetFieldData("t0424OutBlock", "dtsunik", 0));// 실현손익
                    //트레이딩 정보 업데이트
                    mainForm.tradingInfoUpdate();
                    //로그 및 중복 요청 처리 2:코스닥, 3:코스피
                    mainForm.input_t0424_log.Text = "[" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + "]t0424 :: 잔고조회 완료";
                    mainForm.h_totalCount.Text = t0424VoList.Count().ToString();
                    //응답처리 완료
                    completeAt = true;
                }//end
            }catch (Exception ex){
                Log.WriteLine("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.StackTrace);
            }

        }//receiveDataEventHandler END

        public void getSummaryVo(T0424Vo t0424Vo)
        {
            String 종목코드 = t0424Vo.expcode;
            Double 매도가능수량 = t0424Vo.mdposqt;
            String 에러코드 = t0424Vo.errorcd;
            SummaryVo summaryVo = mainForm.tradingHistory.getSummaryVo(종목코드);
            if (summaryVo == null)
            {
                t0424Vo.errorcd = "summaryVo is null";

            }
            else
            {
                String 최대수익율 = Util.nvl(summaryVo.maxRt, "0");
                String 최소수익율 = Util.nvl(summaryVo.minRt, "0");

                t0424Vo.sellCnt = summaryVo.sellCnt;    //매도 횟수.
                t0424Vo.buyCnt = summaryVo.buyCnt;     //매수 횟수
                t0424Vo.firstBuyDt = summaryVo.firstBuyDt; //최초진입일시

                t0424Vo.sumMdposqt = summaryVo.sumMdposqt; //매도가능이력
                t0424Vo.ordermtd = summaryVo.ordermtd;    //주문매체
                t0424Vo.exclWatchAt = summaryVo.exclWatchAt; //감시제외여부
                t0424Vo.searchNm = summaryVo.searchNm;    //검색조건 이름
                t0424Vo.maxRt = 최대수익율;            //최대도달 수익율
                t0424Vo.minRt = 최소수익율;            //최소도달 수익율

                //매도가능수량이 같지 않으면 에러표시 해주자.

                //String c_mdposqt = this.grd_t0424.Rows[rowIndex].Cells["c_mdposqt"].Value.ToString();
                if (매도가능수량.ToString() != summaryVo.sumMdposqt)
                {
                    t0424Vo.errorcd = "mdposqt not equals";
                    //this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (매도가능수량.ToString() == summaryVo.sumMdposqt)
                {
                    if (t0424Vo.errorcd.Equals("mdposqt not equals"))//기존 다른 에러코드가 존재하면 초기화 하지 않는다.
                    {
                        t0424Vo.errorcd = "";
                        //this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                //확장정보 에러일경우 에러상태를 풀어준다.
                if (t0424Vo.errorcd != null && t0424Vo.errorcd.Equals("summaryVo is null"))//summaryVo is null
                {
                    t0424Vo.errorcd = "";
                    //this.grd_t0424.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }
        

        public Double getSunikrt(Double 현재가, Double 평균단가)
        {
            Double 손익률 = 0;

            Double 재비용율 = mainForm.combox_targetServer.SelectedIndex == 0 ? 0.0099 : 0.0033;
  
            현재가 = 현재가 - (현재가 * 재비용율);

            손익률 = ((현재가 / 평균단가) * 100) - 100;

            return Math.Round(손익률, 2);

        }
        
        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
            if (nMessageCode == "00000") {
                ;
            }else {
                //Log.WriteLine("[" + mainForm.input_time.Text + "]t0424 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t0424_log.Text = "[" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "]t0424 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지

                //서버접속 실패로인하여 로그인 여부를 false 로 설정한다.후에 접속실패 코드확보후 조건문 추가해주자.
                //mainForm.exXASessionClass.loginAt = false;
                //00007 :: 시스템 사정으로 자료 서비스를 받을 수 없습니다.
                //00008 :: 시스템 문제로 서비스가 불가능 합니다.
            }
        }

        //감시제외종목 그리드 동기화
        public void exclWatchSync()
        {
            //private EBindingList<T0424Vo> t0424VoList;
            //private EBindingList<T0424Vo> t0424VoExclList;
            this.excludeT0424VoList.Clear();
            for (int i = 0; i < t0424VoList.Count(); i++)
            {
                if (t0424VoList.ElementAt(i).exclWatchAt == "Y")
                {
                    int findIndex = this.excludeT0424VoList.Find("expcode", t0424VoList.ElementAt(i).expcode.Replace("A", ""));

                    if (findIndex >= 0)
                    {
                        //mainForm.grd_t0424Excl.Rows[tmpIndex].Cells["e_ordermtd"       ].Value = tmpT0424Vo.ordermtd;      //주문매체
                        //mainForm.grd_t0424Excl.Rows[findIndex].Cells["e_targClearPrc"   ].Value = Util.GetNumberFormat(t0424VoList.ElementAt(i).targClearPrc);   //목표청산가격
                        //mainForm.grd_t0424Excl.Rows[findIndex].Cells["e_secEntPrc"      ].Value = Util.GetNumberFormat(t0424VoList.ElementAt(i).secEntPrc);     //2차진입가격
                        //mainForm.grd_t0424Excl.Rows[findIndex].Cells["e_secEntAmt"      ].Value = Util.GetNumberFormat(t0424VoList.ElementAt(i).secEntAmt);     //2차진입비중가격
                        //mainForm.grd_t0424Excl.Rows[findIndex].Cells["e_stopPrc"        ].Value = Util.GetNumberFormat(t0424VoList.ElementAt(i).stopPrc);       //손절가격
                        mainForm.grd_t0424Excl.Rows[findIndex].Cells["e_exclWatchAt"    ].Value = t0424VoList.ElementAt(i).exclWatchAt;   //감시제외여부 
                        mainForm.grd_t0424Excl.Rows[findIndex].Cells["e_price"          ].Value = t0424VoList.ElementAt(i).price;   //현재가격
                    }
                    else
                    {
                        //감시제외 그리드에 추가
                        ExcludeT0424Vo excludeT0424Vo = new ExcludeT0424Vo();
                        excludeT0424Vo.expcode         = t0424VoList.ElementAt(i).expcode;
                        excludeT0424Vo.hname           = t0424VoList.ElementAt(i).hname;
                        //excludeT0424Vo.targClearPrc    = t0424VoList.ElementAt(i).targClearPrc;
                        //excludeT0424Vo.secEntPrc       = t0424VoList.ElementAt(i).secEntPrc;
                        //excludeT0424Vo.secEntAmt       = t0424VoList.ElementAt(i).secEntAmt;
                        //excludeT0424Vo.stopPrc         = t0424VoList.ElementAt(i).stopPrc;
                        excludeT0424Vo.exclWatchAt     = t0424VoList.ElementAt(i).exclWatchAt;
                        excludeT0424Vo.price           = t0424VoList.ElementAt(i).price;
                        this.excludeT0424VoList.Add(excludeT0424Vo);
                        int addIndex = excludeT0424VoList.Count() - 1;
                        mainForm.t0424ExclPriceChangedProcess(addIndex);
                    }
                }

                if (t0424VoList.ElementAt(i).exclWatchAt == "N")
                {//감시제외 그리드에서 삭제.
                    int tmpIndex = this.excludeT0424VoList.Find("expcode", t0424VoList.ElementAt(i).expcode.Replace("A", ""));
                    if (tmpIndex >= 0)
                    {
                        this.excludeT0424VoList.RemoveAt(tmpIndex);
                    }
                }
            }

        }//fn END



        //목표 수익율 도달 Test 후 도달여부에 따라 매도 호출
        public Boolean tradingStopTest(T0424Vo t0424Vo, int index, Double 목표수익율){
            //try{
                Double 수익율       = t0424Vo.sunikrt;
                Double 현재가격     = t0424Vo.price;
                String 감시제외여부 = t0424Vo.exclWatchAt;                    //감시제외여부
                
                //종목 에러 있으면 매매 하지 않는다.
                if (t0424Vo.errorcd != "" && t0424Vo.errorcd != null) return false;
              
                //이미 주문이 실행된 상태
                if (t0424Vo.orderAt == "Y") return false;
               
                if (감시제외여부 == "Y")
                {
                    mainForm.grd_t0424.Rows[index].DefaultCellStyle.BackColor = Color.Gray;
                    return false;
                }
                
                Double 평균단가     = t0424Vo.pamt; //매도주문일경우 매수금액을 알기위해 넣어준다.
                String 종목코드     = t0424Vo.expcode;
                String 종목명       = t0424Vo.hname;
                Double 현재가       = t0424Vo.price;
                String 매수전량명   = t0424Vo.searchNm;
                Double 수량         = t0424Vo.mdposqt;
                String 상세매매구분 = null;
                String 매매구분     = null;

                //매도--------------------------------------------------------------------------------
                if (mainForm.cbx_sell_at.Checked) {
                    if (Util.isSellTime())
                    {
                        //손절
                        Boolean 손절기능여부 = Properties.Settings.Default.STOP_LOSS_AT;
                        if (손절기능여부)
                        {
                            Double 손절율 = Properties.Settings.Default.STOP_LOSS_RATE;
                            if (수익율 <= 손절율)
                            {
                                매매구분     = "매도";
                                상세매매구분 = "손절";
                            }
                        }
                        if (종목코드 == "005070")
                        {
                            String tessst = "3";
                        }
                        
                        //목표수익 달성후 지정 비율 하락이상 반전시 매도
                        Double 추적청산율 = Double.Parse(t0424Vo.maxRt) + Double.Parse(Properties.Settings.Default.STOP_TARGET_DOWN_RATE);
                        if (Double.Parse(Properties.Settings.Default.STOP_TARGET_DOWN_RATE) < 0)
                        {//설정값이 0이면 사용하지 않는다.
                            if (Double.Parse(t0424Vo.maxRt) >= 목표수익율 && 수익율 <= 추적청산율)//최대도달 수익율
                            {
                                매매구분     = "매도";
                                상세매매구분 = "추적청산";
                            }
                        } else {
                            //목표수익 달성시...
                            if (수익율 >= 목표수익율)
                            {
                                //ordptnDetail(상세주문구분-신규매수|반복매수|금일매도|청산)
                                //upOrdno(상위매수주문번호-금일매도일때만 셋팅될것같다)
                                //upExecprc">상위체결금액-없으면 평균단가 넣어주자</param>
                                //IsuNo(종목코드) Quantity(수량) Price(가격)
                                //2틀연속 DataLog 의 매수단가가 잘못 들어가는것들이 있어서 원인을 찾기전에 수익율 < 수익율 2 인경우 주문을 제한하자.메세지창으로 관리하자.
                                //청산일때만 체크
                                if (t0424Vo.sunikrt < 1)
                                {
                                    Log.WriteLine("<ERROR-수익청산><t0424-496:" + t0424Vo.hname + "><" + 수익율.ToString() + ">");
                                    t0424Vo.errorcd = "sunikrt error";
                                    return false;
                                }

                                매매구분     = "매도";
                                상세매매구분 = "수익청산";
                            }
                        } 
                    }
                    // 일괄매도 - 매도시간 설정값 영향 없이 단독 실행
                    if (Properties.Settings.Default.ALL_SELL_AT)
                    {
                        TimeSpan nowTimeSpan = TimeSpan.Parse(mainForm.xing_t0167.hour + ":" + mainForm.xing_t0167.minute + ":" + mainForm.xing_t0167.second);
                        DateTime allSellTimeFrom = Properties.Settings.Default.ALL_SELL_TIME_FROM;
                        DateTime allSellTimeTo = Properties.Settings.Default.ALL_SELL_TIME_TO;
                        목표수익율 = Double.Parse(Properties.Settings.Default.ALL_SELL_RATE);
                        if (nowTimeSpan >= allSellTimeFrom.TimeOfDay && nowTimeSpan <= allSellTimeTo.TimeOfDay)
                        {
                            if (Properties.Settings.Default.ALL_SELL_RATE_SE.Equals("이상"))
                            {
                                if (수익율 >= 목표수익율)
                                {
                                    매매구분     = "매도";
                                    상세매매구분 = "일괄매도";
                                }
                            }
                            if (Properties.Settings.Default.ALL_SELL_RATE_SE.Equals("이하"))
                            {
                                if (수익율 < 목표수익율)
                                {
                                    매매구분     = "매도";
                                    상세매매구분 = "일괄매도";
                                }
                            }
                        }
                    }

                    
                    String 지정시간초과수익율 = Properties.Settings.Default.SELL_TARGET_TIME_OVR_RATE;
                    if (Properties.Settings.Default.SELL_TARGET_TIME_AT && 수익율 < Double.Parse(지정시간초과수익율))
                    {
                        Boolean targetTimeFlag = false;
                        String minute = mainForm.xing_t0167.minute;
                        if (Properties.Settings.Default.SELL_TARGET_TIME_10) { if (minute == "11") targetTimeFlag = true; }
                        if (Properties.Settings.Default.SELL_TARGET_TIME_20) { if (minute == "21") targetTimeFlag = true; }
                        if (Properties.Settings.Default.SELL_TARGET_TIME_30) { if (minute == "31") targetTimeFlag = true; }
                        if (Properties.Settings.Default.SELL_TARGET_TIME_40) { if (minute == "41") targetTimeFlag = true; }
                        if (Properties.Settings.Default.SELL_TARGET_TIME_50) { if (minute == "51") targetTimeFlag = true; }
                        if (Properties.Settings.Default.SELL_TARGET_TIME_00) { if (minute == "01") targetTimeFlag = true; }
                        if (!targetTimeFlag) return false;
                    }

                }//mainForm.cbx_sell_at.Checked end

                //추가 매수--------------------------------------------------------------------------------
                if (mainForm.cbx_buy_at.Checked)
                {
                    if (Util.isBuyTime()) { 
                        //지정 하락 비율 이하이면 추가매수 1번만.
                        if (Properties.Settings.Default.ADD_BUY_AT)
                        {
                            if (int.Parse(t0424Vo.buyCnt) < 2 && 수익율 < Double.Parse(Properties.Settings.Default.ADD_BUY_RATE))  {
                                매매구분     = "매수";
                                상세매매구분 = "추가매수(1회)";
                            }
                        }
                    }
                }
                
                //주문.
                if (매매구분 != null)
                {
                    t0424Vo.orderAt = "Y";

                    Xing_CSPAT00600 xing_CSPAT00600 = mainForm.CSPAT00600Mng.get600();
                    xing_CSPAT00600.call_request(종목명, 종목코드, 매매구분, 수량,  현재가, 매수전량명, 평균단가, 상세매매구분);
                    //청산 주문여부를 true로 설정  
                    
                    
                    mainForm.log("<t0424:" + 상세매매구분 + "> " + 종목명 + " " + 수익율 + "% " + 수량 + "주 " + 현재가 + "원<" + 매수전량명 + ">");
                }

           // }
            //catch (Exception ex){
            //    Log.WriteLine("t0424 : " + ex.Message);
            //    Log.WriteLine("t0424 : " + ex.StackTrace);
            //}
            return false;
        }//stopProFitTarget end

       

        //매매이력 DB와 동기화
        public void t0424histoySync()
        {
            
            for (int i = 0; i < this.t0424VoList.Count(); i++)
            {
                //1.보유종목 이력 존재 여부확인
                SummaryVo summaryVo = mainForm.tradingHistory.getSummaryVo(this.t0424VoList.ElementAt(i).expcode.Replace("A", ""));
                if (summaryVo != null)
                {
                    //2.매도가능수량 같은지 확인
                    if (this.t0424VoList.ElementAt(i).mdposqt.ToString() != summaryVo.sumMdposqt)
                    {
                        //3.같지않으면 삭세후 새로 등록.
                        mainForm.tradingHistory.isunoDelete(this.t0424VoList.ElementAt(i).expcode); //종목일괄삭제.
                        this.t0424VoToHistoryInsert(this.t0424VoList.ElementAt(i), i.ToString());//이력등록
                    }
                }else{
                    //이력정보가 없으면 이력정보를 등록해준다.
                    this.t0424VoToHistoryInsert(this.t0424VoList.ElementAt(i), i.ToString());//이력등록
                }//grd_t0424 for END
            }
        }//histoySync END
        //종목매수 정보를 이력 DB에 저장 
        public void t0424VoToHistoryInsert(T0424Vo t0424Vo,String index)
        {
            //이력정보가 없으면 이력정보를 등록해준다.
            t0424Vo.expcode.Replace("A", "");
            //t0424Vo.pamt2 = t0424Vo.pamt;    //평균단가2
            t0424Vo.sellCnt = "0";  //매도 횟수.
            t0424Vo.buyCnt = "1";  //매수 횟수
            //t0424Vo.sellSunik = "0";//중간매도손익
            /////////////////////////DB 신규저장/////////////////////////////
           
            TradingHistoryVo dataLogVo = new TradingHistoryVo();
            dataLogVo.ordno         = "I" + DateTime.Now.ToString("yyMMddHHmmss")+ index; //주문번호
            dataLogVo.dt            = DateTime.Now.ToString("yyyyMMddHHmmss");
            dataLogVo.accno         = mainForm.account;                    //계좌번호
            dataLogVo.ordptncode    = "02";                                //주문구분 01:매도|02:매수
            dataLogVo.Isuno         = t0424Vo.expcode.Replace("A", "");    //종목코드
            dataLogVo.ordqty        = t0424Vo.mdposqt.ToString();          //주문수량 - 매도가능수량
            dataLogVo.execqty       = t0424Vo.mdposqt.ToString();          //체결수량 - 매도가능수량
            dataLogVo.ordprc        = t0424Vo.pamt.ToString();              //주문가격 - 평균단가
            dataLogVo.execprc       = t0424Vo.pamt.ToString();              //체결가격 - 평균단가
            dataLogVo.Isunm         = t0424Vo.hname;                        //종목명
            dataLogVo.ordptnDetail  = "동기화";                             //상세 주문구분 신규매수|반복매수|금일매도|청산 
            dataLogVo.upOrdno       = dataLogVo.ordno;                      //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            dataLogVo.upExecprc     = "0";                                  //상위체결가격
            dataLogVo.sellOrdAt     = "N";                                  //매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
            dataLogVo.useYN         = "Y";                                  //사용여부

            dataLogVo.ordermtd = "XING API";

            //2.DB에 해당 주문 정보가 없을때 처리해줘야한다. volist로 변경후 테스트한후에 필요하면 처리로직 추가해주자.
            mainForm.tradingHistory.insert(dataLogVo); //쿼리 호출
            //mainForm.tradingHistory.getTradingHistoryVoList().Add(dataLogVo);
            //mainForm.dataLog.dbSync();
            Log.WriteLine("t0424::최초 이력 등록" + t0424Vo.hname + "(" + t0424Vo.expcode + ") . ");
            //프로그램 최초에만 동작해야 하는데 신규매수 매도시 이력 등록이 잘 안된다는 뜻이다.
         }

        private int callCnt = 0;
        /// <summary>
        /// 종목검색 호출
        /// </summary>
        public void call_request(String account, String accountPw)
        {

            //if (completeAt)
            //{
                this.completeAt = false;//중복호출 방지

                base.SetFieldData("t0424InBlock", "accno"       , 0, account);    // 계좌번호
                base.SetFieldData("t0424InBlock", "passwd"      , 0, accountPw);  // 비밀번호
                base.SetFieldData("t0424InBlock", "prcgb"       , 0, "1");        // 단가구분 : 1-평균단가, 2:BEP단가
                base.SetFieldData("t0424InBlock", "chegb"       , 0, "2");        // 체결구분 : 0-결제기준, 2-체결기준
                base.SetFieldData("t0424InBlock", "dangb"       , 0, "0");        // 단일가구분 : 0-정규장, 1-시간외단일가 
                base.SetFieldData("t0424InBlock", "charge"      , 0, "1");        // 제비용포함여부 : 0-미포함, 1-포함
                base.SetFieldData("t0424InBlock", "cts_expcode" , 0, "");         // CTS종목번호 : 처음 조회시는 SPACE
                
                base.Request(false);  //연속조회일경우 true

                //폼 메세지.
                mainForm.input_t0424_log.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "><t0424:잔고조회>";

            ////}else{
            //    mainForm.input_t0424_log.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "><t0424:중복>";

            //    callCnt++;
            //    if (callCnt == 5)
            //    {
            //        this.completeAt = true;
            //        callCnt = 0;
            //    }
            ////}
        }	// end function

    } //end class 




    public class T0424Vo : SummaryVo
    {
        public String  expcode   { set; get; } //종목코드
        public String  hname     { set; get; } //종목명
        public Double  mdposqt   { set; get; } //매도가능
        public Double  price     { set; get; } //현재가
        public Double  appamt    { set; get; } //평가금액
        public Double  dtsunik   { set; get; } //평가손익 
        public Double  sunikrt   { set; get; } //수익율
        public Double  pamt      { set; get; } //평균단가
        public Double  mamt      { set; get; } //매입금액
        public Double  fee       { set; get; } //수수료
        public Double  tax       { set; get; } //제세금
        public String jonggb     { set; get; } //종목구분[코스닥|코스피]

        public String  orderAt   { set; get; } //주문여부
        public String  errorcd   { set; get; } //에러코드

        public String deleteAt   { set; get; } //삭제 여부 - 잔고 제외

                                               //public String sunikrt2 { set; get; } //손익율2 --계산값
                                               //String 목표청산가격     = t0424Vo.targClearPrc.Replace(",", "");    //목표청산가격    - 감시제외 일때 사용
                                               //String 추가진입가격     = t0424Vo.secEntPrc.Replace(",", "");      //2차진입가격     - 감시제외 일때 사용 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
                                               //String 추가진입금액     = t0424Vo.secEntAmt.Replace(",", "");     //2차진입비중가격 - 감시제외 일때 사용
                                               //String 감시제외손절가격 = t0424Vo.stopPrc.Replace(",", "");   //손절가격 - 감시제외 일때 사용


        //public String pamt2 { set; get; } //평균단가2
        //public String buyCnt     { set; get; } //매수 횟수
        //public String sellCnt    { set; get; } //매도 횟수   
        //public String sellSunik  { set; get; } //중간매도손익
        //public String firstBuyDt { set; get; } //최초진입일시

    }

    public class ExcludeT0424Vo
    {
        public String expcode       { set; get; } //종목코드
        public String hname         { set; get; } //종목명
        public Double price         { set; get; } //현재가
        //public String pamt2         { set; get; } //평균단가2

        //목표수익율-안만들어도 될듯 당일 기준으로 업데이트 해주자.
       // public String targClearPrc  { set; get; }//목표청산가격    - 감시제외 일때 사용
        //public String secEntPrc     { set; get; }//2차진입가격     - 감시제외 일때 사용 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
        //public String secEntAmt     { set; get; }//2차진입비중가격 - 감시제외 일때 사용
        //public String stopPrc       { set; get; }//손절가격 - 감시제외 일때 사용
        public String exclWatchAt   { set; get; }//감시제외여부

    }

}   // end namespace
