using System;

using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;


using XA_DATASETLib;



using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageSellSystemTrading
{
 //장정보
	public class Real_IJ : XARealClass
    {
		// 멤버변수 선언
		//private IXAReal mReal;
        public MainForm mainForm;


		/// <summary>
		/// 생성자
		/// </summary>
		public Real_IJ()
		{

            base.ResFileName = "₩res₩IJ_.res";

            base.ReceiveRealData += new _IXARealEvents_ReceiveRealDataEventHandler(receiveDataEventHandler);


        }	// end function



         /// <summary>
         /// 실시간 지수 
         /// </summary>
         /// <param name="szTrCode"></param>
        void receiveDataEventHandler(string szTrCode){
            
			try
			{
                string time     = base.GetFieldData("OutBlock", "time"  ); //시간
                string jisu     = base.GetFieldData("OutBlock", "jisu"  ); //지수
                string sign     = base.GetFieldData("OutBlock", "sign"  ); //전일대비구분
                string change   = base.GetFieldData("OutBlock", "change"); //전일비
                string drate    = base.GetFieldData("OutBlock", "drate" ); //등락율
                string upcode   = base.GetFieldData("OutBlock", "upcode"); //업종코드

                //코스피
                if (upcode.Equals("001"))
                {
                    if (Double.Parse(drate) < 0)
                    {
                        mainForm.label_ks.ForeColor = Color.Blue;

                    }
                    else
                    {
                        mainForm.label_ks.ForeColor = Color.Red;
                    }
                   
                    mainForm.label_ks.Text = jisu + " " + change + " " + drate;

                }
                //코스닥
                if (upcode.Equals("301"))
                {
                    if (Double.Parse(drate) < 0)
                    {
                        mainForm.label_kd.ForeColor = Color.Blue;

                    }
                    else
                    {
                        mainForm.label_kd.ForeColor = Color.Red;
                    }
                    mainForm.label_kd.Text = jisu + " " + change + " " + drate;

                }
                    
                
            }
			catch (Exception ex)
			{
				Log.WriteLine("IJ : " + ex.Message);
                Log.WriteLine("IJ : " + ex.StackTrace);
			}
		}	// end function


		/// <summary>
		/// 실시간 호출 등록
		/// </summary>
		public void call_advise(String upcode)
		{
            base.SetFieldData("InBlock", "upcode", upcode);
			base.AdviseRealData();
		}	// end function


	}//end class
}	

