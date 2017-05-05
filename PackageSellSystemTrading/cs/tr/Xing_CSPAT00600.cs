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


        private String shcode;        // 종목번호
        private String quantity;      // 주문수량
        private String price;         // 주문가
        private String ordptnDetail;  //상세주문구분
        private String upOrdno;       //상위매수주문 - 금일매도매수일때만 값이 있다.
        private String upExecprc;     //상위체결금액  
        private String sellOrdAt;   //매도주문여부
        private String hname;

        //상위체결가격

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

            DataLogVo dataLogVo = new DataLogVo();
            

            //데이타로그에 저장
            //public class DataLogVo
            dataLogVo.ordno        = OrdNo;          //주문번호
            dataLogVo.accno        = AcntNo;         //계좌번호
            dataLogVo.ordptncode   = "0" + BnsTpCode;//주문구분 01:매도|02:매수 
            dataLogVo.Isuno        = IsuNo.Replace("A","");  //종목코드
            dataLogVo.ordqty       = OrdQty;         //주문수량
            dataLogVo.execqty      = "0";            //체결수량
            dataLogVo.ordprc       = OrdPrc;         //주문가격
            dataLogVo.execprc      = "0";            //체결가격
            dataLogVo.Isunm        = this.hname;     //종목명
            dataLogVo.ordptnDetail = this.ordptnDetail;   //상세 주문구분 신규매수|반복매수|금일매도|청산
            dataLogVo.upExecprc    = this.upExecprc; //상위 체결가격
            dataLogVo.sellOrdAt    = sellOrdAt;      //매도 주문 여부
            dataLogVo.useYN        = "Y";            //사용여부
            //상위 주문번호
            if (this.upOrdno == ""){
                dataLogVo.upOrdno = OrdNo;               //상위 매수 주문번호 -01:금일매도일때 상위매수주문번호 그외에는 자신의 주문번호를 넣어준다.
            }else{
                dataLogVo.upOrdno = this.upOrdno;
            }
            //dataInsert호출
            mainForm.dataLog.insertData(dataLogVo);

            
        }

        //현물정상주문 메세지 처리
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            
            if (nMessageCode == "00000"){
                ;
            }else{
                Log.WriteLine("CSPAT00600::" + this.hname + "(" + this.shcode + ") "+ szMessage+"("+nMessageCode+ ")- [수량:" + this.quantity + "]");
                //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage;
                // 01222 :: 모의투자 매도잔고가 부족합니다  
                // 00040 :: 모의투자 매수주문 입력이 완료되었습니다.
                // 00039 :: 모의투자 매도주문 입력이 완료되었습니다. 
                // 01221 :: 모의투자 증거금부족으로 주문이 불가능합니다
                // 01219 :: 모의투자 매매금지 종목

                //정규매매장이 종료되었습니다.
                if (nMessageCode=="03563")
                {
                    //mainForm.marketAt = false;
                    //mainForm.stateCd = "마켓종료";
                }
               
                //거래정지 종목으로 주문이 불가능합니다.
                if (nMessageCode == "01069")
                {
                    //잔고그리드 종목찾아서 에러상태 로 만들어서 매도 주문이 안나가도록 조치 하자. 
                    EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
                    var result_t0424 = from item in t0424VoList
                                       where item.expcode == this.shcode.Replace("A","")
                                       select item;
                    //MessageBox.Show(result_t0424.Count().ToString());
                    if (result_t0424.Count() > 0)
                    {
                       result_t0424.ElementAt(0).errorcd = "01069";
                    }  
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

            base.SetFieldData("CSPAT00600Inblock1", "AcntNo"       ,0, mainForm.accountForm.account);       // 계좌번호
            base.SetFieldData("CSPAT00600Inblock1", "InptPwd"      ,0, mainForm.accountForm.accountPw);     // 입력비밀번호
            base.SetFieldData("CSPAT00600Inblock1", "IsuNo"        ,0, shcode);        // 종목번호
            base.SetFieldData("CSPAT00600Inblock1", "OrdQty"       ,0, quantity);      // 주문수량
            base.SetFieldData("CSPAT00600Inblock1", "OrdPrc"       ,0, price);         // 주문가
            base.SetFieldData("CSPAT00600Inblock1", "BnsTpCode"    ,0, divideBuySell); // 매매구분: 1-매도, 2-매수
            base.SetFieldData("CSPAT00600Inblock1", "OrdprcPtnCode",0, "00");          // 호가유형코드: 00-지정가, 05-조건부지정가, 06-최유리지정가, 07-최우선지정가
            base.SetFieldData("CSPAT00600Inblock1", "MgntrnCode"   ,0, "000");         // 신용거래코드: 000-보통
            base.SetFieldData("CSPAT00600Inblock1", "LoanDt"       ,0, "");            // 대출일 : 신용주문이 아닐 경우 SPACE
            base.SetFieldData("CSPAT00600Inblock1", "OrdCndiTpCode",0, "0");           // 주문조건구분 : 0-없음

            if (int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) > 900 && int.Parse(mainForm.xing_t0167.time.Substring(0, 4)) < 1530) 
            {
                if (mainForm.accountForm.accountPw == "" || mainForm.accountForm.account == "")
                {
                    MessageBox.Show("계좌 번호 및 비밀번호가 없습니다.");
                }
                else
                {           
                    base.Request(false);  //연속조회일경우 true
                }
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
            this.price        = price;       //주문가
            this.ordptnDetail = ordptnDetail;//상세주문구분
            this.upOrdno      = upOrdno;     //상위매수주문번호  
            this.upExecprc    = upExecprc;   //상위체결가격
            this.sellOrdAt    = "Y";         //매도주문여부
            this.call_request(shcode, quantity, price, "1");

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
           
            //1.모의투자 여부 구분하여 모의투자이면 A+종목번호
            if (mainForm.combox_targetServer.SelectedIndex == 0)
            {
                shcode = "A" + shcode;
            }
            this.shcode        = shcode;      // 종목번호
            this.hname         = hname;
            this.quantity      = quantity;    // 주문수량
            this.price         = price;       // 주문가
            this.ordptnDetail  = ordptnDetail;//상세주문구분
            this.upOrdno       = "";          //상위매수주문번호  
            this.upExecprc     = "0";         //상위체결가격
            this.sellOrdAt     = "N";
            this.call_request(shcode, quantity, price, "2");

        }	// end function


    } //end class 
   
}   // end namespace
