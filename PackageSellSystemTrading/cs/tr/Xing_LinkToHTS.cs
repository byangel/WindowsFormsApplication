
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
    public class Xing_LinkToHTS : XAQueryClass{


        public MainForm mainForm;


        // 생성자
        public Xing_LinkToHTS(){
            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        

        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
  
        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public bool call_request(String shcode)
        {
            //base.SetFieldData("t0167InBlock", "id", 0, "");
            //base.Request(false);
            base.RequestLinkToHTS("&OPEN_SCREEN", "4001", "");
            return base.RequestLinkToHTS("&STOCK_CODE", shcode, "");
 
        }

    } //end class 
   
}   // end namespace
