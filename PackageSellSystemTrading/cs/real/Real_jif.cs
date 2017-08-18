using System;

using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;


using XA_DATASETLib;

namespace PackageSellSystemTrading
{
    //장정보
	public class Real_jif : XARealClass
    {
		// 멤버변수 선언
		//private IXAReal mReal;
        public MainForm mainForm;

        public string mjstatus = "";
		public string mlabel = "";
		public DateTime mdatetiem;


		/// <summary>
		/// 생성자
		/// </summary>
		public Real_jif()
		{

            base.ResFileName = "₩res₩JIF.res";

            base.ReceiveRealData += new _IXARealEvents_ReceiveRealDataEventHandler(receiveDataEventHandler);
          

        }	// end function



        /// <summary>
        /// 실시간 데이터 처리 
        /// </summary>
        /// <param name="szTrCode"></param>
        void receiveDataEventHandler(string szTrCode){
            
			try
			{
                string jangubun = base.GetFieldData("OutBlock", "jangubun");
				string jstatus = base.GetFieldData("OutBlock", "jstatus");
                //double iTimeCur = mForm.mxTr0167.mTimeCur;

				// 장 구분이 코스피일 경우 
				if (jangubun == "1")
				{
					this.mjstatus = jstatus;
                    this.mdatetiem = DateTime.Now;
                    this.mlabel = mjstatus + "-" + get_label(mjstatus);
                 

                    mainForm.label_jif.Text = this.mlabel;
                    //장이 마감되면 스넵샷을 찍는다.(챠트데이타 저장)
                    if (jstatus == "41")
                    {
                        ChartVo chartVo = new ChartVo();
                        chartVo.date            = DateTime.Now.ToString("yyyyMMdd");                    //날자              
                        chartVo.d2Dps           = mainForm.label_D2Dps          .Text.Replace(",", ""); //예수금(D2)         
                        chartVo.dpsastTotamt    = mainForm.label_DpsastTotamt   .Text.Replace(",", ""); //예탁자산총액          
                        chartVo.mamt            = mainForm.label_mamt           .Text.Replace(",", ""); //매입금액            
                        chartVo.balEvalAmt      = mainForm.label_BalEvalAmt     .Text.Replace(",", ""); //매입평가금액          
                        chartVo.pnlRat          = mainForm.label_PnlRat         .Text.Replace(",", ""); //손익율             
                        chartVo.tdtsunik        = mainForm.label_tdtsunik       .Text.Replace(",", ""); //평가손익            
                        chartVo.dtsunik         = mainForm.label_dtsunik        .Text.Replace(",", ""); //실현손익            
                        chartVo.battingAtm      = mainForm.label_battingAtm     .Text.Replace(",", ""); //배팅금액            
                        chartVo.toDaysunik      = mainForm.label_toDaysunik     .Text.Replace(",", ""); //당일매도 실현손익       
                        chartVo.dtsunik2        = mainForm.label_dtsunik2       .Text.Replace(",", ""); //실현손익2           
                        chartVo.investmentRatio = mainForm.label_InvestmentRatio.Text;                  //투자율             
                        chartVo.itemTotalCnt    = mainForm.h_totalCount         .Text;                  //총 보유종목 수     
                        chartVo.buyFilterCnt    = mainForm.exCnt                .Text;                  //매수금지종목수         
                        chartVo.buyCnt          = mainForm.label_buyCnt         .Text;                  //매수횟수            
                        chartVo.sellCnt         = mainForm.label_sellCnt.Text.Replace(",", "");         //매도횟수  
                        mainForm.chartData.insert(chartVo);

                        mainForm.chartData.dbSync();
                        //누적수익율 출력
                        mainForm.label_sum_dtsunik.Text = Util.GetNumberFormat(mainForm.chartData.getSumDtsunik());
                    }

                }
			}
			catch (Exception ex)
			{
				Log.WriteLine("jif : " + ex.Message);
                Log.WriteLine("jif : " + ex.StackTrace);
			}
		}	// end function


		/// <summary>
		/// 실시간 호출 등록
		/// </summary>
		public void call_advise()
		{
            base.SetFieldData("InBlock", "jangubun", "0");
			base.AdviseRealData();
		}	// end function


		/// <summary>
		/// HTS -> API 연동 등록
		/// </summary>
		public void call_advise_link_from_hts()
		{
            base.AdviseLinkFromHTS();
		}	// end function


		

		/// <summary>
		/// 장 상태값에 해당하는 라벨값 리턴
		/// </summary>
		private string get_label(string jstatus)
		{
			if (jstatus == "11")
			{
				return "장전 동시호가 개시";
			}
			else if (jstatus == "21")
			{
				return "장 시작";
			}
			else if (jstatus == "22")
			{
				return "장 개시 10초 전";
			}
			else if (jstatus == "23")
			{
				return "장 개시 1분 전";
			}
			else if (jstatus == "24")
			{
				return "장 개시 5분 전";
			}
			else if (jstatus == "25")
			{
				return "장 개시 10분 전";
			}
			else if (jstatus == "31")
			{
				return "장후 동시호가 개시";
			}
			else if (jstatus == "41")
			{
				return "장 마감";
			}
			else if (jstatus == "42")
			{
				return "장 마감 10초 전";
			}
			else if (jstatus == "43")
			{
				return "장 마감 1분 전";
			}
			else if (jstatus == "44")
			{
				return "장 마감 5분 전";
			}
			else if (jstatus == "51")
			{
				return "시간외종가 매매 개시";
			}
			else if (jstatus == "52")
			{
				return "시간외종가 매매 종료";
			}
			else if (jstatus == "53")
			{
				return "시간외단일가 매매 개시";
			}
			else if (jstatus == "54")
			{
				return "시간외단일가 매매 종료";
			}
			else if (jstatus == "61")
			{
				return "서킷브레이크 발동";
			}
			else if (jstatus == "62")
			{
				return "서킷브레이크 해제";
			}
			else if (jstatus == "63")
			{
				return "서킷브레이크 단일가 접수";
			}
			else if (jstatus == "64")
			{
				return "사이드카 매도 발동";
			}
			else if (jstatus == "65")
			{
				return "사이드카 매도 해제";
			}
			else if (jstatus == "66")
			{
				return "사이드카 매수 발동";
			}
			else if (jstatus == "67")
			{
				return "사이드카 매수 해제";
			}
			else
			{
				return "해당 코드 없음";
			}
		}	// end function


		/// <summary>
		/// 장 상태값 초기화
		/// </summary>
		public void init_jstatus()
		{
            //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

                double time = Double.Parse(DateTime.Now.ToString("HHmmss"));

				if (time < 73000)
				{
					mlabel = "41-장 마감";
					mdatetiem = DateTime.Now;
				}
				else if (time >= 73000 && time < 80000)		// 7:30 ~ 8:30 시간외종가
				{
					mlabel = "51-시간외종가 매매 개시";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 7:30:00");
				}
				else if (time >= 80000 && time < 85000)		// 8:00 ~ 9:00 장전 동시호가(시간외종가와 중첩 구간 있음)
				{
					mlabel = "11-장전 동시호가 개시";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 8:00:00");
				}
				else if (time >= 85000 && time < 85500)
				{
					mlabel = "25-장 개시 10분 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 8:50:00");
				}
				else if (time >= 85500 && time < 85900)
				{
					mlabel = "24-장 개시 5분 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 8:55:00");
				}
				else if (time >= 85900 && time < 85950)
				{
					mlabel = "23-장 개시 1분 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 8:59:00");
				}
				else if (time >= 85950 && time < 90000)
				{
					mlabel = "22-장 개시 10초 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 8:59:50");
				}
				else if (time >= 90000 && time < 145000)
				{
					mlabel = "21-장 시작";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 9:00:00");
				}
				else if (time >= 145000 && time < 145500)
				{
					mlabel = "31-장후 동시호가 개시";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 14:50:00");
				}
				else if (time >= 145500 && time < 145900)
				{
					mlabel = "44-장 마감 5분 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 14:55:00");
				}
				else if (time >= 145900 && time < 145950)
				{
					mlabel = "43-장 마감 1분 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 14:59:00");
				}
				else if (time >= 145950 && time < 150000)
				{
					mlabel = "42-장 마감 10초 전";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 14:59:50");
				}
				else
				{
					mlabel = "41-장 마감";
					mdatetiem = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 15:00:00");
				}

				mjstatus = mlabel.Split('-')[0];
			}
		}	// end function
	}	// end class
	// end namespace
