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
using System.Data;

namespace PackageSellSystemTrading {
    //주식 잔고2
    public class Xing_t0424 : XAQueryClass {

        public Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        public int sunamt;    //추정자산
        //public int sunamt1;   //d1예수금
        public int dtsunik;   //실현손익

        public int tmpMamt;      //매입금액
        public int tmpTappamt;   //평가금액
        public int tmpTdtsunik;  //평가손익

        public int mamt;      //매입금액
        public int tappamt;   //평가금액
        public int tdtsunik;  //평가손익
       

        public int h_totalCount;

        public Boolean readyAt;
        // 생성자
        public Xing_t0424()
        {
            base.ResFileName = "₩res₩t0424.res";

            readyAt = false;
            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

        }   // end function

        // 소멸자
        ~Xing_t0424()
        {

        }

        /// <summary>
		/// 주식잔고2(T0424) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode) {
           
            String cts_expcode = base.GetFieldData("t0424OutBlock", "cts_expcode", 0);//CTS_종목번호-연속조회키

            //1.계좌 잔고 목록을 그리드에 추가
            int iCount = base.GetBlockCount("t0424OutBlock1");

            //계좌잔고목록
            BindingList<T0424Vo> t0424VoList = ((BindingList<T0424Vo>)mainForm.grd_t0424.DataSource);

            String expcode;//종목코드

            T0424Vo tmpT0424Vo;
            for (int i = 0; i < iCount; i++) {
                expcode = base.GetFieldData("t0424OutBlock1", "expcode", i); //종목코드
                //hname   = base.GetFieldData("t0424OutBlock1", "hname"  , i); //종목명
                //mdposqt = base.GetFieldData("t0424OutBlock1", "mdposqt", i); //매도가능
                //sunikrt = float.Parse(base.GetFieldData("t0424OutBlock1", "sunikrt", i)); //수익율
                //price   = base.GetFieldData("t0424OutBlock1", "price", i); //현재가


                var result_t0424 =  from item in t0424VoList
                              where item.expcode == expcode
                              select item;
                if (result_t0424.Count() > 0){
                    tmpT0424Vo = result_t0424.ElementAt(0);
                }else{
                    tmpT0424Vo = new T0424Vo();
                }

        
                tmpT0424Vo.expcode = base.GetFieldData("t0424OutBlock1", "expcode" , i); //종목코드
                tmpT0424Vo.hname   = base.GetFieldData("t0424OutBlock1", "hname"   , i); //종목명
                tmpT0424Vo.mdposqt = base.GetFieldData("t0424OutBlock1", "mdposqt" , i); //매도가능
                tmpT0424Vo.price   = base.GetFieldData("t0424OutBlock1", "price"   , i); //현재가
                tmpT0424Vo.appamt  = base.GetFieldData("t0424OutBlock1", "appamt"  , i); //평가금액
                tmpT0424Vo.dtsunik = base.GetFieldData("t0424OutBlock1", "dtsunik" , i); //평가손익
                tmpT0424Vo.sunikrt = base.GetFieldData("t0424OutBlock1", "sunikrt" , i); //수익율
                tmpT0424Vo.pamt    = base.GetFieldData("t0424OutBlock1", "pamt"    , i); //평균단가
                tmpT0424Vo.mamt    = base.GetFieldData("t0424OutBlock1", "mamt"    , i); //매입금액
                tmpT0424Vo.msat    = base.GetFieldData("t0424OutBlock1", "msat"    , i); //당일매수금액
                tmpT0424Vo.mpms    = base.GetFieldData("t0424OutBlock1", "mpms"    , i); //당일매수단가
                tmpT0424Vo.mdat    = base.GetFieldData("t0424OutBlock1", "mdat"    , i); //당일매도금액
                tmpT0424Vo.mpmd    = base.GetFieldData("t0424OutBlock1", "mpmd"    , i); //당일매도단가
                tmpT0424Vo.fee     = base.GetFieldData("t0424OutBlock1", "fee"     , i); //수수료
                tmpT0424Vo.tax     = base.GetFieldData("t0424OutBlock1", "tax"     , i); //제세금
                tmpT0424Vo.sininter= base.GetFieldData("t0424OutBlock1", "sininter", i); //신용이자

                tmpT0424Vo.deleteAt = false; //삭제여부
                
                //1.그리드에 없던 새로 매수된 종목이면 테이블에 추가해준다.
                if (result_t0424.Count() == 0){
                    t0424VoList.Add(tmpT0424Vo);
                }

                if (readyAt) {
                    //수익율 2% 이상 매도 Properties.Settings.Default.SELL_RATE
                    if (int.Parse((String)tmpT0424Vo.mdposqt) > 0 && float.Parse(tmpT0424Vo.sunikrt) > Properties.Settings.Default.SELL_RATE )
                    {
                        if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1530)
                        {
                            /// <param name="IsuNo">종목번호</param>
                            /// <param name="Quantity">수량</param>
                            /// <param name="Price">가격</param>
                            /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                            String msg = "[" + mainForm.input_time.Text + "]t0424 ::매도[" + tmpT0424Vo.hname + "]  수익율:" + tmpT0424Vo.sunikrt + "%   " + tmpT0424Vo.mdposqt + "주매도.매도가능:"+ tmpT0424Vo.mdposqt;
                            mainForm.xing_CSPAT00600.call_request(mainForm.exXASessionClass.account, mainForm.exXASessionClass.accountPw, msg, expcode, tmpT0424Vo.mdposqt, tmpT0424Vo.price, "1");
                            tmpT0424Vo.mdposqt = "0";
                        }
                        else
                        {
                            
                           //Log.WriteLine("t0424 ::매도 제어");
                                
                           
                            
                        }

                   
                    }
                }
                //if ((i % 2) == 0)
                // {
                //     tmpDataRow["hname"] = "xxx";
                //}
                //MessageBox.Show("2");
                //mainForm.dataTable_t0424.AcceptChanges();
                //Thread.Sleep(100);
            }


            // 계좌정보 써머리 계산 - 연속 조회이기때문에 합산후 마지막에 폼으로 출력.
            this.tmpMamt     += int.Parse(base.GetFieldData("t0424OutBlock", "mamt"    , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "mamt"    , 0));//매입금액
            this.tmpTappamt  += int.Parse(base.GetFieldData("t0424OutBlock", "tappamt" , 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tappamt" , 0));//평가금액
            this.tmpTdtsunik += int.Parse(base.GetFieldData("t0424OutBlock", "tdtsunik", 0) == "" ? "0" : base.GetFieldData("t0424OutBlock", "tdtsunik", 0));//평가손익
            

            this.h_totalCount += iCount;

            //2.연속 데이타 정보가 남아있는지 구분
            if (base.IsNext) 
            //if (cts_expcode != "")
            {
                //연속 데이타 정보를 호출.
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, cts_expcode);      // CTS종목번호 : 처음 조회시는 SPACE
                base.Request(true); //연속조회일경우 true
                //mainForm.input_t0424_log.Text = "[연속조회]잔고조회를 요청을 하였습니다.";
            } else {//마지막 데이타일때 메인폼에 출력해준다.
                //this.sunamt1 = int.Parse(this.GetFieldData("t0424OutBlock", "sunamt1", 0));// D1예수금
                this.dtsunik = int.Parse(this.GetFieldData("t0424OutBlock", "dtsunik", 0));// 실현손익

                this.mamt     = this.tmpMamt;      //매입금액
                this.tappamt  = this.tmpTappamt;   //평가금액
                this.tdtsunik = this.tmpTdtsunik;  //평가손익
           
                mainForm.input_mamt.Text     = Util.GetNumberFormat(this.mamt);    // 매입금액
                //mainForm.input_BalEvalAmt.Text  = Util.GetNumberFormat(this.tappamt); // 평가금액
                //mainForm.input_totalPamt.Text = Util.GetNumberFormat(this.tdtsunik);//평가금액 합
                //mainForm.input_D2Dps.Text  = Util.GetNumberFormat(this.sunamt1);      // D1예수금
                //mainForm.input_sunamt.Text   = Util.GetNumberFormat((this.sunamt1 + this.tappamt).ToString()); // 추정순자산 - sunamt 값이 이상해서  추정순자산 = 평가금액 + D1예수금 
                mainForm.input_dtsunik.Text = Util.GetNumberFormat(this.dtsunik);      // 실현손익
                mainForm.h_totalCount.Text   = this.h_totalCount.ToString();       //종목수


                //목록에 없는 종목 그리드에서 삭제.
                for (int i = 0; i < t0424VoList.Count; i++){
                    tmpT0424Vo = t0424VoList.ElementAt(i);

                    if (tmpT0424Vo.deleteAt == true){
                        Log.WriteLine("t0424 :: 팔린종목 그리드에서 제거");
                        t0424VoList.RemoveAt(i);
                        i--;
                    }
                    tmpT0424Vo.deleteAt = true;
                }

                //로그 및 중복 요청 처리
                mainForm.input_t0424_log2.Text = "[" + mainForm.input_time.Text + "]t0424 :: 잔고조회 완료";

                //응답처리 완료
                completeAt = true;
                readyAt = true;   
            }

        }//end

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {
            
            if (nMessageCode == "00000") {
                mainForm.setRowNumber(mainForm.grd_t0424);
            }else {
                //Log.WriteLine("[" + mainForm.input_time.Text + "]t0424 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t0424_log2.Text = "[" + mainForm.input_time.Text + "]t0424 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지
            }
        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(String account, String accountPw )
        {

            if (completeAt) {
                this.completeAt = false;//중복호출 방지
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("t0424InBlock", "accno" , 0, account);    // 계좌번호
                base.SetFieldData("t0424InBlock", "passwd", 0, accountPw);  // 비밀번호
                base.SetFieldData("t0424InBlock", "prcgb" , 0, "1");        // 단가구분 : 1-평균단가, 2:BEP단가
                base.SetFieldData("t0424InBlock", "chegb" , 0, "2");        // 체결구분 : 0-결제기준, 2-체결기준
                base.SetFieldData("t0424InBlock", "dangb" , 0, "0");        // 단일가구분 : 0-정규장, 1-시간외단일가 
                base.SetFieldData("t0424InBlock", "charge", 0, "1");        // 제비용포함여부 : 0-미포함, 1-포함
                base.SetFieldData("t0424InBlock", "cts_expcode", 0, "");      // CTS종목번호 : 처음 조회시는 SPACE

                // 계좌잔고 그리드 초기화
                //mainForm.grd_t0424.Rows.Clear();
                //mainForm.dataTable_t0424.Clear();

                //멤버변수 초기화
                this.tmpMamt      = 0; //매입금액
                this.tmpTappamt   = 0; //평가금액
                this.tmpTdtsunik  = 0; //평가손익
                this.h_totalCount = 0; //보유종목수
           
                base.Request(false);  //연속조회일경우 true
                
                //폼 메세지.
                mainForm.input_t0424_log.Text = "["+ mainForm.input_time.Text+ "]t0424::잔고조회";

            } else {
                mainForm.input_t0424_log.Text = "[" + mainForm.input_time.Text + "][중복]t0424::잔고조회";
            }


        }	// end function


    } //end class 

    public class T0424Vo
    {
        public String expcode   { set; get; } //종목코드
        public String hname     { set; get; } //종목명
        public String mdposqt   { set; get; } //매도가능
        public String price     { set; get; } //현재가
        public String appamt    { set; get; } //평가금액
        public String dtsunik   { set; get; } //평가손익
        public String sunikrt   { set; get; } //수익율
        public String pamt      { set; get; } //평균단가
        public String mamt      { set; get; } //매입금액
        public String msat      { set; get; } //당일매수금액
        public String mpms      { set; get; } //당일매수단가
        public String mdat      { set; get; } //당일매도금액
        public String mpmd      { set; get; } //당일매도단가
        public String fee       { set; get; } //수수료
        public String tax       { set; get; } //제세금
        public String sininter  { set; get; } //신용이자
        public Boolean deleteAt { set; get; } //삭제여부

    }

}   // end namespace
