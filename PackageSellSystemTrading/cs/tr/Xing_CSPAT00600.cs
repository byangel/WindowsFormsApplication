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
    //현물 정상주문
    public class Xing_CSPAT00600 : XAQueryClass{

       
        public MainForm mainForm;

        //종목코드,수량,가격,주문구분|상세주문구분,상위매수주문번호(금일매도일때만),상위체결금액,매도주문여부
        public String shcode;        // 종목번호
        public String quantity;      // 주문수량
        public String price;         // 주문가
        public String ordptnDetail;  //상세주문구분
        public String upOrdno;       //상위매수주문 - 금일매도매수일때만 값이 있다.
        public String upExecprc;     //상위체결금액  
        public String sellOrdAt;   //매도주문여부
        public String hname;//종목명

        public String 종목코드;
        public String 수량;
        public String 가격;
        public String 주문구분;
        public String 상세주문구분;
        public String 상위매수주문번호;//(금일매도일때만)
        public String 상위체결금액;
        public String 매도주문여부;

        public Boolean completeAt = true;//완료여부.


        // 생성자
        public Xing_CSPAT00600() {
            base.ResFileName = "₩res₩CSPAT00600.res";

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_CSPAT00600()
        {
          
        }
        TradingHistoryVo dataLogVo = new TradingHistoryVo();
        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            String RecCnt     = base.GetFieldData("CSPAT00600OutBlock1", "RecCnt",    0);//레코드갯수
            String AcntNo     = base.GetFieldData("CSPAT00600OutBlock1", "AcntNo",    0);//계좌번호
            String IsuNo      = base.GetFieldData("CSPAT00600OutBlock1", "IsuNo",     0);//종목번호
            String OrdQty     = base.GetFieldData("CSPAT00600OutBlock1", "OrdQty",    0);//주문수량
            String OrdPrc     = base.GetFieldData("CSPAT00600OutBlock1", "OrdPrc",    0);//주문가격
            String BnsTpCode  = base.GetFieldData("CSPAT00600OutBlock1", "BnsTpCode", 0);//매매구분
            //Log.WriteLine("CSPAT00600 block1:: [레코드:"+ RecCnt + "|계좌번호:" + AcntNo + "|종목번호:" + IsuNo + "|주문수량:" + OrdQty + "| 주문가격:" + OrdPrc + " | 매매구분:" + BnsTpCode + "]");

            String RecCnt2    = base.GetFieldData("CSPAT00600OutBlock2", "RecCnt",     0);//레코드갯수
            String OrdNo      = base.GetFieldData("CSPAT00600OutBlock2", "OrdNo",      0);//주문번호 --block2에서는 주문번호만 참조하면 될듯.
            String OrdTime    = base.GetFieldData("CSPAT00600OutBlock2", "OrdTime",    0);//주문시각
            String OrdMktCode = base.GetFieldData("CSPAT00600OutBlock2", "OrdMktCode", 0);//주문시장코드
            String OrdPtnCode = base.GetFieldData("CSPAT00600OutBlock2", "OrdPtnCode", 0);//주문유형코드
            String ShtnIsuNo  = base.GetFieldData("CSPAT00600OutBlock2", "ShtnIsuNo",  0);//단축종목번호
            String MgempNo    = base.GetFieldData("CSPAT00600OutBlock2", "MgempNo",    0);//관리사원번호
            String OrdAmt     = base.GetFieldData("CSPAT00600OutBlock2", "OrdAmt",     0);//주문금액
            String SpotOrdQty = base.GetFieldData("CSPAT00600OutBlock1", "SpotOrdQty", 0);//실물주문수량  noData
            String MnyOrdAmt  = base.GetFieldData("CSPAT00600OutBlock1", "MnyOrdAmt",  0);//현금주문금액  noData
            String AcntNm     = base.GetFieldData("CSPAT00600OutBlock1", "AcntNm",     0);//계좌명
            String IsuNm      = base.GetFieldData("CSPAT00600OutBlock1", "IsuNm",      0);//종목명 -안넘어온다.
            //Log.WriteLine("CSPAT00600 block2:: [레코드:" + RecCnt2 + "|주문번호:" + OrdNo + "|단축종목번호:" + ShtnIsuNo + "|주문금액:" + OrdAmt + "|실물주문수량:" + SpotOrdQty + "|종목명:" + IsuNm + "]");

            //TradingHistoryVo dataLogVo = new TradingHistoryVo();
            //주문 에러가 났을때 주문번호는 0번이 넘어오는것같다.
            if (OrdNo != "0")
            {
                //데이타로그에 저장
                //public class dataLogVo
                dataLogVo.ordno         = OrdNo;                    //주문번호
                dataLogVo.accno         = AcntNo;                   //계좌번호
                dataLogVo.ordptncode    = "0" + BnsTpCode;          //주문구분 01:매도|02:매수 
                dataLogVo.Isuno         = IsuNo.Replace("A", "");  //종목코드
                dataLogVo.ordqty        = OrdQty;                   //주문수량
                dataLogVo.execqty       = "0";                      //체결수량 --막 주문을 넣었기때문에 체결수량은 0
                dataLogVo.ordprc        = OrdPrc;                   //주문가격
                dataLogVo.execprc       = "0";                      //체결가격
                dataLogVo.Isunm         = this.hname;               //종목명
                dataLogVo.ordptnDetail  = this.ordptnDetail;        //상세 주문구분 신규매수|반복매수|금일매도|청산
                dataLogVo.upExecprc     = this.upExecprc;           //상위 체결가격
                dataLogVo.sellOrdAt     = this.sellOrdAt;           //매도 주문 여부
                dataLogVo.useYN = "Y";                              //사용여부
                dataLogVo.ordermtd      = "XING API";           //주문 매체
                //상위 주문번호
                if (this.upOrdno == ""){
                    dataLogVo.upOrdno = OrdNo;//상위 매수 주문번호 -01:금일매도일때 상위매수주문번호 그외에는 자신의 주문번호를 넣어준다.
                }else{
                    dataLogVo.upOrdno = this.upOrdno;
                }
                //주문정보를 주문이력 DB에 저장 - dataInsert호출
                mainForm.tradingHistory.insert(dataLogVo);
                mainForm.tradingHistory.getTradingHistoryVoList().Add(dataLogVo);
            }

            completeAt = true;
            
        }

        //현물정상주문 메세지 처리
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            
            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("CSPAT00600::" + this.hname + "(" + this.shcode + ") "+ szMessage+"("+nMessageCode+ ")- [수량:" + this.quantity + "]");
                //에러 리턴 받았을때
                this.completeAt = true;

                if (upOrdno!="")
                {
                    //주문이 잘못되었을경우 매도여부를 초기화 해준다.
                    int findIndex = mainForm.xing_t0425.getT0425VoList().Find("ordno", this.upOrdno);//upOrdno
                    if (findIndex >= 0)
                    {
                        mainForm.grd_t0425.Rows[findIndex].Cells["sellOrdAt"].Value = ""; //종목코드


                        //상위주문번호 주문여부 Y로 업데이트
                        var items = from item in mainForm.tradingHistory.getTradingHistoryVoList()
                                    where item.ordno == this.upOrdno
                                        && item.accno == mainForm.account
                                    select item;

                        foreach (TradingHistoryVo item in items)
                        {
                            item.sellOrdAt = "";
                            mainForm.tradingHistory.update(item);//매도주문 여부 상태 업데이트
                        }
                    }

                }






                //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                // 01222 :: 모의투자 매도잔고가 부족합니다  
                // 00040 :: 모의투자 매수주문 입력이 완료되었습니다.
                // 00039 :: 모의투자 매도주문 입력이 완료되었습니다. 
                // 01221 :: 모의투자 증거금부족으로 주문이 불가능합니다
                // 01219 :: 모의투자 매매금지 종목
                // 02705 :: 모의투자 주문가격을 잘못 입력하셨습니다.   


                //정규매매장이 종료되었습니다.
                if (nMessageCode=="03563")
                {
                    ;
                }
                if (nMessageCode == "02705")
                {
                    MessageBox.Show("600:: 모의투자 주문가격을 잘못 입력하셨습니다." + price);
                }

                //거래정지 종목으로 주문이 불가능합니다.
                if (nMessageCode == "01069")
                {
                    //잔고그리드 종목찾아서 에러상태 로 만들어서 매도 주문이 안나가도록 조치 하자. 
                    EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
                    int findIndex = t0424VoList.Find("expcode", this.shcode.Replace("A", ""));
                    if (findIndex >= 0 )
                    {
                       mainForm.grd_t0424.Rows[findIndex].Cells["errorcd"].Value = "01069"; //에러코드
                    }
                    //var result_t0424 = from item in t0424VoList
                    //                   where item.expcode == this.shcode.Replace("A","")
                    //                   select item;
                    ////MessageBox.Show(result_t0424.Count().ToString());
                    //if (result_t0424.Count() > 0)
                    //{
                    //   result_t0424.ElementAt(0).errorcd = "01069";
                    //}  
                }
                

            }
           
        }
        
        /// <summary>
        /// 현물정상주문
        /// </summary>
        /// <param name="IsuNo">종목번호</param>
        /// <param name="Quantity">수량</param>
        /// <param name="Price">가격</param>
        /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
        public void call_request(String shcode, String quantity, String price, String divideBuySell){

            base.SetFieldData("CSPAT00600Inblock1", "AcntNo"       ,0, mainForm.account);       // 계좌번호
            base.SetFieldData("CSPAT00600Inblock1", "InptPwd"      ,0, mainForm.accountPw);     // 입력비밀번호
            base.SetFieldData("CSPAT00600Inblock1", "IsuNo"        ,0, shcode);        // 종목번호
            base.SetFieldData("CSPAT00600Inblock1", "OrdQty"       ,0, quantity);      // 주문수량
            base.SetFieldData("CSPAT00600Inblock1", "OrdPrc"       ,0, price.Replace(",",""));         // 주문가
            base.SetFieldData("CSPAT00600Inblock1", "BnsTpCode"    ,0, divideBuySell); // 매매구분: 1-매도, 2-매수
            base.SetFieldData("CSPAT00600Inblock1", "OrdprcPtnCode",0, "00");          // 호가유형코드: 00-지정가, 05-조건부지정가, 06-최유리지정가, 07-최우선지정가
            base.SetFieldData("CSPAT00600Inblock1", "MgntrnCode"   ,0, "000");         // 신용거래코드: 000-보통
            base.SetFieldData("CSPAT00600Inblock1", "LoanDt"       ,0, "");            // 대출일 : 신용주문이 아닐 경우 SPACE
            base.SetFieldData("CSPAT00600Inblock1", "OrdCndiTpCode",0, "0");           // 주문조건구분 : 0-없음

            
            if (mainForm.accountPw == "" || mainForm.account == "")
            {
                MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
            }
            else
            {           
                base.Request(false);  //연속조회일경우 true
                this.completeAt = false;
            }
            
        }   // end function


        
        /// <summary>
        /// 현물정상주문
        /// </summary>
        /// <param name="ordptnDetail">상세주문구분-신규매수|반복매수|금일매도|청산</param>
        /// <param name="upOrdno">상위매수주문번호-금일매도일때만 셋팅될것같다.</param>
        /// <param name="upExecprc">상위체결금액</param>
        /// <param name="hname">종목명</param>
        /// <param name="IsuNo">종목코드</param>
        /// <param name="Quantity">수량</param>
        /// <param name="Price">가격</param>
        public void call_requestSell(String ordptnDetail, String upOrdno, String upExecprc, String hname, String shcode, String quantity, String price)
        {
            //변수초기화 여기는 굳이 안해줘도 될듯.

            //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + shcode;
            }
            this.shcode       = shcode;      //종목번호
            this.hname        = hname;       //종목명
            this.quantity     = quantity;    //주문수량
            this.price        = price.Replace(",", "");       //주문가
            this.ordptnDetail = ordptnDetail;//상세주문구분
            this.upOrdno      = upOrdno;     //상위매수주문번호  
            this.upExecprc    = upExecprc.Replace(",", "");   //상위체결가격
            this.sellOrdAt    = "";         //매도주문여부-상위매도주문에 설정해야한다.
            this.call_request(shcode, quantity, this.price, "1");

        }   // end function

        
        /// <summary>
        /// 현물정상주문
        /// </summary>
        /// <param name="ordptnDetail">상세주문구분 신규매수|반복매수|금일매도|청산</param>
        /// <param name="IsuNo">종목번호</param>
        /// <param name="Quantity">수량</param>
        /// <param name="Price">가격</param>
        public void call_requestBuy(String ordptnDetail,  String shcode,String hname, String quantity, String price)
        {
            //변수초기화
            //this.shcode       = "";        // 종목번호
            //this.quantity     = "";      // 주문수량
            //this.price        = "";         // 주문가
            //this.ordptnDetail = "";  //상세주문구분
            //this.upOrdno      = "";       //상위매수주문 - 금일매도매수일때만 값이 있다.
            //this.upExecprc    = "";     //상위체결금액  
            //this.hname        = "";
            //this.sellOrdAt    = "";
            //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + shcode;
            }
            this.shcode        = shcode;      // 종목번호
            this.hname         = hname;
            this.quantity      = quantity;    // 주문수량
            this.price         = price.Replace(",", "");       // 주문가
            this.ordptnDetail  = ordptnDetail;//상세주문구분
            this.upOrdno       = "";          //상위매수주문번호  
            this.upExecprc     = "0";         //상위체결가격
            this.sellOrdAt     = "N";
            this.call_request(shcode, quantity, this.price, "2");

        }	// end function


    } //end class 
   
}   // end namespace
