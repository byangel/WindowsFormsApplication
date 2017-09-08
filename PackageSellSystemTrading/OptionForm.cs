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

        Boolean LIMITED_AT        = true;        //운영자금 제한 여부
        Boolean TODAY_SELL_AT     = true;        //중복매수된 종목 금일 매수/매도 기능 활성화 여부 디폴트 true
        Boolean STOP_LOSS_AT      = false;       //손절사용여부
        Boolean EXCL_STOP_LOSS_AT = false;       //매수금지종목 손절 여부
        Boolean TIME_PROFIT_TARGET_AT= false;       //시간차목표수익율

        String STOP_LOSS          = "";      //2분후 무조건 매도
        String REPEAT_RATE        = "-5";        //반복매수 비율
        String BUY_STOP_RATE      = "70";        //자본금 대비 매입금액  제한 비율 - 매입금액 / 자본금(매입금액 + D2예수금) * 100 =자본금 대비 투자율
        String MAX_AMT_LIMIT      = "300000000"; //최대 운영 금액 제한 - 기본 3억
        String BATTING_ATM        = "";
         
        public String CONDITION_NM       = "Condition.ADF";
        public String EXCLUDE_NM         = "Exclude.ADF";
        public String STOP_PROFIT_TARGET = "2.5";      //목표 이익율
        public String STOP_PROFIT_TARGET2 = "2";       //투자율 95%이상 목표 이익율2

        //생성자
        public OptionForm()
        {
            InitializeComponent();

            checkBox_limited.Checked       = Properties.Settings.Default.LIMITED_AT;//운영자금 제한 매입금액+D2예수금
            checkBox_today_sell.Checked    = Properties.Settings.Default.TODAY_SELL_AT;//금일매도/매수
            checkBox_stopLoss.Checked      = Properties.Settings.Default.STOP_LOSS_AT;//손절사용여부
            checkBox_exclStopLossAt.Checked= Properties.Settings.Default.EXCL_STOP_LOSS_AT;//매수금지종목 손절여부
            checkBox_time_profit_target.Checked = Properties.Settings.Default.TIME_PROFIT_TARGET_AT;
            input_repeat_rate.Text         = Properties.Settings.Default.REPEAT_RATE.ToString();
            input_buy_stop_rate.Text       = Properties.Settings.Default.BUY_STOP_RATE.ToString();
            input_max_amt_limit.Text       = Properties.Settings.Default.MAX_AMT_LIMIT.ToString();  
            input_stopLoss.Text            = Properties.Settings.Default.STOP_LOSS.ToString(); //손절 범위
            input_stop_profit_target.Text  = Properties.Settings.Default.STOP_PROFIT_TARGET.ToString();//목표 이익율
            input_stop_profit_target2.Text = Properties.Settings.Default.STOP_PROFIT_TARGET2.ToString();//목표 이익율2
            input_battingAtm.Text          = Properties.Settings.Default.BATTING_ATM.ToString();//배팅금액 설정
            

            //combox_condition.SelectedIndex = 0; 조건검색식 선택 콤보박스 초기화
            //for (int i=0;i< combox_condition.Items.Count; i++)
            //{
            //    if(combox_condition.Items[i].ToString() == Properties.Settings.Default.CONDITION_ADF)
            //    {
            //        combox_condition.SelectedIndex = i;
            //    }
            //}
            
            //체크박스 화면 초기화
            //checkBox_stop_loss_CheckedChanged();
            checkBox_limited_CheckedChanged();
            checkBox_stopLoss_CheckedChanged();
        }

        //프로퍼티 초기화
        public void rollBack()
        {
            checkBox_limited.Checked       = this.LIMITED_AT;     //운영자금 제한 매입금액+D2예수금
            checkBox_today_sell.Checked    = this.TODAY_SELL_AT;  //금일매도/매수
            checkBox_stopLoss.Checked      = this.STOP_LOSS_AT;   //손절사용여부
            checkBox_exclStopLossAt.Checked = this.EXCL_STOP_LOSS_AT;//매수금지종목 손절여부
            checkBox_time_profit_target.Checked = this.TIME_PROFIT_TARGET_AT;////시간차목표수익율
            input_repeat_rate.Text         = this.REPEAT_RATE;    //반복매수 비율
            input_buy_stop_rate.Text       = this.BUY_STOP_RATE;  //자본금 대비 매입금액 제한 비율
            input_max_amt_limit.Text       = this.MAX_AMT_LIMIT;  //최대 운영 금액 제한 - 기본 1억
            input_stopLoss.Text            = this.STOP_LOSS;      //손절
            input_stop_profit_target.Text  = this.STOP_PROFIT_TARGET;//목표 이익율
            input_stop_profit_target2.Text = this.STOP_PROFIT_TARGET2;//목표 이익율2
            input_battingAtm.Text          = this.BATTING_ATM;       //배팅금액
            
            //combox_condition.Text          = this.CONDITION_ADF;     //검색식
            //combox_conditionExclude.Text   = this.CONDITION_EXCLUDE; //매수금지 조건 검색식


            //Properties.Settings.Default.Save();
        }

        //폼내용을 프로퍼티에 저장
        public void btn_config_save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LIMITED_AT          = checkBox_limited.Checked;      //운영자금 제한 사용여부
            Properties.Settings.Default.TODAY_SELL_AT       = checkBox_today_sell.Checked;   //금일매수매도 사용여부
            Properties.Settings.Default.STOP_LOSS_AT        = checkBox_stopLoss.Checked; //손절 사용여부
            Properties.Settings.Default.EXCL_STOP_LOSS_AT   = checkBox_exclStopLossAt.Checked; //매수금지종목 손절여부
            Properties.Settings.Default.TIME_PROFIT_TARGET_AT = checkBox_time_profit_target.Checked;//시간차목표수익율
            Properties.Settings.Default.REPEAT_RATE         = input_repeat_rate.Text;
            Properties.Settings.Default.BUY_STOP_RATE       = input_buy_stop_rate.Text;
            Properties.Settings.Default.MAX_AMT_LIMIT       = input_max_amt_limit.Text;
            Properties.Settings.Default.STOP_LOSS           = input_stopLoss.Text;
            Properties.Settings.Default.STOP_PROFIT_TARGET  = input_stop_profit_target.Text; //목표 이익율
            Properties.Settings.Default.STOP_PROFIT_TARGET2 = input_stop_profit_target2.Text; //목표 이익율2
            Properties.Settings.Default.BATTING_ATM         = input_battingAtm.Text;         //배팅금액 강제설정
            //Properties.Settings.Default.CONDITION_ADF       = combox_condition.Text;        //검색식
            //Properties.Settings.Default.CONDITION_EXCLUDE   = combox_conditionExclude.Text;         //매수금지 조건 검색식
           

            Properties.Settings.Default.Save();
            MessageBox.Show("설정을 저장하였습니다.");
            //this.Close();
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
            checkBox_limited_CheckedChanged();
        }
        private void checkBox_limited_CheckedChanged()
        {
            if (checkBox_limited.Checked)
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
            checkBox_stopLoss_CheckedChanged();
        }
        private void checkBox_stopLoss_CheckedChanged()
        {
            if (checkBox_stopLoss.Checked){//손절사용
                this.input_stopLoss.Enabled = true;
            } else {//손절 미사용
                this.input_stopLoss.Enabled = false;
            }
        }
    }//class end
}//name end
