
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Threading;

namespace PackageSellSystemTrading{
    //현물계좌 예수금/주문가능금액/총평가 조회(API)
    public class Real_K3 : XARealClass{

       
        public MainForm    mainForm;

       

        // 생성자
        public Real_K3()
        {
            base.ResFileName = "₩res₩K3_.res";

            base.ReceiveRealData  += new _IXARealEvents_ReceiveRealDataEventHandler(receiveDataEventHandler);
            //base.ReceiveMessage   += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Real_K3()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){


        }



        /// <summary>
        /// 종목 실시간 체결 등록
        /// </summary>
        /// <param name="IsuNo">종목번호</param>
        public void call_real(String shcode)
        {
            //모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + shcode;
            }

            //base.SetFieldData("InBlock", "shcode", shcode);         // 종목번호

            //base.AdviseRealData();

        }	// end function

    } //end class 

}   // end namespace
