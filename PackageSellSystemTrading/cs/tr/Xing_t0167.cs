
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
       
        // 생성자
        public Xing_t0167(){
          
            base.ResFileName = "₩res₩t0167.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_t0167(){
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            // 시간 값 세팅
            string szDate    = base.GetFieldData("t0167OutBlock", "dt", 0);
            string szTimeCur = base.GetFieldData("t0167OutBlock", "time", 0);

            mainForm.input_date.Text = szDate;
            if (szTimeCur!= "")
            {
                string szHour = szTimeCur.Substring(0, 2);
                string szMinute = szTimeCur.Substring(2, 2);
                string szSecond = szTimeCur.Substring(4, 2);
                mainForm.input_time.Text = szHour + ":" + szMinute + ":" + szSecond;
            }

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
