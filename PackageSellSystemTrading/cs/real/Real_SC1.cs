
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
            String ordno      = base.GetFieldData("OutBlock", "ordno");      //주문번호
            String ordptncode = base.GetFieldData("OutBlock", "ordptncode"); //주문구분 01:매도|02:매수 
            String Isunm      = base.GetFieldData("OutBlock", "Isunm");      //종목명 
            //String Isuno      = base.GetFieldData("OutBlock", "Isuno");      //종목번호 
            String shtnIsuno  = base.GetFieldData("OutBlock", "shtnIsuno");

            String ordqty     = base.GetFieldData("OutBlock", "ordqty");     //ordqty//주문수량
            String ordprc     = base.GetFieldData("OutBlock", "ordprc");     //ordprc//주문가격
            String execqty    = base.GetFieldData("OutBlock", "execqty");    //execqty//체결수량
            String execprc    = base.GetFieldData("OutBlock", "execprc");    //execprc//체결가격

            String avrpchsprc = base.GetFieldData("OutBlock", "avrpchsprc");//평균매입가 -실서버에서 제공하지 않는필드
            String pchsant    = base.GetFieldData("OutBlock", "pchsant");   //매입금액  -실서버에서 제공하지 않는필드
            String accno      = base.GetFieldData("OutBlock", "accno");   //계좌번호

            Log.WriteLine("real ::실시간 체결확인: 계좌번호:"+accno+ "|주문번호"+ordno+"|"+ Isunm + "("+  shtnIsuno+ ")|주문수량:" + ordqty+"|체결수량:" + execqty+"|거래구분:" + ordptncode+ "|평균매입가:" + avrpchsprc+"|체결가걱:"+execprc);
            //Isuno +","+ shtnIsuno


            //매매 체결이력 저장
            DataLogVo dataLogVo  = new DataLogVo();
            dataLogVo.accno      = accno;       //계좌번호
            dataLogVo.ordptncode = ordptncode;  //주문구분 01:매도|02:매수
            dataLogVo.Isuno      = shtnIsuno;   //종목코드
            dataLogVo.ordno      = ordno;       //주문번호
            dataLogVo.ordqty     = ordqty;      //주문수량 - 매도가능수량
            dataLogVo.execqty    = execqty;     //체결수량 - 매도가능수량
            dataLogVo.ordprc     = ordprc;      //주문가격 - 평균단가
            dataLogVo.execprc    = execprc;     //체결가격 - 평균단가
            dataLogVo.Isunm      = Isunm;       //종목명
            mainForm.dataLog.writeLine(dataLogVo);
            

            //실시간 매도가능수량 업데이트(3초마다업데이트되어서 안해줘도되는데...) ->매도가 이루어지면 실시간으로 매도가능수량을 적용해주자.
            EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
            int findIndex = t0424VoList.Find("expcode", shtnIsuno.Replace("A", ""));
            if (findIndex >= 0)
            {
                mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Style.BackColor = Color.Gray;

                if (ordptncode == "01")//매도 - 매도가능수량-체결수량
                {
                    //1.t0424 에 매도가능수량 실시간 출력
                    t0424VoList.ElementAt(findIndex).mdposqt = (int.Parse(t0424VoList.ElementAt(findIndex).mdposqt) - int.Parse(execqty)).ToString();

                    mainForm.input_toDayAtm.Text = mainForm.dataLog.getToDaySellAmt();
                }
                else if (ordptncode == "02")//매수 - 매도가능수량+체결수량
                {
                    t0424VoList.ElementAt(findIndex).mdposqt = (int.Parse(t0424VoList.ElementAt(findIndex).mdposqt) + int.Parse(execqty)).ToString();
                }

                //수정된 평균단가를 실시간 적용해준다.
                HistoryVo historyvo = mainForm.dataLog.getHistoryVo(dataLogVo.Isuno.Replace("A", ""));
                if (historyvo != null)
                {
                    t0424VoList.ElementAt(findIndex).pamt2 = historyvo.pamt;//평균단가2
                    t0424VoList.ElementAt(findIndex).sellCnt = historyvo.sellCnt;//매도 횟수.
                    t0424VoList.ElementAt(findIndex).buyCnt = historyvo.buyCnt;//매수 횟수
                    t0424VoList.ElementAt(findIndex).sellSunik = historyvo.sellSunik;//중간매도손익
                }
               
                //매도가능수량이 0이면 잔고그리드와 dataLog에서 제거해주자.
                if (t0424VoList.ElementAt(findIndex).mdposqt == "0")
                {
                    t0424VoList.RemoveAt(findIndex);
                    mainForm.dataLog.deleteLine(shtnIsuno);
                    Log.WriteLine("real :: 팔린종목 그리드에서 제거[" + shtnIsuno + "]");
                    //dataLog 도 제거해준다.
                }
            }

            

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
