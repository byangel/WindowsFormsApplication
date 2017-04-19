
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

            //4.주문처리유형코드,ordtrxptncode,
            // 0    정상                    
            //6    정정확인                
            //7    정정거부(채권)
            //8    취소확인                
            //9    취소거부(채권)

            RealSc1Vo realSc1Vo = new RealSc1Vo();
            realSc1Vo.ordno         = base.GetFieldData("OutBlock", "ordno");      //주문번호
            realSc1Vo.ordptncode    = base.GetFieldData("OutBlock", "ordptncode"); //주문구분 01:매도|02:매수 
            realSc1Vo.ordtrxptncode = base.GetFieldData("OutBlock", "ordptncode"); // 0:정상|6:정정확인 |7:정정거부(채권) |8:취소확인 |9:취소거부(채권)
            realSc1Vo.Isuno         = base.GetFieldData("OutBlock", "shtnIsuno");  //종목코드
            realSc1Vo.Isunm         = base.GetFieldData("OutBlock", "Isunm");      //종목명 
            realSc1Vo.ordqty        = base.GetFieldData("OutBlock", "ordqty");     //ordqty//주문수량
            realSc1Vo.ordprc        = base.GetFieldData("OutBlock", "ordprc");     //ordprc//주문가격
            realSc1Vo.execqty       = base.GetFieldData("OutBlock", "execqty");    //execqty//체결수량
            realSc1Vo.execprc       = base.GetFieldData("OutBlock", "execprc");    //execprc//체결가격
            realSc1Vo.avrpchsprc    = base.GetFieldData("OutBlock", "avrpchsprc"); //평균매입가 -실서버에서 제공하지 않는필드
            realSc1Vo.pchsant       = base.GetFieldData("OutBlock", "pchsant");    //매입금액  -실서버에서 제공하지 않는필드
            realSc1Vo.accno         = base.GetFieldData("OutBlock", "accno");      //계좌번호

            Log.WriteLine("real ::실시간 체결확인: 계좌번호:"+ realSc1Vo.accno + "|주문번호"+ realSc1Vo.ordno +"|"+ realSc1Vo.Isunm + "("+ realSc1Vo.Isuno + ")|주문수량:" + realSc1Vo.ordqty +"|체결수량:" + realSc1Vo.execqty +"|거래구분:" + realSc1Vo.ordptncode + "|평균매입가:" + realSc1Vo.avrpchsprc +"|체결가걱:"+ realSc1Vo.execprc);
            //Isuno +","+ shtnIsuno


            //매매 체결수량 업데이트
            if (!mainForm.dataLog.updateDataExecqty(realSc1Vo))
            {
                //업데이트 실패하면 - dataFile에 해당 종목이 아직 추가 되지 않은것같다.
                Log.WriteLine("real_SC1 :: 매매 체결수량 업데이트 실패 [종목코드:" + realSc1Vo.Isuno + "]");
            }
            

            //실시간 매도가능수량 업데이트(3초마다업데이트되어서 안해줘도되는데...) ->매도가 이루어지면 실시간으로 매도가능수량을 적용해주자.
            EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
            int findIndex = t0424VoList.Find("expcode", realSc1Vo.Isuno.Replace("A", ""));
            if (findIndex >= 0){
                mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Style.BackColor = Color.Gray;

                if (realSc1Vo.ordptncode == "01")//매도 - 매도가능수량-체결수량
                {
                    //1.t0424 에 매도가능수량 실시간 출력                   
                    mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Value = (int.Parse(t0424VoList.ElementAt(findIndex).mdposqt) - int.Parse(realSc1Vo.execqty)).ToString();
                    mainForm.input_toDayAtm.Text = mainForm.dataLog.getToDaySellAmt();//이상하게 금일매도매수 금액이 잡힌다.
                }
                else if (realSc1Vo.ordptncode == "02")//매수 - 매도가능수량+체결수량
                {
                    mainForm.grd_t0424.Rows[findIndex].Cells["c_mdposqt"].Value = (int.Parse(t0424VoList.ElementAt(findIndex).mdposqt) + int.Parse(realSc1Vo.execqty)).ToString();
                }

                //수정된 평균단가를 실시간 적용해준다.  =======================편입일자와 등록할때 카운트를해서 순서를 알수 있도록 수정하자.
                HistoryVo historyvo = mainForm.dataLog.getHistoryVo(realSc1Vo.Isuno.Replace("A", ""));
                if (historyvo != null)
                {
                    mainForm.grd_t0424.Rows[findIndex].Cells["pamt2"].Value     = historyvo.pamt2;
                    mainForm.grd_t0424.Rows[findIndex].Cells["sellCnt"].Value   = historyvo.sellCnt;
                    mainForm.grd_t0424.Rows[findIndex].Cells["buyCnt"].Value    = historyvo.buyCnt;
                    mainForm.grd_t0424.Rows[findIndex].Cells["sellSunik"].Value = historyvo.sellSunik;
                    //여기서는 평균단가만 적용해주자...실시간 현재가 이벤트 발생시에만 수익률을 계산하여 매매를 하자.
              
                    //평균단가 구한후 평규단가를 기준으로 수익율을 구한다.
                   
                }
               
                //매도가능수량이 0보다 작으면 잔고그리드와 dataLog에서 제거해주자.
                if (double.Parse(t0424VoList.ElementAt(findIndex).mdposqt) <= 0)
                {
                    mainForm.deleteCallBack(realSc1Vo.Isuno);
                    Log.WriteLine("real_SC1 deleteCallBack :: 청산된 종목 그리드와 DataLog Line 제거.[종목코드:" + realSc1Vo.Isuno + "]");
                }
            }else{//실시간 체결정보인데 종목그리드에 존재하지 않는 종목일경우 t0424 를 호출해준다.--신규매수일수잇다.
                mainForm.xing_t0424.call_request(mainForm.accountForm.account, mainForm.accountForm.accountPw);
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


    public class RealSc1Vo
    {
        public String Isuno         { set; get; } //종목코드
        public String Isunm         { set; get; } //종목명
        public String ordno         { set; get; } //주문번호
        public String orgordno      { set; get; } //원주문번호
        public String execno        { set; get; } //체결번호
        public String ordqty        { set; get; } //주문수량
        public String ordprc        { set; get; } //주문가격
        public String execqty       { set; get; } //체결수량
        public String execprc       { set; get; } //체결가격
        public String rjtqty        { set; get; }  //거부수량
        public String ordtrxptncode { set; get; } //주문처리유형코드  // 0:정상|6:정정확인 |7:정정거부(채권) |8:취소확인 |9:취소거부(채권)
        public String mtiordseqno   { set; get; } //복수주문일련번호
        public String avrpchsprc    { set; get; } //평균매입가
        public String pchsant       { set; get; } //매입금액
        public String deposit       { set; get; } //예수금
        public String substamt      { set; get; } //대용금액
        public String accno         { set; get; } //계좌번호
        public String ordptncode    { set; get; } //주문구분 01:매도|02:매수 
                                                                    
                                                 


        //2017-04-09 후회할까? 
    }

}   // end namespace
