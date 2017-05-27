
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

        private EBindingList<T0424Vo> t0424VoList;

        public Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public Double sunamt;    //추정자산
        //public int sunamt1;     //d1예수금
        public Double dtsunik;   //실현손익

        public Double tmpMamt;      //매입금액
        public Double tmpTappamt;   //평가금액
        public Double tmpTdtsunik;  //평가손익

        public Double mamt;      //매입금액
        public Double tappamt;   //평가금액
        public Double tdtsunik;  //평가손익
       

        public int h_totalCount;

        
        // 생성자
        public Xing_t0424()
        {
            base.ResFileName = "₩res₩t0424.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            t0424VoList = new EBindingList<T0424Vo>();
            t0424VoList.ResetBindings();

        }   // end function

        // 소멸자
        ~Xing_t0424()
        {

        }

        public EBindingList<T0424Vo> getT0424VoList()
        {
            return this.t0424VoList;
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

                T0424Vo tmpT0424Vo;

                String expcode;//종목코드
                String jonggb;//마켓구분
                String mdposqt;//매도가능

                for (int i = 0; i < blockCount; i++)
                {
                    expcode = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드
                    mdposqt = base.GetFieldData("t0424OutBlock1", "mdposqt", i); //매도가능
                                                                                 //hname   = base.GetFieldData("t0424OutBlock1", "hname"  , i); //종목명
                                                                                 //sunikrt = float.Parse(base.GetFieldData("t0424OutBlock1", "sunikrt", i)); //수익율
                                                                                 //price   = base.GetFieldData("t0424OutBlock1", "price", i); //현재가


                    //1.t0424 는 매도가능수량이 0 건인것은 없어야하는데 가끔 검색되어 나온다...그냥 스킵하자.
                    if (Double.Parse(mdposqt) <= 0)
                    {
                        //Log.WriteLine("t0424 는 매도가능수량이 0 건인것은 조회 되지 말아야하는데... 가끔 검색되어 나오는지 잘 모르겠다...");
                        //Log.WriteLine("t0424::" + base.GetFieldData("t0424OutBlock1", "hname", i) + "(" + expcode + ")t0424 는 매도가능수량이 0 건인것은 조회 되지 말아야하는데... ");
                        continue;
                    }


                    int findIndex = t0424VoList.Find("expcode", expcode);
                    if (findIndex >= 0)
                    {
                        tmpT0424Vo = this.t0424VoList.ElementAt(findIndex);
                        mainForm.grd_t0424.Rows[findIndex].Cells["c_expcode"].Value = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드
                        mainForm.grd_t0424.Rows[findIndex].Cells["c_hname"].Value = base.GetFieldData("t0424OutBlock1", "hname", i); //종목명
                        mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Value = base.GetFieldData("t0424OutBlock1", "mdposqt", i); //매도가능
                        mainForm.grd_t0424.Rows[findIndex].Cells["price"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "price", i)); //현재가
                        mainForm.grd_t0424.Rows[findIndex].Cells["appamt"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "appamt", i)); //평가금액
                        mainForm.grd_t0424.Rows[findIndex].Cells["dtsunik"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "dtsunik", i)); //평가손익
                        mainForm.grd_t0424.Rows[findIndex].Cells["c_sunikrt"].Value = base.GetFieldData("t0424OutBlock1", "sunikrt", i); //수익율
                        mainForm.grd_t0424.Rows[findIndex].Cells["pamt"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "pamt", i)); //평균단가
                        mainForm.grd_t0424.Rows[findIndex].Cells["mamt"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mamt", i)); //매입금액
                        mainForm.grd_t0424.Rows[findIndex].Cells["msat"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "msat", i)); //당일매수금액
                        mainForm.grd_t0424.Rows[findIndex].Cells["mpms"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mpms", i)); //당일매수단가
                        mainForm.grd_t0424.Rows[findIndex].Cells["mdat"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mdat", i)); //당일매도금액
                        mainForm.grd_t0424.Rows[findIndex].Cells["mpmd"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mpmd", i)); //당일매도단가
                        mainForm.grd_t0424.Rows[findIndex].Cells["fee"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "fee", i)); //수수료
                        mainForm.grd_t0424.Rows[findIndex].Cells["tax"].Value = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "tax", i)); //제세금
                        mainForm.grd_t0424.Rows[findIndex].Cells["sininter"].Value = base.GetFieldData("t0424OutBlock1", "sininter", i); //신용이자   
                    }
                    else
                    {
                        tmpT0424Vo = new T0424Vo();

                        tmpT0424Vo.expcode = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드
                        tmpT0424Vo.hname = base.GetFieldData("t0424OutBlock1", "hname", i); //종목명
                        tmpT0424Vo.mdposqt = base.GetFieldData("t0424OutBlock1", "mdposqt", i); //매도가능
                        tmpT0424Vo.price = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "price", i)); //현재가
                        tmpT0424Vo.appamt = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "appamt", i)); //평가금액
                        tmpT0424Vo.dtsunik = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "dtsunik", i)); //평가손익
                        tmpT0424Vo.sunikrt = base.GetFieldData("t0424OutBlock1", "sunikrt", i); //수익율
                        tmpT0424Vo.pamt = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "pamt", i)); //평균단가
                        tmpT0424Vo.mamt = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mamt", i)); //매입금액
                        tmpT0424Vo.msat = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "msat", i)); //당일매수금액
                        tmpT0424Vo.mpms = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mpms", i)); //당일매수단가
                        tmpT0424Vo.mdat = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mdat", i)); //당일매도금액
                        tmpT0424Vo.mpmd = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "mpmd", i)); //당일매도단가
                        tmpT0424Vo.fee = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "fee", i)); //수수료
                        tmpT0424Vo.tax = Util.GetNumberFormat(base.GetFieldData("t0424OutBlock1", "tax", i)); //제세금
                        tmpT0424Vo.sininter = base.GetFieldData("t0424OutBlock1", "sininter", i); //신용이자

                        tmpT0424Vo.orderAt = false;
                        //1.그리드에 없던 새로 매수된 종목이면 테이블에 추가해준다.
                        this.t0424VoList.Add(tmpT0424Vo);


                        //실시간 현재가 종목  등록
                        jonggb = base.GetFieldData("t0424OutBlock1", "jonggb", i); //종목코드
                        //코스피
                        if (jonggb == "3")
                        {
                            //Log.WriteLine("dddd" + tmpT0424Vo.expcode + "/" + jonggb);
                            mainForm.real_S3.call_real(tmpT0424Vo.expcode);
                        }
                        //코스닥
                        if (jonggb == "2")
                        {
                            mainForm.real_K3.call_real(tmpT0424Vo.expcode);
                        }
                    }

                    //삭제여부
                    tmpT0424Vo.deleteAt = "N";

                    //종목매매 이력 참조

                    HistoryVo historyvo = mainForm.dataLog.getHistoryVo(tmpT0424Vo.expcode.Replace("A", ""));
                    if (historyvo != null)
                    {
                        tmpT0424Vo.pamt2 = historyvo.pamt2;    //평균단가2
                        tmpT0424Vo.sellCnt = historyvo.sellCnt;  //매도 횟수.
                        tmpT0424Vo.buyCnt = historyvo.buyCnt;   //매수 횟수
                        tmpT0424Vo.sellSunik = historyvo.sellSunik;//중간매도손익
                    }
                    else
                    {//이력정보가 없으면 이력정보를 등록해준다. --프로그램 최초에만 동작해야한다.
                        tmpT0424Vo.expcode.Replace("A", "");
                        tmpT0424Vo.pamt2 = tmpT0424Vo.pamt;    //평균단가2
                        tmpT0424Vo.sellCnt = "0";  //매도 횟수.
                        tmpT0424Vo.buyCnt = "1";   //매수 횟수
                        tmpT0424Vo.sellSunik = "0";//중간매도손익

                        mainForm.dataLog.insertDataT0424(tmpT0424Vo, "init" + (mainForm.dataLog.getDataLogVoList().Count() + 1));

                        Log.WriteLine("t0424::" + tmpT0424Vo.hname + "(" + tmpT0424Vo.expcode + ") 최초 이력 등록. ");

                        //프로그램 최초에만 동작해야 하는데 신규매수 매도시 이력 등록이 잘 안된다는 뜻이다.

                    }
                    //매매이력에 따른 손익율 재계산.
                    tmpT0424Vo.sunikrt2 = Util.getSunikrt2(tmpT0424Vo);

                    //손익률 체크 - 원인 찾으면 나주에 지워주자.
                    //2틀연속 DataLog 의 매수단가가 잘못 들어가는것들이 있어서 원인을 찾기전에 수익율 < 수익율 2 인경우 주문을 제한하자.메세지창으로 관리하자.
                    if ((Double.Parse(tmpT0424Vo.sunikrt)+1) < (Double.Parse(tmpT0424Vo.sunikrt2) ))
                    {
                        //Log.WriteLine("t0424 :: 수익률 이상함 dataLog 확인해보자.[" + tmpT0424Vo.hname + "(" + tmpT0424Vo.expcode + ")]  수익율:" + tmpT0424Vo.sunikrt + "%    수익율2:" + tmpT0424Vo.sunikrt2+"/" + (Double.Parse(tmpT0424Vo.sunikrt) + 101).ToString()+"/"+ (Double.Parse(tmpT0424Vo.sunikrt2) + 100).ToString());
                        //MessageBox.Show("t0424 :: 수익률 이상함 dataLog 확인해보자.[" + tmpT0424Vo.hname + "(" + tmpT0424Vo.expcode + ")]  수익율:" + tmpT0424Vo.sunikrt + "%    수익율2:" + tmpT0424Vo.sunikrt2);
                    }

                }//for end

                // 계좌정보 써머리 계산 - 연속 조회이기때문에 합산후 마지막에 폼으로 출력.
                this.tmpMamt     += int.Parse(base.GetFieldData("t0424OutBlock", "mamt"    , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "mamt"    , 0));//매입금액
                this.tmpTappamt  += int.Parse(base.GetFieldData("t0424OutBlock", "tappamt" , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tappamt" , 0));//평가금액
                this.tmpTdtsunik += int.Parse(base.GetFieldData("t0424OutBlock", "tdtsunik", 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tdtsunik", 0));//평가손익


                this.h_totalCount += blockCount;

                //2.연속 데이타 정보가 남아있는지 구분
                if (base.IsNext)
                //if (cts_expcode != "")
                {
                    //연속 데이타 정보를 호출.
                    base.SetFieldData("t0424InBlock", "cts_expcode", 0, cts_expcode);      // CTS종목번호 : 처음 조회시는 SPACE
                    base.Request(true); //연속조회일경우 true
                }else{//마지막 데이타일때 메인폼에 출력해준다.
                    //this.sunamt1 = int.Parse(this.GetFieldData("t0424OutBlock", "sunamt1", 0));// D1예수금
                    this.dtsunik = Double.Parse(this.GetFieldData("t0424OutBlock", "dtsunik", 0));// 실현손익

                    this.mamt = this.tmpMamt;      //매입금액
                    this.tappamt = this.tmpTappamt;   //평가금액
                    this.tdtsunik = this.tmpTdtsunik;  //평가손익

                    //mainForm.input_sunamt.Text      = Util.GetNumberFormat((this.sunamt1 + this.tappamt).ToString()); // 추정순자산 - sunamt 값이 이상해서  추정순자산 = 평가금액 + D1예수금 
                    mainForm.label_dtsunik.Text = Util.GetNumberFormat(this.dtsunik);  // 실현손익
                    mainForm.h_totalCount.Text = this.h_totalCount.ToString();       //종목수

                    //label 출력
                    mainForm.label_mamt.Text = Util.GetNumberFormat(this.mamt);    // 매입금액

                    //로그 및 중복 요청 처리 2:코스닥, 3:코스피
                    mainForm.input_t0424_log2.Text = "[" + mainForm.label_time.Text + "]t0424 :: 잔고조회 완료";


                    //매매거래 가능 시간이고 매매가능여부 값이 Y일때 체크후 매도 로직 호출
                    if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 910 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1520)
                    {
                        if (mainForm.tradingAt == "Y")
                        {
                            this.stopProFitTarget();
                        }
                    }

                    //응답처리 완료
                    completeAt = true;


                }//end
            }catch (Exception ex)
            {
                Log.WriteLine("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.StackTrace);
            }
            
        }


        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
            if (nMessageCode == "00000") {
                ;
            }else {
                //Log.WriteLine("[" + mainForm.input_time.Text + "]t0424 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t0424_log2.Text = "[" + mainForm.label_time.Text + "]t0424 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지

                //서버접속 실패로인하여 로그인 여부를 false 로 설정한다.후에 접속실패 코드확보후 조건문 추가해주자.
                //mainForm.exXASessionClass.loginAt = false;
                //00007 :: 시스템 사정으로 자료 서비스를 받을 수 없습니다.
                //00008 :: 시스템 문제로 서비스가 불가능 합니다.
            }
        }

        private int callCnt = 0;
        /// <summary>
        /// 종목검색 호출
        /// </summary>
        public void call_request(String account, String accountPw )
        {
           
            if (completeAt) {
                this.completeAt = false;//중복호출 방지
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
                this.tmpMamt      = 0; //매입금액
                this.tmpTappamt   = 0; //평가금액
                this.tmpTdtsunik  = 0; //평가손익
                this.h_totalCount = 0; //보유종목수
           
                base.Request(false);  //연속조회일경우 true
                
                //폼 메세지.
                mainForm.input_t0424_log.Text = "["+ mainForm.label_time.Text+ "]t0424::잔고조회";

            } else {
                mainForm.input_t0424_log.Text = "[" + mainForm.label_time.Text + "][중복]t0424::잔고조회";

                callCnt++;
                if (callCnt == 5)
                {
                    this.completeAt = true;
                    callCnt = 0;
                }
            }


        }	// end function

        ///////////////////////////////////////////////////////////
        //목표 수익율 도달 Test 후 도달여부에 따라 매도 호출 2.팔린종목 삭제
        public void stopProFitTarget()
        {

            for (int i = 0; i < this.t0424VoList.Count(); i++)
            {
                //1.거래가능여부 && 주문중상태가 아니고 && 종목거래 에러 여부
                this.stopProFitTargetTest(t0424VoList.ElementAt(i));

                //3.팔린종목 삭제
                if (this.t0424VoList.ElementAt(i).deleteAt == "Y")
                {
                    Log.WriteLine("t0424::" + t0424VoList.ElementAt(i).hname + "(" + t0424VoList.ElementAt(i).expcode + "):: 팔린종목 그리드에서 제거 ");
                    mainForm.deleteCallBack(this.t0424VoList.ElementAt(i).expcode); //이상하게 반복매수에서 보유종목으로 통과되어서 에러난다. 그래서 아래 0424와 순서를 바꿔줘본다.1833에서 에러남
                    i--;
                }
                this.t0424VoList.ElementAt(i).deleteAt = "Y";
            }

        }//stopProFitTarget end

        //목표 수익율 도달 Test 후 도달여부에 따라 매도 호출
        public Boolean stopProFitTargetTest(T0424Vo t0424Vo)
        {
            String infoStr = "[" + t0424Vo.hname + "(" + t0424Vo.expcode + ")] 수익율: " + t0424Vo.sunikrt + " % 수익율2:" + t0424Vo.sunikrt2+ "수익율:" + t0424Vo.sunikrt + " % 주문가격:" + t0424Vo.price + ",   주문수량:" + t0424Vo.mdposqt+ "에러: "+ t0424Vo.errorcd ;
            String 투입비율   = mainForm.xing_t1833.getInputRate();
            String 제한비율   = Properties.Settings.Default.BUY_STOP_RATE;
            String 목표수익율 = Properties.Settings.Default.STOP_PROFIT_TARGET;

            //자본금이 제한비율 근처까지 투입이 된상태이면 빠른 매매 회전율을 위하여 목표수익율을 낮추어 준다.
            if (Double.Parse(투입비율) > (Double.Parse(제한비율) -5)  )
            {
                목표수익율 = "2.5";
            }
            if (t0424Vo.errorcd != null)
            {
                //Log.WriteLine("t0424 :: 에러코드발새 " + infoStr);
                return false;
            }

            //거래가능여부 && 주문중상태가 아니고 && 종목거래 에러 여부
            if (t0424Vo.orderAt == false && (t0424Vo.errorcd == "" || t0424Vo.errorcd == null))
            {

                //1.매도 가능 &&  수익율 2% 이상 매도 Properties.Settings.Default.SELL_RATE
                //2.수익율2 로 변경함.
                String sunikrt = t0424Vo.sunikrt2 == null ? t0424Vo.sunikrt : t0424Vo.sunikrt2;
                
                if (Double.Parse(sunikrt) >= Double.Parse(목표수익율))
                {
                    Log.WriteLine(sunikrt.ToString() + "목표:" + 목표수익율);
                    if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 910 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1520)
                    {
                        /// <summary>
                        /// 현물정상주문
                        /// </summary>
                        /// <param name="ordptnDetail">상세주문구분-신규매수|반복매수|금일매도|청산</param>
                        /// <param name="upOrdno">상위매수주문번호-금일매도일때만 셋팅될것같다.</param>
                        /// <param name="upExecprc">상위체결금액-없으면 평균단가 넣어주자</param>
                        /// <param name="IsuNo">종목코드</param>
                        /// <param name="Quantity">수량</param>
                        /// <param name="Price">가격</param>
                        if (mainForm.xing_CSPAT00600.completeAt)//주문을 호출할수 있을때 호출하고 못하면 그냥 스킵한다.
                        {
                            //2틀연속 DataLog 의 매수단가가 잘못 들어가는것들이 있어서 원인을 찾기전에 수익율 < 수익율 2 인경우 주문을 제한하자.메세지창으로 관리하자.
                            //청산일때만 체크
                            if (Double.Parse(t0424Vo.sunikrt) < 1)
                            {
                                Log.WriteLine("t0424 :: 청산 수익률이 마이너스 확인해보자 ." + infoStr);
                                t0424Vo.errorcd = "sunikrt error";
                                return false;
                            }

                            mainForm.xing_CSPAT00600.call_requestSell("청산", "none", t0424Vo.pamt2, t0424Vo.hname, t0424Vo.expcode, t0424Vo.mdposqt, t0424Vo.price);

                            Log.WriteLine("t0424 ::청산[" + infoStr);
                            mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t0424:" + t0424Vo.hname + ":청산.");
                            t0424Vo.orderAt = true;//청산 주문여부를 true로 설정    
                            return true;
                        }else
                        {
                            Log.WriteLine("t0424 ::주문스킵(청산)"+infoStr);
                            mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t0424:" + t0424Vo.hname + ":주문스킵(청산)");
                        }
                    }
                    else
                    {
                        //Log.WriteLine("t0424 ::청산무시[" + t0424Vo.hname + "(" + t0424Vo.expcode + ")]  수익율:" + t0424Vo.sunikrt + "%    주문수량:" + t0424Vo.mdposqt);
                        //mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t0424:" + t0424Vo.hname + ":청산무시.");
                    }
                }

               
            }
            return false;
        }//stopProFitTarget end
    } //end class 

    public class T0424Vo
    {
        public String  expcode   { set; get; } //종목코드
        public String  hname     { set; get; } //종목명
        public String  mdposqt   { set; get; } //매도가능
        public String  price     { set; get; } //현재가
        public String  appamt    { set; get; } //평가금액
        public String  dtsunik   { set; get; } //평가손익 
        public String  sunikrt   { set; get; } //수익율
        public String  pamt      { set; get; } //평균단가
        public String  mamt      { set; get; } //매입금액
        public String  msat      { set; get; } //당일매수금액
        public String  mpms      { set; get; } //당일매수단가
        public String  mdat      { set; get; } //당일매도금액
        public String  mpmd      { set; get; } //당일매도단가
        public String  fee       { set; get; } //수수료
        public String  tax       { set; get; } //제세금
        public String  sininter  { set; get; } //신용이자
        public Boolean orderAt   { set; get; }  //주문여부
        public String  errorcd   { set; get; } //에러코드

        public String deleteAt   { set; get; } //삭제 여부

        public String buyCnt     { set; get; } //매수 횟수
        public String sellCnt    { set; get; } //매도 횟수
        public String pamt2      { set; get; } //평균단가2
        public String sellSunik  { set; get; } //중간매도손익
        public String sunikrt2   { set; get; } //손익율2
        //후회할까? 
    }



}   // end namespace
