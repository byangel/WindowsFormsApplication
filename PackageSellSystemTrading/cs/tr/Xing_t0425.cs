
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
    //주식 체결/미체결
    public class Xing_t0425 : XAQueryClass {

        private EBindingList<T0425Vo> t0425VoList;
        public EBindingList<T0425Vo> getT0425VoList()
        {
            return this.t0425VoList;
        }

        public Boolean completeAt = true;//완료여부.
        public MainForm mainForm;


       

        public int totalCount;

        public Boolean readyAt;
        // 생성자
        public Xing_t0425()
        {
            readyAt = false;

            base.ResFileName = "₩res₩t0425.res";


            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            this.t0425VoList = new EBindingList<T0425Vo>();

        }   // end function

        // 소멸자
        ~Xing_t0425()
        {

        }

        


        /// <summary>
		/// 주식 체결/미체결(T0425) 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode) {

            //응답 데이타 수
            int blockCount = base.GetBlockCount("t0425OutBlock1");


            

           

            //계좌잔고목록
            EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();

          

            String ordno;  //주문번호
            String status; //상태 체결|미체결
            String qty;    //주문수량
            String cheqty; //체결수량
            T0425Vo tmpT0425Vo;
            for (int i = 0; i < blockCount; i++)
            {
                ordno  = base.GetFieldData("t0425OutBlock1", "ordno" , i); //주문번호
                status = base.GetFieldData("t0425OutBlock1", "status", i); //상태
                qty    = base.GetFieldData("t0425OutBlock1", "qty"   , i); //주문수량
                cheqty = base.GetFieldData("t0425OutBlock1", "cheqty", i); //체결수량
               
                int findIndex = t0425VoList.Find("ordno", ordno);
                if (findIndex >= 0){

                    tmpT0425Vo = this.t0425VoList.ElementAt(findIndex);

                    mainForm.grd_t0425.Rows[findIndex].Cells["ordtime"].Value     = base.GetFieldData("t0425OutBlock1", "ordtime", i); //주문시간
                    mainForm.grd_t0425.Rows[findIndex].Cells["medosu"].Value      = base.GetFieldData("t0425OutBlock1", "medosu"  , i); //매매구분 - 0:전체|1:매수|2:매도
                    mainForm.grd_t0425.Rows[findIndex].Cells["expcode"].Value     = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                    //mainForm.grd_t0425.Rows[findIndex].Cells["t0425_hname"].Value       = ""; //종목명
                    mainForm.grd_t0425.Rows[findIndex].Cells["qty"].Value         = base.GetFieldData("t0425OutBlock1", "qty"     , i); //주문수량
                    mainForm.grd_t0425.Rows[findIndex].Cells["t0425_price"].Value = base.GetFieldData("t0425OutBlock1", "price"   , i); //주문가격
                    mainForm.grd_t0425.Rows[findIndex].Cells["cheqty"].Value      = base.GetFieldData("t0425OutBlock1", "cheqty"  , i); //체결수량
                    mainForm.grd_t0425.Rows[findIndex].Cells["cheprice"].Value    = base.GetFieldData("t0425OutBlock1", "cheprice", i); //체결가격
                    mainForm.grd_t0425.Rows[findIndex].Cells["ordrem"].Value      = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결잔량
                    mainForm.grd_t0425.Rows[findIndex].Cells["status"].Value      = base.GetFieldData("t0425OutBlock1", "status"  , i); //상태
                    mainForm.grd_t0425.Rows[findIndex].Cells["ordno"].Value       = base.GetFieldData("t0425OutBlock1", "ordno"   , i); //주문번호

                } else {
                    tmpT0425Vo = new T0425Vo();
                    tmpT0425Vo.ordtime  = base.GetFieldData("t0425OutBlock1", "ordtime" , i); //주문시간
                    tmpT0425Vo.medosu   = base.GetFieldData("t0425OutBlock1", "medosu"  , i); //매매구분 - 0:전체|1:매수|2:매도
                    tmpT0425Vo.expcode  = base.GetFieldData("t0425OutBlock1", "expcode" , i); //종목번호
                    //tmpT0425Vo.hname    = ""; //종목명
                    tmpT0425Vo.qty      = base.GetFieldData("t0425OutBlock1", "qty"     , i); //주문수량
                    tmpT0425Vo.price    = base.GetFieldData("t0425OutBlock1", "price"   , i); //주문가격
                    tmpT0425Vo.cheqty   = base.GetFieldData("t0425OutBlock1", "cheqty"  , i); //체결수량
                    tmpT0425Vo.cheprice = base.GetFieldData("t0425OutBlock1", "cheprice", i); //체결가격
                    tmpT0425Vo.ordrem   = base.GetFieldData("t0425OutBlock1", "ordrem"  , i); //미체결잔량
                    tmpT0425Vo.status   = base.GetFieldData("t0425OutBlock1", "status"  , i); //상태
                    tmpT0425Vo.ordno    = base.GetFieldData("t0425OutBlock1", "ordno"   , i); //주문번호

                    //목록추가
                    this.t0425VoList.Add(tmpT0425Vo);
                }

                //DataLog 정보 참조 - 매매 상세 구분
                int dataLogIndex = mainForm.dataLog.getDataLogVoList().Find("ordno", ordno);
                if (dataLogIndex >= 0)
                {
                    DataLogVo dataLogVo     = mainForm.dataLog.getDataLogVoList().ElementAt(dataLogIndex);
                    tmpT0425Vo.ordptnDetail = dataLogVo.ordptnDetail;//매매상세구분
                    tmpT0425Vo.hname        = dataLogVo.Isunm;       //종목명
                    tmpT0425Vo.sellOrdAt    = dataLogVo.sellOrdAt;   //금일매도여부
                    tmpT0425Vo.useYN        = dataLogVo.useYN;       //사용여부
                    tmpT0425Vo.upOrdno      = dataLogVo.upOrdno;     //상위 매수 주문번호
                    tmpT0425Vo.upExecprc    = dataLogVo.upExecprc;   //상위체결금액
               
                 
                }

            }//for end

            String cts_ordno = base.GetFieldData("t0425OutBlock", "cts_ordno", 0);//연속키
            //2.연속 데이타 정보가 남아있는지 구분
            //if (base.IsNext)
            if (cts_ordno != "")
            {
                //연속 데이타 정보를 호출.
                base.SetFieldData("t0425InBlock", "cts_ordno", 0, cts_ordno);      //처음 조회시는 SPACE
                base.Request(true); //연속조회일경우 true
                //mainForm.input_t0424_log.Text = "[연속조회]잔고조회를 요청을 하였습니다.";
            } else {//마지막 데이타일때 메인폼에 출력해준다.
                completeAt = true;
                readyAt = true;
                //매수체결 목록
                mainForm.grd_t0425_chegb1_cnt.Text = this.t0425VoList.Count().ToString();
                //Thread.Sleep(5000);
                //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
                mainForm.input_t0425_log2.Text = "[" + mainForm.label_time.Text + "]t0425 :: 채결/미채결 요청완료";
                
            }


        }//end


        //미체결 주문취소
        public void orderCancle()
        {
            //현재시간.
            String time = mainForm.xing_t0167.time;
            if (time == "" || time == null) { time = "1530"; }//에러 안나게 기본값을 셋팅해준다.
            int cTime = (int.Parse(time.Substring(0, 2)) * 60) + int.Parse(time.Substring(2, 2));//현재 시간

            var varT0425VoList = from item in this.t0425VoList
                                   where item.status == "미체결"
                                   select item;
            for (int i=0; i < varT0425VoList.Count(); i++)
            {
                
                    //미체결 시간이 1분 이상이면 취소주문 한다.
                    int tmpTime = (int.Parse(varT0425VoList.ElementAt(i).ordtime.Substring(0, 2)) * 60) + int.Parse(varT0425VoList.ElementAt(i).ordtime.Substring(2, 2));//현재 시간
                    if ((cTime - tmpTime) > 1)
                    {
                        /// <summary>
                        /// 현물 취소 주문
                        /// </summary>
                        /// <param name="OrgOrdNo">원주문번호</param>
                        /// <param name="IsuNo">종목번호</param>
                        /// <param name="OrdQty">주문수량</param>
                        mainForm.xing_CSPAT00800.call_request(mainForm.accountForm.account, mainForm.accountForm.accountPw, varT0425VoList.ElementAt(i).ordno, varT0425VoList.ElementAt(i).expcode, "");
                        Log.WriteLine("t0425::" + varT0425VoList.ElementAt(i).hname + "(" + varT0425VoList.ElementAt(i).expcode + ")::취소주문 [주문번호:" + varT0425VoList.ElementAt(i).ordno + "]");

                    //1.최소하면 dataLog 삭제 -- 매수인지 매도인지 잘모름...
                    //매수 미체결 - public String medosu   { set; get; } //매매구분 - 0:전체|1:매수|2:매도
                    if (varT0425VoList.ElementAt(i).medosu == "1")
                    {
                        //반복매수인지 신규매수인지...
                    }
                    //매도 미체결
                    if (varT0425VoList.ElementAt(i).medosu == "2")
                    {
                        mainForm.dataLog.updateSellOrdAt(varT0425VoList.ElementAt(i).ordno, "N");
                    }

                    
                }
            }
        }

        //금일매도 - 반복매수 중에서 상승한종목을 매도한다.
        public void toDaySellTest()
        {
           
            //금일 매수 and 매도여부:N and 반복매수 and 주문수량=체결수량
            var varT0425VoList = from   item in this.t0425VoList
                                 where  item.sellOrdAt == "N" && item.ordptnDetail == "반복매수" && item.qty == item.cheqty
                                 select item;
            int findIndex;
            Double price0424;//현재가
            Double execprc;//금일체결가격
            int T0425FindIndex;
            String ordno;//주문번호
            String expcode;//종목코드
            String hname;//종목명
            String cheqty;//체결수량
            String cheprice;//체결가격
            //MessageBox.Show(varT0425VoList.Count().ToString()+"/"+ this.t0425VoList.ElementAt(0).medosu);
            for (int i = 0; i < varT0425VoList.Count(); i++)
            {
                ordno    = varT0425VoList.ElementAt(i).ordno;
                expcode  = varT0425VoList.ElementAt(i).expcode;
                hname    = varT0425VoList.ElementAt(i).hname;
                cheqty   = varT0425VoList.ElementAt(i).cheqty;
                cheprice = varT0425VoList.ElementAt(i).cheprice;

                //계좌잔고 그리드에서 해당종목 정보 참조.
                int t0424Index = mainForm.xing_t0424.getT0424VoList().Find("expcode", expcode);
                if (t0424Index >= 0)
                {
                    price0424 = Double.Parse(mainForm.xing_t0424.getT0424VoList().ElementAt(t0424Index).price);//현재가
                    execprc   = Double.Parse(varT0425VoList.ElementAt(i).cheprice);//금일체결가격
                    //1.현재가가 금일매수 값보다 3%이상 올랐으면 금일 매수 수량만큼 매도한다.
                    Double late = ((price0424 / execprc) * 100) - 100;
                    late = Math.Round(late, 2);

                    //금일수익률 그리드에 출력
                    T0425FindIndex = this.t0425VoList.Find("ordno", ordno);
                    mainForm.grd_t0425.Rows[T0425FindIndex].Cells["toDaysunikrt"].Value = late; //금일수익률

                    //금일 매수가격이 3%이상일때 매도해준다.
                    if (late >= double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET))
                    {
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
                        mainForm.xing_CSPAT00600.call_requestSell("금일매도", ordno, cheprice, hname, expcode, cheqty, price0424.ToString());

                        //매수건에대해서 금일매도를 해주었으므로 매수건에 매도여부를 Y로 업데이트 해준다.
                        findIndex = this.t0425VoList.Find("ordno", varT0425VoList.ElementAt(i).ordno);
                        mainForm.grd_t0425.Rows[findIndex].Cells["sellOrdAt"].Value = "Y"; //종목코드
                        //dataLog 업데이트
                        mainForm.dataLog.updateSellOrdAt(ordno, "Y");
                       

                        Log.WriteLine("t0425::" + hname + "(" + expcode + ")::금일 매도 [주문가격:" + price0424.ToString() + "|주문수량:" + cheqty + "|금일수익율:" + late +  " | 주문번호:" + ordno + "]");

                    }
                }
            }
        }//금일매도매수 end

     

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage) {

            if (nMessageCode == "00000") {
                
            //mainForm.setRowNumber(mainForm.grd_t0425_chegb1);
            } else {
                //Thread.Sleep(3000);
                completeAt = true;//중복호출 방지
               
                //-2 :: 서버접속에 실패하였습니다.
                if (nMessageCode == "   -2")
                {
                    mainForm.login();
                    Log.WriteLine("t0425 :: 로그인 호출");
                }
                //서버접속 실패로인하여 로그인 여부를 false 로 설정한다.후에 접속실패 코드확보후 조건문 추가해주자.
                //mainForm.exXASessionClass.loginAt = false;
                //00007 :: 시스템 사정으로 자료 서비스를 받을 수 없습니다.
                //00008 :: 시스템 문제로 서비스가 불가능 합니다.
            }

        }

        /// <summary>
		/// 체결/미체결 요청
		/// </summary>
		public void call_request(String account, String accountPw)
        {

            if (completeAt) {
                completeAt = false;//중복호출 방지

                base.SetFieldData("t0425InBlock", "accno"    , 0, account);    // 계좌번호
                base.SetFieldData("t0425InBlock", "passwd"   , 0, accountPw);  // 비밀번호
                base.SetFieldData("t0425InBlock", "expcode"  , 0, "");         // 종목번호
                base.SetFieldData("t0425InBlock", "chegb"    , 0, "0");        // 체결구분 - 0:전체|1:체결|2|미체결
                base.SetFieldData("t0425InBlock", "medosu"   , 0, "0");        // 매매구분 - 0:전체|1:매수|2:매도  
                base.SetFieldData("t0425InBlock", "sortgb"   , 0, "1");        // 정렬순서
                base.SetFieldData("t0425InBlock", "cts_ordno", 0, "");         // 주문번호

                // 계좌잔고 그리드 초기화

                //멤버변수 초기화
                base.Request(false);  //연속조회일경우 true

                //폼 메세지.
                mainForm.input_t0425_log.Text = "[" + mainForm.label_time.Text + "]t0425::체결/미체결 요청";

            } else {
                mainForm.input_t0425_log.Text = "[" + mainForm.label_time.Text + "][중복]t0425::체결/미체결 요청";
            }
        }	// end function
    } //end class   

    public class T0425Vo {
        public String ordtime  { set; get; } //주문시간
        public String medosu   { set; get; } //매매구분 - 0:전체|1:매수|2:매도
        public String expcode  { set; get; } //종목번호
        public String hname    { set; get; } //종목명
        public String qty      { set; get; } //주문수량
        public String price    { set; get; } //주문가격
        public String cheqty   { set; get; } //체결수량
        public String cheprice { set; get; } //체결가격
        public String ordrem   { set; get; } //미체결잔량
        public String status   { set; get; } //상태
        public String ordno    { set; get; } //주문번호


        public String ordptnDetail { set; get; }//상세 주문구분 신규매수|반복매수|금일매도|청산
        public String upOrdno      { set; get; }//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        public String upExecprc    { set; get; }//상위체결금액
        public String sellOrdAt    { set; get; }//매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String useYN        { set; get; }//사용여부
        public String toDaysunikrt { set; get; }//금일 수익률

    }
}   // end namespace
