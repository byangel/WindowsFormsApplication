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
using System.Drawing;

namespace PackageSellSystemTrading{
 //현물 정상주문
    public class Xing_CSPAT00600 : XAQueryClass{

       
        public MainForm mainForm;

     //종목코드,수량,가격,주문구분|상세주문구분,상위매수주문번호(금일매도일때만),상위체결금액,매도주문여부
        public String shcode;           // 종목번호
        public String price;            // 가격
        public String quantity;         // 주문수량
        public String BnsTpCode;    // 매매구분: 1-매도, 2-매수
        public String hname         = "";  //종목명
        public String ordptnDetail  = "";  //상세주문구분
        public String upOrdno       = "";  //상위매수주문 - 금일매도매수일때만 값이 있다.
        public String upExecprc     = "";  //상위체결금액  
        public String searchNm   = "";  //검색조건 이름

        public Boolean completeAt;//완료여부.


     // 생성자
        public Xing_CSPAT00600(MainForm mainForm) {
            base.ResFileName = "₩res₩CSPAT00600.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            completeAt = false;
            this.mainForm = mainForm;
        }// end function

        
        TradingHistoryVo dataLogVo = new TradingHistoryVo();
     /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            try
            {
                String RecCnt       = base.GetFieldData("CSPAT00600OutBlock1"     , "RecCnt", 0);//레코드갯수
                String AcntNo       = base.GetFieldData("CSPAT00600OutBlock1"     , "AcntNo", 0);//계좌번호
                String IsuNo        = base.GetFieldData("CSPAT00600OutBlock1"      , "IsuNo", 0);//종목번호
                String OrdQty       = base.GetFieldData("CSPAT00600OutBlock1"     , "OrdQty", 0);//주문수량
                String OrdPrc       = base.GetFieldData("CSPAT00600OutBlock1"     , "OrdPrc", 0);//주문가격
                String BnsTpCode    = base.GetFieldData("CSPAT00600OutBlock1"  , "BnsTpCode", 0);//매매구분
                                                                                         //Log.WriteLine("CSPAT00600 block1:: [레코드:"+ RecCnt + "|계좌번호:" + AcntNo + "|종목번호:" + IsuNo + "|주문수량:" + OrdQty + "| 주문가격:" + OrdPrc + " | 매매구분:" + BnsTpCode + "]");

                String RecCnt2      = base.GetFieldData("CSPAT00600OutBlock2"    , "RecCnt", 0);//레코드갯수
                String OrdNo        = base.GetFieldData("CSPAT00600OutBlock2"      , "OrdNo", 0);//주문번호 --block2에서는 주문번호만 참조하면 될듯.
                String OrdTime      = base.GetFieldData("CSPAT00600OutBlock2"    , "OrdTime", 0);//주문시각
                String OrdMktCode   = base.GetFieldData("CSPAT00600OutBlock2" , "OrdMktCode", 0);//주문시장코드
                String OrdPtnCode   = base.GetFieldData("CSPAT00600OutBlock2" , "OrdPtnCode", 0);//주문유형코드
                String ShtnIsuNo    = base.GetFieldData("CSPAT00600OutBlock2"  , "ShtnIsuNo", 0);//단축종목번호
                String MgempNo      = base.GetFieldData("CSPAT00600OutBlock2"    , "MgempNo", 0);//관리사원번호
                String OrdAmt       = base.GetFieldData("CSPAT00600OutBlock2"     , "OrdAmt", 0);//주문금액
                String SpotOrdQty   = base.GetFieldData("CSPAT00600OutBlock1" , "SpotOrdQty", 0);//실물주문수량  noData
                String MnyOrdAmt    = base.GetFieldData("CSPAT00600OutBlock1"  , "MnyOrdAmt", 0);//현금주문금액  noData
                String AcntNm       = base.GetFieldData("CSPAT00600OutBlock1"     , "AcntNm", 0);//계좌명
                String IsuNm        = base.GetFieldData("CSPAT00600OutBlock1"      , "IsuNm", 0);//종목명 -안넘어온다.
                                                                                 //Log.WriteLine("CSPAT00600 block2:: [레코드:" + RecCnt2 + "|주문번호:" + OrdNo + "|단축종목번호:" + ShtnIsuNo + "|주문금액:" + OrdAmt + "|실물주문수량:" + SpotOrdQty + "|종목명:" + IsuNm + "]");

             //TradingHistoryVo dataLogVo = new TradingHistoryVo();
             //주문 에러가 났을때 주문번호는 0번이 넘어오는것같다.
                if (OrdNo != "0")
                {
                 //데이타로그에 저장
                 //public class dataLogVo
                    dataLogVo.ordno         = OrdNo;                 //주문번호
                    dataLogVo.accno         = AcntNo;                //계좌번호
                    dataLogVo.ordptncode    = "0" + BnsTpCode;       //주문구분 01:매도|02:매수 
                    dataLogVo.Isuno         = IsuNo.Replace("A", "");//종목코드
                    dataLogVo.ordqty        = OrdQty;                //주문수량
                    dataLogVo.execqty       = "0";                   //체결수량 --막 주문을 넣었기때문에 체결수량은 0
                    dataLogVo.ordprc        = OrdPrc.Replace(",", "");  //주문가격
                    dataLogVo.execprc       = "0";                   //체결가격
                    dataLogVo.Isunm         = this.hname;            //종목명
                    dataLogVo.ordptnDetail  = this.ordptnDetail;     //상세 주문구분 신규매수|반복매수|금일매도|청산
                    dataLogVo.upExecprc     = this.upExecprc == "" ? "0" : this.upExecprc.Replace(",", "");//상위 체결가격
                    dataLogVo.sellOrdAt     = "N";                   //금일 매도 주문 여부
                    dataLogVo.useYN         = "Y";                   //사용여부
                    dataLogVo.ordermtd      = "XING API";            //주문 매체
                    dataLogVo.searchNm      = this.searchNm;         //검색조건 이름
                                                           //상위 주문번호
                    if (this.upOrdno == "")
                    {
                        dataLogVo.upOrdno = OrdNo;//상위 매수 주문번호 -01:금일매도일때 상위매수주문번호 그외에는 자신의 주문번호를 넣어준다.
                    }
                    else
                    {
                        dataLogVo.upOrdno = this.upOrdno;
                    }
                 //주문정보를 주문이력 DB에 저장 - dataInsert호출
                    mainForm.tradingHistory.insert(dataLogVo);
                 //mainForm.tradingHistory.getTradingHistoryVoList().Add(dataLogVo);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("t0424 : " + ex.Message);
                Log.WriteLine("t0424 : " + ex.StackTrace);
            }


        }

     //현물정상주문 메세지 처리
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {

            if (nMessageCode == "00000" || nMessageCode == "00040" || nMessageCode == "00039")
            {
                ;
            }
            else
            {
                String msg = "CSPAT00600::" + this.hname + "(" + this.shcode + ") " + szMessage + "(" + nMessageCode + ")- [수량:" + this.quantity + "]";

                Log.WriteLine(msg);
             //에러 리턴 받았을때 매수 일때와 매도일때 구분해서 구현하자.

             //매수일때는 에러코드를 출력할곳이 없으므로 안내창을 호출 해준다.
                if (this.BnsTpCode == "1")
                {
                 //금일매도매수일때만 값이 있다.--이게 필요한지 모르겠다...매도취소일때만 매도주문 초기화를 해줘도 될듯한데...
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
                       

                    }


                 //0424 주문안한 상탱로 초기화
                    int findIndexT0424 = mainForm.xing_t0424.getT0424VoList().Find("expcode", this.shcode.Replace("A", ""));
                 //주문여부를 다시 N으로 설정한다.
                    if (findIndexT0424 >= 0)
                    {
                        mainForm.xing_t0424.getT0424VoList().ElementAt(findIndexT0424).orderAt = "N";//주문상태 초기화
                        mainForm.grd_t0424.Rows[findIndexT0424].Cells["errorcd"].Value = szMessage; //에러코드
                        
                        if (nMessageCode.Equals("01219")){
                            mainForm.xing_t0424.getT0424VoList().ElementAt(findIndexT0424).orderAt = "N";//주문상태 초기화
                            mainForm.grd_t0424.Rows[findIndexT0424].Cells["errorcd"].Value = "매매금지"; //에러코드
                        }
                        mainForm.grd_t0424.Rows[findIndexT0424].DefaultCellStyle.BackColor = Color.Red;
                    }

                 //t0424Vo.orderAt
                 //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                 // 01222 :: 모의투자 매도잔고가 부족합니다  
                 // 00040 :: 모의투자 매수주문 입력이 완료되었습니다.
                 // 00039 :: 모의투자 매도주문 입력이 완료되었습니다. 
                 // 01221 :: 모의투자 증거금부족으로 주문이 불가능합니다
                 // 01219 :: 모의투자 매매금지 종목
                 // 02705 :: 모의투자 주문가격을 잘못 입력하셨습니다.  
                    


                } else {
                    MessageBox.Show(msg);
                }//receiveMessageEventHandler END
            }
            completeAt = true;
        }
        
     /// <summary>
     /// 현물정상주문
     /// </summary>
     /// <param name="IsuNo">종목번호</param>
     /// <param name="Quantity">수량</param>
     /// <param name="Price">가격</param>
     /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
        //public void call_request(){

        //    this.shcode = this.shcode.Replace("A","");
        //    if (mainForm.combox_targetServer.SelectedIndex == 0){
        //                shcode = "A" + this.shcode;
        //    }
        //    base.SetFieldData("CSPAT00600Inblock1", "AcntNo"       ,0, mainForm.account);        // 계좌번호
        //    base.SetFieldData("CSPAT00600Inblock1", "InptPwd"      ,0, mainForm.accountPw);      // 입력비밀번호
        //    base.SetFieldData("CSPAT00600Inblock1", "IsuNo"        ,0, this.shcode);             // 종목번호
        //    base.SetFieldData("CSPAT00600Inblock1", "OrdQty"       ,0, this.quantity);           // 주문수량
        //    base.SetFieldData("CSPAT00600Inblock1", "OrdPrc"       ,0, this.price.Replace(",","")); // 가격
        //    base.SetFieldData("CSPAT00600Inblock1", "BnsTpCode"    ,0, this.BnsTpCode);      // 매매구분: 1-매도, 2-매수
        //    base.SetFieldData("CSPAT00600Inblock1", "OrdprcPtnCode",0, "00");                    // 호가유형코드: 00-지정가, 05-조건부지정가, 06-최유리지정가, 07-최우선지정가
        //    base.SetFieldData("CSPAT00600Inblock1", "MgntrnCode"   ,0, "000");                   // 신용거래코드: 000-보통
        //    base.SetFieldData("CSPAT00600Inblock1", "LoanDt"       ,0, "");                      // 대출일 : 신용주문이 아닐 경우 SPACE
        //    base.SetFieldData("CSPAT00600Inblock1", "OrdCndiTpCode",0, "0");                     // 주문조건구분 : 0-없음
            
        //    if (mainForm.accountPw == "" || mainForm.account == "")
        //    {
        //        MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
        //    }else{           
        //        base.Request(false);  //연속조회일경우 true
        //     //this.completeAt = false;
        //    }
            
        //}// end function

        public void call_request(String 종목명, String 종목코드, String 매매구분, Double 수량, Double 현재가, String 매수전량명, Double 평균단가, String 상세매매구분)
        {
            Double 호가 = 현재가;
            if (매매구분.Equals("매도"))
            {
                매매구분 = "1";
                if (!Properties.Settings.Default.SELL_HO.Equals("시장가"))
                {
                    호가 = Util.getTickPrice(현재가, Double.Parse(Properties.Settings.Default.SELL_HO));
                }
            }
            if (매매구분.Equals("매수"))
            {
                매매구분 = "2";
                if (!Properties.Settings.Default.BUY_HO.Equals("시장가"))
                {
                    호가 = Util.getTickPrice(현재가, Double.Parse(Properties.Settings.Default.BUY_HO));
                }
                수량 = (int.Parse(Properties.Settings.Default.ADD_BUY_AMT) * 10000) / int.Parse(호가.ToString());
            }


            //상세주문구분|상위매수주문번호|상위체결금액|종목명|종목코드|수량|가격 -신규매수|반복매수|금일매도|청산|목표청산
            //Xing_CSPAT00600 xing_CSPAT00600 = mainForm.CSPAT00600Mng.get600();
            this.ordptnDetail   = 상세매매구분;         //상세 매매 구분.
            this.shcode         = 종목코드;      //종목코드
            this.hname          = 종목명;        //종목명
            this.quantity       = 수량.ToString(); //수량
            //xing_CSPAT00600.price      = t0424Vo.price.Replace(",", "");   //가격
            this.price          = 호가.ToString();   //가격
            this.BnsTpCode      = 매매구분;             // 매매구분: 1-매도, 2-매수
            this.upOrdno        = "";              //상위매수주문번호 - 금일매도매수일때만 값이 있다.
            this.upExecprc      = 평균단가.ToString();    //매도이면 매수단가 주입, 매수
            this.searchNm       = 매수전량명; //매수전략명

            this.shcode = this.shcode.Replace("A", "");
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + this.shcode;
            }
            base.SetFieldData("CSPAT00600Inblock1", "AcntNo", 0, mainForm.account);        // 계좌번호
            base.SetFieldData("CSPAT00600Inblock1", "InptPwd", 0, mainForm.accountPw);      // 입력비밀번호
            base.SetFieldData("CSPAT00600Inblock1", "IsuNo", 0, this.shcode);             // 종목번호
            base.SetFieldData("CSPAT00600Inblock1", "OrdQty", 0, this.quantity);           // 주문수량
            base.SetFieldData("CSPAT00600Inblock1", "OrdPrc", 0, this.price.Replace(",", "")); // 가격
            base.SetFieldData("CSPAT00600Inblock1", "BnsTpCode", 0, this.BnsTpCode);      // 매매구분: 1-매도, 2-매수
            base.SetFieldData("CSPAT00600Inblock1", "OrdprcPtnCode", 0, "00");                    // 호가유형코드: 00-지정가, 05-조건부지정가, 06-최유리지정가, 07-최우선지정가
            base.SetFieldData("CSPAT00600Inblock1", "MgntrnCode", 0, "000");                   // 신용거래코드: 000-보통
            base.SetFieldData("CSPAT00600Inblock1", "LoanDt", 0, "");                      // 대출일 : 신용주문이 아닐 경우 SPACE
            base.SetFieldData("CSPAT00600Inblock1", "OrdCndiTpCode", 0, "0");                     // 주문조건구분 : 0-없음

            if (mainForm.accountPw == "" || mainForm.account == "")
            {
                MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
            }
            else
            {
                base.Request(false);  //연속조회일경우 true
                                      //this.completeAt = false;
            }

        }


    } //end class 
   
}// end namespace
