using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackageSellSystemTrading
{
    public partial class OptionForm : Form
    {
        public MainForm mainForm;

        
        //기본설정
        public Boolean SELL_TO_RE_BUY_AT; 	        //매도종목 재매수 허용
        public Boolean NOT_MONEY_BUY_AT;  	        //미수 금지 여부

        public Boolean MAX_FUNDS_LIMITED_AT; 	    //최대 운영자금 제한 여부
        public String MAX_FUNDS_LIMITED_AMT;        //최대 운영자금 금액(만원)

        public Boolean BASE_MONEY_BUY_AT; 	        //자본금대비 매수 제한 여부
        public String BASE_MONEY_BUY_RATE; 	        //자본금대비 매수 제한 비율

        public String BUY_BATTING_AMT; 	            //1회매수금액(만워)
        public String MAX_BUY_COUNT; 		        //최대 매수 가능 종목수

        public Boolean KOS_AT; 			            //코스피 여부
        public Boolean KOS_YESTERDAY_AT; 	        //코스피 전일대비 여부
        public String  KOS_YESTERDAY_VAL;            //코스피 전일대비 값
        public String  KOS_YESTERDAY_VAL_SE;        //코스피 전일대비 구분[%| pt]

        public Boolean KOS_START_AT;     	        //코스피 시작가대비 여부
        public String  KOS_START_VAL;		        //코스피 시작가대비 값
        public String  KOS_START_VAL_SE;	        //코스피 시작가대비 구분[%| pt]

        public Boolean KOD_AT; 			            //코스닥 여부
        public Boolean KOD_YESTERDAY_AT; 	        //코스닥 전일대비 여부
        public String  KOD_YESTERDAY_VAL;            //코스닥 전일대비 값
        public String  KOD_YESTERDAY_VAL_SE;        //코스닥 전일대비 구분[%| pt]

        public Boolean KOD_START_AT;     	        //코스닥 시작가대비 여부
        public String  KOD_START_VAL;		        //코스닥 시작가대비 값
        public String  KOD_START_VAL_SE;	        //코스닥 시작가대비 구분[%| pt]

        //조건식설정
        public String BUY_SEARCH_NM1; 		        //매수 조건검색 파일 이름1
        public String BUY_SEARCH_NM2;		        //매수 조건검색 파일 이름2
        public String BUY_SEARCH_NM3;		        //매수 조건검색 파일 이름3
        public String BUY_SEARCH_SE1;		        //매수 조건검색 구분1
        public String BUY_SEARCH_SE2;		        //매수 조건검색 구분2
        public String BUY_HO; 			            //매수 호가
        public String BUY_HO_CHANGE_TIMMER; 	    //매수 호가 정정 타이머
        public String BUY_HO_CHANGE_SE; 	        //매수 호가 정정 구분
        public DateTime BUY_TIME_FROM; 		        //매수 동작 시간 FROM
        public DateTime BUY_TIME_TO; 		        //매수 동작 시간 TO

        public String  SELL_SEARCH_NM1; 	            //매도 조건검색 파일 이름1
        public String  SELL_SEARCH_NM2;		        //매도 조건검색 파일 이름2
        public String  SELL_SEARCH_NM3;		        //매도 조건검색 파일 이름3
        public String  SELL_SEARCH_SE1;		        //매도 조건검색 구분1
        public String  SELL_SEARCH_SE2;		        //매도 조건검색 구분2
        public String  SELL_HO; 		                //매도 호가
        public String  SELL_HO_CHANGE_TIMMER; 	    //매도 호가 정정 타이머
        public String  SELL_HO_CHANGE_SE; 	        //매도 호가 정정 구분
        public DateTime  SELL_TIME_FROM; 		        //매도 동작 시간 FROM
        public DateTime  SELL_TIME_TO; 		        //매도 동작 시간 TO
        public Boolean SELL_SEARCH_ONLY_AT;          //매도 조건검색 매도 금지 여부

        //트레이딩 스탑
        public String STOP_TARGET_RATE; 	        //목표 수익
        public String STOP_TARGET_DOWN_RATE; 	    //목표 수익 대비 하락율

        public Boolean ALL_SELL_AT; 		        //일괄 매도 여부
        public DateTime  ALL_SELL_TIME_FROM; 	        //일괄 매도 시작시간
        public DateTime  ALL_SELL_TIME_TO;	        //일괄 매도 종료시간
        public String  ALL_SELL_RATE;		        //일괄 매도 비율
        public String  ALL_SELL_RATE_SE;	        //일괄 매도 구분

        public Boolean STOP_LOSS_AT; 		        //손절 여부
        public Double  STOP_LOSS_RATE;		        //손절 하락 비율
        
        public Boolean SELL_TARGET_TIME_AT;	        //지정 시간 매도 여부
        public String  SELL_TARGET_TIME_OVR_RATE;   //지정시간 오버 수익 비율
        public Boolean SELL_TARGET_TIME_10;
        public Boolean SELL_TARGET_TIME_20;
        public Boolean SELL_TARGET_TIME_30;
        public Boolean SELL_TARGET_TIME_40;
        public Boolean SELL_TARGET_TIME_50;
        public Boolean SELL_TARGET_TIME_00;

        //추가 매수
        public Boolean ADD_BUY_SIGNAL_AT; 	        //추가매수 신호 여부
        public String  ADD_BUY_SIGNAL_RATE; 	    //추가매수 신호 하락 비율
        public String  ADD_BUY_SIGNAL_AMT;  	    //추가매수 신호 금액(만원)
        public Boolean ADD_BUY_AT; 		            //추가매수 여부
        public String  ADD_BUY_RATE;		        //추가매수 하락 비율
        public String  ADD_BUY_AMT;		            //추가매수 금액(만원)

        public Boolean ADD_BUY_SEARCH_AT;           //추가매수 조건검색 여부
        public String  ADD_BUY_SEARCH_NM;           //추가매수 조건검색 이름
        public DateTime  ADD_BUY_SEARCH_TIME_FROM;  //추가매수 조건검색 시작시간
        public DateTime  ADD_BUY_SEARCH_TIME_TO;	//추가매수 조건검색 종료시간

        //생성자
        public OptionForm()
        {
            InitializeComponent();
            this.init();
        }
        public void init()
        {
            //d_tesst = Double.Parse(input_test.Text);
            //기본설정
            this.SELL_TO_RE_BUY_AT      = cbx_sell_to_re_buy_at.Checked;		            //매도종목 재매수 허용
            this.NOT_MONEY_BUY_AT       = cbx_not_money_buy_at.Checked;		                //미수 금지 여부

            this.MAX_FUNDS_LIMITED_AT   = cbx_max_funds_limited_at.Checked;		            //최대 운영자금 제한 여부 
            this.MAX_FUNDS_LIMITED_AMT  = inp_max_funds_limited_amt.Text;		            //최대 운영자금 금액(만원)

            this.BASE_MONEY_BUY_AT      = cbx_base_money_buy_at.Checked;		            //자본금대비 매수 제한 여부
            this.BASE_MONEY_BUY_RATE    = inp_base_money_buy_rate.Text;		                //자본금대비 매수 제한 비율

            this.BUY_BATTING_AMT        = inp_buy_batting_amt.Text;			                //1회매수금액(만워)
            this.MAX_BUY_COUNT          = inp_max_buy_count.Text;			                //최대 매수 가능 종목수

            this.KOS_AT                 = cbx_kos_at.Checked;				                //코스피 여부
            this.KOS_YESTERDAY_AT       = cbx_kos_yesterday_at.Checked;		                //코스피 전일대비 여부
            this.KOS_YESTERDAY_VAL      = inp_kos_yesterday_val.Text;		                //코스피 전일대비 값
            this.KOS_YESTERDAY_VAL_SE   = sel_kos_yesterday_val_se.Text;		            //코스피 전일대비 구분[%|pt]

            this.KOS_START_AT           = cbx_kos_start_at.Checked;			                //코스피 시작가대비 여부
            this.KOS_START_VAL          = inp_kos_start_val.Text;			                //코스피 시작가대비 값
            this.KOS_START_VAL_SE       = sel_kos_start_val_se.Text;		                //코스피 시작가대비 구분[%|pt]

            this.KOD_AT                 = cbx_kod_at.Checked;				                //코스닥 여부
            this.KOD_YESTERDAY_AT       = cbx_kod_yesterday_at.Checked;		                //코스닥 전일대비 여부
            this.KOD_YESTERDAY_VAL      = inp_kod_yesterday_val.Text;		                //코스닥 전일대비 값
            this.KOD_YESTERDAY_VAL_SE   = sel_kod_yesterday_val_se.Text;		            //코스닥 전일대비 구분[%|pt]

            this.KOD_START_AT           = cbx_kod_start_at.Checked;			                //코스닥 시작가대비 여부
            this.KOD_START_VAL          = inp_kod_start_val.Text;		                    //코스닥 시작가대비 값
            this.KOD_START_VAL_SE       = sel_kod_start_val_se.Text;		                //코스닥 시작가대비 구분[%|pt]

            //조건식설정
            this.BUY_SEARCH_NM1         = sel_buy_search_nm1.Text;			                //매수 조건검색 파일 이름1
            this.BUY_SEARCH_NM2         = sel_buy_search_nm2.Text;			                //매수 조건검색 파일 이름2
            this.BUY_SEARCH_NM3         = sel_buy_search_nm3.Text;			                //매수 조건검색 파일 이름3
            this.BUY_SEARCH_SE1         = sel_buy_search_se1.Text;			                //매수 조건검색 구분1
            this.BUY_SEARCH_SE2         = sel_buy_search_se2.Text;			                //매수 조건검색 구분2
            this.BUY_HO                 = sel_buy_ho.Text;				                    //매수 호가
            this.BUY_HO_CHANGE_TIMMER   = inp_buy_ho_change_timmer.Text;	                //매수 호가 정정 타이머
            this.BUY_HO_CHANGE_SE       = sel_buy_ho_change_se.Text;			            //매수 호가 정정 구분
            this.BUY_TIME_FROM          = inp_buy_time_from.Value;			                //매수 동작 시간 FROM
            this.BUY_TIME_TO            = inp_buy_time_to.Value;				            //매수 동작 시간 TO

            this.SELL_SEARCH_NM1        = sel_sell_search_nm1.Text;				            //매도 조건검색 파일 이름1
            this.SELL_SEARCH_NM2        = sel_sell_search_nm2.Text;				            //매도 조건검색 파일 이름2
            this.SELL_SEARCH_NM3        = sel_sell_search_nm3.Text;				            //매도 조건검색 파일 이름3
            this.SELL_SEARCH_SE1        = sel_sell_search_se1.Text;			                //매도 조건검색 구분1
            this.SELL_SEARCH_SE2        = sel_sell_search_se2.Text;			                //매도 조건검색 구분2
            this.SELL_HO                = sel_sell_ho.Text;					                //매도 호가
            this.SELL_HO_CHANGE_TIMMER  = inp_sell_ho_change_timmer.Text;		            //매도 호가 정정 타이머
            this.SELL_HO_CHANGE_SE      = sel_sell_ho_change_se.Text;		                //매도 호가 정정 구분
            this.SELL_TIME_FROM         = inp_sell_time_from.Value;				            //매도 동작 시간 FROM
            this.SELL_TIME_TO           = inp_sell_time_to.Value;				            //매도 동작 시간 TO
            this.SELL_SEARCH_ONLY_AT    = cbx_sell_search_only_at.Checked;                  //매도 조건검색 매도 금지 여부

            //트레이딩 스탑
            this.STOP_TARGET_RATE       = inp_stop_target_rate.Text;			            //목표 수익
            this.STOP_TARGET_DOWN_RATE  = inp_stop_target_down_rate.Text;		            //목표 수익 대비 하락율

            this.ALL_SELL_AT            = cbx_all_sell_at.Checked;			                //일괄 매도 여부
            this.ALL_SELL_TIME_FROM     = inp_all_sell_time_from.Value;			            //일괄 매도 시작시간
            this.ALL_SELL_TIME_TO       = inp_all_sell_time_to.Value;			            //일괄 매도 종료시간 
            this.ALL_SELL_RATE          = inp_all_sell_rate.Text;			                //일괄 매도 비율 
            this.ALL_SELL_RATE_SE       = sel_all_sell_rate_se.Text;		                //일괄 매도 구분

            this.STOP_LOSS_AT           = cbx_stop_loss_at.Checked;			                //손절 여부
            this.STOP_LOSS_RATE         = Double.Parse(inp_stop_loss_rate.Text);			                //손절 하락 비율

            this.SELL_TARGET_TIME_AT    = cbx_sell_target_time_at.Checked;			        //지정 시간 매도 여부
            this.SELL_TARGET_TIME_OVR_RATE = inp_sell_target_time_ovr_rate.Text;	        //지정시간 오버 수익 비율
            this.SELL_TARGET_TIME_10    = cbx_sell_target_time_10.Checked;
            this.SELL_TARGET_TIME_20    = cbx_sell_target_time_20.Checked;
            this.SELL_TARGET_TIME_30    = cbx_sell_target_time_30.Checked;
            this.SELL_TARGET_TIME_40    = cbx_sell_target_time_40.Checked;
            this.SELL_TARGET_TIME_50    = cbx_sell_target_time_50.Checked;
            this.SELL_TARGET_TIME_00    = cbx_sell_target_time_00.Checked;

            //추가 매수
            this.ADD_BUY_SIGNAL_AT      = cbx_add_buy_signal_at.Checked; 		                //추가매수 신호 여부
            this.ADD_BUY_SIGNAL_RATE    = inp_add_buy_signal_rate.Text; 		                //추가매수 신호 하락 비율
            this.ADD_BUY_SIGNAL_AMT     = inp_add_buy_signal_amt.Text;  		                //추가매수 신호 금액(만원)
            this.ADD_BUY_AT             = cbx_add_buy_at.Checked; 			                    //추가매수 여부
            this.ADD_BUY_RATE           = inp_add_buy_rate.Text;			                    //추가매수 하락 비율
            this.ADD_BUY_AMT            = inp_add_buy_amt.Text;		                            //추가매수 금액(만원)

            this.ADD_BUY_SEARCH_AT      = cbx_add_buy_search_at.Checked;		                //추가매수 조건검색 여부
            this.ADD_BUY_SEARCH_NM      = inp_add_buy_search_nm.Text;		                    //추가매수 조건검색 이름
            this.ADD_BUY_SEARCH_TIME_FROM = inp_add_buy_search_time_from.Value;	    //추가매수 조건검색 시작시간
            this.ADD_BUY_SEARCH_TIME_TO = inp_add_buy_search_time_to.Value;          //추가매수 조건검색 종료시간

            
            //기본설정
            cbx_sell_to_re_buy_at.Checked       = Properties.Settings.Default.SELL_TO_RE_BUY_AT;            //매도종목 재매수 허용
            cbx_not_money_buy_at.Checked        = Properties.Settings.Default.NOT_MONEY_BUY_AT;           	//미수 금지 여부


            cbx_max_funds_limited_at.Checked    = Properties.Settings.Default.MAX_FUNDS_LIMITED_AT;       	//최대 운영자금 제한 여부 
            inp_max_funds_limited_amt.Text      = Properties.Settings.Default.MAX_FUNDS_LIMITED_AMT;      	//최대 운영자금 금액(만원)


            cbx_base_money_buy_at.Checked       = Properties.Settings.Default.BASE_MONEY_BUY_AT;          	//자본금대비 매수 제한 여부
            inp_base_money_buy_rate.Text        = Properties.Settings.Default.BASE_MONEY_BUY_RATE;        	//자본금대비 매수 제한 비율
            
            inp_buy_batting_amt.Text            = Properties.Settings.Default.BUY_BATTING_AMT;            	//1회매수금액(만워)
            inp_max_buy_count.Text              = Properties.Settings.Default.MAX_BUY_COUNT;              	//최대 매수 가능 종목수


            cbx_kos_at.Checked                  = Properties.Settings.Default.KOS_AT;                     	//코스피 여부
            cbx_kos_yesterday_at.Checked        = Properties.Settings.Default.KOS_YESTERDAY_AT;           	//코스피 전일대비 여부
            inp_kos_yesterday_val.Text          = Properties.Settings.Default.KOS_YESTERDAY_VAL;            //코스피 전일대비 값
            sel_kos_yesterday_val_se.Text       = Properties.Settings.Default.KOS_YESTERDAY_VAL_SE;       	//코스피 전일대비 구분[%|pt]
            
            cbx_kos_start_at.Checked            = Properties.Settings.Default.KOS_START_AT;               	//코스피 시작가대비 여부
            inp_kos_start_val.Text              = Properties.Settings.Default.KOS_START_VAL;              	//코스피 시작가대비 값
            sel_kos_start_val_se.Text           = Properties.Settings.Default.KOS_START_VAL_SE;             //코스피 시작가대비 구분[%|pt]
            
            cbx_kod_at.Checked                  = Properties.Settings.Default.KOD_AT;                     	//코스닥 여부
            cbx_kod_yesterday_at.Checked        = Properties.Settings.Default.KOD_YESTERDAY_AT;           	//코스닥 전일대비 여부
            inp_kod_yesterday_val.Text          = Properties.Settings.Default.KOD_YESTERDAY_VAL;            //코스닥 전일대비 값
            sel_kod_yesterday_val_se.Text       = Properties.Settings.Default.KOD_YESTERDAY_VAL_SE;       	//코스닥 전일대비 구분[%|pt]

            cbx_kod_start_at.Checked            = Properties.Settings.Default.KOD_START_AT;               	//코스닥 시작가대비 여부
            inp_kod_start_val.Text              = Properties.Settings.Default.KOD_START_VAL;                //코스닥 시작가대비 값
            sel_kod_start_val_se.Text           = Properties.Settings.Default.KOD_START_VAL_SE;             //코스닥 시작가대비 구분[%|pt]

            //조건식설정
            Util.setComBoByKeyValue(sel_buy_search_nm1, Properties.Settings.Default.BUY_SEARCH_NM1);        //매수 조건검색 파일 이름1
            Util.setComBoByKeyValue(sel_buy_search_nm2, Properties.Settings.Default.BUY_SEARCH_NM2);        //매수 조건검색 파일 이름2
            Util.setComBoByKeyValue(sel_buy_search_nm3, Properties.Settings.Default.BUY_SEARCH_NM3);        //매수 조건검색 파일 이름3

            sel_buy_search_se1.Text             = Properties.Settings.Default.BUY_SEARCH_SE1;             	//매수 조건검색 구분1
            sel_buy_search_se2.Text             = Properties.Settings.Default.BUY_SEARCH_SE2;             	//매수 조건검색 구분2

            sel_buy_ho.Text                     = Properties.Settings.Default.BUY_HO;                     	//매수 호가
            inp_buy_ho_change_timmer.Text       = Properties.Settings.Default.BUY_HO_CHANGE_TIMMER;         //매수 호가 정정 타이머
            sel_buy_ho_change_se.Text           = Properties.Settings.Default.BUY_HO_CHANGE_SE;           	//매수 호가 정정 구분
            inp_buy_time_from.Value             = Properties.Settings.Default.BUY_TIME_FROM;              	//매수 동작 시간 FROM
            inp_buy_time_to.Value               = Properties.Settings.Default.BUY_TIME_TO;                  //매수 동작 시간 TO


            Util.setComBoByKeyValue(sel_sell_search_nm1, Properties.Settings.Default.SELL_SEARCH_NM1);      //매도 조건검색 파일 이름1
            Util.setComBoByKeyValue(sel_sell_search_nm2, Properties.Settings.Default.SELL_SEARCH_NM2);      //매도 조건검색 파일 이름2
            Util.setComBoByKeyValue(sel_sell_search_nm3, Properties.Settings.Default.SELL_SEARCH_NM3);      //매도 조건검색 파일 이름3

            sel_sell_search_se1.Text            = Properties.Settings.Default.SELL_SEARCH_SE1;            	//매도 조건검색 구분1
            sel_sell_search_se2.Text            = Properties.Settings.Default.SELL_SEARCH_SE2;            	//매도 조건검색 구분2

            sel_sell_ho.Text                    = Properties.Settings.Default.SELL_HO;                    	//매도 호가
            inp_sell_ho_change_timmer.Text      = Properties.Settings.Default.SELL_HO_CHANGE_TIMMER;      	//매도 호가 정정 타이머
            sel_sell_ho_change_se.Text          = Properties.Settings.Default.SELL_HO_CHANGE_SE;            //매도 호가 정정 구분
            inp_sell_time_from.Value            = Properties.Settings.Default.SELL_TIME_FROM;     			//매도 동작 시간 FROM
            inp_sell_time_to.Value              = Properties.Settings.Default.SELL_TIME_TO;                 //매도 동작 시간 TO
            cbx_sell_search_only_at.Checked     = Properties.Settings.Default.SELL_SEARCH_ONLY_AT;          //매도 조건검색 매도 금지 여부
            //트레이딩 스탑
            inp_stop_target_rate.Text           = Properties.Settings.Default.STOP_TARGET_RATE;           	//목표 수익
            inp_stop_target_down_rate.Text      = Properties.Settings.Default.STOP_TARGET_DOWN_RATE;      	//목표 수익 대비 하락율


            cbx_all_sell_at.Checked             = Properties.Settings.Default.ALL_SELL_AT;                	//일괄 매도 여부
            inp_all_sell_time_from.Value        = Properties.Settings.Default.ALL_SELL_TIME_FROM;         	//일괄 매도 시작시간
            inp_all_sell_time_to.Value          = Properties.Settings.Default.ALL_SELL_TIME_TO;           	//일괄 매도 종료시간 
            inp_all_sell_rate.Text              = Properties.Settings.Default.ALL_SELL_RATE;              	//일괄 매도 비율 
            sel_all_sell_rate_se.Text           = Properties.Settings.Default.ALL_SELL_RATE_SE;             //일괄 매도 구분


            cbx_stop_loss_at.Checked            = Properties.Settings.Default.STOP_LOSS_AT;               	//손절 여부
            inp_stop_loss_rate.Text             = Properties.Settings.Default.STOP_LOSS_RATE.ToString();             	//손절 하락 비율


            cbx_sell_target_time_at.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_AT;        	//지정 시간 매도 여부
            inp_sell_target_time_ovr_rate.Text  = Properties.Settings.Default.SELL_TARGET_TIME_OVR_RATE;  	//지정시간 오버 수익 비율
            cbx_sell_target_time_10.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_10;
            cbx_sell_target_time_20.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_20;
            cbx_sell_target_time_30.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_30;
            cbx_sell_target_time_40.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_40;
            cbx_sell_target_time_50.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_50;
            cbx_sell_target_time_00.Checked     = Properties.Settings.Default.SELL_TARGET_TIME_00;

            //추가 매수
            cbx_add_buy_signal_at.Checked       = Properties.Settings.Default.ADD_BUY_SIGNAL_AT;          	//추가매수 신호 여부
            inp_add_buy_signal_rate.Text        = Properties.Settings.Default.ADD_BUY_SIGNAL_RATE;        	//추가매수 신호 하락 비율
            inp_add_buy_signal_amt.Text         = Properties.Settings.Default.ADD_BUY_SIGNAL_AMT;         	//추가매수 신호 금액(만원)
            cbx_add_buy_at.Checked              = Properties.Settings.Default.ADD_BUY_AT;                 	//추가매수 여부
            inp_add_buy_rate.Text               = Properties.Settings.Default.ADD_BUY_RATE;               	//추가매수 하락 비율
            inp_add_buy_amt.Text                = Properties.Settings.Default.ADD_BUY_AMT;                  //추가매수 금액(만원)


            cbx_add_buy_search_at.Checked       = Properties.Settings.Default.ADD_BUY_SEARCH_AT;          	//추가매수 조건검색 여부
            inp_add_buy_search_nm.Text          = Properties.Settings.Default.ADD_BUY_SEARCH_NM;            //추가매수 조건검색 이름
            inp_add_buy_search_time_from.Text   = Properties.Settings.Default.ADD_BUY_SEARCH_TIME_FROM;     //추가매수 조건검색 시작시간
            inp_add_buy_search_time_to.Text     = Properties.Settings.Default.ADD_BUY_SEARCH_TIME_TO;     	//추가매수 조건검색 종료시간
            
            //체크박스 화면 초기화
            cbx_add_buy_search_at_CheckedChanged    (this.cbx_add_buy_search_at     , new EventArgs());
            cbx_add_buy_at_CheckedChanged           (this.cbx_add_buy_at            , new EventArgs());
            cbx_add_buy_signal_at_CheckedChanged    (this.cbx_add_buy_signal_at     , new EventArgs());
            cbx_sell_target_time_at_CheckedChanged  (this.cbx_sell_target_time_at   , new EventArgs());
            cbx_all_sell_at_CheckedChanged          (this.cbx_all_sell_at           , new EventArgs());
            cbx_kod_at_CheckedChanged               (this.cbx_kod_at                , new EventArgs());
            cbx_kos_at_CheckedChanged               (this.cbx_kos_at                , new EventArgs());
            cbx_stop_loss_at_CheckedChanged         (this.cbx_stop_loss_at          , new EventArgs());
            cbx_base_money_buy_at_CheckedChanged    (this.cbx_base_money_buy_at     , new EventArgs());
            cbx_max_funds_limited_at_CheckedChanged (this.cbx_max_funds_limited_at  , new EventArgs());


        }

        //프로퍼티 초기화
        public void rollBack(){
            try{
                //기본설정
                cbx_sell_to_re_buy_at.Checked       = this.SELL_TO_RE_BUY_AT;              //매도종목 재매수 허용
                cbx_not_money_buy_at.Checked        = this.NOT_MONEY_BUY_AT;               //미수 금지 여부

                cbx_max_funds_limited_at.Checked    = this.MAX_FUNDS_LIMITED_AT;           //최대 운영자금 제한 여부 
                inp_max_funds_limited_amt.Text      = this.MAX_FUNDS_LIMITED_AMT;          //최대 운영자금 금액(만원)

                cbx_base_money_buy_at.Checked       = this.BASE_MONEY_BUY_AT;              //자본금대비 매수 제한 여부
                inp_base_money_buy_rate.Text        = this.BASE_MONEY_BUY_RATE;            //자본금대비 매수 제한 비율

                inp_buy_batting_amt.Text            = this.BUY_BATTING_AMT;                //1회매수금액(만워)
                inp_max_buy_count.Text              = this.MAX_BUY_COUNT;                  //최대 매수 가능 종목수
                
                cbx_kos_at.Checked                  = this.KOS_AT;                         //코스피 여부
                cbx_kos_yesterday_at.Checked        = this.KOS_YESTERDAY_AT;               //코스피 전일대비 여부
                inp_kos_yesterday_val.Text          = this.KOS_YESTERDAY_VAL;              //코스피 전일대비 값
                sel_kos_yesterday_val_se.Text       = this.KOS_YESTERDAY_VAL_SE;           //코스피 전일대비 구분[%|pt]
                
                cbx_kos_start_at.Checked            = this.KOS_START_AT;                   //코스피 시작가대비 여부
                inp_kos_start_val.Text              = this.KOS_START_VAL;                  //코스피 시작가대비 값
                sel_kos_start_val_se.Text           = this.KOS_START_VAL_SE;               //코스피 시작가대비 구분[%|pt]
                
                cbx_kod_at.Checked                  = this.KOD_AT;                         //코스닥 여부
                cbx_kod_yesterday_at.Checked        = this.KOD_YESTERDAY_AT;               //코스닥 전일대비 여부
                inp_kod_yesterday_val.Text          = this.KOD_YESTERDAY_VAL;              //코스닥 전일대비 값
                sel_kod_yesterday_val_se.Text       = this.KOD_YESTERDAY_VAL_SE;           //코스닥 전일대비 구분[%|pt]
                
                cbx_kod_start_at.Checked            = this.KOD_START_AT;                   //코스닥 시작가대비 여부
                inp_kod_start_val.Text              = this.KOD_START_VAL;                  //코스닥 시작가대비 값
                sel_kod_start_val_se.Text           = this.KOD_START_VAL_SE;               //코스닥 시작가대비 구분[%|pt]

                //조건식설정
                sel_buy_search_nm1.DataSource       = null;
                sel_buy_search_nm2.DataSource       = null;
                sel_buy_search_nm3.DataSource       = null;
                sel_buy_search_nm1.Text             = this.BUY_SEARCH_NM1;                   //매수 조건검색 파일 이름1
                sel_buy_search_nm2.Text             = this.BUY_SEARCH_NM2;                   //매수 조건검색 파일 이름2
                sel_buy_search_nm3.Text             = this.BUY_SEARCH_NM3;                   //매수 조건검색 파일 이름3
                
                sel_buy_search_se1.Text             = this.BUY_SEARCH_SE1;                   //매수 조건검색 구분1
                sel_buy_search_se2.Text             = this.BUY_SEARCH_SE2;                   //매수 조건검색 구분2
                sel_buy_ho.Text                     = this.BUY_HO;                           //매수 호가
                inp_buy_ho_change_timmer.Text       = this.BUY_HO_CHANGE_TIMMER;             //매수 호가 정정 타이머
                sel_buy_ho_change_se.Text           = this.BUY_HO_CHANGE_SE;                 //매수 호가 정정 구분
                inp_buy_time_from.Value             = this.BUY_TIME_FROM;                    //매수 동작 시간 FROM
                inp_buy_time_to.Value               = this.BUY_TIME_TO;                      //매수 동작 시간 TO

                sel_sell_search_nm1.DataSource      = null;
                sel_sell_search_nm2.DataSource      = null;
                sel_sell_search_nm3.DataSource      = null;
                sel_sell_search_nm1.Text            = this.SELL_SEARCH_NM1;                  //매도 조건검색 파일 이름1
                sel_sell_search_nm2.Text            = this.SELL_SEARCH_NM2;                  //매도 조건검색 파일 이름2
                sel_sell_search_nm3.Text            = this.SELL_SEARCH_NM3;                  //매도 조건검색 파일 이름3
                cbx_sell_search_only_at.Checked     = this.SELL_SEARCH_ONLY_AT;              //매도 조건검색 매도 금지 여부
              

                sel_sell_search_se1.Text            = this.SELL_SEARCH_SE1;                  //매도 조건검색 구분1
                sel_sell_search_se2.Text            = this.SELL_SEARCH_SE2;                  //매도 조건검색 구분2
                sel_sell_ho.Text                    = this.SELL_HO;                          //매도 호가
                inp_sell_ho_change_timmer.Text      = this.SELL_HO_CHANGE_TIMMER;            //매도 호가 정정 타이머
                sel_sell_ho_change_se.Text          = this.SELL_HO_CHANGE_SE;                //매도 호가 정정 구분
                inp_sell_time_from.Value            = this.SELL_TIME_FROM;                   //매도 동작 시간 FROM
                inp_sell_time_to.Value              = this.SELL_TIME_TO;                     //매도 동작 시간 TO

                //트레이딩 스탑
                inp_stop_target_rate.Text           = this.STOP_TARGET_RATE;                 //목표 수익
                inp_stop_target_down_rate.Text      = this.STOP_TARGET_DOWN_RATE;            //목표 수익 대비 하락율


                cbx_all_sell_at.Checked             = this.ALL_SELL_AT;                      //일괄 매도 여부
                inp_all_sell_time_from.Value        = this.ALL_SELL_TIME_FROM;               //일괄 매도 시작시간
                inp_all_sell_time_to.Value          = this.ALL_SELL_TIME_TO;                 //일괄 매도 종료시간 
                inp_all_sell_rate.Text              = this.ALL_SELL_RATE;                    //일괄 매도 비율 
                sel_all_sell_rate_se.Text           = this.ALL_SELL_RATE_SE;                 //일괄 매도 구분
                
                cbx_stop_loss_at.Checked            = this.STOP_LOSS_AT;                     //손절 여부
                inp_stop_loss_rate.Text             = this.STOP_LOSS_RATE.ToString();                   //손절 하락 비율
                
                cbx_sell_target_time_at.Checked     = this.SELL_TARGET_TIME_AT;               //지정 시간 매도 여부
                inp_sell_target_time_ovr_rate.Text  = this.SELL_TARGET_TIME_OVR_RATE;         //지정시간 오버 수익 비율
                cbx_sell_target_time_10.Checked     = this.SELL_TARGET_TIME_10;
                cbx_sell_target_time_20.Checked     = this.SELL_TARGET_TIME_20;
                cbx_sell_target_time_30.Checked     = this.SELL_TARGET_TIME_30;
                cbx_sell_target_time_40.Checked     = this.SELL_TARGET_TIME_40;
                cbx_sell_target_time_50.Checked     = this.SELL_TARGET_TIME_50;
                cbx_sell_target_time_00.Checked     = this.SELL_TARGET_TIME_00;

                //추가 매수
                cbx_add_buy_signal_at.Checked       = this.ADD_BUY_SIGNAL_AT;             //추가매수 신호 여부
                inp_add_buy_signal_rate.Text        = this.ADD_BUY_SIGNAL_RATE;            //추가매수 신호 하락 비율
                inp_add_buy_signal_amt.Text         = this.ADD_BUY_SIGNAL_AMT;              //추가매수 신호 금액(만원)
                cbx_add_buy_at.Checked              = this.ADD_BUY_AT;                           //추가매수 여부
                inp_add_buy_rate.Text               = this.ADD_BUY_RATE;                          //추가매수 하락 비율
                inp_add_buy_amt.Text                = this.ADD_BUY_AMT;                            //추가매수 금액(만원)
                
                cbx_add_buy_search_at.Checked       = this.ADD_BUY_SEARCH_AT;             //추가매수 조건검색 여부
                inp_add_buy_search_nm.Text          = this.ADD_BUY_SEARCH_NM;                //추가매수 조건검색 이름
                inp_add_buy_search_time_from.Value  = this.ADD_BUY_SEARCH_TIME_FROM;  //추가매수 조건검색 시작시간
                inp_add_buy_search_time_to.Value    = this.ADD_BUY_SEARCH_TIME_TO;     	//추가매수 조건검색 종료시간
            }
                catch (Exception ex)
            {
                Log.WriteLine("OptionForm : " + ex.Message);
                Log.WriteLine("OptionForm : " + ex.StackTrace);
            }
        }

        

        //폼내용을 프로퍼티에 저장
        public void btn_config_save_Click(object sender, EventArgs e){

            Properties.Settings.Default.SELL_TO_RE_BUY_AT = cbx_sell_to_re_buy_at.Checked;              //매도종목 재매수 허용                      
            Properties.Settings.Default.NOT_MONEY_BUY_AT = cbx_not_money_buy_at.Checked;                //미수 금지 여부                      

            Properties.Settings.Default.MAX_FUNDS_LIMITED_AT = cbx_max_funds_limited_at.Checked;        //최대 운영자금 제한 여부                    
            Properties.Settings.Default.MAX_FUNDS_LIMITED_AMT = inp_max_funds_limited_amt.Text;         //최대 운영자금 금액(만원)                               

            Properties.Settings.Default.BASE_MONEY_BUY_AT = cbx_base_money_buy_at.Checked;              //자본금대비 매수 제한 여부                  
            Properties.Settings.Default.BASE_MONEY_BUY_RATE = inp_base_money_buy_rate.Text;             //자본금대비 매수 제한 비율                              

            Properties.Settings.Default.BUY_BATTING_AMT = inp_buy_batting_amt.Text;                     //1회매수금액(만워)                                   
            Properties.Settings.Default.MAX_BUY_COUNT = inp_max_buy_count.Text;                         //최대 매수 가능 종목수                             

            Properties.Settings.Default.KOS_AT = cbx_kos_at.Checked;                                    //코스피 여부                        
            Properties.Settings.Default.KOS_YESTERDAY_AT = cbx_kos_yesterday_at.Checked;                //코스피 전일대비 여부                  
            Properties.Settings.Default.KOS_YESTERDAY_VAL = inp_kos_yesterday_val.Text;                 //코스피 전일대비 값                                      
            Properties.Settings.Default.KOS_YESTERDAY_VAL_SE = sel_kos_yesterday_val_se.Text;           //코스피 전일대비 구분[%|pt]                

            Properties.Settings.Default.KOS_START_AT = cbx_kos_start_at.Checked;                        //코스피 시작가대비 여부                 
            Properties.Settings.Default.KOS_START_VAL = inp_kos_start_val.Text;                         //코스피 시작가대비 값                              
            Properties.Settings.Default.KOS_START_VAL_SE = sel_kos_start_val_se.Text;                   //코스피 시작가대비 구분[%|pt]                  

            Properties.Settings.Default.KOD_AT = cbx_kod_at.Checked;                                    //코스닥 여부                        
            Properties.Settings.Default.KOD_YESTERDAY_AT = cbx_kod_yesterday_at.Checked;                //코스닥 전일대비 여부                  
            Properties.Settings.Default.KOD_YESTERDAY_VAL = inp_kod_yesterday_val.Text;                 //코스닥 전일대비 값                                      
            Properties.Settings.Default.KOD_YESTERDAY_VAL_SE = sel_kod_yesterday_val_se.Text;           //코스닥 전일대비 구분[%|pt]                

            Properties.Settings.Default.KOD_START_AT = cbx_kod_start_at.Checked;                        //코스닥 시작가대비 여부                 
            Properties.Settings.Default.KOD_START_VAL = inp_kod_start_val.Text;                         //코스닥 시작가대비 값                                 
            Properties.Settings.Default.KOD_START_VAL_SE = sel_kod_start_val_se.Text;		            //코스닥 시작가대비 구분[%|pt]                  
            
            // 조건식설정
            Properties.Settings.Default.BUY_SEARCH_NM1 = sel_buy_search_nm1.SelectedIndex > -1 ? sel_buy_search_nm1.SelectedValue.ToString() :""; //매수 조건검색 파일 이름1 
            Properties.Settings.Default.BUY_SEARCH_NM2 = sel_buy_search_nm2.SelectedIndex > -1 ? sel_buy_search_nm2.SelectedValue.ToString() : "";
            Properties.Settings.Default.BUY_SEARCH_NM3 = sel_buy_search_nm3.SelectedIndex > -1 ? sel_buy_search_nm3.SelectedValue.ToString() : "";
            Properties.Settings.Default.BUY_SEARCH_SE1 = sel_buy_search_se1.Text;                       //매수 조건검색 구분1                  
            Properties.Settings.Default.BUY_SEARCH_SE2 = sel_buy_search_se2.Text;                       //매수 조건검색 구분2                  
            Properties.Settings.Default.BUY_HO = sel_buy_ho.Text;                                       //매수 호가                     
            Properties.Settings.Default.BUY_HO_CHANGE_TIMMER = inp_buy_ho_change_timmer.Text;           //매수 호가 정정 타이머                                    
            Properties.Settings.Default.BUY_HO_CHANGE_SE = sel_buy_ho_change_se.Text;                   //매수 호가 정정 구분                      
            Properties.Settings.Default.BUY_TIME_FROM = inp_buy_time_from.Value;                         //매수 동작 시간 FROM             
            Properties.Settings.Default.BUY_TIME_TO = inp_buy_time_to.Value;                             //매수 동작 시간 TO           

            Properties.Settings.Default.SELL_SEARCH_NM1 = sel_sell_search_nm1.SelectedIndex > -1 ? sel_sell_search_nm1.SelectedValue.ToString() : "";//매도 조건검색 파일 이름1
            Properties.Settings.Default.SELL_SEARCH_NM2 = sel_sell_search_nm2.SelectedIndex > -1 ? sel_sell_search_nm2.SelectedValue.ToString() : "";//매도 조건검색 파일 이름2   
            Properties.Settings.Default.SELL_SEARCH_NM3 = sel_sell_search_nm3.SelectedIndex > -1 ? sel_sell_search_nm3.SelectedValue.ToString() : "";//매도 조건검색 파일 이름3 
                            
            Properties.Settings.Default.SELL_SEARCH_SE1 = sel_sell_search_se1.Text;                     //매도 조건검색 구분1                  
            Properties.Settings.Default.SELL_SEARCH_SE2 = sel_sell_search_se2.Text;                     //매도 조건검색 구분2                  
            Properties.Settings.Default.SELL_HO = sel_sell_ho.Text;                                     //매도 호가                 
            Properties.Settings.Default.SELL_HO_CHANGE_TIMMER = inp_sell_ho_change_timmer.Text;         //매도 호가 정정 타이머                                 
            Properties.Settings.Default.SELL_HO_CHANGE_SE = sel_sell_ho_change_se.Text;                 //매도 호가 정정 구분                         
            Properties.Settings.Default.SELL_TIME_FROM = inp_sell_time_from.Value;                       //매도 동작 시간 FROM     
            Properties.Settings.Default.SELL_TIME_TO = inp_sell_time_to.Value;			 		        //매도 동작 시간 TO    
            Properties.Settings.Default.SELL_SEARCH_ONLY_AT = cbx_sell_search_only_at.Checked;          //매도 조건검색 매도 금지 여부

            // 트레이딩 스탑
            Properties.Settings.Default.STOP_TARGET_RATE = inp_stop_target_rate.Text;                   //목표 수익                                         
            Properties.Settings.Default.STOP_TARGET_DOWN_RATE = inp_stop_target_down_rate.Text;         //목표 수익 대비 하락율                                 

            Properties.Settings.Default.ALL_SELL_AT = cbx_all_sell_at.Checked;                          //일괄 매도 여부                      
            Properties.Settings.Default.ALL_SELL_TIME_FROM = inp_all_sell_time_from.Value;               //일괄 매도 시작시간                       
            Properties.Settings.Default.ALL_SELL_TIME_TO = inp_all_sell_time_to.Value;                   //일괄 매도 종료시간                       
            Properties.Settings.Default.ALL_SELL_RATE = inp_all_sell_rate.Text;                         //일괄 매도 비율                                  
            Properties.Settings.Default.ALL_SELL_RATE_SE = sel_all_sell_rate_se.Text;                   //일괄 매도 구분                             

            Properties.Settings.Default.STOP_LOSS_AT = cbx_stop_loss_at.Checked;                        //손절 여부                         
            Properties.Settings.Default.STOP_LOSS_RATE = Double.Parse(inp_stop_loss_rate.Text);         //손절 하락 비율                                      

            Properties.Settings.Default.SELL_TARGET_TIME_AT = cbx_sell_target_time_at.Checked;          //지정 시간 매도 여부                  
            Properties.Settings.Default.SELL_TARGET_TIME_OVR_RATE = inp_sell_target_time_ovr_rate.Text; //지정시간 오버 수익 비율                                
            Properties.Settings.Default.SELL_TARGET_TIME_10 = cbx_sell_target_time_10.Checked;
            Properties.Settings.Default.SELL_TARGET_TIME_20 = cbx_sell_target_time_20.Checked;
            Properties.Settings.Default.SELL_TARGET_TIME_30 = cbx_sell_target_time_30.Checked;
            Properties.Settings.Default.SELL_TARGET_TIME_40 = cbx_sell_target_time_40.Checked;
            Properties.Settings.Default.SELL_TARGET_TIME_50 = cbx_sell_target_time_50.Checked;
            Properties.Settings.Default.SELL_TARGET_TIME_00 = cbx_sell_target_time_00.Checked;
            
            // 추가 매수
            Properties.Settings.Default.ADD_BUY_SIGNAL_AT = cbx_add_buy_signal_at.Checked;              //추가매수 신호 여부                       
            Properties.Settings.Default.ADD_BUY_SIGNAL_RATE = inp_add_buy_signal_rate.Text;             //추가매수 신호 하락 비율                                
            Properties.Settings.Default.ADD_BUY_SIGNAL_AMT = inp_add_buy_signal_amt.Text;               //추가매수 신호 금액(만원)                               
            Properties.Settings.Default.ADD_BUY_AT = cbx_add_buy_at.Checked;                            //추가매수 여부                       
            Properties.Settings.Default.ADD_BUY_RATE = inp_add_buy_rate.Text;                           //추가매수 하락 비율                               
            Properties.Settings.Default.ADD_BUY_AMT = inp_add_buy_amt.Text;                             //추가매수 금액(만원)                                 

            Properties.Settings.Default.ADD_BUY_SEARCH_AT = cbx_add_buy_search_at.Checked;              //추가매수 조건검색 여부                     
            Properties.Settings.Default.ADD_BUY_SEARCH_NM = inp_add_buy_search_nm.Text;                 //추가매수 조건검색 이름                        
            Properties.Settings.Default.ADD_BUY_SEARCH_TIME_FROM = inp_add_buy_search_time_from.Text;   //추가매수 조건검색 시작시간                         
            Properties.Settings.Default.ADD_BUY_SEARCH_TIME_TO = inp_add_buy_search_time_to.Text;       //추가매수 조건검색 종료시간   

            Properties.Settings.Default.Save();
            MessageBox.Show("설정을 저장하였습니다.");

        }

        

        /////////////////////////////////////////////////
        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //프로퍼티 초기화 버튼 클릭
        private void btn_rollback_Click(object sender, EventArgs e)
        {
            this.rollBack();
        }

        
        
        //자본금대비 신규매수 금지
        private void cbx_base_money_buy_at_CheckedChanged(object sender, EventArgs e) {
            if   (((CheckBox)sender).Checked) this.inp_base_money_buy_rate.Enabled = true;
            else                              this.inp_base_money_buy_rate.Enabled = false;
        }
        
        //코스피
        private void cbx_kos_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                this.cbx_kos_yesterday_at.Enabled       = true;
                this.inp_kos_yesterday_val.Enabled      = true;
                this.sel_kos_yesterday_val_se.Enabled   = true;
                this.cbx_kos_start_at.Enabled           = true;
                this.inp_kos_start_val.Enabled          = true;
                this.sel_kos_start_val_se.Enabled       = true;
            }else {
                this.cbx_kos_yesterday_at.Enabled       = false;
                this.inp_kos_yesterday_val.Enabled      = false;
                this.sel_kos_yesterday_val_se.Enabled   = false;
                this.cbx_kos_start_at.Enabled           = false;
                this.inp_kos_start_val.Enabled          = false;
                this.sel_kos_start_val_se.Enabled       = false;
            }
        }
        
        //코스닥
        private void cbx_kod_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked){
                this.cbx_kod_yesterday_at.Enabled       = true;
                this.inp_kod_yesterday_val.Enabled      = true;
                this.sel_kod_yesterday_val_se.Enabled   = true;
                this.cbx_kod_start_at.Enabled           = true;
                this.inp_kod_start_val.Enabled          = true;
                this.sel_kod_start_val_se.Enabled       = true;
            }else{
                this.cbx_kod_yesterday_at.Enabled       = false;
                this.inp_kod_yesterday_val.Enabled      = false;
                this.sel_kod_yesterday_val_se.Enabled   = false;
                this.cbx_kod_start_at.Enabled           = false;
                this.inp_kod_start_val.Enabled          = false;
                this.sel_kod_start_val_se.Enabled       = false;
            }
        }

        //일괄매도
        private void cbx_all_sell_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked){
                this.inp_all_sell_time_from.Enabled = true;
                this.inp_all_sell_time_to.Enabled   = true;
                this.inp_all_sell_rate.Enabled      = true;
                this.sel_all_sell_rate_se.Enabled   = true;
            }else{
                this.inp_all_sell_time_from.Enabled = false;
                this.inp_all_sell_time_to.Enabled   = false;
                this.inp_all_sell_rate.Enabled      = false;
                this.sel_all_sell_rate_se.Enabled   = false;
            }
        }
        //지정시간 매도 
        private void cbx_sell_target_time_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked){
                this.inp_sell_target_time_ovr_rate.Enabled = true;
                this.cbx_sell_target_time_10.Enabled = true;
                this.cbx_sell_target_time_20.Enabled = true;
                this.cbx_sell_target_time_30.Enabled = true;
                this.cbx_sell_target_time_40.Enabled = true;
                this.cbx_sell_target_time_50.Enabled = true;
                this.cbx_sell_target_time_00.Enabled = true;
            }else{
                this.inp_sell_target_time_ovr_rate.Enabled = false;
                this.cbx_sell_target_time_10.Enabled = false;
                this.cbx_sell_target_time_20.Enabled = false;
                this.cbx_sell_target_time_30.Enabled = false;
                this.cbx_sell_target_time_40.Enabled = false;
                this.cbx_sell_target_time_50.Enabled = false;
                this.cbx_sell_target_time_00.Enabled = false;
            }
        }
        //추가매수 
        private void cbx_add_buy_signal_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked){
                this.inp_add_buy_signal_rate.Enabled = true;
                this.inp_add_buy_signal_amt.Enabled = true;
            }else{
                this.inp_add_buy_signal_rate.Enabled = false;
                this.inp_add_buy_signal_amt.Enabled = false;
            }
        }
        //추가매수 
        private void cbx_add_buy_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked){
                this.inp_add_buy_rate.Enabled = true;
                this.inp_add_buy_amt.Enabled = true;
            }else{
                this.inp_add_buy_rate.Enabled = false;
                this.inp_add_buy_amt.Enabled = false;
            }
        }
        //추가매수 조건 검색
        private void cbx_add_buy_search_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                this.inp_add_buy_search_nm.Enabled = true;
                this.inp_add_buy_search_time_from.Enabled = true;
                this.inp_add_buy_search_time_to.Enabled = true;
            }
            else
            {
                this.inp_add_buy_search_nm.Enabled = false;
                this.inp_add_buy_search_time_from.Enabled = false;
                this.inp_add_buy_search_time_to.Enabled = false;
            }
        }
        //손절기능 체크변경이벤트
        private void cbx_stop_loss_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) this.inp_stop_loss_rate.Enabled = true;
            else this.inp_stop_loss_rate.Enabled = false;
        }
        //체크이벤트 최대운영자금 체크박스 변경 이벤트
        private void cbx_max_funds_limited_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) this.inp_max_funds_limited_amt.Enabled = true;
            else this.inp_max_funds_limited_amt.Enabled = false;
        }

        //public string hour { get; set; }
        //public string minute { get; set; }
        //public string second { get; set; }
        //검색 조건식 선택
        private void btn_buy_search_nm1_Click(object sender, EventArgs e)
        {

            if (!cbx_search_server_at.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.InitialDirectory = Application.StartupPath.Replace("\\bin\\Debug", "") + "\\Resources\\";
                openFileDialog1.Filter = "All files (*.*)|*.*|ACF files (*.ACF)|*.ACF";
                openFileDialog1.FilterIndex = 2;
                //openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Util.setComBoByKeyValue(sel_buy_search_nm1, openFileDialog1.FileName.ToString());
                }
            }
        }

        //검색 조건식 선택
        private void btn_buy_search_nm2_Click(object sender, EventArgs e)
        {
            if (!cbx_search_server_at.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.InitialDirectory = Application.StartupPath.Replace("\\bin\\Debug", "") + "\\Resources\\";
                openFileDialog1.Filter = "All files (*.*)|*.*|ACF files (*.ACF)|*.ACF";
                openFileDialog1.FilterIndex = 2;
                //openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Util.setComBoByKeyValue(sel_buy_search_nm2, openFileDialog1.FileName.ToString());
                }
            }
        }
        //검색 조건식 선택
        private void btn_buy_search_nm3_Click(object sender, EventArgs e)
        {
            if (!cbx_search_server_at.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.InitialDirectory = Application.StartupPath.Replace("\\bin\\Debug", "") + "\\Resources\\";
                openFileDialog1.Filter = "All files (*.*)|*.*|ACF files (*.ACF)|*.ACF";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Util.setComBoByKeyValue(sel_buy_search_nm3, openFileDialog1.FileName.ToString());
                }
            }
        }
        //검색 조건식 선택
        private void btn_sell_search_nm1_Click(object sender, EventArgs e)
        {
            if (!cbx_search_server_at.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.InitialDirectory = Application.StartupPath.Replace("\\bin\\Debug", "") + "\\Resources\\";
                openFileDialog1.Filter = "All files (*.*)|*.*|ACF files (*.ACF)|*.ACF";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Util.setComBoByKeyValue(sel_sell_search_nm1, openFileDialog1.FileName.ToString());
                }
            }
        }
        //검색 조건식 선택
        private void btn_sell_search_nm2_Click(object sender, EventArgs e)
        {
            if (!cbx_search_server_at.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.InitialDirectory = Application.StartupPath.Replace("\\bin\\Debug", "") + "\\Resources\\";
                openFileDialog1.Filter = "All files (*.*)|*.*|ACF files (*.ACF)|*.ACF";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Util.setComBoByKeyValue(sel_sell_search_nm2, openFileDialog1.FileName.ToString());
                }
            }
        }
        //검색 조건식 선택
        private void btn_sell_search_nm3_Click(object sender, EventArgs e)
        {
            if (!cbx_search_server_at.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.InitialDirectory = Application.StartupPath.Replace("\\bin\\Debug", "") + "\\Resources\\";
                openFileDialog1.Filter = "All files (*.*)|*.*|ACF files (*.ACF)|*.ACF";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Util.setComBoByKeyValue(sel_sell_search_nm3, openFileDialog1.FileName.ToString());
                }
            }
        }

        //검색 조건식 서버 사용 여부
        private void cbx_search_server_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                //MessageBox.Show("서비스 준비중입니다.");
                ((CheckBox)sender).Checked = false;
                
                MessageBox.Show(inp_buy_time_from.Value.ToString());
                MessageBox.Show(inp_buy_time_from.Text.ToString());
            }
        }

        
    }//class end
}//name end
