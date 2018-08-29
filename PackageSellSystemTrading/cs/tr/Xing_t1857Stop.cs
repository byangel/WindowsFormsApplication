
using System;
using System.Linq;
using System.Windows.Forms;
using XA_DATASETLib;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

//추가매수 검색 - 추개매수 수익률 참조를 해야하나?
namespace PackageSellSystemTrading{
    public class Xing_t1857Stop : XAQueryClass{
        public  MainForm mainForm;
        private String str_searchFileNm = "";
        private DataTable searchListDt=null;
        public  DataTable getSearchListDt(){
            return this.searchListDt;
        }
        
        //투자 비율
        //public String investmentRatio;
        //public Boolean initAt = false;

        // 생성자
        public Xing_t1857Stop(){

            base.ResFileName = "₩res₩t1857.res";
            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

       
            this.searchListDt = new DataTable();
            searchListDt.Columns.Add("종목코드"     , typeof(string));
            searchListDt.Columns.Add("종목명"       , typeof(string));
            searchListDt.Columns.Add("현재가"       , typeof(double));
            searchListDt.Columns.Add("전일대비구분" , typeof(string));
            searchListDt.Columns.Add("전일대비"     , typeof(string));
            searchListDt.Columns.Add("등락율"       , typeof(string));
            searchListDt.Columns.Add("거래량"       , typeof(double));
            searchListDt.Columns.Add("연속봉수"     , typeof(string));
            searchListDt.Columns.Add("검색조건"     , typeof(string));
            searchListDt.Columns.Add("검색코드"     , typeof(string));
            searchListDt.Columns.Add("상태"         , typeof(string));

        }   // end function



		//데이터 응답 처리 callBack
		void receiveDataEventHandler(string szTrCode){

            //손절검색목록,손절검색목록 초기화.
            searchListDt.Clear();
           
            try {
                int iCount = base.GetBlockCount("t1857OutBlock1");

                DataRow tmpRow;
                DataRow[] foundRows = new DataRow[] { };
                String shcode;
                //String sunikrt;//수익률
                for (int i = 0; i < iCount; i++) {
                    shcode = base.GetFieldData("t1857OutBlock1", "shcode", i);//종목코드
                    
                    tmpRow = searchListDt.NewRow();
                    searchListDt.Rows.Add(tmpRow);
                  
                    
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
                mainForm.input_t1857_log1.Text = "<" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + ">t1857Stop:손절검색" ;
                
                this.execute();
                mainForm.lb_sellCnt.Text = this.searchListDt.Rows.Count.ToString();
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
                mainForm.log("<t1857Stop:03563>정규장 시간이 아닙니다");
            } else if (nMessageCode== "  -21") {
                
            } else {
                mainForm.log("<t1857Stop:" + nMessageCode + ">" + szMessage);
            }
        }

       
        private Boolean execute()
        {
            try
            {
                //손절 유효 시간 인지 비교 - 매수는 상태값을 출력하지 않기 때문에 유효 시간이 아니면 아예 매도 테스트를 호출하지 않는다.
                //TimeSpan nowTimeSpan = TimeSpan.Parse(mainForm.xing_t0167.hour + ":" + mainForm.xing_t0167.minute + ":" + mainForm.xing_t0167.second);
                if (!Util.isStopTime())
                {
                    return false;
                }
                
                //손절 테스트
                foreach (DataRow itemRow in this.searchListDt.AsEnumerable())
                {
                    executeTest(itemRow);
                }
            }
            catch (Exception ex)
            {
                mainForm.log("t1857Stop : " + ex.Message);
                //Log.WriteLine("t1857Stop : " + ex.StackTrace);
            }
            return true;
        }

        //손절 테스트
        private Boolean executeTest(DataRow itemRow)
        {
            String shcode; //종목코드
            String hname; //종목명
            String close; //현재가
            String searchNm; //검색조건
            String searchCode; //검색코드

            shcode      = itemRow["종목코드" ].ToString(); //종목코드
            hname       = itemRow["종목명"   ].ToString(); //종목명
            close       = itemRow["현재가"   ].ToString(); //현재가
            searchNm    = itemRow["검색조건" ].ToString(); //검색조건
            searchCode  = itemRow["검색코드" ].ToString(); //검색코드

            
            //글로벌 매도가능 설정값 체크
            if (!mainForm.cbx_sell_at.Checked){
                return false;
            }

            
            int t0424VoListFindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode);//보유종목인지 체크
            if (t0424VoListFindIndex >= 0)
            {
                T0424Vo t0424Vo = mainForm.xing_t0424.getT0424VoList().ElementAt(t0424VoListFindIndex);

                //이미 주문이 실행된 상태
                if (t0424Vo.orderAt == "Y")
                {
                    return false;
                }

                //주문 전송
                Double 평균단가   = t0424Vo.pamt; //매도주문일경우 매수금액을 알기위해 넣어준다.
                String 종목코드   = t0424Vo.expcode;
                String 종목명     = t0424Vo.hname;
                Double 현재가     = t0424Vo.price;
                String 매도전략명 = t0424Vo.searchNm;
                Double 수량       = t0424Vo.mdposqt;
                Double 수익율     = t0424Vo.sunikrt;
                //String 상세매매구분 = "매도검색 매도";

                //매도주문
                t0424Vo.orderAt = "Y";
                Xing_CSPAT00600 xing_CSPAT00600 = mainForm.CSPAT00600Mng.get600();
                xing_CSPAT00600.call_request(종목명, 종목코드, "매도", 수량, 현재가, 매도전략명, 평균단가, "손절검색");
                
                mainForm.log("<t1857Stop:" + 매도전략명 + "><" + 종목명 + ">" + 수익율 + "% " + 수량 + "주 " + 현재가 + "원");
            }

            return true;

        }//buyTest END
        

        public Boolean call_index()
        {
            
            String searchFileFullPath   = "";
            //검색 조건 명을 확장자 제거

            searchFileFullPath = Properties.Settings.Default.STOP_SEARCH_NM;
            if (searchFileFullPath == "" || searchFileFullPath == "선택")
            {
                //mainForm.log("<t1857Add> 검색 조건식 설정 값이 없습니다.");
                mainForm.input_t1857_log1.Text = " < t1857Stop><" + DateTime.Now.TimeOfDay.ToString().Substring(0, 8) + ">손절 미설정";
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
