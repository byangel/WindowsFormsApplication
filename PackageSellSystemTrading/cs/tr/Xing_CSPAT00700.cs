
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
    public class Xing_CSPAT00700 : XAQueryClass{

        public MainForm mainForm;
        private  String shcode; //종목코드
        private String  OrgOrdNo;//주문번호
        public Boolean completeAt = true;//완료여부.
     // 생성자
        public Xing_CSPAT00700(MainForm mainForm)
        {
            base.ResFileName = "₩res₩CSPAT00700.res";

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
            String OrdNo = base.GetFieldData("CSPAT00700OutBlock2", "OrdNo", 0);//주문번호 --block2에서는 주문번호만 참조하면 될듯.
            if (OrdNo != "0")
            {
                String PrntOrdNo = base.GetFieldData("CSPAT00700OutBlock1", "PrntOrdNo", 0);//부모주문번호
                                                                                          
                String IsuNo = base.GetFieldData("CSPAT00700OutBlock1", "IsuNo", 0);//종목번호
            
                String IsuNm = base.GetFieldData("CSPAT00700OutBlock1", "IsuNm", 0);//종목명 -안넘어온다.

                mainForm.insertListBoxLog("<CSPAT00700::정정주문완료>" + IsuNm + "<주문번호:" + OrdNo + ">");
                Log.WriteLine("<CSPAT00700::정정주문완료>" + IsuNm + "<주문번호:" + OrdNo + ">");
                

                //정정주문
                if (OrgOrdNo != "")
                {
                 //주문이 잘못되었을경우 매도여부를 초기화 해준다.
                    int findIndexT0425 = mainForm.xing_t0425.getT0425VoList().Find("ordno", this.OrgOrdNo);//upOrdno
                    //if (findIndexT0425 >= 0)
                    //{
                    //    //금일매도여부 상관없이 그냥 N해준다..
                    //    mainForm.grd_t0425.Rows[findIndexT0425].Cells["sellOrdAt"].Value = "N"; //매도주문여부 초기화

                     
                    //}

                }//receiveMessageEventHandler END
        

            }
            
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("CSPAT00700 :: " + nMessageCode + " :: " + szMessage);
                

             //02714 :: 주문수량이 매매가능수량을 초과했습니다 .
                if (nMessageCode== "02714")
                {

                }
                
            }
            completeAt = true;

        }

        private int callCnt = 0;
     /// <summary>
		/// 현물 정정 주문
		//OrgOrdNo(원주문번호), IsuNo(종목번호), OrdQty(주문수량), price(주문가격)
		public void call_request(String account, String accountPw, string OrgOrdNo, string IsuNo, Double OrdQty, Double price)
        {
            this.completeAt = false;//중복호출 방지

            this.shcode = IsuNo;
            this.OrgOrdNo = OrgOrdNo;
            //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                IsuNo = "A" + IsuNo;
            }
            base.SetFieldData("CSPAT00700InBlock1", "AcntNo"        , 0, account    ); // 계좌번호
            base.SetFieldData("CSPAT00700InBlock1", "InptPwd"       , 0, accountPw  ); // 입력비밀번호
            base.SetFieldData("CSPAT00700InBlock1", "OrgOrdNo"      , 0, OrgOrdNo   ); // 원주문번호
            base.SetFieldData("CSPAT00700InBlock1", "IsuNo"         , 0, IsuNo      ); // 종목번호
            base.SetFieldData("CSPAT00700InBlock1", "OrdQty"        , 0, OrdQty.ToString()     ); // 주문수량

            base.SetFieldData("CSPAT00700InBlock1", "OrdprcPtnCode" , 0, "00"       ); //호가유형코드 --지정
            base.SetFieldData("CSPAT00700InBlock1", "OrdCndiTpCode" , 0, "0"        ); //주문조건구분 0:없음, 1:IOC, 2:FOK
            base.SetFieldData("CSPAT00700InBlock1", "OrdPrc"        , 0, price.ToString());//주문가
            
            base.Request(false);
        }


    } //end class 
   
}// end namespace
