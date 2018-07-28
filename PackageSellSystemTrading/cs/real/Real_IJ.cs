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

        //코스피 데이타
        public String ks_jisu         = null;  //코스피지수
        public Double ks_change       = 0; //코스피전일비
        public Double ks_drate        = 0; //코스피등락율

        public Double ks_openByChange = 0;    //시가대비
        public Double ks_openByDrate  = 0;      //시가대비 등락율
        //코드닥 데이타
        public String kd_jisu         = null;  //코드닥지수
        public Double kd_change       = 0;//코드닥전일비
        public Double kd_drate        = 0; //코드닥등락율

        public Double kd_openByChange = 0;
        public Double kd_openByDrate  = 0;
        /// <summary>
        /// 실시간 지수 
        /// </summary>
        /// <param name="szTrCode"></param>
        void receiveDataEventHandler(string szTrCode){
            
			try
			{
                string time      = base.GetFieldData("OutBlock", "time"  ); //시간
                string jisu      = base.GetFieldData("OutBlock", "jisu"  ); //지수
                string sign      = base.GetFieldData("OutBlock", "sign"  ); //전일대비구분
                string change    = base.GetFieldData("OutBlock", "change"); //전일비
                string drate     = base.GetFieldData("OutBlock", "drate" ); //등락율
                string upcode    = base.GetFieldData("OutBlock", "upcode"); //업종코드
                string openjisu  = base.GetFieldData("OutBlock", "openjisu"); //시가지수
                string opentime  = base.GetFieldData("OutBlock", "opentime"); //시가시간
                string highjisu  = base.GetFieldData("OutBlock", "highjisu"); //고가지수
                string hightime  = base.GetFieldData("OutBlock", "hightime"); //고가시간
                string lowjisu   = base.GetFieldData("OutBlock", "lowjisu"); //저가지수
                string lowtime   = base.GetFieldData("OutBlock", "lowtime"); //저가시간
                string frgsvalue = base.GetFieldData("OutBlock", "frgsvalue"); //외인순매수금액
                string orgsvalue = base.GetFieldData("OutBlock", "orgsvalue"); //기관순매수금액

                //코스피
                if (upcode.Equals("001"))
                {
                    if (Double.Parse(drate) < 0){
                        mainForm.label_ks.ForeColor = Color.Blue;
                    }else{
                        mainForm.label_ks.ForeColor = Color.Red;
                    }
                   
                    mainForm.label_ks.Text = jisu + " " + change + " " + drate;
                    this.ks_jisu     = jisu;
                    this.ks_change   = Double.Parse(change);
                    this.ks_drate    = Double.Parse(drate);

                    ks_openByChange  = Double.Parse(openjisu) - Double.Parse(kd_jisu);              //시가대비
                    ks_openByDrate   = Util.getRate(Double.Parse(openjisu), Double.Parse(kd_jisu)); //시가대비 등락율

                }
                //코스닥
                if (upcode.Equals("301"))
                {
                    if (Double.Parse(drate) < 0){
                        mainForm.label_kd.ForeColor = Color.Blue;

                    }else{
                        mainForm.label_kd.ForeColor = Color.Red;
                    }
                    mainForm.label_kd.Text = jisu + " " + change + " " + drate;
                    this.kd_jisu     = jisu;
                    this.kd_change   = Double.Parse(change);
                    this.kd_drate    = Double.Parse(drate);
                    
                    kd_openByChange = Double.Parse(openjisu) - Double.Parse(kd_jisu);    //시가대비
                    kd_openByDrate  = Util.getRate(Double.Parse(openjisu), Double.Parse(kd_jisu));  //시가대비 등락율

                }
                  
            }
			catch (Exception ex)
			{
				Log.WriteLine("IJ : " + ex.Message);
                Log.WriteLine("IJ : " + ex.StackTrace);
			}
		}	// end function

        //시가대비 등락 포인트
        public Double openByChange(String upcode)
        {
            if (upcode.Equals("001"))
            {
               
            }
            //코스닥
            if (upcode.Equals("301"))
            {
            }

            return 0;
        }
        
        //시가대비 등락율
        public Double openByDrate(String upcode)
        {
            return 0;
        }
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

