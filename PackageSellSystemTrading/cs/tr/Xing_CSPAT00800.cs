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
using System.Data;

namespace PackageSellSystemTrading{
 //현물 취소주문
    public class Xing_CSPAT00800 : XAQueryClass{

        public MainForm mainForm;
        private  String shcode; //종목코드
        private String  upOrdno;//주문번호
        public Boolean completeAt = true;//완료여부.
     // 생성자
        public Xing_CSPAT00800(MainForm mainForm)
        {
            base.ResFileName = "₩res₩CSPAT00800.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);


            completeAt = false;
            this.mainForm = mainForm;
        }// end function

       

     /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            String OrdNo = base.GetFieldData("CSPAT00800OutBlock2", "OrdNo", 0);//주문번호 --block2에서는 주문번호만 참조하면 될듯.
            if (OrdNo != "0")
            {
             //String RecCnt = base.GetFieldData("CSPAT00800OutBlock1", "RecCnt", 0);//레코드갯수
             //String AcntNo = base.GetFieldData("CSPAT00800OutBlock1", "AcntNo", 0);//계좌번호
                String IsuNo = base.GetFieldData("CSPAT00800OutBlock1", "IsuNo", 0);//종목번호
             //String OrdQty = base.GetFieldData("CSPAT00800OutBlock1", "OrdQty", 0);//주문수량
             //String OrdPrc = base.GetFieldData("CSPAT00800OutBlock1", "OrdPrc", 0);//주문가격
             //String BnsTpCode = base.GetFieldData("CSPAT00800OutBlock1", "BnsTpCode", 0);//매매구분
             ////Log.WriteLine("CSPAT00600 block1:: [레코드:"+ RecCnt + "|계좌번호:" + AcntNo + "|종목번호:" + IsuNo + "|주문수량:" + OrdQty + "| 주문가격:" + OrdPrc + " | 매매구분:" + BnsTpCode + "]");
            
             //String RecCnt2 = base.GetFieldData("CSPAT00600OutBlock2", "RecCnt", 0);//레코드갯수
                
             //String OrdTime = base.GetFieldData("CSPAT00600OutBlock2", "OrdTime", 0);//주문시각
             //String OrdMktCode = base.GetFieldData("CSPAT00600OutBlock2", "OrdMktCode", 0);//주문시장코드
             //String OrdPtnCode = base.GetFieldData("CSPAT00600OutBlock2", "OrdPtnCode", 0);//주문유형코드
             //String ShtnIsuNo = base.GetFieldData("CSPAT00600OutBlock2", "ShtnIsuNo", 0);//단축종목번호
             //String MgempNo = base.GetFieldData("CSPAT00600OutBlock2", "MgempNo", 0);//관리사원번호
             //String OrdAmt = base.GetFieldData("CSPAT00600OutBlock2", "OrdAmt", 0);//주문금액
             //String SpotOrdQty = base.GetFieldData("CSPAT00800OutBlock1", "SpotOrdQty", 0);//실물주문수량  noData
             //String MnyOrdAmt = base.GetFieldData("CSPAT00800OutBlock1", "MnyOrdAmt", 0);//현금주문금액  noData
             //String AcntNm = base.GetFieldData("CSPAT00800OutBlock1", "AcntNm", 0);//계좌명
                String IsuNm = base.GetFieldData("CSPAT00800OutBlock1", "IsuNm", 0);//종목명 -안넘어온다.
                Log.WriteLine("CSPAT00800::취소주문완료" + IsuNm + "(" + IsuNo + ") 주문번호:"+ OrdNo);
             ////Log.WriteLine("CSPAT00600 block2:: [레코드:" + RecCnt2 + "|주문번호:" + OrdNo + "|단축종목번호:" + ShtnIsuNo + "|주문금액:" + OrdAmt + "|실물주문수량:" + SpotOrdQty + "|종목명:" + IsuNm + "]");


             //취소주문이 성공적으로 완료되면 재주문이 들어갈수있도록 t0424와 t0425  주문여부 상태를 초기화 해준다.
                int findIndex = mainForm.xing_t0424.getT0424VoList().Find("expcode", this.shcode);
                if (findIndex >= 0){
                    mainForm.grd_t0424.Rows[findIndex].Cells["orderAt"].Value = "C";//데이타를 새로 수신 받을때 다시 N상태로 해야함.
                }


             //금일매도 주문이 취소되었을경우이다.
                if (upOrdno != "")
                {
                 //주문이 잘못되었을경우 매도여부를 초기화 해준다.
                    int findIndexT0425 = mainForm.xing_t0425.getT0425VoList().Find("ordno", this.upOrdno);//upOrdno
                    if (findIndexT0425 >= 0)
                    {
                     //금일매도여부 상관없이 그냥 N해준다..
                        mainForm.grd_t0425.Rows[findIndexT0425].Cells["sellOrdAt"].Value = "N"; //매도주문여부 초기화

                     //상위주문번호 주문여부 Y로 업데이트
                        var items = from item in mainForm.tradingHistory.getTradingHistoryDt().AsEnumerable()
                                    where item["ordno"].ToString() == this.upOrdno
                                        && item["accno"].ToString() == mainForm.account
                                    select item;
                        if (items.Count() > 0)
                        {
                            items.First()["sellOrdAt"] = "N";
                            mainForm.tradingHistory.sellOrdAtUpdate(items.First());//매도주문 여부 상태 업데이트
                        }
                    }

                }//receiveMessageEventHandler END
              //history 에 등록된 주문이력도 같이 삭제 하는데 이때 체결수량이 0인경우에만 삭제한다.
              //mainForm.tradingHistory.execqtyDelete(IsuNo);굳이 삭제 하지말자.

            }
            
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("CSPAT00800 :: " + nMessageCode + " :: " + szMessage);
                

             //02714 :: 주문수량이 매매가능수량을 초과했습니다 .
                if (nMessageCode== "02714")
                {

                }

          
                

            }
            completeAt = true;

        }

        private int callCnt = 0;
        /// <summary>
        /// 현물 취소 주문
        //OrgOrdNo(원주문번호), IsuNo(종목번호), OrdQty(주문수량)
        public void call_request(String account, String accountPw, string upOrdNo, string paramShcode, string OrdQty)
        {
            this.completeAt = false;//중복호출 방지

            this.shcode = paramShcode;
            this.upOrdno = upOrdNo;
         //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                paramShcode = "A" + paramShcode;
            }

         //2.실시간 체결 정보요청 취소.
         //mainForm.real_SC1.UnadviseRealDataWithKey(shcode);

         //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
         //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

            base.SetFieldData("CSPAT00800InBlock1", "AcntNo"    , 0, account    );// 계좌번호
            base.SetFieldData("CSPAT00800InBlock1", "InptPwd"   , 0, accountPw  );// 입력비밀번호
            base.SetFieldData("CSPAT00800InBlock1", "OrgOrdNo"  , 0, upOrdNo    );// 원주문번호
            base.SetFieldData("CSPAT00800InBlock1", "IsuNo"     , 0, paramShcode);// 종목번호
            base.SetFieldData("CSPAT00800InBlock1", "OrdQty"    , 0, OrdQty     );// 주문수량

            base.Request(false);

            
            
        }


    } //end class 
   
}// end namespace
