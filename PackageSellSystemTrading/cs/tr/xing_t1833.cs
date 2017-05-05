
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
                   this.buyTest(tmpT1833Vo.shcode, tmpT1833Vo.hname, tmpT1833Vo.close, t1833VoList.Count - 1);
                }
               
            }

            //목록에 없는 종목 그리드에서 삭제.
            for (int i = 0; i < t1833VoList.Count; i++)
            {
                tmpT1833Vo = t1833VoList.ElementAt(i);

                if (tmpT1833Vo.deleteAt == true) {
                    t1833VoList.RemoveAt(i);
                    i--;
                }
                tmpT1833Vo.deleteAt = true;
            }

            mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text+ "]조건검색 응답 완료";
            completeAt = true;//중복호출 여부
        }


        //메세지 이벤트 핸들러
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                //Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "]t1833 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지
            }         

        }
        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(){

            if (completeAt) {
                //폼 메세지.
                completeAt = false;//중복호출 방지
                mainForm.input_t1833_log1.Text = "[" + mainForm.label_time.Text + "]조건검색 요청.";
                //Thread.Sleep(1000);
                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                base.RequestService("t1833", startupPath + "\\Resources\\"+ Properties.Settings.Default.CONDITION_ADF);
            } else {
                mainForm.input_t1833_log1.Text = "[중복]조건검색 요청.";
                //mainForm.input_t1833_log2.Text = "대기";
            }
        }


        //진입검색에서 검색된 종목을 매수한다.
        private Boolean buyTest(String shcode,String hname, String close,int addIndex){
            String time = mainForm.xing_t0167.time;
            //if (time == "" ) { time = "153000"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60 * 60) + (int.Parse(time.Substring(2, 2))*60) + (int.Parse(time.Substring(4, 2)));//현재 시간

            //1.미체결항목에 매수 항목이 있는지 확인하자.
            //2.반복매수 - 보유종목 매수 시기가 1시간이상 이전이고 수익률이 -3% 이하이면 반복매수한다.
            //3.실자본금 또는 보유금액 대비 95% 이상 매입이 이루어 졌을경우 신규 매수하지 않는다.
            //4.최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.

            String ordptnDetail; //매수 상세 구분을 해준다. 신규매수|반복매수
            
            //매수금지목록
            EBindingList<T1833Vo> t1833ExcludeVoList = mainForm.xing_t1833Exclude.t1833ExcludeVoList;


            //1.금일 같은종목 진입 금지- 주문구분 01:매도|02:매수 - t0425로 변경해야할듯.
            //var varDataLogVoList = from item in mainForm.dataLog.getDataLogVoList()
            //                      where item.date == DateTime.Now.ToString("yyyyMMdd") && item.Isuno == shcode && item.ordptncode == "02"
            //                     select item;
            //if (varDataLogVoList.Count() > 0)
            //{
            //    Log.WriteLine("t1833::" + hname + "(" + shcode + ") 금일 1회 매수 제한.");
            //    return false;
            //}


            //////////////////////////////////

            //금일 매수 체결 내용이 있고 미체결 잔량이 0인 건은 매수 하지 않는다.
            var varT0425VoList = from item in mainForm.xing_t0425.getT0425VoList()
                               where item.expcode == shcode && item.medosu == "매수"
                               select item;
            if (varT0425VoList.Count() > 0)
            {
                Log.WriteLine("t1833::" + hname + "(" + shcode + ") 금일 1회 매수 제한.");
                mainForm.listBox_log.Items.Insert(0, "[" + mainForm.label_time.Text + "]t1833::" + hname + ": 금일 1회 매수 제한.");
                return false;
            }

           


            ///////////////////////////////////////



            //2.반목매수 기간 제한.

            //    //반복매수 시간 제한. --금일 중복및 반복매수 제한 조건이 있다면 필요없는 조건임.
            //    //반복매수 제한-분으로 푼다음 시간을 계산한다.
            //    tmpTime = (int.Parse(result_t0425.ElementAt(i).ordtime.Substring(0, 2)) * 60 * 60) + (int.Parse(result_t0425.ElementAt(i).ordtime.Substring(2, 2)) * 60) + (int.Parse(result_t0425.ElementAt(i).ordtime.Substring(4, 2)));
            //    tmpTime = (cTime - tmpTime);
            //    if (tmpTime < (int.Parse(Properties.Settings.Default.REPEAT_TERM) * 60))
            //    {
            //        //Log.WriteLine("t1833 :: [" + hname + "] (" + time + ")" + cTime + "-" + "(" + ordtime + ")" + ordMTime + "=" + (cTime - ordMTime));
            //        Log.WriteLine("t1833::" + hname + "(" + shcode + ") 금일반복매수 텀 제한.  [매수후지난시간" + tmpTime + "| 설정시간:" + (int.Parse(Properties.Settings.Default.REPEAT_TERM) * 60) + "]");
            //        return false;
            //    }
            //}


            //3.진입 검색된 종목이 계좌잔고 그리드에 존재하면 반복매수 아니면 신규매수 -> DataLogVoList 로 변경함


            //int dataLogVoListFindIndex = mainForm.dataLog.getDataLogVoList().Find("Isuno", shcode);//linquery 로변경해야할듯.




            //4.매수금지종목 테스트. --신규매수일때에만 매수금지종목 제한한다, 기존 보유종목이면 보유한거 처리하기위해서 사도 된다.
            int t1833ExcludeVoListFindIndex = t1833ExcludeVoList.Find("shcode", shcode);
            int t0424VoListFindIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode);//보유종목인지 체크
            if (t1833ExcludeVoListFindIndex >= 0 && t0424VoListFindIndex < 0 )
            {
                Log.WriteLine("t1833::" + hname + "(" + shcode + ") 매수금지 종목 ");
                mainForm.listBox_log.Items.Insert(0, "[" + mainForm.label_time.Text + "]t1833::" + hname + ":매수금지 종목.");
                mainForm.grd_t1833.Rows[addIndex].Cells["hname"].Style.BackColor = Color.Red;
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
                    Log.WriteLine("t1833::" + hname + "(" + shcode + ") 반복매수 제한 [수익률:" + sunikrt + "%|설정수익률:" + Properties.Settings.Default.REPEAT_RATE + "%]");
                    mainForm.listBox_log.Items.Insert(0, "[" + mainForm.label_time.Text + "]t1833::" + hname + ":반복매수 제한.");
                    return false;
                }

            }else{//-보유종목이 아니고 신규매수해야 한다면.
                ordptnDetail = "신규매수";
                 //자본금테스트 -- 자본금 = 매입금액 + D2예수금 
                 Double 자본금 = this.mainForm.xing_t0424.mamt + int.Parse(this.mainForm.xing_CSPAQ12200.D2Dps);
                //-투자금액 제한 옵션이 참이면 AMT_LIMIT 값을 강제로 삽입해준다.- 자본금이 최대운영자금까지는 복리로 운영이 된다.
                if (Properties.Settings.Default.LIMITED_AT)
                {
                    //이런날이 올까?
                    if (자본금 > int.Parse(Properties.Settings.Default.MAX_AMT_LIMIT)) {
                        자본금 = int.Parse(Properties.Settings.Default.MAX_AMT_LIMIT);
                    }
                }

                //-최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.
                //-매입금액 기초자산의 90% 이상 매입을 할수 없다.
                //-매입금액 / 자본금 * 100 =자본금 대비 투자율
                Double enterRate = (this.mainForm.xing_t0424.mamt / 자본금) * 100;
                if (enterRate > float.Parse(Properties.Settings.Default.BUY_STOP_RATE)){ //자본금대비 투자 비율이 높으면 신규매수 하지 않는다.
                    Log.WriteLine("t1833::" + hname + "(" + shcode + ") 자본금 대비 투자율 제한   [자본금대비투자율:"+ enterRate+"%|설정비율:" + Properties.Settings.Default.BUY_STOP_RATE + "%]");
                    mainForm.listBox_log.Items.Insert(0, "[" + mainForm.label_time.Text + "]t1833::" + hname + ":자본금 대비 투자율 제한.");
                    return false;
                }      
            }


            //4.매수
            int battingAtm = int.Parse(mainForm.label_battingAtm.Text);
            //임시로 넣어둔다 왜 현제가가 0으로 넘어오는지 모르겠다.
            if (close == "0"){
                Log.WriteLine("t1833::" + hname + "(" + shcode + ") [현제가:" + close+";" );
                return false;
            }

            //-매수수량 계산.
            int Quantity = battingAtm / int.Parse(close);
            //int Quantity = 20000;
            //-정규장에만 주문실행.
            if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1530){
                /// <summary>
                /// 현물정상주문
                /// </summary>
                /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
                /// <param name="IsuNo">종목번호</param>
                /// <param name="Quantity">수량</param>
                /// <param name="Price">가격</param>
                mainForm.xing_CSPAT00600.call_requestBuy( ordptnDetail, shcode, hname, Quantity.ToString(), close);

                Log.WriteLine("t1833::" + hname + "(" + shcode + ") "+ ordptnDetail + "   [주문가격:" + close + "|주문수량:" + Quantity + "] ");
                mainForm.listBox_log.Items.Insert(0, "[" + mainForm.label_time.Text + "]t1833::" + hname + ":"+ ordptnDetail);
            }
            else{
                Log.WriteLine("t1833::" + hname + "(" + shcode + ") 비정규장 제어 [주문가격:" + close + "|주문수량:" + Quantity + "]");
                mainForm.listBox_log.Items.Insert(0, "[" + mainForm.label_time.Text + "]t1833::" + hname + ": 비정규장 제어.");
            }
          
            return true;
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
