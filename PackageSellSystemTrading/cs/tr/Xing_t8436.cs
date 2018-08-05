
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
    public class Xing_t8436 : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        private DataTable t8436DataTable = null;
        public DataTable getT8436DataTable()
        {
            return this.t8436DataTable;
        }

        // 생성자
        public Xing_t8436(){
          
            base.ResFileName = "₩res₩t8436.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);


            this.t8436DataTable = new DataTable();
            t8436DataTable.Columns.Add("hname"      , typeof(string));//종목명
            t8436DataTable.Columns.Add("shcode"     , typeof(string));//단축코드
            t8436DataTable.Columns.Add("expcode"    , typeof(string));//확장코드
            t8436DataTable.Columns.Add("etfgubun"   , typeof(string));//EFT구분(1:ETF 2:ETN)
            t8436DataTable.Columns.Add("uplmtprice" , typeof(double));//상한가
            t8436DataTable.Columns.Add("dnlmtprice" , typeof(double));//하한가
            t8436DataTable.Columns.Add("jnilclose"  , typeof(double));//전일가
            t8436DataTable.Columns.Add("memedan"    , typeof(string));//주문수량단위
            t8436DataTable.Columns.Add("recprice"   , typeof(double));//기준가
            t8436DataTable.Columns.Add("gubun"      , typeof(string));//구분(1:코스피 2:코드닥)
            t8436DataTable.Columns.Add("bu12gubun"  , typeof(string));//증권그룹
            t8436DataTable.Columns.Add("spac_gubun" , typeof(string));//기업인수목적회사여부(Y/N)
      
        }   // end function

        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            try
            {


                this.t8436DataTable.Clear();
                DataRow tmpRow;

                int iCount = base.GetBlockCount("t8436OutBlock");
                //String sunikrt;//수익률
                for (int i = 0; i < iCount; i++)
                {

                    tmpRow = t8436DataTable.NewRow();

                    t8436DataTable.Rows.Add(tmpRow);

                    tmpRow["hname"      ] = base.GetFieldData("t8436OutBlock", "hname"     , i);//종목명
                    tmpRow["shcode"     ] = base.GetFieldData("t8436OutBlock", "shcode"    , i);//단축코드
                    tmpRow["expcode"    ] = base.GetFieldData("t8436OutBlock", "expcode"   , i);//확장코드
                    tmpRow["etfgubun"   ] = base.GetFieldData("t8436OutBlock", "etfgubun"  , i);//EFT구분(1:ETF 2:ETN)
                    tmpRow["uplmtprice" ] = base.GetFieldData("t8436OutBlock", "uplmtprice", i);//상한가
                    tmpRow["dnlmtprice" ] = base.GetFieldData("t8436OutBlock", "dnlmtprice", i);//하한가
                    tmpRow["jnilclose"  ] = base.GetFieldData("t8436OutBlock", "jnilclose" , i);//전일가
                    tmpRow["memedan"    ] = base.GetFieldData("t8436OutBlock", "memedan"   , i);//주문수량단위
                    tmpRow["recprice"   ] = base.GetFieldData("t8436OutBlock", "recprice"  , i);//기준가
                    tmpRow["gubun"      ] = base.GetFieldData("t8436OutBlock", "gubun"     , i);//구분(1:코스피 2:코드닥)
                    tmpRow["bu12gubun"  ] = base.GetFieldData("t8436OutBlock", "bu12gubun" , i);//증권그룹
                    tmpRow["spac_gubun" ] = base.GetFieldData("t8436OutBlock", "spac_gubun", i);//기업인수목적회사여부(Y/N)
                }
            
            }
            catch (Exception ex)
            {
                Log.WriteLine("t8436 : " + ex.Message);
                Log.WriteLine("t8436 : " + ex.StackTrace);
            }


        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                completeAt = true;//중복호출 방지
            }
            

        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(){
    
           
            base.SetFieldData("t8436InBlock", "gubun", 0, "0");//구분(0:전체 1:코스피 2:코스닥)
            base.Request(false);
            
        }

    } //end class 
   
}   // end namespace
