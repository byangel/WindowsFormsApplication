
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Data;
using System.Drawing;
using System.Threading;
namespace PackageSellSystemTrading{
    public class Xing_t1857Stop : XAQueryClass{

        public MainForm mainForm;
        private int int_callIndex;
        private String str_searchFileNm = "";

        private DataTable sellListDt;
        public DataTable getSellListDt()
        {
            return this.sellListDt;
        }
        

     // 생성자
        public Xing_t1857Stop(){
            base.ResFileName = "₩res₩t1857.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            this.sellListDt = new DataTable();
            sellListDt.Columns.Add("종목코드", typeof(string));
            sellListDt.Columns.Add("종목명", typeof(string));
            sellListDt.Columns.Add("현재가", typeof(double));
            sellListDt.Columns.Add("전일대비구분", typeof(string));
            sellListDt.Columns.Add("전일대비", typeof(string));
            sellListDt.Columns.Add("등락율", typeof(string));
            sellListDt.Columns.Add("거래량", typeof(double));
            sellListDt.Columns.Add("연속봉수", typeof(string));
            sellListDt.Columns.Add("검색조건", typeof(string));
            sellListDt.Columns.Add("검색코드", typeof(string));

        }// end function

        


     /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            //매수검색목록,매도검색목록 초기화.
            if (this.int_callIndex == 0) sellListDt.Clear();
            String searchSellSe1 = Properties.Settings.Default.SELL_SEARCH_SE1 == "AND" ? " & " : " || ";
            String searchSellSe2 = Properties.Settings.Default.SELL_SEARCH_SE2 == "AND" ? " & " : " || ";

            try {
                int iCount = base.GetBlockCount("t1857OutBlock1");

                DataRow tmpRow;
                DataRow[] foundRows = new DataRow[] { };
                String shcode;
                //String sunikrt;//수익률
                for (int i = 0; i < iCount; i++) {

                    shcode = base.GetFieldData("t1857OutBlock1", "shcode", i);//종목코드

                    if (this.int_callIndex != 0) foundRows = sellListDt.Select("종목코드 = '" + shcode + "'");
                
                    //종목이 없으면 추가 있으면 수정
                    if (foundRows.Count() > 0)
                    {
                        tmpRow = foundRows[0];
                    }
                    else
                    {//new
                        tmpRow = sellListDt.NewRow();
                        tmpRow["검색조건"] = "";
                        tmpRow["검색코드"] = 0;
                        sellListDt.Rows.Add(tmpRow);
                    }

                    tmpRow["종목코드"     ] = base.GetFieldData("t1857OutBlock1", "shcode" , i); //종목코드
                    tmpRow["종목명"       ] = base.GetFieldData("t1857OutBlock1", "hname"  , i); //종목명
                    tmpRow["현재가"       ] = base.GetFieldData("t1857OutBlock1", "price"  , i); //현재가
                    tmpRow["전일대비구분" ] = base.GetFieldData("t1857OutBlock1", "sign"   , i); //전일대비구분 
                    tmpRow["전일대비"     ] = base.GetFieldData("t1857OutBlock1", "change" , i); //전일대비
                    tmpRow["등락율"       ] = base.GetFieldData("t1857OutBlock1", "diff"   , i); //등락율
                    tmpRow["거래량"       ] = base.GetFieldData("t1857OutBlock1", "volume" , i); //거래량
                    tmpRow["연속봉수"     ] = base.GetFieldData("t1857OutBlock1", "jobFlag", i); //연속봉수
                    
                    switch (this.int_callIndex)
                    {
                        case 0:
                            tmpRow["검색조건"] = Util.getShortFileNm(Properties.Settings.Default.SELL_SEARCH_NM1);        //검색조건
                            tmpRow["검색코드"] = int.Parse(tmpRow["검색코드"].ToString()) + 1;                           //검색코드
                            break;
                        case 1:
                            tmpRow["검색조건"] += (tmpRow["검색조건"].ToString().Length > 0 ? searchSellSe1 : "") + Util.getShortFileNm(Properties.Settings.Default.SELL_SEARCH_NM2); //검색조건
                            tmpRow["검색코드"] = int.Parse(tmpRow["검색코드"].ToString()) + 2;                           //검색코드

                            break;
                        case 2:
                            tmpRow["검색조건"] += (tmpRow["검색조건"].ToString().Length > 0 ? searchSellSe2 : "") + Util.getShortFileNm(Properties.Settings.Default.SELL_SEARCH_NM3); //검색조건
                            tmpRow["검색코드"] = int.Parse(tmpRow["검색코드"].ToString()) + 4;                           //검색코드
                            break;
                    };
                }

                switch (this.int_callIndex)
                {
                    case 0:
                        //재귀호출
                        this.call_index(1);
                        break;
                    case 1:
                        //재귀호출
                        this.call_index(2);
                        break;
                    case 2:
                        //매도주문 호출
                        break;
                };
                this.SearchSell();
                mainForm.lb_sellCnt.Text = sellListDt.Rows.Count.ToString();
            } catch (Exception ex){
                Log.WriteLine("t1857 : " + ex.Message);
                Log.WriteLine("t1857 : " + ex.StackTrace);
            }
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                mainForm.insertListBoxLog("<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "><t1857Stop:" + nMessageCode + ">" + szMessage);
            }
            
        }

        private Boolean SearchSell()
        {
            try {
                //검색코드 설정값
                int configSearchCode = 1;
                String searchFileFullPath = Properties.Settings.Default.SELL_SEARCH_NM2;
                if (searchFileFullPath != "")
                {
                    if (Properties.Settings.Default.SELL_SEARCH_SE1.Equals("AND")) configSearchCode += 2;
                }
                searchFileFullPath = Properties.Settings.Default.SELL_SEARCH_NM3;
                if (searchFileFullPath != "")
                {
                    if (Properties.Settings.Default.SELL_SEARCH_SE2.Equals("AND")) configSearchCode += 4;
                }
                //매도 검색 전용 -매수금지 목록으로만 사용된다.
                if (Properties.Settings.Default.SELL_SEARCH_ONLY_AT)
                {
                    return false;
                }

                //매도 유효 시간 인지 비교 - 매수는 사태값을 출력하지 않기 때문에 유효 시간이 아니면 아예 매도 테스트를 호출하지 않는다.
                TimeSpan nowTimeSpan = TimeSpan.Parse(mainForm.xing_t0167.hour + ":" + mainForm.xing_t0167.minute + ":" + mainForm.xing_t0167.second);
                if (!Util.isSellTime())
                {
                    return false;
                }
            
                
                //검색목록 매수 테스트
                foreach (DataRow itemRow in this.sellListDt.AsEnumerable()){
                    SellTest(itemRow, configSearchCode);
                }

            } catch (Exception ex){
                mainForm.insertListBoxLog("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.StackTrace);
            }
            return true;
        }

        //매도 테스트
        private Boolean SellTest(DataRow itemRow, int configSearchCode)
        {
            String shcode; //종목코드
            String hname; //종목명
            String close; //현재가
            String searchNm; //검색조건
            String searchCode; //검색코드

            shcode      = itemRow["종목코드"].ToString(); //종목코드
            hname       = itemRow["종목명"  ].ToString(); //종목명
            close       = itemRow["현재가"  ].ToString(); //현재가
            searchNm    = itemRow["검색조건"].ToString(); //검색조건
            searchCode  = itemRow["검색코드"].ToString(); //검색코드

            //검색코드 설정값 비교
            if (int.Parse(searchCode) < configSearchCode)
            {
                return false;
            }

            if (!mainForm.cbx_sell_at.Checked)
            {
                //itemRow["상태"] = "매도정지 상태";
                return false;
            }

            //5.보유종목 반복매수여부 테스트 -두번째 컨디션일 경우 보유종목일경우에만 중복 매수한다.
            int t0424VoListFindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode);//보유종목인지 체크
            if (t0424VoListFindIndex >= 0)
            {
                T0424Vo t0424Vo = mainForm.xing_t0424.getT0424VoList().ElementAt(t0424VoListFindIndex);

                //이미 주문이 실행된 상태
                if (t0424Vo.orderAt == "Y") {
                    return false;
                }

                //주문 전송
                Double 평균단가     = t0424Vo.pamt; //매도주문일경우 매수금액을 알기위해 넣어준다.
                String 종목코드     = t0424Vo.expcode;
                String 종목명       = t0424Vo.hname;
                Double 현재가       = t0424Vo.price;
                String 매수전량명   = t0424Vo.searchNm;
                Double 수량         = t0424Vo.mdposqt;
                //String 상세매매구분 = "매도검색 매도";
    
                Xing_CSPAT00600 xing_CSPAT00600 = mainForm.CSPAT00600Mng.get600();
                xing_CSPAT00600.call_request(종목명, 종목코드, "매도", 수량, 현재가, 매수전량명, 평균단가, "매도검색 매도");

                Double 수익율 = t0424Vo.sunikrt;
                mainForm.log("<t1857Stop:"+ 매수전량명 +"><" + 종목명 + ">" + 수익율 + "% " + 수량 + "주 " + 현재가 + "원");
            }

            return true;

        }//buyTest END

        public Boolean call_index(int callIndex)
        {
            this.int_callIndex = callIndex;
            String searchFileFullPath = "";
            //검색 조건 명을 확장자 제거

            switch (callIndex)
            {
                case 0:
                    searchFileFullPath = Properties.Settings.Default.SELL_SEARCH_NM1;
                    //매도 검색식이 설정되어있지 않으면 return false
                    if (searchFileFullPath == "") return false;
                    break;
                case 1:
                    searchFileFullPath = Properties.Settings.Default.SELL_SEARCH_NM2;
                    if (searchFileFullPath == "") return false;
                    //Thread.Sleep(600);
                    break;
                case 2:
                    searchFileFullPath = Properties.Settings.Default.SELL_SEARCH_NM3;
                    if (searchFileFullPath == "") return false;
                    //Thread.Sleep(600);
                    break;
            };
            //파일명만 추출
            this.str_searchFileNm = Util.getShortFileNm(searchFileFullPath);

            base.SetFieldData("t1857InBlock", "sRealFlag", 0, "0");                  // 실시간구분   : 0:조회, 1:실시간
            base.SetFieldData("t1857InBlock", "sSearchFlag", 0, "F");                // 종목검색구분 : F:파일, S:서버
            base.SetFieldData("t1857InBlock", "query_index", 0, searchFileFullPath); //

            //재귀호출하면서 이전 데이타를 참조하여 블럭카운터를 0으로 초기화 해준다.
            base.SetBlockCount("t1857OutBlock1", 0);

            //호출
            int nSuccess = -1;

            while (nSuccess < 0)
            {
                
                nSuccess = base.RequestService("t1857", "");
                Thread.Sleep(250);
                //mainForm.insertListBoxLog(nSuccess.ToString());
            }
            //호출 성공 여부
            //if (nSuccess < 0){
            //    mainForm.insertListBoxLog("<" + DateTime.Now.TimeOfDay.ToString().Substring(0,8) + "><t1857:" + nSuccess.ToString() + "> 매도 검색식 파일을 찾을 수 없습니다.");
            //    return false;
            //}
            return true;
        }

      
    } //end class 

   
}// end namespace
