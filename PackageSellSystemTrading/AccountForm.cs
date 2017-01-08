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
    public partial class AccountForm : Form
    {

        public MainForm mainForm;

        public AccountForm(MainForm mainForm){
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void AccountForm_Load(object sender, EventArgs e){
            String account;
            int accountListCount = mainForm.exXASessionClass.GetAccountListCount();
            for (int i = 0; i < accountListCount; i++)
            {
                account = mainForm.exXASessionClass.GetAccountList(i);
                this.listBox_account.Items.Add(account);
            }
            this.listBox_account.SelectedIndex = 0;

            //주식잔고2
            //mainForm.xing_t0424.call_request();
        }


        //확인
        private void btn_account_check_Click(object sender, EventArgs e){
            String listBox_account = this.listBox_account.Text;
            String input_accountPw = this.input_accountPw.Text;

            if (listBox_account == "" || input_accountPw == ""){
                MessageBox.Show("계좌번호 또는 비밀번호를 입력해주세요.");
            }else{
                mainForm.xing_CSPAQ12300.call_request(this, listBox_account, input_accountPw);
            }

        }
    }
}
