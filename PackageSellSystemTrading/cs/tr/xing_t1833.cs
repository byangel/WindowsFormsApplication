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

namespace PackageSellSystemTrading{
    public class Xing_t1833 : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;
        // 생성자
        public Xing_t1833()
        {
            //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            //base.ResFileName = startupPath+"₩Resources₩t1833.res";
            base.ResFileName = "₩res₩t1833.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_t1833()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            int iCount = base.GetBlockCount("t1833OutBlock1");

            //매수종목 검색 그리드 초기화
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
                //Log.WriteLine("t1833.ReceiveDataEventHandler :: ");
            }

            //로그 및 중복 요청 처리
            mainForm.input_t1833_log.Text = "조건검색 요청 완료";
            completeAt = true;//중복호출 여부

        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

            try {
                if (nMessageCode == "00000") {
                    MessageBox.Show("t1833 :: " + nMessageCode + " :: " + szMessage);
                    //Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                } else {
                    Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                    mainForm.input_t1833_log.Text = nMessageCode + " :: " + szMessage;
                    completeAt = true;//중복호출 방지
                }
            } catch (Exception ex) {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(string szFileName){
            if (completeAt) {
                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");

                base.RequestService("t1833", startupPath + "\\Resources\\Condition.ADF");
                Log.WriteLine("t1833.call_request :: " + szFileName);

                completeAt = false;//중복호출 방지
                //폼 메세지.
                mainForm.input_t1833_log.Text = "조건검색 요청을 하였습니다.";

            } else {
                mainForm.input_t1833_log.Text = "[중복]조건검색 요청을 하였습니다.";
            }
        }


    } //end class 
   
}   // end namespace
