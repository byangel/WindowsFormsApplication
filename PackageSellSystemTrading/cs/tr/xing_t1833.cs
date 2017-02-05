
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

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;
       
        // 생성자
        public Xing_t1833(){
            //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            //base.ResFileName = startupPath+"₩Resources₩t1833.res";
            base.ResFileName = "₩res₩t1833.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
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
     
            BindingList<T1833Vo> t1833VoList = (BindingList<T1833Vo>)mainForm.grd_t1833.DataSource;

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

                tmpT1833Vo.deleteAt = false; //삭제여부

                if (result.Count() == 0)
                {
                   t1833VoList.Add(tmpT1833Vo);
                   //매수 -- 그리드에 새로 추가 될때만 매수 호출하여 중복 호출을 막는다.
                   this.buy(tmpT1833Vo.shcode, tmpT1833Vo.hname, tmpT1833Vo.close, t1833VoList.Count - 1);
                }
               
            }

            //목록에 없는 종목 그리드에서 삭제.
            for (int i = 0; i < t1833VoList.Count; i++)
            {
                tmpT1833Vo = t1833VoList.ElementAt(i);

                if (tmpT1833Vo.deleteAt == true)
                {
                    //Log.WriteLine("[" + mainForm.input_time.Text + "]t0118 :: 팔린종목 그리드에서 제거");
                    mainForm.input_tmpLog.Text = "[" + mainForm.input_time.Text + "]t1833 진입검색 목록에서 제거";
                    t1833VoList.RemoveAt(i);
                    i--;
                }
                tmpT1833Vo.deleteAt = true;
            }

            mainForm.input_t1833_log2.Text = "[" + mainForm.input_time.Text+ "]조건검색 응답 완료";
            completeAt = true;//중복호출 여부
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                //Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t1833_log2.Text = "[" + mainForm.input_time.Text + "]t1833 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지
            }         

        }
        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(string conditionFileName){

            if (completeAt) {
                //폼 메세지.
                completeAt = false;//중복호출 방지
                mainForm.input_t1833_log1.Text = "[" + mainForm.input_time.Text + "]조건검색 요청.";
                //Thread.Sleep(1000);
                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                base.RequestService("t1833", startupPath + "\\Resources\\"+ conditionFileName);//_6만급등족목_70_
            } else {
                mainForm.input_t1833_log1.Text = "[중복]조건검색 요청.";
                //mainForm.input_t1833_log2.Text = "대기";
            }
        }


        //진입검색에서 검색된 종목을 매수한다.
        private Boolean buy(String shcode,String hname, String close,int addIndex){
            String time = mainForm.xing_t0167.time;
            //if (time == "" ) { time = "153000"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60 * 60) + (int.Parse(time.Substring(2, 2))*60) + (int.Parse(time.Substring(4, 2)));//현재 시간

            //1.미체결항목에 매수 항목이 있는지 확인하자.
            //2.반복매수 - 보유종목 매수 시기가 1시간이상 이전이고 수익률이 -3% 이하이면 반복매수한다.
            //3.실자본금 또는 보유금액 대비 95% 이상 매입이 이루어 졌을경우 신규 매수하지 않는다.
            //4.최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.

            //String toDayBuyAt="";//금일 매수여부
            String accountAt ="";//잔고 존재여부
            //String t0425_ordrem;       //미체결잔량
            //String t0425_medosu;       //매매구분 - 0:전체|1:매수|2:매도
            //String t0425_expcode;      //종목번호
            //String t0425_ordtime;      //주문시간
            int tmpTime;

            //미체결목록그리드
            BindingList<T0425Vo> t0425VoList_Chegb2 = ((BindingList<T0425Vo>)mainForm.grd_t0425_chegb2.DataSource);//미체결
            //체결목록 그리드
            BindingList<T0425Vo> t0425VoList_Chegb1 = ((BindingList<T0425Vo>)mainForm.grd_t0425_chegb1.DataSource);//체결
            //계좌잔고목록
            BindingList<T0424Vo> t0424VoList = ((BindingList<T0424Vo>)mainForm.grd_t0424.DataSource);
            
            //금일 매수 체결 내용이 있고 미체결 잔량이 0인 건은 매수 하지 않는다.
            var resulT0425 =  from t0425VoChegb1 in t0425VoList_Chegb1
                             where t0425VoChegb1.expcode == shcode && t0425VoChegb1.medosu == "매수"
                            select t0425VoChegb1;

            for (int i=0; i < resulT0425.Count(); i++)
            {
                //금일 같은종목 진입 이력이 있다면 매수진입하지 않는다.
                if (resulT0425.ElementAt(i).ordrem == "0")
                {
                    Log.WriteLine("t1833 :: [" + hname + "] 금일 1회 매수 제한.");
                    return false;
                }
                //미체결 내역 있다면 매수제한.
                if (int.Parse(resulT0425.ElementAt(i).ordrem) > 0)
                {
                    Log.WriteLine("t1833 :: [" + hname + "] 미체결 잔량 " + resulT0425.ElementAt(0).ordrem + "주 매수제한");
                    return false;
                }
                //반복매수 시간 제한. --금일 중복및 반복매수 제한 조건이 있다면 필요없는 조건임.
                //반복매수 제한-분으로 푼다음 시간을 계산한다.
                tmpTime = (int.Parse(resulT0425.ElementAt(i).ordtime.Substring(0, 2)) * 60 * 60) + (int.Parse(resulT0425.ElementAt(i).ordtime.Substring(2, 2)) * 60) + (int.Parse(resulT0425.ElementAt(i).ordtime.Substring(4, 2)));
                tmpTime = (cTime - tmpTime);
                if (tmpTime < (Properties.Settings.Default.REPEAT_BUY_TERM * 60))
                {
                    //Log.WriteLine("t1833 :: [" + hname + "] (" + time + ")" + cTime + "-" + "(" + ordtime + ")" + ordMTime + "=" + (cTime - ordMTime));
                    Log.WriteLine("t1833 :: [" + hname + "] 금일 반복매수 " + tmpTime + "초 <" + (Properties.Settings.Default.REPEAT_BUY_TERM * 60) + "초 제한");
                    return false;
                }
           
                
            }


            //for (int i = 0; i < mainForm.xing_t0425.GetBlockCount("t0425OutBlock1"); i++)
            //{

            //    toDayBuyAt="";//금일 매수여부

            //    t0425_ordrem = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "ordrem", i);//미체결 잔량 - 매도또는 매수 주문후  잔량이 있다면 걔좌에 종목이 있다는뜻이므로 미체결 목록에 뿌려준다.
            //    t0425_medosu = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "medosu", i); //매매구분 - 0:전체|1:매수|2:매도
            //    t0425_expcode = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "expcode", i); //종목번호
            //    t0425_ordtime = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "ordtime", i); //주문시간
            //    //Log.WriteLine("t1833 :: " + "/" + shcode + "/" + i+"/"+ medosu)
            //    //미체결 이력에 있다면 return false;

            //    //미체결 내역 있다면 매수제한.
            //    //if (t0425_medosu == "매수" && t0425_expcode == shcode && int.Parse(t0425_ordrem) > 0)
            //    //{
            //    //    Log.WriteLine("t1833 :: [" + hname + "] 미체결 잔량 [" + t0425_ordrem + "주] 매수제한");
            //    //    return false;
            //    //}

            //    //반복매수 시간 제한.
            //    //if (t0425_medosu == "매수" && t0425_expcode == shcode)
            //    //{

            //    //    //반복매수 제한-분으로 푼다음 시간을 계산한다.
            //    //    tmpTime = (int.Parse(t0425_ordtime.Substring(0, 2)) * 60 * 60) + (int.Parse(t0425_ordtime.Substring(2, 2))*60) + (int.Parse(t0425_ordtime.Substring(4, 2)));
            //    //    tmpTime = (cTime - tmpTime);
            //    //    if (tmpTime < (Properties.Settings.Default.REPEAT_BUY_TERM * 60))
            //    //    {
            //    //        //Log.WriteLine("t1833 :: [" + hname + "] (" + time + ")" + cTime + "-" + "(" + ordtime + ")" + ordMTime + "=" + (cTime - ordMTime));
            //    //        Log.WriteLine("t1833 :: [" + hname + "] 금일 반복매수 " +tmpTime+"초 <" + (Properties.Settings.Default.REPEAT_BUY_TERM * 60) + "초 제한");
            //    //        return false;
            //    //    }
            //    //    toDayBuyAt = "금일매수함";
            //    //}
            //}

            //진입 검색된 종목이 계좌잔고 그리드에 존재하면 반복매수 아니면 신규매수
            var t0424Result = from item in t0424VoList
                             where item.expcode == shcode
                            select item;
           
            //DataRow[] dataRowArray = mainForm.dataTable_t0424.Select("expcode = '" + shcode + "'");
            if (t0424Result.Count() > 0){   
                //보유종목이면..하이라키...
                accountAt = "반복매수";
                mainForm.grd_t1833.Rows[addIndex].Cells["shcode"].Style.BackColor = Color.Gray;
        
                String sunikrt = (String)t0424Result.ElementAt(0).sunikrt;//기존 종목 수익률

                //수익율이 -3% 이하이면 반복매수 해주자.
                if (float.Parse(sunikrt) > Properties.Settings.Default.REPEAT_BUY_RATE)
                {
                    Log.WriteLine("t1833 :: [" + hname + "] 반복매수 " + sunikrt + ">" + Properties.Settings.Default.REPEAT_BUY_RATE + "% 제한");
                    return false;
                }

            }else{//보유종목이 아니고 신규매수해야 한다면.
                accountAt = "신규매수";
                //자본금 = 매입금액 + D2예수금 
                Double baseAmt = this.mainForm.xing_t0424.mamt + int.Parse(this.mainForm.xing_CSPAQ12200.D2Dps);
                   
                if (baseAmt > Properties.Settings.Default.PROGRM_AMT_LIMIT){//이런날이 올까?
                    //Log.WriteLine("ㅡㅡㅡㅡㅡ");
                    baseAmt = Properties.Settings.Default.PROGRM_AMT_LIMIT;
                }

                //4.최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.
                //매입금액 기초자산의 90% 이상 매입을 할수 없다.
                //매입금액 / 자본금 * 100 =자본금 대비 투자율
                Double enterRate = (this.mainForm.xing_t0424.mamt / baseAmt) * 100;
                if (enterRate > Properties.Settings.Default.NEW_BUY_STOP_RATE){ //자본금대비 투자 비율이 높으면 신규매수 하지 않는다.
                    Log.WriteLine("t1833 ::[" + hname + "]  자본금 대비 투자율 "+ this.mainForm.xing_t0424.mamt+"/"+ baseAmt + "*100 = " + enterRate + ">" + Properties.Settings.Default.NEW_BUY_STOP_RATE + "% 제한");
                    return false;
                }      
            }


            //3.매수
            /// <param name="IsuNo">종목번호</param>
            /// <param name="Quantity">수량</param>
            /// <param name="Price">가격</param>
            /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
            int battingAtm = int.Parse(mainForm.textBox_battingAtm.Text);
            //임시로 넣어둔다 왜 현제가가 0으로 넘어오는지 모르겠다.
            if (close == "0"){
                Log.WriteLine("t1833 ::[" + hname + "]  현제가 " + close );
                return false;
            }

            //정규장에만 주문실행.
            if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1530){
                int Quantity = battingAtm / int.Parse(close);
                String buyMsg = "t1833 ::[" + hname + "]  " + accountAt + "   " + close + "원*" + Quantity + "주 매수실행";
                mainForm.xing_CSPAT00600.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw, buyMsg, shcode, Quantity.ToString(), close, "2");
            }else{
                Log.WriteLine("t1833 ::매수 제어");
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
