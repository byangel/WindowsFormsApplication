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

        Boolean LIMITED_AT           = true;     //운영자금 제한 여부
        Boolean TODAY_SELL_AT        = true;     //중복매수된 종목 금일 매수/매도 기능 활성화 여부 디폴트 true
        Boolean STOP_LOSS_AT      = false;       //손절사용여부

        String STOP_PROFIT_TARGET = "2";         //목표 이익율
        String STOP_LOSS          = "";          //손절
        String REPEAT_TERM        = "9600";      //반복매수 시간
        String REPEAT_RATE        = "-3";        //반복매수 비율
        String BUY_STOP_RATE      = "90";        //자본금 대비 매입금액  제한 비율 - 매입금액 / 자본금(매입금액 + D2예수금) * 100 =자본금 대비 투자율
        String MAX_AMT_LIMIT      = "100000000"; //최대 운영 금액 제한 - 기본 1억
        String BATTING_ATM        = "";
        String CONDITION_ADF = "ConditionExtend.ADF";
        public OptionForm()
        {
            InitializeComponent();

            checkBox_limited.Checked      = Properties.Settings.Default.LIMITED_AT;//운영자금 제한 매입금액+D2예수금
            checkBox_today_sell.Checked   = Properties.Settings.Default.TODAY_SELL_AT;//금일매도/매수
            checkBox_stop_loss.Checked    = Properties.Settings.Default.STOP_LOSS_AT;//손절사용여부
            input_repeat_term.Text        = Properties.Settings.Default.REPEAT_TERM.ToString();
            input_repeat_rate.Text        = Properties.Settings.Default.REPEAT_RATE.ToString();
            input_buy_stop_rate.Text      = Properties.Settings.Default.BUY_STOP_RATE.ToString();
            input_max_amt_limit.Text      = Properties.Settings.Default.MAX_AMT_LIMIT.ToString();  
            input_stop_loss.Text          = Properties.Settings.Default.STOP_LOSS.ToString(); //손절
            input_stop_profit_target.Text = Properties.Settings.Default.STOP_PROFIT_TARGET.ToString();//목표 이익율
            input_battingAtm.Text         = Properties.Settings.Default.BATTING_ATM.ToString();//배팅금액 설정
        }

        //프로퍼티 초기화
        public void rollBack()
        {
            checkBox_limited.Checked      = this.LIMITED_AT;     //운영자금 제한 매입금액+D2예수금
            checkBox_today_sell.Checked   = this.TODAY_SELL_AT;  //금일매도/매수
            checkBox_stop_loss.Checked    = this.STOP_LOSS_AT;   //손절사용여부
            input_repeat_term.Text        = this.REPEAT_TERM;    //반복매수 기간(분)
            input_repeat_rate.Text        = this.REPEAT_RATE;    //반복매수 비율
            input_buy_stop_rate.Text      = this.BUY_STOP_RATE;  //자본금 대비 매입금액 제한 비율
            input_max_amt_limit.Text      = this.MAX_AMT_LIMIT;  //최대 운영 금액 제한 - 기본 1억
            input_stop_loss.Text          = this.STOP_LOSS;      //손절
            input_stop_profit_target.Text = this.STOP_PROFIT_TARGET;//목표 이익율
            input_battingAtm.Text         = this.BATTING_ATM;//배팅금액
            Properties.Settings.Default.Save();
        }

        //폼내용을 프로퍼티에 저장
        private void btn_config_save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LIMITED_AT          = checkBox_limited.Checked;      //운영자금 제한 사용여부
            Properties.Settings.Default.TODAY_SELL_AT       = checkBox_today_sell.Checked;   //금일매수매도 사용여부
            Properties.Settings.Default.STOP_LOSS_AT        = checkBox_stop_loss.Checked; //손절 사용여부

            Properties.Settings.Default.REPEAT_TERM         = input_repeat_term.Text;
            Properties.Settings.Default.REPEAT_RATE         = input_repeat_rate.Text;
            Properties.Settings.Default.BUY_STOP_RATE       = input_buy_stop_rate.Text;
            Properties.Settings.Default.MAX_AMT_LIMIT       = input_max_amt_limit.Text;
            Properties.Settings.Default.STOP_LOSS           = input_stop_loss.Text;
            Properties.Settings.Default.STOP_PROFIT_TARGET  = input_stop_profit_target.Text; //목표 이익율
            Properties.Settings.Default.BATTING_ATM         = input_battingAtm.Text; //배팅금액 강제설정

            Properties.Settings.Default.Save();
            MessageBox.Show("설정을 저장하였습니다.");
            //this.Close();
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //프로퍼티 초기화 버튼 클릭
        private void btn_rollback_Click(object sender, EventArgs e)
        {
            this.rollBack();
        }
    }
}
