
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
    public class Xing_t1833 : XAQueryClass{
        
        private EBindingList<T1833Vo> t1833VoList;
        public EBindingList<T1833Vo> getT1833VoList()
        {
            return this.t1833VoList;
        }
        
        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;
        //투자 비율
        public String investmentRatio;

        public Boolean initAt = false;

        // 생성자
        public Xing_t1833(){
            //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            //base.ResFileName = startupPath+"₩Resources₩t1833.res";
            base.ResFileName = "₩res₩t1833.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
            
            this.t1833VoList = new EBindingList<T1833Vo>();
        }   // end function

        // 소멸자
        ~Xing_t1833(){
          
        }
        
        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            try
            {
                //투자율 설정
                this.investmentRatio = this.getInputRate();
                mainForm.label_InvestmentRatio.Text = this.investmentRatio;

                int iCount = base.GetBlockCount("t1833OutBlock1");

                //매수종목 검색 그리드 초기화
                //mainForm.grd_t1833.Rows.Clear();

                EBindingList<T1833Vo> t1833VoList = (EBindingList<T1833Vo>)mainForm.grd_t1833.DataSource;

                String shcode;//종목코드
                T1833Vo tmpT1833Vo;
                //String sunikrt;//수익률
                for (int i = 0; i < iCount; i++) {

                    shcode = base.GetFieldData("t1833OutBlock1", "shcode", i);//종목코드
                    var result = from   item in t1833VoList
                                 where  item.shcode == shcode
                                 select item;
                    if (result.Count() > 0) {
                        tmpT1833Vo = result.ElementAt(0);       
                    
                    }else{
                        tmpT1833Vo = new T1833Vo();
                    }
                    
                    tmpT1833Vo.shcode = base.GetFieldData("t1833OutBlock1", "shcode", i); //종목코드
                    tmpT1833Vo.hname  = base.GetFieldData("t1833OutBlock1", "hname" , i); //종목명
                    tmpT1833Vo.close  = base.GetFieldData("t1833OutBlock1", "close" , i); //현재가
                    tmpT1833Vo.sign   = base.GetFieldData("t1833OutBlock1", "sign"  , i); //전일대비구분 
                    tmpT1833Vo.change = base.GetFieldData("t1833OutBlock1", "change", i); //전일대비
                    tmpT1833Vo.diff   = base.GetFieldData("t1833OutBlock1", "diff"  , i); //등락율
                    tmpT1833Vo.volume = base.GetFieldData("t1833OutBlock1", "volume", i); //거래량

                    tmpT1833Vo.deleteAt = false; //삭제여부 -나중에 true 인거는 다 삭제해준다.

                    if (result.Count() == 0)
                    {
                       t1833VoList.Add(tmpT1833Vo);
                        //매수 -- 그리드에 새로 추가 될때만 매수 호출하여 중복 호출을 막는다.
                        if (mainForm.tradingAt == "Y")
                        {
                            this.buyTest(tmpT1833Vo.shcode, tmpT1833Vo.hname, tmpT1833Vo.close, t1833VoList.Count - 1);
                        }                 
                    }          
                }

                //목록에 없는 종목 그리드에서 삭제.
                for (int i = 0; i < t1833VoList.Count; i++) {
                    tmpT1833Vo = t1833VoList.ElementAt(i);

                    if (tmpT1833Vo.deleteAt == true) {
                        t1833VoList.RemoveAt(i);
                        i--;
                    }
                    tmpT1833Vo.deleteAt = true;
                }
                mainForm.input_t1833_log1.Text = "[" + mainForm.label_time.Text+ "]조건검색 응답 완료";

                completeAt = true;//중복호출 여부
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
            } else { 
                //Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t1833_log1.Text = "[" + mainForm.label_time.Text + "]t1833 :: " + nMessageCode + " :: " + szMessage;
            }
            //중복호출 방지
            this.completeAt = true;
        }

        private int callCnt = 0;
        /// <summary>
        /// 종목검색 호출
        /// </summary>
        public void call_request(){

            if (completeAt) {
                //폼 메세지.
                completeAt = false;//중복호출 방지
                
                //Thread.Sleep(1000);
                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                base.RequestService("t1833", startupPath + "\\Resources\\"+mainForm.optionForm.CONDITION_NM);
                mainForm.input_t1833_log1.Text = "[" + mainForm.label_time.Text + "]조건검색 요청.";
            } else {
                mainForm.input_t1833_log1.Text = "[" + mainForm.label_time.Text + "[중복]조건검색 요청.";

                callCnt++;
                if (callCnt == 5)
                {
                    this.completeAt = true;
                    callCnt = 0;
                }
            }
        }

        //진입검색에서 검색된 종목을 매수한다.
        private Boolean buyTest(String shcode,String hname, String close,int addIndex){
 
            String time = mainForm.xing_t0167.time;
            //if (time == "" ) { time = "153000"; }//에러 안나게 기본값을 셋팅해준다.
            //현재 시간
            int 현재시간 = (int.Parse(time.Substring(0, 2)) * 60 * 60) + (int.Parse(time.Substring(2, 2))*60) + (int.Parse(time.Substring(4, 2)));

            //1.미체결항목에 매수 항목이 있는지 확인하자.
            //2.반복매수 - 보유종목 매수 시기가 1시간이상 이전이고 수익률이 -3% 이하이면 반복매수한다.
            //3.실자본금 또는 보유금액 대비 95% 이상 매입이 이루어 졌을경우 신규 매수하지 않는다.
            //4.최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.

            String ordptnDetail; //매수 상세 구분을 해준다. 신규매수|반복매수
            
            //매수금지목록
            EBindingList<T1833Vo> t1833ExcludeVoList = mainForm.xing_t1833Exclude.getT1833ExcludeVoList();

            
            //시간 초과 손절 을 사용하면 금일 매수 제한 하지 않는다.
            //금일 매수 체결 내용이 있고 미체결 잔량이 0인 건은 매수 하지 않는다.
            var toDayBuyT0425VoList = from item in mainForm.xing_t0425.getT0425VoList()
                                where item.expcode == shcode && item.medosu == "매수"
                               select item;
            if (toDayBuyT0425VoList.Count() > 0 )
            {
                Log.WriteLine("t1833::금일 1회 매수 제한:" + hname + "(" + shcode + ") ");
                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1833::" + hname + ": 금일 1회 매수 제한.");
                return false;
            }

            ///////////////////////////////////////
            //2.반목매수 기간 제한. -이기능 구현할때 참고 소스
            // 여전히 실행중인 상태라면 중복 호출이 되지 않도록 빠져나감

            // 정상적으로 응답 처리가 끝난 상태라면 다시 호출을 시도

            /** -21 시간당 전송 제한 우회를 위한 처리 */
            // 처음 실행되는 거라면 패쓰~~
            //if (mCallRequestTime.Ticks == 0) {
            //        mCallRequestTime = DateTime.Now;
            //    } else {// 이미 실행된적이 있다면..
            //        DateTime iTimeNow = DateTime.Now;
            //        // 이전 실행시간과 현재 시간의 간격이 지정값 보다 작으면..함수 빠져 나감
            //        if (((iTimeNow - mCallRequestTime).Ticks / 10000) < mCallRequestInterval){
                        
            //        }else{  mCallRequestTime = iTimeNow;  }
            //    }

            //4.매수금지종목 테스트. --보유종목이아니고 매수금지종목에 포함되면 매수하지 않는다.
            int t1833ExcludeVoListFindIndex = t1833ExcludeVoList.Find("shcode", shcode);
            int t0424VoListFindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode);//보유종목인지 체크
            //if (t1833ExcludeVoListFindIndex >= 0 && t0424VoListFindIndex < 0 )
            if (t1833ExcludeVoListFindIndex >= 0 )//매수금지종목이면 무조건 패스
            {
                Log.WriteLine("t1833::매수금지 종목:" + hname + "(" + shcode + ")");
                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1833::" + hname + ":매수금지 종목.");
                mainForm.grd_t1833.Rows[addIndex].Cells["hname"].Style.BackColor = Color.Red;
                //만약에 보유종목일경우 보유종목도 색으로 표현해주자.
                if (t0424VoListFindIndex >= 0)
                {
                    mainForm.grd_t0424.Rows[t0424VoListFindIndex].Cells["c_expcode"].Style.BackColor = Color.Red;
                }
                return false;
            }
           
            //5.보유종목 반복매수여부 테스트
            if (t0424VoListFindIndex >= 0){
                ordptnDetail = "반복매수";
                //보유종목이면..하이라키...
                //mainForm.grd_t1833.Rows[addIndex].Cells["shcode"].Style.BackColor = Color.Gray;
                EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
             
                String sunikrt = (String)t0424VoList.ElementAt(t0424VoListFindIndex).sunikrt2;//기존 종목 수익률

                //-수익율이 -5% 이하이면 반복매수 해주자.
                if (double.Parse(sunikrt) > double.Parse(Properties.Settings.Default.REPEAT_RATE)){
                    Log.WriteLine("t1833::반복매수 제한:" + hname + "(" + shcode + ")[수익률:" + sunikrt + "%|설정수익률:" + Properties.Settings.Default.REPEAT_RATE + "%]");
                    mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1833::" + hname + ":반복매수 제한.");
                    return false;
                }

            }else{//-보유종목이 아니고 신규매수해야 한다면.
                ordptnDetail = "신규매수";
               
                if (Double.Parse(this.investmentRatio) > Double.Parse(Properties.Settings.Default.BUY_STOP_RATE)){ //자본금대비 투자 비율이 높으면 신규매수 하지 않는다.
                    Log.WriteLine("t1833::투자율 제한:" + hname + "(" + shcode + ")[투자율:"+ investmentRatio + "%|설정비율:" + Properties.Settings.Default.BUY_STOP_RATE + "%]");
                    mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1833::" + hname + ":투자율 제한.");
                    return false;
                }      
            }

            //4.매수
            int battingAtm = int.Parse(mainForm.label_battingAtm.Text.Replace(",",""));
            //임시로 넣어둔다 왜 현제가가 0으로 넘어오는지 모르겠다.
            if (close == "0"){
                Log.WriteLine("t1833::" + hname + "(" + shcode + ") [현제가:" + close+";" );
                return false;
            }

            //-매수수량 계산.
            int Quantity = battingAtm / int.Parse(close);
            //int Quantity = 20000;
            //-정규장에만 주문실행.
            if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 901 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1520){
                /// <summary>
                /// 현물정상주문
                /// </summary>
                /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
                /// <param name="IsuNo">종목번호</param>
                /// <param name="Quantity">수량</param>
                /// <param name="Price">가격</param>
                Xing_CSPAT00600 xing_CSPAT00600 = new Xing_CSPAT00600(mainForm);
                mainForm.xing_CSPAT00600List.Add(xing_CSPAT00600);
                
                xing_CSPAT00600.ordptnDetail = ordptnDetail;        //상세 매매 구분.
                xing_CSPAT00600.shcode       = shcode;              //종목코드
                xing_CSPAT00600.hname        = hname;               //종목명
                xing_CSPAT00600.quantity     = Quantity.ToString(); //수량
                xing_CSPAT00600.price        = close;               //가격
                xing_CSPAT00600.divideBuySell= "2";                 // 매매구분: 1-매도, 2-매수
                xing_CSPAT00600.upOrdno      = "";                  //상위매수주문 - 금일매도매수일때만 값이 있다.
                xing_CSPAT00600.upExecprc    = "";                  //상위체결금액  

                xing_CSPAT00600.call_request();

                Log.WriteLine("t1833::검색주문" + hname + "(" + shcode + ") " + ordptnDetail + "   [주문가격:" + close + "|주문수량:" + Quantity + "] ");
                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1833::검색주문" + hname + ":" + ordptnDetail);
                
                   
            }
            else{
                Log.WriteLine("t1833::비정규장 제어:" + hname + "(" + shcode + ") [주문가격:" + close + "|주문수량:" + Quantity + "]");
                mainForm.insertListBoxLog("[" + mainForm.label_time.Text.Substring(0,5) + "]t1833::" + hname + ": 비정규장 제어.");
            }
          
            return true;
        }//buyTest END

        //총자산대비 투자 비율을 리턴한다.
        public String getInputRate()
        {
            String returnValue = "0"; ;
            try
            {
                //자본금 투자 비율
                //자본금테스트 -- 자본금 = 매입금액 + D2예수금 
                String D2예수금 = mainForm.xing_CSPAQ12200.D2Dps;
                Double 매입금액 = mainForm.xing_t0424.mamt;
                Double 자본금 = 매입금액 + double.Parse(D2예수금);
                //-투자금액 제한 옵션이 참이면 AMT_LIMIT 값을 강제로 삽입해준다.- 자본금이 최대운영자금까지는 복리로 운영이 된다.
                if (Properties.Settings.Default.LIMITED_AT)
                {
                    //이런날이 올까?
                    if (자본금 > int.Parse(Properties.Settings.Default.MAX_AMT_LIMIT))
                    {
                        자본금 = int.Parse(Properties.Settings.Default.MAX_AMT_LIMIT);
                    }
                }

                //-최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.
                //-매입금액 기초자산의 90% 이상 매입을 할수 없다.
                //-매입금액 / 자본금 * 100 =자본금 대비 투자율
                //Double enterRate = (this.xing_t0424.mamt / 자본금) * 100;
                returnValue = Math.Round(((mainForm.xing_t0424.mamt / 자본금) * 100), 2).ToString();
            }
            catch (Exception ex)
            {
                Log.WriteLine("t1833 : " + ex.Message);
                Log.WriteLine("t1833 : " + ex.StackTrace);
            }
            return returnValue;
        }



    } //end class 

    public class T1833Vo
    {
        public String  shcode   { set; get; } //종목코드
        public String  hname    { set; get; } //종목명
        public String  close    { set; get; } //현재가
        public String  sign     { set; get; } //전일대비구분 
        public String  change   { set; get; } //전일대비
        public String  diff     { set; get; } //등락율
        public String  volume   { set; get; } //거래량
        public Boolean deleteAt { set; get; } //삭제여부
    }
}   // end namespace
