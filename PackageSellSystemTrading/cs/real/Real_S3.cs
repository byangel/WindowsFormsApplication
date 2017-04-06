
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
    public class Real_S3 : XARealClass{

       
        public MainForm    mainForm;

       

        // 생성자
        public Real_S3()
        {
            base.ResFileName = "₩res₩S3_.res";

            base.ReceiveRealData  += new _IXARealEvents_ReceiveRealDataEventHandler(receiveDataEventHandler);
            //base.ReceiveMessage   += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Real_S3()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode)
        {

            String shcode = base.GetFieldData("OutBlock", "shcode");    //단축종목코드
            String price = base.GetFieldData("OutBlock", "price");      //현재가
            
            EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
            int findIndex = t0424VoList.Find("expcode", shcode.Replace("A", ""));
            if (findIndex >= 0)
            {
                t0424VoList.ElementAt(findIndex).price = price;
                //t0424VoList.ResetItem(findIndex);
                //Log.WriteLine("real S3 ::실시간 코스피 체결확인: 종목코드:" + shcode + "|현재가" + price);
            }
        }



        /// <summary>
        /// 종목 실시간 체결 등록
        /// </summary>
        /// <param name="IsuNo">종목번호</param>
        public void call_real(String shcode)
        {   
            base.SetFieldData("InBlock", "shcode", shcode);         // 종목번호
            base.AdviseRealData();
        }	// end function



    } //end class 

}   // end namespace
