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
using System.Drawing;
using System.Threading;
namespace PackageSellSystemTrading{
    public class Xing_t1833 : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;
       
        // 생성자
        public Xing_t1833(){
            //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            //base.ResFileName = startupPath+"₩Resources₩t1833.res";
            base.ResFileName = "₩res₩t1833.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_t1833(){
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            int iCount = base.GetBlockCount("t1833OutBlock1");

            //매수종목 검색 그리드 초기화
            mainForm.grd_t1833.Rows.Clear();
            string[] row = new string[7];
            int addIndex;

            DataRow[] dataRowArray;
            String shcode;//종목코드
            String close; //현재가
            for (int i = 0; i < iCount; i++) {
                shcode = base.GetFieldData("t1833OutBlock1", "shcode", i);//종목코드
                close  = base.GetFieldData("t1833OutBlock1", "close", i); //현재가

                row[0] = shcode;//종목코드
                row[1] = base.GetFieldData("t1833OutBlock1", "hname"  , i); //종목명
                row[2] = Util.GetNumberFormat(close); //현재가
                row[3] = base.GetFieldData("t1833OutBlock1", "sign"   , i); //전일대비구분 
                row[4] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "change" , i)); //전일대비
                row[5] = base.GetFieldData("t1833OutBlock1", "diff"   , i); //등락율
                row[6] = Util.GetNumberFormat(base.GetFieldData("t1833OutBlock1", "volume" , i)); //거래량
                //row[0] = base.GetFieldData("t1833OutBlock1", "signcnt", i);//연속봉수
                //1.그리드 데이터 추가
                addIndex = mainForm.grd_t1833.Rows.Add(row);

                //2.계좌에 존제 여부 체크
               
               
                //종목이 기존 그리드에 존재여부에따라 row 추가 또는 수정 분기를 해준다.
                
                dataRowArray = mainForm.dataTable_t0424.Select("expcode = '" + shcode + "'");
                if (dataRowArray.Length > 0){//보유종목이면..하이라키...
                    mainForm.grd_t1833.Rows[addIndex].Cells["shcode"].Style.BackColor = Color.Gray;
                    //mainForm.grd_t1833.Rows[addIndex].DefaultCellStyle.BackColor = Color.Gray;
                }else{//보유종목이 아니면...
                    ;
                }
                //3.매수
                /// <param name="IsuNo">종목번호</param>
                /// <param name="Quantity">수량</param>
                /// <param name="Price">가격</param>
                /// <param name="DivideBuySell">매매구분 : 1-매도, 2-매수</param>
                
                //1.금일 매수이력이 있으면 매수하지 않는다.
                //2.보유종목 매수 시기가 1시간이상 이전이고 수익률이 -3% 이하이면 반복매수한다.
                //mainForm.xing_CSPAT00600.call_request(shcode, Quantity, close, "2");
                //Log.WriteLine("t1833.ReceiveDataEventHandler :: ");
            }

           
           mainForm.input_t1833_log2.Text = "[" + mainForm.input_time.Text+ "]조건검색 응답 완료";
            completeAt = true;//중복호출 여부
    

        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
              
            } else { 
                Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
                mainForm.input_t1833_log2.Text = "[" + mainForm.input_time.Text + "]t1833 :: " + nMessageCode + " :: " + szMessage;
                completeAt = true;//중복호출 방지
            }
            


        }
        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(string conditionFileName){

            
            if (completeAt) {
                //MessageBox.Show("1");
                //폼 메세지.
                completeAt = false;//중복호출 방지
                mainForm.input_t1833_log1.Text = "[" + mainForm.input_time.Text + "]조건검색 요청.";
                //Thread.Sleep(1000);

                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                base.RequestService("t1833", startupPath + "\\Resources\\"+ conditionFileName);//_6만급등족목_70_

                //mainForm.input_t1833_log2.Text = "대기";

            } else {
                mainForm.input_t1833_log1.Text = "[중복]조건검색 요청.";
                //mainForm.input_t1833_log2.Text = "대기";
            }
        }

    } //end class 
   
}   // end namespace
