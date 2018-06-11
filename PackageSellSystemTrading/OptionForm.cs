using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackageSellSystemTrading
{
    public partial class OptionForm : Form
    {
        public MainForm mainForm;

        Boolean LIMITED_AT              = true;  //운영자금 제한 여부
        Boolean TIME_PROFIT_TARGET_AT   = true; //시간차목표수익율
        Boolean STOP_LOSS_AT            = false; //손절사용여부
        Boolean EXCL_STOP_LOSS_AT       = false; //매수금지종목 손절 여부

        String STOP_LOSS          = "";         //2분후 무조건 매도
        String REPEAT_RATE        = "-3";        //반복매수 비율
        String BUY_STOP_RATE      = "50";        //자본금 대비 매입금액  제한 비율 - 매입금액 / 자본금(매입금액 + D2예수금) * 100 =자본금 대비 투자율
        String MAX_AMT_LIMIT      = "300000000"; //최대 운영 금액 제한 - 기본 3억
        String BATTING_RATE        = "0.005"; //예탁자산 대비 진입비율

        Boolean DPSASTTOTAMT_GROWTH_AT = true; //예탁자산 증가율 여부
        String DPSASTTOTAMT_GROWTH_RATE = "3"; //예탁자산총금액 증가율 - 증가율 달성시 매수금지종목모두 삭제기능

        public String CONDITION_NM       = "Condition.ADF";
        public String EXCLUDE_NM         = "Exclude.ADF";
        public String STOP_PROFIT_TARGET = "7";      //목표 이익율
        public String STOP_PROFIT_TARGET2 = "5";       //3시이후 목표 수익율
        
        
        //생성자
        public OptionForm()
        {
            InitializeComponent();
            this.init();
        }
        public void init()
        {
            //프로그램 최초 설치시 메모리에 값이없을때 최초 기본 값으로 설정해준다.
            
            //운영자금 제한 매입금액+D2예수금
            if (Properties.Settings.Default.LIMITED_AT.ToString() == "")
            {
                Properties.Settings.Default.LIMITED_AT = this.LIMITED_AT;
                Properties.Settings.Default.Save();
            }
            //손절사용여부
            if (Properties.Settings.Default.STOP_LOSS_AT.ToString() == "")
            {
                Properties.Settings.Default.STOP_LOSS_AT = this.STOP_LOSS_AT;
                Properties.Settings.Default.Save();
            }
            //매수금지종목 손절여부
            if (Properties.Settings.Default.EXCL_STOP_LOSS_AT.ToString() == "")
            {
                Properties.Settings.Default.EXCL_STOP_LOSS_AT = this.EXCL_STOP_LOSS_AT;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.TIME_PROFIT_TARGET_AT.ToString() == "")
            {
                Properties.Settings.Default.TIME_PROFIT_TARGET_AT = this.TIME_PROFIT_TARGET_AT;
                Properties.Settings.Default.Save();
            }
            //반복매수 -수익율
            if (Properties.Settings.Default.REPEAT_RATE.ToString() == "")
            {
                Properties.Settings.Default.REPEAT_RATE = this.REPEAT_RATE;
                Properties.Settings.Default.Save();
            }
            //예탁자산대미 투자율
            if (Properties.Settings.Default.BUY_STOP_RATE.ToString() == "")
            {
                Properties.Settings.Default.BUY_STOP_RATE = this.BUY_STOP_RATE;
                Properties.Settings.Default.Save();
            }
            //최대운영자금
            if (Properties.Settings.Default.MAX_AMT_LIMIT.ToString() == "")
            {
                Properties.Settings.Default.MAX_AMT_LIMIT = this.MAX_AMT_LIMIT;
                Properties.Settings.Default.Save();
            }
            //손절 범위
            if (Properties.Settings.Default.STOP_LOSS.ToString() == "")
            {
                Properties.Settings.Default.STOP_LOSS = this.STOP_LOSS;
                Properties.Settings.Default.Save();
            }
            //목표 이익율
            if (Properties.Settings.Default.STOP_PROFIT_TARGET.ToString() == "")
            {
                Properties.Settings.Default.STOP_PROFIT_TARGET = this.STOP_PROFIT_TARGET;
                Properties.Settings.Default.Save();
            }
            ;//목표 이익율2
            if (Properties.Settings.Default.STOP_PROFIT_TARGET2.ToString() == "")
            {
                Properties.Settings.Default.STOP_PROFIT_TARGET2 = this.STOP_PROFIT_TARGET2;
                Properties.Settings.Default.Save();
            }
            //예탁자산 대비 비중
            if (Properties.Settings.Default.BATTING_RATE.ToString() == "")
            {
                Properties.Settings.Default.BATTING_RATE = this.BATTING_RATE;
                Properties.Settings.Default.Save();
            }
            ////예탁자산 총액 증가율 사용여부
            if (Properties.Settings.Default.DPSASTTOTAMT_GROWTH_AT.ToString() == "")
            {
                Properties.Settings.Default.DPSASTTOTAMT_GROWTH_AT = this.DPSASTTOTAMT_GROWTH_AT;
                Properties.Settings.Default.Save();
            }
            //예탁자산 총액 증가율
            if (Properties.Settings.Default.DPSASTTOTAMT_GROWTH_RATE.ToString() == "")
            {
                Properties.Settings.Default.DPSASTTOTAMT_GROWTH_RATE = this.DPSASTTOTAMT_GROWTH_RATE;
                Properties.Settings.Default.Save();
            }



            checkBox_limited.Checked            = Properties.Settings.Default.LIMITED_AT;//운영자금 제한 매입금액+D2예수금
            checkBox_stopLoss.Checked           = Properties.Settings.Default.STOP_LOSS_AT;//손절사용여부
            checkBox_exclStopLossAt.Checked     = Properties.Settings.Default.EXCL_STOP_LOSS_AT;//매수금지종목 손절여부
            checkBox_time_profit_target.Checked = Properties.Settings.Default.TIME_PROFIT_TARGET_AT;
            input_repeat_rate.Text              = Properties.Settings.Default.REPEAT_RATE.ToString(); //반복매수 -수익율
            input_buy_stop_rate.Text            = Properties.Settings.Default.BUY_STOP_RATE.ToString();//예탁자산대미 투자율
            input_max_amt_limit.Text            = Properties.Settings.Default.MAX_AMT_LIMIT.ToString();  //최대운영자금
            input_stopLoss.Text                 = Properties.Settings.Default.STOP_LOSS.ToString(); //손절 범위
            input_stop_profit_target.Text       = Properties.Settings.Default.STOP_PROFIT_TARGET.ToString();//목표 이익율
            input_stop_profit_target2.Text      = Properties.Settings.Default.STOP_PROFIT_TARGET2.ToString();//목표 이익율2
            input_battingRate.Text               = Properties.Settings.Default.BATTING_RATE.ToString();//예탁자산 대비 비중

            checkBox_dpsastTotAmt_growth_at.Checked = Properties.Settings.Default.DPSASTTOTAMT_GROWTH_AT; //예탁자산 총액 증가율 사용여부
            input_dpsastTotAmt_growth_rate.Text     = Properties.Settings.Default.DPSASTTOTAMT_GROWTH_RATE.ToString();//예탁자산 총액 증가율
            label_dpsastTotAmt_max.Text             = Util.GetNumberFormat(Properties.Settings.Default.DPSASTTOTAMT_MAX.ToString()); ;//최대 예탁자산 총액

            //combox_condition.SelectedIndex = 0; 조건검색식 선택 콤보박스 초기화
            //for (int i=0;i< combox_condition.Items.Count; i++)
            //{
            //    if(combox_condition.Items[i].ToString() == Properties.Settings.Default.CONDITION_ADF)
            //    {
            //        combox_condition.SelectedIndex = i;
            //    }
            //}

            //체크박스 화면 초기화
            checkBox_limited_CheckedChanged(this.checkBox_limited, new EventArgs());
            checkBox_stopLoss_CheckedChanged(this.checkBox_stopLoss, new EventArgs());
            checkBox_dpsastTotAmt_growth_at_CheckedChanged(this.checkBox_dpsastTotAmt_growth_at, new EventArgs());
        }

        //프로퍼티 초기화
        public void rollBack()
        {
            try
            {
                checkBox_limited.Checked        = this.LIMITED_AT;               //운영자금 제한 매입금액+D2예수금
                checkBox_stopLoss.Checked       = this.STOP_LOSS_AT;             //손절사용여부
                checkBox_exclStopLossAt.Checked = this.EXCL_STOP_LOSS_AT;        //매수금지종목 손절여부
                checkBox_time_profit_target.Checked = this.TIME_PROFIT_TARGET_AT;    //시간차목표수익율
                input_repeat_rate.Text          = this.REPEAT_RATE;              //반복매수 비율
                input_buy_stop_rate.Text        = this.BUY_STOP_RATE;            //자본금 대비 매입금액 제한 비율
                input_max_amt_limit.Text        = this.MAX_AMT_LIMIT;            //최대 운영 금액 제한 - 기본 1억
                input_stopLoss.Text             = this.STOP_LOSS;                //손절
                input_stop_profit_target.Text   = this.STOP_PROFIT_TARGET;       //목표 이익율
                input_stop_profit_target2.Text  = this.STOP_PROFIT_TARGET2;      //목표 이익율2
                input_battingRate.Text          = this.BATTING_RATE;              //예탁자산 총액대비 배팅 비율

                //checkBox_dpsastTotAmt_growth_at.Checked = this.DPSASTTOTAMT_GROWTH_AT;             //예탁자산 총액 증가율 사용여부
                //input_dpsastTotAmt_growth_rate.Text     = this.DPSASTTOTAMT_GROWTH_RATE.ToString();//예탁자산 총액 증가율
            }
                catch (Exception ex)
            {
                Log.WriteLine("OptionForm : " + ex.Message);
                Log.WriteLine("OptionForm : " + ex.StackTrace);
            }
        }

        

        //폼내용을 프로퍼티에 저장
        public void btn_config_save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LIMITED_AT              = checkBox_limited.Checked;         //운영자금 제한 사용여부
            Properties.Settings.Default.STOP_LOSS_AT            = checkBox_stopLoss.Checked;        //손절 사용여부
            Properties.Settings.Default.EXCL_STOP_LOSS_AT       = checkBox_exclStopLossAt.Checked; //매수금지종목 손절여부
            Properties.Settings.Default.TIME_PROFIT_TARGET_AT   = checkBox_time_profit_target.Checked;//시간차목표수익율
            Properties.Settings.Default.REPEAT_RATE             = input_repeat_rate.Text;
            Properties.Settings.Default.BUY_STOP_RATE           = input_buy_stop_rate.Text;
            Properties.Settings.Default.MAX_AMT_LIMIT           = input_max_amt_limit.Text;
            Properties.Settings.Default.STOP_LOSS               = input_stopLoss.Text;
            Properties.Settings.Default.STOP_PROFIT_TARGET      = input_stop_profit_target.Text;    //목표 이익율
            Properties.Settings.Default.STOP_PROFIT_TARGET2     = input_stop_profit_target2.Text;   //목표 이익율2
            Properties.Settings.Default.BATTING_RATE            = input_battingRate.Text;            //배팅금액 강제설정

            Properties.Settings.Default.DPSASTTOTAMT_GROWTH_AT   = checkBox_dpsastTotAmt_growth_at.Checked; //예탁자산 총액 증가율 사용여부
            Properties.Settings.Default.DPSASTTOTAMT_GROWTH_RATE = input_dpsastTotAmt_growth_rate.Text;   //예탁자산 총액 증가율
            
            Properties.Settings.Default.Save();
            MessageBox.Show("설정을 저장하였습니다.");

        }

        //===================================이벤트

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //프로퍼티 초기화 버튼 클릭
        private void btn_rollback_Click(object sender, EventArgs e)
        {
            this.rollBack();
        }
        
        //최대운영자금 체크박스 변경 이벤트
        private void checkBox_limited_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                this.input_max_amt_limit.Enabled = true;
            }
            else//손절 미사용
            {
                this.input_max_amt_limit.Enabled = false;
            }
        }
        
        //손절기능 체크변경이벤트
        private void checkBox_stopLoss_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {//손절사용
                this.input_stopLoss.Enabled = true;
            }else{//손절 미사용
                this.input_stopLoss.Enabled = false;
            }
        }
        
        //예탁자산 총액 증가율 사용여부
        private void checkBox_dpsastTotAmt_growth_at_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {//손절사용
                this.input_dpsastTotAmt_growth_rate.Enabled = true;
            }
            else
            {//손절 미사용
                this.input_dpsastTotAmt_growth_rate.Enabled = false;
            }
        }
        //예탁자산 대비 배팅 금액 계산
        private void input_battingRate_TextChanged(object sender, EventArgs e)
        {
            
            String dpsastTotAmtMax = Properties.Settings.Default.DPSASTTOTAMT_MAX;//기준 예탁자산 총액

            if (dpsastTotAmtMax != "")
            {
                String input_battingRate = ((TextBox)sender).Text;
                //String 예탁자산총액 = mainForm.label_DpsastTotamt.Text;
                this.label_battingAmt.Text = Util.GetNumberFormat(Util.getBattingAmt(dpsastTotAmtMax, input_battingRate));
            }
        }
    }//class end
}//name end
