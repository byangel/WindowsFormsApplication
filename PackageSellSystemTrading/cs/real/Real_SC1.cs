﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Threading;

namespace PackageSellSystemTrading{
    //현물계좌 예수금/주문가능금액/총평가 조회(API)
    public class Real_SC1 : XARealClass{

       
        public MainForm    mainForm;

       

        // 생성자
        public Real_SC1()
        {
            base.ResFileName = "₩res₩SC1.res";

            base.ReceiveRealData  += new _IXARealEvents_ReceiveRealDataEventHandler(receiveDataEventHandler);
            //base.ReceiveMessage   += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Real_SC1()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            //isuno//종목번호
            //isunm//종목명
            //ordno//주문번호
            //orgordno//원주문번호
            //execno//체결번호
            //ordqty//주문수량
            //ordprc//주문가격
            //execqty//체결수량
            //execprc//체결가격
            //rjtqty //거부수량
            //ordtrxptncode//주문처리유형코드
            //mtiordseqno//복수주문일련번호
            //avrpchsprc//평균매입가
            //pchsant//매입금액
            //deposit//예수금
            //substamt//대용금액
            String ordptncode = base.GetFieldData("OutBlock", "ordptncode"); //주문구분 01:매수|02:매도 
            String ordqty  = base.GetFieldData("OutBlock", "ordqty"); //주문수량 
            String execqty = base.GetFieldData("OutBlock", "execqty");  //체결수량 
            String isunm   = base.GetFieldData("OutBlock", "isunm");  //종목명 
            String isuno   = base.GetFieldData("OutBlock", "isuno");  //종목번호 
            Log.WriteLine("real ::실시간 체결확인 종목명/주문수량/체결수량|거래구분["+ isunm + "|" + ordqty + "|" + execqty+"|"+ ordptncode + "]");
        
            //주문수량과 체결수량이 같으면 실시간 요청을 취소한다.
            //if (ordqty == execqty)
            //{
            //    //base.UnadviseRealData();//모든 종목의 요청을 취소함
                
            //    Log.WriteLine("[" + mainForm.xing_t0167.date + "]real ::실시간요청취소");
            //}

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

            base.AdviseRealData();

        }	// end function
//1.주문체결유형코드,ordxctptncode
// 01                   주문
//02                   정정
//03                   취소
//11                   체결
//12                   정정확인
//13                   취소확인
//14                   거부

//2.주문시장코드,ordmktcode,
// 04 채권 
//10 KSE
// 20 KOSDAQ
// 30 K-OTC
//23 KONEX 

//3.주문유형코드,ordptncode,
// 00                   해당없음
//01                   현금매도
//02                   현금매수
//03                   신용매도
//04                   신용매수

//4.주문처리유형코드,ordtrxptncode,
// 0    정상                    
//6    정정확인                
//7    정정거부(채권)
//8    취소확인                
//9    취소거부(채권)

//5.주문거래유형코드,ordtrdptncode
// 00                   위탁
//01                   신용
//04                   선물대용

//6.신용거래코드,mgntrncode,
//  [신규]      
//000 : 보통                          
//001 : 유통융자신규                  
//003 : 자기융자신규                  
//005 : 유통대주신규                  
//007 : 자기대주신규                  
//080 : 예탁주식담보융자신규          
//082 : 예탁채권담보융자신규

//[상환] 
//101 : 유통융자상환                  
//103 : 자기융자상환                  
//105 : 유통대주상환                  
//107 : 자기대주상환                  
//111 : 유통융자전액상환              
//113 : 자기융자전액상환              
//180 : 예탁주식담보융자상환          
//182 : 예탁채권담보융자상환          
//188 : 담보대출전액상환

//이용에 참고하시기 바랍니다.


    } //end class 

}   // end namespace
