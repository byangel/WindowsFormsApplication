﻿
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
//코스닥 실시간 가격 수신
namespace PackageSellSystemTrading{
 //코스닥 현재가 실시간
    public class Real_K3 : XARealClass{
        
        public MainForm    mainForm;
       
     // 생성자
        public Real_K3(){
            base.ResFileName = "₩res₩K3_.res";

            base.ReceiveRealData  += new _IXARealEvents_ReceiveRealDataEventHandler(receiveDataEventHandler);
         //base.ReceiveMessage   += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }// end function

     /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            String shcode = base.GetFieldData("OutBlock", "shcode"); //단축종목코드
            String price  = base.GetFieldData("OutBlock", "price");  //현재가
         //MessageBox.Show(expcode);
            int findIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", shcode.Replace("A", ""));
            if (findIndex >= 0){ 
                mainForm.grd_t0424.Rows[findIndex].Cells["price"].Value = Double.Parse(price);
            }

            //주문 정정을 위해 추가한다.
            int t0425Index = mainForm.xing_t0425.getT0425VoList().Find("expcode", shcode.Replace("A", ""));
            if (t0425Index >= 0)
            {
                mainForm.grd_t0425.Rows[t0425Index].Cells["currentPrice"].Value = Double.Parse(price);
            }
            //var T0425VoList = (List<T0425Vo>)mainForm.xing_t0425.getT0425VoList().Select(item => item.expcode = shcode );

        }

     /// <summary>
     /// 종목 실시간 체결 등록
     /// </summary>
     /// <param name="IsuNo">종목번호</param>
        public void call_real(String shcode){
            base.SetFieldData("InBlock", "shcode", shcode);      // 종목번호
            base.AdviseRealData();
        }	// end function

    } //end class 

}// end namespace
