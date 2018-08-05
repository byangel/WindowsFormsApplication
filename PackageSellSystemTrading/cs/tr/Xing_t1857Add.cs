
using System;
using System.Linq;
using System.Windows.Forms;
using XA_DATASETLib;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;


namespace PackageSellSystemTrading{
    public class Xing_t1857Add : XAQueryClass{
        public MainForm mainForm;
        
        private String str_searchFileNm = "";
        private DataTable buyListDt=null;
        public DataTable getBuyListDt(){
            return this.buyListDt;
        }
        
        //투자 비율
        //public String investmentRatio;
        //public Boolean initAt = false;

        // 생성자
        public Xing_t1857Add(){

            base.ResFileName = "₩res₩t1857.res";
            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

       
            this.buyListDt = new DataTable();
            buyListDt.Columns.Add("종목코드"     , typeof(string));
            buyListDt.Columns.Add("종목명"       , typeof(string));
            buyListDt.Columns.Add("현재가"       , typeof(double));
            buyListDt.Columns.Add("전일대비구분" , typeof(string));
            buyListDt.Columns.Add("전일대비"     , typeof(string));
            buyListDt.Columns.Add("등락율"       , typeof(string));
            buyListDt.Columns.Add("거래량"       , typeof(double));
            buyListDt.Columns.Add("연속봉수"     , typeof(string));
            buyListDt.Columns.Add("검색조건"     , typeof(string));
            buyListDt.Columns.Add("검색코드"     , typeof(string));
            buyListDt.Columns.Add("상태"         , typeof(string));

        }   // end function



		//데이터 응답 처리 callBack
		void receiveDataEventHandler(string szTrCode){

            //매수검색목록,매도검색목록 초기화.
            buyListDt.Clear();
           
            try {
                int iCount = base.GetBlockCount("t1857OutBlock1");

                DataRow tmpRow;
                DataRow[] foundRows = new DataRow[] { };
                String shcode;
                //String sunikrt;//수익률
                for (int i = 0; i < iCount; i++) {
                    shcode = base.GetFieldData("t1857OutBlock1", "shcode", i);//종목코드
                    
                    tmpRow = buyListDt.NewRow();
                    buyListDt.Rows.Add(tmpRow);
                  
                    
                    tmpRow["종목코드"     ] = base.GetFieldData("t1857OutBlock1", "shcode",  i); //종목코드
                    tmpRow["종목명"       ] = base.GetFieldData("t1857OutBlock1", "hname" ,  i); //종목명
                    tmpRow["현재가"       ] = base.GetFieldData("t1857OutBlock1", "price" ,  i); //현재가
                    tmpRow["전일대비구분" ] = base.GetFieldData("t1857OutBlock1", "sign"  ,  i); //전일대비구분 
                    tmpRow["전일대비"     ] = base.GetFieldData("t1857OutBlock1", "change",  i); //전일대비
                    tmpRow["등락율"       ] = base.GetFieldData("t1857OutBlock1", "diff"  ,  i); //등락율
                    tmpRow["거래량"       ] = base.GetFieldData("t1857OutBlock1", "volume",  i); //거래량
                    tmpRow["연속봉수"     ] = base.GetFieldData("t1857OutBlock1", "jobFlag", i);//연속봉수
                    tmpRow["검색조건"     ] = Util.getShortFileNm(Properties.Settings.Default.ADD_BUY_SEARCH_NM);//추가매수 조건검색명
                    tmpRow["검색코드"     ] = 99;
                   
                }
                mainForm.input_t1857_log1.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + ">t1857Add:추가매수" ;
                
                this.SearchBuy();

            }
            catch (Exception ex){
                Log.WriteLine("t1857Add : " + ex.Message);
                Log.WriteLine("t1857Add : " + ex.StackTrace);
            }
        }


        //메세지 이벤트 핸들러
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                //MessageBox.Show("dd");
                ;
            } else if (nMessageCode == "03563") {
                mainForm.log("<t1857:03563>정규장 시간이 아닙니다");
            } else if (nMessageCode== "  -21") {
                
            } else {
                mainForm.log("<t1857:" + nMessageCode + ">" + szMessage);
            }
        }

       
        private Boolean SearchBuy()
        {
            //1.검색코드 설정값
            int configSearchCode = 1;
            String searchFileFullPath = Properties.Settings.Default.BUY_SEARCH_NM2;
            if (searchFileFullPath != "")
            {
                if (Properties.Settings.Default.BUY_SEARCH_SE1.Equals("AND")) configSearchCode += 2;
            }
            searchFileFullPath = Properties.Settings.Default.BUY_SEARCH_NM3;
            if (searchFileFullPath != "")
            {
                if (Properties.Settings.Default.BUY_SEARCH_SE2.Equals("AND")) configSearchCode += 4;
            }

            //2.현재 시간
            TimeSpan nowTimeSpan = TimeSpan.Parse(mainForm.xing_t0167.hour + ":" + mainForm.xing_t0167.minute + ":" + mainForm.xing_t0167.second);
            //xingAPI의 경우 t8430, t8436을 이용하시어 전 종목 조회 후 해당 종목의 구분값을 확인하셔야 합니다.
            //코스피 코스닥 옵션 매수가능 -업황지수가 오션설정값 이하이면 매수금지한다.
            //추가매수는 업황에따라 매수여부를 체크 하는것을 제한하지 않는다.
            //Boolean kosBuyAt = true;
            //Boolean kodBuyAt = true;
            

            //3.검색목록 매수 테스트
            int rowIndex = 0;
            foreach (DataRow itemRow in this.buyListDt.AsEnumerable())
            {
                BuyTest(itemRow, nowTimeSpan, rowIndex);
                rowIndex++;
            }
            return true;
        }

        //매수 테스트
        private Boolean BuyTest(DataRow itemRow, TimeSpan nowTimeSpan,int rowIndex)
        {
            try
            {
                String shcode     = itemRow["종목코드"].ToString(); //종목코드
                String hname      = itemRow["종목명"  ].ToString(); //종목명
                String close      = itemRow["현재가"  ].ToString(); //현재가
                String searchNm   = itemRow["검색조건"].ToString(); //검색조건
                String searchCode = itemRow["검색코드"].ToString(); //검색코드
                
                //매수금지목록
                DataTable SellListDt = mainForm.xing_t1857Stop.getSellListDt();
                DataRow[] SellListDtRow = null;
                SellListDtRow = SellListDt.Select("종목코드 Like '" + shcode + "'");

                if (SellListDtRow.Count() > 0)
                {
                    //mainForm.grd_t0424.Rows[t0424VoListFindIndex].Cells["c_expcode"].Style.BackColor = Color.Red;
                    mainForm.grd_t1833_dt.Rows[rowIndex].Cells["종목명"].Style.BackColor = Color.Red;
                    itemRow["상태"] = "매수금지";
                    return false;
                }

                

                String ordptnDetail; //매수 상세 구분을 해준다. 신규매수|반복매수
                                     //금일 매수 체결 내용이 있고 미체결 잔량이 0인 건은 매수 하지 않는다.
                var toDayBuyT0425VoList = from item in mainForm.xing_t0425.getT0425VoList()
                                          where item.expcode == shcode && item.medosu == "매수"
                                          select item;
                if (toDayBuyT0425VoList.Count() > 0)
                {
                    itemRow["상태"] = "금일매수종목X";
                    return false;
                }

                var toDaySellT0425VoList = from item in mainForm.xing_t0425.getT0425VoList()
                                           where item.expcode == shcode && item.medosu == "매도"
                                           select item;

                if (toDaySellT0425VoList.Count() > 0)
                {
                    if (!Properties.Settings.Default.SELL_TO_RE_BUY_AT)
                    {
                        itemRow["상태"] = "금일매도종목X";
                        return false;
                    }
                }

                //5.보유종목 반복매수여부 테스트 -두번째 컨디션일 경우 보유종목일경우에만 중복 매수한다.
                int t0424VoListFindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode);//보유종목인지 체크

                if (t0424VoListFindIndex >= 0)
                {
                    
                    //기존 종목 수익률
                    EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
                    Double sunikrt = t0424VoList.ElementAt(t0424VoListFindIndex).sunikrt;

                    //추가매수 하락율 체크 
                    if (sunikrt > double.Parse(Properties.Settings.Default.ADD_BUY_SIGNAL_RATE))
                    {
                        itemRow["상태"] = "하락율 미달x";
                        return false;
                    }

                    ordptnDetail = "추가매수";
                    ////배팅금액 설정
                    //battingAtm = int.Parse(Properties.Settings.Default.ADD_BUY_AMT) * 10000;
                    //1.반복매수면 투자율 제한 하지 않는다.
                    //2.반복매수면 매수금지 종목이라도 매수한다.

                }
                else
                {
                    //자본금대비 투자 비율이 높으면 신규매수 하지 않는다.
                    //Double 투자율 = Double.Parse(mainForm.tradingInfoDt.Rows[0]["투자율"].ToString());
                    //if (투자율 > Double.Parse(Properties.Settings.Default.BASE_MONEY_BUY_RATE))
                    //{
                    //    itemRow["상태"] = "투자율제한 매수X";
                    //    return false;
                    //}
                    itemRow["상태"] = "신규x";
                    return false;
                }


                //추가매수 가능 시간 비교
                if (!Util.isAddBuyTime())
                {
                    itemRow["상태"] = "시간x<" + ordptnDetail + ">";
                    return false;
                }

                //4.매수
                //임시로 넣어둔다 왜 현제가가 0으로 넘어오는지 모르겠다.
                if (close == "0")
                {
                    itemRow["상태"] = "오류:246";
                    return false;
                }

                //this 함수가 3번 호출 된다. 주문이 3번 나갈 수가 있어서 추가 해준다.
                if (itemRow["상태"].ToString().IndexOf("주문전송") >= 0 )
                {
                    return false;
                }

                if (!mainForm.cbx_buy_at.Checked)
                {
                    itemRow["상태"] = "매수정지 상태";
                    return false;
                }
               
                /// <summary>
                /// 현물정상주문
                /// </summary>
                /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
                /// <param name="IsuNo">종목번호</param>
                /// <param name="Quantity">수량</param>
                /// <param name="Price">가격</param>
                
                
                //실시간 가격 모니터링 등록
                mainForm.real_S3.call_real(shcode);
                mainForm.real_K3.call_real(shcode);
                //매수 실행
                Xing_CSPAT00600 xing_CSPAT00600 = mainForm.CSPAT00600Mng.get600();
                xing_CSPAT00600.call_request(hname, shcode, "매수", 0, Double.Parse(close), searchNm, 0, ordptnDetail);
               
                //로그출력
                itemRow["상태"] = "주문전송<" + ordptnDetail + ">";
                mainForm.log("<t1857Add:"+ searchNm +"><" + hname + "> <" + close + "원>"+ ordptnDetail);
            }
            catch (Exception ex)
            {
                Log.WriteLine("t1857Add : " + ex.Message);
                Log.WriteLine("t1857Add : " + ex.StackTrace);
            }
            return true;

        }//buyTest END
        

        public Boolean call_index()
        {
            
            String searchFileFullPath   = "";
            //검색 조건 명을 확장자 제거

            searchFileFullPath = Properties.Settings.Default.ADD_BUY_SEARCH_NM;
            if (searchFileFullPath == "" || searchFileFullPath == "선택")
            {
                //mainForm.log("<t1857Add> 검색 조건식 설정 값이 없습니다.");
                mainForm.input_t1857_log1.Text = "미설정";
                return false;
            }
                   
            //파일명만 추출
            this.str_searchFileNm = Util.getShortFileNm(searchFileFullPath);

            base.SetFieldData("t1857InBlock", "sRealFlag"  , 0, "0");                // 실시간구분   : 0:조회, 1:실시간
            base.SetFieldData("t1857InBlock", "sSearchFlag", 0, "F");                // 종목검색구분 : F:파일, S:서버
            base.SetFieldData("t1857InBlock", "query_index", 0, searchFileFullPath); //
            
            //호출
            int nSuccess = base.RequestService("t1857", "");
           
            return true;
        }

    } //end class 
    
}   // end namespace
