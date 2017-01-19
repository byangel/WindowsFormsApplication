
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
            mainForm.grd_t1833.Rows.Clear();
            string[] row = new string[7];
            int addIndex;

           
            String shcode;//종목코드
            String hname;//종목명
            String close; //현재가
            //String sunikrt;//수익률
            for (int i = 0; i < iCount; i++) {
                shcode = base.GetFieldData("t1833OutBlock1", "shcode", i);//종목코드
                close  = base.GetFieldData("t1833OutBlock1", "close", i); //현재가
                hname  = base.GetFieldData("t1833OutBlock1", "hname", i); //종목명

                row[0] = shcode;//종목코드
                row[1] = hname; //종목명
                row[2] = Util.GetNumberFormat(close); //현재가
                row[3] = base.GetFieldData("t1833OutBlock1", "sign"   , i); //전일대비구분 
                row[4] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "change" , i)); //전일대비
                row[5] = base.GetFieldData("t1833OutBlock1", "diff"   , i); //등락율
                row[6] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "volume" , i)); //거래량
                //row[0] = base.GetFieldData("t1833OutBlock1", "signcnt", i);//연속봉수
                //1.그리드 데이터 추가
                addIndex = mainForm.grd_t1833.Rows.Add(row);

                this.buy(shcode, hname, close, addIndex);   
            }

            mainForm.input_t1833_log2.Text = "[" + mainForm.input_time.Text+ "]조건검색 응답 완료";
            completeAt = true;//중복호출 여부
        }


       

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
              
            } else { 
                Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t1833_log2.Text = "[" + mainForm.input_time.Text + "]t1833 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지
            }         

        }
        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(string conditionFileName){

            
            if (completeAt) {
                //MessageBox.Show("1");
                //폼 메세지.
                completeAt = false;//중복호출 방지
                mainForm.input_t1833_log1.Text = "[" + mainForm.input_time.Text + "]조건검색 요청.";
                //Thread.Sleep(1000);

                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                base.RequestService("t1833", startupPath + "\\Resources\\"+ conditionFileName);//_6만급등족목_70_

                //mainForm.input_t1833_log2.Text = "대기";

            } else {
                mainForm.input_t1833_log1.Text = "[중복]조건검색 요청.";
                //mainForm.input_t1833_log2.Text = "대기";
            }
        }


        //진입검색에서 검색된 종목을 매수한다.
        private Boolean buy(String shcode,String hname, String close, int addIndex){
            String time = mainForm.xing_t0167.time;
            //if (time == "" ) { time = "1530"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60) + int.Parse(time.Substring(2, 2));//현재 시간

            //1.미체결항목에 매수 항목이 있는지 확인하자.
            //2.반복매수 - 보유종목 매수 시기가 1시간이상 이전이고 수익률이 -3% 이하이면 반복매수한다.
            //3.실자본금 또는 보유금액 대비 95% 이상 매입이 이루어 졌을경우 신규 매수하지 않는다.
            //4.최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.
            Boolean shcodeInT0425At = false;//미체결 항목 여부
            String ordrem;  //미체결잔량
            String medosu;  //매매구분 - 0:전체|1:매수|2:매도
            String expcode; //종목번호
            String ordtime; //주문시간
            int tmpTime;
            for (int i = 0; i < mainForm.xing_t0425.GetBlockCount("t0425OutBlock1"); i++)
            {

                ordrem = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "ordrem", i);//미체결 잔량 - 매도또는 매수 주문후  잔량이 있다면 걔좌에 종목이 있다는뜻이므로 미체결 목록에 뿌려준다.
                medosu = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "medosu", i); //매매구분 - 0:전체|1:매수|2:매도
                expcode = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "expcode", i); //종목번호
                ordtime = mainForm.xing_t0425.GetFieldData("t0425OutBlock1", "ordtime", i); //주문시간
                                                                                            //Log.WriteLine("t1833 :: " + "/" + shcode + "/" + i+"/"+ medosu);

                //검색된 종목이 금일 매수이력에 있다면.
                if (medosu == "매수" && expcode == shcode)
                {

                    //미체결목록 -- 미체결 잔량이 있다면...
                    if (int.Parse(ordrem) > 0)
                    {
                        Log.WriteLine("[" + mainForm.input_time.Text + "]t1833 :: ["+hname+"] 미체결 잔량 " + ordrem + "주 매수 제한");
                        return false;
                    }
                    //반복매수 제한-분으로 푼다음 시간을 계산한다.
                    tmpTime = (int.Parse(ordtime.Substring(0, 2)) * 60) + int.Parse(ordtime.Substring(2, 2));
                    tmpTime = (cTime - tmpTime);
                    if (tmpTime < Properties.Settings.Default.REPEAT_BUY_TERM)
                    {
                        Log.WriteLine("[" + mainForm.input_time.Text + "]t1833 :: [" + hname + "] 금일 반복매수 " + Properties.Settings.Default.REPEAT_BUY_TERM + "분 제한");
                        return false;
                    }
                }

                //Log.WriteLine("t1833 :: " + "/" + shcode + "/" + i + "/" + medosu + "/"+ ordtime);
                shcodeInT0425At = true;
                //Log.WriteLine("t1833 :: " + "/(" + time+")"+cTime + "-" + "("+ordtime+")"+ordMTime + "=" + (cTime- ordMTime));

            }

            //진입 검색된 종목이 기존 그리드에 존재하면 반복매수 아니면 신규매수
            DataRow[] dataRowArray = mainForm.dataTable_t0424.Select("expcode = '" + shcode + "'");

            if (dataRowArray.Length > 0)
            {//보유종목이면..하이라키...
                mainForm.grd_t1833.Rows[addIndex].Cells["shcode"].Style.BackColor = Color.Gray;
                //mainForm.grd_t1833.Rows[addIndex].DefaultCellStyle.BackColor = Color.Gray;
                //Log.WriteLine("t1833 :: " + dataRowArray[0]["hname"]+"/"+ dataRowArray[0]["sunikrt"]);

                String sunikrt = (String)dataRowArray[0]["sunikrt"];//기존 종목 수익률

                //수익율이 -3% 이하이면 반복매수 해주자.
                if (float.Parse(sunikrt) > Properties.Settings.Default.REPEAT_BUY_RATE)
                {
                    Log.WriteLine("[" + mainForm.input_time.Text + "]t1833 :: [" + hname + "] 반복매수 " + sunikrt + ">" + Properties.Settings.Default.REPEAT_BUY_RATE + "% 제한");
                    return false;
                }

            }
            else{//보유종목이 아니고 신규매수해야 한다면.

                //자본금 = 매입금액 + 예수금 
                Double tmpAmt = this.mainForm.xing_t0424.mamt + this.mainForm.xing_t0424.sunamt1;
                   
                if (tmpAmt > Properties.Settings.Default.PROGRM_AMT_LIMIT){//이런날이 올까?
                    tmpAmt = Properties.Settings.Default.PROGRM_AMT_LIMIT;
                }

                //4.최대 운영 설정금액 이상일경우 매수 신규매수 하지 않는다.
                //매입금액 기초자산의 90% 이상 매입을 할수 없다.
                //매입금액 / 자본금 * 100 =자본금 대비 투자율
                tmpAmt = (this.mainForm.xing_t0424.mamt / tmpAmt) * 100;
                if (tmpAmt > Properties.Settings.Default.NEW_BUY_STOP_RATE){ //자본금대비 투자 비율이 높으면 신규매수 하지 않는다.
                    Log.WriteLine("[" + mainForm.input_time.Text + "]t1833 ::[" + hname + "]  자본금 대비 투자율 " + tmpAmt + ">" + Properties.Settings.Default.NEW_BUY_STOP_RATE + "% 제한");
                    return false;
                }
            }


            
            
            //3.매수
            /// <param name="IsuNo">종목번호</param>
            /// <param name="Quantity">수량</param>
            /// <param name="Price">가격</param>
            /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
            int battingAtm = int.Parse(mainForm.textBox_battingAtm.Text);
            int Quantity = battingAtm / int.Parse(close);
            mainForm.xing_CSPAT00600.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw, shcode, Quantity.ToString(), "", "2");
            Log.WriteLine("[" + mainForm.input_time.Text + "]t1833 ::[" + hname + "]  "+ Quantity+ "주 매수실행");
            //Log.WriteLine("t1833.ReceiveDataEventHandler :: ");
            return true;
        }

} //end class 
   
}   // end namespace
