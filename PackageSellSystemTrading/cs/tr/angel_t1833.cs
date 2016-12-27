
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
    public class Angel_t1833 : XAQueryClass{
 
      

        // 생성자
        public Angel_t1833()
        {
            //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
            //base.ResFileName = startupPath+"₩Resources₩t1833.res";
            base.ResFileName = "₩res₩t1833.res";

            //base.Request(false); //연속조회가 아닐경우 false

            base.ReceiveData += new _IXAQueryEvents_ReceiveDataEventHandler(_IXAQueryEvents_ReceiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(_IXAQueryEvents_ReceiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Angel_t1833()
        {
          
        }


        /// <summary>
		/// 미체결내역 조회 관련(T0434) 수신 처리부
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void _IXAQueryEvents_ReceiveDataEventHandler(string szTrCode)
        {
            //이벤트를 받았다면 서버에서 수신된 데이터를 가져와야 한다.
            String cnt = base.GetFieldData("t1833OutBlock", "jongCnt", 0);

            int iCount = base.GetBlockCount("t1833OutBlock1");

            for (int i = 0; i < iCount; i++)
            {
                string shcode  = base.GetFieldData("t1833OutBlock1", "shcode", i);
                string hname   = base.GetFieldData("t1833OutBlock1", "hname", i);
                string signcnt = base.GetFieldData("t1833OutBlock1", "signcnt", i);
                string volume  = base.GetFieldData("t1833OutBlock1", "volume", i);
                MessageBox.Show(hname);
            }
                      
            
        }

        void _IXAQueryEvents_ReceiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage)
        {

            if (nMessageCode != "00000")
            {
                MessageBox.Show("t1833 :: " + nMessageCode + " :: " + szMessage);
                //Log.WriteLine("t1833 :: " + nMessageCode + " :: " + szMessage);
            }
            if (bIsSystemError)
            {
                //lbSystemMessage.ForeColor = Color.Red;
                //lbSystemMessage.Text = string.Format("[{0}] {1}", nMessageCode, szMessage);
                //TraceLog(string.Format("[{0}] {1}", nMessageCode, szMessage));
            } else  {
               // lbSystemMessage.ForeColor = Color.DarkBlue;
               // lbSystemMessage.Text = string.Format("[{0}] {1}", nMessageCode, szMessage);
            }
        }

        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(string szFileName)
        {

            String startupPath = Application.StartupPath.Replace("\\bin\\Debug","");
            //MessageBox.Show(startupPath);
            //  file file =(file) Properties.Resources.Condition.na;
            base.RequestService("t1833", startupPath+"\\Resources\\Condition.ADF");
            
        }	// end function


    } //end class 
   
}   // end namespace
