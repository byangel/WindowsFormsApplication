
using System;
using System.Linq;
using System.Windows.Forms;
using XA_DATASETLib;
using System.Data;
using System.Drawing;



namespace PackageSellSystemTrading{
    public class Xing_t1857 : XAQueryClass{
        
        private DataTable t1857Dt;
        public DataTable gett1857Dt(){
            return this.t1857Dt;
        }
        
        public MainForm mainForm;
        //투자 비율
        public String investmentRatio;

        public Boolean initAt = false;

        private int conditionTotalCnt  = 2;
        private int conditionCallIndex = 0;
        //private String[] conditionNm   = {"역정배", "당일고가", "스윙매수", "5일반등", "단타급등","당일눌림목" };
        private String[] conditionNm = { "RSI검색","RSI_RE"};
        //private String[] conditionNm = { "RSI검색"};

        // 생성자
        public Xing_t1857(){
            
            //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            //base.ResFileName = startupPath+"₩Resources₩t1857.res";
            base.ResFileName = "₩res₩t1857.res";
            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            //base.SetFieldData("t1857InBlock", "sRealFlag", 0, "0");        // 실시간구분 : 0:조회, 1:실시간
            //base.SetFieldData("t1857InBlock", "sSearchFlag", 0, "F");         // 종목검색구분 : F:파일, S:서버
            //base.SetFieldData("t1857InBlock", "query_index", 0, "");         //

            this.t1857Dt = new DataTable();
            t1857Dt.Columns.Add("종목코드"     , typeof(string));
            t1857Dt.Columns.Add("종목명"       , typeof(string));
            t1857Dt.Columns.Add("현재가"       , typeof(string));
            t1857Dt.Columns.Add("전일대비구분" , typeof(string));
            t1857Dt.Columns.Add("전일대비"     , typeof(string));
            t1857Dt.Columns.Add("등락율"       , typeof(string));
            t1857Dt.Columns.Add("거래량"       , typeof(double));
            t1857Dt.Columns.Add("연속봉수"     , typeof(string));
            t1857Dt.Columns.Add("검색조건"     , typeof(string));
            t1857Dt.Columns.Add("삭제여부"     , typeof(string));
            t1857Dt.Columns.Add("설명"         , typeof(string));

        }   // end function


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            try
            {
                //투자율 설정
                this.investmentRatio = Util.getInputRate(mainForm);
                mainForm.label_InvestmentRatio.Text = investmentRatio;

                int iCount = base.GetBlockCount("t1857OutBlock1");

                //매수종목 검색 그리드 초기화
                //mainForm.grd_t1857.Rows.Clear();

                //this.t1857Dt  = (DataTable)mainForm.grd_t1857_dt.DataSource;

                //String shcode;//종목코드
                DataRow tmpRow;
                //DataRow[] foundRows;
                //String sunikrt;//수익률
                for (int i = 0; i < iCount; i++) {

                    String shcode = base.GetFieldData("t1857OutBlock1", "shcode", i);//종목코드
                    
                    DataRow[] foundRows = t1857Dt.Select("종목코드 Like '"+shcode+"'");
                    if (foundRows.Count()>0){
                        tmpRow = foundRows[0];
                    }else{
                        tmpRow = t1857Dt.NewRow();
                    }
                    
                    tmpRow["종목코드"     ] = base.GetFieldData("t1857OutBlock1", "shcode", i); //종목코드
                    tmpRow["종목명"       ] = base.GetFieldData("t1857OutBlock1", "hname" , i); //종목명
                    tmpRow["현재가"       ] = base.GetFieldData("t1857OutBlock1", "price" , i); //현재가
                    tmpRow["전일대비구분" ] = base.GetFieldData("t1857OutBlock1", "sign"  , i); //전일대비구분 
                    tmpRow["전일대비"     ] = base.GetFieldData("t1857OutBlock1", "change", i); //전일대비
                    tmpRow["등락율"       ] = base.GetFieldData("t1857OutBlock1", "diff"  , i); //등락율
                    tmpRow["거래량"       ] = base.GetFieldData("t1857OutBlock1", "volume", i); //거래량
                    tmpRow["연속봉수"     ] = base.GetFieldData("t1857OutBlock1", "jobFlag", i); //연속봉수
                    tmpRow["검색조건"     ] = conditionNm[conditionCallIndex];                  //검색조건
                    tmpRow["삭제여부"     ] = "new";                                            //삭제여부 [new|old]

                    //mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Value = 메도가능수.ToString();
                    if (foundRows.Count() == 0){
                        t1857Dt.Rows.Add(tmpRow);
                    }
                    this.BuyTest(tmpRow["종목코드"].ToString(), tmpRow["종목명"].ToString(), tmpRow["현재가"].ToString(), t1857Dt.Rows.Count - 1, tmpRow["검색조건"].ToString());

                }

                foreach (DataRow dr in t1857Dt.Select()){
                    if (dr["삭제여부"].ToString() == "old"){
                        dr.Delete();
                    }else{
                        dr["삭제여부"] = "old";
                    }
                }
                mainForm.input_t1857_log1.Text = "[" + mainForm.label_time.Text+ "][" + conditionNm[conditionCallIndex] + "]조건검색 응답 완료";
                
                //정상호출시 처리
                this.conditionCallIndex = this.conditionCallIndex + 1;
                this.conditionCallIndex = this.conditionCallIndex < this.conditionTotalCnt ? this.conditionCallIndex : 0;


            }
            catch (Exception ex)
            {
                Log.WriteLine("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.StackTrace);
            }
        }


        //메세지 이벤트 핸들러
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
            
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            }else if (nMessageCode == "03563"){
                mainForm.input_t1857_log1.Text = "정규장 시간이 아닙니다. 트레이딩 종료";
                mainForm.tradingAt = "N";
            } else { 
                //Log.WriteLine("t1857 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t1857_log1.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]t1857:" + nMessageCode + ":" + szMessage;
            }
        }
        
        //진입검색에서 검색된 종목을 매수한다.
        private Boolean BuyTest(String shcode,String hname, String close,int addIndex, String searchMod)
        {
            //매수금지목록
            EBindingList<T1857Vo> t1857ExcludeVoList = mainForm.xing_t1857Exclude.getT1857ExcludeVoList();
            int t1857ExcludeVoListFindIndex = t1857ExcludeVoList.Find("shcode", shcode);
            int t0424VoListFindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode);//보유종목인지 체크
            //매수금지종목페인팅
            if (t1857ExcludeVoListFindIndex >= 0)
            {
                mainForm.grd_t1833_dt.Rows[addIndex].Cells["종목명"].Style.BackColor = Color.Red;
                //만약에 보유종목일경우 보유종목도 색으로 표현해주자.
                if (t0424VoListFindIndex >= 0){
                    mainForm.grd_t0424.Rows[t0424VoListFindIndex].Cells["c_expcode"].Style.BackColor = Color.Red;
                }
               
            }
            //매매 시간 채크-장 막판에만 매수한다.
            int nowTime = int.Parse(mainForm.xing_t0167.time.Substring(0, 4));
            if (nowTime > 1500 && nowTime < 1519){
                mainForm.label_trading_condition.Text = "[" + mainForm.label_time.Text + "]매수 가능 시간.";
                   
            }else{
                mainForm.label_trading_condition.Text = "[" + mainForm.label_time.Text + "]매수 금지 시간.";
                return false;
            }
            
            
            String ordptnDetail; //매수 상세 구분을 해준다. 신규매수|반복매수
            

            //금일 매수 체결 내용이 있고 미체결 잔량이 0인 건은 매수 하지 않는다.
            var toDayBuyT0425VoList = from item in mainForm.xing_t0425.getT0425VoList()
                                where item.expcode == shcode && item.medosu == "매수" 
                                      select item;
            if (toDayBuyT0425VoList.Count() > 0 )
            {
                Log.WriteLine("t1857::금일 1회 매수 제한:" + hname + "(" + shcode + ")["+ searchMod + "] ");
                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1857::[ " + hname + " ]: 금일 1회 매수 제한.[" + searchMod + "] ");
                return false;
            }

            //5.보유종목 반복매수여부 테스트 -두번째 컨디션일 경우 보유종목일경우에만 중복 매수한다.
            if (t0424VoListFindIndex >= 0){
                ordptnDetail = "반복매수";

                EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
                String sunikrt = (String)t0424VoList.ElementAt(t0424VoListFindIndex).sunikrt2;//기존 종목 수익률

                //-수익율이 REPEAT_RATE(설정값이하로 떨어졌을때 반복매수 해주자.
                if (double.Parse(sunikrt) > double.Parse(Properties.Settings.Default.REPEAT_RATE)){
                    Log.WriteLine("t1857::반복매수 제한:" + hname + "(" + shcode + ")[수익률:" + sunikrt + "%|설정수익률:" + Properties.Settings.Default.REPEAT_RATE + "%][" + searchMod + "]");
                    mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1857::[ " + hname + " ]:반복매수 제한.[" + searchMod + "]");
                    return false;
                }
                //1.반복매수면 투자율 제한 하지 않는다.
                //2.반복매수면 매수금지 종목이라도 매수한다.

            }
            else
            {//-보유종목이 아니고 신규매수해야 한다면.
                ordptnDetail = "신규매수";
                //자본금대비 투자 비율이 높으면 신규매수 하지 않는다.
                if (Double.Parse(this.investmentRatio) > Double.Parse(Properties.Settings.Default.BUY_STOP_RATE))
                {
                    Log.WriteLine("t1857::투자율 제한:" + hname + "(" + shcode + ")[투자율:" + investmentRatio + "%|설정비율:" + Properties.Settings.Default.BUY_STOP_RATE + "%][" + searchMod + "]");
                    mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0, 5) + "]t1857::" + hname + ":투자율 제한.[" + searchMod + "]");
                    return false;
                }

                //검색조건이 두번째일경우 신규매수하지 않는다.
                if (conditionCallIndex == 1)
                {
                    //Log.WriteLine("t1857::반복매수조건 금지:" + hname + "(" + shcode + ")");
                    return false;
                }
                //매수금지종목이면 무조건 패스
                if (t1857ExcludeVoListFindIndex >= 0)
                {
                    Log.WriteLine("t1857::매수금지 종목:" + hname + "(" + shcode + ")");
                    mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0, 5) + "]t1857::[ " + hname + " ]:매수금지 종목[" + searchMod + "]");

                    return false;
                }
                
            }

            //4.매수
            int battingAtm = int.Parse(mainForm.label_battingAtm.Text.Replace(",",""));
            //임시로 넣어둔다 왜 현제가가 0으로 넘어오는지 모르겠다.
            if (close == "0"){
                Log.WriteLine("t1857::" + hname + "[ " + shcode + " ] [현제가:" + close+ "][" + searchMod + "]");
                return false;
            }
            
            //-매수수량 계산.
            int Quantity = battingAtm / int.Parse(close);
            //int Quantity = 20000;
          
            /// <summary>
            /// 현물정상주문
            /// </summary>
            /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
            /// <param name="IsuNo">종목번호</param>
            /// <param name="Quantity">수량</param>
            /// <param name="Price">가격</param>
            Xing_CSPAT00600 xing_CSPAT00600 = mainForm.CSPAT00600Mng.get600();

            xing_CSPAT00600.ordptnDetail = ordptnDetail;        //상세 매매 구분.
            xing_CSPAT00600.shcode       = shcode;              //종목코드
            xing_CSPAT00600.hname        = hname;               //종목명
            xing_CSPAT00600.quantity     = Quantity.ToString(); //수량
            xing_CSPAT00600.price        = close;               //가격
            xing_CSPAT00600.divideBuySell= "2";                 // 매매구분: 1-매도, 2-매수
            xing_CSPAT00600.upOrdno      = "";                  //상위매수주문 - 금일매도매수일때만 값이 있다.
            xing_CSPAT00600.upExecprc    = "";                  //상위체결금액 
            xing_CSPAT00600.eventNm      = searchMod;           //이벤트명(검색조건명이나 매도이유가 들어간다.)
            //매수 실행
            xing_CSPAT00600.call_request();

            Log.WriteLine("t1857::검색주문" + hname + "(" + shcode + ") " + ordptnDetail + "   [주문가격:" + close + "|주문수량:" + Quantity + "][" + searchMod + "] ");
            mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1857::[ " + hname + " ]:" + ordptnDetail+ "[" + searchMod + "]");

            return true;

        }//buyTest END

   
        /// <summary>
        /// 종목검색 호출
        /// </summary>
        public void call_request()
        {
            String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            
            String conditionName = startupPath + "\\Resources\\Condition" + conditionCallIndex + ".ACF";
            base.SetFieldData("t1857InBlock", "sRealFlag"  , 0, "0");        // 실시간구분 : 0:조회, 1:실시간
            base.SetFieldData("t1857InBlock", "sSearchFlag", 0, "F");         // 종목검색구분 : F:파일, S:서버
            base.SetFieldData("t1857InBlock", "query_index", 0, conditionName);         //

            int nSuccess = base.RequestService("t1857", "");

            if (nSuccess < 0){
                mainForm.input_t1857_log1.Text = "[" + mainForm.label_time.Text + "][" + this.conditionNm[conditionCallIndex] + "]"+ nSuccess;
            }else{
                mainForm.input_t1857_log1.Text = "[" + mainForm.label_time.Text + "][" + this.conditionNm[conditionCallIndex] + "]조건검색 요청.";
            }
        }
    } //end class 
    
}   // end namespace
