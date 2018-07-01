
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
    public class Xing_t0167 : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public string date { get; set; }
        public string time { get; set; }

        public string year { get; set; }
        public string moon { get; set; }
        public string day { get; set; }

        public string hour { get; set; }
        public string minute { get; set; }
        public string second { get; set; }

        public Boolean initAt = false;

        // 생성자
        public Xing_t0167(){
          
            base.ResFileName = "₩res₩t0167.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            // 시간 값 세팅
            this.date = base.GetFieldData("t0167OutBlock", "dt", 0);
            this.time = base.GetFieldData("t0167OutBlock", "time", 0);
            if (this.date != "")
            {
                this.year = this.date.Substring(0, 4);
                this.moon = this.date.Substring(4, 2);
                this.day = this.date.Substring(6, 2);
                //mainForm.label_date.Text = this.year + "-" + this.moon + "-" + this.day;

            }
           
            if (this.time != "")
            {
                this.hour   = this.time.Substring(0, 2);
                this.minute = this.time.Substring(2, 2);
                this.second = this.time.Substring(4, 2);
                //mainForm.label_time.Text = this.hour + ":" + this.minute + ":" + this.second;
            }
            mainForm.tradingInfoDt.Rows[0]["날자"] = this.year + "-" + this.moon + "-" + this.day +" "+ this.hour + ":" + this.minute + ":" + this.second;
            completeAt = true;//중복호출 방지
            
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                completeAt = true;//중복호출 방지
            }
            

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(){
    
            if (completeAt) {
                base.SetFieldData("t0167InBlock", "id", 0, "");
                base.Request(false);
            }
        }

    } //end class 
   
}   // end namespace
