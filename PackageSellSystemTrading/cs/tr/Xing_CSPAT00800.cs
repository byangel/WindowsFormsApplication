
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;

namespace PackageSellSystemTrading{
    //현물 취소주문
    public class Xing_CSPAT00800 : XAQueryClass{

        public MainForm mainForm;
        // 생성자
        public Xing_CSPAT00800()
        {
            base.ResFileName = "₩res₩CSPAT00800.res";

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_CSPAT00800()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            int iCount = base.GetBlockCount("t1833OutBlock1");

            // 매수종목 검색 그리드 초기화
            mainForm.grd_searchBuy.Rows.Clear();
            string[] row = new string[7];
            int addIndex;
            for (int i = 0; i < iCount; i++) {
                row[0] = base.GetFieldData("t1833OutBlock1", "shcode" , i); //종목코드
                row[1] = base.GetFieldData("t1833OutBlock1", "hname"  , i); //종목명
                row[2] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "close"  , i)); //현재가
                row[3] = base.GetFieldData("t1833OutBlock1", "sign"   , i); //전일대비구분 
                row[4] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "change" , i)); //전일대비
                row[5] = base.GetFieldData("t1833OutBlock1", "diff"   , i); //등락율
                row[6] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "volume" , i)); //거래량
                //row[0] = base.GetFieldData("t1833OutBlock1", "signcnt", i);//연속봉수
                //1.그리드 데이터 추가
                addIndex = mainForm.grd_searchBuy.Rows.Add(row);

                //2.계좌에 존제 여부 체크

                //3.매수
                Log.WriteLine("t1833.ReceiveDataEventHandler :: ");
            }
                      
            
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {

            try{
                if (nMessageCode == "00000"){
                    ;
                }else{
                    Log.WriteLine("CSPAT00800 :: " + nMessageCode + " :: " + szMessage);
                    //mainForm.input_t0424_log.Text = nMessageCode + " :: " + szMessage; 
                }
            }catch (Exception ex){
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
		/// 현물 취소 주문
		/// </summary>
		/// <param name="OrgOrdNo">원주문번호</param>
		/// <param name="IsuNo">종목번호</param>
		/// <param name="OrdQty">주문수량</param>
		public void call_request(string OrgOrdNo, string IsuNo, string OrdQty)
        {

            String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
            String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

            base.SetFieldData("CSPAT00800InBlock1", "AcntNo", 0, account);      // 계좌번호
            base.SetFieldData("CSPAT00800InBlock1", "InptPwd", 0, accountPw);   // 입력비밀번호
            base.SetFieldData("CSPAT00800InBlock1", "OrgOrdNo", 0, OrgOrdNo);                    // 원주문번호
            base.SetFieldData("CSPAT00800InBlock1", "IsuNo", 0, "A" + IsuNo);                    // 종목번호
            base.SetFieldData("CSPAT00800InBlock1", "OrdQty", 0, OrdQty);                        // 주문수량

            base.Request(false);
        }


    } //end class 
   
}   // end namespace
